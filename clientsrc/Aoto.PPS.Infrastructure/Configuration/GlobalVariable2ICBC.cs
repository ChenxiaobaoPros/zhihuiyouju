using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Infrastructure.Configuration
{
    /// <summary>
    /// 全局命令信息
    /// </summary>
    public class GlobalVariable2ICBC
    {
        /// <summary>
        /// 签到 命令
        /// </summary>
        public readonly static string ICBC_PARA_QMSSIGN = "qmssign";

        /// <summary>
        /// 打印FROM字符串
        /// </summary>
        public readonly static string PRINT_FORMP_STR = "{\"FormName\":\"Queue7\",\"Fields\":{},\"MediaName\":\"ReceiptMedia\"}";

        /// <summary>
        /// 签到命令JSON字符串
        /// </summary>
        public readonly static string ICBC_QMSSIGN_STR = "{\"biom\":{\"head\":{\"tradeCode\":\"qmssign\",\"qmsIp\":\"" + BuzConfig2ICBC.LocalIP + "\"},\"body\":{}}}";

        /// <summary>
        /// 返回消息 josn字符串
        /// </summary>
        public readonly static string RETURN_MESSAGE_STR = "{\"biom\":{\"head\":{\"retCode\":\"\",\"retMsg\":\"\"},\"body\":{}}}";

        /// <summary>
        /// 替换字符:已屏蔽
        /// </summary>
        public readonly static string REPLACE_LOG_STR_01 = "（已屏蔽）";

        /// <summary>
        /// 签到json缓存
        /// </summary>
        public static JObject ICBC_QMSSIGN = null;

        /// <summary>
        /// 取号 命令
        /// </summary>
        public readonly static string ICBC_PARA_CUSTGETSEQ = "custgetseq";

        /// <summary>
        /// 刷卡折/身份证 命令
        /// </summary>
        public readonly static string ICBC_PARA_KAZHEJUDGE = "kazhejudge";

        /// <summary>
        /// 其他网点信息 命令
        /// </summary>
        public readonly static string ICBC_PARA_BRNOCONDS = "brnoconds";

        /// <summary>
        /// 预约 命令
        /// </summary>
        public readonly static string ICBC_PARA_RESERVATION = "reservation";

        /// <summary>
        /// 心跳包 命令
        /// </summary>
        public readonly static string ICBC_PARA_HEARTBEAT = "heartbeat";

        /// <summary>
        /// 远程卡Bin2查 命令
        /// </summary>
        public readonly static string ICBC_PARA_BIOMQMSCARDBIN = "biomqmscardbin";

        /// <summary>
        /// 本地卡Bin2查 命令
        /// </summary>
        public readonly static string ICBC_PARA_LOCALCARDBIN2SELECT = "localcardbin2select";

        /// <summary>
        /// 本地卡Bin2增 命令
        /// </summary>
        public readonly static string ICBC_PARA_LOCALCARDBIN2ADD = "localcardbin2add";

        /// <summary>
        /// 本地卡Bin2改 命令
        /// </summary>
        public readonly static string ICBC_PARA_LOCALCARDBIN2UPDATE = "localcardbin2update";

        /// <summary>
        /// 本地卡Bin2删 命令
        /// </summary>
        public readonly static string ICBC_PARA_LOCALCARDBIN2DELETE = "localcardbin2delete";

        /// <summary>
        /// 本地卡类型2查 命令
        /// </summary>
        public readonly static string ICBC_PARA_LOCALCARDTYPE2SELECT = "localcardtype2select";

        /// <summary>
        /// 本地卡类型2增 命令
        /// </summary>
        public readonly static string ICBC_PARA_LOCALCARDTYPE2ADD = "localcardtype2add";

        /// <summary>
        /// 本地卡类型2改 命令
        /// </summary>
        public readonly static string ICBC_PARA_LOCALCARDTYPE2UPDATE = "localcardtype2update";

        /// <summary>
        /// 本地卡类型2删 命令
        /// </summary>
        public readonly static string ICBC_PARA_LOCALCARDTYPE2DELETE = "localcardtype2delete";

        /// <summary>
        /// 基础配置2查 命令
        /// </summary>
        public readonly static string ICBC_PARA_BASICCONFIG2SELECT = "basicconfig2select";

        /// <summary>
        /// 基础配置2改 命令
        /// </summary>
        public readonly static string ICBC_PARA_BASICCONFIG2UPDATE = "basicconfig2update";

        /// <summary>
        /// 工行专用2查 命令
        /// </summary>
        public readonly static string ICBC_PARA_ICBCSPECIALUSE2SELECT = "icbcspecialuse2select";

        /// <summary>
        /// 工行专用2改 命令
        /// </summary>
        public readonly static string ICBC_PARA_ICBCSPECIALUSE2UPDATE = "icbcspecialuse2update";

        /// <summary>
        /// 禁止脱机取号 true:禁止，false：不禁止
        /// </summary>
        public static Boolean NoOfflineAccessFlag = false;

        /// <summary>
        /// 号票配置2查 命令
        /// </summary>
        public readonly static string ICBC_PARA_TICKETSCONFIG2SELECT = "ticketsconfig2select";

        /// <summary>
        /// 号票配置2改 命令
        /// </summary>
        public readonly static string ICBC_PARA_TICKETSCONFIG2UPDATE = "ticketsconfig2update";

        /// <summary>
        /// 更新配置2查 命令
        /// </summary>
        public readonly static string ICBC_PARA_UPDATECONFIG2SELECT = "updateconfig2select";

        /// <summary>
        /// 更新配置2改 命令
        /// </summary>
        public readonly static string ICBC_PARA_UPDATECONFIG2UPDATE = "updateconfig2update";

        /// <summary>
        /// 语音配置2查 命令
        /// </summary>
        public readonly static string ICBC_PARA_VOICECONFIG2SELECT = "voiceconfig2select";

        /// <summary>
        /// 语音配置2改 命令
        /// </summary>
        public readonly static string ICBC_PARA_VOICECONFIG2UPDATE = "voiceconfig2update";

        /// <summary>
        /// 关于配置 命令
        /// </summary>
        public readonly static string ICBC_PARA_ABOUT = "about";

        /// <summary>
        /// 营业厅分区2查 命令
        /// </summary>
        public readonly static string ICBC_PARA_BUSINESSHALL2SELECT = "businesshall2select";

        /// <summary>
        /// 营业厅分区2增 命令
        /// </summary>
        public readonly static string ICBC_PARA_BUSINESSHALL2ADD = "businesshall2add";

        /// <summary>
        /// 营业厅分区2改 命令
        /// </summary>
        public readonly static string ICBC_PARA_BUSINESSHALL2UPDATE = "businesshall2update";

        /// <summary>
        /// 营业厅分区2删 命令
        /// </summary>
        public readonly static string ICBC_PARA_BUSINESSHALL2DELETE = "businesshall2delete";

        /// <summary>
        /// 柜台设置2查 命令
        /// </summary>
        public readonly static string ICBC_PARA_COUNTERCONFIG2SELECT = "counterconfig2select";

        /// <summary>
        /// 柜台设置2增 命令
        /// </summary>
        public readonly static string ICBC_PARA_COUNTERCONFIG2ADD = "counterconfig2add";

        /// <summary>
        /// 柜台设置2改 命令
        /// </summary>
        public readonly static string ICBC_PARA_COUNTERCONFIG2UPDATE = "counterconfig2update";

        /// <summary>
        /// 柜台设置2删 命令
        /// </summary>
        public readonly static string ICBC_PARA_COUNTERCONFIG2DELETE = "counterconfig2delete";

        /// <summary>
        /// 窗口队列调整 命令
        /// </summary>
        public readonly static string ICBC_PARA_WINDOWQUEUEADJUST = "windowQueueAdjust";

        /// <summary>
        /// 业务查询 命令
        /// </summary>
        public readonly static string ICBC_PARA_SERVICEQUERY = "servicequery";

        /// <summary>
        /// 队列查询 命令
        /// </summary>
        public readonly static string ICBC_PARA_QUEUEQUERY = "queuequery";

        /// <summary>
        /// 队列属性更改 命令
        /// </summary>
        public readonly static string ICBC_PARA_QUEUEATTRIBUTESUPDATE = "queueAttributesUpdate";

        /// <summary>
        /// 本地号码查询 命令
        /// </summary>
        public readonly static string ICBC_PARA_LOCALNUMBERQUERY = "localNumberQuery";

        /// <summary>
        /// 删除废号 命令
        /// </summary>
        public readonly static string ICBC_PARA_DELETEABOLISHNUMBER = "deleteabolishNumber";

        /// <summary>
        /// 联动关机 命令
        /// </summary>
        public readonly static string ICBC_PARA_LINKAGESHUTDOWN = "linkageshutdown";

        /// <summary>
        /// 设备状态更新 命令
        /// </summary>
        public readonly static string ICBC_PARA_STATEUPDATE = "stateUpdate";


        //////////////////////////////////////

        /// <summary>
        /// 密码不正确|Incorrect password
        /// </summary>
        public readonly static string MESS_TEXT_01 = "密码不正确|Incorrect password";

        /// <summary>
        /// 执行联动关机命令，等待叫号机执行关机返回命令...|The implementation of the linkage shutdown command, wait for the call machine shutdown return command...
        /// </summary>
        public readonly static string MESS_TEXT_02 = "执行联动关机命令，等待叫号机执行关机返回命令...|The implementation of the linkage shutdown command, wait for the call machine shutdown return command...";

        /// <summary>
        /// 执行跳转配置界面成功|Execute the jump configuration interface successfully
        /// </summary>
        public readonly static string MESS_TEXT_03 = "执行跳转配置界面成功|Execute the jump configuration interface successfully";

        /// <summary>
        /// 当前已经切换至脱机模式！|Switch to offline mode!
        /// </summary>
        public readonly static string MESS_TEXT_04 = "当前已经切换至脱机模式！|Switch to offline mode!";

        /// <summary>
        /// 当前已经切换至联机模式！|Switch to online mode!
        /// </summary>
        public readonly static string MESS_TEXT_05 = "当前已经切换至联机模式！|Switch to online mode!";

        /// <summary>
        /// 未知密码|Unknown password
        /// </summary>
        public readonly static string MESS_TEXT_06 = "未知密码|Unknown password";

        /// <summary>
        /// 值守切换非值守模式成功！|Duty switch non duty mode success!
        /// </summary>
        public readonly static string MESS_TEXT_07 = "值守切换非值守模式成功！|Duty switch non duty mode success!";

        /// <summary>
        /// 非值守切换值守模式成功！|Off duty switch mode!
        /// </summary>
        public readonly static string MESS_TEXT_08 = "非值守切换值守模式成功！|Off duty switch mode!";

        /// <summary>
        /// 失败|fail
        /// </summary>
        public readonly static string MESS_TEXT_09 = "失败|fail";

        /// <summary>
        /// 排队机当前不允许切换大堂经理值守/非值守模式！|Queuing machine is currently not allowed to switch to the lobby manager on duty / off duty mode!
        /// </summary>
        public readonly static string MESS_TEXT_10 = "排队机当前不允许切换大堂经理值守/非值守模式！|Queuing machine is currently not allowed to switch to the lobby manager on duty / off duty mode!";

        /// <summary>
        /// 未获取到联脱机状态值！|Offline status value not acquired!
        /// </summary>
        public readonly static string MESS_TEXT_11 = "未获取到联脱机状态值！|Offline status value not acquired!";

        /// <summary>
        /// 调用联脱机接口失败，不允许切换到联机模式！|Failed to call offline interface and is not allowed to switch to online mode!
        /// </summary>
        public readonly static string MESS_TEXT_12 = "调用联脱机接口失败，不允许切换到联机模式！|Failed to call offline interface and is not allowed to switch to online mode!";

    }
}
