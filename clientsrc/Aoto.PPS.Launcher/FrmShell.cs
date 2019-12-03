using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;

using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Aoto.CQMS.Core.Application;
using System.Collections.Generic;
using System.Text;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.Utils;
using System.Runtime.Remoting.Messaging;
using Microsoft.VisualBasic.Devices;
using System.Net.NetworkInformation;


namespace Aoto.PPS.Launcher
{
    [ComVisible(true)]
    public partial class FrmShell : Form, IScriptInvoker
    {
        //界面日志
        private static ILog log = LogManager.GetLogger("app");
        //js的日志
        private static ILog log2JS = LogManager.GetLogger("js");
        //外设日志
        private static ILog log2Job = LogManager.GetLogger("job");

        private System.Threading.Timer idleTimer;
        private bool closed = false;

        //记录界面模板路径
        protected static readonly string qmsPath = Path.Combine(Config.AppRoot, "config\\q\\1");

        /// <summary>
        /// 用户卡操作 true-插入  false-取出
        /// </summary>
        private static bool userCardFlag = false;
        /// <summary>
        /// 音频
        /// </summary>
        Audio sysAudio = null;

        /// <summary>
        /// 设备逻辑名
        /// </summary>
        private readonly static string LOGICALNAME = "MifareCardReader";

        /// <summary>
        /// 磁道信息
        /// </summary>
        private readonly static string TRACKMAP = "1,2,3,CHIP";

        /// <summary>
        /// 读卡超时时间， 0-一直存在
        /// </summary>
        private readonly static int TIMEOUT = 0;
        /// <summary>
        /// 取号业务
        /// </summary>
        ICustgetseqService custgetseqService = null;
        /// <summary>
        /// 预定业务
        /// </summary>
        IReservationService reservationService = null;
        /// <summary>
        /// 联动关机服务
        /// </summary>
        ILinkageshutdownService linkageshutdownService = null;

        public static bool isBusy;
        private bool enabled = true;
        private bool cancelled;

        // private RunAsyncCaller openAsyncCard;
        //通用异步
        private RunAsyncCaller sysCommandAsyncCard;
        //打印异步
        private RunAsyncCaller printAsyncOCX;
        //声音异步
        private RunAsyncCaller playSoundAsync;
        //视频异步
        private RunAsyncCaller playPingAsync;

        /// <summary>
        /// 首页判断标志 true-首页，false-非首页
        /// </summary>
        private static bool popEnvent = true;

        public FrmShell()
        {
            InitializeComponent();
            cancelled = false;
            Text = Config.App.Name + " " + Config.App.Version;
            TopMost = Config.App.TopMost;

            sysAudio = new Audio();

            webBrowser.ObjectForScripting = this;
            webBrowser.ScriptErrorsSuppressed = Config.App.WebBrowser.ScriptErrorsSuppressed;
            webBrowser.ScrollBarsEnabled = Config.App.WebBrowser.ScrollBarsEnabled;

            //openAsyncCard = new RunAsyncCaller(ReadRawData);

            sysCommandAsyncCard = new RunAsyncCaller(SysCommandImpl);

            printAsyncOCX = new RunAsyncCaller(Print);

            playSoundAsync = new RunAsyncCaller(PlaySound);

            // playPingAsync = new RunAsyncCaller(PlayPing);

            Open2OCX();

            if (File.Exists(Config.AppIconFilePath))
            {
                Icon = new Icon(Config.AppIconFilePath);
            }
        }

