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

namespace Aoto.CQMS.Core.Application.Impl
{
    /// <summary>
    /// 远程卡Bin查询业务类
    /// </summary>
    public class BiomqmscardbinServiceImpl
    {
        private static ILog log = LogManager.GetLogger("app");

        public BiomqmscardbinServiceImpl()
        {
    
        }

        /// <summary>
        /// 远程卡Bin查询-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Biomqmscardbin2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string cmdStr = "";

            Biomqmscardbin2CallMachine(jo);
            

            jo["result"] = ErrorCode.Success;


            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 远程卡Bin查询-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Biomqmscardbin2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_BIOMQMSCARDBIN));

            jo = JObject.Parse(dataStr);


            log.DebugFormat("end, args: jo = {0}", jo);
        }
    }
}
