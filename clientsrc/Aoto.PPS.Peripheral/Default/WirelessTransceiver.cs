using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;
using log4net;
using Newtonsoft.Json.Linq;
using System.Runtime.Remoting.Messaging;

namespace Aoto.PPS.Peripheral.Default
{
    public class WirelessTransceiver : IWirelessTransceiver
    {
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int InitDevice(string addr, int logLevel, StringBuilder companyName, StringBuilder hardwareVersion);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int ReadData(ref int counterNo, StringBuilder mode, StringBuilder ext);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int WriteData(int deviceType, int deviceNo, string text, string ext);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int GetDeviceStatus(int deviceType, int counterNo, int timeout);  

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int OpenDevice();

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int CloseDevice();

        private static readonly ILog log = LogManager.GetLogger("wir"); 
        private IntPtr ptr;
        private InitDevice initDevice;
        private OpenDevice openDevice;
        private CloseDevice closeDevice;
        private GetDeviceStatus getDeviceStatus;
        private ReadData readData;
        private WriteData writeData;

        private string addr;
        private int logLevel;
        private string dll;
        private int timeout;
        private bool enabled;
        private bool cancelled;
        private bool isBusy;
        private RunAsyncCaller readAsyncCaller;
        private RunAsyncCaller writeAsyncCaller;

        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public WirelessTransceiver(string addr, int logLevel, string dll, int timeout, bool enabled)
        {
            this.addr = addr;
            this.logLevel = logLevel;
            this.dll = dll;
            this.timeout = timeout;
            this.enabled = enabled;

            cancelled = false;
            isBusy = false;
            readAsyncCaller = new RunAsyncCaller(Read);
            writeAsyncCaller = new RunAsyncCaller(Write);

            Initialize();
        }

        public void Initialize()
        {
            log.DebugFormat("begin");

            string dllPath = Path.Combine(Config.AppRoot, dll);
            ptr = Win32ApiInvoker.LoadLibrary(dllPath);

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "InitDevice");
            initDevice = (InitDevice)Marshal.GetDelegateForFunctionPointer(api, typeof(InitDevice));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = InitDevice", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "ReadData");
            readData = (ReadData)Marshal.GetDelegateForFunctionPointer(api, typeof(ReadData));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = ReadData", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "GetStatus");
            getDeviceStatus = (GetDeviceStatus)Marshal.GetDelegateForFunctionPointer(api, typeof(GetDeviceStatus));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = GetStatus", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "OpenDevice");
            openDevice = (OpenDevice)Marshal.GetDelegateForFunctionPointer(api, typeof(OpenDevice));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = OpenDevice", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CloseDevice");
            closeDevice = (CloseDevice)Marshal.GetDelegateForFunctionPointer(api, typeof(CloseDevice));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = CloseDevice", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "WriteData");
            writeData = (WriteData)Marshal.GetDelegateForFunctionPointer(api, typeof(WriteData));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = WriteData", ptr);

            StringBuilder sbCompany = new StringBuilder(128);
            StringBuilder sbHardwareVersion = new StringBuilder(128);

            int code = initDevice(addr, logLevel, sbCompany, sbHardwareVersion);
            log.DebugFormat("invoke {0} -> InitDevice, args: addr = {1}, logLevel = {2}, company = {3}, hardwareVersion = {4}, return = {5}",
                dll, addr, logLevel, sbCompany, sbHardwareVersion, code);

            code = openDevice();
            log.DebugFormat("invoke {0} -> OpenDevice, return = {0}", dll, code);
            log.DebugFormat("end");
        }

        public void ReadAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            isBusy = true;
            cancelled = false;

            readAsyncCaller.BeginInvoke(jo, new AsyncCallback(ReadCallback), jo);

            log.Debug("end");
        }

        public void Read(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            if (cancelled)
            {
                jo["result"] = ErrorCode.Cancelled;
                return;
            }

            StringBuilder sbMode = new StringBuilder(32);
            StringBuilder sbExt = new StringBuilder(128);
            jo["result"] = ErrorCode.Failure;
            int counterNo = 0;
            log.DebugFormat("readData start：" + DateTime.Now.ToLocalTime().ToString());
            int code = readData(ref counterNo, sbMode, sbExt);
            log.DebugFormat("invoke {0} -> ReadData, args: counterNo = {1}, sbMode = {2}, sbExt = {3}, return = {4}", dll, counterNo, sbMode, sbExt, code);
             
            if (0 == code)
            {
                if (counterNo > 0 && sbMode.Length > 0)
                {
                    JObject joTkt = new JObject();
                    joTkt["counterNo"] = counterNo;
                    jo["mode"] = sbMode.ToString().Trim();
                    jo["ext"] = sbExt.ToString().Trim();
                    jo["ticket"] = joTkt;
                    jo["result"] = ErrorCode.Success;

                    RunCompletedEvent(this, new RunCompletedEventArgs(jo));
                }
            }
        }

        public void WriteAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            writeAsyncCaller.BeginInvoke(jo, new AsyncCallback(WriteCallback), jo);

            log.Debug("end");
        }

        public void Write(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            int deviceType = jo.Value<int>("deviceType");
            int deviceNo = jo.Value<int>("deviceNo");
            string text = jo.Value<string>("text");
            string ext = jo.Value<string>("ext");

            int code = writeData(deviceType, deviceNo, text, ext);
            log.DebugFormat("end, invoke {0} -> WriteData, return = {1}", dll, code);
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

            log.Debug("end, return = 0");

            return 0;
        }

        public int GetStatus(int deviceType, int counterNo, int timeout)
        {
            log.DebugFormat("begin, args: deviceType = {0}, counterNo = {1}, timeout = {2}");

            if (!enabled)
            {
                log.DebugFormat("end, return = {0}", StatusCode.Disabled);
                return StatusCode.Disabled;
            }

            int code = getDeviceStatus(deviceType, counterNo, timeout);
            log.DebugFormat("invoke {0} -> GetStatus, return = {1}", dll, code);

            int state = (0 == code) ? 1 : 3;

            log.DebugFormat("end, return = {0}", state);

            return state;
        }

        public void Dispose()
        {
            log.Debug("begin");

            cancelled = true;

            if (IntPtr.Zero != ptr)
            {
                Win32ApiInvoker.FreeLibrary(ptr);
                log.DebugFormat("FreeLibrary: ptr = {0}", ptr);
            }

            log.Debug("end");
        }

        private void WriteCallback(IAsyncResult ar)
        {
            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
            }
            catch (Exception e)
            {
                log.Error("Error", e);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
            }
            catch (Exception e)
            {
                log.Error("Error", e);
            }

            isBusy = false;

            if (!cancelled)
            {
                Thread.Sleep(200);
                ReadAsync(new JObject());
            }
        }
    }
}
