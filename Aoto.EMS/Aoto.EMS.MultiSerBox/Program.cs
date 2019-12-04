using Aoto.EMS.Common;
using Aoto.EMS.Infrastructure;
using Aoto.EMS.Infrastructure.Configuration;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace Aoto.EMS.MultiSerBox
{
    static class Program
    {
        private static ILog log = LogManager.GetLogger("app");
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            log.Debug("启动日志成功");
            //RequestJsonObject
            //ResponseJsonObjec

            string dir = Config.App.Peripheral.Value<string>("dir");

            string libPath = Path.Combine(Config.PeripheralAbsolutePath, dir, "BoardLib");

            log.InfoFormat("libDir = {0}", libPath);

            string dllPath = Path.Combine(Config.PeripheralAbsolutePath, dir);

            log.InfoFormat("dllDir = {0}", dllPath);

            string envPath = Environment.GetEnvironmentVariable("PATH") + ";" + dllPath + ";" + libPath;

            Environment.SetEnvironmentVariable("PATH", envPath);

            log.InfoFormat("EnvironmentVariable PATH = {0}", envPath);




            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form formShell = (Form)AutofacContainer.ResolveNamed("scriptInvoker");
            Application.Run(formShell);
        }
    }
}
