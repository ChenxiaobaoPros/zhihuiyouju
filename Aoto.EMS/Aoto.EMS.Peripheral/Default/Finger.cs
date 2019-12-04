using Aoto.EMS.Infrastructure;
using Aoto.EMS.Infrastructure.ComponentModel;
using Aoto.EMS.Infrastructure.Configuration;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aoto.EMS.Peripheral
{
    public class Finger : IFinger
    {
        #region 托管接口
        /// <summary>
        /// 检测设备
        /// </summary>
        /// <param name="nPortNo">端口号(0-USB,1-COM1, 2-COM2, ..., 9-COM9)</param>
        /// <returns>0：设备在线；其它：设备不在线</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FPIDevDetect(int nPortNo);

        /// <summary>
        /// 取指纹特征
        /// </summary>
        /// <param name="nPortNo">端口号(0-USB,1-COM1, 2-COM2, ..., 9-COM9)</param>
        /// <param name="nTimeOut">超时时间(毫秒)</param>
        /// <param name="pTzData">指纹特征数据(输出参数,缓冲区大小至少为513字节)</param>
        /// <param name="pLength">指纹数据长度(输出参数)</param>
        /// <param name="lpErrMsg">错误信息(输出参数, 缓冲区大小至少为100字节)</param>
        /// <returns>0：成功；2: 取消；其它：失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FPIGetFeature(int nPortNo, int nTimeOut, StringBuilder pTzData, ref int pLength, StringBuilder lpErrMsg);
        /// <summary>
        /// 取指纹模板
        /// </summary>
        /// <param name="nPortNo">端口号(0-USB,1-COM1, 2-COM2, ..., 9-COM9)</param>
        /// <param name="nTimeOut">超时时间(毫秒)</param>
        /// <param name="pMbData">指纹模板数据(输出参数,缓冲区大小至少为513字节)</param>
        /// <param name="pLength">指纹数据长度(输出参数)</param>
        /// <param name="lpErrMsg">错误信息(输出参数, 缓冲区大小至少为100字节)</param>
        /// <returns>0：成功；2: 取消；其它：失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FPIGetTemplate(int nPortNo, int nTimeOut,StringBuilder pMbData, int pLength, StringBuilder lpErrMsg);
        /// <summary>
        /// 取指纹图像，FPIGetFeature成功后调用
        /// </summary>
        /// <param name="lpImgData">指纹图像(输出参数, 缓冲大小至少为30400+1078)</param>
        /// <param name="nLen">指纹数据长度(输出参数)</param>
        /// <returns>0：成功；<0：失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FPIGetImageData(byte[] lpImgData,ref int nLen);
        /// <summary>
        /// 手指检测
        /// </summary>
        /// <param name="nPortNo">端口号(0-USB)</param>
        /// <returns>0：检测到手指；-1：设备不在线 其它：未检测到手指</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FPIDetectFinger(int nPortNo);
        /// <summary>
        /// 取消
        /// </summary>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FPICancel();

        /// <summary>
        /// 获取设备版本号
        /// </summary>
        /// <param name="nPortNo">端口号(0-USB,1-COM1, 2-COM2, ..., 9-COM9)</param>
        /// <param name="lpVersion">设备版本号(输出参数, 缓冲区大小至少为200字节)</param>
        /// <returns>0：成功；其它：失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FPIGetVersion(int nPortNo, StringBuilder lpVersion);
        /// <summary>
        /// 指纹模板合成
        /// </summary>
        /// <param name="lpFeat1">指纹特征1</param>
        /// <param name="lpFeat2">指纹特征2</param>
        /// <param name="lpFeat3">指纹特征3</param>
        /// <param name="lpTemplate">指纹模板(输出参数,缓冲区大小至少为513字节以上)</param>
        /// <returns>0：成功；其它：失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FPIFeatureToTemplate(StringBuilder lpFeat1, StringBuilder lpFeat2,
    StringBuilder lpFeat3,StringBuilder lpTemplate);
        /// <summary>
        /// 比对指纹
        /// </summary>
        /// <param name="pRegBuf">指纹模板数据</param>
        /// <param name="pVerBuf">指纹特征数据</param>
        /// <param name="nLevel">安全等级(1~5，通常为3)</param>
        /// <returns>0：成功； <0：失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FPIFpMatch(StringBuilder pRegBuf, StringBuilder pVerBuf,ref int nLevel);
        #endregion

        private FPIDevDetect fpiDevDetect;//设备检测
        private FPIGetFeature fpiGetFeature;//获取指纹特征
        private FPIGetImageData fpiGetImageData;//获取指纹图片

        private FPIFpMatch fpiFpMatch;//指纹对比
        private FPIFeatureToTemplate fpiFeatureToTemplate;//指纹模板合成
        private FPIGetVersion fpiGetVersion;//获取设备版本号

        private FPIGetTemplate fpiGetTemplate;//获取指纹模板
        private FPIDetectFinger fpiDetectFinger;//手指检测
        private FPICancel fpiCance;//取消

        private static readonly ILog log = LogManager.GetLogger("finger");
        private IntPtr intPtr;
        private RunAsyncCaller asyncCaller;
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

        //指纹操作类型
        public FingerType FingerType { get; set; } = FingerType.ShowFinger;

        public Finger()
        {
            isBusy = false;
            cancelled = false;
            this.dll = Config.App.Peripheral["finger"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["finger"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["finger"].Value<bool>("enabled");
            this.name = Config.App.Peripheral["finger"].Value<string>("name");

            asyncCaller = new RunAsyncCaller(Read);
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
                dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "FingerLib", dll);
            }

            intPtr = Win32ApiInvoker.LoadLibrary(dllPath);
            log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, intPtr);

            uint idcoed = Win32ApiInvoker.GetLastError();
            IntPtr api = Win32ApiInvoker.GetProcAddress(intPtr, "FPIDevDetect");
            fpiDevDetect = (FPIDevDetect)Marshal.GetDelegateForFunctionPointer(api, typeof(FPIDevDetect));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "FPIGetFeature");
            fpiGetFeature = (FPIGetFeature)Marshal.GetDelegateForFunctionPointer(api, typeof(FPIGetFeature));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "FPIGetImageData");
            fpiGetImageData = (FPIGetImageData)Marshal.GetDelegateForFunctionPointer(api, typeof(FPIGetImageData));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "FPIFpMatch");
            fpiFpMatch = (FPIFpMatch)Marshal.GetDelegateForFunctionPointer(api, typeof(FPIFpMatch));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "FPIFeatureToTemplate");
            fpiFeatureToTemplate = (FPIFeatureToTemplate)Marshal.GetDelegateForFunctionPointer(api, typeof(FPIFeatureToTemplate));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "FPIGetVersion");
            fpiGetVersion = (FPIGetVersion)Marshal.GetDelegateForFunctionPointer(api, typeof(FPIGetVersion));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "FPIGetTemplate");
            fpiGetTemplate = (FPIGetTemplate)Marshal.GetDelegateForFunctionPointer(api, typeof(FPIGetTemplate));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "FPIDetectFinger");
            fpiDetectFinger = (FPIDetectFinger)Marshal.GetDelegateForFunctionPointer(api, typeof(FPIDetectFinger));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "FPICancel");
            fpiCance = (FPICancel)Marshal.GetDelegateForFunctionPointer(api, typeof(FPICancel));

            int re = fpiDevDetect(0);

            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = PPL398_InitialDevice", intPtr);

            log.Debug("end");
        }


        public void Read(JObject jo)
        {
            while (true)
            {
                StringBuilder message = new StringBuilder(100);
                StringBuilder version = new StringBuilder(1000);
                byte[] iamgeBytes = new byte[40000];
                int nlen = 0;
                int len = 0;
                try
                {
                    int ret = fpiGetFeature(0, 10000, version, ref nlen, message);

                    if (nlen > 0)
                    {
                        //图片
                        int re = fpiGetImageData(iamgeBytes, ref len);
                       
                        jo["retCode"] = 0;
                        jo["imageData"] = iamgeBytes;
                        jo["callback"] = "getFinger";
                        RunCompletedEvent(this,new RunCompletedEventArgs(iamgeBytes));
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                //特征
               
                Thread.Sleep(200);
            }
        }

        public void SaveBoard()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //关闭指纹
            fpiCance();
            //卸载dll
            Win32ApiInvoker.FreeLibrary(intPtr);


        }

        public StringBuilder MakeFeatureToTemplate(List<StringBuilder> stringBuilders)
        {
            StringBuilder stringBuilder=new StringBuilder(1000);
            int ret = fpiFeatureToTemplate(stringBuilders[0], stringBuilders[1], stringBuilders[2],stringBuilder);

            return stringBuilder;
        }

    }

    public enum FingerType
    {
        ShowFinger = 0,
        RegisterFinger =1

    }
}
