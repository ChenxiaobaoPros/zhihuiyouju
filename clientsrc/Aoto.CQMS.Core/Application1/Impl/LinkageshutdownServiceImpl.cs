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
    public class LinkageshutdownServiceImpl:ILinkageshutdownService
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
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string cmdStr = "";

            Linkageshutdown2CallMachineByAsync(jo);

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 联动关机-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Linkageshutdown2CallMachineBy(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_LINKAGESHUTDOWN));

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                jo["result"] = ErrorCode.Success;

                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                jo["biom"] = joBiom;

                SetBusinessmParam(jo);
            } 


            log.DebugFormat("end, args: jo = {0}", jo);
        }
        /// <summary>
        /// 基础配置2改-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Linkageshutdown2CallMachineByAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            linkageshutdownCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 设置取号机业务参数
        /// </summary>
        /// <param name="jo"></param>
        private void SetBusinessmParam(JObject jo)
        {
            if (jo.Value<int>("result") == ErrorCode.Success)
            {
                // 叫号机返回消息成功,取号终端业务处理
                int cmdStr = jo.Value<int>("command");
                PageCommand operation = (PageCommand)cmdStr;
             
                    JToken joBody = jo["biom"]["body"];

                    //lock (BuzConfig2ICBC.staticLook)
                    //{

                   
                    //}
              
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
