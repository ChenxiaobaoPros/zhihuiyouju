﻿using System;
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
    /// 本地卡Bin业务类
    /// </summary>
    public class LocalcardbinServiceImpl:ILocalcardbinService
    {
        private static ILog log = LogManager.GetLogger("app");

        public LocalcardbinServiceImpl()
        {
    
        }

        /// <summary>
        /// 本地卡Bin-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardbin2JS(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;

            string cmdStr = "";

            switch (cmdStr)
            {
                case "select":
                    Localcardbin2CallMachineBySelect(jo);
                    break;
                case "add":
                    Localcardbin2CallMachineByAdd(jo);
                    break;
                case "update":
                    Localcardbin2CallMachineByUpdate(jo);
                    break;
                case "delete":
                    Localcardbin2CallMachineByDelete(jo);
                    break;
                default:

                    break;

            }
            

            jo["result"] = ErrorCode.Success;


            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 本地卡Bin2查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardbin2CallMachineBySelect(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_LOCALCARDBIN2SELECT));

            jo = JObject.Parse(dataStr);

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 本地卡Bin2增-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardbin2CallMachineByAdd(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_LOCALCARDBIN2ADD));

            jo = JObject.Parse(dataStr);

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 本地卡Bin2改-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardbin2CallMachineByUpdate(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_LOCALCARDBIN2UPDATE));

            jo = JObject.Parse(dataStr);

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// <summary>
        /// 本地卡Bin2删-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Localcardbin2CallMachineByDelete(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            string dataStr = HttpClient.Post("/", MessagePackage2ICBC.SendMessage(GlobalVariable2ICBC.ICBC_PARA_LOCALCARDBIN2DELETE));

            jo = JObject.Parse(dataStr);

            log.DebugFormat("end, args: jo = {0}", jo);
        }

    }
}
