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
    /// 本地卡Bin业务类
    /// </summary>
    public class LocalcardbinServiceImpl : ILocalcardbinService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller localcardbinCaller;

        private RunAsyncCaller biomqmscardbinCaller;

        public LocalcardbinServiceImpl()
        {
            localcardbinCaller = new RunAsyncCaller(Localcardbin2CallMachine);

            biomqmscardbinCaller = new RunAsyncCaller(Biomqmscardbin2CallMachine);

        }

        /// <summary>
        /// 本地卡Bin-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardbin2JS(JObject jo)
        {
            log.DebugFormat("begin");
            try
            {
                Localcardbin2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("LocalcardbinServiceImpl.Localcardbin2JS error", e);
            }
            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 远程卡Bin-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Biomqmscardbin2JS(JObject jo)
        {
            log.DebugFormat("begin");
            try
            {
                Biomqmscardbin2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("LocalcardbinServiceImpl.Biomqmscardbin2JS error", e);
            }
            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 本地卡Bi-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardbin2CallMachine(JObject jo)
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

           // log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);

            jo.RemoveAll();

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

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
        /// 本地卡宾-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardbin2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            localcardbinCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 远程卡Bin-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Biomqmscardbin2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            // 回调js方法
            string callback = jo.Value<string>("callback");
            jo.Remove("callback");
            IcbcInfos icbcInfo = new IcbcInfos();
            jo["biom"]["head"]["qmsIp"] = BuzConfig2ICBC.LocalIP;
            icbcInfo.QmsIp = jo["biom"]["head"].Value<string>("qmsIp");

            icbcInfo.TradeCode = jo["biom"]["head"].Value<string>("tradeCode");

            icbcInfo.Content = jo.ToString();

            string dataStr = HttpClient.Post("/", icbcInfo);

            // log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);

            jo.RemoveAll();

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

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
        /// 远程卡Bin-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Biomqmscardbin2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            biomqmscardbinCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
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

                log.Error("LocalcardbinServiceImpl.Callback Error", e);
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
