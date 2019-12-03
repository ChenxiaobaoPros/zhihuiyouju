using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Newtonsoft.Json.Linq;
using Aoto.PPS.Infrastructure.Configuration;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ICBC;
using Aoto.PPS.Infrastructure.Utils;
using System.Runtime.Remoting.Messaging;

namespace Aoto.CQMS.Core.Application.Impl
{
    /// <summary>
    /// 预约业务类
    /// </summary>
    public class ReservationServiceImpl : IReservationService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller reservationCaller;

        public ReservationServiceImpl()
        {
            reservationCaller = new RunAsyncCaller(Reservation2CallMachine);
        }

        /// <summary>
        /// 预约-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Reservation2JS(JObject jo)
        {
            log.DebugFormat("begin", jo);
            try
            {
                Reservation2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("ReservationServiceImpl.Reservation2JS error", e);
            }

            log.DebugFormat("end", jo);
        }

        /// <summary>
        /// 预约-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Reservation2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            // 获取页面操作命令
            //int cmdStr = jo.Value<int>("command");
           
            //PageCommand operation = (PageCommand)cmdStr;
            // 回调js方法
            string cardFlag = jo["biom"]["body"].Value<string>("cardFlag");
            string callback = "getTicket4YyqhCallback";//jo.Value<string>("callback");
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
                JObject jokeit = JObject.Parse("{\"biom\":{\"head\":{\"retCode\":\"1\",\"retMsg\":\"\"},\"body\":{\"cardFlag\":\"\"}}}");

                jokeit["biom"]["head"]["retMsg"] = PromptInfos2ICBC.ICBC_MESS_QCMEXT01;
                jokeit["biom"]["body"]["cardFlag"] = cardFlag;

                jo["biom"] = jokeit["biom"];
            }

            jo["callback"] = callback;

            log.DebugFormat("end, args: jo = {0}", jo);
        }
        /// 语音配置2改-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Reservation2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            reservationCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }
        /// <summary>
        /// 预约参数设置
        /// </summary>
        /// <param name="jo"></param>
        private void SetBusinessmParam(JObject jo, PageCommand operation)
        {
            if (PageCommand.Select.Equals(operation))
            {
                JToken joBody = jo["biom"]["body"];

                //lock (BuzConfig2ICBC.staticLook)
                //{
                //号票参数

                if (BuzConfig2ICBC.PrintTemp.Equals(String.Empty))

                    BuzConfig2ICBC.PrintTemp = joBody.Value<string>("printtemp");

                //}

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

                log.Error("ReservationServiceImpl.Callback Error", e);
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
