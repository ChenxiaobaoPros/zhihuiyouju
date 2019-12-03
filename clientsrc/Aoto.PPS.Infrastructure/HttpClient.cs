using System;
using System.IO;
using System.Net;
using System.Text;

using Aoto.PPS.Infrastructure.Configuration;
using Aoto.PPS.Infrastructure.Utils;
using log4net;
using System.Net.Cache;
using System.Windows.Forms;

namespace Aoto.PPS.Infrastructure
{
    public class IcbcInfos
    {
        private string tradeCode="";

        public string TradeCode
        {
            get { return tradeCode; }
            set { tradeCode = value; }
        }

        private string qmsIp="127.0.0.1";

        public string QmsIp
        {
            get { return qmsIp; }
            set { qmsIp = value; }
        }

        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

    }

    public class HttpClient
    {
        private static ILog log = LogManager.GetLogger(typeof(HttpClient));
        private const string contentType = "application/octet-stream; charset=utf-8";//"application/json"

        public static string Post(string path, string body)
        {
            return Post(Config.App.WebServer.Host, Config.App.WebServer.Port, Config.App.WebServer.ContextPath, Config.App.WebServer.Timeout, path, body);
        }

        public static string Post(string host, int port, string contextPath, int timeout, string path, string body)
        {
            log.DebugFormat("begin, args: path = {0}, body = {1}", path, body);

            byte[] data = CommonUtils.Encrypt(Encoding.UTF8.GetBytes(body), Config.App.AesKey);
            data = HttpClient.Post(host, port, contextPath, timeout, path, data);
            string rtn = String.Empty;

            if (null != data && data.Length > 0)
            {
                rtn = Encoding.UTF8.GetString(CommonUtils.Decrypt(data, Config.App.AesKey));
            }

            log.DebugFormat("end, return: {0}", rtn);
            return rtn;
        }

        /// <summary>
        /// 访问叫号机 http通讯封装类
        /// </summary>
        /// <param name="path"></param>
        /// <param name="icbcInfo"></param>
        /// <returns></returns>
        public static string Post(string path, IcbcInfos icbcInfo)
        {
            return Postsg(Config.App.WebServer.Host, Config.App.WebServer.Port, Config.App.WebServer.ContextPath, Config.App.WebServer.Timeout, path, icbcInfo);
        }

        /// <summary>
        /// 访问叫号机，异常返回空字符串
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="contextPath"></param>
        /// <param name="timeout"></param>
        /// <param name="path"></param>
        /// <param name="icbcInfo"></param>
        /// <returns></returns>
        public static string Postsg(string host, int port, string contextPath, int timeout, string path, IcbcInfos icbcInfo)
        {
            log.DebugFormat("begin, args: path = {0}, body = {1}", path, icbcInfo.Content);

   
            byte[]  data = HttpClient.Post(host, port, contextPath, timeout, path, icbcInfo);
            string rtn = String.Empty;

            if (null != data && data.Length > 0)
            {
                if (icbcInfo.TradeCode.Equals("logquery"))
                {
                    try
                    {

                        System.IO.File.WriteAllBytes(Path.Combine(Application.StartupPath, "boxlogs\\boxLog.zip"), data);

                        rtn = "{\"biom\":{\"head\":{\"retCode\":\"0\",\"retMsg\":\"成功\"},\"body\":{}}}";
                    }
                    catch (Exception e)
                    {
                        rtn = String.Empty;
                    }
                }
                else
                {
                    rtn = Encoding.UTF8.GetString(data);
                }

            }
            else
            {
                if (icbcInfo.TradeCode.Equals("logquery"))
                {
                    rtn = "{\"biom\":{\"head\":{\"retCode\":\"1\",\"retMsg\":\"不存在下载日志\"},\"body\":{}}}";
                }
            }

            log.DebugFormat("end, return: {0}", rtn);
            return rtn;
        }

