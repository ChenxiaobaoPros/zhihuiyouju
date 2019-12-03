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
using System.IO;

namespace Aoto.CQMS.Core.Application.Impl
{
    /// <summary>
    /// 号票管理业务类
    /// </summary>
    public class TicketsconfigServiceImpl : ITicketsconfigService
    {
        private static ILog log = LogManager.GetLogger("app");

        protected IScriptInvoker scriptInvoker;

        private RunAsyncCaller ticketsconfigCaller;

        public TicketsconfigServiceImpl()
        {
            ticketsconfigCaller = new RunAsyncCaller(Ticketsconfig2CallMachine);
        }
        /// <summary>
        /// 号票配置-页面
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Ticketsconfig2JS(JObject jo)
        {
            log.DebugFormat("begin", jo);
            try
            {
                Ticketsconfig2CallMachineAsync(jo);
            }
            catch (Exception e)
            {
                log.Error("TicketsconfigServiceImpl.Ticketsconfig2JS error", e);
            }

            log.DebugFormat("end", jo);
        }

        /// <summary>
        /// 号票配置2查-叫号终端
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Ticketsconfig2CallMachine(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            // 获取页面操作命令
            int cmdStr = jo.Value<int>("command");

            string tickePrintForm = String.Empty;

            string tickePrintModTime = String.Empty;

            jo["biom"]["body"]["tickePrintModTime"] = DateTime.Now.ToString();

            if (cmdStr == 33)
            {
                tickePrintForm = jo["biom"]["body"].Value<string>("tickePrintForm").Replace("'", "\""); ;

                tickePrintModTime = DateTime.Now.ToString();

                jo["biom"]["body"]["tickePrintModTime"] = tickePrintModTime;

            }

            //string tickePrintModTime = jo["biom"]["body"].Value<string>("TickePrintModTime") == null ? String.Empty : jo["biom"]["body"].Value<string>("TickePrintModTime");

            PageCommand operation = (PageCommand)cmdStr;
            // 回调js方法

            string callback = jo.Value<string>("callback");
            jo.Remove("callback");
            IcbcInfos icbcInfo = new IcbcInfos();
            jo["biom"]["head"]["qmsIp"] = BuzConfig2ICBC.LocalIP;
            icbcInfo.QmsIp = jo["biom"]["head"].Value<string>("qmsIp");

            icbcInfo.TradeCode = jo["biom"]["head"].Value<string>("tradeCode");

            icbcInfo.Content = jo.ToString();

            //string dataStr = "{\"biom\":{\"head\":{\"retCode\":\"0\",\"retMsg\":\"成功\"},\"body\":{\"tickePrintForm\":\"XFSMEDIA 'Blank'\r\nBEGIN\r\nUNIT MM, 1, 1\r\nSIZE 300, 300\r\nEND\r\nXFSMEDIA 'ReceiptMedia'\r\nBEGIN\r\nUNIT MM, 1, 1\r\nSIZE 300, 300\r\nEND\r\nXFSFORM 'Queue7'\r\nBEGIN\r\nSIZE      40, 14\r\nUNIT     ROWCOLUMN, 1, 1\r\nALIGNMENT TOPLEFT, 1, 0\r\nLANGUAGE 2052\r\nXFSFIELD 'text1'\r\nBEGIN\r\nPOSITION    13, 6\r\nSIZE        40,1\r\nHORIZONTAL  LEFT\r\nSTYLE       NORMAL\r\nINITIALVALUE '固定文本'\r\nEND\r\nEND\",\"tickePrintFormat\":\"\",\"tickePrintModTime\":\"\",\"ticketPrintJson\":\"{'dpi':100,'family':'SimSun','style':'normal','size':12,'offset':{'x':0,'y':0},'items':[{'type':'text','text':'固定文本','family':'SimSun','style':'normal','scale':'normal','size':12,'height':1,'x':13,'y':6}]}\"}}}";//HttpClient.Post("/", icbcInfo);
            string dataStr = HttpClient.Post("/", icbcInfo);


            log.DebugFormat("接收叫号终端返回报文, retMess = {0}", dataStr);


            jo.RemoveAll();

            if (JsonSplit.IsJson(dataStr))    // 接收到返回消息""
            {
                JObject jokeit = JObject.Parse(dataStr);

                JToken joBiom = jokeit["biom"];

                jo["biom"] = joBiom;

                string retCode = jo["biom"]["head"].Value<string>("retCode");

                if (retCode.Equals("0"))
                {

                    if (cmdStr == 33)
                    {

                        log.DebugFormat("进入 保存 号票 .....");

                       

                        log.DebugFormat("....................时间：{0}" , tickePrintModTime);
                        if (tickePrintModTime!="")
                        {

                            log.DebugFormat("....................时间对比：{0}：{1}", Config.App.TickePrintModTime,tickePrintModTime);
                            if (Config.App.TickePrintModTime!=tickePrintModTime)
                            {
                                log.DebugFormat("begin Update print Form，tickePrintModTime : {0}", tickePrintModTime);

                                // 打印form不一致 进行更换
                                string pathDir = @"C:\FORM";

                                if (!Directory.Exists(pathDir))
                                {
                                    Directory.CreateDirectory(pathDir);
                                }

                                string path = pathDir+@"\Receipt.form";

                                File.Delete(path);

                                if (FileHelper.WriteToFile(path, tickePrintForm))
                                {
                                    jo["tickePrintModTime"] = tickePrintModTime;

                                    Config.SaveAppConfig(jo);

                                    log.DebugFormat("Update print success!   {0}",jo);
                                }
                                else
                                {
                                    log.DebugFormat("Update print fail!");
                                }
                            }
                            log.DebugFormat("end");
                        }

                    }

                 



                   


                }


            }
            else
            {
                BuzConfig2ICBC.Jo2Return(jo);
            }

            jo["callback"] = callback;
            jo["command"] = cmdStr;
            log.DebugFormat("end, args: jo = {0}", jo);
        }

        /// 号票配置2查-叫号终端 异步
        /// </summary>
        /// <param name="jo"></param>
        public virtual void Ticketsconfig2CallMachineAsync(JObject jo)
        {
            log.DebugFormat("begin");

            ticketsconfigCaller.BeginInvoke(jo, Callback, jo);

            log.Debug("end");
        }

        /// <summary>
        /// 设置取号机业务参数
        /// </summary>
        /// <param name="jo"></param>
        private void SetBusinessmParam(JObject jo)
        {
                JToken joBody = jo["biom"]["body"];
                //lock (BuzConfig2ICBC.staticLook)
                //{
                //号票打印格式               
                    BuzConfig2ICBC.TickePrintFormat = joBody.Value<string>("tickePrintFormat");                
                //}   
        }

        private void Callback(IAsyncResult ar)
        {
            JObject jo = (JObject)ar.AsyncState;

            int cmdStr = jo.Value<int>("command");
            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);

            }
            catch (Exception e)
            {
                jo = new JObject();
                BuzConfig2ICBC.Jo5Return(jo);
                log.Error("TicketsconfigServiceImpl.Callback Error", e);
            }
            finally
            {
                log.DebugFormat("打印回调..................................................{0}" ,jo);
                if (null == scriptInvoker)
                {
             
                    scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
                }
                if (cmdStr == 33)
                {
                    log.DebugFormat("重置打印机...");
                    scriptInvoker.ResetPrint();
                }
                scriptInvoker.ScriptInvoke(jo);
            }
        }
  }
}
