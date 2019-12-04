using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Aoto.EMS.Infrastructure;
using Aoto.EMS.Infrastructure.ComponentModel;
using Aoto.EMS.Infrastructure.Configuration;
using log4net;
using Newtonsoft.Json.Linq;

namespace Aoto.EMS.Peripheral
{
    public class ReadQRCode : IQRCode
    {
        public delegate int P_HID_POS_RECEIVE_NOTIFY(String data, int len, String noused, String lpparam); //定义一个委托

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int open_hid_ex(P_HID_POS_RECEIVE_NOTIFY callback, String lpparam);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int close_hid(IntPtr intPtr);

        private open_hid_ex open_Hid_Ex;
        private close_hid close_Hid;

        private P_HID_POS_RECEIVE_NOTIFY callback;

        private static readonly ILog log = LogManager.GetLogger("readQRCode");
        private IScriptInvoker scriptInvoker;
        private IntPtr intPtr;
        private IntPtr openApi;
        private IntPtr CcloseApi;

        protected string name;
        protected string dll;
        protected int timeout;

        protected bool enabled;
        protected bool isBusy;
        protected bool cancelled;

        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }

        public ReadQRCode()
        {
            isBusy = false;
            cancelled = false;

            this.dll = Config.App.Peripheral["readQRCode"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["readQRCode"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["readQRCode"].Value<bool>("enabled");
            this.name = Config.App.Peripheral["readQRCode"].Value<string>("name");

            callback = new P_HID_POS_RECEIVE_NOTIFY(ShowMessage);
            scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
            Initialize();
        }



        public void Dispose()
        {
        }

        public int GetStatus()
        {
            return 0;
        }
        public int CloseQRCode()
        {
            int ret = close_Hid(intPtr);
            return ret;
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
                dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "readQRCodeLib", dll);
            }

            intPtr = Win32ApiInvoker.LoadLibrary(dllPath);
            log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, intPtr);

            uint idcoed = Win32ApiInvoker.GetLastError();
            openApi = Win32ApiInvoker.GetProcAddress(intPtr, "open_hid_ex");
            open_Hid_Ex = (open_hid_ex)Marshal.GetDelegateForFunctionPointer(openApi, typeof(open_hid_ex));

            CcloseApi = Win32ApiInvoker.GetProcAddress(intPtr, "close_hid");
            close_Hid = (close_hid)Marshal.GetDelegateForFunctionPointer(CcloseApi, typeof(close_hid));

            //IntPtr ptr = Marshal.AllocHGlobal(125);
            open_Hid_Ex(callback, null);
        }
        public int ShowMessage(String data, int len, String noused, String lpparam)
        {
            JObject jo = new JObject();
            jo["retCode"] = 0;
            jo["data"] = data;
            jo["callback"] = "getQRCodeData";
            scriptInvoker.ScriptInvoke(jo);

            return 0;
        }
    }
}
