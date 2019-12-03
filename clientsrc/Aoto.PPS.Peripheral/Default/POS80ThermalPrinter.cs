using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;
using log4net;
using Newtonsoft.Json.Linq;
using System.Management;

namespace Aoto.PPS.Peripheral.Default
{
    public class POS80ThermalPrinter : ThermalPrinter
    {
        private static readonly ILog log = LogManager.GetLogger("printer");
        private const string searchQuery = "SELECT * FROM Win32_PrintJob";

        public POS80ThermalPrinter() : base()
        {

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

            ManagementObjectSearcher searchPrintJobs = new ManagementObjectSearcher(searchQuery);
            ManagementObjectCollection prntJobCollection = searchPrintJobs.Get();

            string jobName = String.Empty;
            int result = 11;

            foreach (ManagementObject prntJob in prntJobCollection)
            {
                jobName = prntJob.Properties["Name"].Value.ToString();
                log.InfoFormat("jobName = {0}", jobName);

                if (jobName.StartsWith(base.name, StringComparison.OrdinalIgnoreCase))
                {
                    result = 10;
                    break;
                }
            }

            log.InfoFormat("result = {0}", result);
            log.Debug("end");
            return result;
        }

        protected override void WaitCompleted(JObject jo)
        {
            int status = 0;
            long start = DateTime.Now.Ticks;
            TimeSpan elapsedSpan = TimeSpan.Zero;

            while (true)
            {
                Thread.Sleep(100);

                status = GetStatus();

                if (10 != status)
                {
                    jo["result"] = ErrorCode.Success;
                    break;
                }

                if (TimeSpan.FromTicks(DateTime.Now.Ticks - start).TotalMilliseconds >= timeout)
                {
                    jo["result"] = 10;
                    break;
                }
            }
        }
    }
}