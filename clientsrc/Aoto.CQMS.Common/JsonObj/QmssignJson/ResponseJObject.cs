using System.Collections.Generic;
using Newtonsoft.Json;

namespace Aoto.CQMS.Common.JsonObj.QmssignJson
{
    public class SctBusisItem
    {
        /// <summary>
        /// 业务类型编号
        /// </summary>
        public string busiNo { get; set; }

        /// <summary>
        /// 业务类型名称
        /// </summary>
        public string busiName { get; set; }

        private string _sTime1 = string.Empty;
        /// <summary>
        /// 上午开始时间
        /// </summary>
        [JsonProperty("amsTime")]
        public string sTime1 
        { 
            get 
            {
                return _sTime1; 
            } 
            set 
            { 
                _sTime1 = string.IsNullOrEmpty(value) ? "00:00:01" : value; 
            } 
        }

        private string _eTime1 = string.Empty;
        /// <summary>
        /// 上午结束时间
        /// </summary>
        [JsonProperty("ameTime")]
        public string eTime1
        {
            get
            {
                return _eTime1;
            } 
            set 
            {
                _eTime1 = string.IsNullOrEmpty(value) ? "12:00:00" : value; 
            } 
        }

        private string _sTime2 = string.Empty;
        /// <summary>
        /// 下午开始时间
        /// </summary>
        [JsonProperty("pmsTime")]
        public string sTime2 
        {
            get 
            { 
                return _sTime2; 
            } 
            set 
            { 
                _sTime2 = string.IsNullOrEmpty(value) ? "12:00:01" : value;
            } 
        }

        private string _eTime2 = string.Empty;
        /// <summary>
        /// 下午结束时间
        /// </summary>
        [JsonProperty("pmeTime")]
        public string eTime2 
        {
            get 
            { 
                return _eTime2; 
            } 
            set
            { 
                _eTime2 = string.IsNullOrEmpty(value) ? "23:59:59" : value; 
            } 
        }

        /// <summary>
        /// 分流提示
        /// </summary>
        public string flnote { get; set; }

        /// <summary>
        /// 是否在排号机显示
        /// </summary>
        public string qmsFlag { get; set; }

        /// <summary>
        /// 非值守菜单
        /// </summary>
        public string isDuty { get; set; }

        /// <summary>
        /// 业务类型英文名称
        /// </summary>
        public string busiNameEn { get; set; }

        /// <summary>
        /// 英文分流提示
        /// </summary>
        public string flnoteEn { get; set; }
    }

    /// <summary>
    /// 普通
    /// </summary>
    public class OrdBusiness
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("sctNoBusi")]
        public List<SctBusisItem> sctBusis { get; set; }
    }
    
    /// <summary>
    /// 对私介质
    /// </summary>
    public class MediaBusiness
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("sctPerBusi")]
        public List<SctBusisItem> sctBusis { get; set; }
    }

    /// <summary>
    /// 对公介质
    /// </summary>
    public class VipMediaBusiness
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("sctCusBusi")]
        public List<SctBusisItem> sctBusis { get; set; }
    }

    public class Head
    {
        private string _retCode = string.Empty;
        /// <summary>
        /// 平台返回码 (参照《状态返回码定义》)，返回显示返回信息|显示二级菜单|直接打印号票信息
        /// </summary>
        public string retCode { get { return _retCode; } set { _retCode = TransformData.TransformRetCode(value); } }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string retMsg { get; set; }

        /// <summary>
        /// 交易代码
        /// </summary>
        public string tradeCode { get; set; }

        /// <summary>
        /// 响应时间
        /// </summary>
        public string retTime { get; set; }

        /// <summary>
        /// 机器人ID
        /// </summary>
        public string robotID { get; set; }

        /// <summary>
        /// 机构编码
        /// </summary>
        public string orgCode { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string channel { get; set; }
    }

    public class Body
    {
        /// <summary>
        /// 营业开始时间
        /// </summary>
        public string orgTimePeriodS { get; set; }

        /// <summary>
        /// 营业结束时间
        /// </summary>
        public string orgTimePeriodE { get; set; }

        /// <summary>
        /// 分流提示
        /// </summary>
        public string remind { get; set; }

        /// <summary>
        /// 英文提示标志
        /// </summary>
        public string remindFlag { get; set; }

        /// <summary>
        /// 英文分流提示
        /// </summary>
        public string remindEn { get; set; }

        /// <summary>
        /// 预约客户菜单显示标志
        /// </summary>
        public string ptShowFlag { get; set; }

        /// <summary>
        /// 客户持有产品情况是否打印
        /// </summary>
        public string cliProPrintFlag { get; set; }

        /// <summary>
        /// 客户取号时间是否打印
        /// </summary>
        public string cliTimPrintFlag { get; set; }

        /// <summary>
        /// 分流提示是否打印
        /// </summary>
        public string flPrintFlag { get; set; }

        /// <summary>
        /// 营销信息是否打印
        /// </summary>
        public string yxPrintFlag { get; set; }

        /// <summary>
        /// 要客提示是否打印
        /// </summary>
        public string spcliPrintFlag { get; set; }

        /// <summary>
        /// 温馨提示是否打印
        /// </summary>
        public string notePrintFlag { get; set; }

        /// <summary>
        /// 号票打印样式Json
        /// </summary>
        public string ticketPrintJson { get; set; }

        /// <summary>
        /// 同步时间 yyyy-mm-dd hh:mm:ss
        /// </summary>
        public string synchroTime { get; set; }

        /// <summary>
        /// 值守启用标志
        /// </summary>
        public string dutyFlag { get; set; }

        /// <summary>
        /// 普通业务
        /// </summary>
        [JsonProperty("sctNoBusiList")]
        public OrdBusiness ordBusiness { get; set; }

        /// <summary>
        /// 对私介质
        /// </summary>
        [JsonProperty("sctPerBusiList")]
        public MediaBusiness mediaBusiness { get; set; }

        /// <summary>
        /// 对公介质
        /// </summary>
        [JsonProperty("sctCusBusiList")]
        public VipMediaBusiness vipMediaBusiness { get; set; }
    }

    public class Robot
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
        [JsonProperty("biom")]
        public Robot robot { get; set; }
    }
}
