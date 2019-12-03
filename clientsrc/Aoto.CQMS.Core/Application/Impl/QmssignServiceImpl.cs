using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Newtonsoft.Json.Linq;
using Aoto.PPS.Infrastructure.ICBC;
using Aoto.PPS.Infrastructure.Configuration;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Utils;
using System.Runtime.Remoting.Messaging;

namespace Aoto.CQMS.Core.Application.Impl
{
    /// <summary>
    /// 签到业务类
    /// </summary>
    public class QmssignServiceImpl : IQmssignService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller qmssignCaller;

        public QmssignServiceImpl()
        {

            qmssignCaller = new RunAsyncCaller(Qmssign2CallMachine);
        }

        /// <summary>
        /// 签到-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void GetQmssign2JS(JObject jo)
        {
            log.DebugFormat("begin");
            try
            {
                Qmssign2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("QmssignServiceImpl.GetQmssign2JS error", e);
            }
            log.DebugFormat("end");
        }

        /// <summary>
        /// 签到-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Qmssign2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            // 获取页面操作命令
            //int cmdStr = jo.Value<int>("command");
            //PageCommand operation = (PageCommand)cmdStr;
            // 回调js方法
            string callback = jo.Value<string>("callback");
            jo.Remove("callback");
            string dataStr = String.Empty;

            if (null == GlobalVariable2ICBC.ICBC_QMSSIGN)
            {
                IcbcInfos icbcInfo = new IcbcInfos();
                jo["biom"]["head"]["qmsIp"] = BuzConfig2ICBC.LocalIP;
                icbcInfo.QmsIp = jo["biom"]["head"].Value<string>("qmsIp");
                icbcInfo.TradeCode = jo["biom"]["head"].Value<string>("tradeCode");
                icbcInfo.Content = jo.ToString();

                dataStr = HttpClient.Post("/", icbcInfo);

                //log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);

                jo.RemoveAll();

                if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
                {
                    JObject jokeit = JObject.Parse(dataStr);

                    JToken joBiom = jokeit["biom"];

                    jo["biom"] = joBiom;

                    if (jo["biom"]["head"].Value<string>("retCode").Equals("0"))
                    {

                        GlobalVariable2ICBC.ICBC_QMSSIGN = jo;
                        SetBusinessmParam(jo, PageCommand.Select);
                    }

                }
                else
                {

                    BuzConfig2ICBC.Jo4Return(jo);

                    GlobalVariable2ICBC.ICBC_QMSSIGN = null;

                }

                jo["callback"] = callback;

                log.DebugFormat("end, args: jo = {0}", jo);
            }
            else
            {
                JObject joket = GlobalVariable2ICBC.ICBC_QMSSIGN;
                jo["biom"] = joket["biom"];
                jo["callback"] = callback;

                log.DebugFormat("end");
            }
        }

        /// <summary>
        /// 签到-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Qmssign2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            qmssignCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 基础设置-取号机业务参数赋值
        /// </summary>
        /// <param name="jo"></param>
        private void SetBusinessmParam(JObject jo, PageCommand operation)
        {
            JToken joBody = jo["biom"]["body"];

            if (BuzConfig2ICBC.DevStatus.Equals(String.Empty))
            {
                BuzConfig2ICBC.DevStatus = joBody.Value<string>("devStatus");
            }
            if (BuzConfig2ICBC.PageTimeout.Equals(String.Empty))
            {
                BuzConfig2ICBC.PageTimeout = joBody.Value<string>("pageTimeout");
            }
            if (BuzConfig2ICBC.DelayedStartTime.Equals(String.Empty))
            {
                BuzConfig2ICBC.DelayedStartTime = joBody.Value<string>("DelayedStartTime");
            }

            if (BuzConfig2ICBC.ShutdownPwd.Equals(String.Empty))
            {
                BuzConfig2ICBC.ShutdownPwd = joBody.Value<string>("shutdownPwd");
            }

            if (BuzConfig2ICBC.ExitGetTicketPwd.Equals(String.Empty))
            {
                BuzConfig2ICBC.ExitGetTicketPwd = joBody.Value<string>("exitGetTicketPwd");
            }

            if (BuzConfig2ICBC.OnlineSwitchPwd.Equals(String.Empty))
            {
                BuzConfig2ICBC.OnlineSwitchPwd = joBody.Value<string>("onlineSwitchPwd");
            }
            if (BuzConfig2ICBC.QmsUseFlag.Equals(String.Empty))
            {
                BuzConfig2ICBC.QmsUseFlag = joBody.Value<string>("qmsUseFlag");
            }

            if (BuzConfig2ICBC.DutySwitchPwd.Equals(String.Empty))
            {
                BuzConfig2ICBC.DutySwitchPwd = joBody.Value<string>("dutySwitchPwd");
            }

            BuzConfig2ICBC.DutyFlag = joBody.Value<string>("dutyFlag") == null ? String.Empty : joBody.Value<string>("dutyFlag");

            BuzConfig2ICBC.CertImageFlag = joBody.Value<string>("certImageFlag")==null?String.Empty:joBody.Value<string>("certImageFlag");
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

                log.Error("QmssignServiceImpl.Callback Error", e);
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