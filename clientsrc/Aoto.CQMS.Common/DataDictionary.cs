using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoto.CQMS.Common
{
    public enum ResponseCode 
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 失败
        /// </summary>
        Fail = 1,

        /// <summary>
        /// 显示返回信息
        /// </summary>
        ShowReturnInfo,

        /// <summary>
        /// 显示个人二级菜单
        /// </summary>
        ShowPerL2Menu,

        /// <summary>
        /// 显示对公二级菜单
        /// </summary>
        ShowPubL2Menu,

        /// <summary>
        /// 直接打印号票信息
        /// </summary>
        PrintInfo,

        /// <summary>
        /// 提前
        /// </summary>
        InAdvance,

        /// <summary>
        /// 延后
        /// </summary>
        PutOff,

        /// <summary>
        /// 非预约
        /// </summary>
        NonBooked,

        /// <summary>
        /// 其他
        /// </summary>
        Other,

        /// <summary>
        /// 显示非工行卡-跳无卡无折
        /// </summary>
        ShowUnICBC_NoCard_NoBook,
    }

    public enum ReturnRobotRetCode
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        Success = 200,

        /// <summary>
        /// 接收请求，获取报文长度异常
        /// </summary>
        Fail_1 = 400,

        /// <summary>
        /// 获取的数据长度和指定的接收长度不一致
        /// </summary>
        Fail_2,

        /// <summary>
        /// 请求处理内部异常
        /// </summary>
        Fail_3,

        /// <summary>
        /// 不可知的请求类型
        /// </summary>
        Fail_4,

        /// <summary>
        /// 解析请求数据异常
        /// </summary>
        Fail_5,

        /// <summary>
        /// 解析请求报文中的查询条件异常，未能获取到机构号，或者预约日期
        /// </summary>
        Fail_6,

        /// <summary>
        /// 根据机构编码未能查询到机构
        /// </summary>
        Fail_7,

        /// <summary>
        /// 查询网点业务信息，数据库操作异常
        /// </summary>
        Fail_8,

        /// <summary>
        /// 直接取号次数达到限定值
        /// </summary>
        Fail_9,

        /// <summary>
        /// 发送到排队机的报文，打包出错
        /// </summary>
        Fail_10,

        /// <summary>
        /// 发送数据到排队机出错
        /// </summary>
        Fail_11,

        /// <summary>
        /// 【远程取号】：解析返回值出现异常
        /// </summary>
        Fail_12,

        /// <summary>
        /// 【远程取号】：手动组包出错
        /// </summary>
        Fail_13,

        /// <summary>
        /// 排队机端获取请求数据异常
        /// </summary>
        Fail_14,

        /// <summary>
        /// 排队机端生成远程票号异常
        /// </summary>
        Fail_15,

        /// <summary>
        /// 排队机端位置的异常
        /// </summary>
        Fail_16,

        /// <summary>
        /// 验证客户是否还能取号异常
        /// </summary>
        Fail_17,

        /// <summary>
        /// 绑定出票成功，返回号票信息
        /// </summary>
        CheckTicket = 1,

        /// <summary>
        /// 刷卡失败，请选择普通业务
        /// </summary>
        CardFailure,

        /// <summary>
        /// 选择介质取号业务
        /// </summary>
        SelectCardBusi,

        /// <summary>
        /// 选择VIP取号业务
        /// </summary>
        SelectVipBusi,
    }

    public static class TransformData
    {
        public static string TransformRetCode(string sourceRetCode)
        {
            switch (sourceRetCode)
            {
                case "0":
                    sourceRetCode = "200";
                    break;
                case "5":
                    sourceRetCode = "200";
                    break;
                case "10":
                    sourceRetCode = "2";
                    break;
                default:
                    break;
            }
            return sourceRetCode;
        }

        public static string TransfromTicketTemplate(string template)
        {
            if (template == null) return string.Empty;
            var tickDict = new Dictionary<string, string>();
            var tempArr = template.Trim().Split(new char[] { '|' });
            if (tempArr.Length != 0)
            {
                foreach (var item in tempArr)
                {
                    var tempItemArr = item.Trim().Split(new char[] { '=' });
                    if (tempItemArr.Length == 2 && !tickDict.Keys.Contains(tempItemArr[0].Trim()))
                    {
                        tickDict.Add(tempItemArr[0], tempItemArr[1]);
                    }
                }
            }
            //TODO:拼接，有改动在这边改
            template = GetDictValue(tickDict, "ticketNo") +
                        GetDictValue(tickDict, "buzWaitingCount") +
                        GetDictValue(tickDict, "buzCnname") +
                        GetDictValue(tickDict, "warmPrompt") +
                        GetDictValue(tickDict, "star");
            return template;
        }

        private static string GetDictValue(Dictionary<string,string> dict, string keyName) 
        {
            if (dict == null || dict.Count == 0)
            {
                return "|";
            }
            return dict.Keys.Contains(keyName) ? dict[keyName] + "|" : "|";
        }
    }
}
