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
    public class HighMeter: IHighMeter
    {
        #region 托管

        //初始化
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int camInitCameraLib();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int camUnInitCameraLib();

        //获取设备个数  (ref 的参数这样调用)
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int camGetDevCount(ref int count);

        //获取设备名
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate String camGetDevName(int devIndex);

        //开启设备
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int camOpenDev(int devIndex, int subtype, int width, int height);

        //关闭设备
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int camCloseDev(int devIndex);

        //设置预览窗口，第二参数要获取窗口句柄
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int camSetPreviewWindow(int devIndex, IntPtr hPreviewWindow);

        //视频放大
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int camZooIn(int devIndex);

        //视频缩小
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int camZoomOut(int devIndex);

        //视频以最适比例显示到窗口
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int camOptimalPreview(int devIndex);

        //视频以原始比例显示到窗口
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int camOriginalPreview(int devIndex);


        // delegate  bool callBackPreviewImage(StringBuilder src,long width,long height,long size);
        //注册视频图像（注册回调）
        // [DllImport("CmCapture.dll", EntryPoint = "camRegCallBackPreviewImage", CharSet = CharSet.Ansi,
        // CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        //public delegate long camRegCallBackPreviewImage(long devIndex,callBackPreviewImage fun);


        //拍照
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int camCaptureImageFile(int devIndex, StringBuilder filePath);
        //public delegate long camCaptureImageFile(long devIndex, String filePath);

        //保存图片
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int camSaveImage(String src, int width, int height, int bpp, String filePath);

        //补边
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int camSetImageFillBorder(int devIndex, int type);

        //绿边0不裁切1单图裁切2多图裁切
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int camSetImageAutoCrop(int devIndex, int CropType);

        #endregion

        private camInitCameraLib intit;
        private camGetDevCount getCount;
        private camGetDevName getName;
        private camOpenDev open;
        private camSetPreviewWindow set;

        private camCaptureImageFile image;

        private camCloseDev close;
        private camUnInitCameraLib uninit;

        private camSetImageFillBorder fillBorder;
        private camSetImageAutoCrop autoCrop;

        private String PrintLog;
        private int DevID = 0;
        private int CamCount = 0;
        private IntPtr IntPtr;
        private IntPtr panlePtr;

        public HighMeter()
        {
            IntPtr = Win32ApiInvoker.LoadLibrary(Path.Combine(Application.StartupPath, "peripheral", "aoto", "HighMeterLib", "CmCapture.dll"));

            IntPtr api = Win32ApiInvoker.GetProcAddress(IntPtr, "camInitCameraLib");
            intit = (camInitCameraLib)Marshal.GetDelegateForFunctionPointer(api, typeof(camInitCameraLib));

            api = Win32ApiInvoker.GetProcAddress(IntPtr, "camGetDevCount");
            getCount = (camGetDevCount)Marshal.GetDelegateForFunctionPointer(api, typeof(camGetDevCount));
          
            api = Win32ApiInvoker.GetProcAddress(IntPtr, "camGetDevName");
            getName = (camGetDevName)Marshal.GetDelegateForFunctionPointer(api, typeof(camGetDevName));

            api = Win32ApiInvoker.GetProcAddress(IntPtr, "camOpenDev");
            open = (camOpenDev)Marshal.GetDelegateForFunctionPointer(api, typeof(camOpenDev));

            api = Win32ApiInvoker.GetProcAddress(IntPtr, "camSetPreviewWindow");
            set = (camSetPreviewWindow)Marshal.GetDelegateForFunctionPointer(api, typeof(camSetPreviewWindow));

            api = Win32ApiInvoker.GetProcAddress(IntPtr, "camCaptureImageFile");
            image = (camCaptureImageFile)Marshal.GetDelegateForFunctionPointer(api, typeof(camCaptureImageFile));

            api = Win32ApiInvoker.GetProcAddress(IntPtr, "camCloseDev");
            close = (camCloseDev)Marshal.GetDelegateForFunctionPointer(api, typeof(camCloseDev));

            api = Win32ApiInvoker.GetProcAddress(IntPtr, "camUnInitCameraLib");
            uninit = (camUnInitCameraLib)Marshal.GetDelegateForFunctionPointer(api, typeof(camUnInitCameraLib));

            api = Win32ApiInvoker.GetProcAddress(IntPtr, "camSetImageFillBorder");
            fillBorder = (camSetImageFillBorder)Marshal.GetDelegateForFunctionPointer(api, typeof(camSetImageFillBorder));

            api = Win32ApiInvoker.GetProcAddress(IntPtr, "camSetImageAutoCrop");
            autoCrop = (camSetImageAutoCrop)Marshal.GetDelegateForFunctionPointer(api, typeof(camSetImageAutoCrop));
        }

        public void Initialize(IntPtr panlePtr)
        {
            if (panlePtr == IntPtr.Zero)
                return;

            this.panlePtr = panlePtr;

            int ret = intit();
           
            int flag = getCount(ref CamCount);

            String flag_camGetDevName = getName(1);

            int flag_camOpenDev = open(DevID, 0, 0, 0);

            int flag_camSetPreviewWindow = set(DevID, panlePtr);

        }
        public void Photograph()
        {
            StringBuilder PicPath = new StringBuilder("C:\\Program Files (x86)\\iamge.bmp");
            int flag_camCaptureImageFile = image(DevID, PicPath);       //返回值6，参数错误
            //if ((int)CAMSDK_ERR.ERR_NONE == flag_camCaptureImageFile)
            //{
            //    PrintLog = String.Format("拍照成功，图片路径：{0},返回值：{1}\r\n", PicPath.ToString(), flag_camCaptureImageFile);
            //    textBox1.Text += (PrintLog);
            //}
            //else
            //{
            //    PrintLog = String.Format("拍照失败，图片路径：{0},返回值：{1} \r\n", PicPath.ToString(), flag_camCaptureImageFile);
            //    textBox1.Text += (PrintLog);
            //}
        }

        public void CloseCamera()
        {
            int flag_camCloseDev = close(DevID);
            if ((int)CAMSDK_ERR.ERR_NONE == flag_camCloseDev)
            {
                PrintLog = String.Format("关闭设备成功，返回值{0} \r\n", flag_camCloseDev);
                //textBox1.Text += (PrintLog);
            }
            else
            {
                PrintLog = String.Format("关闭设备失败，返回值{0} \r\n", flag_camCloseDev);
                //textBox1.Text += (PrintLog);
            }


            int flag_camUnInitCameraLib = uninit();
            if ((int)CAMSDK_ERR.ERR_NONE == flag_camUnInitCameraLib)
            {
                PrintLog = String.Format("反初始化成功，返回值{0} \r\n", flag_camUnInitCameraLib);
                //textBox1.Text += (PrintLog);
            }
            else
            {
                PrintLog = String.Format("反初始化失败，返回值{0} \r\n", flag_camUnInitCameraLib);
                //textBox1.Text += (PrintLog);
            }
        }
        public void Other()
        {
            //补边
            //int ret = fillBorder(0,1);

            //绿边
            int ret = autoCrop(0, 1);
        }
    }
    public enum CAMSDK_ERR
    {
        ERR_NONE = 0,       //无错误
        ERR_UNKNOW,         //未知错误
        ERR_PROPERTY,       //属性设置错误
        ERR_INIT,           //初始化错误
        ERR_OUTOFBOUNDS,    //数组越界
        ERR_NULLPOINT,      //空指针
        ERR_PARAM,          //参数错误
        ERR_INVALIDPATH,    //无效的路径
        ERR_NONSUPPORT = 8,     //不支持

        ERR_OPENDEVFAILD = 257, //开启设备失败
        ERR_STARTVIDEOFAILD,    //开启视频失败
        ERR_SETRESOLUTIONFAILD, //设置分辨率事变
        ERR_AUTOFOCUSFAILD,     //自动对焦失败
        ERR_CAPTURENULLBUF,     //拍照空指针     
        ERR_SAVEIMAGEFAILD,     //保存图像失败
        ERR_NOLICENSE,      //没有授权
        ERR_WRITEDEVFAILED, //写设备数据失败

        ERR_FTPCONNECTFAILED = 1025,    //FTP连接失败
        ERR_FTPDISCONNECTFAILED,    //FTP断开失败
        ERR_FTPADDRESSERROR,        //FTP路径有误
        ERR_FTPPUTFILEFAILED,   //FTP上传失败
        ERR_HTTPPUTFILEFAILED,  //FTP连接失败

        ERR_LOADIDCARDOCRFAILED = -99,  //加载证件识别失败

        ERR_LOADBARCODEFAILED = 128,    //加载条码识别失败
    };
}
