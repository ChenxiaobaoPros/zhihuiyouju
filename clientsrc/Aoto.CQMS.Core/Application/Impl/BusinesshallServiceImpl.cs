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
    /// 营业厅分区业务类
    /// </summary>
    public class BusinesshallServiceImpl : IBusinesshallService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller businesshallCaller;

        public BusinesshallServiceImpl()
        {
            businesshallCaller = new RunAsyncCaller(Businesshall2CallMachine);

        }

        /// <summary>
        /// 营业厅分区-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Businesshall2JS(JObject jo)
        {
            log.DebugFormat("begin");
            try
            {
                Businesshall2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("BusinesshallServiceImpl.Businesshall2JS error", e);
            }
            log.DebugFormat("end");

        }

        /// <summary>
        /// 营业厅分区2查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Businesshall2CallMachine(JObject jo)
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
        /// 营业厅分-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Businesshall2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            businesshallCaller.BeginInvoke(jo, Callback, jo);

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

                log.Error("BusinesshallServiceImpl.Callback Error", e);
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
