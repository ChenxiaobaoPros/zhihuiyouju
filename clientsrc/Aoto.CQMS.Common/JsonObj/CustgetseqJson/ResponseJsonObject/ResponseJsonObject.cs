using Newtonsoft.Json;

namespace Aoto.CQMS.Common.JsonObj.CustgetseqJson.ResponseJsonObject
{
    public class Head
    {
        private string _retCode = string.Empty;
        /// <summary>
        /// 平台返回码 (参照《状态返回码定义》)，返回显示返回信息|显示二级菜单|直接打印号票信息
        /// </summary>
        public string retCode { get { return _retCode; } set { _retCode = TransformData.TransformRetCode(value); } }
        /// <summary>
        /// 交易代码
        /// </summary>
        public string tradeCode { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string retMsg { get; set; }
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
        private string _ticketTemplate = string.Empty;
        /// <summary>
        /// 号票参数
        /// </summary>
        [JsonProperty("printtemp")]
        public string ticketTemplate 
        {
            get { return _ticketTemplate; }
            set { _ticketTemplate = TransformData.TransfromTicketTemplate(value); }
        }
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