        private void FrmShellLoad(object sender, EventArgs e)
        {
            int interval = Config.App.Timeout / 4;
            int intervalMill = interval * 1000;

            //idleTimer = new System.Threading.Timer(
            //    new TimerCallback(delegate(object state) {

            //        long idleTime = systemService.GetIdleTick();
            //        log.DebugFormat("idle time: {0}", idleTime);

            //        if (idleTime < intervalMill)
            //        {
            //            if (!closed)
            //            {
            //                Invoke(new MethodInvoker(delegate()
            //                {
            //                    webBrowser.Document.InvokeScript("resetTiming");
            //                }));
            //            }
            //        }

            //    }), null, 2000, intervalMill);

            webBrowser.Navigate(AppState.WelcomeUrl);
            //webBrowser.Navigate(Path.Combine(Config.AppRoot, "web\\qms\\html\\q\\q1.html"));
            //webBrowser.Navigate(Path.Combine(Config.AppRoot, "web\\qms\\html\\admin\\basic.html"));



            custgetseqService = AutofacContainer.ResolveNamed<ICustgetseqService>("custgetseqService");

            reservationService = AutofacContainer.ResolveNamed<IReservationService>("reservationService");

            linkageshutdownService = AutofacContainer.ResolveNamed<ILinkageshutdownService>("linkageshutdownService");

            SetCache("qmsIp", BuzConfig2ICBC.LocalIP);

            /// 预约 0：不在  1：在
            SetCache("yyzt", "0");

            /// 值守 0：非值守  1：值守
            SetCache("dutyStatus", "1");

            //  首页判断 0：首页  1：预约
            SetCache("isYyqhPage", "1");


            //  签到标记  1：已签到  0：未签到
            //SetCache("signFlag", "0");

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

            if (null != jo["mask"])
            {
                string message = jo["mask"].Value<string>("message");
                string skipText = jo["mask"].Value<string>("skipText");
                int timeout = jo["mask"].Value<int>("timeout");

            }

            string sound = jo.Value<string>("sound");
            int index = 0;

            if (!String.IsNullOrWhiteSpace(sound))
            {
                index = sound.IndexOf(";");

                if (index > 0)
                {
                    string wav = sound.Substring(0, index);
                    //voicePlayer.Play(wav);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logStr"></param>
        public void Log(string logStr)
        {
            log2JS.InfoFormat("-> args = {0}", logStr);
        }


        /// <summary>
        /// 页面调用 缓存数据，保存键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public string GetCache(string key)
        {
            string result = String.Empty;

            if (AppCache.dicPageCache.ContainsKey(key))
            {
                result = AppCache.dicPageCache[key];
            }

            return result;
        }

        /// <summary>
        /// 页面调用 缓存数据，根据科研获取值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetCache(string key, string value)
        {

            if (AppCache.dicPageCache.ContainsKey(key))  // 判断是否存在key
            {
                AppCache.dicPageCache[key] = value;
            }
            else
            {
                AppCache.dicPageCache.Add(key, value);
            }
        }

        /// <summary>
        /// 获取界面模板，读取本地json
        /// </summary>
        /// <param name="jo"></param>
        public string GetQueueTemplateJsonFile(string joStr)
        {
            JObject jo = JObject.Parse(joStr);

            //获取界面模板
            string screenResolution = jo["queueTemplate"].Value<string>("screenResolution");

            string filePath = "";

            filePath = Path.Combine(qmsPath, screenResolution + ".json");

            jo.RemoveAll();
            //模板转换
            JObject joket = JObject.Parse(GlobalVariable2ICBC.RETURN_MESSAGE_STR);

            joket["biom"]["head"]["retCode"] = "1";
            joket["biom"]["head"]["retMsg"] = "失败";


            if (File.Exists(filePath))
            {
                joket["biom"]["head"]["retCode"] = "0";
                joket["biom"]["head"]["retMsg"] = "成功";

                joket["biom"]["body"]["queueTemplate"] = new JObject();
                joket["biom"]["body"]["queueTemplate"]["print"] = JObject.Parse(File.ReadAllText(filePath, Encoding.UTF8));

            }
            jo["biom"] = joket["biom"];

            return jo.ToString();
        }

        /// <summary>
        /// 判断首页状态
        /// </summary>
        /// <param name="value"></param>
        public void SetPopEvent(string value)
        {
            if (value.Equals("0"))
            {
                popEnvent = false;
            }
            else
            {
                popEnvent = true;
            }
        }

        /// <summary>
        /// 实现遮罩
        /// </summary>
        public void ShowLoading(string mess)
        {
            log.DebugFormat("begin  开启遮罩...");
            JObject jo = new JObject();
            jo["callback"] = "showLoadingCallback";

            jo["biom"] = JObject.Parse(GlobalVariable2ICBC.RETURN_MESSAGE_STR)["biom"];

            jo["biom"]["head"]["retCode"] = "0";
            jo["biom"]["head"]["retMsg"] = mess;

            ScriptInvoke(jo);
            log.DebugFormat("end");
        }

        /// <summary>
        /// 隐藏遮罩
        /// </summary>
        public void HideLoading()
        {
            log.DebugFormat("begin  关闭遮罩...");
            JObject jo = new JObject();
            jo["callback"] = "hideLoading";

            ScriptInvoke(jo);
            log.DebugFormat("end");
        }


        private void FrmShellClosed(object sender, FormClosedEventArgs e)
        {
            closed = true;
            //idleTimer.Change(Timeout.Infinite, Timeout.Infinite);
            //idleTimer.Dispose();

            Dispose();

        }


        public void SystemCommond2JS(JObject jo)
        {
            log.DebugFormat("begin");

            sysCommandAsyncCard.BeginInvoke(jo, null, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 播放本地声音
        /// </summary>
        /// <param name="sName"></param>
        public void PlaySound2JS(string sName)
        {


            log.DebugFormat("begin args = {0}", sName);
            JObject jo = new JObject();
            jo["playSound"] = sName;


            try
            {
                if (userCardFlag)
                {

                    playSoundAsync.BeginInvoke(jo, null, jo);

                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("playSound -> Error . " + e);
            }
            log.DebugFormat("end");
        }

        /// <summary>
        /// 打开取号机日志
        /// </summary>
        /// <param name="sName"></param>
        public void OpenQsmLog()
        {
            log.DebugFormat("OpenQsmLog", "");
            System.Diagnostics.ProcessStartInfo spi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            spi.Arguments = "/e,/select," + Config.AppRoot + "\\logs\\app";
            System.Diagnostics.Process.Start(spi);
            log.DebugFormat("end");
        }
        /// <summary>
        /// 打开叫号机日志
        /// </summary>
        /// <param name="sName"></param>
        public void OpenBoxLog()
        {
            log.DebugFormat("OpenBoxLog", "");
            System.Diagnostics.ProcessStartInfo spi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            string boxLogPath = Config.AppRoot + "\\boxlogs\\boxLog.zip";

            if (!File.Exists(boxLogPath))
            {
                spi.Arguments = "/e,/select," + Config.AppRoot + "\\boxlogs";
            }
            else
            {
                spi.Arguments = "/e,/select," + Config.AppRoot + "\\boxlogs\\boxLog.zip";
            }
            System.Diagnostics.Process.Start(spi);
            log.DebugFormat("end");
        }
        /// <summary>
        /// 叫号机IP读取
        /// </summary>
        /// <param name="sName"></param>
        public string SelectWebServer()
        {

            log.DebugFormat("selectWebServer", "");

            JObject jokeit = JObject.Parse("{ \"biom\": { \"head\": { \"retCode\": \"1\", \"retMsg\": \"\" }, \"body\": {  \"host\": \"1\", \"port\": \"\", \"timeout\": \"\"  }} }");
            if (!File.Exists(Config.AppConfigAbsoluteFilePath)) throw new Exception(string.Format("配置文件不存在:{0}", Config.AppConfigAbsoluteFilePath));

            JObject old = JObject.Parse(File.ReadAllText(Config.AppConfigAbsoluteFilePath, Encoding.UTF8));

            if (string.Empty.Equals(old))
            {

                jokeit["biom"]["head"]["retCode"] = "1";

            }
            else
            {
                jokeit["biom"]["head"]["retCode"] = "0";
            }
            jokeit["biom"]["head"]["retMsg"] = "";

            jokeit["biom"]["body"]["host"] = old["webServer"]["host"];
            jokeit["biom"]["body"]["port"] = old["webServer"]["port"];
            jokeit["biom"]["body"]["timeout"] = old["webServer"]["timeout"];

            log.DebugFormat("jokeit", jokeit.ToString());




            return jokeit.ToString();


        }
        /// <summary>
        /// 设置配置文件
        /// </summary>
        /// <param name="sName"></param>
        public bool UpdateWebServer(string str)


        {



            JObject jo = JObject.Parse(str);

            log.DebugFormat("updateWebServer", "");

            if (!File.Exists(Config.AppConfigAbsoluteFilePath)) throw new Exception(string.Format("配置文件不存在:{0}", Config.AppConfigAbsoluteFilePath));

            JObject old = JObject.Parse(File.ReadAllText(Config.AppConfigAbsoluteFilePath, Encoding.UTF8));


            string webServerHost = jo["biom"]["body"].Value<string>("host");

            if (!String.IsNullOrWhiteSpace(webServerHost))
            {
                old["webServer"]["host"] = webServerHost;
            }

            int webServerPort = jo["biom"]["body"].Value<int>("port");
            if (webServerPort > 0)
            {
                old["webServer"]["port"] = webServerPort;
            }
            int timeout = jo["biom"]["body"].Value<int>("timeout");
            if (timeout > 0)
            {
                old["webServer"]["timeout"] = timeout;
            }

            string appConfigString = old.ToString();
            try
            {
                File.WriteAllText(Config.AppConfigAbsoluteFilePath, appConfigString, Encoding.UTF8);
            }
            catch (Exception e)
            {

                return false;
            }
            return true;

            log.DebugFormat("end");

        }
        /// <summary>
        /// 重启取号机
        /// </summary>
        /// <param name="sName"></param>
        /// <summary>
        public void QmsRestart()
        {
            Win32ApiInvoker.DoExitWin(2);
        }

        /// 播放本地声音
        /// </summary>
        /// <param name="sName"></param>
        public void PlaySound2Machine(string sName)
        {


            log.DebugFormat("begin args = {0}", sName);
            JObject jo = new JObject();
            jo["playSound"] = sName;


            try
            {
                //if (userCardFlag)
                //{

                playSoundAsync.BeginInvoke(jo, null, jo);

                //}
            }
            catch (Exception e)
            {
                log.ErrorFormat("playSound -> Error . " + e);
            }
            log.DebugFormat("end");
        }

        public void PlaySound(JObject jo)
        {
            log.DebugFormat("begin jo= {0}", jo);

            string sName = jo.Value<string>("playSound");
            // playSoundAsync.BeginInvoke(jo, null, jo);
            try
            {
                // sysAudio.Stop();

                Invoke(new MethodInvoker(delegate ()
                {
                    string filePath = Path.Combine(Application.StartupPath, @"sound\" + sName);
                    sysAudio.Play(filePath);
                }));

            }
            catch (Exception e)
            {
                log.ErrorFormat("playSound -> Error . " + e);
            }
            log.DebugFormat("end");

        }


        /// <summary>
        /// 密码命令执行操作
        /// </summary>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public string SystemCommond(string passWord)
        {
            log.InfoFormat("获取系统命令执行-密码指令执行, passWord:  {0}", passWord);

            JObject jo = new JObject();

            try
            {
                JObject joket = JObject.Parse(GlobalVariable2ICBC.RETURN_MESSAGE_STR);

                joket["biom"]["head"]["retCode"] = "1";
                joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_01;

                if (null != passWord || !passWord.Equals(String.Empty))
                {
                    if (passWord.Equals(BuzConfig2ICBC.ShutdownPwd))    // 关机密码
                    {
                        // 执行联动关机
                        JObject joTemp = new JObject();

                        joTemp = JObject.Parse(linkageshutdown2Str);

                        joTemp["biom"]["head"]["qmsIp"] = BuzConfig2ICBC.LocalIP;
                        joTemp["biom"]["body"]["delayedCloseTime"] = "0";

                        jo["biom"] = joTemp["biom"];

                        linkageshutdownService.Linkageshutdown2JS(jo);

                        joket["biom"]["head"]["retCode"] = "25";
                        joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_02;

                        ShowLoading(GlobalVariable2ICBC.MESS_TEXT_02);

                        //sysCommandAsyncCard.BeginInvoke(jo, null, jo);
                    }
                    else if (passWord.Equals(BuzConfig2ICBC.ExitGetTicketPwd))  //  取号界面退出密码
                    {
                        joket["biom"]["head"]["retCode"] = "26";
                        joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_03;
                        CancelReadRawData();
                        webBrowser.Navigate(Path.Combine(Config.AppRoot, "web\\qms\\html\\admin\\index.html"));

                    }
                    else if (passWord.Equals(BuzConfig2ICBC.OnlineSwitchPwd))  //  联机脱机切换密码
                    {
                        JObject joTemp = new JObject();

                        joTemp = JObject.Parse(offLineSwich2Str);
                        joTemp["biom"]["head"]["qmsIp"] = BuzConfig2ICBC.LocalIP;

                        jo["biom"] = joTemp["biom"];

                        string offLineStatus = String.Empty;

                        if (!BuzConfig2ICBC.DevStatus.Equals(String.Empty))
                        {
                            offLineStatus = BuzConfig2ICBC.DevStatus.Split('|')[2];

                            if (offLineStatus.Equals("1"))
                            {
                                offLineStatus = "0";
                                joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_04;
                            }
                            else if (offLineStatus.Equals("0"))
                            {
                                offLineStatus = "1";
                                joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_05;
                            }
                            else
                            {

                            }

                            jo["biom"]["body"]["offLineStatus"] = offLineStatus;

                            if (!offLineStatus.Equals(String.Empty))
                            {
                                custgetseqService.OffLineSwich2CallMachine(jo);

                                string retCodde = jo["biom"]["head"].Value<string>("retCode");

                                if (retCodde.Equals("0"))
                                {
                                    joket["biom"]["head"]["retCode"] = "27";

                                }
                                else
                                {
                                    joket["biom"]["head"]["retCode"] = "1";
                                    joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_12;
                                }

                            }
                            else
                            {
                                joket["biom"]["head"]["retCode"] = "1";
                                joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_11;

                            }

                        }
                        else
                        {
                            joket["biom"]["head"]["retCode"] = "1";
                            joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_11;
                        }

                    }
                    else if (passWord.Equals(BuzConfig2ICBC.DutySwitchPwd))  //  值守/非值守模式切换密码?
                    {
                        joket["biom"]["head"]["retCode"] = "28";

                        if (BuzConfig2ICBC.DutyFlag.Equals("1"))
                        {

                            if (GetCache("dutyStatus").Equals("1"))
                            {
                                SetCache("dutyStatus", "0");
                                joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_07;
                            }
                            else
                            {
                                SetCache("dutyStatus", "1");
                                joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_08;
                            }
                        }
                        else
                        {
                            joket["biom"]["head"]["retCode"] = "1";
                            joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_10;
                        }

                    }
                    else if (passWord.Equals(BuzConfig2ICBC.MinimizedPage))
                    {

                        this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
                        joket["biom"]["head"]["retCode"] = "29";
                        joket["biom"]["head"]["retMsg"] = "";
                    }
                    else if (passWord.Equals("999"))
                    {

                        joket["biom"]["head"]["retCode"] = "26";
                        joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_03;
                        CancelReadRawData();
                        webBrowser.Navigate(Path.Combine(Config.AppRoot, "web\\qms\\html\\admin\\index.html"));
                    }
                    else
                    {
                        joket["biom"]["head"]["retCode"] = "1";
                        joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_06;
                    }

                }
                else
                {
                    joket["biom"]["head"]["retCode"] = "1";
                    joket["biom"]["head"]["retMsg"] = GlobalVariable2ICBC.MESS_TEXT_01;
                }






                jo["biom"] = joket["biom"];

            }
            catch (Exception e)
            {
                log.ErrorFormat("SystemCommond.error -> ", e);
            }

            return jo.ToString();
        }

        /// <summary>
        /// 系统命令执行
        /// </summary>
        /// <param name="jo"></param>
        public void SysCommandImpl(JObject jo)
        {
            log.InfoFormat("begin, args: jo = {0}", jo);

            string cmdStr = jo["biom"]["head"].Value<string>("retCode");

            switch (cmdStr)
            {
                case "25":
                    // 关机密码
                    //log.InfoFormat("执行关机命令...");
                    //Win32ApiInvoker.DoExitWin(8);
                    break;


            }


            log.InfoFormat("end");
        }

        /// <summary>
        /// 关机
        /// </summary>
        public void ShutDownSystem()
        {
            log.InfoFormat("begin");

            log.InfoFormat("执行关机命令...");
            Win32ApiInvoker.DoExitWin(8);

            log.InfoFormat("end");
        }

        public string getAppState()
        {
            return "{\"result\":0,\"appState\":" + AppState.ToJsonString() + "}";
        }

        public string getAdvConfig()
        {
            return "{\"result\":0,\"advConfig\":" + Config.GetAdvConfig() + "}";
        }

        public string GetAppConfig()
        {
            return "{\"result\":0,\"appConfig\":" + Config.GetAppConfig() + "}";
        }

        public string SaveAppConfig(string json)
        {
            int result = 0;

            try
            {
                JObject jo = JObject.Parse(json);
                Config.SaveAppConfig(jo["appConfig"]);
            }
            catch (Exception e)
            {
                result = 1;
                log.Error("SaveAppConfig error", e);
            }

            return "{\"result\":" + result + "}";
        }

        public string GetPrintConfig(string groupType, string receiptType)
        {
            return "{\"result\":0,\"printConfig\":" + Config.GetPrintConfig(groupType, receiptType) + "}";
        }

        public void SaveAndPrintConfig(string args)
        {
            log.ErrorFormat("begin SaveAndPrintConfig(string args) args : args = {0}", args);

            try
            {
                JObject jo = JObject.Parse(args);

                if (null != jo["mask"])
                {
                    string message = jo["mask"].Value<string>("message");

                }

                Config.SavePrintConfig(jo.Value<string>("groupType"), jo.Value<string>("receiptType"), jo["print"]);
                //peripheralManager.NeedlePrinter.PrintAsync(jo);
            }
            catch (Exception e)
            {
                log.ErrorFormat("SaveAndPrintConfig error", e);
            }
        }

        public void PaperBacking()
        {
            this.TopMost = Config.App.TopMost;
            //peripheralManager.NeedlePrinter.Execute(1);
        }

        public void Shut()
        {
            Invoke(new MethodInvoker(delegate ()
            {
                this.Close();
            }));
        }

        public void Navigate(string url)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                webBrowser.Navigate(url);
            }));
        }

        public void ScriptInvoke(JObject jo)
        {
            log.DebugFormat("回调 closed = {0}", closed);
            if (closed)
            {
                log.DebugFormat("回调 退出.......");
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
                            //voicePlayer.Play(ar[1]);
                            break;
                        }
                    }
                }
            }

            if (!closed)
            {
                Invoke(new MethodInvoker(delegate ()
                {
                    if (!String.IsNullOrWhiteSpace(callback))
                    {
                        //log.DebugFormat("触发页面回调..............{0}", callback);
                        webBrowser.Document.InvokeScript(callback, new object[] { jo.ToString(Formatting.None) });
                    }
                }));
            }
        }


