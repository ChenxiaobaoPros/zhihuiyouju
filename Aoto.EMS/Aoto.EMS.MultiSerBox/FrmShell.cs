using Aoto.EMS.Common;
using Aoto.EMS.Common.RequestJsonObject;
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
        private string result = "";
        ResponseJsonObject responseJsonObject;
        public FrmShell()
        {
            InitializeComponent();
            webBrowser.ObjectForScripting = this;
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.ScrollBarsEnabled = true ;

            RequestJsonObject root = new RequestJsonObject
            {
                head = new Head
                {
                    HighBeatState = 0,
                    FingerState = 0,
                    SweepCodeState = 0,
                    DoubleShotState = 0,
                    IDReaderState = 0,
                    MagCardState = 0,
                },
                body = new Body
                {
                    AccountNo = 12312,
                    CreateTime = DateTime.Now,
                    DetailAdress = "广东省 东莞市 凤岗镇 永盛大街 ",
                    Adress = "广东省 东莞市 凤岗镇 永盛大街 ",
                    BusinessName = "业务名字",
                    BusinessType = "业务类型",
                    FaceUrl = "img/LOGO.png",
                    FingerTemp = "web/LOGO.png",
                    IdHeadsUrl = "img/LOGO.png",
                    IdNo = 2,
                    IdtailsUrl = "img/LOGO.png",
                    Name = "名字",
                }
            };

            responseJsonObject = new ResponseJsonObject
            {
                InsuranceType = new string[] { "养老保险", "人寿保险", "车辆保险" },
                businesslist = new List<Business>()
                {
                    new Business
                    {
                        BusinessType = "养老保险",
                        BusinessName = "sdsdsd",
                        BusinessNameNo = "sdsd",
                        RealBusiness = "sds"

                    },
                    new Business
                    {
                         BusinessType = "养老保险",
                        BusinessName = "sdsdsd",
                        BusinessNameNo = "sdsd",
                        RealBusiness = "sds"
                    },
                    new Business
                    {
                         BusinessType = "人寿保险",
                        BusinessName = "sdsdsd",
                        BusinessNameNo = "sdsd",
                        RealBusiness = "sds"
                    },
                    new Business
                    {
                         BusinessType = "人寿保险",
                        BusinessName = "sdsdsd",
                        BusinessNameNo = "sdsd",
                        RealBusiness = "sds"
                    },
                    new Business
                    {
                         BusinessType = "车辆保险",
                        BusinessName = "sdsdsd",
                        BusinessNameNo = "sdsd",
                        RealBusiness = "sds"
                    },
                    new Business
                    {
                         BusinessType = "车辆保险",
                        BusinessName = "sdsdsd",
                        BusinessNameNo = "sdsd",
                        RealBusiness = "sds"
                    }
                }
            };


            result = JsonConvert.SerializeObject(responseJsonObject);

        }

        public void Navigate(string url)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                webBrowser.Navigate(url);
            }));
        }

        public string GetInfo()
        {
            return result;
        }
        public string SeachBuss(string type)
        {
            List<Business> list = new List<Business>();
            foreach (Business item in responseJsonObject.businesslist)
            {
                if (item.BusinessType == type)
                    list.Add(item);
            }

            return JsonConvert.SerializeObject(list);
        }
        public string HandleBuss(string bus)
        {
            JObject jo = JObject.Parse(bus);
            string str = jo.Value<string>("BusinessNameNo");
            IEnumerable<Business> list = responseJsonObject.businesslist.Where(b => b.BusinessNameNo == jo.Value<string>("BusinessNameNo"));
            if (list != null && list.Count() != 0)
                return "0";
            else
                return "1";
        }
        public void PeripheralInvoke(global::Newtonsoft.Json.Linq.JObject jo)
        {
            throw new NotImplementedException();
        }

        public void ReadRawDataInvoker(global::Newtonsoft.Json.Linq.JObject jo)
        {
            throw new NotImplementedException();
        }

        public void ResetPrint()
        {
            throw new NotImplementedException();
        }

        public void ScriptInvoke(global::Newtonsoft.Json.Linq.JObject jo)
        {
            throw new NotImplementedException();
        }

        public void Shut()
        {
            throw new NotImplementedException();
        }

        private void FrmShell_Load(object sender, EventArgs e)
        {
            webBrowser.Navigate(Path.Combine(Config.AppRoot, "web\\qms\\html\\admin\\index.html"));
            //webBrowser.Navigate(AppState.WelcomeUrl);
        }


        public void LoadBorad()
        {
            DialogResult dialogResult = FrmBoard.Instance.ShowDialog();
            if (DialogResult.OK != dialogResult)
            {
                return;
            }

        }
        public void loadFinger()
        {
            DialogResult dialogResult = FrmFinger.Instance.ShowDialog();
            if (DialogResult.OK != dialogResult)
            {
                return;
            }

        }
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
        IDCardReader iDCardReader;
        public string ReadIDCard()
        {
            if(iDCardReader==null)
                iDCardReader = new IDCardReader();
            else
                iDCardReader.Initialize();
            JObject jo = new JObject();
            iDCardReader.Read(jo);
            return jo.ToString();
        }
        MoveCard moveCard;
        public string loadMoveCard()
        {
            if(moveCard ==null)
                moveCard = new MoveCard();
            moveCard.BackCard();
            return null;
        }
        public void loadKeyBoard()
        {
        }
        private void FrmShell_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
