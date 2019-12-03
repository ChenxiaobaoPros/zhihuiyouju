using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Newtonsoft.Json.Linq;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ICBC;
using System.Runtime.Remoting.Messaging;
using Aoto.PPS.Infrastructure.Utils;

namespace Aoto.CQMS.Core.Application.Impl
{
    /// <summary>
    /// 取号业务类
    /// </summary>
    public class CustgetseqServiceImpl:ICustgetseqService
    {

        private static ILog log = LogManager.GetLogger("app");

         protected IScriptInvoker scriptInvoker;

         private RunAsyncCaller custgetseqCaller;

         private RunAsyncCaller kazhejudgeCaller;

         private RunAsyncCaller offLineSwichCaller;

         public CustgetseqServiceImpl()
        {
            custgetseqCaller = new RunAsyncCaller(Custgetseq2CallMachine);

            kazhejudgeCaller = new RunAsyncCaller(Kazhejudge2CallMachine);

            offLineSwichCaller = new RunAsyncCaller(OffLineSwich2CallMachine);
            
        }

        /// <summary>
        /// 取号-页面
        /// </summary>
        /// <param name="jo"></param>
         public virtual void Custgetseq2JS(JObject jo)
        {
            log.DebugFormat("begin");

            try
            {
                Custgetseq2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("CustgetseqServiceImpl.Custgetseq2JS error", e);
            }

            log.DebugFormat("end");
        }

         /// <summary>
         /// 刷卡折-页面
         /// </summary>
         /// <param name="jo"></param>
         public virtual void Kazhejudge2JS(JObject jo)
         {
             log.DebugFormat("begin");
             try
             {
                 Kazhejudge2CallMachineAsync(jo);
             }
             catch (Exception e)
             {
                 log.Error("CustgetseqServiceImpl.Kazhejudge2JS error", e);
             }
             log.DebugFormat("end");
         }

        /// <summary>
        /// 日志加密
        /// </summary>
        /// <param name="jo"></param>
        /// <returns></returns>
         public JObject LogEncryption(JObject jo)
         {
             log.DebugFormat("begin");

             JObject joket = new JObject();
             try
             {
                 joket["biom"]=jo["biom"];

                 string cardFlag = joket["biom"]["body"].Value<string>("cardFlag");
                
                 switch (cardFlag)
                 {
                     case "3":  // 身份证
                          string custName = joket["biom"]["body"].Value<string>("custName") == null ? String.Empty : joket["biom"]["body"].Value<string>("custName");
                 string certNo = joket["biom"]["body"].Value<string>("certNo") == null ? String.Empty : joket["biom"]["body"].Value<string>("certNo");
                         joket["biom"]["body"]["addr"] = GlobalVariable2ICBC.REPLACE_LOG_STR_01;
                         joket["biom"]["body"]["birthday"] = GlobalVariable2ICBC.REPLACE_LOG_STR_01;
                         if (custName.Length > 0)
                         {
                             joket["biom"]["body"]["custName"] = custName.Substring(0, custName.Length - 1) + "*";
                         }
                         if (certNo.Length >= 15)
                         {
                             joket["biom"]["body"]["certNo"] = certNo.Substring(0, certNo.Length - 5) + "****" + certNo.Substring(certNo.Length - 1, 1);
                         }
                         break;
                     case "1":
                     case "6":
                         string secondTrack = joket["biom"]["body"].Value<string>("secondTrack") == null ? String.Empty : joket["biom"]["body"].Value<string>("secondTrack");
                         string thirdTrack = joket["biom"]["body"].Value<string>("thirdTrack") == null ? String.Empty : joket["biom"]["body"].Value<string>("thirdTrack");


                         if (secondTrack.Contains("="))
                         {
                             secondTrack = secondTrack.Substring(0, secondTrack.IndexOf('=')+1);

                             if (secondTrack.Length > 5)
                             {
                                 joket["biom"]["body"]["secondTrack"] = secondTrack.Substring(0, secondTrack.Length - 5) + "****" + secondTrack.Substring(secondTrack.Length - 1, 1) + GlobalVariable2ICBC.REPLACE_LOG_STR_01; ;
                             }

                         }
                         if (thirdTrack.Contains("="))
                         {
                             thirdTrack = thirdTrack.Substring(0, thirdTrack.IndexOf('=')+1);

                             if (thirdTrack.Length > 5)
                             {
                                 joket["biom"]["body"]["thirdTrack"] = thirdTrack.Substring(0, thirdTrack.Length - 5) + "****" + thirdTrack.Substring(thirdTrack.Length - 1, 1) + GlobalVariable2ICBC.REPLACE_LOG_STR_01; ;
                             }

                         }

                         break;
                     default:

                         break;


                 }

             }
             catch (Exception e)
             {
                 log.ErrorFormat("日志加密外设信息异常："+e);
             }

             log.DebugFormat("end");

             return joket;
         }


