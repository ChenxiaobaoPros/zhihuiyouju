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
    /// 语音配置业务类
    /// </summary>
    public class VoiceconfigServiceImpl:IVoiceconfigService
    {
        private static ILog log = LogManager.GetLogger("app");
        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller voiceconfig2SelectCaller;
        private RunAsyncCaller voiceconfig2UpdateCaller;

        public VoiceconfigServiceImpl()
        {
            voiceconfig2SelectCaller = new RunAsyncCaller(Voiceconfig2CallMachineBySelect);
            voiceconfig2UpdateCaller = new RunAsyncCaller(Voiceconfig2CallMachineByUpdate);
        }

        /// <summary>
        /// 语音配置-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Voiceconfig2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

       

            int cmdStr = jo.Value<int>("command");

            PageCommand operation = (PageCommand)cmdStr;

            switch (operation)
            {
                case PageCommand.Select:
                    Voiceconfig2CallMachineBySelectAsync(jo);
                    break;
                case PageCommand.Update:
                    Voiceconfig2CallMachineByUpdateAsync(jo);
                    break;
                default:
                    jo["result"] = ErrorCode.Failure;
                    break;

            }
           log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 语音配置2查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Voiceconfig2CallMachineBySelect(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_VOICECONFIG2SELECT));

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
        /// 语音配置2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Voiceconfig2CallMachineBySelectAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            voiceconfig2SelectCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }


        /// <summary>
        /// 语音配置2改-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Voiceconfig2CallMachineByUpdate(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_VOICECONFIG2UPDATE));

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                jo["result"] = ErrorCode.Success;

                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                jo["biom"] = joBiom;
            }

            log.DebugFormat("end, args: jo = {0}", jo);
        }
        /// 语音配置2改-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Voiceconfig2CallMachineByUpdateAsync(JObject jo)
        {
            log.DebugFormat("begin, jo = {0}", jo);

            voiceconfig2UpdateCaller.BeginInvoke(jo, Callback, jo);

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
                    //语音播放格式
                    BuzConfig2ICBC.TICKEPRINTFORMAT = joBody.Value<string>("soundSpeakTimes");
                    BuzConfig2ICBC.USESAMELANSPEAKFLAG = joBody.Value<string>("useSameLanSpeakFlag");
                    BuzConfig2ICBC.SPEAKLANGUAGE = joBody.Value<string>("speakLanguage");
                    BuzConfig2ICBC.SPEAKSPECIFICWINFLAG = joBody.Value<string>("speakSpecificWinFlag");
                    BuzConfig2ICBC.SPECIFICWIN = joBody.Value<string>("specificWin");
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
