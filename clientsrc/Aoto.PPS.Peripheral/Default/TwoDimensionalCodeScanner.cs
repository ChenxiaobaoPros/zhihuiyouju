using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;
using log4net;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Peripheral.Default
{
    public class TwoDimensionalCodeScanner : IReader
    {
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int InitDevice(StringBuilder companyName, StringBuilder hardwareVersion);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int ReadData(StringBuilder info);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int OpenDevice();

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int CloseDevice();

        private static readonly ILog log = LogManager.GetLogger("peripheral"); 
        private IntPtr ptr;
        private InitDevice initDevice;
        private ReadData readData;
        private OpenDevice openDevice;
        private CloseDevice closeDevice;
        private RunAsyncCaller readAsyncCaller;

        private string dll;
        private int timeout;
        private bool enabled;
        private bool cancelled;
        private bool isBusy;

        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public TwoDimensionalCodeScanner(string dll, int timeout, bool enabled)
        {
            cancelled = false;
            isBusy = false;

            this.dll = dll;
            this.timeout = timeout * 1000;
            this.enabled = enabled;

            readAsyncCaller = new RunAsyncCaller(Read);

            Initialize();
        }

        public void Initialize()
        {
            log.Debug("begin");

            string dllPath = Path.Combine(Config.AppRoot, dll);
            ptr = Win32ApiInvoker.LoadLibrary(dllPath);

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "InitDevice");
            initDevice = (InitDevice)Marshal.GetDelegateForFunctionPointer(api, typeof(InitDevice));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = InitDevice", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "ReadData");
            readData = (ReadData)Marshal.GetDelegateForFunctionPointer(api, typeof(ReadData));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = ReadData", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "OpenDevice");
            openDevice = (OpenDevice)Marshal.GetDelegateForFunctionPointer(api, typeof(OpenDevice));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = OpenDevice", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CloseDevice");
            closeDevice = (CloseDevice)Marshal.GetDelegateForFunctionPointer(api, typeof(CloseDevice));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = CloseDevice", ptr);

            StringBuilder sbCompany = new StringBuilder(128);
            StringBuilder sbHardwareVersion = new StringBuilder(128);

            int code = initDevice(sbCompany, sbHardwareVersion);
            log.DebugFormat("invoke {0} -> InitDevice, args: company = {1}, hardwareVersion = {2}, return = {3}",
                dll, sbCompany, sbHardwareVersion, code);

            log.Debug("end");
        }

        public void ReadAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            if (isBusy)
            {
                jo["result"] = ErrorCode.Busy;
                RunCompletedEvent(this, new RunCompletedEventArgs(jo));
                log.DebugFormat("end, args: jo = {0}", jo);
                return;
            }

            isBusy = true;
            cancelled = false;
            readAsyncCaller.BeginInvoke(jo, new AsyncCallback(Callback), jo);

            log.DebugFormat("end");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 0：禁用；1：正常；2：忙；3：离线；
        /// </returns>
        public int GetStatus()
        {
            log.Debug("begin");

            if (!enabled)
            {
                log.DebugFormat("end, return = {0}", StatusCode.Disabled);
                return StatusCode.Disabled;
            }

            if (isBusy)
            {
                log.DebugFormat("end, return = {0}", StatusCode.Busy);
                return StatusCode.Busy;
            }

            int code = openDevice();
            log.DebugFormat("invoke {0} -> OpenDevice, return = {1}", dll, code);

            int state = (0 == code) ? StatusCode.Normal : StatusCode.Offline;

            code = closeDevice();
            log.DebugFormat("invoke {0} -> CloseDevice, return = {1}", dll, code);
            log.DebugFormat("end, return = {0}", state);
            return state;
        }

        public void Dispose()
        {
            log.DebugFormat("begin");

            if (IntPtr.Zero != ptr)
            {
                Win32ApiInvoker.FreeLibrary(ptr);
                log.DebugFormat("FreeLibrary: ptr = {0}", ptr);
            }

            log.DebugFormat("end");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jo"></param>
        /// <returns>
        /// jc["result"] 0：成功；4：取消；5：超时；其他失败
        /// </returns>
        public void Read(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            int code = openDevice();
            log.DebugFormat("invoke {0} -> OpenDevice, return = {1}", dll, code);

            if (0 != code)
            {
                code = closeDevice();
                log.DebugFormat("invoke {0} -> CloseDevice, return = {1}", dll, code);
                jo["result"] = ErrorCode.Failure;
                log.DebugFormat("end, args: jo = {0}", jo);

                return;
            }

            StringBuilder info = new StringBuilder(4096);

            // app 的返回值，应用包装
            int result = ErrorCode.Failure;
            long start = DateTime.Now.Ticks;
            TimeSpan elapsedSpan = TimeSpan.Zero;
            string cardNo = String.Empty;

            while (true)
            {
                if (cancelled)
                {
                    result = ErrorCode.Cancelled;
                    break;
                }

                if (TimeSpan.FromTicks(DateTime.Now.Ticks - start).TotalMilliseconds >= timeout)
                {
                    result = ErrorCode.Timeout;
                    break;
                }

                code = readData(info);
                log.DebugFormat("invoke {0} -> ReadData, args: info = {1}, return = {2}", dll, info, code);

                if (0 == code)
                {
                    if (info.Length > 0)
                    {
                        jo["info"] = info.ToString().Trim();
                        result = ErrorCode.Success;
                    }

                    break;
                }

                Thread.Sleep(200);
            }

            code = closeDevice();
            log.DebugFormat("invoke {0} -> CloseDevice, return = {1}", dll, code);

            jo["result"] = result;
            log.DebugFormat("end, args: jo = {0}", jo);
        }

        private void Callback(IAsyncResult ar)
        {
            JObject jo = (JObject)ar.AsyncState;

            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
            }
            catch (Exception e)
            {
                jo["result"] = ErrorCode.Failure;
                log.Error("Error", e);
            }
            finally
            {
                isBusy = false;
                RunCompletedEvent(this, new RunCompletedEventArgs(jo));
            }
        }
    }
}
