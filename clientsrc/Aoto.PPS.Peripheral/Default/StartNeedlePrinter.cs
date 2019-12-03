using System;
using System.IO;
using System.Runtime.InteropServices;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;
using log4net;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Peripheral.Default
{
    public class StartNeedlePrinter : NeedlePrinter
    {
        //获得打印机状态
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int GetPrinterStatusCaller();

        // 退纸
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        private delegate int PaperBackCaller();

        // 清除
        //[UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        //private delegate int RebootPrinterStatusCaller();

        //private LocalPrintServer lps;

        private GetPrinterStatusCaller getPrinterStatusCaller;
        private PaperBackCaller paperBackCaller;
        //private RebootPrinterStatusCaller rebootPrinterStatusCaller;
        private static readonly ILog log = LogManager.GetLogger("printer"); 

        public StartNeedlePrinter() : base()
        {

        }

        public override void Initialize()
        {
            log.Debug("begin");

            //lps = new LocalPrintServer(PrintSystemDesiredAccess.AdministrateServer);

            string dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, dll);

            if (!File.Exists(dllPath))
            {
                dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "lib", dll);
            }

            ptr = Win32ApiInvoker.LoadLibrary(dllPath);
            log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, ptr);

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "GetUSBStatus");
            getPrinterStatusCaller = (GetPrinterStatusCaller)Marshal.GetDelegateForFunctionPointer(api, typeof(GetPrinterStatusCaller));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = GetUSBStatus", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "PaperBack");
            paperBackCaller = (PaperBackCaller)Marshal.GetDelegateForFunctionPointer(api, typeof(PaperBackCaller));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = PaperBack", ptr);

            //api = Win32ApiInvoker.GetProcAddress(ptr, "RebootPrinterStatus");
            //rebootPrinterStatusCaller = (RebootPrinterStatusCaller)Marshal.GetDelegateForFunctionPointer(api, typeof(RebootPrinterStatusCaller));
            //log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = RebootPrinterStatus", ptr);

            log.Debug("end");
        }

        public override void Execute(int command)
        {
            log.DebugFormat("begin, args: command = {0}", command);

            // 退纸
            if (1 == command)
            {
                int state = GetStatus();

                if (11 == state)
                {
                    paperBackCaller();
                }
            }
            else if (2 == command) // 清除队列缓存
            {
                //lps.Refresh();
                //PrintJobInfoCollection pjc = lps.DefaultPrintQueue.GetPrintJobInfoCollection();

                //foreach (var p in pjc)
                //{
                //    p.Cancel();
                //}

                //rebootPrinterStatusCaller();
            }

            log.Debug("end");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 0：禁用；1：正常；2：忙；3：离线；
        /// 10：空闲未装载纸；11：空闲已装载纸，准备就绪
        /// </returns>
        public override int GetStatus()
        {
            log.Debug("begin");

            if (!enabled)
            {
                log.DebugFormat("end, return = {0}", StatusCode.Disabled);
                return StatusCode.Disabled;
            }

            int s = getPrinterStatusCaller();
            log.DebugFormat("invoke {0} -> GetPrinterStatus, return = {1}", dll, s);

            int result = -1;

            switch (s)
            {
                case 24:    // 打印机就绪
                    result = 11;
                    break;
                case 25:    // 打印机忙
                    result = 2;
                    break;
                case 48:    // 打印机未装纸
                    result = 10;
                    break;
                case -1:    // 离线
                case 0:     // 发送请求状态函数失败
                default: 
                    result = 3;
                    break;
            }

            log.DebugFormat("end, return = {0}", result);
            return result;
        }
    }
}
