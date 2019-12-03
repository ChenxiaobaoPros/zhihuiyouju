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
    2001	PB_BERR_PORT_OPEN_ERROR	端口打开失败
    2002	PB_BERR_DEVICE_INIT_ERROR	设备初始化失败
    2003	PB_BERR_NOT_SUPPORTED 	设备不支持此功能
    2004	PB_BERR_NOT_INITIALIZED	设备未初始化
    2005	PB_BERR_COMMUNICATION_ERROR 	通讯错误
    2006	PB_BERR_BUSY	设备忙，无法响应
    2049	PB_BERR_INTERNAL_ERROR	厂商内部错误
    2051	PB_BERR_INVALID_CONFIG_XML	设备配置参数错误：Xml格式非法或不符合规范定义
    2052	PB_BERR_INVALID_COUNTER_NO	柜台号错误
    2053	PB_BERR_INVALID_COMMAND_XML	显示命令错误：Xml格式非法或不符合规范定义
    */
    public class BarScreen : IWriter
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int BERR_Initialize(string lpszConfigXml);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int BERR_Show(string lpszCounterNo, string lpszCommandXml);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int BERR_GetStatus(out int lpStatus);

        private static readonly ILog log = LogManager.GetLogger("screen");
        private IntPtr ptr;
        private BERR_Initialize berrInitialize;
        private BERR_Show berrShow;
        private BERR_GetStatus berrGetstatus;

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

        public BarScreen()
        {
            this.cancelled = false;
            this.dll = Config.App.Peripheral["barScreen"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["barScreen"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["barScreen"].Value<bool>("enabled");
            this.logLevel = Config.App.Peripheral["barScreen"].Value<int>("logLevel");

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

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "BERR_Initialize");
            berrInitialize = (BERR_Initialize)Marshal.GetDelegateForFunctionPointer(api, typeof(BERR_Initialize));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = BERR_Initialize", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "BERR_Show");
            berrShow = (BERR_Show)Marshal.GetDelegateForFunctionPointer(api, typeof(BERR_Show));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = BERR_Show", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "BERR_GetStatus");
            berrGetstatus = (BERR_GetStatus)Marshal.GetDelegateForFunctionPointer(api, typeof(BERR_GetStatus));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = BERR_GetStatus", ptr);

            string xml = "<Device><DeviceId>BERR001</DeviceId><LogLevel>" + logLevel  + "</LogLevel></Device>";
            int code = berrInitialize(xml);
            log.InfoFormat("invoke {0} -> BERR_Initialize, args: xml = {1}, return = {2}", dll, xml, code);
            log.Debug("end");
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

            string counterNo = jo.Value<string>("counterNo");
            string xml = jo.Value<string>("xml");
            int code = berrShow(counterNo, xml);
            log.InfoFormat("invoke {0} -> BERR_Show, args: counterNo = {1}, xml = {2}, return = {3}", dll, counterNo, xml, code);
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
                int code = berrGetstatus(out status);
                log.InfoFormat("invoke {0} -> BERR_GetStatus, args: status = {1}, return = {2}", dll, status, code);

                if (0 == status)
                {
                    s = StatusCode.Normal;
                }
                else if (2003 == status)
                {
                    s = StatusCode.NotSupport;
                }
                else if (2006 == status)
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

            cancelled = true;

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
    }
}
