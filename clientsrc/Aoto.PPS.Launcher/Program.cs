using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Configuration;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;

using log4net;
using Newtonsoft.Json.Linq;
using Aoto.CQMS.Core.Application;
using Aoto.PPS.Infrastructure.Utils;
using Aoto.CQMS.Core.Application.Impl;
using Aoto.CQMS.Core;
using Aoto.CQMS.Common.Sockets;
using System.Threading;
using Newtonsoft.Json;
using System.Web.Script.Serialization;



namespace Aoto.PPS.Launcher
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

            #region 临时测试用20190420 暂时未引用

            //String sDataLen = "{\"robot\":{\"body\":{\"addr\":\"江苏省丹阳市凤美新村14号508室\",\"birthday\":\"1995年12月03日\",\"busiType1\":\"2\",\"busiType2\":\"4\",\"cardFlag\":\"3\",\"certNo\":\"32118119951203001X\",\"certType\":\"0\",\"custName\":\"胡俊\",\"custTime\":\"\",\"firstTrack\":\"\",\"image\":\"\",\"indate\":\"2012.02.04-2022.02.04\",\"nation\":\"汉\",\"office\":\"丹阳市公安局\",\"secondTrack\":\"\",\"sex\":\"男\",\"signDate\":\"2012.02.04-2022.02.04\",\"thirdTrack\":\"\"},\"head\":{\"channel\":\"aoto\",\"orgCode\":\"\",\"robotID\":\"lvdawei\",\"tradeCode\":\"custgetseq\",\"transDt\":\"20190419172306000\"}}}";

            //var jsonObjDoc = JObject.Parse(sDataLen);

            //var requestTemp = JsonConvert.DeserializeObject<Aoto.CQMS.Common.JsonObj.CustgetseqJson.RequestJsonObject.Root>(sDataLen);

            //requestTemp.biom.head.qmsIp = "97.21.128.44";

            //var reqJson= JsonConvert.SerializeObject(requestTemp);

            //String tempStr = "";

            //tempStr = new JavaScriptSerializer().Serialize(requestTemp);


            // 接收

            String resStr = "{\"biom\":{\"body\":{\"addr\":\"（已屏蔽）\",\"birthday\":\"（已屏蔽）\",\"cardFlag\":\"3\",\"certNo\":\"3211811995120****X\",\"certType\":\"0\",\"custName\":\"胡*\",\"custTime\":\"\",\"image\":\"\",\"indate\":\"\",\"leftCipher\":\"A0066|0|00000000\",\"nation\":\"汉族\",\"office\":\"\",\"printtemp\":\"ticketNo=A0066|buzWaitingCount=2|buzCnname=存取款|fetchedDate=2019-04-21 14:48:24|warmPrompt=◆温馨提示\n|vldCode=25020103-1010 0662|orgName=工行昆明分行营业室|star=--------------☆☆☆☆☆☆☆--------------|secretCode=--------------○○○○○○○○--------------\",\"secgs\":\"\",\"secondTrack\":\"\",\"sex\":\"男\",\"signDate\":\"\",\"thirdTrack\":\"\"},\"head\":{\"retCode\":\"5\",\"retMsg\":\"直接打印票号信息\"}}}";

            var response = JsonConvert.DeserializeObject<CQMS.Common.JsonObj.CustgetseqJson.ResponseJsonObject.Root>(resStr);


            string responseJsonCustgetseq = new JavaScriptSerializer().Serialize(response);

            #endregion

            try
            {
                BuzConfig2ICBC.LocalIP = CommonUtils.GetHostAddresses();

                // 延后启动
                int deleyedStartTime= Config.App.DelayedStartTime;
                log.Debug("Delayed start : " + deleyedStartTime);
                Thread.Sleep(deleyedStartTime * 1000);

                // 测试数据
                GlobalVariable2ICBC.ICBC_QMSSIGN = JObject.Parse("{\"biom\":{\"head\":{\"retCode\":\"0\",\"retMsg\":\"成功\"},\"body\":{\"carList\":{\"carRecord\":[{\"cardBin\":\"451810\",\"servelevel\":\"6\"},{\"cardBin\":\"6222\",\"servelevel\":\"6\"},{\"cardBin\":\"95588\",\"servelevel\":\"4\"}]},\"cliProPrintFlag\":\"1\",\"cliTimPrintFlag\":\"1\",\"cusLevelPrintFlag\":\"1\",\"cusLevelShowFlag\":\"1\",\"cusShowFlag\":\"1\",\"delayedStartTime\":\"\",\"devStatus\":\"1|0|1|0\",\"dutyFlag\":\"1\",\"dutySwitchPwd\":\"147\",\"exitGetTicketPwd\":\"456\",\"flPrintFlag\":\"1\",\"notePrintFlag\":\"1\",\"onlineSwitchPwd\":\"789\",\"pageTimeout\":\"\",\"perShowFlag\":\"1\",\"ptShowFlag\":\"1\",\"qmsRecNote\":\"\",\"qmsUseFlag\":\"0\",\"queList\":{\"queRecord\":[{\"busiNo\":\"236\",\"queueNo\":\"A\",\"servelevel\":\"0\"},{\"busiNo\":\"236\",\"queueNo\":\"A\",\"servelevel\":\"3\"},{\"busiNo\":\"236\",\"queueNo\":\"A\",\"servelevel\":\"4\"},{\"busiNo\":\"236\",\"queueNo\":\"A\",\"servelevel\":\"5\"},{\"busiNo\":\"236\",\"queueNo\":\"A\",\"servelevel\":\"6\"},{\"busiNo\":\"236\",\"queueNo\":\"A\",\"servelevel\":\"7\"},{\"busiNo\":\"237\",\"queueNo\":\"C\",\"servelevel\":\"0\"},{\"busiNo\":\"237\",\"queueNo\":\"C\",\"servelevel\":\"3\"},{\"busiNo\":\"237\",\"queueNo\":\"C\",\"servelevel\":\"4\"},{\"busiNo\":\"237\",\"queueNo\":\"C\",\"servelevel\":\"5\"},{\"busiNo\":\"237\",\"queueNo\":\"C\",\"servelevel\":\"6\"},{\"busiNo\":\"237\",\"queueNo\":\"C\",\"servelevel\":\"7\"},{\"busiNo\":\"233\",\"queueNo\":\"D\",\"servelevel\":\"0\"},{\"busiNo\":\"233\",\"queueNo\":\"D\",\"servelevel\":\"3\"},{\"busiNo\":\"233\",\"queueNo\":\"D\",\"servelevel\":\"4\"},{\"busiNo\":\"233\",\"queueNo\":\"D\",\"servelevel\":\"5\"},{\"busiNo\":\"233\",\"queueNo\":\"D\",\"servelevel\":\"6\"},{\"busiNo\":\"233\",\"queueNo\":\"D\",\"servelevel\":\"7\"},{\"busiNo\":\"235\",\"queueNo\":\"E\",\"servelevel\":\"0\"},{\"busiNo\":\"235\",\"queueNo\":\"E\",\"servelevel\":\"3\"},{\"busiNo\":\"235\",\"queueNo\":\"E\",\"servelevel\":\"4\"},{\"busiNo\":\"235\",\"queueNo\":\"E\",\"servelevel\":\"5\"},{\"busiNo\":\"235\",\"queueNo\":\"E\",\"servelevel\":\"6\"},{\"busiNo\":\"235\",\"queueNo\":\"E\",\"servelevel\":\"7\"},{\"busiNo\":\"239\",\"queueNo\":\"E\",\"servelevel\":\"0\"},{\"busiNo\":\"239\",\"queueNo\":\"E\",\"servelevel\":\"3\"},{\"busiNo\":\"239\",\"queueNo\":\"E\",\"servelevel\":\"4\"},{\"busiNo\":\"239\",\"queueNo\":\"E\",\"servelevel\":\"5\"},{\"busiNo\":\"239\",\"queueNo\":\"E\",\"servelevel\":\"6\"},{\"busiNo\":\"239\",\"queueNo\":\"E\",\"servelevel\":\"7\"},{\"busiNo\":\"234\",\"queueNo\":\"G\",\"servelevel\":\"0\"},{\"busiNo\":\"234\",\"queueNo\":\"G\",\"servelevel\":\"3\"},{\"busiNo\":\"234\",\"queueNo\":\"G\",\"servelevel\":\"4\"},{\"busiNo\":\"234\",\"queueNo\":\"G\",\"servelevel\":\"5\"},{\"busiNo\":\"234\",\"queueNo\":\"G\",\"servelevel\":\"6\"},{\"busiNo\":\"234\",\"queueNo\":\"G\",\"servelevel\":\"7\"},{\"busiNo\":\"238\",\"queueNo\":\"H\",\"servelevel\":\"0\"},{\"busiNo\":\"238\",\"queueNo\":\"H\",\"servelevel\":\"3\"},{\"busiNo\":\"238\",\"queueNo\":\"H\",\"servelevel\":\"4\"},{\"busiNo\":\"238\",\"queueNo\":\"H\",\"servelevel\":\"5\"},{\"busiNo\":\"238\",\"queueNo\":\"H\",\"servelevel\":\"6\"},{\"busiNo\":\"238\",\"queueNo\":\"H\",\"servelevel\":\"7\"},{\"busiNo\":\"240\",\"queueNo\":\"H\",\"servelevel\":\"0\"},{\"busiNo\":\"240\",\"queueNo\":\"H\",\"servelevel\":\"3\"},{\"busiNo\":\"240\",\"queueNo\":\"H\",\"servelevel\":\"4\"},{\"busiNo\":\"240\",\"queueNo\":\"H\",\"servelevel\":\"5\"},{\"busiNo\":\"240\",\"queueNo\":\"H\",\"servelevel\":\"6\"},{\"busiNo\":\"240\",\"queueNo\":\"H\",\"servelevel\":\"7\"},{\"busiNo\":\"817\",\"queueNo\":\"I\",\"servelevel\":\"3\"},{\"busiNo\":\"817\",\"queueNo\":\"I\",\"servelevel\":\"4\"},{\"busiNo\":\"817\",\"queueNo\":\"I\",\"servelevel\":\"5\"},{\"busiNo\":\"817\",\"queueNo\":\"I\",\"servelevel\":\"6\"},{\"busiNo\":\"817\",\"queueNo\":\"I\",\"servelevel\":\"7\"},{\"busiNo\":\"877\",\"queueNo\":\"I\",\"servelevel\":\"3\"},{\"busiNo\":\"877\",\"queueNo\":\"I\",\"servelevel\":\"4\"},{\"busiNo\":\"877\",\"queueNo\":\"I\",\"servelevel\":\"5\"},{\"busiNo\":\"877\",\"queueNo\":\"I\",\"servelevel\":\"6\"},{\"busiNo\":\"877\",\"queueNo\":\"I\",\"servelevel\":\"7\"}]},\"remind\":\"温馨提示：为了节省您的时间，如果您需要办理2万元以下卡存、取业务，请至我行ATM机自助处理；如果您需要办理账户查询、卡内转账汇款、分行特色缴费等业务，请于我行自助终端办理。\",\"remindEn\":\"Prompt: To save your time, for card deposit or withdrawal within RMB20,000, use ATM. For account inquiry, card transfer and remittance, featured bill payment and others, use self-service terminal.\",\"remindFlag\":\"1\",\"sctCusBusiList\":{\"sctCusBusi\":[{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"对公业务\",\"busiNameEn\":\"\",\"busiNo\":\"817\",\"flnote\":\"请自觉排队等候叫号。\",\"flnoteEn\":\"\",\"isDuty\":\"1\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"1\",\"suBusiCode\":\"07\",\"suBusiName\":\"理财顾问\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"对公理财\",\"busiNameEn\":\"\",\"busiNo\":\"877\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"0\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"07\",\"suBusiName\":\"理财顾问\"}]},\"sctNoBusiList\":{\"sctNoBusi\":[{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"开销户\",\"busiNameEn\":\"\",\"busiNo\":\"233\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"1\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"01\",\"suBusiName\":\"开销户\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"转帐汇款\",\"busiNameEn\":\"\",\"busiNo\":\"234\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"0\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"02\",\"suBusiName\":\"转账汇款\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"理财\",\"busiNameEn\":\"\",\"busiNo\":\"235\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"0\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"03\",\"suBusiName\":\"理财\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"其他业务\",\"busiNameEn\":\"\",\"busiNo\":\"240\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"0\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"08\",\"suBusiName\":\"其他业务\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"缴费\",\"busiNameEn\":\"\",\"busiNo\":\"237\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"0\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"05\",\"suBusiName\":\"缴费\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"外币\",\"busiNameEn\":\"\",\"busiNo\":\"238\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"0\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"06\",\"suBusiName\":\"外币\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"理财顾问\",\"busiNameEn\":\"\",\"busiNo\":\"239\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"0\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"07\",\"suBusiName\":\"理财顾问\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"存取款\",\"busiNameEn\":\"\",\"busiNo\":\"236\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"0\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"04\",\"suBusiName\":\"存取款\"}]},\"sctPerBusiList\":{\"sctPerBusi\":[{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"开销户\",\"busiNameEn\":\"\",\"busiNo\":\"233\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"1\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"01\",\"suBusiName\":\"开销户\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"转帐汇款\",\"busiNameEn\":\"\",\"busiNo\":\"234\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"1\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"02\",\"suBusiName\":\"转账汇款\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"理财\",\"busiNameEn\":\"\",\"busiNo\":\"235\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"1\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"03\",\"suBusiName\":\"理财\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"其他业务\",\"busiNameEn\":\"\",\"busiNo\":\"240\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"1\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"08\",\"suBusiName\":\"其他业务\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"缴费\",\"busiNameEn\":\"\",\"busiNo\":\"237\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"1\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"05\",\"suBusiName\":\"缴费\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"外币\",\"busiNameEn\":\"\",\"busiNo\":\"238\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"1\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"06\",\"suBusiName\":\"外币\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"理财顾问\",\"busiNameEn\":\"\",\"busiNo\":\"239\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"1\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"07\",\"suBusiName\":\"理财顾问\"},{\"ameTime\":\"\",\"amsTime\":\"\",\"busiName\":\"存取款\",\"busiNameEn\":\"\",\"busiNo\":\"236\",\"flnote\":\"\",\"flnoteEn\":\"\",\"isDuty\":\"1\",\"pmeTime\":\"\",\"pmsTime\":\"\",\"qmsFlag\":\"0\",\"suBusiCode\":\"04\",\"suBusiName\":\"存取款\"}]},\"shutdownPwd\":\"123\",\"spcliPrintFlag\":\"1\",\"synchroTime\":\"1970-02-15 06:56:21\",\"winList\":{\"winRecord\":[{\"winCode\":\"0000001618\",\"winIp\":\"122.133.29.93\",\"winLatTac\":\"2\",\"winName\":\"王绵锗叫号窗口\",\"winPreTac\":\"3\",\"winTac\":\"1\"},{\"winCode\":\"0000001637\",\"winIp\":\"122.136.13.118\",\"winLatTac\":\"1\",\"winName\":\"邹懿的窗口\",\"winPreTac\":\"1\",\"winTac\":\"1\"},{\"winCode\":\"0000001817\",\"winIp\":\"122.136.13.156\",\"winLatTac\":\"1\",\"winName\":\"梁铃叫号窗口\",\"winPreTac\":\"1\",\"winTac\":\"1\"},{\"winCode\":\"0000001617\",\"winIp\":\"122.136.13.173\",\"winLatTac\":\"1\",\"winName\":\"王智安叫号窗口\",\"winPreTac\":\"1\",\"winTac\":\"1\"},{\"winCode\":\"0000001777\",\"winIp\":\"122.138.45.1\",\"winLatTac\":\"4\",\"winName\":\"浪潮窗口1\",\"winPreTac\":\"1\",\"winTac\":\"2\"},{\"winCode\":\"0000001778\",\"winIp\":\"122.138.45.2\",\"winLatTac\":\"5\",\"winName\":\"浪潮窗口2\",\"winPreTac\":\"3\",\"winTac\":\"1\"},{\"winCode\":\"0000001837\",\"winIp\":\"122.138.45.223\",\"winLatTac\":\"1\",\"winName\":\"广州3楼浪潮叫号窗口\",\"winPreTac\":\"1\",\"winTac\":\"1\"},{\"winCode\":\"0000001757\",\"winIp\":\"122.2.13.190\",\"winLatTac\":\"1\",\"winName\":\"邓立恒窗口\",\"winPreTac\":\"1\",\"winTac\":\"1\"}]},\"winQueList\":{\"winQueRecord\":[{\"aptque\":\"B0\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"A\",\"queueSeq\":\"1\",\"winCode\":\"0000001757\"},{\"aptque\":\"B0\",\"num\":\"60\",\"preFlag\":\"1\",\"queueNo\":\"A\",\"queueSeq\":\"1\",\"winCode\":\"0000001637\"},{\"aptque\":\"B2\",\"num\":\"60\",\"preFlag\":\"1\",\"queueNo\":\"D\",\"queueSeq\":\"3\",\"winCode\":\"0000001637\"},{\"aptque\":\"F0\",\"num\":\"\",\"preFlag\":\"1\",\"queueNo\":\"E\",\"queueSeq\":\"2\",\"winCode\":\"0000001618\"},{\"aptque\":\"B2\",\"num\":\"5\",\"preFlag\":\"3\",\"queueNo\":\"D\",\"queueSeq\":\"1\",\"winCode\":\"0000001618\"},{\"aptque\":\"H0\",\"num\":\"6\",\"preFlag\":\"2\",\"queueNo\":\"G\",\"queueSeq\":\"2\",\"winCode\":\"0000001618\"},{\"aptque\":\"B2\",\"num\":\"5\",\"preFlag\":\"3\",\"queueNo\":\"J\",\"queueSeq\":\"1\",\"winCode\":\"0000001617\"},{\"aptque\":\"B0\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"A\",\"queueSeq\":\"1\",\"winCode\":\"0000001618\"},{\"aptque\":\"F1\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"E\",\"queueSeq\":\"3\",\"winCode\":\"0000001617\"},{\"aptque\":\"F0\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"H\",\"queueSeq\":\"5\",\"winCode\":\"0000001617\"},{\"aptque\":\"B3\",\"num\":\"5\",\"preFlag\":\"1\",\"queueNo\":\"A\",\"queueSeq\":\"1\",\"winCode\":\"0000001617\"},{\"aptque\":\"B1\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"D\",\"queueSeq\":\"2\",\"winCode\":\"0000001617\"},{\"aptque\":\"I1\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"G\",\"queueSeq\":\"4\",\"winCode\":\"0000001617\"},{\"aptque\":\"B3\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"A\",\"queueSeq\":\"1\",\"winCode\":\"0000001837\"},{\"aptque\":\"B9\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"C\",\"queueSeq\":\"2\",\"winCode\":\"0000001837\"},{\"aptque\":\"B1\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"D\",\"queueSeq\":\"3\",\"winCode\":\"0000001837\"},{\"aptque\":\"F1\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"E\",\"queueSeq\":\"4\",\"winCode\":\"0000001837\"},{\"aptque\":\"I1\",\"num\":\"5\",\"preFlag\":\"1\",\"queueNo\":\"G\",\"queueSeq\":\"1\",\"winCode\":\"0000001837\"},{\"aptque\":\"F0\",\"num\":\"5\",\"preFlag\":\"3\",\"queueNo\":\"H\",\"queueSeq\":\"1\",\"winCode\":\"0000001837\"},{\"aptque\":\"B2\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"J\",\"queueSeq\":\"5\",\"winCode\":\"0000001837\"},{\"aptque\":\"B9\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"C\",\"queueSeq\":\"1\",\"winCode\":\"0000001617\"},{\"aptque\":\"B1\",\"num\":\"3\",\"preFlag\":\"2\",\"queueNo\":\"C\",\"queueSeq\":\"5\",\"winCode\":\"0000001757\"},{\"aptque\":\"B2\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"D\",\"queueSeq\":\"4\",\"winCode\":\"0000001757\"},{\"aptque\":\"B1\",\"num\":\"3\",\"preFlag\":\"1\",\"queueNo\":\"A\",\"queueSeq\":\"1\",\"winCode\":\"0000001817\"},{\"aptque\":\"B0\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"D\",\"queueSeq\":\"2\",\"winCode\":\"0000001817\"},{\"aptque\":\"F1\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"G\",\"queueSeq\":\"4\",\"winCode\":\"0000001817\"},{\"aptque\":\"F4\",\"num\":\"3\",\"preFlag\":\"2\",\"queueNo\":\"I\",\"queueSeq\":\"6\",\"winCode\":\"0000001817\"},{\"aptque\":\"B2\",\"num\":\"4\",\"preFlag\":\"2\",\"queueNo\":\"C\",\"queueSeq\":\"1\",\"winCode\":\"0000001817\"},{\"aptque\":\"F0\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"E\",\"queueSeq\":\"3\",\"winCode\":\"0000001817\"},{\"aptque\":\"F3\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"H\",\"queueSeq\":\"5\",\"winCode\":\"0000001817\"},{\"aptque\":\"B3\",\"num\":\"5\",\"preFlag\":\"2\",\"queueNo\":\"E\",\"queueSeq\":\"2\",\"winCode\":\"0000001757\"}]},\"yxPrintFlag\":\"1\"}},\"callback\":null}");

                ServicePointManager.DefaultConnectionLimit = 4;
                caption = Config.App.Name + " " + Config.App.Version;

                log.InfoFormat("Launcher caption = {0}", caption);

                if (IsRuning())
                {
                    log.InfoFormat("应用程序已经运行");
                    //MessageBox.Show("应用程序已经运行", caption, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }

                string dir = Config.App.Peripheral.Value<string>("dir");

                string libPath = Path.Combine(Config.PeripheralAbsolutePath, dir, "lib");

                log.InfoFormat("libDir = {0}", libPath);

                string dllPath = Path.Combine(Config.PeripheralAbsolutePath, dir);

                log.InfoFormat("dllDir = {0}", dllPath);

                string envPath = Environment.GetEnvironmentVariable("PATH") + ";" + dllPath + ";" + libPath;

                Environment.SetEnvironmentVariable("PATH", envPath);

                log.InfoFormat("EnvironmentVariable PATH = {0}", envPath);

                Application.EnableVisualStyles();

                Application.SetCompatibleTextRenderingDefault(false);

                DialogResult dialogResult = FrmSelfChecked.Instance.ShowDialog();

                if (DialogResult.OK != dialogResult)
                {
                    return;
                }

                if (Config.App.Online)
                {
                    Heartbeat heartbeat = AutofacContainer.ResolveNamed<Heartbeat>("heartbeat");
                    heartbeat.Start();

                    ShutdownSevice shutdown = AutofacContainer.ResolveNamed<ShutdownSevice>("shutdownSevice");
                    shutdown.Start();
                }

                //IAppInitializer appInitializer = AutofacContainer.ResolveNamed<IAppInitializer>("appInitializer");
                //appInitializer.Initializing();

                Form formShell = (Form)AutofacContainer.ResolveNamed("scriptInvoker");

                //开启socket监听 Modify by 俞超梁
                SocketServer.Instance.Start(BuzConfig2ICBC.LocalIP, System.Configuration.ConfigurationManager.AppSettings["SocketLinstenPort"]);
                
                Application.Run(formShell);


                //appInitializer.Closing();

                JObject jo = new JObject();

                //if (OperationCommand.Shutdown == AppState.Command)
                //{
                //    systemService.Shutdown(jo);
                //}
                //else if (OperationCommand.Restart == AppState.Command)
                //{
                //    systemService.Restart(jo);
                //}
                //else if (OperationCommand.AppRestart == AppState.Command)
                //{
                //    Process.Start(Config.UpdateExeAbsolutePath);
                //}
            }
            catch (Exception e)
            {
                log.Error("Launcher Main error", e);
                MessageBox.Show("应用程序运行发生错误，请联系管理员", caption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

            log.Debug("end");

            Environment.Exit(0);
        }
        //进程集合
        private static bool IsRuning()
        {
            Process[] processes = Process.GetProcessesByName("Aoto.PPS.Launcher");
            return (processes.Length > 1);
        }
    }
}
