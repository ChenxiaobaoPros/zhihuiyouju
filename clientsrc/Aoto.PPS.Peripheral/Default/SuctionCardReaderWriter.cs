using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using log4net;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Peripheral.Default
{
    public class SuctionCardReaderWriter : IReader, IWriter
    {
        /*
        //吸卡器
        #define SCR_NOT_INIT_ERR			901		//初始化失败
        #define SCR_OPEN_PORT_ERR			902		//吸卡器打开串口失败

        #define SCR_UNKNOWN_ERR				904		//未知错误
        #define SCR_OPEN_DOOR_ERR			905		//使能进卡失败,开门失败
        #define SCR_CLOSE_PORT_ERR			906		//吸卡器关闭串口失败
        #define SCR_CLOSE_DOOR_ERR			907		//使能禁卡失败,关门失败

        //走卡位
        #define SCR_CARD_IN_FRONT			911		//前端,不持卡
        #define SCR_GET_CARD_IN_FRONT		912		//前端,持卡
        #define SCR_CARD_IN_BACK			913		//后端,不持卡
        #define SCR_GET_CARD_IN_BACK		914		//后端,持卡
        #define SCR_M1CARD_POSITION			915		//将卡重新走位到卡机内位置,操作成功后可进行M1射频卡操作
        #define SCR_ICCARD_POSITION			916		//将卡重新走位到卡机内位置,并将IC卡触点落下,操作成功后可进行接触式IC卡操作

        #define SCR_CARD_IN_READER			918		//卡机内停卡位置有卡
        #define SCR_CARD_IN_ICPOSITION		919		//卡机内IC 卡操作位置有卡,并且IC 卡触电已下落
        #define SCR_NO_CARD_IN_READER		920		//卡机内无卡
        #define SCR_ERR_CARD_STYLE			921		//错误的卡类型,卡太长或者太短

        #define SCR_LOAD_PCOMMDLL_ERR		930		//加载PCOMM动态库错误
        #define SCR_POP_CARD_ERR			934		//吸卡器弹卡失败
        #define SCR_CHECK_CARD_TYPE_ERR		935		//检测卡类型失败
        #define SCR_SET_CARD_POS_ERR		936		//设置停卡位置失败
        #define SCR_UNKNOWN_CARD_TYPE		937		//未知卡类型
        #define SCR_ERR_CARD_POS			938		//卡不在允许操作的位置上

        #define SCR_S50_RFCARD				940		//非接触式S50 射频卡(结算证)
        #define SCR_S70_RFCARD				941		//非接触式S70 射频卡
        #define SCR_UL_RFCARD				942		//非接触式UL 射频卡
        #define SCR_UNKNOWN_RFCARD			943		//非接触式射频卡但未知类型
        #define SCR_TYPEA_CPUCARD			944		//ISO1443 TYPEA CPU 卡
        #define SCR_TYPEB_CPUCARD			945		//ISO1443 TYPEB CPU 卡
        #define SCR_T0_CPUCARD				946		//接触式T=0 的CPU 卡
        #define SCR_T1_CPUCARD				947		//接触式T=1 的CPU 卡

        #define SCR_READ_CARD_ERR			960		//读卡数据失败
        #define SCR_SEND_APDU_ERR			961		//发apdu指令失败
        #define SCR_PARAM_ERR				962		//接口参数错误
        #define SCR_SET_POWER_ERR			963		//上下电操作失败
        #define SCR_SET_MES_ERR				964		//发送数据失败
        #define SCR_GET_MES_ERR				965		//接收数据失败
        #define SCR_RETURN_VALUE_ERR		966		//返回值错误为空
        #define SCR_CPU_RESET_ERR			967		//CPU卡复位失败
         */

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int InitCaller(StringBuilder companyName, StringBuilder hardwareVersion);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int OpenCaller();

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int ReadCaller(int type, string tag, StringBuilder info);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int CloseCaller();

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int PopCaller();

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int GetStateCaller();

        private static readonly ILog log = LogManager.GetLogger("peripheral"); 
        private IntPtr ptr;
        private InitCaller initCaller;
        private OpenCaller openCaller;
        private CloseCaller closeCaller;
        private ReadCaller readCaller;
        private PopCaller popCaller;
        private GetStateCaller getStateCaller;
        private RunAsyncCaller readAsyncCaller;
        private RunAsyncCaller writeAsyncCaller;
        private Dictionary<string, string> tagDic;

        private string dll;
        private int timeout;
        private bool enabled;
        private bool isBusy;
        private bool cancelled;
        private int lightNo;
        private int lightType;

        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public SuctionCardReaderWriter(int lightNo, int lightType, string dll, int timeout, bool enabled)
        {
            isBusy = false;
            cancelled = false;

            this.dll = dll;
            this.timeout = timeout * 1000;
            this.enabled = enabled;
            this.lightNo = lightNo;
            this.lightType = lightType;

            Initialize();
        }

        public void Initialize()
        {
            log.Debug("begin");

            //isBusy = false;
            //cancelled = false;

            //dll = jc.Value<string>("dll");
            //timeout = jc.Value<int>("timeout") * 1000;
            //enabled = jc.Value<bool>("enabled");

            //tagDic = new Dictionary<string, string>();
            //tagDic.Add("A", "cardNo");
            //tagDic.Add("B", "name");
            //tagDic.Add("C", "certType");
            //tagDic.Add("D", "certNo");
            //tagDic.Add("E", "track2");
            //tagDic.Add("F", "track1");
            //tagDic.Add("G", "balance");
            //tagDic.Add("H", "limit");
            //tagDic.Add("I", "expireDate");
            //tagDic.Add("J", "serialNo");

            //readAsyncCaller = new RunAsyncCaller(Read);
            //writeAsyncCaller = new RunAsyncCaller(Write);

            //string dllPath = Path.Combine(Config.AppRoot, dll);
            //ptr = Win32ApiInvoker.LoadLibrary(dllPath);

            //IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "Init");
            //initCaller = (InitCaller)Marshal.GetDelegateForFunctionPointer(api, typeof(InitCaller));
            //log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = Init", ptr);

            //api = Win32ApiInvoker.GetProcAddress(ptr, "Read");
            //readCaller = (ReadCaller)Marshal.GetDelegateForFunctionPointer(api, typeof(ReadCaller));
            //log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = Read", ptr);

            //api = Win32ApiInvoker.GetProcAddress(ptr, "Open");
            //openCaller = (OpenCaller)Marshal.GetDelegateForFunctionPointer(api, typeof(OpenCaller));
            //log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = Open", ptr);

            //api = Win32ApiInvoker.GetProcAddress(ptr, "Close");
            //closeCaller = (CloseCaller)Marshal.GetDelegateForFunctionPointer(api, typeof(CloseCaller));
            //log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = Close", ptr);

            //api = Win32ApiInvoker.GetProcAddress(ptr, "Pop");
            //popCaller = (PopCaller)Marshal.GetDelegateForFunctionPointer(api, typeof(PopCaller));
            //log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = Pop", ptr);

            //api = Win32ApiInvoker.GetProcAddress(ptr, "GetState");
            //getStateCaller = (GetStateCaller)Marshal.GetDelegateForFunctionPointer(api, typeof(GetStateCaller));
            //log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = GetState", ptr);

            //StringBuilder sbCompany = new StringBuilder(128);
            //StringBuilder sbHardwareVersion = new StringBuilder(16);

            //int code = initCaller(sbCompany, sbHardwareVersion);
            //log.DebugFormat("invoke {0} -> Init args: company = {1}, hardwareVersion = {2}, return = {3}",
            //    dll, sbCompany, sbHardwareVersion, code);

            //code = getStateCaller();
            //log.DebugFormat("invoke {0} -> GetState args: return = {1}", dll, code);

            //// 如果吸卡器有卡，则退卡
            //if (910 == code)
            //{
            //    code = popCaller();
            //    log.DebugFormat("invoke {0} -> Close args: return = {1}", dll, code);
            //}

            log.Debug("end");
        }

        public void ReadAsync(JObject jo)
        {
            log.DebugFormat("begin SuctionCardReaderWriter.ReadAsync, args = {0}", jo);

            if (isBusy)
            {
                jo["result"] = ErrorCode.Busy;
                RunCompletedEvent(this, new RunCompletedEventArgs(jo));
                log.DebugFormat("end SuctionCardReaderWriter.ReadAsync, return = {0}", jo);
                return;
            }

            isBusy = true;
            cancelled = false;
            readAsyncCaller.BeginInvoke(jo, new AsyncCallback(Callback), jo);

            log.DebugFormat("end SuctionCardReaderWriter.ReadAsync, wait callback");
        }

        public void WriteAsync(JObject jo)
        {
            log.DebugFormat("begin SuctionCardReaderWriter.WriteAsync, args = {0}", jo);

            if (isBusy)
            {
                jo["result"] = ErrorCode.Busy;
                RunCompletedEvent(this, new RunCompletedEventArgs(jo));
                log.DebugFormat("end SuctionCardReaderWriter.WriteAsync, return = {0}", jo);
                return;
            }

            isBusy = true;
            cancelled = false;
            writeAsyncCaller.BeginInvoke(jo, new AsyncCallback(Callback), jo);

            log.DebugFormat("end SuctionCardReaderWriter.WriteAsync, wait callback");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 0：禁用；1：正常；2：忙；3：离线；
        /// </returns>
        public int GetStatus()
        {
            log.DebugFormat("begin SuctionCardReaderWriter.GetState");

            if (!enabled)
            {
                log.DebugFormat("end SuctionCardReaderWriter.GetState, return = {0}", StatusCode.Disabled);
                return StatusCode.Disabled;
            }

            if (isBusy)
            {
                log.DebugFormat("end SuctionCardReaderWriter.GetState, return = {0}", StatusCode.Busy);
                return StatusCode.Busy;
            }

            int code = getStateCaller();
            log.DebugFormat("invoke {0} -> return = {0}", dll, code);

            if (301 == code)
            {
                log.DebugFormat("end SuctionCardReaderWriter.GetState, return = {0}", StatusCode.Normal);
                return StatusCode.Normal;
            }
            else
            {
                log.DebugFormat("end SuctionCardReaderWriter.GetState, return = {0}", StatusCode.Offline);
                return StatusCode.Offline;
            }
        }

        public void Dispose()
        {
            log.DebugFormat("begin SuctionCardReaderWriter.Dispose");

            if (IntPtr.Zero != ptr)
            {
                Win32ApiInvoker.FreeLibrary(ptr);
                log.DebugFormat("FreeLibrary: ptr = {0}", ptr);
            }

            log.DebugFormat("end SuctionCardReaderWriter.Dispose");
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
            log.DebugFormat("begin SuctionCardReaderWriter.Read, args = {0}", jo);

            string tag = jo.Value<string>("tag");
            StringBuilder info = new StringBuilder(2048);

            // app 的返回值，应用包装
            int result = 0;
            // c++驱动返回值，驱动封装，打开串口
            int code = openCaller();
            log.DebugFormat("invoke {0} -> Open args: return = {1}", dll, code);

            long start = DateTime.Now.Ticks;

            while (true)
            {
                code = getStateCaller();

                if (910 == code)
                {
                    Thread.Sleep(1000);
                    code = readCaller(4, tag, info);
                    log.DebugFormat("invoke {0} -> Read args: type = {1}, tag = {2}, info = {3}, return = {4}", dll, 4, tag, info, code);
                    break;
                }

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

                Thread.Sleep(200);
            }

            if (ErrorCode.Cancelled == result || ErrorCode.Timeout == result)
            {
                jo["result"] = result;
            }
            else if (0 == code)
            {
                if (!tag.Contains(","))
                {
                    jo[tagDic[tag]] = info.ToString();
                }
                else
                {
                    string[] a1 = tag.Split(',');
                    string[] a2 = info.ToString().Split(',');

                    for (int i = 0; i < a1.Length; i++)
                    {
                        jo[tagDic[a1[i]]] = a2[i];
                    }
                }

                jo["result"] = ErrorCode.Success;
            }
            else
            {
                jo["result"] = ErrorCode.Failure;
            }

            Thread.Sleep(2000);
            code = popCaller();
            log.DebugFormat("invoke {0} -> Pop args: return = {1}", dll, code);

            Thread.Sleep(500);
            //code = getStateCaller();
            //log.DebugFormat("invoke {0} -> GetState args: return = {1}", dll, code);

            code = closeCaller();
            log.DebugFormat("invoke {0} -> Close args: return = {1}", dll, code);
            log.DebugFormat("end SuctionCardReaderWriter.Read return = {0}", jo);
        }

        public void Write(JObject jo)
        {
            log.DebugFormat("begin SuctionCardReaderWriter.Write, args = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            log.DebugFormat("end SuctionCardReaderWriter.Write return = {0}", jo);
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
                log.Error("SuctionCardReaderWriter.AsyncCallback Error", e);
            }
            finally
            {
                isBusy = false;
                RunCompletedEvent(this, new RunCompletedEventArgs(jo));
            }
        }
    }
}