        private void WebBrowserNavigated(object sender, WebBrowserNavigatedEventArgs e)
       {
            string localPath = e.Url.LocalPath;
            string filename = Path.GetFileName(localPath);
            bool enabled = false;
            bool visibled = false;

            if (localPath.Contains("web\\welcome.html"))
            {
                enabled = true;
                visibled = false;
            }
            else if (localPath.Contains("web\\qms\\html\\q"))
            {
                enabled = true;
                visibled = false;
            }
            else if (localPath.Contains("web\\pps\\html\\admin\\welcome_admin.html"))
            {
                enabled = false;
                visibled = false;
            }
            else if (localPath.Contains("web\\pps\\html"))
            {
                enabled = true;
                visibled = true;
            }
            else if (localPath.Contains("web\\qms\\html\\admin"))
            {
                enabled = false;
                visibled = false;
            }


        }

        //签到
        IQmssignService qmssginService = AutofacContainer.ResolveNamed<IQmssignService>("qmssignService");
        //基础设置
        IBasicconfigService basicconfigService = AutofacContainer.ResolveNamed<IBasicconfigService>("basicconfigService");

        //关于
        IAboutService aboutService = AutofacContainer.ResolveNamed<IAboutService>("aboutService");
        //
        int rCount2 = 1;

        private void SetRichTestBox2(string mess, int state, bool isNoNumber)
        {

            switch (state)
            {
                case 2:
                    {
                        richTextBox2.SelectionColor = Color.Black;
                    }
                    break;
                case 3:
                    {
                        richTextBox2.SelectionColor = Color.Red;
                    }
                    break;
                case 4:
                    {
                        richTextBox2.SelectionColor = Color.Blue;
                    }
                    break;
                case 5:
                    {
                        richTextBox2.SelectionColor = Color.Green;
                    }
                    break;

            }
            if (isNoNumber)
            {
                richTextBox2.AppendText("序号 " + rCount2.ToString() + " ：");
                richTextBox2.AppendText("\n");
                richTextBox2.AppendText(mess);
                richTextBox2.AppendText("\n");
                richTextBox2.AppendText("\n");
            }
            else
            {
                richTextBox2.AppendText("\t" + mess);
                richTextBox2.AppendText("\n");
            }


            if (isNoNumber)
            {
                rCount2++;
            }

        }

        private void SetRichTestBox(string mess)
        {
            SetRichTestBox2(mess, 4, false);

        }

