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
    public class VoiceconfigServiceImpl : IVoiceconfigService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller voiceconfigCaller;

        public VoiceconfigServiceImpl()
        {
            voiceconfigCaller = new RunAsyncCaller(Voiceconfig2CallMachine);
        }

        /// <summary>
        /// 语音配置-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Voiceconfig2JS(JObject jo)
        {
            log.DebugFormat("begin", jo);
            try
            {
                Voiceconfig2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("VoiceconfigServiceImpl.Voiceconfig2JS error", e);
            }

            log.DebugFormat("end", jo);
        }

        /// <summary>
        /// 语音配置2查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Voiceconfig2CallMachine(JObject jo)
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
        /// 语音配置-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Voiceconfig2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            voiceconfigCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 设置取号机业务参数
        /// </summary>
        /// <param name="jo"></param>
        private void SetBusinessmParam(JObject jo)
        {

                JToken joBody = jo["biom"]["body"];

                //lock (BuzConfig2ICBC.staticLook)
                //{
                //语音播放格式

                
                    //语音呼叫次数
                    BuzConfig2ICBC.SoundsPeakTimes = joBody.Value<string>("soundSpeakTimes");
               
                    //所有业务使用相同语言播放",（0否 1是）
                    BuzConfig2ICBC.UsesameLanspeakFlag = joBody.Value<string>("useSameLanSpeakFlag");
               
                    //播放语言，0-中文 1-英文 2-粤语，多个用“|”拼接
                    BuzConfig2ICBC.SpeakLanguage = joBody.Value<string>("speakLanguage");
               
                    //是否只播放指定窗口语音",（0否 1是）
                    BuzConfig2ICBC.SpeakSpecificwinFlag = joBody.Value<string>("speakSpecificWinFlag");
              

                
                    //指定窗口，多个窗口用“|”拼接
                    BuzConfig2ICBC.SpecificWin = joBody.Value<string>("specificWin");
                //}

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

                log.Error("VoiceconfigServiceImpl.Callback Error", e);
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
