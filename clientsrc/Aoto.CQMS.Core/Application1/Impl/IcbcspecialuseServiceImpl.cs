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
    /// 工行专用业务类
    /// </summary>
    public class IcbcspecialuseServiceImpl:IIcbcspecialuseService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller icbcspecialuse2SelectCaller;
        private RunAsyncCaller icbcspecialuse2UpdateCaller;

        public IcbcspecialuseServiceImpl()
        {
            icbcspecialuse2SelectCaller = new RunAsyncCaller(Icbcspecialuse2CallMachineBySelect);
            icbcspecialuse2UpdateCaller = new RunAsyncCaller(Icbcspecialuse2CallMachineByUpdate);
        }

        /// <summary>
        /// 工行专用业-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Icbcspecialuse2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            int cmdStr = jo.Value<int>("command");

            PageCommand operation = (PageCommand)cmdStr;

            switch (operation)
            {
                case PageCommand.Select:
                    Icbcspecialuse2CallMachineBySelectAsync(jo);
                    break;
                case PageCommand.Update:
                    Icbcspecialuse2CallMachineByUpdateAsync(jo);
                    break;
                default:
                    jo["result"] = ErrorCode.Failure;
                    break;

            }
            
            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 工行专用2查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Icbcspecialuse2CallMachineBySelect(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_ICBCSPECIALUSE2SELECT));

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
        /// 工行专用2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Icbcspecialuse2CallMachineBySelectAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            icbcspecialuse2SelectCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 工行专用2改-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Icbcspecialuse2CallMachineByUpdate(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_ICBCSPECIALUSE2UPDATE));

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
        /// 工行专用2改-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Icbcspecialuse2CallMachineByUpdateAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            icbcspecialuse2UpdateCaller.BeginInvoke(jo, Callback, jo);

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
                if (operation.Equals(PageCommand.Select))
                {
                    JToken joBody = jo["biom"]["body"];

                    //lock (BuzConfig2ICBC.staticLook)
                    //{

                    // 叫号机返回消息成功,取号终端业务处理

                    
                    //禁止脱机取号标记
                    BuzConfig2ICBC.DISABLEOFFLINEGETTICKETFLAG = joBody.Value<string>("disableOfflineGetTicketFlag");
                    //}
                }
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

            SetBusinessmParam(jo);

            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);

            }
            catch (Exception e)
            {
                jo["result"] = ErrorCode.Failure;
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