        private void SetRichTestBoxByMaim(string mess)
        {
            SetRichTestBox2(mess, 4, true);

        }

        //private IReader mifareCardReader;


        private void button2_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Application.StartupPath, @"sound\idCard.wav");
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.SoundLocation = filePath;
            player.LoadAsync();
            player.PlaySync();

            //string tStr = "{\"biom\":{\"head\":{\"tradeCode\":\"custgetseq\",\"qmsIp\":\"122.138.45.223\",\"channel\":\"1\"},\"body\":{\"busiType1\":\"1\",\"busiType2\":\"236\",\"cardFlag\":\"1\",\"secondTrack\":\";6222081602003068318=23052203429991847?\",\"thirdTrack\":\";996222081602003068318=1561560000000000001003342999010000023050=000000000000=000000000000=00000000?\",\"certType\":\"\",\"certNo\":\"\",\"custName\":\"\",\"secgs\":\"\",\"custTime\":\"\",\"nation\":\"\",\"office\":\"\",\"signDate\":\"\",\"indate\":\"\",\"addr\":\"\",\"sex\":\"\",\"birthday\":\"\",\"image\":\"\"}}}";

            //JObject jo = JObject.Parse(tStr);

            //custgetseqService.Custgetseq2JS(jo);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }




        private void timer_tick(int days)
        {

            DirectoryInfo di = new DirectoryInfo(Config.AppRoot + "\\logs");


            FileInfo[] files = di.GetFiles("*.*");

            // string[] files = Directory.GetFiles(Config.AppRoot + "\\logs","*.*",SearchOption.AllDirectories); 

            foreach (FileInfo file in files)
            {
                // string s = file;
                // FileInfo f = new FileInfo(s);
                DateTime nowtime = DateTime.Now;
                TimeSpan t = nowtime - file.CreationTime;
                int day = t.Days;
                if (day > days)
                {
                    try
                    {

                        file.Delete();

                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("删除过期日志失败" + e);
                    }

                }

            }



        }
        // 签到
        private void button1_Click(object sender, EventArgs e)
        {
            // string dataStr = "{\"biom\":{\"head\":{\"tradeCode\":\"pingLisServiceIp\",\"qmsIp\":\"127.0.0.1\",\"channel\":\"1\"},\"body\":{\"busiType1\":\"1\",\"busiType2\":\"236\",\"cardFlag\":\"1\",\"secondTrack\":\";6222081602003068318=23052203429991847?\",\"thirdTrack\":\";996222081602003068318=1561560000000000001003342999010000023050=000000000000=000000000000=00000000?\",\"certType\":\"\",\"certNo\":\"\",\"custName\":\"\",\"secgs\":\"\",\"custTime\":\"\",\"nation\":\"\",\"office\":\"\",\"signDate\":\"\",\"indate\":\"\",\"addr\":\"\",\"sex\":\"\",\"birthday\":\"\",\"image\":\"\"}}}";

            //ITicketsconfigService tSer = AutofacContainer.ResolveNamed<ITicketsconfigService>("ticketsconfigService");

            // IIcbcspecialuseService sss = AutofacContainer.ResolveNamed<IIcbcspecialuseService>("icbcspecialuseService");
            string dataStr = "{\"biom\":{\"head\":{\"retCode\":\"0\",\"retMsg\":\"成功\"},\"body\":{\"isAutoUpdate\":\"0\",\"ftpUserName\":\"\",\"ftpUserPwd\":\"\",\"ftpIp\":\"\",\"ftpPort\":\"\"}},\"callback\":\"updateconfig2JSCallback\"}";
            IAboutService ic = AutofacContainer.ResolveNamed<IAboutService>("aboutService");
            JObject jo = new JObject();

            jo["biom"] = JObject.Parse(dataStr)["biom"];

            JObject jokeit = JObject.Parse("{ \"biom\": { \"head\": { \"retCode\": \"1\", \"retMsg\": \"\" }, \"body\": {  \"host\": \"127.0.55.3\", \"port\": \"9999\", \"timeout\": \"12000\"  }} }");

            //QmsRestart();
            // UpdateWebServer(jokeit);
            // ic.About2JS(jo);

            // selectWebServer(jo);

            // ic.Updateconfig2JS(jo);
            //sss.IcbcspecialusePing2JS(jo);


            //if (JsonSplit.IsJson(dataStr))    // 接收到返回消息""
            //{
            //    JObject jo = JObject.Parse(dataStr);

            //    string tickePrintForm = jo["biom"]["body"].Value<string>("tickePrintForm");
            //    string path = @"D:\Receipt.form";
            //    if (FileHelper.WriteToFile(path, tickePrintForm))
            //    {


            //        log.DebugFormat("Update print success!");
            //    }
            //    else
            //    {
            //        log.DebugFormat("Update print fail!");
            //    }
            //}
            //else
            //{

            //    string op1 = "";
            //}


            //tSer.Ticketsconfig2JS(jo);

            //string op = "{\"biom\":{\"head\":{\"tradeCode\":\"kazhejudge\",\"qmsIp\":\"122.138.45.220\"},\"body\":{\"cardFlag\":\"1\",\"secondTrack\":\";6222300456547325=22122010010879499999?\",\"thirdTrack\":\"\",\"certType\":\"107\",\"certNo\":\"\",\"custName\":\"\",\"secgs\":\"\",\"custTime\":\"\",\"nation\":\"\",\"office\":\"\",\"signDate\":\"\",\"indate\":\"\",\"addr\":\"\",\"sex\":\"\",\"birthday\":\"\",\"image\":\"\"}}}";

            //JObject jo = new JObject();

            //jo["biom"] = JObject.Parse(op)["biom"];


            //custgetseqService.Kazhejudge2CallMachine(jo);

            //string filePath = Path.Combine(Application.StartupPath, @"sound\idCard.wav");

            //PlaySound2JS("idCardFailure.wav");

            //sysAudio.Stop();
            //sysAudio.Play(filePath);  

            #region
            //JObject ket = new JObject();

            //ket["biom"] = new JObject();

            //ket["biom"]["head"] = new JObject();

            //ket["biom"]["head"]["tradeCode"] = "shukazhe";

            //string op5 = ket.ToString();


            //string opStr = "{\"biom\":{\"head\":{\"tradeCode\":\"kazhejudge\",\"qmsIp\":\"127.0. 0.1\"},\"body\":{\"cardFlag\":\"6\",\"secondTrack\":\"6232600200001260085D49126202779991108F\",\"thirdTrack\":\"\",\"certType\":\"107\",\"certNo\":\"\",\"custName\":\"\",\"secgs\":\"\",\"custTime\":\"\",\"nation\":\"\",\"office\":\"\",\"signDate\":\"\",\"indate\":\"\",\"addr\":\"\",\"sex\":\"\",\"birthday\":\"\",\"image\":\"\"}}}";

            //byte[] data = Encoding.UTF8.GetBytes(opStr);


            //int len = data.Length;

            //JObject jopp = new JObject();

            //jopp= JObject.Parse(opStr);

            //string op11 = jopp.ToString().Replace("\r\n", "").Replace(" ","");

            //data = Encoding.UTF8.GetBytes(op11);


            //len = data.Length;


            //string str1 = "{\"biom\":{\"head\":{\"tradeCode\":\"basicconfig2select\",\"qmsIp\":\"127.0.0.1\"},\"body\":{}}}";

            //JObject jo = new JObject();

            ////jo = JObject.Parse(str1);

            ////jo["command"] = "34";

            ////string screenResolution = jo["queueTemplate"].Value<string>("screenResolution");
            //// 关于
            ////aboutService.About2JS(jo);

            //jo["queueTemplate"] = new JObject();
            //jo["queueTemplate"]["screenResolution"] = "1024x1280";
            //jo["command"] = "select";

            //axIdcAx2Mifare_ReadRawDataComplete(new object(),new AxIdcAxLib._DIdcAxEvents_ReadRawDataCompleteEvent(""));

            ////basicconfigService.GetBasicconfig2JS(jo);

            ////GetQueueTemplateJsonFile(jo.ToString());

            ////qmssginService.GetQmssign2JS(jo);

            //SetRichTestBox(jo.ToString());
            #endregion
        }

        // 打印
        private void button6_Click(object sender, EventArgs e)
        {


            string pStr = "存取款|A0001|3|脱机模式不存在排队验证码|--------★★★★☆☆☆--------|欢迎光临|1970-01-02 15:45:19||";


            //Print2JS(pStr);
        }