        /// <summary>
        /// 取号-叫号终端
        /// </summary>
        /// <param name="jo"></param>
         public virtual void Custgetseq2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", LogEncryption(jo));
            // 获取页面操作命令
            //int cmdStr = jo.Value<int>("command");
            //PageCommand operation = (PageCommand)cmdStr;
            // 回调js方法

            //String op = "{\"biom\":{\"head\":{\"tradeCode\":\"custgetseq\",\"qmsIp\":\"122.138.45.223\",\"channel\":\"1\"},\"body\":{\"busiType1\":\"1\",\"busiType2\":\"236\",\"cardFlag\":\"1\",\"secondTrack\":\";6222081602003068318=23052203429991847?\",\"thirdTrack\":\";996222081602003068318=1561560000000000001003342999010000023050=000000000000=000000000000=00000000?\",\"certType\":\"\",\"certNo\":\"\",\"custName\":\"\",\"secgs\":\"\",\"custTime\":\"\",\"nation\":\"\",\"office\":\"\",\"signDate\":\"\",\"indate\":\"\",\"addr\":\"\",\"sex\":\"\",\"birthday\":\"\",\"image\":\"\"}}}";
            string cardFlag = jo["biom"]["body"].Value<string>("cardFlag");
             string callback = jo.Value<string>("callback");
            jo.Remove("callback");
            //JObject joket = JObject.Parse(op);

            // jo["biom"]=joket["biom"];
            
            IcbcInfos icbcInfo = new IcbcInfos();
            jo["biom"]["head"]["qmsIp"] = BuzConfig2ICBC.LocalIP;
            icbcInfo.QmsIp = jo["biom"]["head"].Value<string>("qmsIp");
            icbcInfo.TradeCode = jo["biom"]["head"].Value<string>("tradeCode");
            icbcInfo.Content = jo.ToString();

            string dataStr = HttpClient.Post("/", icbcInfo);

            //log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);

            jo.RemoveAll();

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {

                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                jo["biom"] = joBiom;
            }
            else
            {
                JObject jokeit = JObject.Parse("{\"biom\":{\"head\":{\"retCode\":\"1\",\"retMsg\":\"\"},\"body\":{\"cardFlag\":\"\"}}}");

                jokeit["biom"]["head"]["retMsg"] = PromptInfos2ICBC.ICBC_MESS_QCMEXT01;
                jokeit["biom"]["body"]["cardFlag"] = cardFlag;

                jo["biom"]=jokeit["biom"];

            }

            jo["callback"] = callback;


            log.DebugFormat("end, args: jo = {0}", LogEncryption(jo));
        }

       
         /// <summary>
         /// 刷卡折/身份证-叫号终端 异步
         /// </summary>
         /// <param name="jo"></param>
         public virtual void Custgetseq2CallMachineAsync(JObject jo)
         {
             log.DebugFormat("begin");
         
             custgetseqCaller.BeginInvoke(jo, Callback, jo);

             log.Debug("end");
         }

         /// <summary>
         /// 刷卡折-叫号终端
         /// </summary>
         /// <param name="jo"></param>
         public virtual void Kazhejudge2CallMachine(JObject jo)
         {
             log.DebugFormat("begin, args: jo = {0}", LogEncryption(jo));

             // 获取页面操作命令
             //int cmdStr = jo.Value<int>("command");
             //PageCommand operation = (PageCommand)cmdStr;
             // 回调js方法
             string cardFlag = jo["biom"]["body"].Value<string>("cardFlag");
             string callback = "qmsSwipeCardCallback";//jo.Value<string>("callback");
             jo.Remove("callback");
             IcbcInfos icbcInfo = new IcbcInfos();
             jo["biom"]["head"]["qmsIp"] = BuzConfig2ICBC.LocalIP;
             icbcInfo.QmsIp = jo["biom"]["head"].Value<string>("qmsIp");
             icbcInfo.TradeCode = jo["biom"]["head"].Value<string>("tradeCode");
             icbcInfo.Content = jo.ToString();

             string dataStr = HttpClient.Post("/", icbcInfo);

             //log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);

             jo.RemoveAll();

             if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
             {

                 JObject jokeit = JObject.Parse(dataStr);

                 JToken joBiom = jokeit["biom"];

                 jo["biom"] = joBiom;
             }
             else
             {
                 //BuzConfig2ICBC.Jo2Return(jo);
                 JObject jokeit = JObject.Parse("{\"biom\":{\"head\":{\"retCode\":\"1\",\"retMsg\":\"\"},\"body\":{\"cardFlag\":\"\"}}}");

                 jokeit["biom"]["head"]["retMsg"] = PromptInfos2ICBC.ICBC_MESS_QCMEXT01;
                 jokeit["biom"]["body"]["cardFlag"] = cardFlag;

                 jo["biom"] = jokeit["biom"];
             }

             jo["callback"] = callback;


             log.DebugFormat("end, args: jo = {0}", LogEncryption(jo));
         }

