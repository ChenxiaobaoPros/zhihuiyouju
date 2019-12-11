using Aoto.EMS.Common;
using Aoto.EMS.Infrastructure;
using Aoto.EMS.Infrastructure.ComponentModel;
using Aoto.EMS.Infrastructure.Configuration;
using Aoto.EMS.Peripheral;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Aoto.EMS.MultiSerBox
{
    [ComVisible(true)]
    public partial class FrmShell : Form, IScriptInvoker
    {
        private PeripheralManager peripheralManager;
        public FrmShell()
        {
            InitializeComponent();
            webBrowser.ObjectForScripting = this;
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.ScrollBarsEnabled = true;

            //SetDeiveState("LoRaPlug",new string[]{ "GEK9320663" },"1");
        }
        public void Shut()
        {
            throw new NotImplementedException();
        }
        public void ResetPrint()
        {
            throw new NotImplementedException();
        }
        public void ReadRawDataInvoker(JObject jo)
        {
            throw new NotImplementedException();
        }
        public void Navigate(string url)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                webBrowser.Navigate(url);
            }));
        }
        public void PeripheralInvoke(JObject jo)
        {
            throw new NotImplementedException();
        }
        public void ScriptInvoke(JObject jo)
        {
            string callback = jo["callback"].ToString();

            Invoke(new MethodInvoker(delegate ()
            {
                if (!String.IsNullOrWhiteSpace(callback))
                {
                    //log.DebugFormat("触发页面回调..............{0}", callback);
                    webBrowser.Document.InvokeScript(callback, new object[] { jo.ToString(Formatting.None) });
                }
            }));
        }

        private void FrmShell_Load(object sender, EventArgs e)
        {
            //webBrowser.Navigate(Path.Combine(Config.AppRoot, "web\\qms\\html\\admin\\index.html"));
            webBrowser.Navigate(AppState.WelcomeUrl);
            //webBrowser.Navigate(Path.Combine(Config.AppRoot, "wulianwang\\html\\inde.html"));
            peripheralManager = new PeripheralManager();//也可以用反射

            peripheralManager.ThermalPrinter.PrintAsync(new JObject());
        }

        #region 多功能业务柜

        #region 页面数据
        private static List<Product> list = new List<Product>() {
            new Product() { BusinessType = "养老保险",BusinessName = "sdsd1",BusinessNameNo = "sdsdsd",RealBusiness = "sds"},
            new Product() { BusinessType = "养老保险",BusinessName = "sdsd2",BusinessNameNo = "sdsdsd",RealBusiness = "sds"},
            new Product() { BusinessType = "人寿保险",BusinessName = "sdsd3",BusinessNameNo = "sdsdsd",RealBusiness = "sds"},
            new Product() { BusinessType = "人寿保险",BusinessName = "sdsd4",BusinessNameNo = "sdsdsd",RealBusiness = "sds"},
            new Product() { BusinessType = "车辆保险",BusinessName = "sdsd5",BusinessNameNo = "sdsdsd",RealBusiness = "sds"},
            new Product() { BusinessType = "车辆保险",BusinessName = "sdsd6",BusinessNameNo = "sdsdsd",RealBusiness = "sds"},
        };
        private static PageData PageData = new PageData() { businesslist = list };
        /// <summary>
        /// 加载业务类型
        /// </summary>
        /// <returns></returns>
        public string LoadBussisTypes()
        {
            return JsonConvert.SerializeObject(PageData.BussisTypes);
        }
        /// <summary>
        /// 加载业务产品
        /// </summary>
        /// <returns></returns>
        public string LoadBussisProduct()
        {
            return JsonConvert.SerializeObject(list);
        }
        public string LoadBussisPage()
        {
            string str = JsonConvert.SerializeObject(PageData);
            return str;
        }
        #endregion

        #region 页面操作
        //办理业务
        private string bussisType { get; set; }
        public string HandleBuss(string bus)
        {
            JObject jo = JObject.Parse(bus);
            IEnumerable<Product> quelist = list.Where(b => b.BusinessName == jo.Value<string>("BusinessName"));
            bussisType = quelist.FirstOrDefault().BusinessType;
            return "0";
        }
        //查询业务
        public string SeachBuss(string type)
        {
            IEnumerable<Product> products = list.Where(b => b.BusinessType == type);
            return JsonConvert.SerializeObject(products);
        }
        #endregion

        #region 身份证读取
        public void InitIDCard()
        {
            peripheralManager.IDCardReader.ReadAsync(new JObject());
        }
        #endregion

        #region 二维码
        public void loadQRCode()
        {
            //创建就开启了
            //peripheralManager.QRCode.Read(new JObject());
        }
        public void closeQRCode()
        {

        }


        #endregion

        #region 指纹
        public void loadFinger()
        {
            peripheralManager.Finger.Read(new JObject());
        }
        #endregion

        #region 键盘
        public void loadKeyBoard()
        {
            peripheralManager.KeyBoard.Read(new JObject());
        }

        #endregion

        #region 签字板
        public void LoadBorad()
        {
            DialogResult dialogResult = FrmBoard.Instance.ShowDialog();
            if (DialogResult.OK != dialogResult)
            {
                return;
            }

        }
        #endregion

        #region 吸卡器
        HybridCardReader hybridCardReader;
        public string loadMoveCard()
        {
            if (hybridCardReader == null)
                hybridCardReader = new HybridCardReader();
            hybridCardReader.BackCard();
            return null;
        }
        #endregion

        #region 高拍仪

        #endregion

        #endregion

        #region 物联网    尚未加锁

        #region 开启设备
        //公共变量
        public string api_token = "39e58bd22275ccca486a3c3a9e00e578";
        public string accessToken = "";
        public string url = "";
        public string jsonRequest = "";
        public string jsonResponse = "";
        public JObject responseJo;
        public JObject requestJo;

        public List<App> appList { get; set; }
        public JObject accessTokenJo;
        public JObject appListJo;
        public JObject deiveListJo;
        public JObject deiveJo;

        public JObject allJo;
        //设备类型数组
        public string[] type_names = { "433Changer"/*433转换器*/, "LoRaDoorLock"/*LoRa门锁*/, "MPM4780_Transmitters"/*MPM4780智能变送器*/, "YBL-SingleFireThreeKeySwitch"/*单火三路开关控制器*/, "ThreeWayLoraSwitch"/*智能三路触控开关*/, "BODY_SENSOR"/*智能三鉴探测器*/, "OneWayLoraSwitch"/*智能单路触控开关*/, "TwoWayLoraSwitch"/*智能双路触控开关*/, "LoRaPlug"/*智能插座*/, "Liquid_Transmitters"/*智能液位变送器*/, "LoRaTempHumid"/*温湿度传感器*/, "temperature-sensor"/*温湿度传感器-XW-TH-B/BO-485*/, "fluid-leakage-controller"/*漏液不定位传感器*/, "positioning-fluid-leakage-controller"/*漏液定位传感器*/, "SmokeSensor"/*烟雾传感器*/, "LoRaAirPanel"/*空调面板*/, "human-detection"/*红外人体检测传感器*/};
        //1、token
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        public JObject GetAccessToken(string api_token)
        {
            url = "api/v1/accessToken";
            //jsonRequest = "{\"api_token\": \"39e58bd22275ccca486a3c3a9e00e578\"}";
            JObject requestJo = new JObject(
                new JProperty("api_token", api_token));
            jsonRequest = requestJo.ToString();
            jsonResponse = HttpClient.IOTPost(url, jsonRequest);

            accessTokenJo = JObject.Parse(jsonResponse);

            return accessTokenJo;
        }
        /// <summary>
        /// 获取简单应用列表
        /// </summary>
        /// <returns></returns>
        public JObject GetAppList(string accessTaken)
        {
            url = "api/v1/application/getAppList";
            //jsonRequest = "{\"current\": 1, \"rowCount\": 20}";
            JObject requestJo = new JObject(
                new JProperty("current", 1),
                new JProperty("rowCount", 20));
            jsonRequest = requestJo.ToString();
            jsonResponse = HttpClient.IOTPost(url, accessTaken, jsonRequest);

            return JObject.Parse(jsonResponse);
        }
        /// <summary>
        /// 获取指定应用下的设备列表
        /// </summary>
        /// <returns></returns>
        public JObject GetDeiveList(string accessTaken, int app_id, string[] type_names)
        {
            url = "api/v1/device/getGroup";
            //jsonRequest = "{\"app_id\": 2, \"type_names\": [\"LoRaTempHumid\",\"human-detection\",\"LoRaPlug\"]}";
            JObject requestJo = new JObject(
                new JProperty("app_id", app_id),
                new JProperty("type_names", type_names));
            jsonRequest = requestJo.ToString();
            jsonResponse = HttpClient.IOTPost(url, accessTaken, jsonRequest);

            return JObject.Parse(jsonResponse);
        }
        /// <summary>
        /// 得到状态
        /// </summary>
        /// <param name="jo"></param>
        /// <returns></returns>
        public JObject GetState(string accessTaken, JObject jo)
        {
            string url = "api/v1/device/getStatus";
            //string jsonRequest = "{'LoraPlug': {'all': false, 'group_ids': [ ], 'device_ids': ['GEK6510018','fake_sn']}}"; //all 是否忽略device_ids和group_ids
            string jsonResponse = HttpClient.IOTPost(url, accessTaken, jo.ToString());
            return JObject.Parse(jsonResponse);

        }

        public string SetDeiveState(string typeNmae,string[] idArray,string code)
        {
            string url = "api/v1/device/setStatus";
            accessTokenJo = GetAccessToken(api_token);
            if (accessTokenJo.Value<int>("expiresIn") <= 100)
            {
                //超时
            }
            string accessToken = accessTokenJo["accessToken"].ToString();
            requestJo = new JObject(new JProperty(typeNmae, new JObject(
                new JProperty("device_ids", new JArray(idArray)),
                new JProperty("data", new JObject(new JProperty("DEV_SWITCH_STA", code)))
                )));
            string jsonResponse = HttpClient.IOTPost(url, accessToken, requestJo.ToString());

            //校验

            //返回

            return jsonResponse;
        }

        public void LoadStaticPage()
        {
            string[] type_names = { "433Changer"/*433转换器*/, "LoRaDoorLock"/*LoRa门锁*/, "MPM4780_Transmitters"/*MPM4780智能变送器*/, "YBL-SingleFireThreeKeySwitch"/*单火三路开关控制器*/, "ThreeWayLoraSwitch"/*智能三路触控开关*/, "BODY_SENSOR"/*智能三鉴探测器*/, "OneWayLoraSwitch"/*智能单路触控开关*/, "TwoWayLoraSwitch"/*智能双路触控开关*/, "LoRaPlug"/*智能插座*/, "Liquid_Transmitters"/*智能液位变送器*/, "LoRaTempHumid"/*温湿度传感器*/, "temperature-sensor"/*温湿度传感器-XW-TH-B/BO-485*/, "fluid-leakage-controller"/*漏液不定位传感器*/, "positioning-fluid-leakage-controller"/*漏液定位传感器*/, "SmokeSensor"/*烟雾传感器*/, "LoRaAirPanel"/*空调面板*/, "human-detection"/*红外人体检测传感器*/};
            //1、token
            accessTokenJo = GetAccessToken(api_token);
            if (accessTokenJo.Value<int>("expiresIn") <= 100)
            {
                //超时
            }
            string accessToken = accessTokenJo["accessToken"].ToString();
            //3、applist所有应用
            appListJo = GetAppList(accessToken);
            List<App> applist = new List<App>();
            IEnumerable<JToken> jAppProperties = deiveListJo["data"]["apps"].Ancestors();
            foreach (JToken jt in jAppProperties)
            {
                App app = new App()
                {
                    appID = jt.Value<int>("appID"),
                    appName = jt.Value<string>("appName"),
                    deiveList = new List<Deive>()
                };
                //4、当前应用下的所有设备
                deiveListJo = GetDeiveList(accessToken, app.appID, type_names);
                IEnumerable<JProperty> jDevProperties = (deiveListJo["data"]["children"] as JObject).Properties();//设备集合
                foreach (JProperty jp in jDevProperties)
                {
                    deiveJo = JObject.Parse(jp.Value.ToString());
                    Deive deive = new Deive()
                    {
                        deviceId = deiveJo.Value<string>("deviceId"),
                        deviceName = deiveJo.Value<string>("deviceName"),
                        typeName = deiveJo.Value<string>("typeName"),
                    };

                    app.deiveList.Add(deive);
                }
                applist.Add(app);
            }
        }
        /// <summary>
        /// 加载概览
        /// </summary>
        /// <returns></returns>
        public string LoadOverView()
        {
            accessTokenJo = GetAccessToken(api_token);
            if (accessTokenJo.Value<int>("expiresIn") <= 100)
            {
                //超时
                return "";
            }
            accessToken = accessTokenJo["accessToken"].ToString();

            //2、查询所有设备状态  所有应用，所有设备
            JObject requestJo = new JObject();
            foreach (string typeName in type_names)
            {
                requestJo.Add(
                    new JProperty(typeName,
                        new JObject(
                            new JProperty("all", true),
                            new JProperty("group_ids", new JArray()),
                            new JProperty("device_ids", new JArray())
                            )));
            }
            allJo = GetState(accessToken, requestJo);//所有类型，所有设备
            IEnumerable<JProperty> jTypeProperties = allJo.Properties();

            #region 解析
            //appList = new List<App>();
            //foreach (JProperty jp in jTypeProperties)
            //{
            //    if (jp.Value.Count() != 0)
            //    {
            //        IEnumerable<JProperty> jDevProperties = (jp.Value as JObject).Properties(); //获取该类型所有设备

            //        foreach (JProperty jdp in jDevProperties)
            //        {
            //           IEnumerable<App> querylist= appList.Where(j => j.appID == jdp.Value.Value<int>("app_id"));
            //            if (querylist != null && querylist.Count()>0)
            //            {
            //                querylist.FirstOrDefault().deiveList.Add(new Deive()
            //                {
            //                    deviceId = jdp.Path.Split('.')[1],
            //                    deviceName = jdp.Value.Value<string>("name"),
            //                    typeName = jp.Path,
            //                    state = jdp.Value.Value<string>("states")
            //                });
            //            }
            //            else
            //            {
            //                App app = new App()
            //                {
            //                    appID = jdp.Value.Value<int>("app_id"),
            //                    appName = jdp.Value.Value<string>("app_name"),
            //                    deiveList = new List<Deive>()
            //                };
            //                app.deiveList.Add(new Deive()
            //                {
            //                    deviceId = jdp.Path.Split('.')[1],
            //                    deviceName = jdp.Value.Value<string>("name"),
            //                    typeName = jp.Path,
            //                    state = jdp.Value.Value<string>("states")
            //                });
            //                appList.Add(app);
            //            }
            //        }
            //    }

            //}
            //string jsonStr = JsonConvert.SerializeObject(appList);
            #endregion

            return "{[" +
                "{\"appID\":2,\"appName\":\"奥拓电子\",\"" +
                    "deiveList\":" +
                        "[" +
                            "{\"deviceId\":\"LoRaAirPanel.GIK8330011\",\"deviceName\":\"空调面板\",\"state\":\"offline\",\"typeName\":\"LoRaAirPanel\"}," +
                            "{\"deviceId\":\"LoRaPlug.GEK9320663\",\"deviceName\":\"智能排插\",\"state\":\"online\",\"typeName\":\"LoRaPlug\"}," +
                            "{\"deviceId\":\"LoRaPlug.GEK9320715\",\"deviceName\":\"智能排插2\",\"state\":\"online\",\"typeName\":\"LoRaPlug\"}," +
                            "{\"deviceId\":\"LoRaPlug.GMK8501686\",\"deviceName\":\"智能插座\",\"state\":\"offline\",\"typeName\":\"LoRaPlug\"}," +
                            "{\"deviceId\":\"LoRaPlug.GMK8502203\",\"deviceName\":\"智能插座2\",\"state\":\"offline\",\"typeName\":\"LoRaPlug\"}," +
                            "{\"deviceId\":\"LoRaTempHumid.GJK9291964\",\"deviceName\":\"温湿度传感器\",\"state\":\"online\",\"typeName\":\"LoRaTempHumid\"}," +
                            "{\"deviceId\":\"SmokeSensor.GQK9320712 - 3 - 1\",\"deviceName\":\"烟雾传感器\",\"state\":\"offline\",\"typeName\":\"SmokeSensor\"}," +
                            "{\"deviceId\":\"YBL-SingleFireThreeKeySwitch.ybl_DD3A0300\",\"deviceName\":\"单火三路\",\"state\":\"online\",\"typeName\":\"YBL-SingleFireThreeKeySwitch\"}," +
                            "{\"deviceId\":\"human-detection.GLK8500117\",\"deviceName\":\"红外人体感应\",\"state\":\"offline\",\"typeName\":\"human-detection\"}," +
                            "{\"deviceId\":\"human-detection.GLK8500320\",\"deviceName\":\"红外人体感应2\",\"state\":\"online\",\"typeName\":\"human-detection\"}]}]}";


        }
        public string LoadManager(int app_id)
        {
            accessTokenJo = GetAccessToken(api_token);
            if (accessTokenJo.Value<int>("expiresIn") <= 100)
            {
                //超时
                return "";
            }
            accessToken = accessTokenJo["accessToken"].ToString();

            //2、查询指定应用下，所有设备
            JObject requestJo = new JObject();

            foreach (string typeName in type_names)
            {
                requestJo.Add(
                    new JProperty(typeName,
                        new JObject(
                            new JProperty("all", true),
                            new JProperty("group_ids", new JArray(app_id)),
                            new JProperty("device_ids", new JArray())
                            )));
            }
            allJo = GetState(accessToken, requestJo);//所有类型，所有设备

            //将状态填充
            IEnumerable<JProperty> jTypeProperties = allJo.Properties();

            appList = new List<App>();

            this.requestJo = new JObject();
            foreach (JProperty jp in jTypeProperties)
            {
                if (jp.Value.Count() != 0)
                {
                    IEnumerable<JProperty> jDevProperties = (jp.Value as JObject).Properties(); //获取该类型所有设备

                    JObject deiveListJo = new JObject();

                    switch (jp.Path)
                    {
                        //433转换器
                        case "433Changer":
                            break;
                        //LoRa门锁
                        case "LoRaDoorLockx":
                            break;
                        //MPM4780智能变送器
                        case "MPM4780_Transmitters":
                            break;
                        //单火三路开关控制器
                        case "YBL-SingleFireThreeKeySwitch":
                            break;
                        //智能三路触控开关
                        case "ThreeWayLoraSwitch":
                            break;
                        //智能三鉴探测器
                        case "BODY_SENSOR":
                            break;
                        //智能单路触控开关
                        case "OneWayLoraSwitch":
                            break;
                        //智能双路触控开关
                        case "TwoWayLoraSwitch":
                            break;
                        //智能插座
                        case "LoRaPlug":
                            foreach (JProperty jdp in jDevProperties)
                            {
                                JObject deiveJo = new JObject();
                                deiveJo.Add(new JProperty("设备ID", jdp.Path));
                                deiveJo.Add(new JProperty("设备名称", jdp.Value["name"]));
                                deiveJo.Add(new JProperty("设备类型", jp.Path));
                                deiveJo.Add(new JProperty("设备状态", jdp.Value["states"]));
                                string i = jdp.Value["data"]["cloud_state"]["DEV_SWITCH_STA"].ToObject<string>();
                                if (i != "" && i == "0.00")
                                    deiveJo.Add(new JProperty("开启/关闭", "关闭"));
                                else
                                    deiveJo.Add(new JProperty("开启/关闭", "开启"));

                                i = jdp.Value["data"]["cloud_state"]["DEV_LED_STA"].ToObject<string>();
                                if (i != "" && i == "0.00")
                                    deiveJo.Add(new JProperty("按键灯状态", "关闭"));
                                else
                                    deiveJo.Add(new JProperty("按键灯状态", "开启"));
                                deiveListJo.Add(new JProperty(jdp.Path, deiveJo));
                            }
                            break;
                        //智能液位变送器
                        case "Liquid_Transmitters":
                            break;
                        //温湿度传感器
                        case "LoRaTempHumid":
                            foreach (JProperty jdp in jDevProperties)
                            {
                                JObject deiveJo = new JObject();
                                deiveJo.Add(new JProperty("设备ID", jdp.Path));
                                deiveJo.Add(new JProperty("设备名称", jdp.Value["name"]));
                                deiveJo.Add(new JProperty("设备类型", jp.Path));
                                deiveJo.Add(new JProperty("设备状态", jdp.Value["states"]));

                                deiveJo.Add(new JProperty("温度", jdp.Value["data"]["DEV_LORADBM"]));
                                deiveJo.Add(new JProperty("湿度", jdp.Value["data"]["DEV_MOISTURE"]));
                                deiveJo.Add(new JProperty("光照强度", jdp.Value["data"]["DEV_LUX"]));

                                deiveListJo.Add(new JProperty(jdp.Path, deiveJo));
                            }
                            break;
                        //温湿度传感器-XW-TH-B/BO-485
                        case "temperature-sensor":
                            break;
                        //漏液不定位传感器
                        case "fluid-leakage-controller":
                            break;
                        //漏液定位传感器
                        case "positioning-fluid-leakage-controller":
                            break;
                        //烟雾传感器
                        case "SmokeSensor":
                            break;
                        //空调面板
                        case "LoRaAirPanel":
                            foreach (JProperty jdp in jDevProperties)
                            {
                                JObject deiveJo = new JObject();
                                deiveJo.Add(new JProperty("设备ID", jdp.Path));
                                deiveJo.Add(new JProperty("设备名称", jdp.Value["name"]));
                                deiveJo.Add(new JProperty("设备类型", jp.Path));
                                deiveJo.Add(new JProperty("设备状态", jdp.Value["states"]));
                                string i = jdp.Value["data"]["cloud_state"]["DEV_SWITCH_STA"].ToObject<string>();
                                if (i == "" && i == "0.00")
                                    deiveJo.Add(new JProperty("开启/关闭", "关闭"));
                                else
                                    deiveJo.Add(new JProperty("开启/关闭", "开启"));

                                deiveJo.Add(new JProperty("当前温度", jdp.Value["data"]["DEV_CURRENT_TEMP"]));

                                //deiveJo.Add(new JProperty("工作模式", jdp.Value["data"]["cloud_state"]["DEV_MODE"]));
                                //deiveJo.Add(new JProperty("开关按键键状态", jdp.Value["data"]["DEV_EVENT_SWITCH"]));
                                //deiveJo.Add(new JProperty("减小按键", jdp.Value["data"]["DEV_EVENT_DOWN"]));
                                //deiveJo.Add(new JProperty("调节模式按钮", jdp.Value["data"]["DEV_EVENT_DOWN"]));
                                //deiveJo.Add(new JProperty("锁定状态", jdp.Value["data"]["cloud_state"]["DEV_LOCKED"]));
                                //deiveJo.Add(new JProperty("目标温度", jdp.Value["data"]["cloud_state"]["DEV_TARGET_TEMP"]));
                                //deiveJo.Add(new JProperty("风速", jdp.Value["data"]["cloud_state"]["DEV_SPEED"]));
                                //deiveJo.Add(new JProperty("增加按键", jdp.Value["data"]["DEV_CURRENT_TEMP"]));
                                //deiveJo.Add(new JProperty("调节风速按钮", jdp.Value["data"]["DEV_CURRENT_TEMP"]));
                                //deiveJo.Add(new JProperty("重启状态", jdp.Value["data"]["DEV_CURRENT_TEMP"]));
                                //deiveJo.Add(new JProperty("通信网关", jdp.Value["data"]["DEV_CURRENT_TEMP"]));
                                deiveListJo.Add(new JProperty(jdp.Path, deiveJo));
                            }
                            break;
                        //红外人体检测传感器
                        case "human-detection":
                            foreach (JProperty jdp in jDevProperties)
                            {
                                JObject deiveJo = new JObject();
                                deiveJo.Add(new JProperty("设备ID", jdp.Path));
                                deiveJo.Add(new JProperty("设备名称", jdp.Value["name"]));
                                deiveJo.Add(new JProperty("设备类型", jp.Path));
                                deiveJo.Add(new JProperty("设备状态", jdp.Value["states"]));
                                string i = jdp.Value["data"]["HUMAN_STATUS"].ToObject<string>();
                                if (i != "" && i == "0.00")
                                    deiveJo.Add(new JProperty("人体感应", "无人"));
                                else
                                    deiveJo.Add(new JProperty("人体感应", "有人"));

                                deiveListJo.Add(new JProperty(jdp.Path, deiveJo));
                            }
                            break;
                        default:
                            break;
                    }

                    this.requestJo.Add(new JProperty(jp.Path, deiveListJo));

                }

            }

            #region 假数据

            #endregion

            return "{{\"LoRaAirPanel\": {\"LoRaAirPanel.GIK8330011\": {\"设备ID\": \"LoRaAirPanel.GIK8330011\",\"设备名称\": \"空调面板\",\"设备类型\": \"LoRaAirPanel\",\"设备状态\": \"offline\",\"开启/关闭\": \"开启\",\"当前温度\": \"\"}},\"LoRaPlug\": {\"LoRaPlug.GEK9320663\": {\"设备ID\": \"LoRaPlug.GEK9320663\",\"设备名称\": \"智能排插\",\"设备类型\": \"LoRaPlug\",\"设备状态\": \"online\",\"开启/关闭\": \"开启\",\"按键灯状态\": \"关闭\"},\"LoRaPlug.GEK9320715\": {\"设备ID\": \"LoRaPlug.GEK9320715\",\"设备名称\": \"智能排插2\",\"设备类型\": \"LoRaPlug\",\"设备状态\": \"online\",\"开启/关闭\": \"开启\",\"按键灯状态\": \"开启\"},\"LoRaPlug.GMK8501686\": {\"设备ID\": \"LoRaPlug.GMK8501686\",\"设备名称\": \"智能插座\",\"设备类型\": \"LoRaPlug\",\"设备状态\": \"offline\",\"开启/关闭\": \"开启\",\"按键灯状态\": \"开启\"},\"LoRaPlug.GMK8502203\": {\"设备ID\": \"LoRaPlug.GMK8502203\",\"设备名称\": \"智能插座2\",\"设备类型\": \"LoRaPlug\",\"设备状态\": \"offline\",\"开启/关闭\": \"开启\",\"按键灯状态\": \"开启\"}},\"LoRaTempHumid\": {\"LoRaTempHumid.GJK9291964\": {\"设备ID\": \"LoRaTempHumid.GJK9291964\",\"设备名称\": \"温湿度传感器\",\"设备类型\": \"LoRaTempHumid\",\"设备状态\": \"online\",\"温度\": \"-49.00\",\"湿度\": \"46.60\",\"光照强度\": \"186.60\"}},\"SmokeSensor\": { },\"YBL-SingleFireThreeKeySwitch\": { },\"human-detection\": {\"human-detection.GLK8500117\": {\"设备ID\": \"human-detection.GLK8500117\",\"设备名称\": \"红外人体感应\",\"设备类型\": \"human-detection\",\"设备状态\": \"offline\",\"人体感应\": \"有人\"},\"human-detection.GLK8500320\": {\"设备ID\": \"human-detection.GLK8500320\",\"设备名称\": \"红外人体感应2\",\"设备类型\": \"human-detection\",\"设备状态\": \"online\",\"人体感应\": \"无人\"}}}}";
        }

        #endregion

        #endregion

        private void FrmShell_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }

    #region 多功能业务柜
    public class Product
    {
        public string BusinessType { get; set; }
        public string BusinessNameNo { get; set; }
        public string BusinessName { get; set; }
        public string RealBusiness { get; set; }
    }
    public class PageData
    {
        public string[] BussisTypes { get; set; } = { "养老保险", "人寿保险", "车辆保险" };
        public List<Product> businesslist { get; set; }

    }
    #endregion

    #region 物联网
    public class App
    {
        public int appID { get; set; }
        public string appName { get; set; }
        public List<Deive> deiveList { get; set; }
    }
    public class Deive
    {
        public string deviceId { get; set; }
        public string deviceName { get; set; }
        public string state { get; set; }
        public string typeName { get; set; }
    }
    #endregion
}
