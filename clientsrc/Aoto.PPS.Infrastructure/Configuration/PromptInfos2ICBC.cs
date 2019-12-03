using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoto.PPS.Infrastructure.Configuration
{
    /// <summary>
    /// 全局提示信息
    /// </summary>
    public class PromptInfos2ICBC
    {

        public readonly static string ICBC_MESS_QCMEXT01 = "通讯故障,请再次尝试;或咨询大堂经理|Communication failure. Please try again or consult the lobby manager";


        public readonly static string ICBC_MESS_QCMEXT04 = "执行回调数据异常|Perform callback data exception";

        public readonly static string ICBC_MESS_QCMEXT02 = "接收返回数据失败|Receive return data failed";


        public readonly static string ICBC_MESS_QCMEXT03 = "签到失败，启动自动签到...|Sign in failed, start auto sign in...";

        public readonly static string ICBC_MESS_QCMEXT05 = "保存数据库失败|Failed to save the database";

    }
}
