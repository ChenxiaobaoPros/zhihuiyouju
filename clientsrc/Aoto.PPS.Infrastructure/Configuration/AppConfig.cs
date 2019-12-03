using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
namespace Aoto.PPS.Infrastructure.Configuration
{
    /// <summary>
    /// 配置app信息
    /// </summary>
    public class AppConfig
    {
        private string name;
        private Dev device;
        private int heartbeatInterval;
        private Svr webServer;
        private string aesKey = "1234567890abcdef";
        private bool online;
        private bool topMost;
        private int timeout;
        private int ticketTimeout;
        private int ppsPopUpTimeout;
        private int mode;
        private WB wb;
        private string version;
        private JObject peri;
        private float opacity;
        private CustRecognition custRec;
        private RunMod runMode;
        private string callLang;

        private int delayedStartTime;


        private string tickePrintModTime;


        private int delLogDate = 1;

        public int DelLogDate
        {
            get { return delLogDate; }
            set { delLogDate = value; }
        }

        public string TickePrintModTime
        {
            get { return tickePrintModTime; }
            set { tickePrintModTime = value; }
        }

        public int DelayedStartTime
        {
            get { return delayedStartTime; }
            set { delayedStartTime = value; }
        }

        public string Name { get { return name; } set { name = value; } }
        public Dev Device { get { return device; } set { device = value; } }
        public int HeartbeatInterval { get { return heartbeatInterval; } set { heartbeatInterval = value; } }
        public Svr WebServer { get { return webServer; } set { webServer = value; } }
        public string AesKey { get { return aesKey; } }
        public bool Online { get { return online; } set { online = value; } }
        public bool TopMost { get { return topMost; } set { topMost = value; } }
        public int Timeout { get { return timeout; } set { timeout = value; } }
        public int Mode { get { return mode; } set { mode = value; } }
        public int TicketTimeout { get { return ticketTimeout; } set { ticketTimeout = value; } }
        public int PpsPopUpTimeout { get { return ppsPopUpTimeout; } set { ppsPopUpTimeout = value; } }
        public WB WebBrowser { get { return wb; } set { wb = value; } }
        public string Version { get { return version; } set { version = value; } }
        public JObject Peripheral { get { return peri; } set { peri = value; } }
        public float Opacity { get { return opacity; } set { opacity = value; } }
        public CustRecognition CustRec { get { return custRec; } set { custRec = value; } }
        public RunMod RunMode { get { return runMode; } set { runMode = value; } }
        public string CallLang { get { return callLang; } set { callLang = value.ToUpper(); } }

        public class Dev
        {
            private int id;
            private int orgId;
            private string no;
            private string orgCode;
            private string orgName;
            //供应商
            private string supplier;

            public int Id { get { return id; } set { id = value; } }
            public int OrgId { get { return orgId; } set { orgId = value; } }
            public string No { get { return no; } set { no = value; } }
            public string OrgCode { get { return orgCode; } set { orgCode = value; } }
            public string OrgName { get { return orgName; } set { orgName = value; } }
            public string Supplier { get { return supplier; } set { supplier = value; } }
        }

        public class WB
        {
            private bool scriptErrorsSuppressed;
            private bool scrollBarsEnabled;

            public bool ScriptErrorsSuppressed { get { return scriptErrorsSuppressed; } set { scriptErrorsSuppressed = value; } }
            public bool ScrollBarsEnabled { get { return scrollBarsEnabled; } set { scrollBarsEnabled = value; } }
        }

        public class Svr
        {
            private string host;
            private int port;
            private string contextPath;
            private string username = "aoto";
            private string password = "aoto123";
            private int timeout;

            public string Host { get { return host; } set { host = value; } }
            public int Port { get { return port; } set { port = value; } }
            public string ContextPath { get { return contextPath; } set { contextPath = value; } }
            public string Username { get { return username; } }
            public string Password { get { return password; } }
            public int Timeout { get { return timeout; } set { timeout = value; } }
        }

        public class CustRecognition
        {
            private bool online;
            private string url;

            public bool Online { get { return online; } set { online = value; } }
            public string Url { get { return url; } set { url = value; } }
        }

        public class RunMod
        {
            private string model;
            private Svr webServer;
            private SvrMapping[] mappings;

            public string Model { get { return model; } set { model = value; } }
            public Svr WebServer { get { return webServer; } set { webServer = value; } }
            public SvrMapping[] Mappings { get { return mappings; } set { mappings = value; } }
        }

        public class SvrMapping
        {
            private string path;
            private string id;
            private string method;

            public string Path { get { return path; } set { path = value; } }
            public string Id { get { return id; } set { id = value; } }
            public string Method { get { return method; } set { method = value; } }
        }
    }
}
