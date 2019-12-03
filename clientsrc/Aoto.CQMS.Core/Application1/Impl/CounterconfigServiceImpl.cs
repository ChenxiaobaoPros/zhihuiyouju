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
    public class CounterconfigServiceImpl:ICounterconfigService
    {
        private static ILog log = LogManager.GetLogger("app");
        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller counterconfig2SelectCaller;
        private RunAsyncCaller counterconfig2UpdateCaller;
        private RunAsyncCaller counterconfig2SAddCaller;
        private RunAsyncCaller counterconfig2DeleteCaller;
        private RunAsyncCaller windowQueueAdjust2Caller;

        public CounterconfigServiceImpl()
        {
            counterconfig2SelectCaller = new RunAsyncCaller(Counterconfig2CallMachineBySelect);
            counterconfig2UpdateCaller = new RunAsyncCaller(Counterconfig2CallMachineByUpdate);
            counterconfig2SAddCaller = new RunAsyncCaller(Counterconfig2CallMachineByAdd);
            counterconfig2DeleteCaller = new RunAsyncCaller(Counterconfig2CallMachineByDelete);
            windowQueueAdjust2Caller = new RunAsyncCaller(WindowQueueAdjust2CallMachine);
        }

        /// <summary>
        /// 柜台配置-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Counterconfig2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            int cmdStr = jo.Value<int>("command");
            PageCommand operation = (PageCommand)cmdStr;

            switch (operation)
            {
                case PageCommand.Select:
                    Counterconfig2CallMachineBySelectAsync(jo);
                    break;
                case PageCommand.Update:
                    Counterconfig2CallMachineByUpdateAsync(jo);
                    break;
                case PageCommand.Add:
                    Counterconfig2CallMachineByAddAsync(jo);
                    break;
                case PageCommand.Delete:
                    Counterconfig2CallMachineByDeleteAsync(jo);
                    break;
                case PageCommand.Adjust:
                    WindowQueueAdjust2CallMachineAsync(jo);
                    break;
                default:

                    break;

            }

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 柜台配置2查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Counterconfig2CallMachineBySelect(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_COUNTERCONFIG2SELECT));

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                jo["result"] = ErrorCode.Success;

                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                jo["biom"] = joBiom;

            }

            log.DebugFormat("end, args: jo = {0}", jo);
        } 
        /// 柜台配置2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Counterconfig2CallMachineBySelectAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            counterconfig2SelectCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 柜台配置2改-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Counterconfig2CallMachineByUpdate(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_COUNTERCONFIG2UPDATE));

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                jo["result"] = ErrorCode.Success;

                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                jo["biom"] = joBiom;
            }

            log.DebugFormat("end, args: jo = {0}", jo);
        }    /// <summary>
        /// 柜台配置2改-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Counterconfig2CallMachineByUpdateAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            counterconfig2UpdateCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 柜台配置2增-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Counterconfig2CallMachineByAdd(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_COUNTERCONFIG2ADD));

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
        /// 柜台配置2增-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Counterconfig2CallMachineByAddAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            counterconfig2SAddCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }
        /// <summary>
        /// 柜台配置2删-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Counterconfig2CallMachineByDelete(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_COUNTERCONFIG2DELETE));

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
        /// 柜台配置2删-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Counterconfig2CallMachineByDeleteAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            counterconfig2DeleteCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 窗口队列调整-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void WindowQueueAdjust2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_WINDOWQUEUEADJUST));

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
        /// 窗口队列调整-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void WindowQueueAdjust2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            windowQueueAdjust2Caller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 设置取号机业务参数
        /// </summary>
        /// <param name="jo"></param>
        private void SetCounterParam(JObject jo)
        {
            if (jo.Value<int>("result") == ErrorCode.Success)
            {
                // 叫号机返回消息成功,取号终端业务处理
               
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

            SetCounterParam(jo);

            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);

            }
            catch (Exception e)
            {
                jo["result"] = ErrorCode.Failure;
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
