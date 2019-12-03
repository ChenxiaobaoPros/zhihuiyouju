using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Peripheral;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.PeripheralTest
{
    [ComVisible(true)]
    public partial class FrmShell : Form, IScriptInvoker
    {
        private static ILog log = LogManager.GetLogger("app");
        private PeripheralManager peripheralManager;
        private bool closed = false;
        
        public FrmShell()
        {
            InitializeComponent();
            Text = "外设测试";
            TopMost = false;

            webBrowser.ObjectForScripting = this;
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.ScrollBarsEnabled = false;
        }

        /// <summary>
        /// $.aoto.pluginInvoke();
        /// {
        ///    "id" : "theme",
        ///    "method": "GetListByPageAsync",
        ///    "args" : "{}"
        /// }
        /// </summary>
        /// <param name="json"></param>
        public string PluginInvoke(string id, string method, string args)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return "{\"result\":1}";
            }

            if (String.IsNullOrWhiteSpace(method))
            {
                return "{\"result\":1}";
            }

            JObject jo = null;

            if (String.IsNullOrWhiteSpace(args))
            {
                jo = new JObject();
            }
            else
            {
                jo = JObject.Parse(args);
            }

            string sound = jo.Value<string>("sound");
            int index = 0;

            if (!String.IsNullOrWhiteSpace(sound))
            {
                index = sound.IndexOf(";");

                if (index > 0)
                {
                    string wav = sound.Substring(0, index);
                }
            }

            try
            {
                object o = AutofacContainer.ResolveNamed(id);
                o.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, o, new object[] { jo });
            }
            catch (Exception e)
            {
                jo["result"] = ErrorCode.Failure;
                log.ErrorFormat("PluginInvoke error, args: id = {0}, method = {1}, args = {2}\r\n{3}", id, method, args, e);
            }

            return jo.ToString(Formatting.None);
        }

        private void FrmShellLoad(object sender, EventArgs e)
        {
            peripheralManager = AutofacContainer.ResolveNamed<PeripheralManager>("peripheralManager");
            webBrowser.Navigate(Path.Combine(Application.StartupPath, "web\\test.html"));
            peripheralManager.Caller.ReadAsync(new JObject());
        }

        private void FrmShellClosed(object sender, FormClosedEventArgs e)
        {
            closed = true;
            peripheralManager.Dispose();
        }

        public void Shut()
        {
            Invoke(new MethodInvoker(delegate()
            {
                this.Close();
            }));
        }

        public void Navigate(string url)
        {
            Invoke(new MethodInvoker(delegate()
            {
                webBrowser.Navigate(url);
            }));
        }

        public void ScriptInvoke(JObject jo)
        {
            log.DebugFormat("closed = {0}", closed);
            if (closed)
            {
                return;
            }

            int result = jo.Value<int>("result");
            string callback = jo.Value<string>("callback");
            string sound = jo.Value<string>("sound");

            jo.Remove("mask");
            jo.Remove("sound");
            //jo.Remove("callback");

            int index = 0;

            if (!String.IsNullOrWhiteSpace(sound))
            {
                index = sound.IndexOf(";");

                if (index >= 0)
                {
                    string play = sound.Substring(index + 1);
                    string[] arr = play.Split(',');

                    foreach (string s in arr)
                    {
                        if (!s.Contains(":"))
                        {
                            continue;
                        }

                        string[] ar = s.Split(':');

                        if (result.ToString().Equals(ar[0]))
                        {
                            break;
                        }
                    }
                }
            }

            if (!closed)
            {
                try
                {
                    Invoke(new MethodInvoker(delegate()
                    {
                        if (!String.IsNullOrWhiteSpace(callback))
                        {
                            webBrowser.Document.InvokeScript(callback, new object[] { jo.ToString(Formatting.None) });
                        }
                    }));
                }
                catch (Exception e)
                {
                    log.Error("webBrowser InvokeScript error", e);
                }
            }
        }
    }
}