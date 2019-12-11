using Aoto.EMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Aoto.EMS.Peripheral
{
    public class FaceCamera
    {
        /// <summary>
        /// 登录设备
        /// </summary>
        /// <param name="sDVRIP">设备IP地址 </param>
        /// <param name="wDVRPort">设备端口号 </param>
        /// <param name="sUserName">登录的用户名 </param>
        /// <param name="sPassword">用户密码 </param>
        /// <param name="lpDeviceInfo">设备信息 </param>
        /// <returns>-1表示失败，其他值表示返回的用户ID值</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Int32 NET_DVR_Login_V30(string sDVRIP, Int32 wDVRPort, string sUserName, string sPassword, ref NET_DVR_DEVICEINFO_V30 lpDeviceInfo);
        /// <summary>
        /// 错误代码(对比查看帮助文档)
        /// </summary>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint NET_DVR_GetLastError();
        /// <summary>
        /// 初始SDK
        /// </summary>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool NET_DVR_Init();
        /// <summary>
        /// 启动日志
        /// </summary>
        /// <param name="bLogEnable"></param>
        /// <param name="strLogDir"></param>
        /// <param name="bAutoDel"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool NET_DVR_SetLogToFile(int bLogEnable, string strLogDir, bool bAutoDel);
        /// <summary>
        /// 实时预览扩展接口
        /// </summary>
        /// <param name="iUserID"></param>
        /// <param name="lpPreviewInfo">预览参数</param>
        /// <param name="fRealDataCallBack_V30">码流数据回调函数</param>
        /// <param name="pUser">用户数据 </param>
        /// <returns>1表示失败，其他值作为NET_DVR_StopRealPlay等函数的句柄参数</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int NET_DVR_RealPlay_V40(int iUserID, ref NET_DVR_PREVIEWINFO lpPreviewInfo, RelDataCallBack fRealDataCallBack_V30, IntPtr pUser);
        /// <summary>
        /// 停止预览
        /// </summary>
        /// <param name="iRealHandle">预览句柄，NET_DVR_RealPlay或者NET_DVR_RealPlay_V30的返回值 </param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool NET_DVR_StopRealPlay(int iRealHandle);
        /// <summary>
        /// 抓拍
        /// </summary>
        /// <param name="lRealHandle"></param>
        /// <param name="sPicFileName"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool NET_DVR_CapturePicture(Int32 lRealHandle, string sPicFileName);

        #region 录像
        //动态生成I帧
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool NET_DVR_MakeKeyFrame(Int32 lUserID, Int32 lChannel);//主码流

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool NET_DVR_SaveRealData(Int32 lRealHandle, string sFileName);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool NET_DVR_StopSaveRealData(Int32 lRealHandle);
        #endregion

        private IntPtr intPtr; //加载句柄
        private IntPtr dvrPtr; //窗体预览句柄
        private Int32 m_RealHandle = -1; //预览编号

        private RelDataCallBack callBack; //回调委托


        private Int32 m_UserID = -1; //用户id
        private uint iLastErr = 0;//错误代码
        private int lChannel = 1; //预览/抓拍通道
        private bool m_bInitSDK = false;
        private bool m_bRecord = false;
        private bool m_bTalk = false;
        private int voiceComHandle = -1;
        private string str;


        private string bmpPath = Path.Combine(Application.StartupPath, "BMP");
        private string jpegPath = Path.Combine(Application.StartupPath, "JPEG");
        private string videoPath = Path.Combine(Application.StartupPath, "VIDEO");

        //参数
        private NET_DVR_DEVICEINFO_V30 deviceInfo;
        private string dvrIPAddress;
        private Int16 dvrPortNumber;
        private string dvrUserName;
        private string dvrPassword;

        //托管
        private NET_DVR_Init init;
        private NET_DVR_Login_V30 login;
        private NET_DVR_GetLastError lastError;
        private NET_DVR_SetLogToFile setLog;

        private NET_DVR_RealPlay_V40 realPlay;
        private NET_DVR_StopRealPlay stopPlay;
        private NET_DVR_CapturePicture capturePicture;

        private NET_DVR_MakeKeyFrame makeKeyFrame;
        private NET_DVR_SaveRealData saveReallData;
        private NET_DVR_StopSaveRealData stopRealData;

        public FaceCamera()
        {
            deviceInfo = new NET_DVR_DEVICEINFO_V30();
            dvrIPAddress = "172.16.210.240"; //设备IP地址或者域名
            dvrPortNumber = Int16.Parse("8000");//设备服务端口号
            dvrUserName = "admin";//设备登录用户名
            dvrPassword = "aoto@123";//设备登录密码
        }

        public int InitCamera(IntPtr dvrHandle)
        {
            string logPath = Path.Combine(Application.StartupPath, "SdkLog");
            string dllPath = Path.Combine(Application.StartupPath, "HCNetSDK.dll");
            intPtr = Win32ApiInvoker.LoadLibrary(dllPath);

            IntPtr api = Win32ApiInvoker.GetProcAddress(intPtr, "NET_DVR_Init");
            init = (NET_DVR_Init)Marshal.GetDelegateForFunctionPointer(api, typeof(NET_DVR_Init));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "NET_DVR_Login_V30");
            login = (NET_DVR_Login_V30)Marshal.GetDelegateForFunctionPointer(api, typeof(NET_DVR_Login_V30));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "NET_DVR_GetLastError");
            lastError = (NET_DVR_GetLastError)Marshal.GetDelegateForFunctionPointer(api, typeof(NET_DVR_GetLastError));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "NET_DVR_SetLogToFile");
            setLog = (NET_DVR_SetLogToFile)Marshal.GetDelegateForFunctionPointer(api, typeof(NET_DVR_SetLogToFile));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "NET_DVR_RealPlay_V40");
            realPlay = (NET_DVR_RealPlay_V40)Marshal.GetDelegateForFunctionPointer(api, typeof(NET_DVR_RealPlay_V40));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "NET_DVR_StopRealPlay");
            stopPlay = (NET_DVR_StopRealPlay)Marshal.GetDelegateForFunctionPointer(api, typeof(NET_DVR_StopRealPlay));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "NET_DVR_CapturePicture");
            capturePicture = (NET_DVR_CapturePicture)Marshal.GetDelegateForFunctionPointer(api, typeof(NET_DVR_CapturePicture));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "NET_DVR_MakeKeyFrame");
            makeKeyFrame = (NET_DVR_MakeKeyFrame)Marshal.GetDelegateForFunctionPointer(api, typeof(NET_DVR_MakeKeyFrame));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "NET_DVR_SaveRealData");
            saveReallData = (NET_DVR_SaveRealData)Marshal.GetDelegateForFunctionPointer(api, typeof(NET_DVR_SaveRealData));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "NET_DVR_StopSaveRealData");
            stopRealData = (NET_DVR_StopSaveRealData)Marshal.GetDelegateForFunctionPointer(api, typeof(NET_DVR_StopSaveRealData));


            //初始化SDK
            m_bInitSDK = init();
            if (m_bInitSDK == false)
            {
                //MessageBox.Show("NET_DVR_Init error!");
            }
            else
            {
                //初始化日志
                setLog(3, logPath, true);
            }

            //初始化摄像机
            if (m_UserID < 0)
            {
                m_UserID = login(dvrIPAddress, dvrPortNumber, dvrUserName, dvrPassword, ref deviceInfo);

                iLastErr = lastError();
                if (m_UserID == 0)
                    dvrPtr = dvrHandle;
            }
            else
            {

            }
            return m_UserID;
        }

        public string DisplayDVR()
        {
            string message = "";
            if (m_UserID < 0)
            {
                message = "Please login the device firstly";
            }

            if (m_RealHandle < 0)
            {
                NET_DVR_PREVIEWINFO lpPreviewInfo = new NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = dvrPtr;//预览窗口
                lpPreviewInfo.lChannel = 1;//预te览的设备通道      ***********************
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 1; //播放库播放缓冲区最大缓冲帧数
                lpPreviewInfo.byProtoType = 0;
                lpPreviewInfo.byPreviewMode = 0;

                //流ID摄制 
                //if (textBoxID.Text != "")
                //{
                //    lpPreviewInfo.lChannel = -1;
                //    byte[] byStreamID = System.Text.Encoding.Default.GetBytes(textBoxID.Text);
                //    lpPreviewInfo.byStreamID = new byte[32];
                //    byStreamID.CopyTo(lpPreviewInfo.byStreamID, 0);
                //}


                if (callBack == null)
                {
                    callBack = new RelDataCallBack(RealDataCallBack);//预览实时流回调函数
                }

                IntPtr pUser = new IntPtr();//用户数据

                //打开预览 Start live view 
                m_RealHandle = realPlay(m_UserID, ref lpPreviewInfo, null/*RealData*/, pUser);
                if (m_RealHandle < 0)
                {
                    iLastErr = lastError();
                    message = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr; //预览失败，输出错误号
                }
                else
                {
                    message = "Stop Live View";
                }
            }
            else
            {
                //停止预览 Stop live view 
                if (!stopPlay(m_RealHandle))
                {
                    iLastErr = lastError();
                    message = "NET_DVR_StopRealPlay failed, error code= " + iLastErr;

                }
                m_RealHandle = -1;
            }

            return message;
        }
        /// <summary>
        /// 抓拍
        /// </summary>
        /// <returns></returns>
        public string CapturePicture()
        {
            //图片保存路径和文件名 the path and file name to save
            string sBmpPicFileName = Path.Combine(bmpPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp");

            //BMP抓图 Capture a BMP picture
            if (!capturePicture(m_RealHandle, sBmpPicFileName))
            {
                iLastErr = lastError();
                str = "NET_DVR_CapturePicture failed, error code= " + iLastErr;
                //MessageBox.Show(str);
                return "";
            }
            else
            {
                str = "Successful to capture the BMP file and the saved file is " + sBmpPicFileName;
                //MessageBox.Show(str);
            }
            return sBmpPicFileName;
        }

        public string StartRecord()
        {
            //录像保存路径和文件名 the path and file name to save  
            string sVideoFileName = Path.Combine(videoPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4");

            //强制I帧 Make a I frame
            makeKeyFrame(m_UserID, lChannel);

            //开始录像 Start recording
            bool ret = saveReallData(m_RealHandle, sVideoFileName);
            if (ret)
            {
                return sVideoFileName;
            }
            else
            {
                iLastErr = lastError();
                return "NET_DVR_SaveRealData failed, error code= " + iLastErr;
            }

        }

        public bool StopRecord()
        {
            //停止录像 Stop recording
            return stopRealData(m_RealHandle);
        }


        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
            if (dwBufSize > 0)
            {
                byte[] sData = new byte[dwBufSize];
                Marshal.Copy(pBuffer, sData, 0, (Int32)dwBufSize);

                string str = "实时流数据.ps";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)dwBufSize;
                fs.Write(sData, 0, iLen);
                fs.Close();
            }
        }


        public const int STREAM_ID_LEN = 32;

        public const int SERIALNO_LEN = 48;//序列号长度

        //NET_DVR_Login_V30()参数结构
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO_V30
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;  //序列号
            public byte byAlarmInPortNum;		        //报警输入个数
            public byte byAlarmOutPortNum;		        //报警输出个数
            public byte byDiskNum;				    //硬盘个数
            public byte byDVRType;				    //设备类型, 1:DVR 2:ATM DVR 3:DVS ......
            public byte byChanNum;				    //模拟通道个数
            public byte byStartChan;			        //起始通道号,例如DVS-1,DVR - 1
            public byte byAudioChanNum;                //语音通道数
            public byte byIPChanNum;					//最大数字通道个数，低位  
            public byte byZeroChanNum;			//零通道编码个数 //2010-01-16
            public byte byMainProto;			//主码流传输协议类型 0-private, 1-rtsp,2-同时支持private和rtsp
            public byte bySubProto;				//子码流传输协议类型0-private, 1-rtsp,2-同时支持private和rtsp
            public byte bySupport;        //能力，位与结果为0表示不支持，1表示支持，
                                          //bySupport & 0x1, 表示是否支持智能搜索
                                          //bySupport & 0x2, 表示是否支持备份
                                          //bySupport & 0x4, 表示是否支持压缩参数能力获取
                                          //bySupport & 0x8, 表示是否支持多网卡
                                          //bySupport & 0x10, 表示支持远程SADP
                                          //bySupport & 0x20, 表示支持Raid卡功能
                                          //bySupport & 0x40, 表示支持IPSAN 目录查找
                                          //bySupport & 0x80, 表示支持rtp over rtsp
            public byte bySupport1;        // 能力集扩充，位与结果为0表示不支持，1表示支持
                                           //bySupport1 & 0x1, 表示是否支持snmp v30
                                           //bySupport1 & 0x2, 支持区分回放和下载
                                           //bySupport1 & 0x4, 是否支持布防优先级	
                                           //bySupport1 & 0x8, 智能设备是否支持布防时间段扩展
                                           //bySupport1 & 0x10, 表示是否支持多磁盘数（超过33个）
                                           //bySupport1 & 0x20, 表示是否支持rtsp over http	
                                           //bySupport1 & 0x80, 表示是否支持车牌新报警信息2012-9-28, 且还表示是否支持NET_DVR_IPPARACFG_V40结构体
            public byte bySupport2; /*能力，位与结果为0表示不支持，非0表示支持							
							bySupport2 & 0x1, 表示解码器是否支持通过URL取流解码
							bySupport2 & 0x2,  表示支持FTPV40
							bySupport2 & 0x4,  表示支持ANR
							bySupport2 & 0x8,  表示支持CCD的通道参数配置
							bySupport2 & 0x10,  表示支持布防报警回传信息（仅支持抓拍机报警 新老报警结构）
							bySupport2 & 0x20,  表示是否支持单独获取设备状态子项
							bySupport2 & 0x40,  表示是否是码流加密设备*/
            public ushort wDevType;              //设备型号
            public byte bySupport3; //能力集扩展，位与结果为0表示不支持，1表示支持
                                    //bySupport3 & 0x1, 表示是否多码流
                                    // bySupport3 & 0x4 表示支持按组配置， 具体包含 通道图像参数、报警输入参数、IP报警输入、输出接入参数、
                                    // 用户参数、设备工作状态、JPEG抓图、定时和时间抓图、硬盘盘组管理 
                                    //bySupport3 & 0x8为1 表示支持使用TCP预览、UDP预览、多播预览中的"延时预览"字段来请求延时预览（后续都将使用这种方式请求延时预览）。而当bySupport3 & 0x8为0时，将使用 "私有延时预览"协议。
                                    //bySupport3 & 0x10 表示支持"获取报警主机主要状态（V40）"。
                                    //bySupport3 & 0x20 表示是否支持通过DDNS域名解析取流

            public byte byMultiStreamProto;//是否支持多码流,按位表示,0-不支持,1-支持,bit1-码流3,bit2-码流4,bit7-主码流，bit-8子码流
            public byte byStartDChan;		//起始数字通道号,0表示无效
            public byte byStartDTalkChan;	//起始数字对讲通道号，区别于模拟对讲通道号，0表示无效
            public byte byHighDChanNum;		//数字通道个数，高位
            public byte bySupport4;
            public byte byLanguageType;// 支持语种能力,按位表示,每一位0-不支持,1-支持  
                                       //  byLanguageType 等于0 表示 老设备
                                       //  byLanguageType & 0x1表示支持中文
                                       //  byLanguageType & 0x2表示支持英文
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;		//保留
        }
        //预览V40接口
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PREVIEWINFO
        {
            public Int32 lChannel;//通道号
            public uint dwStreamType;	// 码流类型，0-主码流，1-子码流，2-码流3，3-码流4 等以此类推
            public uint dwLinkMode;// 0：TCP方式,1：UDP方式,2：多播方式,3 - RTP方式，4-RTP/RTSP,5-RSTP/HTTP 
            public IntPtr hPlayWnd;//播放窗口的句柄,为NULL表示不播放图象
            public bool bBlocked;  //0-非阻塞取流, 1-阻塞取流, 如果阻塞SDK内部connect失败将会有5s的超时才能够返回,不适合于轮询取流操作.
            public bool bPassbackRecord; //0-不启用录像回传,1启用录像回传
            public byte byPreviewMode;//预览模式，0-正常预览，1-延迟预览
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = STREAM_ID_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byStreamID;//流ID，lChannel为0xffffffff时启用此参数
            public byte byProtoType; //应用层取流协议，0-私有协议，1-RTSP协议
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public uint dwDisplayBufNum;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 216, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }
    }

    public delegate void RelDataCallBack(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser);
}
