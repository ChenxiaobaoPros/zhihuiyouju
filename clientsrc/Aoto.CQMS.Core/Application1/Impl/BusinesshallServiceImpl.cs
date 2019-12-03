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
    public class BusinesshallServiceImpl:IBusinesshallService
    {
        private static ILog log = LogManager.GetLogger("app");
        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller businesshall2SelectCaller;
        private RunAsyncCaller businesshall2UpdateCaller;
        private RunAsyncCaller businesshall2SAddCaller;
        private RunAsyncCaller businesshall2DeleteCaller;

        public BusinesshallServiceImpl()
        {
            businesshall2SelectCaller = new RunAsyncCaller(Businesshall2CallMachineBySelect);
            businesshall2UpdateCaller = new RunAsyncCaller(Businesshall2CallMachineByUpdate);
            businesshall2SAddCaller = new RunAsyncCaller(Businesshall2CallMachineByAdd);
            businesshall2DeleteCaller = new RunAsyncCaller(Businesshall2CallMachineByDelete);
        }

        /// <summary>
        /// 营业厅分区-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Businesshall2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            int cmdStr = jo.Value<int>("command");

            PageCommand operation = (PageCommand)cmdStr;

            switch (operation)
            {
                case PageCommand.Select:
                    Businesshall2CallMachineBySelectAsync(jo);
                    break;
                case PageCommand.Update:
                    Businesshall2CallMachineByUpdateAsync(jo);
                    break;
                case PageCommand.Add:
                    Businesshall2CallMachineByAddAsync(jo);
                    break;
                case PageCommand.Delete:
                    Businesshall2CallMachineByDeleteAsync(jo);
                    break;
                default:
                    jo["result"] = ErrorCode.Failure;
                    break;

            }
   
            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 营业厅分区2查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Businesshall2CallMachineBySelect(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_BUSINESSHALL2SELECT));

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
        /// 营业厅分区2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Businesshall2CallMachineBySelectAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            businesshall2SelectCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }
        /// <summary>
        /// 营业厅分区2改-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Businesshall2CallMachineByUpdate(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_BUSINESSHALL2UPDATE));

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
        /// 营业厅分区2改-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Businesshall2CallMachineByUpdateAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            businesshall2UpdateCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 营业厅分区2增-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Businesshall2CallMachineByAdd(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_BUSINESSHALL2ADD));

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
        /// 营业厅分区2增-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Businesshall2CallMachineByAddAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            businesshall2SAddCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }
        /// <summary>
        /// 营业厅分区2删-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Businesshall2CallMachineByDelete(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_BUSINESSHALL2DELETE));

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
        /// 营业厅分区2删-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Businesshall2CallMachineByDeleteAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            businesshall2DeleteCaller.BeginInvoke(jo, Callback, jo);

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
