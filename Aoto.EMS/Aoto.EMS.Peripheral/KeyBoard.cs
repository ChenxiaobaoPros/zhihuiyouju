using Aoto.EMS.Infrastructure.Configuration;
using log4net;
using System;
using System.Collections.Generic;
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

            Initialize();
        }

        private void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
