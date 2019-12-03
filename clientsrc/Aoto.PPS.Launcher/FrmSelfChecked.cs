using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.Configuration;

using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Aoto.CQMS.Core.Application;
using Aoto.PPS.Infrastructure.Utils;
using Microsoft.Win32;

namespace Aoto.PPS.Launcher
{
    public partial class FrmSelfChecked : Form
    {
        private static ILog log = LogManager.GetLogger("app");
        private static FrmSelfChecked instance;

        /// <summary>
        /// 设备逻辑名
        /// </summary>
        private readonly static string LOGICALNAME = "MifareCardReader";

        public static FrmSelfChecked Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new FrmSelfChecked();
                }

                return instance;
            }
        }

        public FrmSelfChecked()
        {
            InitializeComponent();
            Text = Config.App.Name + " " + Config.App.Version;

            if (File.Exists(Config.AppIconFilePath))
            {
                Icon = new Icon(Config.AppIconFilePath);
            }
        }

        private void FrmSelfCheckedLoad(object sender, EventArgs e)
        {
            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int percent = 0;
            string retMess = String.Empty;
            JObject jo = new JObject();

            string synchroTime = String.Empty;  // 同步时间 yyyy-mm-dd hh:mm:ss
            int delayedStartTime = 0; //  延后启动时间 秒
            string tickePrintModTime = String.Empty;    //  号票更新时间
            string tickePrintForm = String.Empty;   //  号票打印Form

            #region 签到

            IQmssignService qmssginService = AutofacContainer.ResolveNamed<IQmssignService>("qmssignService");

            try
            {
                jo.RemoveAll();

                backgroundWorker.ReportProgress(percent + 5, "执行叫号机签到指令中...     ");

                jo = JObject.Parse(GlobalVariable2ICBC.ICBC_QMSSIGN_STR);

                JObject joket = new JObject();

                joket["biom"] = jo["biom"];

                qmssginService.Qmssign2CallMachine(joket);

                if (joket["biom"]["head"].Value<string>("retCode").Equals("0"))    // 签到成功
                {
                    retMess = "成功";

                    if (AppCache.dicPageCache.ContainsKey("signFlag"))
                    {
                        AppCache.dicPageCache["signFlag"] = "1";
                    }
                    else
                    {
                        AppCache.dicPageCache.Add("signFlag", "1");
                    }

                    synchroTime = joket["biom"]["body"].Value<string>("synchroTime") == null ? String.Empty : joket["biom"]["body"].Value<string>("synchroTime");

                    delayedStartTime = joket["biom"]["body"].Value<int>("DelayedStartTime") == null ? 0 : joket["biom"]["body"].Value<int>("DelayedStartTime");

                    tickePrintModTime = joket["biom"]["body"].Value<string>("TickePrintModTime") == null ? String.Empty : joket["biom"]["body"].Value<string>("TickePrintModTime");

                    tickePrintForm = joket["biom"]["body"].Value<string>("tickePrintForm") == null ? String.Empty : joket["biom"]["body"].Value<string>("tickePrintForm").Replace("'", "\"");

                    //tickePrintModTime = "1";

                    //tickePrintForm = "XFSMEDIA\"Blank\"BEGINUNITMM,1,1SIZE300,300ENDXFSMEDIA\"ReceiptMedia\"BEGINUNITMM,1,1SIZE300,300ENDXFSFORM\"Queue7\"BEGINSIZE40,14UNITROWCOLUMN,1,1ALIGNMENTTOPLEFT,1,0LANGUAGE2052XFSFIELD\"LOGO\"BEGINPOSITION0,0SIZE46,1HORIZONTALCENTERSTYLEBOLD|DOUBLE|DOUBLEHIGHINITIALVALUE\"中国工商银行\"ENDXFSFIELD\"field1\"BEGINPOSITION10,1SIZE18,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"您的主办业务：\"ENDXFSFIELD\"field2\"BEGINPOSITION30,1SIZE12,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"测试业务1\"ENDXFSFIELD\"field3\"BEGINPOSITION5,2SIZE12,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"排队号码：\"ENDXFSFIELD\"field4\"BEGINPOSITION20,2SIZE10,1HORIZONTALLEFTSTYLEBOLD|DOUBLE|DOUBLEHIGHINITIALVALUE\"D0001\"ENDXFSFIELD\"field5\"BEGINPOSITION5,3SIZE24,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"您所在的排队队列前还有\"ENDXFSFIELD\"field6\"BEGINPOSITION33,3SIZE5,1HORIZONTALLEFTSTYLEBOLD|DOUBLE|DOUBLEHIGHINITIALVALUE\"10\"ENDXFSFIELD\"field7\"BEGINPOSITION39,3SIZE5,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"人等候\"ENDXFSFIELD\"field8\"BEGINPOSITION5,4SIZE40,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"请耐心留意叫号，过号做废。\"ENDXFSFIELD\"field15\"BEGINPOSITION5,5SIZE14,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"排队验证码:\"ENDXFSFIELD\"field9\"BEGINPOSITION20,5SIZE20,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"02000202-3040 0034\"ENDXFSFIELD\"field10\"BEGINPOSITION2,6SIZE40,1HORIZONTALLEFTINITIALVALUE\"-----------------*****-------------------\"ENDXFSFIELD\"field16\"BEGINPOSITION2,7SIZE12,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"温馨提示:\"ENDXFSFIELD\"field11\"BEGINPOSITION5,8SIZE40,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"测试分流提示\"ENDXFSFIELD\"field12\"BEGINPOSITION5,9SIZE40,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"两万元一下取款业务请到自助终端办理\"ENDXFSFIELD\"field19\"BEGINPOSITION2,10SIZE40,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"-----------------@-----------------\"ENDXFSFIELD\"field13\"BEGINPOSITION2,11SIZE40,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"2017-01-04 12:50:30\"ENDXFSFIELD\"field14\"BEGINPOSITION30,11SIZE24,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"  南京凤锦支行\"ENDXFSFIELD\"field17\"BEGINPOSITION27,12SIZE40,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"\"ENDXFSFIELD\"field18\"BEGINPOSITION5,13SIZE40,1HORIZONTALLEFTSTYLEBOLDINITIALVALUE\"请妥善保管本凭条\"ENDEND";
                    
                }
                else
                {
                    retMess = "失败";

                    if (AppCache.dicPageCache.ContainsKey("signFlag"))
                    {
                        AppCache.dicPageCache["signFlag"] = "0";
                    }
                    else
                    {
                        AppCache.dicPageCache.Add("signFlag", "0");
                    }
                }

                backgroundWorker.ReportProgress(percent + 5, "" + retMess + "\r\n");
            }
            catch (Exception ex)
            {
                log.Error("sign error", ex);
                backgroundWorker.ReportProgress(percent + 5, "失败\r\n");
            }

            #endregion

            #region 系统基础配置加载
            try
            {
                jo.RemoveAll();

                backgroundWorker.ReportProgress(percent + 5, "加载配置文件并初始化...     ");
               
                // 判断log文件夹
                string boxLogPath = Path.Combine(Application.StartupPath, "boxlogs");

                if (!Directory.Exists(boxLogPath))
                {
                    Directory.CreateDirectory(boxLogPath);
                } 


                // 同步时间
                if (!synchroTime.Equals(String.Empty) && !synchroTime.Equals(""))
                {
                    log.DebugFormat("begin Time calibration，synchroTime : {0}", synchroTime);

                    DateTime dt = DateTime.Parse(synchroTime);

                    ISystemService systemService = AutofacContainer.ResolveNamed<ISystemService>("systemService");

                    systemService.SetLocalTime(dt);

                    log.DebugFormat("end");
                }
                //  重置延后开机时间
                if (!delayedStartTime.Equals(String.Empty))
                {
                    log.DebugFormat("begin Long delay time，delayedStartTime : {0}", delayedStartTime);

                    jo["DelayedStartTime"] = delayedStartTime;

                    Config.SaveAppConfig(jo);

                    log.DebugFormat("end");
                }

                //  打印模板
                if (!tickePrintModTime.Equals(String.Empty))
                {
                    if (!Config.App.TickePrintModTime.Equals(tickePrintModTime))
                    {
                        log.DebugFormat("begin Update print Form，tickePrintModTime : {0}", tickePrintModTime);

                        // 打印form不一致 进行更换
                        string path = @"D:\Receipt.form";

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        if (FileHelper.WriteToFile(path, tickePrintForm))
                        {
                            jo["tickePrintModTime"] = tickePrintModTime;

                            Config.SaveAppConfig(jo);

                            log.DebugFormat("Update print success!");
                        }
                        else
                        {
                            log.DebugFormat("Update print fail!");
                        }
                    }
                    log.DebugFormat("end");
                }

                backgroundWorker.ReportProgress(percent + 5, "成功\r\n");
            }
            catch (Exception ex)
            {
                log.Error("SelfCheck Config.Init error", ex);
                backgroundWorker.ReportProgress(percent + 5, "异常\r\n");
            }

            #endregion

            #region 升级

            //if (File.Exists(Config.VersionPath))
            //{
            //    jo.RemoveAll();
            //    //ICommandService commandService = AutofacContainer.ResolveNamed<ICommandService>("commandService");
            //    string json = String.Empty;

            //    try
            //    {
            //        json = File.ReadAllText(Config.VersionPath, Encoding.UTF8);
            //        jo = JObject.Parse(json);

            //        int cmd = jo.Value<int>("command");
            //        int result = jo.Value<int>("commandResult");

            //        if (2 == result || 3 == result)
            //        {
            //            if (cmd == (int)OperationCommand.UpdateAfterRestart)
            //            {
            //                //commandService.UpdateCommand(jo);
            //            }

            //            File.Delete(Config.VersionPath);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        log.ErrorFormat("update cmd error, jo = {0}\r\n{1}", json, ex);
            //    }
            //}

            #endregion     

            #region 检测身份证、磁条卡、IC卡

            try
            {


                jo.RemoveAll();

                backgroundWorker.ReportProgress(percent + 5, "检测身份证阅读器状态...     ");

                // 初始化卡
                axIdcAx1.LogicalName = LOGICALNAME;

                if (axIdcAx1.OpenConnection())  // 打开ocx
                {
                    string statusStr = axIdcAx1.GetStatus();

                    if (JsonSplit.IsJson(statusStr))
                    {
                        jo = JObject.Parse(statusStr);

                        backgroundWorker.ReportProgress(percent + 5, GetStateText(jo.Value<string>("Device")) + "\r\n");
                    }
                    else
                    {
                        backgroundWorker.ReportProgress(percent + 5, "非法格式" + "\r\n");
                    }
                }
                else
                {

                    backgroundWorker.ReportProgress(percent + 5, "打开失败" + "\r\n");

                }
            }
            catch (Exception ex)
            {
                log.Error("axIdcAx error", ex);
                backgroundWorker.ReportProgress(percent + 5, "失败\r\n");
            }

            try
            {
                jo.RemoveAll();

                backgroundWorker.ReportProgress(percent + 5, "检测磁条卡阅读器状态...     ");

                //if (axIdcAx1.OpenConnection())  // 打开ocx
                //{
                    string statusStr = axIdcAx1.GetStatus();

                    if (JsonSplit.IsJson(statusStr))
                    {
                        jo = JObject.Parse(statusStr);

                        backgroundWorker.ReportProgress(percent + 5, GetStateText(jo.Value<string>("Device")) + "\r\n");
                    }
                    else
                    {
                        backgroundWorker.ReportProgress(percent + 5, "非法格式" + "\r\n");
                    }
                //}
                //else
                //{

                //    backgroundWorker.ReportProgress(percent + 5, "打开失败" + "\r\n");

                //}
            }
            catch (Exception ex)
            {
                log.Error("axIdcAx error", ex);
                backgroundWorker.ReportProgress(percent + 5, "失败\r\n");
            }

            try
            {
                jo.RemoveAll();

                backgroundWorker.ReportProgress(percent + 5, "检测智能卡阅读器状态...     ");



                //if (axIdcAx1.OpenConnection())  // 打开ocx
                //{
                    string statusStr = axIdcAx1.GetStatus();

                    if (JsonSplit.IsJson(statusStr))
                    {
                        jo = JObject.Parse(statusStr);

                        backgroundWorker.ReportProgress(percent + 5, GetStateText(jo.Value<string>("Device")) + "\r\n");
                    }
                    else
                    {
                        backgroundWorker.ReportProgress(percent + 5, "非法格式" + "\r\n");
                    }
                //}
                //else
                //{

                //    backgroundWorker.ReportProgress(percent + 5, "打开失败" + "\r\n");

                //}
            }
            catch (Exception ex)
            {
                log.Error("axIdcAx error", ex);
                backgroundWorker.ReportProgress(percent + 5, "失败\r\n");
            }

            try
            {
                axIdcAx1.CloseConnection();
            }
            catch (Exception ex)
            {
                log.Error("axIdcAx.CloseConnection() error", ex);
            }

            #endregion

            #region 检测号票打印机

            try
            {
                jo.RemoveAll();

                backgroundWorker.ReportProgress(percent + 5, "检测热号票打印机状态...     ");

                // 初始化卡
                axPtrAx1.LogicalName = "ReceiptPrinter";

                if (axPtrAx1.OpenConnection())
                {
                    string statusStr = axPtrAx1.GetStatus();

                    if (JsonSplit.IsJson(statusStr))
                    {
                        jo = JObject.Parse(statusStr);

                        backgroundWorker.ReportProgress(percent + 5, GetStateText(jo.Value<string>("Device")) + "\r\n");
                    }
                    else
                    {
                        backgroundWorker.ReportProgress(percent + 5, "非法格式" + "\r\n");
                    }

                }
                else
                {

                    backgroundWorker.ReportProgress(percent + 5, "打开失败" + "\r\n");

                }
            }
            catch (Exception ex)
            {
                log.Error("axPtrAx error", ex);
                backgroundWorker.ReportProgress(percent + 5, "失败\r\n");
            }

            try
            {
                axPtrAx1.CloseConnection();
            }
            catch (Exception ex)
            {
                log.Error("axPtrAx.CloseConnection() error", ex);
            }

            #endregion

            #region 清理日志

           
            try
            {

      

                backgroundWorker.ReportProgress(percent + 5, "正在清理过期日志...         ");

                DirectoryInfo di = new DirectoryInfo(Config.AppRoot + "\\logs");
                getVersion();

                FileInfo[] files = di.GetFiles("*.*");
                foreach (FileInfo file in files)
                {
                    
                    DateTime nowtime = DateTime.Now;
                    TimeSpan t = nowtime - file.CreationTime;
                    int day = t.Days;
                    if (day > Config.App.DelLogDate)
                    {
                        
                            file.Delete();
                                                                      
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error("axPtrAx error", ex);
                backgroundWorker.ReportProgress(percent + 5, "失败\r\n");
            }

            try
            {
                axPtrAx1.CloseConnection();
            }
            catch (Exception ex)
            {
                log.Error("axPtrAx.CloseConnection() error", ex);
            }

            #endregion


            Thread.Sleep(3000);
        }

        private void BackgroundWorkerProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            txtMessage.Text += e.UserState.ToString();
            lblMessage.Focus();
        }
        private void getVersion() {
            BuzConfig2ICBC.QmsVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
           // RegistryKey key = RegistryKey.HKEY_CURRENT_USER;
            RegistryKey key1 = Registry.CurrentUser;
            RegistryKey serviceName = key1.OpenSubKey("SOFTWARE\\icbc\\version");
           // string[] str = serviceName.GetSubKeyNames();
           // RegistryKey keytest = key.OpenSubKey("\\SOFTWARE\\icbc\\version",true);
            BuzConfig2ICBC.OcxVersion =(string) serviceName.GetValue("OCX");
            BuzConfig2ICBC.SpVersion = (string)serviceName.GetValue("SP");
            
        }
        private void BackgroundWorkerRunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private string GetStateText(string state)
        {
            string text = String.Empty;

            switch (state)
            {
                case "HEALTHY":
                    text = "正常";
                    break;
                case "NODEVICE":
                    text = "无效";
                    break;
                case "FATAL":
                    text = "异常";
                    break;
                default:
                    text = "未知";
                    break;
            }

            return text;
        }

        private string GetStateText(int state)
        {
            string text = String.Empty;

            switch (state)
            {
                case 1:
                    text = "正常";
                    break;
                case 3:
                    text = "离线";
                    break;
                case 2:
                    text = "忙";
                    break;
                case 0:
                    text = "禁用";
                    break;
                case 9:
                    text = "模拟";
                    break;
                default:
                    text = "未知";
                    break;
            }

            return text;
        }
    }
}