        #region ocx调用

        /// <summary>
        /// 打开设备连接
        /// </summary>
        private void Open2OCX()
        {
            log.InfoFormat("begin");
            try
            {
                // 初始化卡
                axIdcAx2Mifare.LogicalName = LOGICALNAME;

                string rStr = axEMVICAx2Mifare.SetDeviceName(LOGICALNAME);

                log.InfoFormat("设置EMV逻辑名 -> SetDeviceName = {0}", rStr);

                int emvInt = axEMVICAx2Mifare.EmvInitialize("MifareCardReader");

                log.DebugFormat(" -> EmvInitialize = {0}", emvInt);

                if (axIdcAx2Mifare.OpenConnection())
                {

                    cancelled = false;

                    //MessageBox.Show("初始化 卡 成功");

                    ReadRawData();

                    //openAsyncCard.BeginInvoke(null, null, null);
                }
                else
                {
                    cancelled = true;

                    log.ErrorFormat("初始化OCX 卡 失败...");

                }

                // 初始化打印机
                axPtrAx2Print.LogicalName = "ReceiptPrinter";

                if (axPtrAx2Print.OpenConnection())
                {

                    cancelled = false;

                }
                else
                {
                    cancelled = true;

                    log.ErrorFormat("初始化OCX 打印机 失败...");
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("初始化OCX失败..." + e);
            }

            log.InfoFormat("end");
        }

        /// <summary>
        /// 重置打印机
        /// </summary>
        public void ResetPrint()
        {
            log.DebugFormat("begin");

            try
            {
                // 关闭打印机

                axPtrAx2Print.CloseConnection();


                // 初始化打印机
                axPtrAx2Print.LogicalName = "ReceiptPrinter";

                if (axPtrAx2Print.OpenConnection())
                {

                    cancelled = false;

                }
                else
                {
                    cancelled = true;

                    log.ErrorFormat("初始化OCX 打印机 失败...");
                }

            }
            catch (Exception e)
            {
                log.ErrorFormat("重置打印机失败:" + e);

            }



            log.DebugFormat("end");

        }

        /// <summary>
        /// 设备更新
        /// </summary>
        /// <param name="jo"></param>
        public void PeripheralInvoke(JObject jo)
        {
            log2Job.DebugFormat("begin");

            string rStr = "9999";
            string pStr = "9999";
            string trendMicroStr = ConvertUtil.GetTrendMicroStatus();
            string dsmClentStr = ConvertUtil.GetDSMClientStatus();
            string devStr = GetPeripheralStatus();
            string printStr = GetPrintStatus2JS();

            if (!devStr.Equals(String.Empty))
            {
                if (JsonSplit.IsJson(devStr))
                {
                    JObject joket = JObject.Parse(devStr);

                    string device = joket.Value<string>("Device");

                    if (device.Equals("HEALTHY"))
                    {
                        rStr = "0000";
                    }
                    else if (device.Equals("FATAL"))
                    {
                        rStr = "0003";
                    }
                    else
                    {
                        rStr = "9999";
                    }

                }
            }
            else
            {
                rStr = "9999";
            }

            if (!printStr.Equals(String.Empty))
            {
                if (JsonSplit.IsJson(printStr))
                {
                    JObject joket = JObject.Parse(printStr);

                    string device = joket.Value<string>("Device");
                    string paper = joket.Value<string>("Paper");

                    if (device.Equals("HEALTHY"))
                    {
                        if (paper.Equals("FULL"))
                        {
                            pStr = "0000";
                        }
                        else
                        {
                            pStr = "0002";
                        }
                    }
                    else if (device.Equals("FATAL"))
                    {
                        pStr = "0003";
                    }
                    else
                    {
                        pStr = "9999";
                    }

                }

            }
            else
            {
                pStr = "9999";
            }
            AppState.PrintStatus = pStr;
            jo["biom"]["body"]["status"]["card"] = rStr;
            jo["biom"]["body"]["status"]["rfcard"] = rStr;
            jo["biom"]["body"]["status"]["miniprint"] = pStr;
            jo["biom"]["body"]["status"]["idcard"] = rStr;
            jo["biom"]["body"]["status"]["TrendMicro"] = trendMicroStr;
            jo["biom"]["body"]["status"]["DSMClient"] = dsmClentStr;

            log2Job.DebugFormat("end，args = {0}", jo);

        }

        /// <summary>
        /// 获取外设状态
        /// </summary>
        /// <returns></returns>
        public string GetPeripheralStatus()
        {
            string statusStr = String.Empty;

            Invoke(new MethodInvoker(delegate ()
            {
                if (null != axPtrAx2Print)
                {
                    statusStr = axIdcAx2Mifare.GetStatus();

                    if (!JsonSplit.IsJson(statusStr))
                    {
                        statusStr = String.Empty;
                    }

                }
            }));

            return statusStr;

        }

        /// <summary>
        /// 表示卡片已经被用户取走
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axIdcAx2Mifare_CardTaken(object sender, EventArgs e)
        {
            log.DebugFormat("begin User take card ... ");

            userCardFlag = false;

            log.DebugFormat("end");
        }

        /// <summary>
        /// 表示卡片已经插入设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void axIdcAx2Mifare_CardInserted(object sender, EventArgs e)
        {
            log.DebugFormat("begin User card ... ");

            userCardFlag = true;

            log.DebugFormat("end");
        }

        /// <summary>
        /// 日志加密
        /// </summary>
        /// <param name="jo"></param>
        /// <returns></returns>
        public JObject LogEncryption2(JObject jo)
        {
            log.DebugFormat("begin");

            JObject joket = new JObject();
            try
            {


                joket["biom"] = jo["biom"];

                string cardFlag = joket["biom"]["body"].Value<string>("cardFlag");

                switch (cardFlag)
                {
                    case "3":  // 身份证
                        string custName = joket["biom"]["body"].Value<string>("custName") == null ? String.Empty : joket["biom"]["body"].Value<string>("custName");
                        string certNo = joket["biom"]["body"].Value<string>("certNo") == null ? String.Empty : joket["biom"]["body"].Value<string>("certNo");
                        joket["biom"]["body"]["addr"] = GlobalVariable2ICBC.REPLACE_LOG_STR_01;
                        joket["biom"]["body"]["birthday"] = GlobalVariable2ICBC.REPLACE_LOG_STR_01;
                        if (custName.Length > 0)
                        {
                            joket["biom"]["body"]["custName"] = custName.Substring(0, custName.Length - 1) + "*";
                        }
                        if (certNo.Length >= 15)
                        {
                            joket["biom"]["body"]["certNo"] = certNo.Substring(0, certNo.Length - 5) + "****" + certNo.Substring(certNo.Length - 1, 1);
                        }
                        break;
                    case "1":
                    case "6":
                        string secondTrack = joket["biom"]["body"].Value<string>("secondTrack") == null ? String.Empty : joket["biom"]["body"].Value<string>("secondTrack");
                        string thirdTrack = joket["biom"]["body"].Value<string>("thirdTrack") == null ? String.Empty : joket["biom"]["body"].Value<string>("thirdTrack");


                        if (secondTrack.Contains("="))
                        {
                            secondTrack = secondTrack.Substring(0, secondTrack.IndexOf('=') + 1);

                            if (secondTrack.Length > 5)
                            {
                                joket["biom"]["body"]["secondTrack"] = secondTrack.Substring(0, secondTrack.Length - 5) + "****" + secondTrack.Substring(secondTrack.Length - 1, 1) + GlobalVariable2ICBC.REPLACE_LOG_STR_01; ;
                            }

                        }
                        if (thirdTrack.Contains("="))
                        {
                            thirdTrack = thirdTrack.Substring(0, thirdTrack.IndexOf('=') + 1);

                            if (thirdTrack.Length > 5)
                            {
                                joket["biom"]["body"]["thirdTrack"] = thirdTrack.Substring(0, thirdTrack.Length - 5) + "****" + thirdTrack.Substring(thirdTrack.Length - 1, 1) + GlobalVariable2ICBC.REPLACE_LOG_STR_01; ;
                            }

                        }

                        break;
                    default:

                        break;


                }

            }
            catch (Exception e)
            {
                log.ErrorFormat("日志加密外设信息异常：" + e);
            }

            log.DebugFormat("end");

            return joket;
        }

        private JObject LogEncryption(JObject jo)
        {
            JObject joket = new JObject();

            joket["Chip"] = jo.Value<string>("Chip");
            joket["ChipStatus"] = jo.Value<string>("ChipStatus");
            joket["Result"] = jo.Value<string>("Result");
            joket["Track1"] = jo.Value<string>("Track1");
            joket["Track1Status"] = jo.Value<string>("Track1Status");
            joket["Track2"] = jo.Value<string>("Track2");
            joket["Track2Status"] = jo.Value<string>("Track2Status");
            joket["Track3"] = jo.Value<string>("Track3");
            joket["Track3Status"] = jo.Value<string>("Track3Status");

            log.DebugFormat("begin");

            try
            {
                string track1 = jo.Value<string>("Track1") == null ? String.Empty : jo.Value<string>("Track1");

                string track2 = jo.Value<string>("Track2") == null ? String.Empty : jo.Value<string>("Track2");

                string track3 = jo.Value<string>("Track3") == null ? String.Empty : jo.Value<string>("Track3");

                if (track1 != String.Empty)
                {
                    track1 = track1.Substring(0, track1.IndexOf('=') + 1);

                    if (track1.Length > 5)
                    {
                        joket["Track1"] = GlobalVariable2ICBC.REPLACE_LOG_STR_01; ;
                    }

                }

                if (track2 != String.Empty)
                {
                    track2 = track2.Substring(0, track2.IndexOf('=') + 1);

                    if (track2.Length > 5)
                    {
                        joket["Track2"] = track2.Substring(0, track2.Length - 5) + "****" + track2.Substring(track2.Length - 1, 1) + GlobalVariable2ICBC.REPLACE_LOG_STR_01;
                    }

                }

                if (track3 != String.Empty)
                {
                    track3 = track3.Substring(0, track3.IndexOf('=') + 1);

                    if (track3.Length > 5)
                    {
                        joket["Track3"] = track3.Substring(0, track3.Length - 5) + "****" + track3.Substring(track3.Length - 1, 1) + GlobalVariable2ICBC.REPLACE_LOG_STR_01;
                    }

                }


            }
            catch (Exception e)
            {
                log.ErrorFormat("日志加密外设信息异常：" + e);

            }
            log.DebugFormat("end");


            return joket;

        }

        /// <summary>
        /// 读卡操作完成时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axIdcAx2Mifare_ReadRawDataComplete(object sender, AxIdcAxLib._DIdcAxEvents_ReadRawDataCompleteEvent e)
        {
            log.InfoFormat("begin");
            try
            {
                AppState.PerSign = false;
                //if (popEnvent)
                //{

                // 测试IC卡
                //string cardRawDataStr = "{\"Chip\":\"3B6D000080318065B0872701BC83089000\",\"ChipStatus\":\"READ\",\"Result\":\"Success\",\"Track1\":\"\",\"Track1Status\":\"BLANK\",\"Track2\":\"\",\"Track2Status\":\"BLANK\",\"Track3\":\"\",\"Track3Status\":\"BLANK\"}";

                // 测试身份证
                //string cardRawDataStr = "{\"Chip\":\"\",\"ChipStatus\":\"BLANK\",\"Result\":\"Success\",\"Track1\":\"Name=刘牛|Sex=男|Nation=汉族|Born=19940419|Address=陕西省勉县褒城镇青沟村一组010号|IDCardNo=612325199404190710|GrantDept=勉县公安局|UserLifeBegin=20140715|UserLifeEnd=20240715|PhotoFileName=C://picture//photo.bmp\",\"Track1Status\":\"READ\",\"Track2\":\"\",\"Track2Status\":\"BLANK\",\"Track3\":\"\",\"Track3Status\":\"BLANK\"}";

                string cardRawDataStr = (sender as AxIdcAxLib.AxIdcAx).CardRawData;

                cardRawDataStr = cardRawDataStr.Replace('\\', '/');

                //log.InfoFormat("CardRawData = {0}", cardRawDataStr);

                if (JsonSplit.IsJson(cardRawDataStr))
                {
                    JObject jo = JObject.Parse(cardRawDataStr);

                    Read(jo);
                }
                else
                {
                    log.ErrorFormat("不是合法的JOSN格式字符串 CardRawData = {0}", cardRawDataStr);
                }


                //}
                //else
                //{
                //    log.InfoFormat("非首页，不进行刷卡or折操作...");
                //}

            }
            catch (Exception ex)
            {
                HideLoading();
                log.Error("外设ocx触发 失败:" + ex);
            }
            finally
            {


            }

            //openAsyncCard.BeginInvoke(null, null, null);
            log.InfoFormat("end");
        }

        /// <summary>
        /// 刷卡/折
        /// </summary>
        private readonly static string shukazJosn2Str = "{\"biom\":{\"head\":{\"tradeCode\":\"kazhejudge\",\"qmsIp\":\"\",\"channel\":\"1\"},\"body\":{\"cardFlag\":\"\",\"secondTrack\":\"\",\"thirdTrack\":\"\",\"certType\":\"\",\"certNo\":\"\",\"custName\":\"\",\"secgs\":\"\",\"custTime\":\"\",\"nation\":\"\",\"office\":\"\",\"signDate\":\"\",\"indate\":\"\",\"addr\":\"\",\"sex\":\"\",\"birthday\":\"\",\"image\":\"\"}}}";

        /// <summary>
        /// 联脱机切换
        /// </summary>
        private readonly static string offLineSwich2Str = "{\"biom\":{\"head\":{\"tradeCode\":\"offLineSwich\",\"qmsIp\":\"\"},\"body\":{\"offLineStatus\":\"\"}}}";

        /// <summary>
        /// 联动关机
        /// </summary>
        private readonly static string linkageshutdown2Str = "{\"biom\":{\"head\":{\"tradeCode\":\"linkageshutdown\",\"qmsIp\":\"\"},\"body\":{\"delayedCloseTime\":\"\"}}}";

        public void Read(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", LogEncryption(jo));

            ShowLoading(BuzConfig2ICBC.INFO_39);

            bool rSign = true;
            JObject joket = JObject.Parse(shukazJosn2Str);

            joket["biom"]["head"]["qmsIp"] = BuzConfig2ICBC.LocalIP;
            // 一磁道
            string track1Str = String.Empty;
            // 二磁道
            string track2Str = String.Empty;
            // 三磁道
            string track3Str = String.Empty;

            //if (cancelled)
            //{
            //    jo["result"] = ErrorCode.Cancelled;
            //    log.Info("end, cancelled");
            //    return;
            //}

            string chipStatusStr = jo.Value<string>("ChipStatus");

            log.DebugFormat(" -> ChipStatus = {0}", chipStatusStr);

            if (chipStatusStr.Equals("READ"))
            {
                //int emvInt = axEMVICAx2Mifare.EmvInitialize("MifareCardReader");

                //log.DebugFormat(" -> EmvInitialize = {0}", emvInt);

                string chip = jo.Value<string>("Chip");

                axEMVICAx2Mifare.WaitTag = "57";

                axEMVICAx2Mifare.ICAtr = chip;

                string emvStr = axEMVICAx2Mifare.StartTransaction();

                log.DebugFormat(" -> StartTransaction = {0}", emvStr);

                //开启EMV操作
                if ("ERR_NOSHOW" == emvStr)
                {
                    emvStr = axEMVICAx2Mifare.SelectApplication(0);

                    log.DebugFormat(" -> SelectApplication = {0}", emvStr);

                    if ("SUCCESS" == emvStr)
                    {
                        emvStr = axEMVICAx2Mifare.GetTagData("57");	// 读取二磁信息

                        if (emvStr.Equals(""))
                        {
                            rSign = false;  // 异常卡,未读到值
                            log.DebugFormat(" -> GetTagData = {0}", GlobalVariable2ICBC.REPLACE_LOG_STR_01);
                        }
                        else
                        {

                            log.DebugFormat(" -> GetTagData = {0}", emvStr);
                        }

                        //int nIndex = emvStr.IndexOf("F");

                        //if (nIndex > 10)
                        //{
                        //    emvStr = emvStr.Substring(0, nIndex);

                        //    log.InfoFormat("存在分割符 F ，args = {0}", emvStr);
                        //}

                        // 获取到数据，返回调用 

                        joket["biom"]["body"]["cardFlag"] = "6";
                        joket["biom"]["body"]["certType"] = "7";
                        joket["biom"]["body"]["secondTrack"] = emvStr;

                        //axEMVICAx2Mifare.CompleteTransaction();

                    }
                    else
                    {
                        rSign = false;
                    }

                }
                else
                {

                    rSign = false;  // 异常卡,未读到值

                }

            }
            else if (chipStatusStr.Equals("BLANK"))
            {
                //log.InfoFormat("判断卡为【BLANK】");

                string track1StatusStr = jo.Value<string>("Track1Status");
                string track2StatusStr = jo.Value<string>("Track2Status");
                string track3StatusStr = jo.Value<string>("Track3Status");

                if (track1StatusStr.Equals("READ"))
                {
                    // 身份证
                    joket["biom"]["body"]["cardFlag"] = "3";
                    joket["biom"]["body"]["certType"] = "0";
                    track1Str = jo.Value<string>("Track1");

                    if (!track1Str.Equals(""))
                    {
                        string[] idStrs = track1Str.Split('|');

                        joket["biom"]["body"]["custName"] = idStrs[0].Replace("Name=", "");

                        joket["biom"]["body"]["sex"] = idStrs[1].Replace("Sex=", "");

                        joket["biom"]["body"]["nation"] = idStrs[2].Replace("Nation=", "");

                        joket["biom"]["body"]["birthday"] = idStrs[3].Replace("Born=", "");

                        joket["biom"]["body"]["addr"] = idStrs[4].Replace("Address=", "");

                        joket["biom"]["body"]["certNo"] = idStrs[5].Replace("IDCardNo=", "");

                        joket["biom"]["body"]["office"] = idStrs[6].Replace("GrantDept=", "");

                        joket["biom"]["body"]["signDate"] = idStrs[7].Replace("UserLifeBegin=", "");

                        joket["biom"]["body"]["indate"] = idStrs[8].Replace("UserLifeEnd=", "");

                        if (BuzConfig2ICBC.CertImageFlag.Equals("1"))
                        {
                            joket["biom"]["body"]["image"] = ConvertUtil.ImageToBase64(idStrs[9].Replace("PhotoFileName=", ""));
                        }
                        else
                        {
                            joket["biom"]["body"]["image"] = "";
                        }
                    }

                }
                else
                {
                    // 磁条卡或IC卡
                    joket["biom"]["body"]["cardFlag"] = "1";
                    joket["biom"]["body"]["certType"] = "7";
                    if (track2StatusStr.Equals("READ"))
                    {

                        track2Str = TrackChange(jo.Value<string>("Track2"));

                        joket["biom"]["body"]["secondTrack"] = track2Str;

                    }
                    if (track3StatusStr.Equals("READ"))
                    {
                        track3Str = TrackChange(jo.Value<string>("Track3"));
                        joket["biom"]["body"]["thirdTrack"] = track3Str;
                    }

                }

            }
            else
            {
                //log.InfoFormat("判断卡为【INVALID】");
                log.DebugFormat("获取 ChipStatus = {0} 异常...", joket);
                rSign = false;  // 异常卡
            }
            //log.DebugFormat("刷卡折 jo = {0}", joket);
            if (rSign)
            {
                // 刷卡折

                //string opStr = "{\"biom\":{\"head\":{\"tradeCode\":\"kazhejudge\",\"qmsIp\":\"127.88.111.1\"},\"body\":{\"cardFlag\":\"6\",\"secondTrack\":\"6232600200001260085D49126202779991108S\",\"thirdTrack\":\"\",\"certType\":\"107\",\"certNo\":\"\",\"custName\":\"\",\"secgs\":\"\",\"custTime\":\"\",\"nation\":\"\",\"office\":\"\",\"signDate\":\"\",\"indate\":\"\",\"addr\":\"\",\"sex\":\"\",\"birthday\":\"\",\"image\":\"\"}}}";


                //JObject jokk = JObject.Parse(opStr);
                //joket["biom"]=jokk["biom"];


                if (GetCache("isYyqhPage").Equals("0"))
                {
                    log.DebugFormat("首页-触发刷卡折操作...");
                    //  首页刷卡/折
                    custgetseqService.Kazhejudge2JS(joket);
                }
                else
                {
                    log.DebugFormat("预约界面-触发刷卡折操作...");

                    joket["biom"]["head"]["channel"] = "1";
                    joket["biom"]["head"]["tradeCode"] = "reservation";
                    joket["biom"]["body"]["PhoneNo"] = "";
                    //  预约界面
                    reservationService.Reservation2JS(joket);
                }
            }
            else
            {
                log.ErrorFormat("刷卡折异常...");

                string cardFlag = joket["biom"]["body"].Value<string>("cardFlag");
                if (cardFlag.Equals("3"))
                {
                    PlaySound2Machine("idCardFailure.wav");
                }
                else
                {
                    PlaySound2Machine("bankCardFailure.wav");
                }

                HideLoading();

                ReadRawData();
            }

            log.DebugFormat("end");
        }

        /// <summary>
        /// 磁道转换
        /// </summary>
        /// <param name="trackStr"></param>
        /// <returns></returns>
        private string TrackChange(string trackStr)
        {
            string newTrackStr = trackStr;
            if (trackStr.Contains("'"))
            {
                log.DebugFormat("卡磁道信息分隔符为 '，转换为=分隔符");
                newTrackStr = trackStr.Replace("'", "=");
            }
            else if (trackStr.Contains(">"))
            {
                log.DebugFormat("卡磁道信息分隔符为 >，转换为=分隔符");
                newTrackStr = trackStr.Replace(">", "=");
            }
            return newTrackStr;
        }

        /// <summary>
        /// 获取打印机状态
        /// </summary>
        public string GetPrintStatus2JS()
        {
            string statusStr = String.Empty;

            Invoke(new MethodInvoker(delegate ()
            {
                if (null != axPtrAx2Print)
                {
                    statusStr = axPtrAx2Print.GetStatus();

                    if (!JsonSplit.IsJson(statusStr))
                    {
                        statusStr = String.Empty;
                    }

                }
            }));

            return statusStr;

        }

        /// <summary>
        /// 打印 FROM
        /// </summary>
        /// <param name="pStr"></param>
        public void Print2JS(string joStr)
        {
            log.DebugFormat("begin");
            //"{\"Device\":\"HEALTHY\",\"Extra\":{\"ERRORDETAIL\":\"00000\"},\"Ink\":\"FULL\",\"Lamp\":\"OK\",\"Media\":\"PRESENT\",\"MediaOnStacker\":0,\"Paper\":\"FULL\",\"Toner\":\"FULL\"}";

            try
            {
                JObject jo = new JObject();

                JObject joket = JObject.Parse(joStr);
                jo["biom"] = joket["biom"];

                printAsyncOCX.BeginInvoke(jo, CallbackPrint, jo);
            }
            catch (Exception e)
            {
                log.Error("打印异常:" + e);
            }


            log.DebugFormat("end");
        }

        public void PrintTest(String printformat)
        {

            log.DebugFormat("begin");

            string retPrintFormat = "{\"biom\":{\"body\":{\"addr\":\"\",\"birthday\":\"\",\"cardFlag\":\"\",\"certNo\":\"\",\"certType\":\"\",\"custName\":\"\",\"custTime\":\"\",\"image\":\"\",\"indate\":\"\",\"leftCipher\":\"\",\"nation\":\"\",\"office\":\"\",\"printtemp\":\"\",\"secgs\":\"\",\"secondTrack\":\"\",\"sex\":\"\",\"signDate\":\"\",\"thirdTrack\":\"\"},\"head\":{\"retCode\":\"5\",\"retMsg\":\"直接打印票号信息\"}}}";

            if (printformat.Equals(String.Empty))
            {
                return;
            }

            JObject jo = new JObject();

            jo["biom"] = JObject.Parse(retPrintFormat)["biom"];

            jo["biom"]["body"]["printtemp"] = printformat;

            printAsyncOCX.BeginInvoke(jo, CallbackPrint, jo);

            log.DebugFormat("end");
        }


        public void Print(JObject jo)
        {
            log.DebugFormat("begin args: jo = {0}", LogEncryption2(jo));

            JObject joket = JObject.Parse(GlobalVariable2ICBC.RETURN_MESSAGE_STR);

            string statusStr = GetPrintStatus2JS();

            log.DebugFormat(" -> GetPrintStatus2JS, statusStr = {0}", statusStr);

            if (statusStr != String.Empty)
            {
                JObject joStatus = JObject.Parse(statusStr);

                // log.DebugFormat(" -> GetPrintStatus2JS, args: jo = {0}", jo);

                string deviceStr = joStatus.Value<string>("Device");

                string pageStr = joStatus.Value<string>("Paper");

                joket["biom"]["body"]["device"] = deviceStr;

                joket["biom"]["body"]["paper"] = pageStr;

                if (deviceStr.Equals("HEALTHY") && (pageStr.Equals("FULL") || pageStr.Equals("LOW")))
                {

                    string printFromStr = GlobalVariable2ICBC.PRINT_FORMP_STR;

                    if (JsonSplit.IsJson(printFromStr))
                    {
                        string pStr = jo["biom"]["body"].Value<string>("printtemp");

                        // "ticketNo=A10001|ticketNo=A10001|"
                        JObject joPrint = JObject.Parse(printFromStr);

                        string[] ticketStrs = pStr.Split('|');

                        int i = 1;
                        string fieldStr = "field1";
                        string temPrintStr = String.Empty;
                        foreach (string tStr in ticketStrs)
                        {
                            fieldStr = "field" + i;
                            temPrintStr = tStr.Replace("ticketNo=", "").Replace("buzWaitingCount=", "").Replace("buzCnname=", "").Replace("fetchedDate=", "").Replace("orgName=", "").Replace("warmPrompt=", "").Replace("vldCode=", "").Replace("secretCode=", "").Replace("star=", "");
                            joPrint["Fields"][fieldStr] = temPrintStr;
                            i++;
                        }


                        log.DebugFormat("记录打印的拼接Josn：{0}", joPrint);

                        //joPrint["Fields"]["field2"] = ticketStrs[0];
                        //joPrint["Fields"]["field4"] = ticketStrs[1];
                        //joPrint["Fields"]["field6"] = ticketStrs[2];
                        //joPrint["Fields"]["field9"] = ticketStrs[3];
                        //joPrint["Fields"]["field10"] = ticketStrs[4];
                        //joPrint["Fields"]["field11"] = ticketStrs[5];
                        //joPrint["Fields"]["field12"] = ticketStrs[6];
                        //joPrint["Fields"]["field13"] = ticketStrs[7];
                        //joPrint["Fields"]["field14"] = ticketStrs[8];

                        //                 string ticketTempStr= "              您的主办业务："+ticketStrs[0]+
                        //"\n              排队号码：^120"+ticketStrs[1]+"^000\n        您所在的排队队列前还有 ^120"+ticketStrs[2]+
                        //"^000 人等待\n          请您耐心留意叫号，过号作废。\n          排队验证码：" + ticketStrs[3] + "\n" + "----------------☆☆☆☆☆☆☆----------------" + "\n温馨提示：\n" + "          " +
                        //"1)天道酬勤；2）凡事看开" + "\n" + "----------------○○○○○○○----------------" + "\n" + "        " + ticketStrs[7] + ticketStrs[8] + "\n          " + "请妥善保管本凭条";


                        //log.DebugFormat("打印号票JOSN, args: jo = {0}", joPrint);

                        Invoke(new MethodInvoker(delegate ()
                        {
                            // 打印 
                            //if (axPtrAx2Print.PrintRawData(ticketTempStr))
                            //{
                            //    joket["biom"]["head"]["retCode"] = "0";
                            //    joket["biom"]["head"]["retMsg"] = "成功";
                            //    log.DebugFormat("打印成功!");
                            //    if (axPtrAx2Print.Eject(false))
                            //    {
                            //        log.DebugFormat("打印切纸成功!");
                            //    }
                            //    else
                            //    {
                            //        log.DebugFormat("打印切纸失败!");
                            //    }
                            //}
                            //else
                            //{
                            //    joket["biom"]["head"]["retCode"] = "1";
                            //    joket["biom"]["head"]["retMsg"] = "失败";
                            //    log.DebugFormat("打印失败!");
                            //}

                            // 打印 form
                            if (axPtrAx2Print.PrintForm(joPrint.ToString().Replace("\r\n", "")))
                            {
                                joket["biom"]["head"]["retCode"] = "0";
                                joket["biom"]["head"]["retMsg"] = "成功";
                                log.DebugFormat("打印成功!");
                                if (axPtrAx2Print.Eject(false))
                                {
                                    log.DebugFormat("打印切纸成功!");
                                }
                                else
                                {
                                    log.DebugFormat("打印切纸失败!");
                                }
                            }
                            else
                            {
                                joket["biom"]["head"]["retCode"] = "1";
                                joket["biom"]["head"]["retMsg"] = "失败";
                                log.DebugFormat("打印失败!");
                            }
                        }));

                    }
                    else
                    {
                        joket["biom"]["head"]["retCode"] = "1";
                        joket["biom"]["head"]["retMsg"] = "打印格式不正确";
                    }

                }
                else
                {
                    // 打印机故障，不出号票
                    joket["biom"]["head"]["retCode"] = "1";
                    joket["biom"]["head"]["retMsg"] = "打印机故障或缺纸";
                    log.DebugFormat("打印机故障或缺纸");
                }
            }

            jo["biom"] = joket["biom"];

            log.DebugFormat("end");
        }


        private void CallbackPrint(IAsyncResult ar)
        {
            log.DebugFormat("begin");
            JObject jo = (JObject)ar.AsyncState;

            jo["callback"] = "printTicketCallback";

            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);

            }
            catch (Exception e)
            {
                jo.RemoveAll();

                BuzConfig2ICBC.Jo2Return(jo);

                log.Error("AboutServiceImpl.Callback Error", e);
            }
            finally
            {
                ScriptInvoke(jo);
            }
        }

        public string GetPrinterStatus(string status)
        {
            string strRet = string.Empty;
            switch (status)
            {
                case "HEALTHY":
                    strRet = "正常";
                    break;
                case "NODEVICE":
                    strRet = "无效";
                    break;
                case "FATAL":
                    strRet = "异常";
                    break;
                case "UNKNOWN":
                    strRet = "未知";
                    break;
                case "PRESENT":
                    strRet = "准备";
                    break;
                case "NOTPRESENT":
                    strRet = "无效";
                    break;
                case "JAMMED":
                    strRet = "卡纸";
                    break;
                case "INJAWS":
                    strRet = "脱机";
                    break;
                case "FULL":
                    strRet = "完整";
                    break;
                case "LOW":
                    strRet = "缺纸";
                    break;
                case "OUT":
                    strRet = "溢出";
                    break;
                case "NOTSUPP":
                    strRet = "不足";
                    break;
                case "OK":
                    strRet = "正常";
                    break;
                case "FADING":
                    strRet = "退出";
                    break;
                case "INOP":
                    strRet = "失效";
                    break;
                default:
                    strRet = "未知";
                    break;
            }
            return strRet;
        }

        public void ReadRawDataInvoker(JObject jo)
        {
            //openAsyncCard.BeginInvoke(jo, CallbackReadRawData, jo);
        }

        /// <summary>
        /// 取消读卡
        /// </summary>
        public void CancelReadRawData()
        {
            try
            {
                if (AppState.PerSign)
                {
                    Invoke(new MethodInvoker(delegate ()
                    {

                        if (axIdcAx2Mifare.CancelReadRawData())
                        {
                            log.InfoFormat("取消读卡成功！");
                            AppState.PerSign = false;
                        }
                        else
                        {
                            log.ErrorFormat("取消读卡失败...");
                            //AppState.PerSign = false;
                        }
                    }));
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("取消读卡异常:" + e);
            }

        }

        /// <summary>
        /// 打开读卡操作
        /// </summary>
        public void ReadRawData()
        {
            try
            {
                if (!AppState.PerSign)
                {
                    Invoke(new MethodInvoker(delegate ()
                    {
                        if (axIdcAx2Mifare.ReadRawData(TRACKMAP, TIMEOUT))
                        {
                            log.InfoFormat("开启读卡成功！");
                            AppState.PerSign = true;
                        }
                        else
                        {
                            log.ErrorFormat("开启读卡失败...");
                            //AppState.PerSign = false;
                        }
                    }));
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("开启读卡异常:" + e);
            }
        }

        private void CallbackReadRawData(IAsyncResult ar)
        {
            JObject jo = (JObject)ar.AsyncState;

            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);

            }
            catch (Exception e)
            {
                jo.RemoveAll();


                log.Error("CallbackReadRawData.Callback Error", e);
            }
            finally
            {
                isBusy = false;
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            log.Debug("begin");


            if (null != axIdcAx2Mifare)
            {
                axIdcAx2Mifare.CloseConnection();

            }

            log.Debug("end");
        }

        private void OpenCardCallback(IAsyncResult ar)
        {
            JObject jo = (JObject)ar.AsyncState;

            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
            }
            catch (Exception e)
            {
                jo["result"] = ErrorCode.Failure;
                log.Error("Error", e);
            }
            finally
            {

            }
        }

        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            SetPopEvent("0");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetPopEvent("1");
        }




    }
}