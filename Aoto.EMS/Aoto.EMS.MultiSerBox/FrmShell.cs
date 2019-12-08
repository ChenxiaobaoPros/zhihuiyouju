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
            LoadBussisPage();
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
            //webBrowser.Navigate(AppState.WelcomeUrl);
            webBrowser.Navigate(Path.Combine(Config.AppRoot, "wulianwang\\html\\inde.html"));
            peripheralManager = new PeripheralManager();//也可以用反射

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

        #endregion

        #region 物联网

        #region 开启设备
        //公共变量
        public string accessTaken = "";
        public string url = "";
        public string jsonRequest = "";
        public string jsonResponse = "";

        public JObject jo;
        public JObject accessTokenJo;
        public JObject appListJo;
        public JObject deiveListJo;
        public JObject GetAccessToken()
        {
            url = "api/v1/accessToken";
            jsonRequest = "{\"api_token\": \"39e58bd22275ccca486a3c3a9e00e578\"}";
            jsonResponse = HttpClient.IOTPost(url, jsonRequest);

            jo = JObject.Parse(jsonResponse);
            accessTaken = jo["accessToken"].ToString();
            return jo;
        }
        public string GetAppList()
        {
            url = "api/v1/application/getAppList";
            jsonRequest = "{\"current\": 1, \"rowCount\": 20}";
            jsonResponse = HttpClient.IOTPost(url, accessTaken, jsonRequest);

            return jsonResponse;
        }
        //JObject jo
        public string GetDeiveList()
        {
            //token
            accessTokenJo = GetAccessToken();

            //应用列表
            appListJo = JObject.Parse(GetAppList());

            //设备列表
            url = "api/v1/device/getGroup";
            jsonRequest = "{\"app_id\": 2, \"type_names\": [\"LoRaTempHumid\",\"human-detection\",\"LoRaPlug\"]}";
            jsonResponse = HttpClient.IOTPost(url, accessTaken, jsonRequest);

            WholeInfo wholeInfo = JsonConvert.DeserializeObject<WholeInfo>(jsonResponse);

            IEnumerable<JProperty> jProperties = wholeInfo.data.Properties();//拿到所有应用空间 每个应用空间中存在自己所有的设备信息

            foreach (JProperty jp in jProperties)
            {
                DetailGroupInfo detailGroupInfo = JsonConvert.DeserializeObject<DetailGroupInfo>(jp.Value.ToString());

                IEnumerable<JProperty> jgProperties = detailGroupInfo.children.Properties();//拿到当前应用空间下的所有设备


                foreach (JProperty jgp in jgProperties)
                {
                    DeiveInfo deiveInfo = JsonConvert.DeserializeObject<DeiveInfo>(jp.Value.ToString());
                }


                //写入新json
            }


            JObject j = new JObject(
                new JProperty("token", accessTokenJo),
                new JProperty("apps", appListJo),
                new JProperty("deives", jsonResponse)
                );
            return j.ToString();
        }

        //public string OpenDeive(JObject jo)
        //{
        //    SetState("","","1");
        //}
        //public string CloseDeive(JObject jo)
        //{
        //    SetState("","","0");
        //}
        //public string GetState(JObject jo)
        //{
        //    string url = "api/v1/device/getStatus";
        //    //string jsonRequest = "{'LoraPlug': {'all': false, 'group_ids': [ ], 'device_ids': ['GEK6510018','fake_sn']}}"; //all 是否忽略device_ids和group_ids
        //    jo.Add(new JProperty(Device.TypeName, new JObject(
        //        new JProperty("all", false),
        //        new JProperty("group_ids", new JArray()),
        //        new JProperty("device_ids", new JArray(Device.DeviceId))
        //        )));
        //    string jsonResponse = HttpClient.Post(url, txtTaken.Text.Trim(), jo.ToString());

        //}
        private void SetState(string groupId, string deiveId, string state)
        {
            string url = "api/v1/device/setStatus";
            JObject jo = new JObject();
            jo.Add(new JProperty("LoraPlug",
                new JObject(
                    new JProperty("device_ids", new JArray(deiveId)),
                    new JProperty("data", new JObject(new JProperty("DEV_SWITCH_STA", state))))
                ));
            string jsonResponse = HttpClient.IOTPost(url, accessTaken, jo.ToString());

        }
        #endregion

        #endregion

        private void FrmShell_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }

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

    #region 物联网
    public class TokenInfo
    {
        public string accessToken { get; set; }
        public int expiresIn { get; set; }
    }


    public class AppGroupInfo
    {
        public GroupInfo data { get; set; }
        public bool success { get; set; }
    }
    public class GroupInfo
    {
        public int current { get; set; }
        public int rowCount { get; set; }
        public int total { get; set; }
        public AppInfo[] apps { get; set; }
    }
    public class AppInfo
    {
        public int appID { get; set; }
        public string appName { get; set; }

    }


    public class WholeInfo
    {
        public bool success { get; set; }
        public int code { get; set; }
        public JObject data { get; set; }
    }
    public class DetailGroupInfo
    {
        public int groupId { get; set; }
        public bool leaf { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public JObject children { get; set; }
    }
    public class DeiveInfo
    {
        public string deviceId { get; set; }
        public string deviceName { get; set; }
        public string typeId { get; set; }
        public bool leaf { get; set; }
        public string id { get; set; }
        public string typeName { get; set; }
    }
    public class GroupData
    {
        public int groupId { get; set; }
        public bool leaf { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public List<DeiveInfo> deives { get; set; }
    }
    #endregion
}
