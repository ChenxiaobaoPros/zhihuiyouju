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
using System.Net.NetworkInformation;

namespace Aoto.CQMS.Core.Application.Impl
{
    /// <summary>
    /// 工行专用业务类
    /// </summary>
    public class IcbcspecialuseServiceImpl : IIcbcspecialuseService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller icbcspecialuseCaller;
        private RunAsyncCaller icbcspecialusePingCaller;

        public IcbcspecialuseServiceImpl()
        {
            icbcspecialuseCaller = new RunAsyncCaller(Icbcspecialuse2CallMachine);
            icbcspecialusePingCaller = new RunAsyncCaller(IcbcspecialusePing2CallMachine);
        }

        /// <summary>
        /// 工行专用业-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Icbcspecialuse2JS(JObject jo)
        {
            log.DebugFormat("begin", jo);
            try
            {
                Icbcspecialuse2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("IcbcspecialuseServiceImpl.Icbcspecialuse2JS error", e);
            }
            log.DebugFormat("end", jo);
        }

        public virtual void IcbcspecialusePing2JS(JObject jo)
        {
            log.DebugFormat("begin", jo);
            try
            {
                IcbcspecialusePing2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("IcbcspecialuseServiceImpl.IcbcspecialusePing2JS error", e);
            }
            log.DebugFormat("end", jo);
        }

        /// <summary>
        /// 工行专用2查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Icbcspecialuse2CallMachine(JObject jo)
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

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
            {

                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                String code = joBiom["head"].Value<string>("retCode");

                if (BuzConfig2ICBC.Success.Equals(code))
                {
                    if (PageCommand.Select.Equals(operation)) { SetBusinessmParam(jokeit, operation); }
                    else if (PageCommand.Update.Equals(operation)) { SetBusinessmParam(jo, operation); }
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
        public virtual void IcbcspecialusePing2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);
            // 获取页面操作命令
            string cmdStr = jo["biom"]["head"].Value<string>("tradeCode");

           
            // 回调js方法

            string callback = jo.Value<string>("callback");

            jo.Remove("callback");

            string path = jo["biom"]["head"].Value<string>("qmsIp");

            jo.RemoveAll();

            JObject jokeit = JObject.Parse("{ \"biom\": { \"head\": { \"retCode\": \"" + 1 + "\", \"retMsg\": \"" + PromptInfos2ICBC.ICBC_MESS_QCMEXT01 + "\" }, \"body\": {  }	 } }");

            JToken joBiom = jokeit["biom"];

            jo["biom"] = joBiom;

            if ("pingLisServiceIp".Equals(cmdStr))
            {

                if (string.Empty.Equals(path))
                {
                   
                    jo["biom"]["head"]["retCode"] = "1";
                    jo["biom"]["head"]["retMsg"] = "传入IP值为空";
                }
                else
                {
                    
                    PingOptions po = new PingOptions();
                    po.DontFragment = true;
                    string data = "Test Data!";
                    byte[] buffer = Encoding.ASCII.GetBytes(data);
                    int timeout = 1000;
                    try
                    {
                        PingReply pr = new Ping().Send(path, timeout, buffer, po);
                  
                    if (pr.Status == IPStatus.Success)
                    {
                        jo["biom"]["head"]["retCode"] = "0";
                        jo["biom"]["head"]["retMsg"] = "监控服务器网络通畅";
                    }
                    else if(pr.Status == IPStatus.TimedOut)
                     {
                        jo["biom"]["head"]["retCode"] = "1";
                        jo["biom"]["head"]["retMsg"] = "连接超时";
                    }else{
                        jo["biom"]["head"]["retCode"] = "1";
                        jo["biom"]["head"]["retMsg"] = "监控服务器网络故障";
                    }
                          }catch(Exception e){
                              jo["biom"]["head"]["retCode"] = "1";
                              jo["biom"]["head"]["retMsg"] = "监控服务器网络故障";
                    }

                }
            }
            else {
                jo["biom"]["head"]["retCode"] = "1";
                jo["biom"]["head"]["retMsg"] = "发送命令出错";
            }
            jo["biom"]["body"]= "";

            jo["callback"] = callback;

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 工行专用2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Icbcspecialuse2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            icbcspecialuseCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }
        public virtual void IcbcspecialusePing2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            icbcspecialusePingCaller.BeginInvoke(jo, Callback, jo);

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
                if (BuzConfig2ICBC.DisOffGetFlag.Equals(String.Empty))
                {
                    //禁止脱机取号标记
                    BuzConfig2ICBC.DisOffGetFlag = joBody.Value<string>("disableOfflineGetTicketFlag");
                }
                
            }
            else
            {
                //禁止脱机取号标记
                BuzConfig2ICBC.DisOffGetFlag = joBody.Value<string>("disableOfflineGetTicketFlag");

                if (AppCache.dicPageCache.ContainsKey("disableOfflineGetTicketFlag"))
                {
                    AppCache.dicPageCache["disableOfflineGetTicketFlag"] = BuzConfig2ICBC.DisOffGetFlag;
                }
                else
                {
                    AppCache.dicPageCache.Add("disableOfflineGetTicketFlag", BuzConfig2ICBC.DisOffGetFlag);
                }


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
