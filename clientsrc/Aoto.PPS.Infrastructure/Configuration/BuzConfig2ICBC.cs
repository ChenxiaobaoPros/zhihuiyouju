using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Infrastructure.Configuration
{
    /// <summary>
    /// 设备网关配置缓存信息
    /// </summary>
    public class BuzConfig2ICBC
    {
        public static string LIP = "122.138.45.222";

        /// <summary>
        /// 参数锁
        /// </summary>
        public readonly static object staticLook = new object();

        /// <summary>
        /// 本机IP
        /// </summary>
        public static string LocalIP = String.Empty;

        #region 签到
        /// <summary>
        /// 主机状态|备机状态|联网状态|值守非值守状态
        /// </summary>
        public static string DevStatus = String.Empty;
        /// <summary>
        /// 页面超时时间 秒
        /// </summary>
        public static string PageTimeout = String.Empty;
        /// <summary>
        /// 延后启动时间 秒
        /// </summary>
        public static string DelayedStartTime = String.Empty;
        /// <summary>
        /// 叫号显示屏产品推荐启用状态
        /// </summary>
        public static string QmsUseFlag = String.Empty;

        /// <summary>
        /// 上传身份证图片标记 0-不启用 1-启用，默认 0
        /// </summary>
        public static string CertImageFlag = "0";

        /// <summary>
        /// 值守非值守启用标记  1：启用  0：不启用
        /// </summary>
        public static string DutyFlag = "1";

        #endregion


        #region 提示信息


        /// <summary>
        /// 模式切换中,请稍候...|Please wait while mode switch...
        /// </summary>
        public readonly static string INFO_01 = "模式切换中,请稍候...|Please wait while mode switch...";

        /// <summary>
        /// 正在处理中,请稍候...|Queuing parameters are being obtained. Please wait...
        /// </summary>
        public readonly static string INFO_39 = "正在处理中,请稍候...|Queuing parameters are being obtained. Please wait...";

        #endregion


        #region 基础配置

        /// <summary>
        /// 关机密码
        /// </summary>
        public static string ShutdownPwd = String.Empty;

        /// <summary>
        /// 取号界面退出密码
        /// </summary>
        public static string ExitGetTicketPwd = String.Empty;

        /// <summary>
        /// 联机脱机切换密码
        /// </summary>
        public static string OnlineSwitchPwd = String.Empty;

        /// <summary>
        /// 值守/非值守模式切换密码
        /// </summary>
        public static string DutySwitchPwd = String.Empty;

        /// <summary>
        /// 取号是否自动关机 1-开启，0-关闭
        /// </summary>
        public static string GetAutoShutdownFlag = String.Empty;

        /// <summary>
        /// 取号机关机时间
        /// </summary>
        public static string GetShutdownTime = String.Empty;

        /// <summary>
        /// 最小化程序
        /// </summary>
        public static string MinimizedPage = "930608";

        #endregion

        #region 工行专用

        /// <summary>
        /// 禁止脱机取号
        /// </summary>
        public static string DisOffGetFlag = String.Empty;

        #endregion

        #region 号票配置
        /// <summary>
        ///号票打印格式
        /// </summary>
        public static string TickePrintFormat = String.Empty;
        #endregion

        #region 更新配置
        /// <summary>
        /// 是否启用更新(0否 1是)
        /// </summary>
        public static string UpdateFlag = String.Empty;
        /// <summary>
        /// 服务器IP
        /// </summary>
        public static string HttpIp = String.Empty;
        /// <summary>
        /// 服务器端口
        /// </summary>
        public static string HttpPort = String.Empty;
        /// <summary>
        /// 本地存储地址
        /// </summary>
        public static string LocalPath = String.Empty;
        /// <summary>
        /// 备份文件地址
        /// </summary>
        public static string BackupPath = String.Empty;
        #endregion

        #region 语音配置
        /// <summary>
        /// 语音呼叫次数
        /// </summary>
        public static string SoundsPeakTimes = String.Empty;
        /// <summary>
        /// 所有业务使用同一语音播放（0否 1是）
        /// </summary>
        public static string UsesameLanspeakFlag = String.Empty;
        /// <summary>
        /// 播放语言，0-中文 1-英文 2-粤语，多个用“|”拼接",
        /// </summary>
        public static string SpeakLanguage = String.Empty;
        /// <summary>
        /// 是否只播放指定窗口语音",（0否 1是）
        /// </summary>
        public static string SpeakSpecificwinFlag = String.Empty;
        /// <summary>
        /// 指定窗口，多个窗口用“|”拼接
        /// </summary>
        public static string SpecificWin = String.Empty;
        #endregion

        #region 营业分区
        /// <summary>
        /// "分区编号"
        /// </summary>
        public static string DistrictId = "1";
        /// <summary>
        ///  "分区名称"
        /// </summary>
        public static string DistrictName = "";
        #endregion

        #region 柜台配置
        /// <summary>
        /// "窗口ID"
        /// </summary>
        public static string WinId = "";
        /// <summary>
        ///"分区编号"
        /// </summary>
        public static string Zoneno = "";
        /// <summary>
        /// "显示为"
        /// </summary>
        public static string WinViewNum = "";
        /// <summary>
        /// "窗口编号"
        /// </summary>
        public static string WinCode = "";
        /// <summary>
        /// "窗口IP"
        /// </summary>
        public static string WinIp = "";
        /// <summary>
        /// "窗口名称"
        /// </summary>
        public static string WinName = "";


        #endregion

        #region 业务查询
        /// <summary>
        /// 是否在排号机显示
        /// </summary>
        public static string QmsFlag = String.Empty;
        /// <summary>
        ///非值守菜单
        /// </summary>
        public static string IsDuty = String.Empty;
        #endregion

        #region 关于
        /// <summary>
        /// 叫号app版本
        /// </summary>
        public static string AppVersion = String.Empty;
        /// <summary>
        ///  驱动号版本
        /// </summary>
        public static string DriveVersion = String.Empty;
        /// <summary>
        /// 厂商标识
        /// </summary>
        public static string VenDor = String.Empty;

        /// <summary>
        /// 关于的json报文
        /// </summary>
        public static string About2JsonStr = String.Empty;

        public static string SpVersion = String.Empty;
        public static string OcxVersion = String.Empty;
        public static string QmsVersion = String.Empty;

        #endregion

        #region 预约

        //号票参数
        public static string PrintTemp = String.Empty;

        #endregion

        #region  成功或失败

        public readonly static string Success = "0";
        public readonly static string Filed = "1";
        #endregion

        public static void Jo2Return1(JObject jo,string str)
        {
            //一种json数据结构
            JObject jokeit = JObject.Parse("{\"biom\":{\"head\":{\"retCode\":\"1\",\"retMsg\":\"\"},\"body\":{\"cardFlag\":\"\"}}}");

            JToken joBiom = jokeit["biom"];

            jo["biom"] = joBiom;
        }

        public static void Jo2Return(JObject jo)
        {
            JObject jokeit = JObject.Parse("{ \"biom\": { \"head\": { \"retCode\": \"" + 1 + "\", \"retMsg\": \"" + PromptInfos2ICBC.ICBC_MESS_QCMEXT01 + "\" }, \"body\": {  }	 } }");

            JToken joBiom = jokeit["biom"];

            jo["biom"] = joBiom;
        }
        public static void Jo3Return(JObject jo)
        {
            JObject jokeit = JObject.Parse("{ \"biom\": { \"head\": { \"retCode\": \"" + 1 + "\", \"retMsg\": \"" + PromptInfos2ICBC.ICBC_MESS_QCMEXT02 + "\" }, \"body\": {  }	 } }");

            JToken joBiom = jokeit["biom"];

            jo["biom"] = joBiom;
        }

        public static void Jo4Return(JObject jo)
        {
            JObject jokeit = JObject.Parse("{ \"biom\": { \"head\": { \"retCode\": \"" + 1 + "\", \"retMsg\": \"" + PromptInfos2ICBC.ICBC_MESS_QCMEXT03 + "\" }, \"body\": {  }	 } }");

            JToken joBiom = jokeit["biom"];

            jo["biom"] = joBiom;
        }


        public static void Jo5Return(JObject jo)
        {
            JObject jokeit = JObject.Parse("{ \"biom\": { \"head\": { \"retCode\": \"" + 1 + "\", \"retMsg\": \"" + PromptInfos2ICBC.ICBC_MESS_QCMEXT04 + "\" }, \"body\": {  }	 } }");

            JToken joBiom = jokeit["biom"];

            jo["biom"] = joBiom;
        }

        public static void Jo6Return(JObject jo)
        {
            JObject jokeit = JObject.Parse("{ \"biom\": { \"head\": { \"retCode\": \"" + 1 + "\", \"retMsg\": \"" + PromptInfos2ICBC.ICBC_MESS_QCMEXT05 + "\" }, \"body\": {  }	 } }");

            JToken joBiom = jokeit["biom"];

            jo["biom"] = joBiom;
        }

    }
}