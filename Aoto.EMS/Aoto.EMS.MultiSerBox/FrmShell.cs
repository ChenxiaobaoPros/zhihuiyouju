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
        public FrmShell()
        {
            InitializeComponent();
            webBrowser.ObjectForScripting = this;
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.ScrollBarsEnabled = true ;
        }
        public void Shut()
        {
            throw new NotImplementedException();
        }
        public void ResetPrint()
        {
            throw new NotImplementedException();
        }
        public void ReadRawDataInvoker(global::Newtonsoft.Json.Linq.JObject jo)
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
        public void PeripheralInvoke(global::Newtonsoft.Json.Linq.JObject jo)
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
        }

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
            return JsonConvert.SerializeObject(PageData);
        }
        #endregion

        #region 页面操作
        //办理业务
        private string bussisType { get; set; }
        public string HandleBuss(string bus)
        {
            JObject jo = JObject.Parse(bus);
            IEnumerable<Product> quelist = list.Where(b=>b.BusinessName == jo.Value<string>("BusinessName"));
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
        IDCardReader iDCardReader;
        public void InitIDCard()
        {
            if (iDCardReader == null)
            {
                iDCardReader = new IDCardReader();
                iDCardReader.RunCompletedEvent += new RunCompletedEventHandler(IDCallBack);
            }
            iDCardReader.ReadAsync(new JObject());//参数暂时无用，结合项目使用
        }

        private void IDCallBack(object sender, RunCompletedEventArgs e)
        {
            JObject ret = (JObject)e.Result;
            JObject jo = new JObject();
            jo["accountNo"] = "123123123";
            jo["date"] = DateTime.Now;
            jo["enterpriseName"] = "企业名称";
            jo["bussisType"] = bussisType;

            jo["name"] = ret.Value<string>("certName");
            jo["idNo"] = ret.Value<string>("certNo");

            jo["retCode"] = 0;
            jo["callback"] = "identity";
            ScriptInvoke(jo);
        }
        #endregion

        #region 二维码
        ReadQRCode readQRCode;
        public void loadQRCode()
        {
            readQRCode = new ReadQRCode();

            readQRCode.Initialize();

        }
        public void closeQRCode()
        {
            if (readQRCode != null)
                readQRCode.CloseQRCode();

        }


        #endregion

        #region 指纹
        Finger finger;
        public void loadFinger()
        {
            if(finger==null)
                finger = new Finger();

            finger.Initialize();
            //DialogResult dialogResult = FrmFinger.Instance.ShowDialog();
            //if (DialogResult.OK != dialogResult)
            //{
            //    return;
            //}

        }
        #endregion

        #region 键盘
        KeyBoard KeyBoard;
        public void loadKeyBoard()
        {
            if (KeyBoard == null)
                KeyBoard = new KeyBoard();
        }
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
            if(hybridCardReader == null)
                hybridCardReader = new HybridCardReader();
            hybridCardReader.BackCard();
            return null;
        }
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
        public string[] BussisTypes { get; set; } ={ "养老保险", "人寿保险", "车辆保险" };
        public List<Product> businesslist { get; set; }

    }
}
