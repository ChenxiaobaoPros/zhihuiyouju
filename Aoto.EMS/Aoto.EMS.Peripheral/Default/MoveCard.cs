using Aoto.EMS.Infrastructure;
using Aoto.EMS.Infrastructure.Configuration;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Aoto.EMS.Peripheral
{
    public class MoveCard 
    {

        #region 托管接口
        //打开串口
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint CommOpen(string port);
        //按指定的波特率打开串口
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate long CommOpenWithBaut(string port, ushort _data);
        //关闭串口
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CommClose(uint ComHandle);
        //读卡机序列号
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CRT310_ReadSnr(uint ComHandle, byte[] _SNData, ref byte _dataLen);
        //复位读卡机//0x30=不弹卡 0x31=前端弹卡 0x32=后端弹卡
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CRT310_Reset(uint ComHandle, byte _Eject);
        //停卡位置
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CRT310_CardPosition(UInt32 ComHandle, byte _Position);
        //移动卡
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CRT310_MovePosition(UInt32 ComHandle, byte _Position);
        //进卡控制
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CRT310_CardSetting(UInt32 ComHandle, byte _CardIn, byte _EnableBackIn);
        //从读卡机读取状态信息
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CRT310_GetStatus(UInt32 ComHandle, ref byte _CardStatus, ref byte _frontStatus, ref byte _RearStatus);
        //从读卡机读取传感器信息
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CRT310_SensorStatus(UInt32 ComHandle, ref byte _PSS0, ref byte _PSS1, ref byte _PSS2, ref byte _PSS3, ref byte _PSS4, ref byte _PSS5, ref byte _CTSW, ref byte _KSW);
        //指示灯亮灭控制
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CRT310_LEDSet(UInt32 ComHandle, byte _ONOFF);
        //指示灯时间
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CRT310_LEDTime(UInt32 ComHandle, byte _T1, byte _T2);
        //读磁轨数据
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int MC_ReadTrack(UInt32 ComHandle, byte _Mode, byte _track, ref int _TrackDataLen, byte[] _TrackData);
        //IC CARD POWER ON
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CRT_IC_CardOpen(UInt32 ComHandle);
        //IC CARD POWER OFF
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CRT_IC_CardClose(UInt32 ComHandle);

        [DllImport("CRT_310.dll")]
        public static extern int CRT_R_DetectCard(UInt32 ComHandle, ref byte _CardType, ref byte _CardInfor);

        [DllImport("CRT_310.dll")]
        public static extern int SLE4442_Reset(UInt32 ComHandle);

        [DllImport("CRT_310.dll")]
        public static extern int SLE4442_Read(UInt32 ComHandle, byte _Address, byte _BlockDataLen, byte[] _BlockData);

        [DllImport("CRT_310.dll")]
        public static extern int SLE4442_ReadP(UInt32 ComHandle, byte _BlockDataLen, byte[] _BlockData);

        [DllImport("CRT_310.dll")]
        public static extern int SLE4442_ReadS(UInt32 ComHandle, byte _BlockDataLen, byte[] _BlockData);

        [DllImport("CRT_310.dll")]
        public static extern int SLE4442_VerifyPWD(UInt32 ComHandle, byte _PWDatalen, byte[] _PWData);

        [DllImport("CRT_310.dll")]
        public static extern int SLE4442_Write(UInt32 ComHandle, byte _Address, byte _dataLen, byte[] _BlockData);

        [DllImport("CRT_310.dll")]
        public static extern int SLE4442_WriteP(UInt32 ComHandle, byte _Address, byte _dataLen, byte[] _BlockData);

        [DllImport("CRT_310.dll")]
        public static extern int SLE4442_WritePWD(UInt32 ComHandle, byte _PWDatalen, byte[] _PWData);

        [DllImport("CRT_310.dll")]
        public static extern int CPU_ColdReset(UInt32 ComHandle, byte _CPUMode, ref byte _CPUType, byte[] _exData, ref int _exdataLen);

        [DllImport("CRT_310.dll")]
        public static extern int CPU_WarmReset(UInt32 ComHandle, ref byte _CPUType, byte[] _exData, ref int _exdataLen);

        [DllImport("CRT_310.dll")]
        public static extern int CPU_T0_C_APDU(UInt32 ComHandle, int _dataLen, byte[] _APDUData, byte[] _exData, ref int _exdataLen);

        [DllImport("CRT_310.dll")]
        public static extern int CPU_T1_C_APDU(UInt32 ComHandle, int _dataLen, byte[] _APDUData, byte[] _exData, ref int _exdataLen);


        [DllImport("CRT_310.dll")]
        public static extern int SIM_Reset(UInt32 ComHandle, byte _VOLTAGE, byte _SIMNo, ref byte _SIMTYPE, byte[] _exData, ref int _exdataLen);

        [DllImport("CRT_310.dll")]
        public static extern int SIM_T0_C_APDU(UInt32 ComHandle, byte SIMNo, int _dataLen, byte[] _APDUData, byte[] _exData, ref int _exdataLen);

        [DllImport("CRT_310.dll")]
        public static extern int SIM_T1_C_APDU(UInt32 ComHandle, byte SIMNo, int _dataLen, byte[] _APDUData, byte[] _exData, ref int _exdataLen);

        [DllImport("CRT_310.dll")]
        public static extern int SIM_CardClose(UInt32 ComHandle, byte _AddH, byte _AddL);

        [DllImport("CRT_310.dll")]
        public static extern int RF_DetectCard(UInt32 ComHandle);

        [DllImport("CRT_310.dll")]
        public static extern int RF_GetCardID(UInt32 ComHandle, ref byte _CardIDLen, byte[] _CardID);

        [DllImport("CRT_310.dll")]
        public static extern int RF_LoadSecKey(UInt32 ComHandle, byte _Sec, byte _KEYType, byte _KEYLen, byte[] _KEY);

        [DllImport("CRT_310.dll")]
        public static extern int RF_ChangeSecKey(UInt32 ComHandle, byte _Sec, byte _KEYLen, byte[] _KEY);

        [DllImport("CRT_310.dll")]
        public static extern int RF_ReadBlock(UInt32 ComHandle, byte _Sec, byte _Block, ref byte _BlockDataLen, byte[] _BlockData);

        [DllImport("CRT_310.dll")]
        public static extern int RF_WriteBlock(UInt32 ComHandle, byte _Sec, byte _Block, byte _BlockDataLen, byte[] _BlockData);

        [DllImport("CRT_310.dll")]
        public static extern int RF_InitValue(UInt32 ComHandle, byte _Sec, byte _Block, byte _DataLen, byte[] _Data);

        [DllImport("CRT_310.dll")]
        public static extern int RF_ReadValue(UInt32 ComHandle, byte _Sec, byte _Block, ref byte _BlockDataLen, byte[] _BlockData);

        [DllImport("CRT_310.dll")]
        public static extern int RF_Increment(UInt32 ComHandle, byte _Sec, byte _Block, byte _DataLen, byte[] _Data);

        [DllImport("CRT_310.dll")]
        public static extern int RF_Decrement(UInt32 ComHandle, byte _Sec, byte _Block, byte _DataLen, byte[] _Data);
        #endregion

        private CommOpen commOpen;
        private CommClose commClose;
        private CRT310_Reset cRT310_Reset;//复位读卡机
        private CRT310_MovePosition cRT310_MovePosition;//移动卡
        private CRT310_CardSetting cRT310_CardSetting;//进卡控制
        private CRT310_GetStatus cRT310_GetStatus;//从读卡机读取状态信息
        private CRT310_SensorStatus cRT310_SensorStatus;//从读卡机读取传感器信息
        private MC_ReadTrack mC_ReadTrack;//读磁轨数据
        private CRT_IC_CardClose cRT_IC_CardClose;//IC卡关
        private CRT_IC_CardOpen cRT_IC_CardOpen;//IC开



        private static readonly ILog log = LogManager.GetLogger("moveCard");
        private IntPtr intPtr;

        protected string name;
        protected string dll;
        protected int timeout;

        protected bool enabled;
        protected bool isBusy;
        protected bool cancelled;

        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public MoveCard()
        {
            isBusy = false;
            cancelled = false;

            this.dll = Config.App.Peripheral["moveCard"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["moveCard"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["moveCard"].Value<bool>("enabled");
            this.name = Config.App.Peripheral["moveCard"].Value<string>("name");

            Initialize();
        }
        UInt32 Hdle = 0;
        byte CPUType = 2;
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
                dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "MoveCardLib", dll);
            }

            intPtr = Win32ApiInvoker.LoadLibrary(dllPath);
            log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, intPtr);

            IntPtr api = Win32ApiInvoker.GetProcAddress(intPtr, "CommOpen");
            commOpen = (CommOpen)Marshal.GetDelegateForFunctionPointer(api, typeof(CommOpen));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "CommClose");
            commClose = (CommClose)Marshal.GetDelegateForFunctionPointer(api, typeof(CommClose));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "CRT310_MovePosition");
            cRT310_MovePosition = (CRT310_MovePosition)Marshal.GetDelegateForFunctionPointer(api, typeof(CRT310_MovePosition));

            Hdle = commOpen("COM3");

        }
        /// <summary>
        /// 退卡
        /// </summary>
        public void BackCard()
        {
            if (Hdle != 0)
            {
                int i = cRT310_MovePosition(Hdle, 0x01);
                if (i == 0)
                {
                    //MessageBox.Show("Move Card OK", "Move Card");
                }
                else if (i == 69)
                {
                   //MessageBox.Show("No Card In", "Move Card");
                }
                else if (i == 87)
                {
                    //MessageBox.Show("The card is not on the card operation position", "Move Card");
                }
                else if (i == 78)
                {
                    //MessageBox.Show("Move card Error", "Move Card");
                }
                else
                {
                    //MessageBox.Show("unknow Error", "Move Card");
                }
            }
            else
            {
                //MessageBox.Show("Comm. port is not Opened", "Caution");
            }


        }


    }
}
