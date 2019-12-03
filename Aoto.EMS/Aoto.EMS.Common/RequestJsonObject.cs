using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoto.EMS.Common.RequestJsonObject
{
    /// <summary>
    /// 外设状态，信息
    /// </summary>
    public class Head
    {
        /// <summary>
        /// 高拍
        /// </summary>
        public int HighBeatState { get; set; }
        //指纹
        public int FingerState { get; set; }
        //扫码
        public int SweepCodeState { get; set; }
        //双摄监控
        public int DoubleShotState { get; set; }
        //身份证
        public int IDReaderState { get; set; }
        //磁条卡
        public int MagCardState { get; set; }
    }
    /// <summary>
    /// 人的信息
    /// </summary>
    public class Body
    {
        public int AccountNo { get; set; }
        public DateTime CreateTime { get; set; }
        public string BusinessName { get; set; }
        public string BusinessType { get; set; }
        public string Name { get; set; }
        public int IdNo { get; set; }
        public string Adress { get; set; }
        public string DetailAdress { get; set; }
        public string IdHeadsUrl { get; set; }
        public string IdtailsUrl { get; set; }
        public string FaceUrl { get; set; }
        public string FingerTemp { get; set; }

    }
    public class RequestJsonObject
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
}
