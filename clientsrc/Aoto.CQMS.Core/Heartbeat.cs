using System;
using System.Collections;
using System.Globalization;
using System.Threading;

using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;


using log4net;
using Newtonsoft.Json.Linq;
using Aoto.PPS.Infrastructure.Utils;
using Aoto.CQMS.Core.Application;

namespace Aoto.CQMS.Core
{
    /// <summary>
    /// 心跳包
    /// </summary>
    public class Heartbeat
    {
        private static ILog log = LogManager.GetLogger("job");

        private IScriptInvoker scriptInvoker;

        private IStateUpdateService statusUpdateInvoker;

        private readonly static string heartbeatJosnStr = "{\"biom\":{\"head\":{\"tradeCode\":\"heartbeat\",\"qmsIp\":\"\"},\"body\":{}}}";

        private readonly static string devUpdateJosnStr = "{\"biom\":{\"head\":{\"tradeCode\":\"stateUpdate\",\"qmsIp\":\"\"},\"body\":{\"status\":{\"card\":\"\",\"rfcard\":\"\",\"miniprint\":\"\",\"idcard\":\"\",\"TrendMicro\":\"\",\"DSMClient\":\"\"}}}}";

        private Thread thread;
        private int statusInterval;

        private static string heartbeatStr = String.Empty;

        public Heartbeat()
        {
            log.DebugFormat("begin  初始化...");

            statusInterval = Config.App.Peripheral.Value<int>("statusInterval");

            thread = new Thread(Run);

            log.DebugFormat("end");
        }

        public void Start()
        {
            thread.Start();
        }

        private void Run()
        {
            Thread.Sleep(60000);

            log.DebugFormat("begin");
            JObject jo = new JObject();
            JObject joket = new JObject();
            JObject jodev = new JObject();
            JObject jodevket = new JObject();

            string devStr = String.Empty;
            jo = JObject.Parse(heartbeatJosnStr);
            jodevket = JObject.Parse(devUpdateJosnStr);

            jo["biom"]["head"]["qmsIp"] = BuzConfig2ICBC.LocalIP;

            IcbcInfos icbcInfo = new IcbcInfos();
            icbcInfo.QmsIp = jo["biom"]["head"].Value<string>("qmsIp");
            icbcInfo.TradeCode = jo["biom"]["head"].Value<string>("tradeCode");
            icbcInfo.Content = jo.ToString();

            heartbeatStr= BuzConfig2ICBC.DevStatus;

            while (true)
            {
                try
                {
                    log.DebugFormat("begin heartbeat send...");

                    jodev.RemoveAll();
                    jo.RemoveAll();
                    joket.RemoveAll();

                    string dataStr = HttpClient.Post("/", icbcInfo);

                    //  测试数据
                    //dataStr = "{\"biom\":{\"head\":{\"retCode\":\"0\",\"retMsg\":\"成功\"},\"body\":{\"devStatus\":\"1|0|1|\"}}}";
                    if (JsonSplit.IsJson(dataStr))    // 接收到返回消息
                    {
                        joket = JObject.Parse(dataStr);

                        log.DebugFormat("Receive heartbeat packet data ：{0}", joket);

                        if (joket["biom"]["head"].Value<string>("retCode").Equals("0"))
                        {
                            devStr = joket["biom"]["body"].Value<string>("devStatus") +"|"+ AppState.PrintStatus;

                            if (!heartbeatStr.Equals(devStr))
                            {
                                BuzConfig2ICBC.DevStatus = devStr;

                                log.DebugFormat("Trigger page status update...");

                                heartbeatStr = devStr;

                                jo["biom"] = joket["biom"];

                                jo["biom"]["head"]["retMsg"] = BuzConfig2ICBC.INFO_01;

                                // 测试数据
                                jo["biom"]["body"]["devStatus"] = jo["biom"]["body"]["devStatus"];

                                jo["callback"] = "updateOnlineStatusCallback";

                                if (null == scriptInvoker)
                                {
                                    scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
                                }

                                scriptInvoker.ScriptInvoke(jo);

                            }

                        }
                        else
                        {
                            // 心跳包失败
                            log.DebugFormat("Receive heartbeat packet data failed...");
                        }

                    }
                    else
                    {
                        // 心跳异常
                        log.ErrorFormat("Receive heartbeat packet data format is not correct...");
                    }

                    if (null == scriptInvoker)
                    {
                        scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
                    }

                    jodev["biom"] = jodevket["biom"];

                    scriptInvoker.PeripheralInvoke(jodev);

                    if (null == statusUpdateInvoker)
                    {
                        statusUpdateInvoker = AutofacContainer.ResolveNamed<IStateUpdateService>("stateUpdateService");
                    }

                    statusUpdateInvoker.StateUpdate2CallMachine(jodev);

                    //log.DebugFormat("上传设备状态返回值：{0}", jodev);

                    Thread.Sleep(statusInterval);

                    log.DebugFormat("end");
                }
                catch (Exception e)
                {
                    log.Error("log.Run() error", e);
                }
            }



        }
    
   
    }
}