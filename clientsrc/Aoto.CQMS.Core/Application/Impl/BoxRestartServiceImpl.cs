using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.Configuration;
using Aoto.PPS.Infrastructure.Utils;
using Newtonsoft.Json.Linq;
using System.Runtime.Remoting.Messaging;

namespace Aoto.CQMS.Core.Application.Impl
{
    class BoxRestartServiceImpl : IBoxRestartService



    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller boxRestart2CallMachineCaller;

        public BoxRestartServiceImpl()
        {
            boxRestart2CallMachineCaller = new RunAsyncCaller(BoxRestart2CallMachine);
        }
        
        
        public void BoxRestart2JS(Newtonsoft.Json.Linq.JObject jo)
        {

            log.DebugFormat("begin");

            try
            {

                BoxRestart2CallMachineAsync(jo);


            }
            catch (Exception e)
            {
                log.Error("BoxRestartServiceImpl.BoxRestart2JS error", e);
            }

            log.DebugFormat("end");
        }

        public void BoxRestart2CallMachine(Newtonsoft.Json.Linq.JObject jo)
        {


            log.DebugFormat("begin, args: jo = {0}", jo);

            // 获取页面操作命令
            int cmdStr = jo.Value<int>("command");

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

           //dataStr = "{ \"biom\": { \"head\": { \"retCode\": \"1\", \"retMsg\": \"\" }, \"body\": {  \"host\": \"127.0.55.3\", \"port\": \"9999\", \"timeout\": \"12000\"  }} }";
            //log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);

            jo.RemoveAll();

            log.DebugFormat("chongqifanhui", dataStr);

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {


                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                String code = joBiom["head"].Value<string>("retCode");



                jo["biom"] = joBiom;

            }
            else
            {
                BuzConfig2ICBC.Jo2Return(jo);
               
            }



            jo["callback"] = callback;

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        public void BoxRestart2CallMachineAsync(Newtonsoft.Json.Linq.JObject jo)
        {
            log.DebugFormat("begin");

            boxRestart2CallMachineCaller.BeginInvoke(jo, Callback, jo);

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

                log.Error("BoxRestartServiceImpl.Callback Error", e);
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
