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

         public CustgetseqServiceImpl()
        {
            custgetseqCaller = new RunAsyncCaller(Custgetseq2CallMachine);
            kazhejudgeCaller = new RunAsyncCaller(Kazhejudge2CallMachine);
        }

        /// <summary>
        /// 取号-页面
        /// </summary>
        /// <param name="jo"></param>
         public virtual void Custgetseq2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            Custgetseq2CallMachineAsync(jo);

            jo["result"] = ErrorCode.Success;
            

            log.DebugFormat("end, args: jo = {0}", jo);
        }

         /// <summary>
         /// 刷卡折-页面
         /// </summary>
         /// <param name="jo"></param>
         public virtual void Kazhejudge2JS(JObject jo)
         {
             log.DebugFormat("begin, args: jo = {0}", jo);

             jo["result"] = ErrorCode.Failure;

             Kazhejudge2CallMachineAsync(jo);

             jo["result"] = ErrorCode.Success;


             log.DebugFormat("end, args: jo = {0}", jo);
         }

        /// <summary>
        /// 取号-叫号终端
        /// </summary>
        /// <param name="jo"></param>
         public virtual void Custgetseq2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_CUSTGETSEQ));

            jo = JObject.Parse(dataStr);

            log.DebugFormat("end, args: jo = {0}", jo);
        }

         /// <summary>
         /// 取号-叫号终端 异步
         /// </summary>
         /// <param name="jo"></param>
         public virtual void Custgetseq2CallMachineAsync(JObject jo)
         {
             log.DebugFormat("begin, jo = {0}", jo);

             custgetseqCaller.BeginInvoke(jo, Callback, jo);

             log.Debug("end");
         }

         /// <summary>
         /// 刷卡折/身份证-叫号终端
         /// </summary>
         /// <param name="jo"></param>
         public virtual void Kazhejudge2CallMachine(JObject jo)
         {
             log.DebugFormat("begin, args: jo = {0}", jo);

             string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_KAZHEJUDGE));

             jo = JObject.Parse(dataStr);

             log.DebugFormat("end, args: jo = {0}", jo);
         }

         /// <summary>
         /// 刷卡折/身份证-叫号终端 异步
         /// </summary>
         /// <param name="jo"></param>
         public virtual void Kazhejudge2CallMachineAsync(JObject jo)
         {
             log.DebugFormat("begin, jo = {0}", jo);

             kazhejudgeCaller.BeginInvoke(jo, Callback, jo);

             log.Debug("end");
         }


        /// <summary>
        /// 设置取号机业务参数
        /// </summary>
        /// <param name="jo"></param>
         private void SetBusinessmParam(JObject jo)
         {
             if (jo.Value<int>("result") == ErrorCode.Success)
             {
                 // 叫号机返回消息成功

             }
             else
             {
                 // 叫号机返回消息异常
                 jo["retMsg"] = PromptInfos2ICBC.ICBC_MESS_QCMEXT01;
             }

         }

         private void Callback(IAsyncResult ar)
         {
             JObject jo = (JObject)ar.AsyncState;

             SetBusinessmParam(jo);

             try
             {
                 ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
             }
             catch (Exception e)
             {
                 jo["result"] = ErrorCode.Failure;
                 log.Error("QmssignServiceImpl.Callback Error", e);
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
