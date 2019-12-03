﻿using System;
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
    /// 联动关机业务类
    /// </summary>
    public class LinkageshutdownServiceImpl : ILinkageshutdownService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller linkageshutdownCaller;

        public LinkageshutdownServiceImpl()
        {
            linkageshutdownCaller = new RunAsyncCaller(Linkageshutdown2CallMachineBy);
        }

        /// <summary>
        /// 联动关机-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Linkageshutdown2JS(JObject jo)
        {
            log.DebugFormat("begin");
            try
            {
                Linkageshutdown2CallMachineByAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("LinkageshutdownServiceImpl.Linkageshutdown2JS error", e);
            }
            log.DebugFormat("end");
        }

        /// <summary>
        /// 联动关机-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Linkageshutdown2CallMachineBy(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);
            // 获取页面操作命令

            //int cmdStr = jo.Value<int>("command");

            //PageCommand operation = (PageCommand)cmdStr;
            // 回调js方法

            string callback = "systemCommond25Callback";//jo.Value<string>("callback");
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
        /// 基础配置2改-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Linkageshutdown2CallMachineByAsync(JObject jo)
        {
            log.DebugFormat("begin");

            linkageshutdownCaller.BeginInvoke(jo, Callback, jo);

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

                log.Error("IcbcspecialuseServiceImpl.Callback Error", e);
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
