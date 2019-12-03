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
using System.Runtime.Remoting.Messaging;
using Aoto.PPS.Infrastructure.Utils;

namespace Aoto.CQMS.Core.Application.Impl
{
    /// <summary>
    /// 关于业务类
    /// </summary>
    public class AboutServiceImpl:IAboutService
    {
        private static ILog log = LogManager.GetLogger("app");
        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller about2CallMachineCaller;

        public AboutServiceImpl()
        {
            about2CallMachineCaller = new RunAsyncCaller(About2CallMachine);
        }

        /// <summary>
        /// 关于配置-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void About2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            if (BuzConfig2ICBC.About2JsonStr.Equals(String.Empty))
            {
                About2CallMachineAsync(jo);
            }
            else
            {
                // 组装json格式
                JObject jokeit = JObject.Parse(BuzConfig2ICBC.About2JsonStr);

                JToken joBiom = jokeit["biom"];

                jo["biom"] = joBiom;

                jo["result"] = ErrorCode.Success;
            }

           
            
            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 关于配置-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void About2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_ABOUT));

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                jo["result"] = ErrorCode.Success;

                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                jo["biom"] = joBiom;

                SetAboutParam(jo);
            } 

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 关于-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void About2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            about2CallMachineCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }
          /// <summary>
        /// 设置取号机业务参数
        /// </summary>
        /// <param name="jo"></param>
        private void SetAboutParam(JObject jo)
        {
            if (jo.Value<int>("result") == ErrorCode.Success)
            {
                // 叫号机返回消息成功,取号终端业务处理

                BuzConfig2ICBC.About2JsonStr = jo.ToString();
               
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
