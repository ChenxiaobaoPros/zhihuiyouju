using Aoto.EMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Aoto.EMS.Peripheral
{
    public class YPBox :IYPBox
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int OpenDevice(int nPort);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetSerialNumber(byte[] info);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int MotorPoll(byte[] info);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int MotorScan(int MotorID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int MotorRun(byte[] info, int nIndex, int nType);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int ReadTemp(byte[] info);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int WriteDO(byte[] info, int index, int operate);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int ReadDI(byte[] info, int SlaveAddress);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CloseDevice();

        private OpenDevice openDevice;
        private GetSerialNumber getSerialNumber;
        private MotorRun motorRun;
        private MotorPoll motorPoll;
        private CloseDevice closeDevice;


        private IntPtr intPtr;
        private int ComNumber = 3;



        public YPBox()
        {
            string dllPath = Path.Combine(Application.StartupPath, "peripheral", "aoto", "YPBoxLib", "YPGDriver.dll");
            intPtr = Win32ApiInvoker.LoadLibrary(dllPath);

            IntPtr api = Win32ApiInvoker.GetProcAddress(intPtr, "OpenDevice");
            openDevice = (OpenDevice)Marshal.GetDelegateForFunctionPointer(api, typeof(OpenDevice));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "GetSerialNumber");
            getSerialNumber = (GetSerialNumber)Marshal.GetDelegateForFunctionPointer(api, typeof(GetSerialNumber));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "MotorRun");
            motorRun = (MotorRun)Marshal.GetDelegateForFunctionPointer(api, typeof(MotorRun));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "MotorPoll");
            motorPoll = (MotorPoll)Marshal.GetDelegateForFunctionPointer(api, typeof(MotorPoll));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "CloseDevice");
            closeDevice = (CloseDevice)Marshal.GetDelegateForFunctionPointer(api, typeof(CloseDevice));

        }
        public void Initialize()
        {
            int ret = openDevice(ComNumber);
        }
        public void StartingMotor(int index)
        {
            byte[] Serial_Num = new byte[100];

            //开启索引为0的电机
            int ret = motorRun(Serial_Num, index, (int)MotorType.TwoWireMotor);//0成功

            Thread.Sleep(1000);

            ret = motorPoll(Serial_Num);
            /* 状态
             "峰值电流:" + (Serial_Num[5] * 256 + Serial_Num[6]) + "mA 平均电流：" + (Serial_Num[7] * 256 + Serial_Num[8]) + "mA 运行时间：" + (Serial_Num[9] * 256 + Serial_Num[10]) + "mS\r\n"
             */

        }
        public void QueryMotorStatus()
        {
            byte[] Serial_Num = new byte[100];
            motorPoll(Serial_Num);
        }
        /// <summary>
        /// 巡检121
        /// </summary>
        public void Inspecting()
        {
            try
            {
                for (int i = 0; i < 121; i++)
                {
                    byte[] Serial_Num = new byte[10];
                    //开启索引为0的电机
                    int ret = motorRun(Serial_Num, i, (int)MotorType.TwoWireMotor);//0成功
                    Thread.Sleep(200);

                    while (true)
                    {
                        ret = motorPoll(Serial_Num);

                        if (ret == 3)
                        {
                           break;
                        }
                    }

                }
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public enum MotorType
    {
        NonFeedbackElectromagnet =0,//无反馈电磁铁
        FeedbackElectromagnet = 1,//有反馈电磁铁
        TwoWireMotor = 2,//两线制电机   (现在选这个)
        ThreeWireMotor = 3,//三线制电机
        ThreeLinesStrongThreeTracks = 6 //三线强三履带

    }
}
