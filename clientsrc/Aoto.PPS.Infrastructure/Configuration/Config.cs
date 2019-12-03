using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Infrastructure.Configuration
{
    //本地文件数据读取
    public class Config
    {
        private static AppConfig appConfig;

        private const string appFileName = "app.json";
        public const string ConfigRelativePath = "config";
        public const string LauncherExe = "Aoto.PPS.Launcher.exe";
        public const string UpdateExe = "Aoto.PPS.Update.exe";

        //根路径
        public static readonly string AppRoot = Application.StartupPath;

        public static readonly string LauncherExeAbsolutePath = Path.Combine(AppRoot, LauncherExe);
        public static readonly string UpdateExeAbsolutePath = Path.Combine(AppRoot, UpdateExe);
        public static readonly string PatchAbsolutePath = Path.Combine(AppRoot, "patch");
        public static readonly string PeripheralAbsolutePath = Path.Combine(AppRoot, "peripheral");
        public static readonly string AppIconFilePath = Path.Combine(AppRoot, "web\\images\\logo.ico");

        private static readonly string configAbsolutePath = Path.Combine(AppRoot, ConfigRelativePath);
        public static readonly string AppConfigAbsoluteFilePath = Path.Combine(configAbsolutePath, appFileName);
        public static readonly string AdvConfigAbsoluteFilePath = Path.Combine(configAbsolutePath, "advs.json");
        public static readonly string SeqFilePath = Path.Combine(AppRoot, "data\\seq");
        public static readonly string VersionPath = Path.Combine(Config.AppRoot, "version");

        public static AppConfig App { get { return appConfig; } }
        private static Dictionary<string, string> dic;
        private static string appConfigString;
        private static string advConfigString;

        private Config()
        {

        }

        static Config()
        {
            dic = new Dictionary<string, string>();

            if (File.Exists(AdvConfigAbsoluteFilePath))
            {
                advConfigString = File.ReadAllText(AdvConfigAbsoluteFilePath, Encoding.UTF8);
            }

            if (File.Exists(AppConfigAbsoluteFilePath))
            {
                //读取配置
                appConfigString = File.ReadAllText(AppConfigAbsoluteFilePath, Encoding.UTF8);
                //反序列化
                appConfig = JsonConvert.DeserializeObject<AppConfig>(appConfigString);
            }
        }

        public static string GetAdvConfig()
        {
            return advConfigString;
        }

        public static string GetPrintConfig(string groupType, string receiptType)
        {
            string key = groupType + "\\" + receiptType + ".json";
            
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }

            string s = File.ReadAllText(Path.Combine(configAbsolutePath, key), Encoding.UTF8);
            dic[key] = s;
            return s;
        }

        public static void SavePrintConfig(string bizType, string receiptType, JToken config)
        {
            JObject ne = (JObject)config;
            string jsonFileName = bizType + "\\" + receiptType + ".json";
            string jsonFilePath = Path.Combine(configAbsolutePath, jsonFileName);

            JObject old = JObject.Parse(File.ReadAllText(jsonFilePath, Encoding.UTF8));
            JToken jtItem = null;

            if (ne.TryGetValue("items1", out jtItem))
            {
                old["items1"] = jtItem;
            }
            else if (ne.TryGetValue("items2", out jtItem))
            {
                old["items2"] = jtItem;
            }
            else if (ne.TryGetValue("items3", out jtItem))
            {
                old["items3"] = jtItem;
            }
            else if (ne.TryGetValue("items", out jtItem))
            {
                old["items"] = jtItem;
            }
            else if (ne.TryGetValue("backItems", out jtItem))
            {
                old["backItems"] = jtItem;
            }

            JToken jtOffset = null;

            if (ne.TryGetValue("offset1", out jtOffset))
            {
                old["offset1"] = jtOffset;
            }
            else if (ne.TryGetValue("offset2", out jtOffset))
            {
                old["offset2"] = jtOffset;
            }
            else if (ne.TryGetValue("offset3", out jtOffset))
            {
                old["offset3"] = jtOffset;
            }
            else if (ne.TryGetValue("offset", out jtOffset))
            {
                old["offset"] = jtOffset;
            }
            else if (ne.TryGetValue("backOffset", out jtOffset))
            {
                old["backOffset"] = jtOffset;
            }

            string text = old.ToString();
            File.WriteAllText(jsonFilePath, text, Encoding.UTF8);
            dic[jsonFileName] = text;
        }

        public static string GetAppConfig()
        {
            return appConfigString;
        }

        public static void SaveAppConfig(string json)
        {
            JObject jo = JObject.Parse(json);
            SaveAppConfig(jo);
        }

        public static void SaveAppConfig(JToken ne)
        {
            JObject old = JObject.Parse(File.ReadAllText(Config.AppConfigAbsoluteFilePath, Encoding.UTF8));

            //获取设备信息
            if (null != ne["device"])
            {
                int deviceId = ne["device"].Value<int>("id");
                if (deviceId > 0)
                {
                    old["device"]["id"] = deviceId;
                }

                int orgId = ne["device"].Value<int>("orgId");
                if (orgId > 0)
                {
                    old["device"]["orgId"] = orgId;
                }

                string deviceNo = ne["device"].Value<string>("no");
                if (!String.IsNullOrWhiteSpace(deviceNo))
                {
                    old["device"]["no"] = deviceNo;
                }

                string orgCode = ne["device"].Value<string>("orgCode");
                if (!String.IsNullOrWhiteSpace(orgCode))
                {
                    old["device"]["orgCode"] = orgCode;
                }
            }
            //获取控件信息
            if (null != ne["webServer"])
            {
                string webServerHost = ne["webServer"].Value<string>("host").Trim();
                if (!String.IsNullOrWhiteSpace(webServerHost))
                {
                    old["webServer"]["host"] = webServerHost;
                }

                int webServerPort = ne["webServer"].Value<int>("port");
                if (webServerPort > 0)
                {
                    old["webServer"]["port"] = webServerPort;
                }
            }
            //客户记录
            if (null != ne["custRec"])
            {
                bool custRecOnline = ne["custRec"].Value<bool>("online");
                old["custRec"]["online"] = custRecOnline;
                string custRecUrl = ne["custRec"].Value<string>("url");
                if (!String.IsNullOrWhiteSpace(custRecUrl))
                {
                    old["custRec"]["url"] = custRecUrl;
                }
            }

            if (null != ne["runMode"])
            {
                string runMode = ne["runMode"].Value<string>("model");
                if (!String.IsNullOrWhiteSpace(runMode))
                {
                    old["runMode"]["model"] = runMode;
                }

                if (null != ne["runMode"]["webServer"])
                {
                    string host = ne["runMode"]["webServer"].Value<string>("host");
                    if (!String.IsNullOrWhiteSpace(host))
                    {
                        old["runMode"]["webServer"]["host"] = host;
                    }

                    int port = ne["runMode"]["webServer"].Value<int>("port");
                    if (port > 0)
                    {
                        old["runMode"]["webServer"]["port"] = port;
                    }
                }
            }
            //  延后开机时间 秒
            if (null != ne["DelayedStartTime"])
            {
               string delayedStartTime=  ne.Value<string>("DelayedStartTime");

               if (!String.IsNullOrWhiteSpace(delayedStartTime))
               {
                   old["DelayedStartTime"] = delayedStartTime;
               }
            }

            //  号票打印模板更新时间
            if (null != ne["tickePrintModTime"])
            {
                string tickePrintModTime = ne.Value<string>("tickePrintModTime");

                if (!String.IsNullOrWhiteSpace(tickePrintModTime))
                {
                    old["tickePrintModTime"] = tickePrintModTime;
                }
            }

            appConfigString = old.ToString();
            File.WriteAllText(Config.AppConfigAbsoluteFilePath, appConfigString, Encoding.UTF8);
        }
    }

    //数据库连接字符串
    public static class ConnectionStrings
    {
        private static readonly string bizDb;
        private static readonly string configDb;
        private static readonly string logDb;
        private static readonly string queueDb;
        private static readonly string customerDb;
        private static readonly string historyDb;

        /// <summary>
        /// 链接字符串
        /// </summary>
        static ConnectionStrings()
        {
            bizDb = "Data Source=" + Path.Combine(Config.AppRoot, "data\\biz.db") + ";Version=3;Pooling=true;Max Pool Size=10;";
            configDb = "Data Source=" + Path.Combine(Config.AppRoot, "data\\config.db") + ";Version=3;Pooling=true;Max Pool Size=10;";
            logDb = "Data Source=" + Path.Combine(Config.AppRoot, "data\\log.db") + ";Version=3;Pooling=true;Max Pool Size=10;";
            queueDb = "Data Source=" + Path.Combine(Config.AppRoot, "data\\queue.db") + ";Version=3;Pooling=true;Max Pool Size=10;";
            customerDb = "Data Source=" + Path.Combine(Config.AppRoot, "data\\customer.db") + ";Version=3;Pooling=true;Max Pool Size=10;";
            historyDb = "Data Source=" + Path.Combine(Config.AppRoot, "data\\history.db") + ";Version=3;Pooling=true;Max Pool Size=2;";
        }

        public static string BizDb { get { return bizDb; } }
        public static string ConfigDb { get { return configDb; } }
        public static string LogDb { get { return logDb; } }
        public static string QueueDb { get { return queueDb; } }
        public static string CustomerDb { get { return customerDb; } }
        public static string HistoryDb { get { return historyDb; } }
    }
}