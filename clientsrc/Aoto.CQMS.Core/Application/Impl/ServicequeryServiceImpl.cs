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
    /// 业务查询业务类
    /// </summary>
    public class ServicequeryServiceImpl : IServicequeryService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller servicequeryCaller;

        public ServicequeryServiceImpl()
        {
            servicequeryCaller = new RunAsyncCaller(Servicequery2CallMachine);
        }
        /// <summary>
        /// 业务查询-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Servicequery2JS(JObject jo)
        {
            log.DebugFormat("begin", jo);
            try
            {
                Servicequery2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("ServicequeryServiceImpl.Servicequery2JS error", e);
            }
            log.DebugFormat("end", jo);
        }

        /// <summary>
        /// 业务查询-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Servicequery2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            // 获取页面操作命令
            //int cmdStr = jo.Value<int>("command");

            //PageCommand operation = (PageCommand)cmdStr;
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
        /// 业务查询-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Servicequery2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            servicequeryCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }
        /// <summary>
        /// 设置取号机业务参数
        /// </summary>
        /// <param name="jo"></param>
        private void SetBusinessmParam(JObject jo, PageCommand operation)
        {
            if (operation.Equals(PageCommand.Select))
            {
                JToken joBody = jo["biom"]["body"];

                //lock (BuzConfig2ICBC.staticLook)
                //{                        
                    //是否在排号机显示
                    BuzConfig2ICBC.QmsFlag = joBody.Value<string>("qmsFlag");             
                    //非值守菜单
                    BuzConfig2ICBC.IsDuty = joBody.Value<string>("isDuty");
                                //}

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

                log.Error("ServicequeryServiceImpl.Callback Error", e);
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
