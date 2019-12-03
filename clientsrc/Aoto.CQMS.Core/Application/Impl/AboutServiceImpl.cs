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
    /// 关于业务类
    /// </summary>
    public class AboutServiceImpl : IAboutService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller about2CallMachineCaller;

        public AboutServiceImpl()
        {
            about2CallMachineCaller = new RunAsyncCaller(About2CallMachine);
        }

        /// <summary>
        /// 关于配置-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void About2JS(JObject jo)
        {
            log.DebugFormat("begin");

            try
            {
                if (BuzConfig2ICBC.About2JsonStr.Equals(String.Empty))
                {
                    About2CallMachineAsync(jo);
                }
                else
                {
                    // 组装json格式
                    JObject jokeit = JObject.Parse(BuzConfig2ICBC.About2JsonStr);

                    JToken joBiom = jokeit["biom"];

                    jo["biom"] = joBiom;

                    if (null == scriptInvoker)
                    {
                        scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
                    }
                    scriptInvoker.ScriptInvoke(jo);

                }
            }
            catch (Exception e)
            {
                log.Error("AboutServiceImpl.About2JS error", e);
            }

            log.DebugFormat("end");
        }

        /// <summary>
        /// 关于配置-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void About2CallMachine(JObject jo)
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

            //dataStr="{ \"biom\": { \"head\": { \"retCode\": \"1\", \"retMsg\": \"\" }, \"body\": {  \"host\": \"1\", \"port\": \"\", \"timeout\": \"\"  }} }";

            log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);

            jo.RemoveAll();

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                //BuzConfig2ICBC.About2JsonStr = dataStr;

                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                String code = joBiom["head"].Value<string>("retCode");

                joBiom["body"]["qmsVersion"] = BuzConfig2ICBC.QmsVersion;
                joBiom["body"]["spVersion"] = BuzConfig2ICBC.SpVersion;
                joBiom["body"]["ocxVersion"] = BuzConfig2ICBC.OcxVersion;

                if (BuzConfig2ICBC.Success.Equals(code))
                {
                    SetAboutParam(jokeit);
                }
             
                jo["biom"] = joBiom;

                BuzConfig2ICBC.About2JsonStr = jo.ToString(); 

            }
            else
            {
                BuzConfig2ICBC.Jo2Return(jo);
            }

            jo["callback"] = callback;

            log.DebugFormat("end, args: jo = {0}", jo);
        }
       

        /// <summary>
        /// 关于-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void About2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            about2CallMachineCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 设置取号机业务参数
        /// </summary>
        /// <param name="jo"></param>
        private void SetAboutParam(JObject jo)
        {
            JToken joBody = jo["biom"]["body"];

            if (BuzConfig2ICBC.AppVersion.Equals(String.Empty))
            {
                BuzConfig2ICBC.AppVersion = joBody.Value<string>("appVersion");
            }
            if (BuzConfig2ICBC.AppVersion.Equals(String.Empty))
            {
                BuzConfig2ICBC.DriveVersion = joBody.Value<string>("driveVersion");
            }
            if (BuzConfig2ICBC.AppVersion.Equals(String.Empty))
            {
                BuzConfig2ICBC.VenDor = joBody.Value<string>("vendor");
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

                log.Error("AboutServiceImpl.Callback Error", e);
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
