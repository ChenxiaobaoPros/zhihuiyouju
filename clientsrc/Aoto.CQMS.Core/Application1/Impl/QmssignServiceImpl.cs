using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Newtonsoft.Json.Linq;
using Aoto.PPS.Infrastructure.ICBC;
using Aoto.PPS.Infrastructure.Configuration;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Utils;
using System.Runtime.Remoting.Messaging;

namespace Aoto.CQMS.Core.Application.Impl
{
    /// <summary>
    /// 签到业务类
    /// </summary>
    public class QmssignServiceImpl:IQmssignService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller qmssignCaller;

        public QmssignServiceImpl()
        {

            qmssignCaller = new RunAsyncCaller(Qmssign2CallMachine);
        }

        /// <summary>
        /// 签到-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void GetQmssign2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            Qmssign2CallMachineAsync(jo);

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 签到-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Qmssign2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_QMSSIGN));

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
        /// 签到-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Qmssign2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            qmssignCaller.BeginInvoke(jo, Callback, jo);

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
