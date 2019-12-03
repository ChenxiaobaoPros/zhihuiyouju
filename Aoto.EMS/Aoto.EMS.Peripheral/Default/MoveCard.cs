using System;
using System.Collections.Generic;
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
        [DllImport("CRT_310.dll")]
        public static extern int CRT_IC_CardOpen(UInt32 ComHandle);
        //IC CARD POWER OFF
        [DllImport("CRT_310.dll")]
        public static extern int CRT_IC_CardClose(UInt32 ComHandle);

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

        public MoveCard()
        {

        }
    }
}
