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
    /// 本地卡类型业务类
    /// </summary>
    public class LocalcardtypeServiceImpl:ILocalcardtypeService
    {
        private static ILog log = LogManager.GetLogger("app");

        public LocalcardtypeServiceImpl()
        {
    
        }

        /// <summary>
        /// 本地卡类型-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardtype2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string cmdStr = "";

            switch (cmdStr)
            {
                case "select":
                    Localcardtype2CallMachineBySelect(jo);
                    break;
                case "add":
                    Localcardtype2CallMachineByAdd(jo);
                    break;
                case "update":
                    Localcardtype2CallMachineByUpdate(jo);
                    break;
                case "delete":
                    Localcardtype2CallMachineByDelete(jo);
                    break;
                default:

                    break;

            }
            

            jo["result"] = ErrorCode.Success;


            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 本地卡类型查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardtype2CallMachineBySelect(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_LOCALCARDTYPE2SELECT));

            jo = JObject.Parse(dataStr);

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 本地卡类型2增-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardtype2CallMachineByAdd(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_LOCALCARDTYPE2ADD));

            jo = JObject.Parse(dataStr);

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 本地卡类型2改-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardtype2CallMachineByUpdate(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_LOCALCARDTYPE2UPDATE));

            jo = JObject.Parse(dataStr);

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 本地卡类型2删-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardtype2CallMachineByDelete(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_LOCALCARDTYPE2DELETE));

            jo = JObject.Parse(dataStr);

            log.DebugFormat("end, args: jo = {0}", jo);
        }
    }
}
