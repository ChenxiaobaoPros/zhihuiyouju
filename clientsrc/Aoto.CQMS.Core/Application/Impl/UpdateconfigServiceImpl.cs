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

        private RunAsyncCaller updateconfigCaller;
  
        public UpdateconfigServiceImpl()
        {
            updateconfigCaller = new RunAsyncCaller(Updateconfig2CallMachine);
          
        }

        /// <summary>
        /// 更新配置-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Updateconfig2JS(JObject jo)
        {
            log.DebugFormat("begin", jo);
            try
            {
                Updateconfig2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("UpdateconfigServiceImpl.Updateconfig2JS error", e);
            }
            log.DebugFormat("end", jo);
        }

        /// <summary>
        /// 更新配置2查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Updateconfig2CallMachine(JObject jo)
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

            
           
            // log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {
                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                String code = joBiom["head"].Value<string>("retCode");

                if (BuzConfig2ICBC.Success.Equals(code))
                {
                    if (PageCommand.Select.Equals(operation)) { 

                        //SetBusinessmParam(jokeit, operation);

                    }
                    else if (PageCommand.Update.Equals(operation)) {

                        // SetBusinessmParam(jo, operation); 
                    }
                }

                jo.RemoveAll();

                jo["biom"] = joBiom;

            }
            else
            {
                jo.RemoveAll();

                BuzConfig2ICBC.Jo2Return(jo);
            }

            jo["callback"] = callback;

            log.DebugFormat("end, args: jo = {0}", jo);
        }
        /// 更新配置2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Updateconfig2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            updateconfigCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

      
        /// <summary>
        /// 设置取号机业务参数
        /// </summary>
        /// <param name="jo"></param>
        private void SetBusinessmParam(JObject jo, PageCommand operation)
        {
            JToken joBody = jo["biom"]["body"];

            if (operation.Equals(PageCommand.Select))
            {
                if (BuzConfig2ICBC.UpdateFlag.Equals(String.Empty))
                {
                    BuzConfig2ICBC.UpdateFlag = joBody.Value<string>("isAutoUpdate");
                }
                if (BuzConfig2ICBC.HttpIp.Equals(String.Empty))
                {
                    BuzConfig2ICBC.HttpIp = joBody.Value<string>("ftpIp");
                }
                if (BuzConfig2ICBC.HttpPort.Equals(String.Empty))
                {
                    BuzConfig2ICBC.HttpPort = joBody.Value<string>("ftpPort");
                }
               

            }
            else
            {
                BuzConfig2ICBC.UpdateFlag = joBody.Value<string>("isAutoUpdate");

                BuzConfig2ICBC.HttpIp = joBody.Value<string>("ftpIp");

                BuzConfig2ICBC.HttpPort = joBody.Value<string>("ftpPort");

                

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
             
                log.Error("UpdateconfigServiceImpl.Callback Error", e);
          
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