         /// <summary>
         /// 刷卡折-叫号终端 异步
         /// </summary>
         /// <param name="jo"></param>
         public virtual void Kazhejudge2CallMachineAsync(JObject jo)
         {
             log.DebugFormat("begin");

             kazhejudgeCaller.BeginInvoke(jo, Callback, jo);

             log.Debug("end");
         }

         /// <summary>
         /// 联脱机切换-叫号终端
         /// </summary>
         /// <param name="jo"></param>
         public virtual void OffLineSwich2CallMachine(JObject jo)
         {
             log.DebugFormat("begin, args: jo = {0}", jo);

             string offLineStatus = jo["biom"]["body"].Value<string>("offLineStatus");
             // 回调js方法
             string callback = "systemCommondCallback";//jo.Value<string>("callback");
             jo.Remove("callback");
             IcbcInfos icbcInfo = new IcbcInfos();
             jo["biom"]["head"]["qmsIp"] = BuzConfig2ICBC.LocalIP;
             icbcInfo.QmsIp = jo["biom"]["head"].Value<string>("qmsIp");
             icbcInfo.TradeCode = jo["biom"]["head"].Value<string>("tradeCode");
             icbcInfo.Content = jo.ToString();

             string dataStr = HttpClient.Post("/", icbcInfo);

             //log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);

             jo.RemoveAll();

             if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
             {

                 JObject jokeit = JObject.Parse(dataStr);

                 JToken joBiom = jokeit["biom"];

                 jo["biom"] = joBiom;

                 string retCode = jo["biom"]["head"].Value<string>("retCode");

                 if (retCode.Equals("0"))
                 {
                     jo["biom"]["head"]["retCode"] = "27";
                     jo["biom"]["body"]["offLineStatus"] = offLineStatus;
                 }

             }
             else
             {
                 BuzConfig2ICBC.Jo6Return(jo);

             }

             jo["callback"] = callback;

             if (null == scriptInvoker)
             {
                 scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
             }

             scriptInvoker.ScriptInvoke(jo);


             log.DebugFormat("end, args: jo = {0}", jo);
         }

         /// <summary>
         /// 联脱机切换-叫号终端 异步
         /// </summary>
         /// <param name="jo"></param>
         public virtual void OffLineSwich2CallMachineAsync(JObject jo)
         {
             log.DebugFormat("begin");

             offLineSwichCaller.BeginInvoke(jo, Callback, jo);

             log.Debug("end");
         }

        /// <summary>
        /// 设置取号机业务参数
        /// </summary>
        /// <param name="jo"></param>
         private void SetBusinessmParam(JObject jo,PageCommand operation)
         {       JToken joBody = jo["biom"]["body"];
             if (operation.Equals(PageCommand.Kazhejudge))
             {
                 if (BuzConfig2ICBC.PrintTemp.Equals(String.Empty))
                 {
                     BuzConfig2ICBC.PrintTemp = joBody.Value<string>("printtemp");
                 }
           }

         }

         private void Callback(IAsyncResult ar)
         {
             JObject jo = (JObject)ar.AsyncState;
             //string callback = String.Empty;
             try
             {
                 //callback = jo.Value<string>("callback");

                 ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
             }
             catch (Exception e)
             {
                 jo = new JObject();
                 BuzConfig2ICBC.Jo5Return(jo);
                 //jo["callback"] = callback;
                 log.Error("CustgetseqServiceImpl.Callback Error", e);
             }
             finally
             {
                 if (null == scriptInvoker)
                 {
                     scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
                 }

                 scriptInvoker.ScriptInvoke(jo);
             }
         }

    }
}
