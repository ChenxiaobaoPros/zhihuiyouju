using log4net;
using Newtonsoft.Json;
using System;

namespace Aoto.CQMS.Common.JsonObj.CustgetseqJson.RequestJsonObject
{
    public class Head
    {
        //private static ILog log = LogManager.GetLogger("app");

        private string _tradeCode = string.Empty;
        private string _cmsIp = string.Empty;
        private string _channel = string.Empty;

        /// <summary>
        /// 交易代码
        /// </summary>
        public string tradeCode 
        {
            get 
            {
                return _tradeCode; 
            }
            set
            {
                _tradeCode = value.Trim().Equals("authentication") ? "kazhejudge" : value;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public string qmsIp
        {
            get
            {
                return _cmsIp;
            }
            set
            {
                _cmsIp = value;
            }
        }

        /// <summary>
        /// 渠道
        /// </summary>
        public string channel
        {
            get
            {
                return _channel;
            }
            set
            {
                _channel = value.Equals ("aoto")? "1" : value;
            }
        }
    }

    public class Body
    {
        private string _secgs = string.Empty;
        private string _phoneNo = string.Empty;

        /// <summary>
        /// 一级业务类型编号
        /// </summary>
        public string busiType1 { get; set; }

        /// <summary>
        /// 二级业务类型编号
        /// </summary>
        public string busiType2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string secgs
        {
            get
            {
                return _secgs;
            }
            set
            {
                _secgs = value;
            }
        }

        /// <summary>
        /// 介质类型：1-磁条卡/折、6-IC卡、3-身份证
        /// </summary>
        public string cardFlag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string phoneNo
        {
            get
            {
                return _phoneNo;
            }
            set
            {
                _phoneNo = value;
            }
        }

        /// <summary>
        /// 二磁信息
        /// </summary>
        public string secondTrack { get; set; }

        /// <summary>
        /// 三磁信息
        /// </summary>
        public string thirdTrack { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string certType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string certNo { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string custName { get; set; }

        /// <summary>
        /// 客户时间
        /// </summary>
        public string custTime { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string nation { get; set; }

        /// <summary>
        /// 发证机关
        /// </summary>
        public string office { get; set; }

        private string _signDate = string.Empty;
        /// <summary>
        /// 证件签发日期
        /// </summary>
        public string signDate 
        {
            get { return _signDate; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var temp = value.Split(new[] { '-' });
                    if (!string.IsNullOrEmpty(temp[0]))
                    {
                        DateTime dt = new DateTime();
                        if (DateTime.TryParse(temp[0], out dt))
                        {
                            _signDate = dt.Year.ToString() + dt.Month.ToString("D2") + dt.Day.ToString("D2");
                        }
                    }
                } 
            }
        }

        private string _indate = string.Empty;
        /// <summary>
        /// 证件有效期
        /// </summary>
        public string indate 
        {
            get { return _indate; }
            set 
            {
                if (!string.IsNullOrEmpty(value)) 
                {
                    var temp = value.Split(new[] { '-' });
                    if (!string.IsNullOrEmpty(temp[1])) 
                    {
                        DateTime dt = new DateTime();
                        if (DateTime.TryParse(temp[1], out dt))
                        {
                            _indate = dt.Year.ToString() + dt.Month.ToString("D2") + dt.Day.ToString("D2");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 户籍地址
        /// </summary>
        public string addr { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string sex { get; set; }

        private string _brithday = string.Empty;
        /// <summary>
        /// 出生日期
        /// </summary>
        public string birthday 
        { 
            get { return _brithday; } 
            set 
            {
                if (!string.IsNullOrEmpty(value))
                    _brithday = value;
                DateTime dt = new DateTime();
                if (DateTime.TryParse(_brithday, out dt)) 
                {
                    _brithday = dt.Year.ToString() + dt.Month.ToString("D2") + dt.Day.ToString("D2");
                }
            } 
        }

        /// <summary>
        /// 证件影像
        /// </summary>
        public string image { get; set; }
    }

    public class Biom
    {
        /// <summary>
        /// 
        /// </summary>
        public Head head { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Body body { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("robot")]
        public Biom biom { get; set; }
    }
}
