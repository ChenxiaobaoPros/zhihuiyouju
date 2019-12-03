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

namespace Aoto.CQMS.Core.Application.Impl
{
    /// <summary>
    /// 预约业务类
    /// </summary>
    public class ReservationServiceImpl:IReservationService
    {
        private static ILog log = LogManager.GetLogger("app");

        public ReservationServiceImpl()
        {
    
        }

        /// <summary>
        /// 预约-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Reservation2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            Reservation2CallMachine(jo);

            jo["result"] = ErrorCode.Success;


            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 预约-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Reservation2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_RESERVATION));

            jo = JObject.Parse(dataStr);

            log.DebugFormat("end, args: jo = {0}", jo);
        }
    }
}
