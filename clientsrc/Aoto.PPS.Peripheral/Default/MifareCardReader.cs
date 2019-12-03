using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Newtonsoft.Json.Linq;
using AxIdcAxLib;
using AxEMVICAxLib;
using Aoto.PPS.Infrastructure.ComponentModel;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using Aoto.PPS.Infrastructure.Utils;

namespace Aoto.PPS.Peripheral.Default
{
    /// <summary>
    /// 四合一外设ocx调用
    /// </summary>
    public class MifareCardReader : IReader
    {
        private static readonly ILog log = LogManager.GetLogger("id");

        private AxIdcAx axIdcAx2Mifare;

        private AxEMVICAx axEMVICAx2Mifare;

        /// <summary>
        /// 设备逻辑名
        /// </summary>
        private readonly static string LOGICALNAME = "MifareCardReader";

        /// <summary>
        /// 磁道信息
        /// </summary>
        private readonly static string TRACKMAP = "1,2,3,CHIP";

        /// <summary>
        /// 读卡超时时间， 0-一直存在
        /// </summary>
        private readonly static int TIMEOUT = 0;

        private RunAsyncCaller readAsyncCaller;
        private IScriptInvoker scriptInvoker;

        private bool isBusy;
        private bool enabled=true;
        private bool cancelled;

        public bool IsBusy { get { return isBusy; } }
        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public MifareCardReader()
        {
            InitializeComponent();

            readAsyncCaller = new RunAsyncCaller(Read);

        }

        /// <summary>
        /// 初始化ocx控件
        /// </summary>
        private void InitializeComponent()
        {
            // InspIdcocx
            axIdcAx2Mifare = new AxIdcAx();

            // 读卡操作完成时候
            axIdcAx2Mifare.ReadRawDataComplete += new _DIdcAxEvents_ReadRawDataCompleteEventHandler(axIdcAx2Mifare_ReadRawDataComplete);

            // InspEmv
            axEMVICAx2Mifare = new AxEMVICAx();

            Initialize();
        }

        public void Initialize()
        {
            log.Debug("begin");

            if (OpenDevCon())
            {
                log.InfoFormat("打开 AxIdcAxLib.AxIdcAx 连接成功！");
            }
            else
            {
                log.ErrorFormat("打开 AxIdcAxLib.AxIdcAx 连接失败！");
            }


            log.Debug("end");

        }

