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
    /// 柜台配置业务类
    /// </summary>
    public class CounterconfigServiceImpl : ICounterconfigService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller counterconfigCaller;

        private RunAsyncCaller windowQueueCaller;


        public CounterconfigServiceImpl()
        {
            counterconfigCaller = new RunAsyncCaller(Counterconfig2CallMachine);

            windowQueueCaller = new RunAsyncCaller(WindowQueueAdjust2CallMachine);

        }

        /// <summary>
        /// 柜台配置-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Counterconfig2JS(JObject jo)
        {
            log.DebugFormat("begin");
            try
            {
                CounterconfigAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("CounterconfigServiceImpl.Counterconfig2JS error", e);
            }
            log.DebugFormat("end");
        }

        /// <summary>
        /// 窗口队列调整-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void WindowQueueAdjust2JS(JObject jo)
        {
            log.DebugFormat("begin");
            try
            {
                CounterconfigAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("CounterconfigServiceImpl.WindowQueueAdjust2JS error", e);
            }
            log.DebugFormat("end");
        }

        /// <summary>
        /// 柜台配置2查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Counterconfig2CallMachine(JObject jo)
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
        /// 窗口队列调整-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void WindowQueueAdjust2CallMachine(JObject jo)
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

        /// 柜台配置2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void CounterconfigAsync(JObject jo)
        {
            log.DebugFormat("begin");

            counterconfigCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// 窗口队列调整-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void WindowQueueAdjustAsync(JObject jo)
        {
            log.DebugFormat("begin");

            windowQueueCaller.BeginInvoke(jo, Callback, jo);

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

                log.Error("CounterconfigServiceImpl.Callback Error", e);
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
