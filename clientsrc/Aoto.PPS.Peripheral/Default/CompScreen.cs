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
    1001	PB_COMP_PORT_OPEN_ERROR	端口打开失败
    1002	PB_COMP_DEVICE_INIT_ERROR 	设备初始化失败
    1003	PB_COMP_NOT_SUPPORTED	设备不支持此功能
    1004	PB_COMP_NOT_INITIALIZED	设备未初始化
    1005	PB_COMP_COMMUNICATION_ERROR 	通讯错误
    1006	PB_COMP_BUSY	设备忙，无法响应
    1049	PB_COMP_INTERNAL_ERROR	厂商内部错误
    1051	PB_COMP_INVALID_CONFIG_XML	设备配置参数错误：Xml格式非法或不符合规范定义
    1052	PB_COMP_INVALID_ADDRESS	设备地址错误
    1053	PB_COMP_INVALID_COMMAND_XML	显示命令错误：Xml格式非法或不符合规范定义
    */
    public class CompScreen : IWriter
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int COMP_Initialize(string lpszConfigXml);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int COMP_Show(string lpszAddress, string lpszCommandXml);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int COMP_GetStatus(out int lpStatus);

        private static readonly ILog log = LogManager.GetLogger("screen");
        private IntPtr ptr;
        private COMP_Initialize compInitialize;
        private COMP_Show compShow;
        private COMP_GetStatus compGetstatus;

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

        public CompScreen()
        {
            this.cancelled = false;
            this.dll = Config.App.Peripheral["compScreen"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["compScreen"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["compScreen"].Value<bool>("enabled");
            this.logLevel = Config.App.Peripheral["compScreen"].Value<int>("logLevel");

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

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "COMP_Initialize");
            compInitialize = (COMP_Initialize)Marshal.GetDelegateForFunctionPointer(api, typeof(COMP_Initialize));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = COMP_Initialize", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "COMP_Show");
            compShow = (COMP_Show)Marshal.GetDelegateForFunctionPointer(api, typeof(COMP_Show));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = COMP_Show", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "COMP_GetStatus");
            compGetstatus = (COMP_GetStatus)Marshal.GetDelegateForFunctionPointer(api, typeof(COMP_GetStatus));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = COMP_GetStatus", ptr);

            string xml = "<Device><DeviceId>COMP001</DeviceId><LogLevel>" + logLevel  + "</LogLevel></Device>";
            int code = compInitialize(xml);
            log.InfoFormat("invoke {0} -> COMP_Initialize, args: xml = {1}, return = {2}", dll, xml, code);
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
            string address = jo.Value<string>("address");
            string xml = jo.Value<string>("xml");
            int code = compShow(address, xml);
            log.InfoFormat("invoke {0} -> COMP_Show, args: address = {1}, xml = {2}, return = {3}", dll, address, xml, code);
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
                int code = compGetstatus(out status);
                log.InfoFormat("invoke {0} -> COMP_GetStatus, args: status = {1}, return = {2}", dll, status, code);

                if (0 == status)
                {
                    s = StatusCode.Normal;
                }
                else if (1003 == status)
                {
                    s = StatusCode.NotSupport;
                }
                else if (1006 == status)
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