        /// <summary>
        /// 打开设备连接
        /// </summary>
        private Boolean OpenDevCon()
        {
            axIdcAx2Mifare.LogicalName = LOGICALNAME;

            if (axIdcAx2Mifare.OpenConnection())
            {
                string rStr= axEMVICAx2Mifare.SetDeviceName(LOGICALNAME);
                log.InfoFormat("设置EMV逻辑名 -> SetDeviceName = {0}", rStr);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 打开读卡操作
        /// </summary>
        private void ReadRawData()
        {
            if (axIdcAx2Mifare.ReadRawData(TRACKMAP, TIMEOUT))
            {
                log.InfoFormat("开启读卡成功！");
            }
            else
            {
                log.InfoFormat("开启读卡失败...");
            }
        }

        /// <summary>
        /// 读卡操作完成时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void axIdcAx2Mifare_ReadRawDataComplete(object sender, AxIdcAxLib._DIdcAxEvents_ReadRawDataCompleteEvent e)
        {
            
            log.InfoFormat("begin");

            string cardRawDataStr = "{\"Chip\":\"3B6D000080318065B0872701BC83089000\",\"ChipStatus\":\"READ\",\"Result\":\"Success\",\"Track1\":\"\",\"Track1Status\":\"BLANK\",\"Track2\":\"\",\"Track2Status\":\"BLANK\",\"Track3\":\"\",\"Track3Status\":\"BLANK\"}";//(sender as AxIdcAxLib.AxIdcAx).CardRawData;

            log.InfoFormat("CardRawData = {0}", cardRawDataStr);

            if (JsonSplit.IsJson(cardRawDataStr))
            {
                JObject jo = JObject.Parse(cardRawDataStr);

                //ReadAsync(jo);
                Read(jo);
            }
            else
            {
                log.ErrorFormat("CardRawData 的返回字符串，不是合法JOSN格式");
            }


            

            ReadRawData();

            log.DebugFormat("end");
        }


        public void ReadAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            if (!enabled)
            {
                return;
            }

            if (isBusy)
            {
                jo["result"] = ErrorCode.Busy;
                log.InfoFormat("end, isBusy = {0}", isBusy);
                return;
            }

            isBusy = true;
            readAsyncCaller.BeginInvoke(jo, new AsyncCallback(ReadCallback), jo);
            log.Debug("end");
        }


        public void Read(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            if (cancelled)
            {
                jo["result"] = ErrorCode.Cancelled;
                log.Info("end, cancelled");
                return;
            }

            string chipStatusStr = jo.Value<string>("ChipStatus");

            log.InfoFormat("ChipStatus = {0}", chipStatusStr);

            if (chipStatusStr.Equals("READ"))
            {
                axEMVICAx2Mifare.SetDeviceName(LOGICALNAME);
                int emvInt = axEMVICAx2Mifare.EmvInitialize("MifareCardReader");

                log.InfoFormat(" -> EmvInitialize = {0}", emvInt);

                string emvStr = axEMVICAx2Mifare.StartTransaction();

                log.InfoFormat(" -> StartTransaction = {0}", emvStr);

                //开启EMV操作
                if ("ERR_NOSHOW" == emvStr)
                {
                    emvStr = axEMVICAx2Mifare.SelectApplication(0);

                    log.InfoFormat(" -> SelectApplication = {0}", emvStr);

                    if ("SUCCESS" == emvStr)
                    {

                        emvStr = axEMVICAx2Mifare.GetTagData("5A");	// 读取二磁信息

                        log.InfoFormat(" -> GetTagData = {0}", emvStr);

                        int nIndex = emvStr.IndexOf("F");

                        if (nIndex > 10)
                        {
                            emvStr = emvStr.Substring(0, nIndex);

                            log.InfoFormat("存在分割符 F ，args = {0}", emvStr);
                        }

                        // 获取到数据，返回调用 

                    }

                }



            }
            else if (chipStatusStr.Equals("BLANK"))
            {
                //log.InfoFormat("判断卡为【BLANK】");
            }
            else
            {
                //log.InfoFormat("判断卡为【INVALID】");
            }

            //jo["result"] = result;
            log.DebugFormat("end, args: jo = {0}", jo);
        }

        public int GetStatus()
        {
            log.Debug("begin");
            int s = StatusCode.Offline;

            if (enabled)
            {
                int status = 0;
                string statusStr = axIdcAx2Mifare.GetStatus();
                log.InfoFormat(" -> GetStatus = {0}", statusStr);

                if (0 == status)
                {
                    s = StatusCode.Normal;
                }
                else if (3003 == status)
                {
                    s = StatusCode.NotSupport;
                }
                else if (3006 == status)
                {
                    s = StatusCode.Busy;
                }
                else
                {
                    s = StatusCode.Offline;
                }
            }
            else
            {
                s = StatusCode.Disabled;
            }

            log.DebugFormat("end, return = {0}", s);
            return s;
        }

        public void Dispose()
        {
            log.Debug("begin");

            if (!enabled)
            {
                return;
            }

            if (null != axIdcAx2Mifare)
            {
                axIdcAx2Mifare.CloseConnection();
 
            }

            log.Debug("end");
        }


        private void ReadCallback(IAsyncResult ar)
        {
            JObject jo = (JObject)ar.AsyncState;

            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
            }
            catch (Exception e)
            {
                jo["result"] = ErrorCode.Failure;
                log.Error("Error", e);
            }
            finally
            {
                isBusy = false;
                int t = jo.Value<int>("timeout");

                if (864000000 == t)
                {
                    if (ErrorCode.Offline != jo.Value<int>("result"))
                    {
                        scriptInvoker.ScriptInvoke(jo);
                    }

                    if (!cancelled)
                    {
                        Thread.Sleep(200);

                        JObject joo = new JObject();
                        joo["objId"] = jo["objId"];
                        joo["callback"] = jo["callback"];
                        joo["tag"] = jo["tag"];
                        joo["type"] = jo["type"];
                        joo["timeout"] = t;
                        ReadAsync(joo);
                    }
                }
                else
                {
                    RunCompletedEvent(this, new RunCompletedEventArgs(jo));
                }
            }
        }

    }
}
