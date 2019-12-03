using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Threading;
using Aoto.PPS.Infrastructure.Configuration;
using Aoto.PPS.Infrastructure;
using Aoto.CQMS.Core.Application;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core
{
    /// <summary>
    /// 关机服务
    /// </summary>
    public class ShutdownSevice
    {
        private static ILog log = LogManager.GetLogger("job");

        private Thread thread;

        private DateTime curDateTime = DateTime.Now;

        private DateTime shutdownDataTime;

        private int shutdownInterval=15000;

        IBasicconfigService basicconfigService = null;

        private readonly string basicconfigStr = "{\"biom\":{\"head\":{\"tradeCode\":\"basicconfig2select\",\"qmsIp\":\"\"},\"body\":{}}}";

        public ShutdownSevice()
        {
            thread = new Thread(Run);

            basicconfigService = AutofacContainer.ResolveNamed<IBasicconfigService>("basicconfigService");
        }

        public void Start()
        {
            

            thread.Start();
        }

        private void Run()
        {
            JObject jo = new JObject();

            jo["biom"] = JObject.Parse(basicconfigStr)["biom"];

            basicconfigService.Basicconfig2CallMachine(jo);


            Thread.Sleep(30000);

            log.DebugFormat("begin");
            int curHour = 0;
            int curMin = 0;

            int shtHour = 0;
            int shtMin = 0;

            while (true)
            {
                try
                {

                    //log.DebugFormat("关机循序...   启动状态：" + BuzConfig2ICBC.GetAutoShutdownFlag + "关机时间：" + BuzConfig2ICBC.GetShutdownTime);

                    if (BuzConfig2ICBC.GetAutoShutdownFlag.Equals("1"))
                    {
                        curDateTime=DateTime.Now;

                        shutdownDataTime = DateTime.Parse(BuzConfig2ICBC.GetShutdownTime);

                        curHour = curDateTime.Hour;
                        curMin = curDateTime.Minute;

                        shtHour = shutdownDataTime.Hour;
                        shtMin = shutdownDataTime.Minute;

                        // 判断 小时和分
                        if (curHour == shtHour && curMin == shtMin)
                        {
                            // 通知事件关机了
                            log.DebugFormat("准备关机...");
                            Win32ApiInvoker.DoExitWin(8);

                            break;
                        }
                    }

                }
                catch (Exception e)
                {
                    log.ErrorFormat("ShutdownSevice.Run " + e);
                }

                Thread.Sleep(shutdownInterval);

            }

            log.DebugFormat("end");
        }

    }
}
