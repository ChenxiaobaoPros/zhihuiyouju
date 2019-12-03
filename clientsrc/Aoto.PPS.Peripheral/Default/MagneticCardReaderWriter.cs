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
    5001	PB_CRDR_PORT_OPEN_ERROR	端口打开失败
    5002	PB_CRDR_DEVICE_INIT_ERROR	设备初始化失败
    5003	PB_CRDR_NOT_SUPPORTED 	设备不支持此功能
    5004	PB_CRDR_NOT_INITIALIZED	设备未初始化
    5005	PB_CRDR_COMMUNICATION_ERROR 	通讯错误
    5006	PB_CRDR_BUSY	设备忙，无法响应
    5011	PB_CRDR_INVALID_OPERATION	非法操作：设备当前状态不允许此操作
    5049	PB_CRDR_INTERNAL_ERROR	厂商内部错误
    5051	PB_CRDR_INVALID_CONFIG_XML	设备配置参数错误：Xml格式非法或不符合规范定义
    5052	PB_CRDR_INSUFFICIENT_BUFFER	缓冲区过小
    */
    public class MagneticCardReaderWriter : IReader, IWriter
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CRDR_Initialize(string lpszConfigXml);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CRDR_BeginRead(int dwTrack);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CRDR_DataAvailable(out int lpAvailable);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CRDR_ReadData(byte[] lpBuffer, int dwBufferSize, out int lpNumberOfBytesRead);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CRDR_CancelRead();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CRDR_GetStatus(out int lpStatus);

        private static readonly ILog log = LogManager.GetLogger("mag");
        private IntPtr ptr;
        private CRDR_Initialize crdrInitialize;
        private CRDR_BeginRead crdrBeginRead;
        private CRDR_DataAvailable crdrDataAvailable;
        private CRDR_ReadData crdrReadData;
        private CRDR_CancelRead crdrCancelRead;
        private CRDR_GetStatus crdrGetstatus;

        private RunAsyncCaller readAsyncCaller;
        private RunAsyncCaller writeAsyncCaller;
        private IScriptInvoker scriptInvoker;

        private string dll;
        private int track;
        private int timeout;
        private bool enabled;
        private bool cancelled;
        private int logLevel;
        private bool isBusy;

        public bool IsBusy { get { return isBusy; } }
        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public MagneticCardReaderWriter()
        {
            this.cancelled = false;
            this.dll = Config.App.Peripheral["magneticCardReaderWriter"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["magneticCardReaderWriter"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["magneticCardReaderWriter"].Value<bool>("enabled");
            this.logLevel = Config.App.Peripheral["magneticCardReaderWriter"].Value<int>("logLevel");
            this.track = Config.App.Peripheral["magneticCardReaderWriter"].Value<int>("track");

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

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "CRDR_Initialize");
            crdrInitialize = (CRDR_Initialize)Marshal.GetDelegateForFunctionPointer(api, typeof(CRDR_Initialize));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CRDR_Initialize", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CRDR_BeginRead");
            crdrBeginRead = (CRDR_BeginRead)Marshal.GetDelegateForFunctionPointer(api, typeof(CRDR_BeginRead));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CRDR_BeginRead", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CRDR_DataAvailable");
            crdrDataAvailable = (CRDR_DataAvailable)Marshal.GetDelegateForFunctionPointer(api, typeof(CRDR_DataAvailable));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CRDR_DataAvailable", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CRDR_ReadData");
            crdrReadData = (CRDR_ReadData)Marshal.GetDelegateForFunctionPointer(api, typeof(CRDR_ReadData));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CRDR_ReadData", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CRDR_CancelRead");
            crdrCancelRead = (CRDR_CancelRead)Marshal.GetDelegateForFunctionPointer(api, typeof(CRDR_CancelRead));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CRDR_CancelRead", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CRDR_GetStatus");
            crdrGetstatus = (CRDR_GetStatus)Marshal.GetDelegateForFunctionPointer(api, typeof(CRDR_GetStatus));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = CRDR_GetStatus", ptr);

            string xml = "<Device><DeviceId>CRDR001</DeviceId><LogLevel>" + logLevel + "</LogLevel></Device>";
            int code = crdrInitialize(xml);
            log.InfoFormat("invoke {0} -> CRDR_Initialize, args: xml = {1}, return = {2}", dll, xml, code);
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

            int code = crdrBeginRead(track);
            log.DebugFormat("invoke {0} -> CRDR_BeginRead, args: track = {1}, return = {2}", dll, track, code);

            if (0 != code)
            {
                code = crdrCancelRead();
                log.DebugFormat("invoke {0} -> CRDR_CancelRead, return = {1}", dll, code);
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

                
                code = crdrDataAvailable(out available);
                log.DebugFormat("invoke {0} -> CRDR_DataAvailable, args: available = {1}, return = {2}", dll, available, code);

                if (0 == code && available > 0)
                {
                    buffer = new byte[256];
                    code = crdrReadData(buffer, buffer.Length, out readCount);
                    log.InfoFormat("invoke {0} -> CRDR_ReadData, args: readCount = {1}, return = {2}", dll, readCount, code);
                    break;
                }

                Thread.Sleep(300);
            }

            code = crdrCancelRead();
            log.InfoFormat("invoke {0} -> ICCR_CancelRead, return = {1}", dll, code);

            if (readCount > 0)
            {
                string data = Encoding.GetEncoding("gbk").GetString(buffer, 0, readCount);
                string cardNo = String.Empty;
                result = GetCardNo(data, out cardNo);
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
                int code = crdrGetstatus(out status);
                log.InfoFormat("invoke {0} -> CRDR_Getstatus, args: status = {1}, return = {2}", dll, status, code);

                if (0 == status)
                {
                    s = StatusCode.Normal;
                }
                else if (5003 == status)
                {
                    s = StatusCode.NotSupport;
                }
                else if (5006 == status)
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

        private int GetCardNo(string data, out string cardNo)
        {
            log.DebugFormat("begin, args: data = {0}", data);
            cardNo = String.Empty;

            if (!String.IsNullOrEmpty(data))
            {
                int index = 0;

                if (23 == track)
                {
                    if (data.Contains("|"))
                    {
                        string[] a = data.Split('|');
                        string track2 = a[0];
                        string track3 = a[1];

                        if (!String.IsNullOrEmpty(track2))
                        {
                            index = track2.IndexOf("=");

                            if (index > 0)
                            {
                                cardNo = track2.Substring(0, index);
                                return ErrorCode.Success;
                            }
                        }

                        if (!String.IsNullOrEmpty(track3))
                        {
                            index = track3.IndexOf("=");

                            if (index > 0)
                            {
                                cardNo = track3.Substring(0, index);
                                cardNo = cardNo.TrimStart("99".ToCharArray());
                                return ErrorCode.Success;
                            }
                        } 
                    }
                }
                else if (2 == track)
                {
                    index = data.IndexOf("=");

                    if (index > 0)
                    {
                        cardNo = data.Substring(0, index);
                        return ErrorCode.Success;
                    }
                }
                else if (3 == track)
                {
                    index = data.IndexOf("=");

                    if (index > 0)
                    {
                        cardNo = data.Substring(0, index);
                        cardNo = cardNo.TrimStart("99".ToCharArray());
                        return ErrorCode.Success;
                    }
                }
            }

            log.DebugFormat("end, args: cardNo = {0}", cardNo);
            return ErrorCode.Failure;
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
