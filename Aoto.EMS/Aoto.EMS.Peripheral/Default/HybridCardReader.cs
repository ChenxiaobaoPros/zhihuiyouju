using Aoto.EMS.Infrastructure;
using Aoto.EMS.Infrastructure.Configuration;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Aoto.EMS.Peripheral
{
    public class HybridCardReader
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

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CRT_R_DetectCard(UInt32 ComHandle, ref byte _CardType, ref byte _CardInfor);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SLE4442_Reset(UInt32 ComHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SLE4442_Read(UInt32 ComHandle, byte _Address, byte _BlockDataLen, byte[] _BlockData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SLE4442_ReadP(UInt32 ComHandle, byte _BlockDataLen, byte[] _BlockData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SLE4442_ReadS(UInt32 ComHandle, byte _BlockDataLen, byte[] _BlockData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SLE4442_VerifyPWD(UInt32 ComHandle, byte _PWDatalen, byte[] _PWData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SLE4442_Write(UInt32 ComHandle, byte _Address, byte _dataLen, byte[] _BlockData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SLE4442_WriteP(UInt32 ComHandle, byte _Address, byte _dataLen, byte[] _BlockData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SLE4442_WritePWD(UInt32 ComHandle, byte _PWDatalen, byte[] _PWData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CPU_ColdReset(UInt32 ComHandle, byte _CPUMode, ref byte _CPUType, byte[] _exData, ref int _exdataLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CPU_WarmReset(UInt32 ComHandle, ref byte _CPUType, byte[] _exData, ref int _exdataLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CPU_T0_C_APDU(UInt32 ComHandle, int _dataLen, byte[] _APDUData, byte[] _exData, ref int _exdataLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CPU_T1_C_APDU(UInt32 ComHandle, int _dataLen, byte[] _APDUData, byte[] _exData, ref int _exdataLen);


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SIM_Reset(UInt32 ComHandle, byte _VOLTAGE, byte _SIMNo, ref byte _SIMTYPE, byte[] _exData, ref int _exdataLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SIM_T0_C_APDU(UInt32 ComHandle, byte SIMNo, int _dataLen, byte[] _APDUData, byte[] _exData, ref int _exdataLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SIM_T1_C_APDU(UInt32 ComHandle, byte SIMNo, int _dataLen, byte[] _APDUData, byte[] _exData, ref int _exdataLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SIM_CardClose(UInt32 ComHandle, byte _AddH, byte _AddL);

        /// <summary>
        /// 检测是否是M卡
        /// </summary>
        /// <param name="ComHandle"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RF_DetectCard(UInt32 ComHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RF_GetCardID(UInt32 ComHandle, ref byte _CardIDLen, byte[] _CardID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RF_LoadSecKey(UInt32 ComHandle, byte _Sec, byte _KEYType, byte _KEYLen, byte[] _KEY);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RF_ChangeSecKey(UInt32 ComHandle, byte _Sec, byte _KEYLen, byte[] _KEY);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RF_ReadBlock(UInt32 ComHandle, byte _Sec, byte _Block, ref byte _BlockDataLen, byte[] _BlockData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RF_WriteBlock(UInt32 ComHandle, byte _Sec, byte _Block, byte _BlockDataLen, byte[] _BlockData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RF_InitValue(UInt32 ComHandle, byte _Sec, byte _Block, byte _DataLen, byte[] _Data);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RF_ReadValue(UInt32 ComHandle, byte _Sec, byte _Block, ref byte _BlockDataLen, byte[] _BlockData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RF_Increment(UInt32 ComHandle, byte _Sec, byte _Block, byte _DataLen, byte[] _Data);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RF_Decrement(UInt32 ComHandle, byte _Sec, byte _Block, byte _DataLen, byte[] _Data);

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
        private RF_DetectCard rF_DetectCard;//检测是否是M卡
        private RF_ReadBlock rF_ReadBlock;//读取M卡指定扇区数据
        private CPU_ColdReset cPU_ColdReset;//CPU卡复位
        private CPU_T0_C_APDU cPU_T0_C_APDU;//CPU指令执行
        private CPU_T1_C_APDU cPU_T1_C_APDU;//CPU指令执行


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
        public HybridCardReader()
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

            api = Win32ApiInvoker.GetProcAddress(intPtr, "CRT310_GetStatus");
            cRT310_GetStatus = (CRT310_GetStatus)Marshal.GetDelegateForFunctionPointer(api, typeof(CRT310_GetStatus));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "MC_ReadTrack");
            mC_ReadTrack = (MC_ReadTrack)Marshal.GetDelegateForFunctionPointer(api, typeof(MC_ReadTrack));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "RF_DetectCard");
            rF_DetectCard = (RF_DetectCard)Marshal.GetDelegateForFunctionPointer(api, typeof(RF_DetectCard));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "RF_ReadBlock");
            rF_ReadBlock = (RF_ReadBlock)Marshal.GetDelegateForFunctionPointer(api, typeof(RF_ReadBlock));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "CPU_ColdReset");
            cPU_ColdReset = (CPU_ColdReset)Marshal.GetDelegateForFunctionPointer(api, typeof(CPU_ColdReset));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "CPU_T0_C_APDU");
            cPU_T0_C_APDU = (CPU_T0_C_APDU)Marshal.GetDelegateForFunctionPointer(api, typeof(CPU_T0_C_APDU));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "CPU_T1_C_APDU");
            cPU_T1_C_APDU = (CPU_T1_C_APDU)Marshal.GetDelegateForFunctionPointer(api, typeof(CPU_T1_C_APDU));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "CRT_IC_CardClose");
            cRT_IC_CardClose = (CRT_IC_CardClose)Marshal.GetDelegateForFunctionPointer(api, typeof(CRT_IC_CardClose));

            Hdle = commOpen("COM5");

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
        /// <summary>
        /// 退一半
        /// </summary>
        public void HalfwayBack()
        {
            if (Hdle != 0)
            {
                int i = cRT310_MovePosition(Hdle, 0x02);
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
        public void RFCard()
        {
            if (Hdle != 0)
            {
                int i = cRT310_MovePosition(Hdle, 0x03);
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
        public void ICCard()
        {
            if (Hdle != 0)
            {
                int i = cRT310_MovePosition(Hdle, 0x04);
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
        /// <summary>
        /// 吞卡
        /// </summary>
        public void DrawBack()
        {
            if (Hdle != 0)
            {
                int i = cRT310_MovePosition(Hdle, 0x06);
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
        /// <summary>
        /// 拿到卡(还可以控制)
        /// </summary>
        public void HoldCard()
        {
            if (Hdle != 0)
            {
                int i = cRT310_MovePosition(Hdle, 0x05);
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

        public void GetCardReaderInfo()
        {
            byte _CardStatus, _frontSetting, _RearSetting;
            _CardStatus = 0;
            _frontSetting = 0;
            _RearSetting = 0;
            string Sb1, Sb2, Sb3;
            Sb1 = "";
            Sb2 = "";
            Sb3 = "";
            int i = cRT310_GetStatus(Hdle, ref _CardStatus, ref _frontSetting, ref _RearSetting);
            if (i == 0)
            {
                switch (_CardStatus)
                {
                    case 70:
                        Sb1 = "There is long card in the reader";
                        break;
                    case 72:
                        Sb1 = "There is card at the front side";
                        break;
                    case 73:
                        Sb1 = "There is card at the front card-hold position";
                        break;
                    case 74:
                        Sb1 = "There is card in the reader";
                        break;
                    case 75:
                        Sb1 = "There is card at IC card operation position";
                        break;
                    case 76:
                        Sb1 = "There is card at the rear card-hold position";
                        break;
                    case 77:
                        Sb1 = "There is card at the rear side";
                        break;
                    case 78:
                        Sb1 = "No Card In The Reader";
                        break;
                }
                switch (_frontSetting)
                {
                    case 73:
                        Sb2 = "Permit magnetic cards in only from front";
                        break;
                    case 74:
                        Sb2 = "Permit all cards in from front";
                        break;
                    case 75:
                        Sb2 = "Permit card-in by mag signal";
                        break;
                    case 78:
                        Sb2 = "Prohibit the cards in from front";
                        break;
                }
                switch (_RearSetting)
                {
                    case 74:
                        Sb3 = "Permit all cards in from front";
                        break;
                    case 78:
                        Sb3 = "Prohibit the cards in from front";
                        break;
                }
                //MessageBox.Show("Card Status:  " + Sb1 + "\r\n" + "frontSetting:  " + Sb2 + "\r\n" + "_RearSetting:  " + Sb3, "Status");

            }
            else
            {
                //MessageBox.Show("Status Error", "Status");
            }
        }

        public void MagRead()
        {
            if (Hdle != 0)
            {
                byte[] _TrackData = new byte[500];
                int _TrackDataLen = 0;
                string track1 = "";
                string track2 = "";
                string track3 = "";
                int i = -1;
                i = mC_ReadTrack(Hdle, 0x30, 0x37, ref _TrackDataLen, _TrackData);
                if (i == 0)
                {
                    int n;
                    int weizhi1 = 0;
                    int weizhi2 = 0; int weizhi3 = 0;
                    string Tra1Buf = "";
                    string Tra2Buf = "";
                    string Tra3Buf = "";

                    for (n = 0; n < _TrackDataLen; n++)
                    {
                        if (_TrackData[n] == 31)
                        {
                            weizhi1 = n;
                            break;
                        }
                    }
                    for (n = weizhi1 + 1; n < _TrackDataLen; n++)
                    {
                        if (_TrackData[n] == 31)
                        {
                            weizhi2 = n;
                            break;
                        }
                    }
                    for (n = weizhi2 + 1; n < _TrackDataLen; n++)
                    {
                        if (_TrackData[n] == 31)
                        {
                            weizhi3 = n;
                            break;
                        }
                    }
                    switch (_TrackData[weizhi1 + 1])
                    {
                        case 89:
                            for (n = weizhi1 + 2; n < weizhi2 - 1; n++)
                            {
                                //Tra1Buf += _TrackData[n].ToString(); 
                                Tra1Buf += (char)_TrackData[n];
                            }
                            track1 = Tra1Buf;
                            break;
                        case 78:
                            track1 = "Read/Parity Error" + Environment.NewLine;
                            switch (_TrackData[weizhi1 + 2])
                            {
                                case 225:
                                    track1 = track1 + "No start bits (STX)";
                                    break;
                                case 226:
                                    track1 = track1 + "No stop bits (ETX)";
                                    break;
                                case 227:
                                    track1 = track1 + "Byte Parity Error(Parity))";
                                    break;
                                case 228:
                                    track1 = track1 + "Parity Bit Error(LRC)";
                                    break;
                                case 229:
                                    track1 = track1 + "Card Track Data is Blank";
                                    break;
                            }
                            break;
                        case 69:
                            track1 = "No Read for this Track" + Environment.NewLine + "Card Track Data is 0xE0";
                            break;
                        case 0:
                            track1 = "No Read Operation" + Environment.NewLine + "Card Track Data is 0x00";
                            break;
                    }
                    switch (_TrackData[weizhi2 + 1])
                    {
                        case 89:
                            for (n = weizhi2 + 2; n < weizhi3 - 1; n++)
                            {
                                //Tra1Buf += _TrackData[n].ToString(); 
                                Tra2Buf += (char)_TrackData[n];
                            }
                            track2 = Tra2Buf;
                            break;
                        case 78:
                            track2 = "Read/Parity Error" + Environment.NewLine;
                            switch (_TrackData[weizhi2 + 2])
                            {
                                case 225:
                                    track2 = track2 + "No start bits (STX)";
                                    break;
                                case 226:
                                    track2 = track2 + "No stop bits (ETX)";
                                    break;
                                case 227:
                                    track2 = track2 + "Byte Parity Error(Parity))";
                                    break;
                                case 228:
                                    track2 = track2 + "Parity Bit Error(LRC)";
                                    break;
                                case 229:
                                    track2 = track2 + "Card Track Data is Blank";
                                    break;
                            }
                            break;
                        case 69:
                            track2 = "No Read for this Track" + Environment.NewLine + "Card Track Data is 0xE0";
                            break;
                        case 0:
                            track2 = "No Read Operation" + Environment.NewLine + "Card Track Data is 0x00";
                            break;
                    }
                    switch (_TrackData[weizhi3 + 1])
                    {
                        case 89:
                            for (n = weizhi3 + 2; n < _TrackDataLen - 1; n++)
                            {
                                //Tra1Buf += _TrackData[n].ToString(); 
                                Tra3Buf += (char)_TrackData[n];
                            }
                            track3 = Tra3Buf;
                            break;
                        case 78:
                            track3 = "Read/Parity Error" + Environment.NewLine;
                            switch (_TrackData[weizhi3 + 2])
                            {
                                case 225:
                                    track3 = track3 + "No start bits (STX)";
                                    break;
                                case 226:
                                    track3 = track3 + "No stop bits (ETX)";
                                    break;
                                case 227:
                                    track3 = track3 + "Byte Parity Error(Parity))";
                                    break;
                                case 228:
                                    track3 = track3 + "Parity Bit Error(LRC)";
                                    break;
                                case 229:
                                    track3 = track3 + "Card Track Data is Blank";
                                    break;
                            }
                            break;
                        case 69:
                            track3 = "No Read for this Track" + Environment.NewLine + "Card Track Data is 0xE0";
                            break;
                        case 0:
                            track3 = "No Read Operation" + Environment.NewLine + "Card Track Data is 0x00";
                            break;
                    }
                    //MessageBox.Show("Mag-Card Read OK", "Mag-Card Read");
                }
                else
                {
                    //MessageBox.Show("Mag-Card Read Error", "Mag-Card Read");
                }
            }
            else
            {
                //MessageBox.Show("Comm. port is not Opened", "Caution");
            }
        }

        public void IsMCard()
        {
          int ret =  rF_DetectCard(Hdle);

            /*
             如果函数调用成功，返回值为 0。
             // 如果函数调用失败，返回值不为 0。
             P=‘N’（0X4E） 寻卡不成功 
             P=‘E’（0X45） 卡机内无卡
             P=‘W’（0X57） 卡不在允许操作的位置上
             */
        }
        public void MRead()
        {
            if (Hdle != 0)
            {
                byte[] DataBlock = new byte[16];
                byte DataBlockLen = 0;
                byte SecNo = 0;
                byte BlockNo = 0;
                //扇区号
                SecNo = (byte)00;
                //块号
                BlockNo = (byte)00;
                //RF_ReadBlock(UInt32 ComHandle, byte _Sec, byte _Block, ref byte _BlockDataLen, byte[] _BlockData);
                int i = rF_ReadBlock(Hdle, SecNo, BlockNo, ref DataBlockLen, DataBlock);
                #region 查询数据(不知道扇区的情况下)
                //for (int s = 0; s < 12; s++)
                //{
                //    for (int n = 0; n < 12; n++)
                //    {
                //        SecNo = (byte)s;
                //        BlockNo = (byte)n;
                //        i = rF_ReadBlock(Hdle, SecNo, BlockNo, ref DataBlockLen, DataBlock);

                //        if (i == 0)
                //            break;
                //        else
                //            continue;


                //    }
                //}
                #endregion
                if (i == 0)
                {
                    int n;
                    string StrBuf = "";

                    for (n = 0; n < 16; n++)
                    {
                        StrBuf += DataBlock[n].ToString("X2") + " ";
                    }

                    //MessageBox.Show("Read Data OK", "Read Data");
                }
                else if (i == 69)
                {
                    //MessageBox.Show("No Card In", "Read Data");
                }
                else if (i == 78)
                {
                   //MessageBox.Show("Read Data Error", "Read Data");
                }
                else if (i == 0x30)
                {
                   //MessageBox.Show("No RF Card In", "Read Data");
                }
                else if (i == 0x31)
                {
                    //MessageBox.Show("Sector Error", "Read Data");
                }
                else if (i == 0x32)
                {
                    //MessageBox.Show("S/N Error", "Read Data");
                }
                else if (i == 0x33)
                {
                    //MessageBox.Show("Password Error", "Read Data");
                }
                else if (i == 0x34)
                {
                    //MessageBox.Show("Block Error", "Read Data");
                }
                else if (i == 0x35)
                {
                    //MessageBox.Show("Value overflow", "Read Data");
                }
                else
                {
                    //MessageBox.Show("Command Excute Error" + "\r\n" + "Error Code is " + i, "Read Data");
                }
            }
            else
            {
                //MessageBox.Show("Comm. port is not Opened", "Caution");
            }
        }
        public void CPURead()
        {
            //读卡前将卡片移动到ic读取位置
            ICCard();

            //CPU卡复位
            CPU_Reset();

            //发送CPU指令
            CPU_Command();
        }
        string CPUReset = "";
        string CPUCommand = "0084000008";
        string CPURetrun = "";
        public void CPU_Reset()
        {
            if (Hdle != 0)
            {
                byte[] _exData = new byte[500];
                int exdataLen = 0;
                int i = -1;
                CPUType = 2;

                //if (CPUModeOption1.Checked)
                //{
                //    i = cPU_ColdReset(Hdle, 0, ref CPUType, _exData, ref exdataLen);//EMV Model
                //}
                //else if (CPUModeOption2.Checked)
                //{
                //    i = cPU_ColdReset(Hdle, 1, ref CPUType, _exData, ref exdataLen);//ISO7816 Model(5V)
                //}
                //else
                //{
                //    i = cPU_ColdReset(Hdle, 2, ref CPUType, _exData, ref exdataLen);//ISO7816 Model(3V)
                //}

                i = cPU_ColdReset(Hdle, 2, ref CPUType, _exData, ref exdataLen);//ISO7816 Model(3V)

                if (i == 0)
                {
                    int n;
                    string StrBuf = "";

                    for (n = 0; n < exdataLen; n++)
                    {
                        StrBuf += _exData[n].ToString("X2");
                    }
                    CPUReset = StrBuf;
                    switch (CPUType)
                    {
                        case 0:
                            //MessageBox.Show("CPU CARD (T=0)", "Reset");
                            break;
                        case 1:
                            //MessageBox.Show("CPU CARD (T=1)", "Reset");
                            break;
                    }
                }
                else
                {
                    //MessageBox.Show("Reset Error", "Reset");
                }
            }
            else
            {
                //MessageBox.Show("Comm. port is not Opened", "Caution");
            }
        }
        public void CPU_Command()
        {
            if (Hdle != 0)
            {
                byte[] APDUByte = new byte[CPUCommand.Length / 2];
                byte[] _exData = new byte[500];
                int exdataLen = 0;
                int i = -1;

                for (int N = 0; N < (CPUCommand.Length / 2); N++)
                {
                    APDUByte[N] = byte.Parse(CPUCommand.Substring(N * 2, 2), NumberStyles.HexNumber);
                }
                if (CPUType == 2)
                {
                    //MessageBox.Show("Please Reset CPU Card First!", "Caution");
                }
                else
                {
                    if (CPUType == 0)
                    {
                        i = cPU_T0_C_APDU(Hdle, CPUCommand.Length / 2, APDUByte, _exData, ref exdataLen);
                    }
                    else
                    {
                        i = cPU_T1_C_APDU(Hdle, CPUCommand.Length / 2, APDUByte, _exData, ref exdataLen);
                    }
                    if (i == 0)
                    {
                        int n;
                        string StrBuf = "";

                        for (n = 0; n < exdataLen; n++)
                        {
                            StrBuf += _exData[n].ToString("X2");
                        }
                        CPURetrun = StrBuf;
                        //MessageBox.Show("Send APDU Access", "Send APDU");
                    }
                    else
                    {
                        //MessageBox.Show("Send APDU Error", "Send APDU");
                    }
                }
            }
            else
            {
                //MessageBox.Show("Comm. port is not Opened", "Caution");
            }
        }
        public void CPU_Down()
        {
            if (Hdle != 0)
            {

                int i = cRT_IC_CardClose(Hdle);
                if (i == 0)
                {
                    //MessageBox.Show("Power off OK", "Power off");
                }
                else if (i == 69)
                {
                    //MessageBox.Show("No Card In", "Power off");
                }
                else if (i == 87)
                {
                    //MessageBox.Show("The card is not on the IC card operation position", "Power off");
                }
                else if (i == 78)
                {
                    //MessageBox.Show("Power off Error", "Power off");
                }
                else
                {
                    //MessageBox.Show("Command Excute Error" + "\r\n" + "Error Code is " + i, "Power off");
                }
            }
            else
            {
                //MessageBox.Show("Comm. port is not Opened", "Caution");
            }
        }

    }
}
