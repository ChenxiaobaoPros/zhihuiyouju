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

namespace Aoto.EMS.Peripheral
{
    public class KeyBoard
    {
        #region 托管接口
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_OpenCom(int nPortNo, int nTimeOut);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_GetVersionNo(StringBuilder ReturnInfo);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_CloseCom();
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_UseEppPlainTextMode(int PlaintextLength, int AutoEnd, StringBuilder ReturnInfo);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_ResetEpp(StringBuilder ReturnInfo);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_ScanKeyPress(StringBuilder ucValue);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_LoadUserKey(int ucKeyId, int ucDecryptKeyId, int KeyAttribute, int ucKeyLen, StringBuilder KeyValue, StringBuilder ReturnInfo);
        //
        //int WINAPI	SUNSON_CloseEppPlainTextMode(unsigned char *ReturnInfo);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_CloseEppPlainTextMode(StringBuilder ucValue);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_GetPin(int ucPinMinLen, int ucPinMaxLen, int AutoReturnFlag, StringBuilder ReturnInfo);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_GetPinBlock(int UserKeyId, int JM_mode, int padchar, int ucCardLen, String ucCardNumber, byte[] PinBlockResult);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_DataCompute(int KeyId, int JM_mode, int SF_mode, int padchar, int datalen, byte[] data, StringBuilder DataResult);
        #endregion

        private SUNSON_OpenCom sUNSON_OpenCom;
        private SUNSON_GetVersionNo sUNSON_GetVersionNo;

        private SUNSON_CloseCom sUNSON_CloseCom;
        private SUNSON_UseEppPlainTextMode sUNSON_UseEppPlainTextMode;
        private SUNSON_ResetEpp sUNSON_ResetEpp;
        private SUNSON_ScanKeyPress sUNSON_ScanKeyPress;
        private SUNSON_LoadUserKey sUNSON_LoadUserKey;
        private SUNSON_CloseEppPlainTextMode sUNSON_CloseEppPlainTextMode;
        private SUNSON_GetPin sUNSON_GetPin;
        private SUNSON_GetPinBlock sUNSON_GetPinBlock;
        private SUNSON_DataCompute sUNSON_DataCompute;

        private static readonly ILog log = LogManager.GetLogger("keyBoard");
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
        public KeyBoard()
        {
            isBusy = false;
            cancelled = false;

            this.dll = Config.App.Peripheral["keyBoard"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["keyBoard"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["keyBoard"].Value<bool>("enabled");
            this.name = Config.App.Peripheral["keyBoard"].Value<string>("name");
            runAsyncCaller = new RunAsyncCaller(UseEppPlainTextMode);
            Initialize();
        }
        /// <summary>
        /// 明文模式
        /// </summary>
        /// <param name="jo"></param>
        private void UseEppPlainTextMode(JObject jo)
        {
            throw new NotImplementedException();
        }

        private void Initialize()
        {
            log.Debug("begin");

            if (!enabled)
            {
                return;
            }

            string dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, dll);

            if (!File.Exists(dllPath))
            {
                dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "KeyBoardLib", dll);
            }

            intPtr = Win32ApiInvoker.LoadLibrary(dllPath);
            log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, intPtr);

            uint idcoed = Win32ApiInvoker.GetLastError();
            IntPtr api = Win32ApiInvoker.GetProcAddress(intPtr, "SUNSON_OpenCom");
            sUNSON_OpenCom = (SUNSON_OpenCom)Marshal.GetDelegateForFunctionPointer(api, typeof(SUNSON_OpenCom));
        }



    }
}
