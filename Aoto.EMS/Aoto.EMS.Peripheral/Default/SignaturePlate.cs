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
    public class SignaturePlate : ISignaturePlate
    {
        #region L398
        /// <summary>
        /// l398
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_InitialDevice", CharSet = CharSet.Ansi,
           CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern bool PPL398_InitialDevice(DEVICE_TYPE_ID deviceID, IntPtr hWnd, Int32 x, Int32 y, Int32 ulCanvasWidth, Int32 ulCanvasHeight);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_UnInitialDevice", CharSet = CharSet.Ansi,
           CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_UnInitialDevice(DEVICE_TYPE_ID deviceID);

        // 设置签名窗尺寸 
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_SetCanvasSize", CharSet = CharSet.Ansi,
           CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_SetCanvasSize(DEVICE_TYPE_ID deviceID, Int32 x, Int32 y, Int32 ulCanvasWidth, Int32 ulCanvasHeight);

        //获取手写签名设备 
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_getDeviceInfo", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_getDeviceInfo(DEVICE_TYPE_ID deviceID, Int32 nIndex, out IntPtr lpOutput);

        //获取手写签名设备的X,Y的范围值
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_getDeviceInfo", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_getDeviceInfoPPAXIS(DEVICE_TYPE_ID deviceID, Int32 nIndex, out tagPPAXIS lpOutput);

        //获取手写签名设备 
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_getDeviceInfo", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_getDeviceInfoString(DEVICE_TYPE_ID deviceID, Int32 nIndex, StringBuilder lpOutput);

        // 
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_GetHWPadID", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_GetHWPadID(DEVICE_TYPE_ID deviceID, StringBuilder padID);

        // // 获取笔的唯一ID值
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_GetHWPenID", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_GetHWPenID(DEVICE_TYPE_ID deviceID, StringBuilder buffer, UInt32 bufferLen, ref UInt32 idLen);

        //  获取L398板的version ID
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_GetHWVersionID", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_GetHWVersionID(DEVICE_TYPE_ID deviceID, StringBuilder version);
        //// L398显示Version ID
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_DisplayHWVersion", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_DisplayHWVersion(DEVICE_TYPE_ID deviceID, Int32 bVal);

        ////开LED屏 
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_OpenLed", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_OpenLed(DEVICE_TYPE_ID deviceID);
        //// 关闭LED屏
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_CloseLed", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_CloseLed(DEVICE_TYPE_ID deviceID);

        //// 
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_GetTotalPacketsNumber", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_GetTotalPacketsNumber(DEVICE_TYPE_ID deviceID);

        //// // 获取签名数据
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_getPackets", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_getPackets(DEVICE_TYPE_ID deviceID, ref tagPACKETS InputReport);

        ////L398中读取签名图像 
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_SaveDeviceImage", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_SaveDeviceImage(DEVICE_TYPE_ID deviceID, StringBuilder ImageName);

        ////L398保存笔迹图像， 
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_SaveDrawingImage", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_SaveDrawingImage(DEVICE_TYPE_ID deviceID, UInt32 ulFormat, StringBuilder ImageName, Int32 dpi);

        //
        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void CallBackMyFun1(Int32 lParam);

        ////L398  ， 
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_SignatureStatusCallback", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_SignatureStatusCallback(DEVICE_TYPE_ID deviceID, CallBackMyFun1 myFun);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_Clear", CharSet = CharSet.Ansi,
             CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_Clear(DEVICE_TYPE_ID deviceID);

        ////L398 
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_PacketsBase64Encode", CharSet = CharSet.Ansi,
             CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        // [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern IntPtr PPL398_PacketsBase64Encode(DEVICE_TYPE_ID deviceID, Int32 ulFormat);
        ////L398， 
        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_PacketsBase64EncodeFree", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void PPL398_PacketsBase64EncodeFree(DEVICE_TYPE_ID deviceID, IntPtr pMem);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_Base64Decode", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_Base64Decode(DEVICE_TYPE_ID deviceID, UInt32 ulFormat, StringBuilder EncodeStr, StringBuilder SaveFileStr);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_GetTotalDecodePacketsNumber", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_GetTotalDecodePacketsNumber(DEVICE_TYPE_ID deviceID);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_GetDecodePacketsData", CharSet = CharSet.Ansi,
             CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_GetDecodePacketsData(DEVICE_TYPE_ID deviceID, Int32 lIndex, Int32 lGetInkDataType);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_SetPenColor", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_SetPenColor(DEVICE_TYPE_ID deviceID, UInt32 ulR, UInt32 ulG, UInt32 ulB);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_SetPenStyle", CharSet = CharSet.Ansi,
             CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_SetPenStyle(DEVICE_TYPE_ID deviceID, UInt32 ulPenID);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_ReadDeviceData", CharSet = CharSet.Unicode,
             CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_ReadDeviceData(DEVICE_TYPE_ID deviceID, StringBuilder CertPathOut, Int32 nIndex);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_SaveDeviceData", CharSet = CharSet.Unicode,
              CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_SaveDeviceData(DEVICE_TYPE_ID deviceID, StringBuilder CertPath, Int32 nIndex);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_ClearDeviceData", CharSet = CharSet.Ansi,
             CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_ClearDeviceData(DEVICE_TYPE_ID deviceID, Int32 index);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_SetSaveCenterImageClip", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_SetSaveCenterImageClip(DEVICE_TYPE_ID deviceID, Int32 bClipBlank, UInt32 ulHorMargin, UInt32 ulVerMargin);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_BeginCalibrate", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_BeginCalibrate(DEVICE_TYPE_ID deviceID);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_EndCalibrate", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_EndCalibrate(DEVICE_TYPE_ID deviceID, bool bCalibrate);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_AboutBox", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void PPL398_AboutBox();

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_SetPenWidth", CharSet = CharSet.Ansi,
             CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_SetPenWidth(DEVICE_TYPE_ID deviceID, UInt32 ulWidth);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_GetInkPointTime", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr PPL398_GetInkPointTime(DEVICE_TYPE_ID deviceID, Int32 lIndex);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_FreeInkPointTime", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void PPL398_FreeInkPointTime(DEVICE_TYPE_ID deviceID, IntPtr pTimeString);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void NOTIFYREALTIMESIGNDATA(IntPtr wParam, IntPtr lParam);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_RealTimeSignatureDataCallback", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern long PPL398_RealTimeSignatureDataCallback(DEVICE_TYPE_ID deviceID, NOTIFYREALTIMESIGNDATA pfnRealTimeSignDataCallback);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_SaveDrawingVideo", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_SaveDrawingVideo(DEVICE_TYPE_ID deviceID, UInt32 ulFormat, Int32 fps, StringBuilder ImageName);

        [DllImport("PPSignToolSDK.dll", EntryPoint = "PPL398_EnableSaveVideoData", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Int32 PPL398_EnableSaveVideoData(DEVICE_TYPE_ID deviceID, bool bEnable);
        #endregion

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool Board_Initialize(DEVICE_TYPE_ID deviceID, IntPtr hWnd, Int32 x, Int32 y, Int32 ulCanvasWidth, Int32 ulCanvasHeight);

        private Board_Initialize boardInitialize;//委托实例


        private static readonly ILog log = LogManager.GetLogger("writingBoard");
        private IntPtr ptr;

        protected string name;
        protected string dll;
        protected int timeout;

        protected bool enabled;
        protected bool isBusy;
        protected bool cancelled;

        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }

        public IntPtr panlePtr { get; set; }
        public int heigh { get; set; }
        public int width { get; set; }

        public SignaturePlate()
        {
            isBusy = false;
            cancelled = false;

            this.dll = Config.App.Peripheral["writingBoard"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["writingBoard"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["writingBoard"].Value<bool>("enabled");
            this.name = Config.App.Peripheral["writingBoard"].Value<string>("name");
        }

        public void Initialize(IntPtr panlePtr, int high, int wide)
        {
            if (panlePtr == IntPtr.Zero || high == 0 || wide == 0)
                return;

            this.panlePtr = panlePtr;
            this.heigh = high;
            this.width = wide;

            bool result = PPL398_InitialDevice(DEVICE_TYPE_ID.PPL398_DEVICE, panlePtr, 0, 0, wide, high);
            if (result == false)
            {
                return;
            }
            CallBackMyFun1 fun = new CallBackMyFun1(PPSignStatusNotify);

            NOTIFYREALTIMESIGNDATA _RealDataCallback = new NOTIFYREALTIMESIGNDATA(RealTimeDataCallback);
            Int32 ret = PPL398_SignatureStatusCallback(DEVICE_TYPE_ID.PPL398_DEVICE, fun);

            long retlong = PPL398_RealTimeSignatureDataCallback(DEVICE_TYPE_ID.PPL398_DEVICE, _RealDataCallback);


            //log.Debug("begin");

            //if (!enabled)
            //{
            //    return;
            //}

            //string dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, dll);

            //if (!File.Exists(dllPath))
            //{
            //    dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "BoardLib", dll);
            //}

            //ptr = Win32ApiInvoker.LoadLibrary(dllPath);
            //log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, ptr);
            //uint idcoed = Win32ApiInvoker.GetLastError();
            //IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "PPL398_InitialDevice");
            //boardInitialize = (Board_Initialize)Marshal.GetDelegateForFunctionPointer(api, typeof(Board_Initialize));

            //bool re = boardInitialize(DEVICE_TYPE_ID.PPL398_DEVICE, panlePtr, 0, 0, wide, high);
            //log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = PPL398_InitialDevice", ptr);

            //log.Debug("end");

        }

        private void RealTimeDataCallback(IntPtr wParam, IntPtr lParam)
        {

        }

        private void PPSignStatusNotify(int lParam)
        {

        }

        public void SaveBoard()
        {
            string strName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            string strFileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + strName;
            StringBuilder sb = new StringBuilder(strFileName);

            PPL398_SaveDrawingImage(DEVICE_TYPE_ID.PPL398_DEVICE, 1, sb, 0);

            #region 原版
            ////选择格式，
            //UInt32 nItem = Convert.ToUInt32(combL398Image.SelectedIndex);
            //string strExt = ".bmp";
            //if (nItem == 0)
            //    strExt = ".bmp";
            //else if (nItem == 1)
            //    strExt = ".jpg";
            //else if (nItem == 2)
            //    strExt = ".png";
            //else if (nItem == 3)
            //    strExt = ".gif";
            //else if (nItem == 4)
            //    strExt = ".tiff";
            //else if (nItem == 5)
            //    strExt = ".pdf";
            //else if (nItem == 6)
            //    strExt = ".svg";

            //if (nItem >= 5)
            //    nItem++;

            //string strName = DateTime.Now.ToString("yyyyMMddHHmmss") + strExt;

            ////FolderBrowserDialog dlg = new FolderBrowserDialog();
            ////dlg.SelectedPath = "c:";
            ////dlg.Description = "请选择保存文件的目录";
            ////if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            ////    return;
            ////string strFileName = dlg.SelectedPath + "\\";
            ////strFileName += strName;

            //string strFileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + strName;
            //StringBuilder sb = new StringBuilder(strFileName);

            ////像素点
            //Int32 index = comboBox_dpi.SelectedIndex;
            //string dpi = comboBox_dpi.Items[index].ToString();
            //int iDpi = index;

            //Int32 ret = 0;
            //nItem = nItem + 1;
            //if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPL398_DEVICE)
            //    ret = ImportDll.PPL398_SaveDrawingImage(ImportDll.DEVICE_TYPE_ID.PPL398_DEVICE, nItem, sb, iDpi);
            //else if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPNature_DEVICE)
            //    ret = ImportDll.PPNature_SaveDrawingImage(ImportDll.DEVICE_TYPE_ID.PPNature_DEVICE, nItem, sb, iDpi);
            //else if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPL500_DEVICE)
            //    ret = ImportDll.PPL500_SaveDrawingImage(ImportDll.DEVICE_TYPE_ID.PPL500_DEVICE, nItem, sb, iDpi);
            //else if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPE580_DEVICE)
            //{
            //    ret = ImportDll.PPE580_SaveDrawingImage(ImportDll.DEVICE_TYPE_ID.PPE580_DEVICE, nItem, sb, iDpi);
            //}
            //else if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPL501F_DEVICE)
            //{
            //    ret = ImportDll.PPL501F_SaveDrawingImage(ImportDll.DEVICE_TYPE_ID.PPL501F_DEVICE, nItem, sb, iDpi);
            //}
            //else if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPW300_DEVICE)
            //{
            //    ret = ImportDll.PPW300_SaveDrawingImage(ImportDll.DEVICE_TYPE_ID.PPW300_DEVICE, nItem, sb, iDpi);
            //}
            //else if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPL1000_DEVICE)
            //{
            //    string strDrawImagePath = strFileName;
            //    if (nItem == 7)
            //    {
            //        strDrawImagePath = strDrawImagePath.Replace(".pdf", ".jpg");
            //        StringBuilder sbDrawImagePath = new StringBuilder(strDrawImagePath);
            //        ret = ImportDll.PPL1000_SaveDrawingImage(ImportDll.DEVICE_TYPE_ID.PPL1000_DEVICE, 2, sbDrawImagePath, iDpi);
            //    }
            //    else if (nItem == 8)
            //    {
            //        strDrawImagePath = strDrawImagePath.Replace(".svg", ".jpg");
            //        StringBuilder sbDrawImagePath = new StringBuilder(strDrawImagePath);
            //        ret = ImportDll.PPL1000_SaveDrawingImage(ImportDll.DEVICE_TYPE_ID.PPL1000_DEVICE, 2, sbDrawImagePath, iDpi);
            //    }

            //    ret = ImportDll.PPL1000_SaveDrawingImage(ImportDll.DEVICE_TYPE_ID.PPL1000_DEVICE, nItem, sb, iDpi);

            //    Image imageSaved = Image.FromFile(strDrawImagePath);
            //    Graphics g = Graphics.FromHwnd(panelDrawArea.Handle);
            //    int nNewWidth = panelDrawArea.Width;
            //    int nNewHeight = (int)((float)nNewWidth * (float)imageSaved.Height / (float)imageSaved.Width);
            //    int nNewY = (panelDrawArea.Height - nNewHeight) >> 1;
            //    g.DrawImage(imageSaved, 0, nNewY, nNewWidth, nNewHeight);
            //    imageSaved.Dispose();
            //    g.Dispose();
            //}

            //textL398Status.Text = strFileName + ", result : " + ret;

            #endregion
        }
        public void Clear()
        {
          
            PPL398_Clear(DEVICE_TYPE_ID.PPL398_DEVICE);

            #region 原版
            //textL398Status.Text = "Clearing data, please wait...";

            //System.Windows.Forms.Application.DoEvents();

            //Int32 nVal = 0;
            //if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPL398_DEVICE)
            //    nVal = ImportDll.PPL398_Clear(m_deviceID);
            //else if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPNature_DEVICE)
            //    nVal = ImportDll.PPNature_Clear(m_deviceID);
            //else if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPL500_DEVICE)
            //    nVal = ImportDll.PPL500_Clear(m_deviceID);
            //else if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPE580_DEVICE)
            //{
            //    nVal = ImportDll.PPE580_Clear(m_deviceID);
            //}
            //else if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPL501F_DEVICE)
            //{
            //    nVal = ImportDll.PPL501F_Clear(m_deviceID);
            //}
            //else if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPW300_DEVICE)
            //{
            //    nVal = ImportDll.PPW300_Clear(m_deviceID);
            //}
            //else if (m_deviceID == ImportDll.DEVICE_TYPE_ID.PPL1000_DEVICE)
            //{
            //    panelDrawArea.Invalidate(null);
            //    nVal = ImportDll.PPL1000_Clear(m_deviceID);
            //}

            //textL398Status.Text = "Clear finished, result = " + nVal.ToString();
            #endregion
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
  
    }
    public enum DEVICE_TYPE_ID
    {
        NONE_DEVICE = 0,
        PPNature_DEVICE = 1,
        PPL398_DEVICE = 2,
        PPL500_DEVICE = 5,
        PPE580_DEVICE = 6,
        PPL501F_DEVICE = 7,
        PPW300_DEVICE = 8,
        PPL1000_DEVICE = 9
    };
    public struct tagPPAXIS
    {
        public Int32 axMin;
        public Int32 axMax;
    }
    public struct tagPACKETS
    {
        public Int32 btn;
        public Int32 X;
        public Int32 Y;
        public Int32 Press;
        public bool bStrokeEnd;
    }
}
