using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Aoto.PPS.Infrastructure.Configuration;
using log4net;
using Aoto.PPS.Infrastructure.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Aoto.PPS.Infrastructure
{
    public class HttpServer
    {
        private HttpListener listener;
        private Thread thread;
        private static readonly ILog log = LogManager.GetLogger("app");
        private string basicKey;

        public HttpServer()
        {
            byte[] bytes = Encoding.Default.GetBytes(Config.App.WebServer.Username + ":" + Config.App.WebServer.Password);
            basicKey = Convert.ToBase64String(bytes);

            listener = new HttpListener();
            string prefix = "http://" + Config.App.RunMode.WebServer.Host + ":" + Config.App.RunMode.WebServer.Port + Config.App.RunMode.WebServer.ContextPath + "/";
            listener.Prefixes.Add(prefix);
            listener.AuthenticationSchemes = AuthenticationSchemes.Basic;

            log.DebugFormat("listener.Prefixes.Add : {0}", prefix);
            listener.AuthenticationSchemeSelectorDelegate = new AuthenticationSchemeSelector(delegate(HttpListenerRequest request)
            {
                string auth = request.Headers["Authorization"];

                if (!String.IsNullOrEmpty(auth))
                {
                    if (basicKey != auth.TrimStart("Baisc ".ToCharArray()))
                    {
                        return AuthenticationSchemes.None;
                    }
                }

                return AuthenticationSchemes.Basic;
            });

            thread = new Thread(Run);
            thread.IsBackground = true;
        }

        public void Start()
        {
            thread.Start();
        }

        public void Stop()
        {
            listener.Stop();
            listener.Close();
        }

        private void Run()
        {
            try
            {
                listener.Start();

                while (listener.IsListening)
                {
                    HttpListenerContext context = listener.GetContext();
                    log.DebugFormat("HttpServer accept a request {0}", context.Request.RawUrl);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(HandleRequest), context);
                }
            }
            catch (Exception e)
            {
                log.Error("HttpListener thread run error", e);
            }
        }

        private void ConfigSync(HttpListenerContext context)
        {
            FileStream fs = null;

            try
            {
                string fileName = Path.Combine(Config.PatchAbsolutePath, "data" + DateTime.Now.ToString("yyyyMMdd") + ".zip");

                if (!File.Exists(fileName))
                {
                    log.DebugFormat("fileName = {0} not exist", fileName);
                    context.Response.StatusCode = 500;
                    return;
                }

                fs = File.OpenRead(fileName);
                int readCount = 0;
                int bufferSize = 2048;
                byte[] buffer = new byte[bufferSize];

                while ((readCount = fs.Read(buffer, 0, bufferSize)) > 0)
                {
                    context.Response.OutputStream.Write(buffer, 0, readCount);
                }

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/octet-stream; charset=utf-8";
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.OutputStream.Flush();
            }
            finally
            {
                if (null != fs)
                {
                    fs.Close();
                }
            }
        }

        private void Response(HttpListenerContext context, string id, string method)
        {
            int bufferSize = 2048;
            int readCount = 0;
            byte[] buffer = new byte[bufferSize];
            MemoryStream ms = new MemoryStream();

            while ((readCount = context.Request.InputStream.Read(buffer, 0, bufferSize)) > 0)
            {
                ms.Write(buffer, 0, readCount);
            }

            if (ms.Length > 0)
            {
                string rtn = Encoding.UTF8.GetString(CommonUtils.Decrypt(ms.ToArray(), Config.App.AesKey));
                JObject jo = JObject.Parse(rtn);
                jo["runMode"] = "slave";

                object o = AutofacContainer.ResolveNamed(id);
                o.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, o, new object[] { jo });

                string responseData = jo.ToString(Formatting.None);
                byte[] data = CommonUtils.Encrypt(Encoding.UTF8.GetBytes(responseData), Config.App.AesKey);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/octet-stream; charset=utf-8";
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.OutputStream.Write(data, 0, data.Length);
                context.Response.OutputStream.Flush();
            }
        }

        private void HandleRequest(object state)
        {
            HttpListenerContext context = null;

            try
            {
                context = state as HttpListenerContext;

                if (null == context)
                {
                    return;
                }

                string path = context.Request.Url.AbsolutePath;
                string p = String.Empty;

                p = Config.App.RunMode.WebServer.ContextPath + "/services/configs/sync";

                if (p.Equals(path, StringComparison.OrdinalIgnoreCase))
                {
                    ConfigSync(context);
                }
                else
                {
                    foreach (AppConfig.SvrMapping m in Config.App.RunMode.Mappings)
                    {
                        p = Config.App.RunMode.WebServer.ContextPath + m.Path;

                        if (p.Equals(path, StringComparison.OrdinalIgnoreCase))
                        {
                            Response(context, m.Id, m.Method);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("handle http request error", e);
            }
            finally
            {
                try
                {
                    if (null != context && null != context.Response)
                    {
                        context.Response.Close();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("context.Response.Close() error", ex);
                }
            }
        }
    }
}