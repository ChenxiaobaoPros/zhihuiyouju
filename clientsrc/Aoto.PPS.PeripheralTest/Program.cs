using System;
using System.IO;
using System.Windows.Forms;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.Configuration;
using log4net;

namespace Aoto.PPS.PeripheralTest
{
    static class Program
    {
        private static ILog log = LogManager.GetLogger("app");

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            log.Debug("begin");
            string caption = String.Empty;

            try
            {
                log.DebugFormat("libDir = {0}", Config.PeripheralLibAbsolutePath);
                string envPath = Environment.GetEnvironmentVariable("PATH") + ";" + Config.PeripheralLibAbsolutePath;
                Environment.SetEnvironmentVariable("PATH", envPath);
                log.DebugFormat("EnvironmentVariable PATH = {0}", envPath);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Form formShell = (Form)AutofacContainer.ResolveNamed("scriptInvoker");
                Application.Run(formShell);
            }
            catch (Exception e)
            {
                log.Error("Launcher Main error", e);
                MessageBox.Show("应用程序运行发生错误，请联系管理员", caption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

            log.Debug("end");
            Environment.Exit(0);
        }
    }
}