        public static byte[] Post(string host, int port, string contextPath, int timeout, string path, IcbcInfos icbcInf)
        {
            HttpWebRequest request = null;
            Stream requestStream = null;
            WebResponse response = null;
            Stream responseStream = null;
            MemoryStream ms = null;

            try
            {
                //WebProxy proxyObject = new WebProxy(host, 8080);//代理

                UriBuilder builder = new UriBuilder("http", host, port);
                builder.Path = contextPath + path;

                request = (HttpWebRequest)HttpWebRequest.Create(builder.Uri.AbsoluteUri);
                request.Timeout = timeout;
                request.KeepAlive = false;
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = contentType;
                //
                //request.Proxy = proxyObject;//代理
                request.Accept = "*/*";
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);


                request.Headers.Add("tradeCode", icbcInf.TradeCode);
                request.Headers.Add("qmsIp", icbcInf.QmsIp);


                byte[] data = Encoding.UTF8.GetBytes(icbcInf.Content.Replace("\r\n", ""));

                request.ContentLength = data.Length;

                if (null != data && data.Length > 0)
                {
                    requestStream = request.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Close();
                }

                response = request.GetResponse();
                responseStream = response.GetResponseStream();

                int bufferSize = 2048;
                int readCount = 0;
                byte[] buffer = new byte[bufferSize];
                ms = new MemoryStream();

                while ((readCount = responseStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    ms.Write(buffer, 0, readCount);
                }

                AppState.Online = true;
                return ms.ToArray();
            }
            catch (WebException e)
            {
                if (WebExceptionStatus.ConnectFailure == e.Status || WebExceptionStatus.Timeout == e.Status)
                {
                    AppState.Online = false;
                }

                //throw;  抛出异常
                return null;
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }

                if (null != requestStream)
                {
                    requestStream.Close();
                }

                if (null != response)
                {
                    response.Close();
                }
            }
        }

       

        public static byte[] Post(string host, int port, string contextPath, int timeout, string path, byte[] data)
        {
            HttpWebRequest request = null;
            Stream requestStream = null;
            WebResponse response = null;
            Stream responseStream = null;
            MemoryStream ms = null;

            try
            {
                UriBuilder builder = new UriBuilder("http", host, port);
                builder.Path = contextPath + path;

                request = (HttpWebRequest)HttpWebRequest.Create(builder.Uri.AbsoluteUri);
                request.Timeout = timeout;
                request.KeepAlive = false;
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = contentType;

                request.Headers.Add("tradeCode", "qmssign");
                request.Headers.Add("qmsIp","192.168.43");

                //request.Credentials = new NetworkCredential(Config.App.WebServer.Username, Config.App.WebServer.Password);          

                if (null != data && data.Length > 0)
                {
                    requestStream = request.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                }
            
                response = request.GetResponse();
                responseStream = response.GetResponseStream();

                int bufferSize = 2048;
                int readCount = 0;
                byte[] buffer = new byte[bufferSize];
                ms = new MemoryStream();

                while ((readCount = responseStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    ms.Write(buffer, 0, readCount);
                }

                AppState.Online = true;
                return ms.ToArray();
            }
            catch (WebException e)
            {
                if (WebExceptionStatus.ConnectFailure == e.Status || WebExceptionStatus.Timeout == e.Status)
                {
                    AppState.Online = false;
                }

                throw;
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }

                if (null != requestStream)
                {
                    requestStream.Close();
                }

                if (null != response)
                {
                    response.Close();
                }
            }
        }

        public static byte[] Post(string path, byte[] data)
        {
            return Post(Config.App.WebServer.Host, Config.App.WebServer.Port, Config.App.WebServer.ContextPath, Config.App.WebServer.Timeout, path, data);
        }

