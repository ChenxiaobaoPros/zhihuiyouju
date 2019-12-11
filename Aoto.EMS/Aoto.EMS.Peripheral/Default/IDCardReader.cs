using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using Aoto.EMS.Infrastructure.ComponentModel;
using Aoto.EMS.Peripheral;
using Aoto.EMS.Infrastructure;
using Aoto.EMS.Infrastructure.Configuration;
using log4net;
using Newtonsoft.Json.Linq;

namespace Aoto.EMS.Peripheral
{
    /*
    3001	PB_IDCR_PORT_OPEN_ERROR	端口打开失败
    3002	PB_IDCR_DEVICE_INIT_ERROR	设备初始化失败
    3003	PB_IDCR_NOT_SUPPORTED 	设备不支持此功能
    3004	PB_IDCR_NOT_INITIALIZED	设备未初始化
    3005	PB_IDCR_COMMUNICATION_ERROR 	通讯错误
    3006	PB_IDCR_BUSY	设备忙，无法响应
    3011	PB_IDCR_INVALID_OPERATION	非法操作：设备当前状态不允许此操作
    3049	PB_IDCR_INTERNAL_ERROR	厂商内部错误
    3051	PB_IDCR_INVALID_CONFIG_XML	设备配置参数错误：Xml格式非法或不符合规范定义
    3052	PB_IDCR_INVALID_INDEX	索引无效
    3053	PB_IDCR_INSUFFICIENT_BUFFER	缓冲区过小
     */
    public class IDCardReader : IReader
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int IDCR_Initialize(string lpszConfigXml);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int IDCR_BeginRead();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int IDCR_DataAvailable(out int lpAvailable);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int IDCR_ReadData(int dwIndex, byte[] lpBuffer, int dwBufferSize, out int lpNumberOfBytesRead);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int IDCR_CancelRead();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int IDCR_GetStatus(out int lpStatus);

        private static readonly ILog log = LogManager.GetLogger("id");
        private IntPtr ptr;
        private IDCR_Initialize idcrInitialize;
        private IDCR_BeginRead idcrBeginRead;
        private IDCR_DataAvailable idcrDataAvailable;
        private IDCR_ReadData idcrReadData;
        private IDCR_CancelRead idcrCancelRead;
        private IDCR_GetStatus idcrGetstatus;

        private RunAsyncCaller readAsyncCaller;
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

        public IDCardReader()
        {
            this.cancelled = false;
            this.dll = Config.App.Peripheral["idCardReader"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["idCardReader"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["idCardReader"].Value<bool>("enabled");
            this.logLevel = Config.App.Peripheral["idCardReader"].Value<int>("logLevel");
            this.index = Config.App.Peripheral["idCardReader"].Value<int>("index");

            //scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
            readAsyncCaller = new RunAsyncCaller(Read);

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
                dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "IDCardlib", dll);
            }

            ptr = Win32ApiInvoker.LoadLibrary(dllPath);
            log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, ptr);

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "IDCR_Initialize");
            idcrInitialize = (IDCR_Initialize)Marshal.GetDelegateForFunctionPointer(api, typeof(IDCR_Initialize));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = IDCR_Initialize", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "IDCR_BeginRead");
            idcrBeginRead = (IDCR_BeginRead)Marshal.GetDelegateForFunctionPointer(api, typeof(IDCR_BeginRead));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = IDCR_BeginRead", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "IDCR_DataAvailable");
            idcrDataAvailable = (IDCR_DataAvailable)Marshal.GetDelegateForFunctionPointer(api, typeof(IDCR_DataAvailable));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = IDCR_DataAvailable", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "IDCR_ReadData");
            idcrReadData = (IDCR_ReadData)Marshal.GetDelegateForFunctionPointer(api, typeof(IDCR_ReadData));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = IDCR_ReadData", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "IDCR_CancelRead");
            idcrCancelRead = (IDCR_CancelRead)Marshal.GetDelegateForFunctionPointer(api, typeof(IDCR_CancelRead));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = IDCR_CancelRead", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "IDCR_GetStatus");
            idcrGetstatus = (IDCR_GetStatus)Marshal.GetDelegateForFunctionPointer(api, typeof(IDCR_GetStatus));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = IDCR_GetStatus", ptr);

            string xml = "<?xml version =\"1.0\" encoding=\"utf-8\"?><Device>  <DeviceId>IDCard001</DeviceId>  <LogLevel>3</LogLevel></Device>";
            int code = idcrInitialize(xml);
            log.InfoFormat("invoke {0} -> IDCR_Initialize, args: xml = {1}, return = {2}", dll, xml, code);
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

            int code = idcrBeginRead();
            log.DebugFormat("invoke {0} -> IDCR_BeginRead, args: return = {1}", dll, code);

            if (0 != code)
            {
                code = idcrCancelRead();
                log.DebugFormat("invoke {0} -> IDCR_CancelRead, return = {1}", dll, code);
                jo["result"] = ErrorCode.Offline;
                log.DebugFormat("end, args: jo = {0}", jo);
                return;
            }
            
            int result = ErrorCode.Failure;
            int available = 0;
            int readCount = 0;
            byte[] buffer = null;
            long start = DateTime.Now.Ticks;

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


                code = idcrDataAvailable(out available);
                log.DebugFormat("invoke {0} -> IDCR_DataAvailable, args: available = {1}, return = {2}", dll, available, code);

                if (0 == code && available > 0)
                {
                    buffer = new byte[256];
                    code = idcrReadData(0, buffer, buffer.Length, out readCount);
                    log.InfoFormat("invoke {0} -> IDCR_ReadData, args: readCount = {1}, return = {2}", dll, readCount, code);
                    break;
                }

                Thread.Sleep(300);
            }

            code = idcrCancelRead();
            log.InfoFormat("invoke {0} -> IDCR_CancelRead, return = {1}", dll, code);

            if (readCount > 0)
            {
                string data = Encoding.GetEncoding("gbk").GetString(buffer, 0, readCount);
                log.InfoFormat("data = {0}", data);

                string[] a = data.Split('|');
                jo["certName"] = a[0];
                jo["gender"] = a[1];
                jo["nationality"] = a[2];
                jo["birthday"] = a[3];
                jo["address"] = a[4];
                jo["certNo"] = a[5];
                jo["issuedBy"] = a[6];
                jo["expDate"] = a[7] + "-" + a[8];
                jo["certType"] = 1;

                result = ErrorCode.Success;
            }

            jo["result"] = result;
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
                int code = idcrGetstatus(out status);
                log.InfoFormat("invoke {0} -> IDCR_Getstatus, args: status = {1}, return = {2}", dll, status, code);

                if (0 == status)
                {
                    s = StatusCode.Normal;
                }
                else if (3003 == status)
                {
                    s = StatusCode.NotSupport;
                }
                else if (3006 == status)
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
                        joo["tag"] = jo["tag"];
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
