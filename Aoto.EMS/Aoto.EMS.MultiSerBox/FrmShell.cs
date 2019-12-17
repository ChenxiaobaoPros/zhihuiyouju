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
            //SetDeiveState("YBL-SingleFireThreeKeySwitch", "1", "1", "1", new string[] { "ybl_DD3A0300" });
            //SetDeiveState("YBL-SingleFireThreeKeySwitch", "0", "0", "0", new string[] { "ybl_DD3A0300" });
            //SetDeiveState("LoRaAirPanel", "1", new string[] { "GIK8330011" });

            //peripheralManager = AutofacContainer.ResolveNamed<PeripheralManager>("peripheralManager"); //也可以用反射
            peripheralManager = new PeripheralManager();
            //webBrowser.Navigate(AppState.WelcomeUrl);

            //webBrowser.Navigate(Path.Combine(Config.AppRoot, "web\\mySelf\\qms\\html\\admin\\index.html"));
            //webBrowser.Navigate(Path.Combine(Config.AppRoot, "web\\duogong\\html\\index.html"));
            webBrowser.Navigate(Path.Combine(Config.AppRoot, "web\\wulianwang\\html\\inde.html"));
            //webBrowser.Navigate(Path.Combine(Config.AppRoot, "web\\youpingui\\html\\index.html"));
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
            JObject jo = new JObject();
            jo["bussisType"] = bussisType;
            peripheralManager.IDCardReader.ReadAsync(jo);
        }
        #endregion

        #region 二维码
        public void loadQRCode()
        {
            //创建就开启了
            peripheralManager.QRCode.OpenQRCode();
        }
        public void closeQRCode()
        {
            peripheralManager.QRCode.CloseQRCode();
        }


        #endregion

        #region 指纹
        public void loadFinger()
        {
            peripheralManager.Finger.ReadAsync(new JObject());
        }
        #endregion

        #region 键盘
        //明文
        public void TextModel()
        {
            peripheralManager.KeyBoard.ReadAsync(new JObject());
            peripheralManager.KeyBoard.TurnOnTextMode();
        }
        //密文
        public void CipherTextMode()
        {
            peripheralManager.KeyBoard.TurnOnCipherTextMode();
        }
        //解密
        public void Decrypt()
        {
            peripheralManager.KeyBoard.Decrypt();
        }
        #endregion

        #region 签字板
        public void LoadBorad()
        {
            DialogResult dialogResult = FrmSignaturePlate.Instance.ShowDialog();
            if (DialogResult.OK != dialogResult)
            {
                return;
            }

        }
        #endregion

        #region 吸卡器
        HybridCardReader hybridCardReader;
        public void loadMoveCard()
        {
            if (hybridCardReader == null)
                hybridCardReader = new HybridCardReader();

        }
        public void HalfBacnk()
        {
            hybridCardReader.HalfwayBack();
        }
        public void BackCard()
        {
            hybridCardReader.BackCard();
        }
        public void RFCard()
        {
            hybridCardReader.RFCard();
        }
        public void ICCard()
        {
            hybridCardReader.ICCard();
        }
        public void GetCardReaderInfo()
        {
            hybridCardReader.GetCardReaderInfo();
        }
        public void MagRead()
        {
            hybridCardReader.MagRead();
        }
        public void IsMCard()
        {
            hybridCardReader.IsMCard();
        }
        public void MRead()
        {
            hybridCardReader.MRead();
        }
        public void CPURead()
        {
            hybridCardReader.CPURead();
        }
        #endregion

        #region 高拍仪 
        public void LoadHighMeter()
        {
            DialogResult dialogResult = FrmHighMeter.Instance.ShowDialog();
            if (DialogResult.OK != dialogResult)
            {
                return;
            }

        }
        #endregion

        #region 打印
        public void PrintWorke()
        {
            peripheralManager.ThermalPrinter.PrintAsync(new JObject());
        }
        #endregion

        #region 人脸识别
        public void LoadFaceCam()
        {
            DialogResult dialogResult = FrmFaceCamera.Instance.ShowDialog();
            if (DialogResult.OK != dialogResult)
            {
                return;
            }

        }
        #endregion

        #endregion

        #region 邮品柜

        #region 页面数据

        private Root ResponseRoot { get; set; }
        public string GetProductList()
        {
            ResponseRoot = new Root();
            ResponseRoot.head = new Head();
            ResponseRoot.head.retCode = 0;
            ResponseRoot.bodyList = new List<Body>();
            ResponseRoot.bodyList.Add(new Body() { ProductNmae = "万年邮册", Url =Path.Combine(Application.StartupPath, "web\\youpingui\\img\\p1.png"), Price = 20.00, Stock = 12 });
            ResponseRoot.bodyList.Add(new Body() { ProductNmae = "包装纸箱", Url = Path.Combine(Application.StartupPath, "web\\youpingui\\img\\p2.png"), Price = 20.00, Stock = 30 });
            ResponseRoot.bodyList.Add(new Body() { ProductNmae = "恐龙小型帐", Url = Path.Combine(Application.StartupPath, "web\\youpingui\\img\\p3.png"), Price = 5.00, Stock = 5 });


            return JsonConvert.SerializeObject(ResponseRoot);
        }

        public string GetProductInfoByName(string name)
        {
            /*
                string path = "C:\\dir1\\dir2\\foo.txt";
                string str = "GetFullPath：" + Path.GetFullPath(path) + "\r\n";
                str += "GetDirectoryName：" + Path.GetDirectoryName(path) + "\r\n";
                str += "GetFileName：" + Path.GetFileName(path) + "\r\n";
                str += "GetFileNameWithoutExtension：" + Path.GetFileNameWithoutExtension(path) + "\r\n";
                str += "GetExtension：" + Path.GetExtension(path) + "\r\n";
                str += "GetPathRoot：" + Path.GetPathRoot(path) + "\r\n";
                MessageBox.Show(str);

                // 本文来自技术世界www.js4j.com 技术论坛//

                结果： // 本文来自技术世界www.js4j.com 专业技术门户网站//

                GetFullPath：C:\dir1\dir2\foo.txt
                GetDirectoryName：C:\dir1\dir2
                GetFileName：foo.txt
                GetFileNameWithoutExtension：foo
                GetExtension：.txt
                GetPathRoot：C:\

                // 内容来自技术世界www.js4j.com  技术爱好者//
                这里要说明 path 是如何C# 轻松获取路径中文件名、目录、扩展名等判断目录和文件名的：它把最后一个 \ 后面的内容当作是文件名。 // 内容来自js4j.com//

                C:\dir1\dir2\foo.txt 文件名是 foo.txt，目录名是 C:\dir1\dir2。
                C:\dir1\dir2\ 文件名是零长度字符串，目录名是 C:\dir1\dir2。
                C:\dir1\dir2 文件名是 dir2，目录名是 C:\dir1。
                C# 轻松获取路径中文件名、目录、扩展名等
             */
            Body body = ResponseRoot.bodyList.Where(p => p.ProductNmae == name).FirstOrDefault();
            body.Url = Path.Combine(Path.GetDirectoryName(body.Url), Path.GetFileNameWithoutExtension(body.Url).Replace('p','g') + Path.GetExtension(body.Url));


            JObject jo = new JObject(
             new JProperty("head", new JObject(
                 new JProperty("retCode", 0))),
             new JProperty("body", new JObject(
                 new JProperty("ProductNmae", body.ProductNmae),
                 new JProperty("Url", body.Url),
                 new JProperty("Price", body.Price),
                 new JProperty("Stock", body.Stock))));

            return jo.ToString(Formatting.None);
        }

        public void Pay()
        {
            //开启二维码 （需要将二维码改成手动开启）
            peripheralManager.QRCode.OpenQRCode();
            //扫码  回调  返回支付成功

            //开门  回调 开门成功
            int index = 0;
            StartMotor(index);
        }

        #endregion

        #region 邮品柜

        public void LoadYPBox()
        {
            peripheralManager.YPBox.Initialize();
            peripheralManager.YPBox.Inspecting();//循环开门

        }
        public void StartMotor(int index)
        {
            peripheralManager.YPBox.StartingMotor(index);
        }

        #endregion

        #region RFID
        public void LoadRFID()
        {
            peripheralManager.RFID.Initialize();
            peripheralManager.RFID.Inspection();
        }
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
        public string[] type_names = { "433Changer"/*433转换器*/, "LoRaDoorLock"/*LoRa门锁*/, "MPM4780_Transmitters"/*MPM4780智能变送器*/, "YBL-SingleFireThreeKeySwitch"/*单火三路开关控制器*/, "ThreeWayLoraSwitch"/*智能三路触控开关*/, "BODY_SENSOR"/*智能三鉴探测器*/, "OneWayLoraSwitch"/*智能单路触控开关*/, "TwoWayLoraSwitch"/*智能双路触控开关*/, "LoRaPlug"/*智能插座*/, "Liquid_Transmitters"/*智能液位变送器*/, "LoRaTempHumid"/*温湿度传感器*/, "temperature-sensor"/*温湿度传感器-XW-TH-B/BO-485*/, "fluid-leakage-controller"/*漏液不定位传感器*/, "positioning-fluid-leakage-controller"/*漏液定位传感器*/, "SmokeSensor"/*烟雾传感器*/, "LoRaAirPanel"/*空调面板*/, "human-detection"/*红外人体检测传感器*/, "AIR_WEIGUOKEJI"/*空气质量传感器*/ };
        //1、token
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        public JObject GetAccessToken(string api_token)
        {
            url = "api/v1/accessToken";
            //jsonRequest = "{\"api_token\": \"39e58bd22275ccca486a3c3a9e00e578\"}";
            JObject requestJo = new JObject(new JProperty("api_token", api_token));
            jsonRequest = requestJo.ToString(Formatting.None);
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
        //SetDeiveState("LoRaPlug",new string[]{ "GEK9320663" },"1");
        /// <summary>
        /// 同一种类型
        /// </summary>
        /// <param name="typeNmae"></param>
        /// <param name="code"></param>
        /// <param name="idArray"></param>
        /// <returns></returns>
        public string SetDeiveState(string typeNmae, string code, string[] idArray)
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

            try
            {
                string jsonResponse = HttpClient.IOTPost(url, accessToken, requestJo.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
            JObject jo = new JObject(new JProperty("retCode", false));

            if (JObject.Parse(jsonResponse).Value<bool>("success"))
                jo["retCode"] = true;

            return jsonResponse;
        }
        /// <summary>
        /// 多个类型
        /// </summary>
        /// <param name="typeNmae"></param>
        /// <param name="code"></param>
        /// <param name="idArray"></param>
        /// <returns></returns>
        public string SetDeiveState(string[] typeNmae, string code, string[] idArray)
        {
            string url = "api/v1/device/setStatus";
            accessTokenJo = GetAccessToken(api_token);
            if (accessTokenJo.Value<int>("expiresIn") <= 100)
            {
                //超时
            }
            string accessToken = accessTokenJo["accessToken"].ToString();
            requestJo = new JObject();

            foreach (string type in typeNmae)
            {
                requestJo.Add(new JProperty(type, new JObject(
                new JProperty("device_ids", new JArray(idArray)),
                new JProperty("data", new JObject(new JProperty("DEV_SWITCH_STA", code)))
                )));
            }

            try
            {
                string jsonResponse = HttpClient.IOTPost(url, accessToken, requestJo.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            JObject jo = new JObject(new JProperty("retCode", false));

            if (JObject.Parse(jsonResponse).Value<bool>("success"))
                jo["retCode"] = true;

            return jsonResponse;
        }
        /// <summary>
        /// 多种类型多个开关
        /// </summary>
        /// <param name="typeNmae"></param>
        /// <param name="code1"></param>
        /// <param name="code2"></param>
        /// <param name="code3"></param>
        /// <param name="idArray"></param>
        /// <returns></returns>
        public string SetDeiveState(string typeNmae, string code1,string code2, string code3, string[] idArray)
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
                new JProperty("data", new JObject(new JProperty("DEV_SWITCH_STA_ONE", code1),
                                                  new JProperty("DEV_SWITCH_STA_TWO", code2),
                                                  new JProperty("DEV_SWITCH_STA_THREE", code3)))
                )));

            try
            {
                string jsonResponse = HttpClient.IOTPost(url, accessToken, requestJo.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            JObject jo = new JObject(new JProperty("retCode", false));

            if (JObject.Parse(jsonResponse).Value<bool>("success"))
                jo["retCode"] = true;

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
            #region 真实数据

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

            appList = new List<App>();
            foreach (JProperty jp in jTypeProperties)
            {
                if (jp.Value.Count() != 0)
                {
                    IEnumerable<JProperty> jDevProperties = (jp.Value as JObject).Properties(); //获取该类型所有设备

                    foreach (JProperty jdp in jDevProperties)
                    {
                        IEnumerable<App> querylist = appList.Where(j => j.appID == jdp.Value.Value<int>("app_id"));

                        Deive deive = new Deive()
                        {
                            deviceId = jdp.Path.Split('.')[1],
                            deviceName = jdp.Value.Value<string>("name"),
                            typeName = jp.Path,
                        };
                        string state = jdp.Value.Value<string>("states");
                        App app = null;
                        if (querylist != null && querylist.Count() > 0)
                        {
                            app = querylist.FirstOrDefault();
                            app.all++;

                            if (state == "offline")
                            {
                                deive.state = "离线";
                            }
                            else if (state == "online")
                            {
                                deive.state = "在线";
                                app.online++;
                            }
                            app.deiveList.Add(deive);
                        }
                        else
                        {
                            app = new App()
                            {
                                appID = jdp.Value.Value<int>("app_id"),
                                appName = jdp.Value.Value<string>("app_name"),
                                date = DateTime.Now.ToString("yyyy:mm:dd:hh:ss"),
                                all = 1,

                                deiveList = new List<Deive>()
                            };
                            if (state == "offline")
                            {
                                deive.state = "离线";
                                app.online = 0;
                            }
                            else if (state == "online")
                            {
                                deive.state = "在线";
                                app.online = 1;
                            }
                            app.deiveList.Add(deive);
                            appList.Add(app);
                        }
                    }
                }

            }
            string jsonStr = JsonConvert.SerializeObject(appList);

            return jsonStr;
            #endregion

            #region 测试数据
            //return "[{\"appID\":2,\"appName\":\"奥拓电子\",\"date\":\"2019:28:11:04:06\",\"online\":10,\"all\":30,\"deiveList\":[{\"deviceId\":\"GIK8330011\",\"deviceName\":\"空调面板\",\"state\":\"离线\",\"typeName\":\"LoRaAirPanel\"},{\"deviceId\":\"GEK9320663\",\"deviceName\":\"智能排插\",\"state\":\"在线\",\"typeName\":\"LoRaPlug\"},{\"deviceId\":\"GEK9320715\",\"deviceName\":\"智能排插2\",\"state\":\"在线\",\"typeName\":\"LoRaPlug\"},{\"deviceId\":\"GMK8501686\",\"deviceName\":\"智能插座\",\"state\":\"离线\",\"typeName\":\"LoRaPlug\"},{\"deviceId\":\"GMK8502203\",\"deviceName\":\"智能插座2\",\"state\":\"离线\",\"typeName\":\"LoRaPlug\"},{\"deviceId\":\"GJK9291964\",\"deviceName\":\"温湿度传感器\",\"state\":\"离线\",\"typeName\":\"LoRaTempHumid\"},{\"deviceId\":\"GQK9320712-3-1\",\"deviceName\":\"烟雾传感器\",\"state\":\"离线\",\"typeName\":\"SmokeSensor\"},{\"deviceId\":\"ybl_DD3A0300\",\"deviceName\":\"单火三路\",\"state\":\"在线\",\"typeName\":\"YBL - SingleFireThreeKeySwitch\"},{\"deviceId\":\"GLK8500117\",\"deviceName\":\"红外人体感应\",\"state\":\"离线\",\"typeName\":\"human-detection\"},{\"deviceId\":\"GLK8500320\",\"deviceName\":\"红外人体感应2\",\"state\":\"离线\",\"typeName\":\"human-detection\"}]},{\"appID\":3,\"appName\":\"奥拓电子11\",\"date\":\"2019:28:11:04:06\",\"online\":10,\"all\":30,\"deiveList\":[{\"deviceId\":\"GIK8330011\",\"deviceName\":\"空调面板1\",\"state\":\"离线\",\"typeName\":\"LoRaAirPanel\"},{\"deviceId\":\"GEK9320663\",\"deviceName\":\"智能排插1\",\"state\":\"在线\",\"typeName\":\"LoRaPlug\"},{\"deviceId\":\"GEK9320715\",\"deviceName\":\"智能排插2\",\"state\":\"在线\",\"typeName\":\"LoRaPlug\"},{\"deviceId\":\"GMK8501686\",\"deviceName\":\"智能插座23\",\"state\":\"离线\",\"typeName\":\"LoRaPlug\"},{\"deviceId\":\"GMK8502203\",\"deviceName\":\"智能插座2\",\"state\":\"离线\",\"typeName\":\"LoRaPlug\"},{\"deviceId\":\"GJK9291964\",\"deviceName\":\"温湿度传感器\",\"state\":\"离线\",\"typeName\":\"LoRaTempHumid\"},{\"deviceId\":\"GQK9320712-3-1\",\"deviceName\":\"烟雾传感器\",\"state\":\"离线\",\"typeName\":\"SmokeSensor\"},{\"deviceId\":\"ybl_DD3A0300\",\"deviceName\":\"单火三路\",\"state\":\"在线\",\"typeName\":\"YBL - SingleFireThreeKeySwitch\"},{\"deviceId\":\"GLK8500117\",\"deviceName\":\"红外人体感应\",\"state\":\"离线\",\"typeName\":\"human-detection\"},{\"deviceId\":\"GLK8500320\",\"deviceName\":\"红外人体感应2\",\"state\":\"离线\",\"typeName\":\"human-detection\"}]}]";
            #endregion

        }
        public string LoadManager(int app_id)
        {
            #region 真实数据
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
            allJo = GetState(accessToken, requestJo);//所有类型，所有设备(指定空间下)

            //将状态填充
            IEnumerable<JProperty> jTypeProperties = allJo.Properties();

            this.responseJo = new JObject();
            foreach (JProperty jp in jTypeProperties)
            {
                if (jp.Value.Count() != 0)
                {
                    IEnumerable<JProperty> jDevProperties = (jp.Value as JObject).Properties(); //获取该类型所有设备

                    JObject deiveListByTypeJo = new JObject();
                    JArray deiveArrayJo = new JArray();
                    switch (jp.Path)
                    {
                        //空气质量传感器
                        case "AIR_WEIGUOKEJI":
                            foreach (JProperty jdp in jDevProperties)
                            {
                                JObject deiveInfo = CreateDeiveInfo(jp,jdp);

                                //首先判断是否在线
                                CheckOnline(jdp, deiveInfo);

                                JObject dataJo = new JObject(
                                    new JProperty("deive_name", jdp.Value["name"]),
                                    new JProperty("CO2", jdp.Value["data"]["CO2"]),
                                    new JProperty("PM2_5", jdp.Value["data"]["PM2_5"]));// 需要转换

                                deiveInfo.Add(new JProperty("data", dataJo));
                                deiveArrayJo.Add(deiveInfo);
                            }
                            break;
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
                            foreach (JProperty jdp in jDevProperties)
                            {
                                JObject deiveInfo = CreateDeiveInfo(jp,jdp);
                                //首先判断是否在线
                                CheckOnline(jdp, deiveInfo);

                                JObject dataJo = new JObject(
                                    new JProperty("deive_name", jdp.Value["name"]),
                                    //new JProperty("deive_name", jdp.Value["data"]["CONTROL_PKT_NUM"]),//控制次数
                                    //new JProperty("deive_name", jdp.Value["data"]["FAILED_PKT_NUM"]),//失败次数
                                    //new JProperty("deive_name", jdp.Value["data"]["RETRANS_PKT_NUM"]),//重传次数
                                    new JProperty("deive_one", jdp.Value["data"]["cloud_state"]["DEV_SWITCH_STA_ONE"]),
                                    new JProperty("deive_two", jdp.Value["data"]["cloud_state"]["DEV_SWITCH_STA_TWO"]),
                                    new JProperty("deive_three", jdp.Value["data"]["cloud_state"]["DEV_SWITCH_STA_THREE"]));

                                deiveInfo.Add(new JProperty("data", dataJo));
                                deiveArrayJo.Add(deiveInfo);
                            }
                            break;
                        //智能三路触控开关
                        case "ThreeWayLoraSwitch":
                            foreach (JProperty jdp in jDevProperties)
                            {
                                JObject deiveInfo = CreateDeiveInfo(jp,jdp);
                                //首先判断是否在线
                                CheckOnline(jdp, deiveInfo);

                                JObject dataJo = new JObject(
                                    new JProperty("deive_name", jdp.Value["name"]),
                                    //new JProperty("deive_name", jdp.Value["data"]["CONTROL_PKT_NUM"]),//控制次数
                                    //new JProperty("deive_name", jdp.Value["data"]["FAILED_PKT_NUM"]),//失败次数
                                    //new JProperty("deive_name", jdp.Value["data"]["RETRANS_PKT_NUM"]),//重传次数
                                    new JProperty("deive_one", jdp.Value["data"]["cloud_state"]["DEV_SWITCH_STA_ONE"]),
                                    new JProperty("deive_two", jdp.Value["data"]["cloud_state"]["DEV_SWITCH_STA_TWO"]),
                                    new JProperty("deive_three", jdp.Value["data"]["cloud_state"]["DEV_SWITCH_STA_THREE"]));

                                deiveInfo.Add(new JProperty("data", dataJo));
                                deiveArrayJo.Add(deiveInfo);
                            }
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
                                JObject deiveInfo = CreateDeiveInfo(jp,jdp);

                                //首先判断是否在线
                                CheckOnline(jdp, deiveInfo);

                                JObject dataJo = new JObject(
                                new JProperty("deive_name", jdp.Value["name"]),
                                new JProperty("deive_i", jdp.Value["data"]["DEV_CURRENT"]),
                                new JProperty("deive_v", jdp.Value["data"]["DEV_VOLRAGE"]));

                                deiveInfo.Add(new JProperty("data", dataJo));
                                deiveArrayJo.Add(deiveInfo);
                            }
                            break;
                        //智能液位变送器
                        case "Liquid_Transmitters":
                            break;
                        //温湿度传感器
                        case "LoRaTempHumid":
                            foreach (JProperty jdp in jDevProperties)
                            {
                                JObject deiveInfo = CreateDeiveInfo(jp,jdp);

                                //首先判断是否在线
                                CheckOnline(jdp, deiveInfo);

                                JObject dataJo = new JObject(
                                    new JProperty("deive_name", jdp.Value["name"]),
                                    new JProperty("deive_temperature", jdp.Value["data"]["DEV_TEMPRATURE"]),//温度
                                    new JProperty("deive_humidity", jdp.Value["data"]["DEV_MOISTURE"])//湿度
                                      );

                                deiveInfo.Add(new JProperty("data", dataJo));
                                deiveArrayJo.Add(deiveInfo);
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
                            foreach (JProperty jdp in jDevProperties)
                            {
                                JObject deiveInfo = CreateDeiveInfo(jp,jdp);

                                //首先判断是否在线
                                CheckOnline(jdp, deiveInfo);

                                JObject dataJo = new JObject(
                                    new JProperty("deive_name", jdp.Value["name"]),
                                    new JProperty("deive_smoke", jdp.Value["data"]["IS_SMOKE"]));// 需要转换

                                deiveInfo.Add(new JProperty("data", dataJo));
                                deiveArrayJo.Add(deiveInfo);
                            }
                            break;
                        //空调面板
                        case "LoRaAirPanel":
                            foreach (JProperty jdp in jDevProperties)
                            {
                                JObject deiveInfo = CreateDeiveInfo(jp,jdp);

                                //首先判断是否在线
                                CheckOnline(jdp, deiveInfo);

                                JObject dataJo = new JObject(
                                    new JProperty("deive_name", jdp.Value["name"]),
                                    new JProperty("deive_temperature", jdp.Value["data"]["DEV_CURRENT_TEMP"]));//当前温度

                                deiveInfo.Add(new JProperty("data", dataJo));
                                deiveArrayJo.Add(deiveInfo);
                                //deiveJo.Add(new JProperty("工作模式", jdp.Value["data"]["cloud_state"]["DEV_MODE"]));//1是制暖，3是循环风，0是制冷
                                //deiveJo.Add(new JProperty("开关按键键状态", jdp.Value["data"]["DEV_EVENT_SWITCH"]));//1是开0是关
                                //deiveJo.Add(new JProperty("锁定状态", jdp.Value["data"]["cloud_state"]["DEV_LOCKED"]));
                                //deiveJo.Add(new JProperty("目标温度", jdp.Value["data"]["cloud_state"]["DEV_TARGET_TEMP"]));
                                //deiveJo.Add(new JProperty("风速", jdp.Value["data"]["cloud_state"]["DEV_SPEED"]));
                                //deiveJo.Add(new JProperty("重启状态", jdp.Value["data"]["DEV_CURRENT_TEMP"]));
                                //deiveJo.Add(new JProperty("通信网关", jdp.Value["data"]["DEV_GATEWAY"]));
                            }
                            break;
                        //红外人体检测传感器
                        case "human-detection":
                            foreach (JProperty jdp in jDevProperties)
                            {
                                JObject deiveInfo = CreateDeiveInfo(jp,jdp);

                                CheckOnline(jdp, deiveInfo);

                                JObject dataJo = new JObject(
                                      new JProperty("deive_name", jdp.Value["name"]),
                                            new JProperty("deive_someone", jdp.Value["data"]["HUMAN_STATUS"]));//需要转换

                                deiveInfo.Add(new JProperty("data", dataJo));
                                deiveArrayJo.Add(deiveInfo);
                            }
                            break;
                        default:
                            break;
                    }

                    this.responseJo.Add(new JProperty(jp.Path, deiveArrayJo));

                }

            }
            jsonResponse = responseJo.ToString(Formatting.None);
            return jsonResponse;

            #endregion

            #region 测试数据
            //return "{\"LoRaAirPanel\": [{\"deive_id\": \"GIK8330011\",\"group_id\": 2,\"group_name\": \"奥拓电子\",\"lastDate\": \"\",\"Online_State\": false,\"data\": {\"deive_name\": \"空调面板\",\"deive_temperature\": \"\"}}],\"LoRaPlug\": [{\"deive_id\": \"GEK9320663\",\"group_id\": 2,\"group_name\": \"奥拓电子\",\"lastDate\": \"2019-12-12 17:31:59\",\"Online_State\": true,\"Always_Open\": false,\"Open_State\": true,\"data\": {\"deive_name\": \"智能排插\",\"deive_i\": \"0.00\",\"deive_v\": \"234.00\"}},{\"deive_id\": \"GEK9320715\",\"group_id\": 2,\"group_name\": \"奥拓电子\",\"lastDate\": \"2019-12-12 17:31:16\",\"Online_State\": true,\"Always_Open\": false,\"Open_State\": true,\"data\": {\"deive_name\": \"智能排插2\",\"deive_i\": \"0.27\",\"deive_v\": \"234.00\"}},{\"deive_id\": \"GMK8501686\",\"group_id\": 2,\"group_name\": \"奥拓电子\",\"lastDate\": \"\",\"Online_State\": false,\"data\": {\"deive_name\": \"智能插座\",\"deive_i\": \"\",\"deive_v\": \"\"}},{\"deive_id\": \"GMK8502203\",\"group_id\": 2,\"group_name\": \"奥拓电子\",\"lastDate\": \"2019-12-12 17:31:21\",\"Online_State\": true,\"Always_Open\": false,\"Open_State\": true,\"data\": {\"deive_name\": \"智能插座2\",\"deive_i\": \"0.00\",\"deive_v\": \"235.00\"}}],\"LoRaTempHumid\": [{\"deive_id\": \"GJK9291964\",\"group_id\": 2,\"group_name\": \"奥拓电子\",\"lastDate\": \"2019-12-12 17:30:34\",\"Online_State\": true,\"Always_Open\": true,\"data\": {\"deive_name\": \"温湿度传感器\",\"deive_temperature\": \"22.30\",\"deive_humidity\": \"37.20\"}}],\"SmokeSensor\": [{\"deive_id\": \"GQK9320712-3-1\",\"group_id\": 2,\"group_name\": \"奥拓电子\",\"lastDate\": \"\",\"Online_State\": false,\"data\": {\"deive_name\": \"烟雾传感器\",\"deive_smoke\": \"\"}}],\"YBL-SingleFireThreeKeySwitch\": [{\"deive_id\": \"ybl_DD3A0300\",\"group_id\": 2,\"group_name\": \"奥拓电子\",\"lastDate\": \"\",\"Online_State\": true,\"Always_Open\": true,\"data\": {\"deive_name\": \"单火三路\",\"deive_one\": \"\",\"deive_two\": \"\",\"deive_three\": \"\"}}],\"human-detection\": [{\"deive_id\": \"GLK8500117\",\"group_id\": 2,\"group_name\": \"奥拓电子\",\"lastDate\": \"\",\"Online_State\": false,\"data\": {\"deive_name\": \"红外人体感应\",\"deive_someone\": \"\" }},{\"deive_id\": \"GLK8500320\",\"group_id\": 2,\"group_name\": \"奥拓电子\",\"lastDate\": \"\",\"Online_State\": false,\"data\": {\"deive_name\": \"红外人体感应2\",\"deive_someone\": \"\"}}]}";
            #endregion
        }
        /// <summary>
        ///  1 是开启  0 是关闭 ； 在线 1  离线 0 ； 常开 1  不是常开0
        /// </summary>
        /// <param name="jdp"></param>
        /// <param name="deiveInfo"></param>
        public void CheckOnline(JProperty jdp, JObject deiveInfo)
        {
            //首先判断是否在线
            if (jdp.Value["states"].ToString() == "online")
            {
                deiveInfo.Add(new JProperty("Online_State", true));

                //判断是否是常开
                if (jdp.Value["data"]["cloud_state"]["DEV_SWITCH_STA"] == null)
                {
                    deiveInfo.Add(new JProperty("Always_Open", true));//常开则不用开启状态
                }
                else
                {
                    deiveInfo.Add(new JProperty("Always_Open", false));
                    string startStr = jdp.Value["data"]["cloud_state"]["DEV_SWITCH_STA"].ToString();
                    if (startStr == "" || startStr == "0.00")
                    {
                        deiveInfo.Add(new JProperty("Open_State", false));
                    }
                    else if (startStr == "1.00")
                    {
                        deiveInfo.Add(new JProperty("Open_State", true));
                    }

                }
            }
            else if (jdp.Value["states"].ToString() == "offline")
            {
                deiveInfo.Add(new JProperty("Online_State", false));
            }
        }
        public JObject CreateDeiveInfo(JProperty jp, JProperty jdp)
        {
            return new JObject(
                            new JProperty("deive_id", jdp.Path.Split('.')[1]),
                            new JProperty("deive_type", jp.Path),
                            new JProperty("group_id", jdp.Value["group_id"]),
                            new JProperty("group_name", jdp.Value["group_name"]),
                            new JProperty("lastDate", jdp.Value["lastTime"]));
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
        public string date { get; set; }
        public int online { get; set; }
        public int all { get; set; }
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

    #region 邮品柜
    public class Root
    {
        public Head head { get; set; }
        public List<Body> bodyList { get; set; }
    }
    public class Head  //一般放操作状态等提示
    {
        public int retCode { get; set; }
    }
    public class Body //一般放数据主体
    {
        public string ProductNmae { get; set; }
        public string Url { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
    }
    #endregion

}
