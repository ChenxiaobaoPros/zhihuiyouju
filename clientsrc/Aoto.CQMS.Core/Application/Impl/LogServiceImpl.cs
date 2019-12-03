using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Aoto.PPS.Infrastructure.ComponentModel;
using Newtonsoft.Json.Linq;
using Aoto.PPS.Infrastructure.Configuration;
using Aoto.PPS.Infrastructure;
using System.Runtime.Remoting.Messaging;
using Aoto.PPS.Infrastructure.Utils;

namespace Aoto.CQMS.Core.Application.Impl
{
    class LogServiceImpl : ILogService
    {

        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller qcmLogSelect2CallMachineCaller;

        public LogServiceImpl()
        {
            qcmLogSelect2CallMachineCaller = new RunAsyncCaller(SelectLog2CallMachine);
        }

        public void SelectLog2JS(Newtonsoft.Json.Linq.JObject jo)
        {
            log.DebugFormat("begin");

            try
            {
                
                    SelectLog2CallMachineAsync(jo);
                
                
            }
            catch (Exception e)
            {
                log.Error("LogServiceImpl.SelectLog2JS error", e);
            }

            log.DebugFormat("end");
        }

        public void SelectLog2CallMachine(Newtonsoft.Json.Linq.JObject jo)
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

            //log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);

            jo.RemoveAll();


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

        public void SelectLog2CallMachineAsync(Newtonsoft.Json.Linq.JObject jo)
        {
            log.DebugFormat("begin");

            qcmLogSelect2CallMachineCaller.BeginInvoke(jo, Callback, jo);

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

                log.Error("LogServiceImpl.Callback Error", e);
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
