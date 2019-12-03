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

        private RunAsyncCaller qcmConfig2Caller;

        public BasicconfigServiceImpl()
        {
            basicconfig2Caller = new RunAsyncCaller(Basicconfig2CallMachine);

            qcmConfig2Caller = new RunAsyncCaller(QcmConfig2CallMachine);
        }

   
        /// <summary>
        /// 基础配置-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void GetBasicconfig2JS(JObject jo)
        {
            log.DebugFormat("begin");

            try
            {
                Basicconfig2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("BasicconfigServiceImpl.GetBasicconfig2JS error", e);
            }

            log.DebugFormat("end, args: jo = {0}", jo);
        }

  
        /// <summary>
        /// 取号机配置-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void GetQcmConfig2JS(JObject jo)
        {
            log.DebugFormat("begin");

            try
            {
                QcmConfig2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("BasicconfigServiceImpl.GetBasicconfig2JS error", e);
            }

            log.DebugFormat("end, args: jo = {0}", jo);
        }
        public virtual void BasicconfigcWebServer(JObject jo) { 
        
        
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
            jo.Remove("callback");
            IcbcInfos icbcInfo = new IcbcInfos();
            jo["biom"]["head"]["qmsIp"] = BuzConfig2ICBC.LocalIP;
            icbcInfo.QmsIp = jo["biom"]["head"].Value<string>("qmsIp");
            icbcInfo.TradeCode = jo["biom"]["head"].Value<string>("tradeCode");
            icbcInfo.Content = jo.ToString();

            string dataStr = HttpClient.Post("/", icbcInfo);

            //log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                string retCodoStr = joBiom["head"].Value<string>("retCode");

                if (BuzConfig2ICBC.Success.Equals(retCodoStr))
                {
                    if (operation.Equals(PageCommand.Select))
                    {
                        SetBusinessmParam(jokeit, operation);
                    }
                    else if (operation.Equals(PageCommand.Update))
                    {
                        SetBusinessmParam(jo, operation);


                    }

                }

                jo.RemoveAll();

                jo["biom"] = joBiom;
            }
            else
            {
                BuzConfig2ICBC.Jo2Return(jo);
            }

            jo["callback"] = callback;

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 取号机配置2查、改-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void QcmConfig2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            // 获取页面操作命令
            int cmdStr = jo.Value<int>("command");
            PageCommand operation = (PageCommand)cmdStr;
            // 回调js方法
            string callback = jo.Value<string>("callback");
            jo.Remove("callback");
            IcbcInfos icbcInfo = new IcbcInfos();
            jo["biom"]["head"]["qmsIp"] = BuzConfig2ICBC.LocalIP;
            icbcInfo.QmsIp = jo["biom"]["head"].Value<string>("qmsIp");
            icbcInfo.TradeCode = jo["biom"]["head"].Value<string>("tradeCode");
            icbcInfo.Content = jo.ToString();

            string dataStr = HttpClient.Post("/", icbcInfo);

            //log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                jo.RemoveAll();

                jo["biom"] = joBiom;
            }
            else
            {
                BuzConfig2ICBC.Jo2Return(jo);
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
            log.DebugFormat("begin");

            basicconfig2Caller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 取号配置2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void QcmConfig2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            qcmConfig2Caller.BeginInvoke(jo, Callback, jo);

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
                jo = new JObject();
                BuzConfig2ICBC.Jo5Return(jo);

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
