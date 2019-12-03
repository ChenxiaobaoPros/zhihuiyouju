﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Infrastructure.ICBC
{
    /// <summary>
    /// 消息封装类 工行业务
    /// </summary>
    public class MessagePackage2ICBC
    {

        public static IcbcInfos SendMessage(string cmdStr)
        {
            IcbcInfos icbcInf = new IcbcInfos();
            icbcInf.QmsIp = "127.0.0.1";

            switch (cmdStr)
            {
                case "qmssign":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{ \"biom\": { \"head\": {  	\"TradeCode\": \"Qmssign\", 	\"qmsIp\": \"192.168.43.133\" 	}, \"body\": {  } } }";
                    break;
                case "custgetseq":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"custgetseq\",\"qmsIp\":\"192.168.43.133\",\"channel\":\"测试取号\"},\"body\":{}}}";
                    break;
                case "brnoconds":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"brnoconds\",\"qmsIp\":\"192.168.43.133\"},\"body\":{}}}";
                    break;
                case "kazhejudge":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"kazhejudge\",\"qmsIp\":\"192.168.43.133\"},\"body\":{\"cardFlag\":\"介质类型：1-磁条卡/折、6-IC卡、3-身份证\",\"secondTrack\":\"二磁信息\",\"thirdTrack\":\"三磁信息\",\"certType\":\"证件类型\",\"certNo\":\"证件号码\",\"custName\":\"客户姓名\",\"secgs\":\"二次取号标志\",\"custTime\":\"客户时间\",\"nation\":\"民族\",\"office\":\"发证机关\",\"signDate\":\"证件签发日期\",\"indate\":\"证件有效期\",\"addr\":\"户籍地址\",\"sex\":\"性别\",\"birthday\":\"出生日期\",\"image\":\"证件影像\"}}}";
                    break;
                case "reservation":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"reservation\",\"channel\":\"渠道\",\"qmsIp\":\"取号机IP\"},\"body\":{\"cardFlag\":\"介质类型：1-磁条卡/折、6-IC卡、3-身份证、4-手机号码\",\"PhoneNo\":\"手机号码\",\"secondTrack\":\"二磁信息\",\"thirdTrack\":\"三磁信息\",\"certType\":\"证件类型\",\"certNo\":\"证件号码\",\"custName\":\"客户姓名\",\"custTime\":\"客户时间\",\"nation\":\"民族\",\"office\":\"发证机关\",\"signDate\":\"证件签发日期\",\"indate\":\"证件有效期\",\"addr\":\"户籍地址\",\"sex\":\"性别\",\"birthday\":\"出生日期\",\"image\":\"证件影像\"}}}";
                    break;
                case "heartbeat":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"heartbeat\",\"qmsIp\":\"取号机IP\"},\"body\":{}}}";
                    break;
                case "localcardbin2delete":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"localcardbin2delete\",\"qmsIp\":\"排队机IP\"},\"body\":{\"cardBin\":\"卡bin-01\",\"servelevel\":\"服务星级\"}}}";
                    break;
                case "localcardbin2select":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"localcardbin2select\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;
                case "localcardbin2update":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"localcardbin2update\",\"qmsIp\":\"排队机IP\"},\"body\":{\"cardBin\":\"卡bin-01\",\"servelevel\":\"服务星级\"}}}";
                    break;
                case "localcardbin2add":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"localcardbin2add\",\"qmsIp\":\"排队机IP\"},\"body\":{\"cardBin\":\"卡bin-01\",\"servelevel\":\"服务星级\"}}}";
                    break;
                case "localcardtype2delete":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"localcardtype2delete\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;
                case "localcardtype2select":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"localcardtype2select\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;
                case "localcardtype2update":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"localcardbin2update\",\"qmsIp\":\"排队机IP\"},\"body\":{\"cardBin\":\"卡bin-01\",\"servelevel\":\"服务星级\"}}}";
                    break;
                case "localcardtype2add":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"localcardbin2add\",\"qmsIp\":\"排队机IP\"},\"body\":{\"cardBin\":\"卡bin-01\",\"servelevel\":\"服务星级\"}}}";
                    break;
                case "basicconfig2select":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"basicconfig2select\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;
                case "basicconfig2update":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"basicconfig2select\",\"qmsIp\":\"排队机IP\"},\"body\":{\"siteId\":\"网点代码\",\"zoneNo\":\"地区编号\",\"siteName\":\"网点名称\",\"getAutoShutdownFlag\":\"取号是否自动关机\",\"callAutoShutdownFlag\":\"叫号是否自动关机\",\"getShutdownTime\":\"取号机关机时间\",\"callShutdownTime\":\"叫号机关机时间\",\"shutdownPwd\":\"关机密码\",\"exitGetTicketPwd\":\"取号界面退出密码\",\"onlineSwitchPwd\":\"联机脱机切换密码\",\"dutySwitchPwd\":\"值守/非值守模式切换密码\"}}}";
                    break;

                case "icbcspecialuse2select":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"icbcspecialuse2select\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;
                case "icbcspecialuse2update":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"icbcspecialuse2update\",\"qmsIp\":\"排队机IP\"},\"body\":{\"biomUrl\":\"工商银行网点运营管理平台的地址\",\"biomTimeOut\":\"BIOM连接超时，单位毫秒\",\"biomTimeSpan\":\"BIOM联网失败间隔，单位秒\",\"autoOnlieSwitchTimeSpan\":\"守护线程间隔时间（自动脱机转联机），单位秒\",\"synOtherTicketFlag\":\"同步其他端机取号数据\",\"synTimeSpan\":\"同步时间间隔，单位秒\",\"disableOfflineGetTicketFlag\":\"禁止脱机取号\",\"monitorFlag\":\"启用监控\",\"monitorIp\":\"监控地址\",\"monitorTimeSpan\":\"监控时间间隔，单位秒\",\"monitorPort\":\"监控端口\",\"monitorDNS1\":\"DNS1\",\"monitorDNS2\":\"DNS2\"}}}";
                    break;

                case "ticketsconfig2select":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"ticketsconfig2select\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;
                case "ticketsconfig2update":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"ticketsconfig2update\",\"qmsIp\":\"排队机IP\"},\"body\":{\"tickePrintFormat\":\"号票打印格式\"}}}";
                    break;
                case "updateconfig2select":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"updateconfig2select\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;
                case "updateconfig2update":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"updateconfig2update\"},\"body\":{\"qmsIp\":\"排队机IP\",\"updateFlag\":\"是否启用更新(0否 1是)\",\"httpIP\":\"http服务器IP地址\",\"httpPort\":\"http服务器端口\",\"localPath\":\"本地存储地址\",\"backupPath\":\"备份文件地址\"}}}";
                    break;

                case "voiceconfig2select":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"voiceconfig2select\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;
                case "voiceconfig2update":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"voiceconfig2update\",\"qmsIp\":\"排队机IP\"},\"body\":{\"soundSpeakTimes\":\"语音呼叫次数\",\"useSameLanSpeakFlag\":\"所有业务使用相同语言播放（0否1是）\",\"speakLanguage\":\"播放语言，0-中文 1-英文 2-粤语，多个用“|”拼接\",\"speakSpecificWinFlag\":\"是否只播放指定窗口语音（0否1是）\",\"specificWin\":\"指定窗口多个窗口用“|”拼接\"}}}";
                    break;
                case "about":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"about\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;
                case "businesshall2delete":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"businesshall2delete\",\"qmsIp\":\"排队机IP\"},\"body\":{\"districtId\":\"分区编号\",\"districtName\":\"分区名称\"}}}";
                    break;
                case "businesshall2select":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"businesshall2select\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;
                case "businesshall2update":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"businesshall2update\",\"qmsIp\":\"排队机IP\"},\"body\":{\"districtId\":\"分区编号\",\"districtName\":\"分区名称\"}}}";
                    break;
                case "businesshall2add":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"businesshall2add\",\"qmsIp\":\"排队机IP\"},\"body\":{\"districtId\":\"分区编号\",\"districtName\":\"分区名称\"}}}";
                    break;
                case "counterconfig2delete":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"counterconfig2delete\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;
                case "counterconfig2select":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"counterconfig2select\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;
                case "counterconfig2update":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"counterconfig2update\",\"qmsIp\":\"排队机IP\"},\"body\":{\"winId\":\"窗口ID\",\"zoneNo\":\"分区编号\",\"winViewNum\":\"显示为\",\"winCode\":\"窗口编号\",\"winIp\":\"窗口IP\",\"winName\":\"窗口名称\",\"winPreTac\":\"窗口优先队列叫号策略（显示用，无法修改）\",\"winTac\":\"窗口普通队列叫号策略（显示用，无法修改）\",\"winLatTac\":\"窗口延后队列叫号策略（显示用，无法修改）\"}}}";
                    break;
                case "counterconfig2add":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"businesshall2add\",\"qmsIp\":\"排队机IP\"},\"body\":{\"districtId\":\"分区编号\",\"districtName\":\"分区名称\"}}}";
                    break;
                case "servicequery":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"servicequery\",\"qmsIp\":\"排队机IP\"},\"body\":{}}}";
                    break;

                case "queuequery":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"queuequery\",\"qmsIp\":\"排队机IP\"},\"body\":{\"queueNo\":\"队列编码\",\"busTypeNo\":\"业务类型编号\",\"serviceStar\":\"服务星级\"}}}";
                    break;
                case "logquery":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"logquery\",\"qmsIp\":\"取号机IP\"},\"body\":{\"logName\":\"模块日志名称\",\"downPath\":\"下载路径\"}}}";
                    break;


                case "LocalNumberQuery":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"LocalNumberQuery\",\"qmsIp\":\"取号机IP\"},\"body\":{}}}";
                    break;

                case "DeleteabolishNumber":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"DeleteabolishNumber\",\"qmsIp\":\"排队机IP\"},\"body\":{\"queCustNo\":\"客户号码\"}}}";
                    break;
                case "WindowQueueAdjust":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"WindowQueueAdjust\",\"qmsIp\":\"排队机IP\"},\"body\":{\"branchId\":\"机构编码\",\"zoneNo\":\"地区号\",\"brNo\":\"网点号\",\"oldWinNo\":\"原窗口编号\",\"newWinNo\":\"新窗口编号\",\"qmsNo\":\"排队机编号\",\"queueNo\":\"队列编号\",\"locate\":\"队列归属\",\"queueOrd\":\"队列顺序号\",\"num\":\"超时时间/连续叫号个数\",\"lstModiUser\":\"最后修改人\"}}}";
                    break;
                case "QueueAttributesUpdate":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"QueueAttributesUpdate\"},\"body\":{\"lstModUser\":\"最后修改人\",\"oldWinNo\":\"原窗口编号\",\"newWinNo\":\"新窗口编号\",\"oldQueueNo\":\"原队列编号\",\"newQueueNo\":\"新队列编号\",\"oldAttrList\":{\"detail\":[{\"busiTypeNo\":\"原队列属性业务类型编号\",\"serveLevel\":\"原队列属性星级\"},{\"busiTypeNo\":\"原队列属性业务类型编号\",\"serveLevel\":\"原队列属性星级\"}],\"newAttrList\":{\"detail\":[{\"busiTypeNo\":\"新队列属性业务类型编号\",\"serveLevel\":\"新队列属性星级\"},{\"busiTypeNo\":\"新队列属性业务类型编号\",\"serveLevel\":\"新队列属性星级\"}]}}}}}";
                    break;
                case "linkageshutdown":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"linkageshutdown\",\"qmsIp\":\"取号机IP\"},\"body\":{\"delayedCloseTime\":\"延后关机时间 秒\"}}}";
                    break;
                case "stateUpdate":
                    icbcInf.TradeCode = cmdStr;
                    icbcInf.Content = "{\"biom\":{\"head\":{\"tradeCode\":\"stateUpdate\",\"qmsIp\":\"排队机IP\"},\"body\":{\"status\":{\"card\":\"磁条卡读卡器状态值\",\"rfcard\":\"接触式和非接触式合一芯片卡读卡器状态值\",\"miniprint\":\"凭条打印机状态值\",\"idcard\":\"二代身份证读卡器状态值\",\"TrendMicro\":\"趋势状态\",\"DSMClient\":\"DSM状态\"}}}}";
                    break;



            }




            return icbcInf;
        }



    }
}
