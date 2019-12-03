using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;
using log4net;

namespace Aoto.PPS.Peripheral.Default
{
    public class IndicateorLight : IIndicateorLight
    {
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int InitDevice(StringBuilder companyName, StringBuilder hardwareVersion);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int ControlLight(int lightNo, int type);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int OpenDevice();

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int CloseDevice();

        private static readonly ILog log = LogManager.GetLogger("peripheral"); 
        private IntPtr ptr;
        private InitDevice initDevice;
        private ControlLight controlLight;
        private OpenDevice openDevice;
        private CloseDevice closeDevice;

        private string dll;
        private bool enabled;
        private bool cancelled;
        private bool isBusy;

        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public IndicateorLight(string dll, int timeout, bool enabled)
        {
            isBusy = false;
            cancelled = false;

            this.dll = dll;
            this.enabled = enabled;

            Initialize();
        }

        public void Initialize()
        {
            log.Debug("begin");

            string dllPath = Path.Combine(Config.AppRoot, dll);
            ptr = Win32ApiInvoker.LoadLibrary(dllPath);

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "InitDevice");
            initDevice = (InitDevice)Marshal.GetDelegateForFunctionPointer(api, typeof(InitDevice));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = InitDevice", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "ControlLight");
            controlLight = (ControlLight)Marshal.GetDelegateForFunctionPointer(api, typeof(ControlLight));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = ControlLight", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "OpenDevice");
            openDevice = (OpenDevice)Marshal.GetDelegateForFunctionPointer(api, typeof(OpenDevice));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = OpenDevice", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "CloseDevice");
            closeDevice = (CloseDevice)Marshal.GetDelegateForFunctionPointer(api, typeof(CloseDevice));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = CloseDevice", ptr);

            StringBuilder sbCompany = new StringBuilder(128);
            StringBuilder sbHardwareVersion = new StringBuilder(128);

            int code = initDevice(sbCompany, sbHardwareVersion);
            log.DebugFormat("invoke {0} -> InitDevice, args: company = {1}, hardwareVersion = {2}, return = {3}",
                dll, sbCompany, sbHardwareVersion, code);
            log.Debug("end");
        }

        void IIndicateorLight.ControlLight(int lightNo, int lightType)
        {
            log.DebugFormat("begin, args: lightNo = {0}, type = {1}", lightNo, lightType);

            isBusy = true;
            cancelled = false;
            int code = openDevice();
            log.DebugFormat("invoke {0} -> OpenDevice, return = {1}", dll, code);

            if (0 == code)
            {
                code = controlLight(lightNo, lightType);
                log.DebugFormat("invoke {0} -> ControlLight, return = {1}", dll, code);
            }

            code = closeDevice();
            log.DebugFormat("invoke {0} -> CloseDevice, return = {1}", dll, code);
            log.Debug("end");
            isBusy = false;
        }

        public int GetStatus()
        {
            log.Debug("begin");

            if (!enabled)
            {
                log.DebugFormat("end, return = {0}", StatusCode.Disabled);
                return StatusCode.Disabled;
            }

            if (isBusy)
            {
                log.DebugFormat("end, return = {0}", StatusCode.Busy);
                return StatusCode.Busy;
            }

            int code = openDevice();
            log.DebugFormat("invoke {0} -> OpenDevice, return = {1}", dll, code);

            int state = (0 == code) ? StatusCode.Normal : StatusCode.Offline;

            code = closeDevice();
            log.DebugFormat("invoke {0} -> CloseDevice, return = {1}", dll, code);
            log.DebugFormat("end, return = {0}", state);
            return state;
        }

        public void Dispose()
        {
            log.Debug("begin");

            if (IntPtr.Zero != ptr)
            {
                Win32ApiInvoker.FreeLibrary(ptr);
                log.DebugFormat("FreeLibrary: ptr = {0}", ptr);
            }

            log.Debug("end");
        }
    }
}