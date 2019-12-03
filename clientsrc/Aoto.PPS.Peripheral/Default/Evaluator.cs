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
    /*
    4001	PB_APPR_PORT_OPEN_ERROR	端口打开失败
    4002	PB_APPR_DEVICE_INIT_ERROR	设备初始化失败
    4003	PB_APPR_NOT_SUPPORTED 	设备不支持此功能
    4004	PB_APPR_NOT_INITIALIZED	设备未初始化
    4005	PB_APPR_COMMUNICATION_ERROR 	通讯错误
    4006	PB_APPR_BUSY	设备忙，无法响应
    4011	PB_APPR_INVALID_OPERATION	非法操作：设备当前状态不允许此操作
    4049	PB_APPR_INTERNAL_ERROR	厂商内部错误
    4051	PB_APPR_INVALID_CONFIG_XML	设备配置参数错误：Xml格式非法或不符合规范定义
    4052	PB_APPR_INVALID_COUNTER_NO	柜台号错误
    4053	PB_APPR_INVALID_USER_XML	柜员信息错误：Xml格式非法或不符合规范定义
    */
    public class Evaluator : IEvaluator
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int APPR_Initialize(string lpszConfigXml);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int APPR_BeginRead(string lpszCounterNo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int APPR_DataAvailable(string lpszCounterNo, out int lpAvailable);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int APPR_ReadData(string lpszCounterNo, out int lpAppraisal);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int APPR_CancelRead(string lpszCounterNo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int APPR_PlaySound(string lpszCounterNo, int dwSoundIndex);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int APPR_GetStatus(out int lpStatus);

        private static readonly ILog log = LogManager.GetLogger("evaluator");
        private IntPtr ptr;
        private APPR_Initialize apprInitialize;
        private APPR_BeginRead apprBeginRead;
        private APPR_DataAvailable apprDataAvailable;
        private APPR_ReadData apprReadData;
        private APPR_CancelRead apprCancelRead;
        private APPR_GetStatus apprGetStatus;
        private APPR_PlaySound apprPlaySound;

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

        public Evaluator()
        {
            this.cancelled = false;
            this.dll = Config.App.Peripheral["evaluator"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["evaluator"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["evaluator"].Value<bool>("enabled");
            this.logLevel = Config.App.Peripheral["evaluator"].Value<int>("logLevel");

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

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "APPR_Initialize");
            apprInitialize = (APPR_Initialize)Marshal.GetDelegateForFunctionPointer(api, typeof(APPR_Initialize));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = APPR_Initialize", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "APPR_BeginRead");
            apprBeginRead = (APPR_BeginRead)Marshal.GetDelegateForFunctionPointer(api, typeof(APPR_BeginRead));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = APPR_BeginRead", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "APPR_DataAvailable");
            apprDataAvailable = (APPR_DataAvailable)Marshal.GetDelegateForFunctionPointer(api, typeof(APPR_DataAvailable));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = APPR_DataAvailable", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "APPR_ReadData");
            apprReadData = (APPR_ReadData)Marshal.GetDelegateForFunctionPointer(api, typeof(APPR_ReadData));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = APPR_ReadData", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "APPR_CancelRead");
            apprCancelRead = (APPR_CancelRead)Marshal.GetDelegateForFunctionPointer(api, typeof(APPR_CancelRead));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = APPR_CancelRead", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "APPR_GetStatus");
            apprGetStatus = (APPR_GetStatus)Marshal.GetDelegateForFunctionPointer(api, typeof(APPR_GetStatus));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = APPR_GetStatus", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "APPR_PlaySound");
            apprPlaySound = (APPR_PlaySound)Marshal.GetDelegateForFunctionPointer(api, typeof(APPR_PlaySound));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = APPR_PlaySound", ptr);

            string xml = "<Device><DeviceId>CALL001</DeviceId><LogLevel>" + logLevel + "</LogLevel></Device>";
            int code = apprInitialize(xml);
            log.InfoFormat("invoke {0} -> APPR_Initialize, args: xml = {1}, return = {2}", dll, xml, code);
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
                log.Debug("end, cancelled");
                return;
            }

            int t = jo.Value<int>("timeout");
            timeout = t > 0 ? t : timeout;
            log.DebugFormat("timeout = {0}", timeout);

            string counterNo = jo.Value<string>("counterNo");
            int code = apprBeginRead(counterNo);
            log.DebugFormat("invoke {0} -> APPR_BeginRead, args: return = {1}", dll, code);

            if (0 != code)
            {
                code = apprCancelRead(counterNo);
                log.DebugFormat("invoke {0} -> APPR_CancelRead, return = {1}", dll, code);
                jo["result"] = ErrorCode.Offline;
                log.DebugFormat("end, args: jo = {0}", jo);
                return;
            }

            int available = 0;
            int rate = -1;
            long start = DateTime.Now.Ticks;

            while (true)
            {
                code = apprDataAvailable(counterNo, out available);
                log.DebugFormat("invoke {0} -> APPR_DataAvailable, args: counterNo = {1}, available = {2}, return = {3}", dll, counterNo, available, code);

                if (0 == code && available > 0)
                {
                    code = apprReadData(counterNo, out rate);
                    log.InfoFormat("invoke {0} -> APPR_ReadData, args: counterNo = {1}, rate = {2}, return = {3}", dll, counterNo, rate, code);
                    break;
                }

                Thread.Sleep(100);
            }

            code = apprCancelRead(counterNo);
            log.InfoFormat("invoke {0} -> APPR_CancelRead, args: counterNo = {1}, return = {2}", dll, counterNo, code);

            jo["rate"] = rate;
            jo["result"] = ErrorCode.Success;
            log.DebugFormat("end, args: jo = {0}", jo);
        }

        public void Write(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            if ("PlaySound" == jo.Value<string>("method"))
            {
                string counterNo = jo.Value<string>("counterNo");
                int soundIndex = jo.Value<int>("soundIndex");
                int code = apprPlaySound(counterNo, soundIndex);
                log.InfoFormat("invoke {0} -> APPR_PlaySound, counterNo = {1}, soundIndex = {2}, return = {3}", dll, counterNo, soundIndex, code);
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
                int code = apprGetStatus(out status);
                log.InfoFormat("invoke {0} -> APPR_GetStatus, args: status = {1}, return = {2}", dll, status, code);

                if (0 == status)
                {
                    s = StatusCode.Normal;
                }
                else if (4003 == status)
                {
                    s = StatusCode.NotSupport;
                }
                else if (4006 == status)
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
                RunCompletedEvent(this, new RunCompletedEventArgs(jo));
            }
        }
    }
}
