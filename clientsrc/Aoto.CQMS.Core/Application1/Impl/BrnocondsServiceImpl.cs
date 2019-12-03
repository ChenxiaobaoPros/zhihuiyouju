using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Aoto.PPS.Infrastructure.ComponentModel;
using Newtonsoft.Json.Linq;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ICBC;
using Aoto.PPS.Infrastructure.Configuration;
using System.Runtime.Remoting.Messaging;
using Aoto.PPS.Infrastructure.Utils;

namespace Aoto.CQMS.Core.Application.Impl
{
    /// <summary>
    /// 其他网点信息业务类
    /// </summary>
    public class BrnocondsServiceImpl:IBrnocondsService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller brnocondsCaller;

        public BrnocondsServiceImpl()
        {
            brnocondsCaller = new RunAsyncCaller(Brnoconds2CallMachine);
        }

        /// <summary>
        /// 其他网点信息-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Brnoconds2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            Brnoconds2CallMachine(jo);

            jo["result"] = ErrorCode.Success;


            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 其他网点信息-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Brnoconds2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_BRNOCONDS));

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
        /// 其他网点信息-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Brnoconds2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            brnocondsCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        private void Callback(IAsyncResult ar)
        {
            JObject jo = (JObject)ar.AsyncState;

            if (jo.Value<int>("result") == ErrorCode.Success)
            {
                // 叫号机返回消息成功

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
