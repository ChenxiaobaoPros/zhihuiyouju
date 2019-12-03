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
    /// 更新配置业务类
    /// </summary>
    public class UpdateconfigServiceImpl:IUpdateconfigService
    {
        private static ILog log = LogManager.GetLogger("app");
        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller updateconfig2SelectCaller;
        private RunAsyncCaller updateconfig2UpdateCaller;

        public UpdateconfigServiceImpl()
        {
            updateconfig2SelectCaller = new RunAsyncCaller(Updateconfig2CallMachineBySelect);
            updateconfig2UpdateCaller = new RunAsyncCaller(Updateconfig2CallMachineByUpdate);
        }

        /// <summary>
        /// 更新配置-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Updateconfig2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            int cmdStr = jo.Value<int>("command");

            PageCommand operation = (PageCommand)cmdStr;

            switch (operation)
            {
                case PageCommand.Select:
                    Updateconfig2CallMachineBySelectAsync(jo);
                    break;
                case PageCommand.Update:
                    Updateconfig2CallMachineByUpdateAsync(jo);
                    break;
                default:
                    jo["result"] = ErrorCode.Failure;
                    break;

            }

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 更新配置2查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Updateconfig2CallMachineBySelect(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_UPDATECONFIG2SELECT));

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
        /// 更新配置2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Updateconfig2CallMachineBySelectAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            updateconfig2SelectCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 更新配置2改-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Updateconfig2CallMachineByUpdate(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_UPDATECONFIG2UPDATE));

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                jo["result"] = ErrorCode.Success;

                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                jo["biom"] = joBiom;
            } 

            log.DebugFormat("end, args: jo = {0}", jo);
        }
        /// 更新配置2改-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Updateconfig2CallMachineByUpdateAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            updateconfig2UpdateCaller.BeginInvoke(jo, Callback, jo);

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
                    //是否启用更新(0否 1是)
                    BuzConfig2ICBC.UPDATEFLAG = joBody.Value<string>("updateFlag");
                    BuzConfig2ICBC.HTTPIP = joBody.Value<string>("httpIP");
                    BuzConfig2ICBC.HTTPPORT = joBody.Value<string>("httpPort");
                    BuzConfig2ICBC.LOCALPATH = joBody.Value<string>("localPath");
                    BuzConfig2ICBC.BACKUPPATH = joBody.Value<string>("backupPath");
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
