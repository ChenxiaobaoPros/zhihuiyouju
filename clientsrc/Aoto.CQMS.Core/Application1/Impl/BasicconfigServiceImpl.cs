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
    /// 基础配置业务类
    /// </summary>
    public class BasicconfigServiceImpl:IBasicconfigService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller basicconfig2Caller;

        public BasicconfigServiceImpl()
        {
            basicconfig2Caller = new RunAsyncCaller(Basicconfig2CallMachine); 
        }

        /// <summary>
        /// 基础配置-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void GetBasicconfig2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            Basicconfig2CallMachineAsync(jo);

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 基础配置2查、改-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Basicconfig2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            // 获取页面操作命令
            int cmdStr = jo.Value<int>("command");
            PageCommand operation = (PageCommand)cmdStr;
            // 回调js方法
            string callback = jo.Value<string>("callback");

            IcbcInfos icbcInfo = new IcbcInfos();
            icbcInfo.QmsIp = jo["biom"]["head"].Value<string>("qmsIp");
            icbcInfo.TradeCode = jo["biom"]["head"].Value<string>("tradeCode");
            icbcInfo.Content = jo.ToString();

            string dataStr = HttpClient.Post("/", icbcInfo);

            jo.RemoveAll();

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                SetBusinessmParam(jokeit, operation);

                jo["biom"] = joBiom;
            }
            else
            {           
                JObject jokeit = JObject.Parse("{ \"biom\": { \"head\": { \"retCode\": \"" + 1 + "\", \"retMsg\": \"" + PromptInfos2ICBC.ICBC_MESS_QCMEXT01 + "\" }, \"body\": {  }	 } }");

                JToken joBiom = jokeit["biom"];

                jo["biom"] = joBiom;

            }
            jo["callback"] = callback;


            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 基础配置2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Basicconfig2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            basicconfig2Caller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }


        /// <summary>
        /// 基础设置-取号机业务参数赋值
        /// </summary>
        /// <param name="jo"></param>
        private void SetBusinessmParam(JObject jo, PageCommand operation)
        {
            JToken joBody = jo["biom"]["body"];

            if (operation.Equals(PageCommand.Select))
            {
                if (BuzConfig2ICBC.ShutdownPwd.Equals(String.Empty))
                {
                    BuzConfig2ICBC.ShutdownPwd = joBody.Value<string>("shutdownPwd");
                }
                if (BuzConfig2ICBC.ExitGetTicketPwd.Equals(String.Empty))
                {
                    BuzConfig2ICBC.ExitGetTicketPwd = joBody.Value<string>("exitGetTicketPwd");
                }
                if (BuzConfig2ICBC.OnlineSwitchPwd.Equals(String.Empty))
                {
                    BuzConfig2ICBC.OnlineSwitchPwd = joBody.Value<string>("onlineSwitchPwd");
                }
                if (BuzConfig2ICBC.DutySwitchPwd.Equals(String.Empty))
                {
                    BuzConfig2ICBC.DutySwitchPwd = joBody.Value<string>("dutySwitchPwd");
                }
                if (BuzConfig2ICBC.GetAutoShutdownFlag.Equals(String.Empty))
                {
                    BuzConfig2ICBC.GetAutoShutdownFlag = joBody.Value<string>("getAutoShutdownFlag");
                }
                if (BuzConfig2ICBC.GetShutdownTime.Equals(String.Empty))
                {
                    BuzConfig2ICBC.GetShutdownTime = joBody.Value<string>("getShutdownTime");
                }
            }
            else
            {
                BuzConfig2ICBC.ShutdownPwd = joBody.Value<string>("shutdownPwd");
                BuzConfig2ICBC.ExitGetTicketPwd = joBody.Value<string>("exitGetTicketPwd");
                BuzConfig2ICBC.OnlineSwitchPwd = joBody.Value<string>("onlineSwitchPwd");
                BuzConfig2ICBC.DutySwitchPwd = joBody.Value<string>("dutySwitchPwd");

                BuzConfig2ICBC.GetAutoShutdownFlag = joBody.Value<string>("getAutoShutdownFlag");
                BuzConfig2ICBC.GetShutdownTime = joBody.Value<string>("getShutdownTime");

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