        public static byte[] Get(string path, string query)
        {
            HttpWebRequest request = null;
            WebResponse response = null;
            Stream responseStream = null;
            MemoryStream ms = null;

            try
            {
                UriBuilder builder = new UriBuilder("http", Config.App.WebServer.Host, Config.App.WebServer.Port);
                builder.Path = Config.App.WebServer.ContextPath + path;
                builder.Query = query;

                request = (HttpWebRequest)HttpWebRequest.Create(builder.Uri.AbsoluteUri);
                request.Timeout = Config.App.WebServer.Timeout;
                request.KeepAlive = false;
                request.Method = WebRequestMethods.Http.Get;
                request.ContentType = contentType;
                request.Credentials = new NetworkCredential(Config.App.WebServer.Username, Config.App.WebServer.Password);

                response = request.GetResponse();
                responseStream = response.GetResponseStream();

                int bufferSize = 2048;
                int readCount = 0;
                byte[] buffer = new byte[bufferSize];
                ms = new MemoryStream();

                while ((readCount = responseStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    ms.Write(buffer, 0, readCount);
                }

                AppState.Online = true;
                return ms.ToArray();
            }
            catch (WebException e)
            {
                if (WebExceptionStatus.ConnectFailure == e.Status || WebExceptionStatus.Timeout == e.Status)
                {
                    AppState.Online = false;
                }

                throw;
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }

                if (null != response)
                {
                    response.Close();
                }
            }
        }

        public static void UploadFile(string path, string filename)
        {
            HttpWebRequest request = null;
            WebResponse response = null;
            Stream responseStream = null;
            Stream requestStream = null;
            FileStream fs = null;

            try
            {
                UriBuilder builder = new UriBuilder("http", Config.App.WebServer.Host, Config.App.WebServer.Port);
                builder.Path = Config.App.WebServer.ContextPath + path;

                request = (HttpWebRequest)HttpWebRequest.Create(builder.Uri.AbsoluteUri);
                request.Timeout = Config.App.WebServer.Timeout;
                request.KeepAlive = false;
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = contentType;
                request.Credentials = new NetworkCredential(Config.App.WebServer.Username, Config.App.WebServer.Password);

                fs = File.OpenRead(filename);
                int readCount = 0;
                int bufferSize = 2048;
                byte[] buffer = new byte[bufferSize];
                requestStream = request.GetRequestStream();

                while ((readCount = fs.Read(buffer, 0, bufferSize)) > 0)
                {
                    requestStream.Write(buffer, 0, readCount);
                }

                response = request.GetResponse();
                responseStream = response.GetResponseStream();
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }

                if (responseStream != null)
                {
                    responseStream.Close();
                }

                if (null != response)
                {
                    response.Close();
                }
            }
        }

        public static void DownloadFile(string host, int port, string contextPath, int timeout, string path, string filename)
        {
            HttpWebRequest request = null;
            WebResponse response = null;
            Stream responseStream = null;
            FileStream fs = null;

            try
            {
                UriBuilder builder = new UriBuilder("http", host, port);
                builder.Path = contextPath + path;

                request = (HttpWebRequest)HttpWebRequest.Create(builder.Uri.AbsoluteUri);
                request.Timeout = timeout;
                request.KeepAlive = false;
                request.Method = WebRequestMethods.Http.Get;
                request.ContentType = contentType;
                request.Credentials = new NetworkCredential(Config.App.WebServer.Username, Config.App.WebServer.Password);

                response = request.GetResponse();
                responseStream = response.GetResponseStream();

                int bufferSize = 2048;
                int readCount = 0;
                byte[] buffer = new byte[bufferSize];

                FileInfo fileInfo = new FileInfo(filename);

                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }

                fs = fileInfo.Create();

                while ((readCount = responseStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    fs.Write(buffer, 0, readCount);
                }
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }

                if (null != response)
                {
                    response.Close();
                }
            }
        }

        public static void DownloadFile(string path, string filename)
        {
            DownloadFile(Config.App.WebServer.Host, Config.App.WebServer.Port, Config.App.WebServer.ContextPath, Config.App.WebServer.Timeout, path, filename);
        }
    }
}
