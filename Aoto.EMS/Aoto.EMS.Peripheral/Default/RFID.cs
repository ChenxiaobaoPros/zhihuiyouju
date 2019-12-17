using Aoto.EMS.Infrastructure;
using Aoto.EMS.Infrastructure.ComponentModel;
using Aoto.EMS.Infrastructure.Configuration;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;

namespace Aoto.EMS.Peripheral
{
    public class RFID : IRFID
    {
        #region 托管
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int OpenDevice(); //打开设备

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetHardwareVersionNumber(StringBuilder data);	//获取硬件版本号

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetFirmwareVersion(StringBuilder data);			//获取固件版本号

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetUniqueID(StringBuilder UniqueID);			//获取唯一ID

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SendHeartbeat();					//心跳帧

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetdBm(StringBuilder data, int isOpen, int isSave, int antennaID, int ReadPower, int WritePower);	//设置发射功率 

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetCurrentdBm(StringBuilder data); //获取当前设备发射功率

        //[DllImport("AOTO_RFID_2019.dll", EntryPoint = " ", CharSet = CharSet.Ansi,
        // CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        //public delegate int SetFrequencyHopping(StringBuilder data,StringBuilder fmt,...);		//设置跳频
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetTiaoPing(StringBuilder data, ref int len);	// 获取跳频状态   部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetGen2(StringBuilder data, int len);// 设置Gen2参数   部分阅读器暂不支持该命令    数据由上层决定

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetGen2(StringBuilder data, ref int len);// 获取Gen2参数  部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetCW(int open);// 设置CW

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetCW(ref int Status);// 获取CW状态

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetAntenna(StringBuilder data);// 设置天线

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetAntenna(StringBuilder data, ref int len);// 获取天线设置

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetArea(int save, int area);// 设置区域

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetArea(ref int area);// 获取区域设置

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetPortVSWR(ref int fsdBm);// 获取端口回波损耗（驻波比)    部分阅读器暂不支持该命

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetTemperature(ref int temperature);// 获取设备当前温度    部分阅读器暂不支持该命令


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetTemperatureProtect(int temperature);// 设置温度保护    ，UM0、UM1、UR0 及UR1 暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetTemperatureProtect(ref int temperature);// 寻卡速率，当温度值超过85 摄氏度时，自动停止，并上报温度过高异常。

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetFindCardAndWaitTime(int save, int worktime, int waittime);// 设置连续寻卡工作及等待时间   仅部分阅读器支持此功能

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetFindCardAndWaitTime(ref int worktime, ref int waittime);// 获取连续寻卡工作及等待时间设置   部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetErrorFlag(ref int data);// 获取错误标志   部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetClearErrorFlag();// 清除错误标志   部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetGPIO(StringBuilder data);// 设置GPIO 操作

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetGPIO(char bytesrc, int ilen, StringBuilder data, ref int olen);// 获取GPIO操作

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetAntennaWorkTime(int Antnum, int save, int worktime);// 设置天线工作时间   部分阅读器暂不支持该命令,worktime：10ms~65535ms

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetAntennaWorkTime(int Antnum, ref int worktime);// 获取天线工作时间    部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetSomeAntennaWorkTTime(int save, int worktime);// 设置多天线工作间隔时间    部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetSomeAntennaWorkTTime(ref int worktime);// 获取多天线工作间隔时间    部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetRF(int save, int set);// 设置推荐RF 链路组合    部分阅读器暂不支持该命令，

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetRF(ref int status);// 获取推荐RF 链路组合设置   部分阅读器暂不支持该命令，

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetBeeperStatus(StringBuilder data, int len);// 设置蜂鸣器状态    部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetInternetPort(StringBuilder input);// 设置网口参数    部分阅读器暂不支持该命令---------------搁浅

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetWifi(StringBuilder data, int len);// 设置WIFI参数

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetFastID(int open);// 设置FastID 功能     部分阅读器暂不支持该命令


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetFastID(ref int status);// 获取FastID 功能状态

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetTagfocus(int open);// 设置Tagfocus 功能    部分阅读器不支持

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetTagFocus(ref int status);// 获取TagFocus 功能状态

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetRSSI(ref int RSSI);// 获取环境RSSI值   部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetREIDioctl(int ioctl);// 设置模块波特率     部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SoftwareReset();		// 软件复位    部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetDualAndSingel(int save, int mode);// 设置Dual 和Singel 模式    部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetDualAndSingel(ref int mode);// 获取Dual 和Singel 模式    部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetFindTagsfilters(StringBuilder data, int len);	// 寻标签过滤设置-------------由上层控制指令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetReadTagsDATA(int save, int Tags);// 设置同时读取标签数据模式	共三种模式EPC，EPC+TID，EPC+USR

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetReadTagsDATA(ref int status);// 获取同时读取标签数据模式

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RestoreFactoryDefaults();// 恢复出厂设置

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetInventory(int save, int mode);// 设置盘点工作模式

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetInventory(ref int mode);// 获取盘点工作模式

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int OneFindTags(int timeout, StringBuilder data, ref int len, StringBuilder EPC);// 单次寻标签

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int ReadEPC(StringBuilder EPC);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int ConstantFindTags(int Num, StringBuilder data, ref int len);// 连续寻标签

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int StopFindTags();// 停止连续寻标签

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int ReadDATA(int AccPwd, int MMB, int MSA, int MDL,
                                            StringBuilder MData, int MB, int SA, int DL,
                                            ref int Errflag, StringBuilder outdata, ref int olen);// 读数据********************************//?

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int WriteDATA(int AccPwd, int MMB, int MSA, int MDL,
                                             StringBuilder MData, int MB, int SA, int DL,
                                             StringBuilder indata, int datalen, ref int Errflag);// 写数据

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int LockTags(int AccPwd, int MMB, int MSA, int MDL,
                                            StringBuilder MData, int datalen, int LD, ref int Errflag);// 锁标签           部分阅读器暂不支持该命令


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int KillTags(int AccPwd, int MMB, int MSA, int MDL,
                                             StringBuilder MData, int datalen, ref int Errflag);// Kill 标签           部分阅读器暂不支持该命令


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SomeTimeFindTags(int timeout, ref int tagsnum);	// 时间段寻标签          部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetSomeTimeFindTags();// 获取时间段寻标签结果       部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int BlockWrite(int AccPwd, int MMB, int MSA, int MDL,
                                            StringBuilder MData, int MB, int SA, int DL,
                                            StringBuilder indata, int datalen, ref int Errflag);// Block Write 数据        部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int BlockErase(int AccPwd, int MMB, int MSA, int MDL,
                                            StringBuilder MData, int datalen, int MB, int SA, int DL,
                                            ref int Errflag);// Block Erase 数据        部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SetQT(int AccPwd, int MMB, int MSA, int MDL,
             StringBuilder MData, int datalen, int QTdata, ref int Errflag);// 设置QT 命令参数    部分阅读器暂不支持该命令


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetQT(int AccPwd, int MMB, int MSA, int MDL,
                                        StringBuilder MData, int datalen, ref int QTdata);// 获取QT 命令参数    部分阅读器暂不支持该命令


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int ReadQT(int AccPwd, int MMB, int MSA, int MDL,
                                        StringBuilder MData, int datalen, int QTdata,
                                        StringBuilder outdata, int olen, ref int Errflag);// QT 读操作    部分阅读器暂不支持该命令


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int WriteQT(int AccPwd, int MMB, int MSA, int MDL,
                                        StringBuilder MData, int datalen, int QTdata,
                                        int MB, int SA, int DL, StringBuilder indata,
                                        ref int Errflag);// QT 写操作    部分阅读器暂不支持该命令

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int OnlyRead(StringBuilder output, ref int len, ref int Num);

        #endregion

        private static readonly ILog log = LogManager.GetLogger("rfid");
        private IScriptInvoker scriptInvoker;
        public event RunCompletedEventHandler RunCompletedEvent;
        protected string name;
        protected string dll;
        protected int timeout;

        protected bool enabled;
        protected bool isBusy;
        protected bool cancelled;

        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }


        private IntPtr intPtr;
        private OpenDevice openDevice;
        private OneFindTags oneFindTags;
        public GetHardwareVersionNumber getHardwareVersionNumber;
        public GetFirmwareVersion getFirmwareVersion;
        private GetUniqueID getUniqueID;
        private SendHeartbeat sendHeartbeat;

        private ConstantFindTags constantFindTags;//连续巡检
        private StopFindTags stopFindTags;//停止连续巡检
        private OnlyRead onlyRead;


        private RunAsyncCaller asyncCaller;

        private List<string> tagList = null;


        public RFID()
        {
            isBusy = false;
            cancelled = false;
            this.dll = Config.App.Peripheral["rfid"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["rfid"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["rfid"].Value<bool>("enabled");
            this.name = Config.App.Peripheral["rfid"].Value<string>("name");
            scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
            asyncCaller = new RunAsyncCaller(Read);
            tagList = new List<string>();
            //Initialize();
        }
        public void Initialize()
        {
            tagList = new List<string>();
            string dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, dll);

            if (!File.Exists(dllPath))
            {
                dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "RFIDLib", dll);
            }

            intPtr = Win32ApiInvoker.LoadLibrary(dllPath);
            log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, intPtr);

            uint idcoed = Win32ApiInvoker.GetLastError();
            IntPtr api = Win32ApiInvoker.GetProcAddress(intPtr, "OpenDevice");
            openDevice = (OpenDevice)Marshal.GetDelegateForFunctionPointer(api, typeof(OpenDevice));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "OneFindTags");
            oneFindTags = (OneFindTags)Marshal.GetDelegateForFunctionPointer(api, typeof(OneFindTags));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "GetHardwareVersionNumber");
            getHardwareVersionNumber = (GetHardwareVersionNumber)Marshal.GetDelegateForFunctionPointer(api, typeof(GetHardwareVersionNumber));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "GetFirmwareVersion");
            getFirmwareVersion = (GetFirmwareVersion)Marshal.GetDelegateForFunctionPointer(api, typeof(GetFirmwareVersion));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "GetUniqueID");
            getUniqueID = (GetUniqueID)Marshal.GetDelegateForFunctionPointer(api, typeof(GetUniqueID));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "SendHeartbeat");
            sendHeartbeat = (SendHeartbeat)Marshal.GetDelegateForFunctionPointer(api, typeof(SendHeartbeat));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "ConstantFindTags");
            constantFindTags = (ConstantFindTags)Marshal.GetDelegateForFunctionPointer(api, typeof(ConstantFindTags));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "StopFindTags");
            stopFindTags = (StopFindTags)Marshal.GetDelegateForFunctionPointer(api, typeof(StopFindTags));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "OnlyRead");
            onlyRead = (OnlyRead)Marshal.GetDelegateForFunctionPointer(api, typeof(OnlyRead));

            int ret = openDevice();
            if (0 == ret)
            {
                list = new List<string>();
            }
            else
            {
            }
        }
        public void Read(JObject jo)
        {
            StringBuilder info = new StringBuilder(100);
            StringBuilder EPC = new StringBuilder(100);
            int len = 0;
            int timeout = 100;

            while (true)
            {
                int ret = oneFindTags(timeout, info, ref len, EPC);
                if (0 == ret)
                {
                    if (tagList.Where(t => t == EPC.ToString()).FirstOrDefault() == null)
                    {
                        tagList.Add(EPC.ToString());
                    }

                }
                else
                {
          
                }
            }
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
                //log.InfoFormat("end, isBusy = {0}", isBusy);
                return;
            }

            isBusy = true;
            asyncCaller.BeginInvoke(jo, new AsyncCallback(ReadCallback), jo);
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

        StringBuilder sendStringBuilder;
        StringBuilder getStringBuilder;
        List<string> list;

        public void Inspection()
        {
            sendStringBuilder = new StringBuilder(100);
            int sedlen = 0;
            int sednum = 0;
            constantFindTags(sednum, sendStringBuilder, ref sedlen);

            getStringBuilder = new StringBuilder(5000);
            int len = 0;
            int num = 0;
            for (int i = 0; i < 200; i++)
            {
                int ret = onlyRead(getStringBuilder, ref len, ref num);
                list.Add(getStringBuilder.ToString());

                if (list.Count > 200)
                    break;
            }

            stopFindTags();

            HashSet<string> hs = new HashSet<string>(list); //此时已经去掉重复的数据保存在hashset中
            List<string> ii = hs.ToList();
        }
        public void GetReadResult()
        {
            
        }

        #region 硬件相关
        /// <summary>
        /// 获取硬件版本
        /// </summary>
        private void GetHardware()
        {
            StringBuilder info = new StringBuilder(100);
            int ret = getHardwareVersionNumber(info);
        }
        /// <summary>
        /// 获取固件版本
        /// </summary>
        private void GetFirmware()
        {
            StringBuilder info = new StringBuilder(100);
            int ret = getFirmwareVersion(info);
        }
        /// <summary>
        /// 获取模块唯一id
        /// </summary>
        private void GetOnlyID()
        {
            StringBuilder info = new StringBuilder(100);
            int ret = getUniqueID(info);
        }
        /// <summary>
        /// 网口通信的设备才具有心跳功能，读写器如果超过 5 秒时间，没有上 报数据给服务端，自动上报一条心跳帧
        /// </summary>
        private void SwtHeartbeat()
        {
            //A5 5A 00 0A 0F 00 00 05 0D 0A
            int ret = sendHeartbeat();
        }
        #endregion

    }
}
