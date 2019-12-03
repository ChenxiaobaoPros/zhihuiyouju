using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Newtonsoft.Json.Linq;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ICBC;
using Aoto.PPS.Infrastructure.Configuration;
using Aoto.PPS.Infrastructure.Utils;
using System.Runtime.Remoting.Messaging;

namespace Aoto.CQMS.Core.Application.Impl
{
    /// <summary>
    /// 队列查询业务类
    /// </summary>
    public class QueuequeryServiceImpl:IQueuequeryService
    {
        private static ILog log = LogManager.GetLogger("app");
        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller queueAttributesUpdateCaller;
        private RunAsyncCaller queuequeryCaller;

        public QueuequeryServiceImpl()
        {
            queueAttributesUpdateCaller = new RunAsyncCaller(QueueAttributesUpdate2CallMachine);
            queuequeryCaller = new RunAsyncCaller(Queuequery2CallMachine);
        }

        /// <summary>
        /// 队列查询-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Queuequery2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string cmdStr = jo.Value<string>("command");

            switch (cmdStr)
            {
                case "select":
                    Queuequery2CallMachineAsync(jo);
                    break;
                case "update":
                    QueueAttributesUpdate2CallMachineAsync(jo);
                    break;
                default:

                    break;

            }

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 队列查询-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Queuequery2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_QUEUEQUERY));

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                jo["result"] = ErrorCode.Success;

                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                jo["biom"] = joBiom;
            }

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 基础配置2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Queuequery2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            queuequeryCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }
        /// <summary>
        /// 队列属性更改-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void QueueAttributesUpdate2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_QUEUEATTRIBUTESUPDATE));

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                jo["result"] = ErrorCode.Success;

                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                jo["biom"] = joBiom;
            }

            log.DebugFormat("end, args: jo = {0}", jo);
        }
        /// <summary>
        /// 基础配置2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void QueueAttributesUpdate2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            queueAttributesUpdateCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }
        private void Callback(IAsyncResult ar)
        {
            JObject jo = (JObject)ar.AsyncState;

            if (jo.Value<int>("result") == ErrorCode.Success)
            {
                // 叫号机返回消息成功,取号终端业务处理
                string cmdStr = jo.Value<string>("command");
              
                    JToken joBody = jo["biom"]["body"];

                    BuzConfig2ICBC.ShutdownPwd = joBody.Value<string>("shutdownPwd");
                    BuzConfig2ICBC.ExitGetTicketPwd = joBody.Value<string>("exitGetTicketPwd");
                    BuzConfig2ICBC.OnlineSwitchPwd = joBody.Value<string>("onlineSwitchPwd");
                    BuzConfig2ICBC.DutySwitchPwd = joBody.Value<string>("dutySwitchPwd");

                 
                
            }
            else
            {
                // 叫号机返回消息异常
                jo["retMsg"] = PromptInfos2ICBC.ICBC_MESS_QCMEXT01;
            }

            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);

            }
            catch (Exception e)
            {
                jo["result"] = ErrorCode.Failure;
                log.Error("BasicconfigServiceImpl.Callback Error", e);
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
