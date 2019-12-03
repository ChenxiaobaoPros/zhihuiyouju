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
    public class Caller : ICaller
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CALL_Initialize(string lpszConfigXml);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CALL_BeginRead();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CALL_DataAvailable(out int lpAvailable);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CALL_ReadData(byte[] lpszCounterNo, int dwBufferSize, out int lpNumberOfBytesRead, byte[] mode, int dwBufferSize2, out int lpNumberOfBytesRead2);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CALL_CancelRead();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CALL_ShowNumber(string lpszCounterNo, string queueNum, string waitNum);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CALL_GetStatus(out int lpStatus);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CALL_SendStatus(string lpszCounterNo, int status);

        private static readonly ILog log = LogManager.GetLogger("caller");
        private IntPtr ptr;
        private CALL_Initialize callInitialize;
        private CALL_BeginRead callBeginRead;
        private CALL_DataAvailable callDataAvailable;
        private CALL_ReadData callReadData;
        private CALL_CancelRead callCancelRead;
        private CALL_GetStatus callGetStatus;
        private CALL_ShowNumber callShowNumber;
        private CALL_SendStatus callSendStatus;

        private RunAsyncCaller readAsyncCaller;
        private RunAsyncCaller writeAsyncCaller;

        private string dll;
        private int timeout;
        private bool enabled;
        private bool cancelled;
        private int logLevel;
        private bool isBusy;

        public bool IsBusy { get { return isBusy; } }
        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public Caller()
        {
            this.cancelled = false;
            this.dll = Config.App.Peripheral["caller"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["caller"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["caller"].Value<bool>("enabled");
            this.logLevel = Config.App.Peripheral["caller"].Value<int>("logLevel");

            readAsyncCaller = new RunAsyncCaller(Read);
            writeAsyncCaller = new RunAsyncCaller(Write);
 
            Initialize();
        }

        public void Initialize()
        {
            log.Debug("begin");

            string dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, dll);

            if (!File.Exists(dllPath))
            {
                dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "lib", dll);
            }

            ptr = Win32ApiInvoker.LoadLibrary(dllPath);
            log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, ptr);

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "CALL_Initialize");
            callInitialize = (CALL_Initialize)Marshal.GetDelegateForFunctionPointer(api, typeof(CALL_Initialize));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CALL_Initialize", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CALL_BeginRead");
            callBeginRead = (CALL_BeginRead)Marshal.GetDelegateForFunctionPointer(api, typeof(CALL_BeginRead));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CALL_BeginRead", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CALL_DataAvailable");
            callDataAvailable = (CALL_DataAvailable)Marshal.GetDelegateForFunctionPointer(api, typeof(CALL_DataAvailable));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CALL_DataAvailable", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CALL_ReadData");
            callReadData = (CALL_ReadData)Marshal.GetDelegateForFunctionPointer(api, typeof(CALL_ReadData));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CALL_ReadData", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CALL_CancelRead");
            callCancelRead = (CALL_CancelRead)Marshal.GetDelegateForFunctionPointer(api, typeof(CALL_CancelRead));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CALL_CancelRead", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CALL_GetStatus");
            callGetStatus = (CALL_GetStatus)Marshal.GetDelegateForFunctionPointer(api, typeof(CALL_GetStatus));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CALL_GetStatus", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CALL_ShowNumber");
            callShowNumber = (CALL_ShowNumber)Marshal.GetDelegateForFunctionPointer(api, typeof(CALL_ShowNumber));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CALL_ShowNumber", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CALL_SendStatus");
            callSendStatus = (CALL_SendStatus)Marshal.GetDelegateForFunctionPointer(api, typeof(CALL_SendStatus));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CALL_SendStatus", ptr);

            string xml = "<Device><DeviceId>CALL001</DeviceId><LogLevel>" + logLevel + "</LogLevel></Device>";
            int code = callInitialize(xml);
            log.InfoFormat("invoke {0} -> CALL_Initialize, args: xml = {1}, return = {2}", dll, xml, code);
            log.Debug("end");
        }

        public void ReadAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

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

        public void Read(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            if (cancelled)
            {
                jo["result"] = ErrorCode.Cancelled;
                log.Info("end, cancelled");
                return;
            }

            int code = callBeginRead();
            log.DebugFormat("invoke {0} -> CALL_BeginRead, args: return = {1}", dll, code);

            if (0 != code)
            {
                code = callCancelRead();
                log.DebugFormat("invoke {0} -> CALL_CancelRead, return = {1}", dll, code);
                jo["result"] = ErrorCode.Offline;
                log.DebugFormat("end, args: jo = {0}", jo);
                return;
            }

            int available = 0;
            int readCount1 = 0;
            int readCount2 = 0;
            byte[] buffer1 = null;
            byte[] buffer2 = null;
            long start = DateTime.Now.Ticks;

            while (true)
            {
                if (cancelled)
                {
                    log.Info("cancelled");
                    break;
                }

                code = callDataAvailable(out available);
                log.DebugFormat("invoke {0} -> CALL_DataAvailable, args: available = {1}, return = {2}", dll, available, code);

                if (0 == code && available > 0)
                {
                    buffer1 = new byte[4];
                    buffer2 = new byte[128];
                    code = callReadData(buffer1, buffer1.Length, out readCount1, buffer2, buffer2.Length, out readCount2);
                    log.InfoFormat("invoke {0} -> CALL_ReadData, args: readCount1 = {1}, readCount2 = {2}, return = {3}", dll, readCount1, readCount2, code);
                    break;
                }

                Thread.Sleep(100);
            }

            code = callCancelRead();
            log.InfoFormat("invoke {0} -> CALL_CancelRead, return = {1}", dll, code);

            if (readCount1 > 0 && readCount2 > 0)
            {
                string counterNo = Encoding.UTF8.GetString(buffer1, 0, readCount1);
                string mode = Encoding.UTF8.GetString(buffer2, 0, readCount2);

                log.InfoFormat("counterNo = {0}, mode = {1}", counterNo, mode);

                JObject joTkt = new JObject();
                joTkt["counterNo"] = Convert.ToInt32(counterNo);
                int index = mode.IndexOf("|");

                if (index > 0)
                {
                    jo["mode"] = mode.Substring(0, index);
                    jo["ext"] = mode.Substring(index + 1);
                }
                else
                {
                    jo["mode"] = mode;
                }

                jo["ticket"] = joTkt;
                jo["result"] = ErrorCode.Success;
            }

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        public void Write(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string method = jo.Value<string>("method");
            int code = 0;
            string counterNo = jo.Value<string>("counterNo");

            if ("ShowNumber" == method)
            {
                string ticketNo = jo.Value<string>("ticketNo");
                string buzWaitingCount = jo.Value<string>("buzWaitingCount");

                ticketNo = String.IsNullOrEmpty(ticketNo) ? "0000" : ticketNo;
                buzWaitingCount = String.IsNullOrEmpty(buzWaitingCount) ? "0" : buzWaitingCount;

                code = callShowNumber(counterNo, ticketNo, buzWaitingCount);
                log.InfoFormat("invoke {0} -> CALL_ShowNumber, args: counterNo = {1}, ticketNo = {2}, buzWaitingCount = {3}, return = {4}", dll, counterNo, ticketNo, buzWaitingCount, code);
            }
            else if ("SendStatus" == method)
            {
                int status = jo.Value<int>("status");
                code = callSendStatus(counterNo, status);
                log.InfoFormat("invoke {0} -> CALL_SendStatus, counterNo = {1}, status = {2}, return = {3}", dll, counterNo, status, code);
            }
            
            log.Debug("end");
        }

        public void WriteAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);
            writeAsyncCaller.BeginInvoke(jo, new AsyncCallback(WriteCallback), jo);
            log.Debug("end");
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
                int code = callGetStatus(out status);
                log.InfoFormat("invoke {0} -> CALL_GetStatus, args: status = {1}, return = {2}", dll, status, code);

                if (0 == status)
                {
                    s = StatusCode.Normal;
                }
                else if (8003 == status)
                {
                    s = StatusCode.NotSupport;
                }
                else if (8006 == status)
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

            if (IntPtr.Zero != ptr)
            {
                Win32ApiInvoker.FreeLibrary(ptr);
                log.InfoFormat("FreeLibrary: ptr = {0}", ptr);
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

                if (null != RunCompletedEvent)
                {
                    RunCompletedEvent(this, new RunCompletedEventArgs(jo));
                }

                if (!cancelled)
                {
                    ReadAsync(new JObject());
                }
            }
        }
    }
}
