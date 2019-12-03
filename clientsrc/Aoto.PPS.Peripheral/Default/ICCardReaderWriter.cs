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
    public class ICCardReaderWriter : IReader, IWriter
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int ICCR_Initialize(string lpszConfigXml);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int ICCR_BeginRead();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int ICCR_DataAvailable(out int lpAvailable);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int ICCR_ReadData(int dwIndex, byte[] lpBuffer, int dwBufferSize, out int lpNumberOfBytesRead);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int ICCR_CancelRead();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int ICCR_GetStatus(out int lpStatus);

        private static readonly ILog log = LogManager.GetLogger("ic");
        private IntPtr ptr;
        private ICCR_Initialize iccrInitialize;
        private ICCR_BeginRead iccrBeginRead;
        private ICCR_DataAvailable iccrDataAvailable;
        private ICCR_ReadData iccrReadData;
        private ICCR_CancelRead iccrCancelRead;
        private ICCR_GetStatus iccrGetstatus;

        private RunAsyncCaller readAsyncCaller;
        private RunAsyncCaller writeAsyncCaller;
        private IScriptInvoker scriptInvoker;

        private string dll;
        private int timeout;
        private bool enabled;
        private bool cancelled;
        private int logLevel;
        private bool isBusy;
        private int index;

        public bool IsBusy { get { return isBusy; } }
        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public ICCardReaderWriter()
        {
            this.cancelled = false;
            this.dll = Config.App.Peripheral["icCardReaderWriter"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["icCardReaderWriter"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["icCardReaderWriter"].Value<bool>("enabled");
            this.logLevel = Config.App.Peripheral["icCardReaderWriter"].Value<int>("logLevel");
            this.index = Config.App.Peripheral["icCardReaderWriter"].Value<int>("index");

            scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");

            readAsyncCaller = new RunAsyncCaller(Read);
            writeAsyncCaller = new RunAsyncCaller(Write);

            Initialize();
        }

        public void Initialize()
        {
            log.Debug("begin");

            if (!enabled)
            {
                return;
            }

            string dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, dll);

            if (!File.Exists(dllPath))
            {
                dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "lib", dll);
            }

            ptr = Win32ApiInvoker.LoadLibrary(dllPath);
            log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, ptr);

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "ICCR_Initialize");
            iccrInitialize = (ICCR_Initialize)Marshal.GetDelegateForFunctionPointer(api, typeof(ICCR_Initialize));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = ICCR_Initialize", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "ICCR_BeginRead");
            iccrBeginRead = (ICCR_BeginRead)Marshal.GetDelegateForFunctionPointer(api, typeof(ICCR_BeginRead));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = ICCR_BeginRead", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "ICCR_DataAvailable");
            iccrDataAvailable = (ICCR_DataAvailable)Marshal.GetDelegateForFunctionPointer(api, typeof(ICCR_DataAvailable));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = ICCR_DataAvailable", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "ICCR_ReadData");
            iccrReadData = (ICCR_ReadData)Marshal.GetDelegateForFunctionPointer(api, typeof(ICCR_ReadData));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = ICCR_ReadData", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "ICCR_CancelRead");
            iccrCancelRead = (ICCR_CancelRead)Marshal.GetDelegateForFunctionPointer(api, typeof(ICCR_CancelRead));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = ICCR_CancelRead", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "ICCR_GetStatus");
            iccrGetstatus = (ICCR_GetStatus)Marshal.GetDelegateForFunctionPointer(api, typeof(ICCR_GetStatus));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = ICCR_GetStatus", ptr);

            string xml = "<Device><DeviceId>ICCR001</DeviceId><LogLevel>" + logLevel  + "</LogLevel></Device>";
            int code = iccrInitialize(xml);
            log.InfoFormat("invoke {0} -> ICCR_Initialize, args: xml = {1}, return = {2}", dll, xml, code);
            log.Debug("end");
        }

        public void ReadAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            if (!enabled)
            {
                return;
            }

            if (isBusy)
            {
                jo["result"] = ErrorCode.Busy;
                log.InfoFormat("end, isBusy = {0}", isBusy);
                return;
            }

            isBusy = true;
            readAsyncCaller.BeginInvoke(jo, new AsyncCallback(ReadCallback), jo);
            log.Debug("end");
        }

        public void WriteAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);
            writeAsyncCaller.BeginInvoke(jo, new AsyncCallback(WriteCallback), jo);
            log.Debug("end");
        }

        public void Read(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            if (cancelled)
            {
                jo["result"] = ErrorCode.Cancelled;
                log.Info("end, cancelled");
                return;
            }

            int t = jo.Value<int>("timeout");
            t = t > 0 ? t : timeout;
            log.DebugFormat("t = {0}", t);

            int code = iccrBeginRead();
            log.DebugFormat("invoke {0} -> ICCR_BeginRead, args: return = {1}", dll, code);

            if (0 != code)
            {
                code = iccrCancelRead();
                log.DebugFormat("invoke {0} -> ICCR_CancelRead, return = {1}", dll, code);
                jo["result"] = ErrorCode.Offline;
                log.DebugFormat("end, args: jo = {0}", jo);
                return;
            }

            int result = ErrorCode.Failure;
            long start = DateTime.Now.Ticks;
            int available = 0;
            int readCount = 0;
            byte[] buffer = null;

            while (true)
            {
                if (cancelled)
                {
                    result = ErrorCode.Cancelled;
                    log.Info("cancelled");
                    break;
                }

                if (TimeSpan.FromTicks(DateTime.Now.Ticks - start).TotalMilliseconds >= t)
                {
                    result = ErrorCode.Timeout;
                    log.Info("timeout");
                    break;
                }

                code = iccrDataAvailable(out available);
                log.DebugFormat("invoke {0} -> ICCR_DataAvailable, args: available = {1}, return = {2}", dll, available, code);

                if (0 == code && available > 0)
                {
                    buffer = new byte[128];
                    code = iccrReadData(index, buffer, buffer.Length, out readCount);
                    log.InfoFormat("invoke {0} -> ICCR_ReadData, args: readCount = {1}, return = {2}", dll, readCount, code);
                    break;
                }

                Thread.Sleep(300);
            }

            code = iccrCancelRead();
            log.InfoFormat("invoke {0} -> ICCR_CancelRead, return = {1}", dll, code);

            if (readCount > 0)
            {
                string cardNo = Encoding.GetEncoding("gbk").GetString(buffer, 0, readCount);
                log.InfoFormat("cardNo = {0}", cardNo);

                jo["cardNo"] = cardNo;
                result = ErrorCode.Success;
            }

            jo["result"] = result;

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        public void Write(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            log.DebugFormat("end, args: jo = {0}", jo);
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
            int s = StatusCode.Offline;

            if (enabled)
            {
                int status = 0;
                int code = iccrGetstatus(out status);
                log.InfoFormat("invoke {0} -> ICCR_GetStatus, args: status = {1}, return = {2}", dll, status, code);

                if (0 == status)
                {
                    s = StatusCode.Normal;
                }
                else if (9003 == status)
                {
                    s = StatusCode.NotSupport;
                }
                else if (9006 == status)
                {
                    s = StatusCode.Busy;
                }
                else
                {
                    s = StatusCode.Offline;
                }
            }
            else
            {
                s = StatusCode.Disabled;
            }

            log.DebugFormat("end, return = {0}", s);
            return s;
        }

        public void Dispose()
        {
            log.Debug("begin");

            if (!enabled)
            {
                return;
            }

            if (IntPtr.Zero != ptr)
            {
                Win32ApiInvoker.FreeLibrary(ptr);
                log.InfoFormat("FreeLibrary: ptr = {0}", ptr);
            }

            log.Debug("end");
        }

        private void WriteCallback(IAsyncResult ar)
        {

        }

        private void ReadCallback(IAsyncResult ar)
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
                int t = jo.Value<int>("timeout");

                if (864000000 == t)
                {
                    if (ErrorCode.Offline != jo.Value<int>("result"))
                    {
                        scriptInvoker.ScriptInvoke(jo);
                    }

                    if (!cancelled)
                    {
                        Thread.Sleep(200);

                        JObject joo = new JObject();
                        joo["objId"] = jo["objId"];
                        joo["callback"] = jo["callback"];
                        joo["type"] = jo["type"];
                        joo["timeout"] = t;
                        ReadAsync(joo);
                    }
                }
                else
                {
                    RunCompletedEvent(this, new RunCompletedEventArgs(jo));
                }
            }
        }
    }
}
