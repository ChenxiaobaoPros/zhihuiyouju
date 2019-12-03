using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;
using Aoto.PPS.Infrastructure.Utils;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;

namespace Aoto.CQMS.Core.Application.Impl
{
    public class SystemServiceImpl : ISystemService
    {
        private static ILog log = LogManager.GetLogger("app");

        private IScriptInvoker scriptInvoker;
        private RunAsyncCaller updateFromUsbCaller;

        public SystemServiceImpl()
        {
           
            updateFromUsbCaller = new RunAsyncCaller(UpdateFromUsb);
        }

        /// <summary>
        /// 设置系统声音
        /// </summary>
        public virtual void SetVolume(JObject jo)
        {
            jo["result"] = ErrorCode.Failure;
            double val = jo["settings"].Value<double>("volume");

            SetVolume(val);
            SaveSettings(jo);
            jo["result"] = ErrorCode.Success;
        }

        protected virtual IDictionary GetSettings(string key)
        {
            return null;
        }

        protected virtual void SaveSettings(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            jo["result"] = ErrorCode.Failure;
            IList<IDictionary> newList = new List<IDictionary>();
            IList<IDictionary> modifiedList = new List<IDictionary>();

            JToken settings = jo["settings"];
            JProperty jp = null;
            DateTime now = DateTime.Now;

            foreach (JToken t in settings.Children())
            {
                jp = (JProperty)t;
                IDictionary set = GetSettings(jp.Name);

                if (null == set)
                {
                    set = new Dictionary<string, object>();
                    set["type"] = 0;
                    set["key"] = jp.Name;
                    set["value"] = jp.Value.ToString();
                    set["createdDate"] = now;
                    set["lastUpdatedDate"] = now;
                    newList.Add(set);
                }
                else
                {
                    set["value"] = jp.Value.ToString();
                    set["lastUpdatedDate"] = now;
                    modifiedList.Add(set);
                }
            }

            jo.Remove("settings");

            //



            //
        }

        public virtual void SetVolume(double val)
        {
            VolumeControl.Instance.MasterVolume = val;
        }

        /// <summary>
        /// 设置静音
        /// </summary>
        /// <param name="jo"></param>
        public virtual void SetMute(JObject jo)
        {
            jo["result"] = ErrorCode.Failure;
            bool isMute = jo["settings"].Value<bool>("isMute");

            SetMute(isMute);
            //Settings.Save(jo);
            jo["result"] = ErrorCode.Success;
        }

        public virtual void SetMute(bool isMute)
        {
            VolumeControl.Instance.IsMute = isMute;
        }

        /// <summary>
        /// 重启
        /// </summary>
        public virtual void Restart(JObject jo)
        {
            Win32ApiInvoker.DoExitWin(2);
            jo["result"] = ErrorCode.Success;
        }

        public virtual void AppRestart(JObject jo)
        {
            AppState.Command = OperationCommand.AppRestart;
            scriptInvoker.Shut();
            jo["result"] = ErrorCode.Success;
        }

        public virtual void UpdateFromUsbAsync(JObject jo)
        {
            updateFromUsbCaller.BeginInvoke(jo, new AsyncCallback(Callback), jo);
        }

        /// <summary>
        /// U盘更新
        /// </summary>
        /// <param name="jo"></param>
        public virtual void UpdateFromUsb(JObject jo)
        {
            jo["result"] = ErrorCode.Failure;
            JObject joUpdate = null;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            string versionPath = String.Empty;
            string version = String.Empty;
            string patchFile = String.Empty;
            string srcPatchFileName = String.Empty;

            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType == DriveType.Removable)
                {
                    versionPath = Path.Combine(d.Name, "version");

                    if (File.Exists(versionPath))
                    {
                        joUpdate = JObject.Parse(File.ReadAllText(versionPath, Encoding.UTF8));
                        version = joUpdate.Value<string>("version");
                        patchFile = joUpdate.Value<string>("patchFile");
                        srcPatchFileName = Path.Combine(d.Name, patchFile);
                        break;
                    }
                }
            }

            if (null == joUpdate)
            {
                return;
            }

            string destPatchFileName = Path.Combine(Config.PatchAbsolutePath, patchFile);
            FileInfo file = new FileInfo(destPatchFileName);

            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }

            if (file.Exists)
            {
                file.Delete();
            }

            File.Copy(srcPatchFileName, destPatchFileName, true);
            string extractDirName = Path.GetFileNameWithoutExtension(patchFile);
            string extractDir = Path.Combine(Config.PatchAbsolutePath, extractDirName);
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(destPatchFileName, extractDir, null);

            file = new FileInfo(Path.Combine(extractDir, Config.UpdateExe));

            if (file.Exists)
            {
                file.CopyTo(Config.UpdateExeAbsolutePath, true);
            }

            joUpdate.Add(new JProperty("extractDirName", extractDirName));
            joUpdate.Add(new JProperty("commandId", 0));
            joUpdate.Add(new JProperty("command", (int)OperationCommand.UsbUpdate));
            joUpdate.Add(new JProperty("commandResult", 1));

            File.WriteAllText(Path.Combine(Config.AppRoot, "version"), joUpdate.ToString(Formatting.None), Encoding.UTF8);

            jo["result"] = ErrorCode.Success;
            AppRestart(jo);
        }

        /// <summary>
        /// 关机
        /// </summary>
        public virtual void Shutdown(JObject jo)
        {
            Win32ApiInvoker.DoExitWin(8);
            jo["result"] = ErrorCode.Success;
        }

        public virtual long GetIdleTick()
        {
            Win32ApiInvoker.LASTINPUTINFO lastInputInfo = new Win32ApiInvoker.LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);

            if (!Win32ApiInvoker.GetLastInputInfo(ref lastInputInfo))
            {
                return 0;
            }

            return Environment.TickCount - (long)lastInputInfo.dwTime;
        }

        public virtual void SetLocalTime(DateTime current)
        {
            Win32ApiInvoker.SystemTime sysTime = new Win32ApiInvoker.SystemTime();
            sysTime.wYear = Convert.ToUInt16(current.Year);
            sysTime.wMonth = Convert.ToUInt16(current.Month);
            sysTime.wDay = Convert.ToUInt16(current.Day);
            sysTime.wHour = Convert.ToUInt16(current.Hour);
            sysTime.wMinute = Convert.ToUInt16(current.Minute);
            sysTime.wSecond = Convert.ToUInt16(current.Second);
            sysTime.wMiliseconds = Convert.ToUInt16(current.Millisecond);
            Win32ApiInvoker.SetLocalTime(ref sysTime);
        }

        public virtual void BackupData()
        {
            string zipFilename = String.Empty;
            string srcDir = String.Empty;
            string srcDirPath = String.Empty;

            try
            {
                string date = DateTime.Now.ToString("yyyyMMdd");
                srcDir = "data" + date;
                srcDirPath = Path.Combine(Config.PatchAbsolutePath, srcDir);
                zipFilename = Path.Combine(Config.PatchAbsolutePath, srcDir + ".zip");

                if (Directory.Exists(srcDirPath))
                {
                    Directory.Delete(srcDirPath, true);
                }

                Directory.CreateDirectory(srcDirPath);

                string[] files1 = Directory.GetFiles(Path.Combine(Config.AppRoot, "data"), "*.db", SearchOption.TopDirectoryOnly);
                string[] files2 = Directory.GetFiles(Path.Combine(Config.AppRoot, "config"), "*.json", SearchOption.AllDirectories);
                string[] files = new string[files1.Length + files2.Length];
                Array.Copy(files1, files, files1.Length);
                Array.Copy(files2, 0, files, files1.Length, files2.Length);

                FileInfo destFileInfo = null;
                string relativeFileName = String.Empty;

                foreach (string f in files)
                {
                    if (f.EndsWith("advs.json", StringComparison.OrdinalIgnoreCase)
                        || f.EndsWith("app.json", StringComparison.OrdinalIgnoreCase)
                        || f.EndsWith("history.db", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    relativeFileName = f.Substring(f.IndexOf(Config.AppRoot) + Config.AppRoot.Length + 1);
                    destFileInfo = new FileInfo(Path.Combine(Config.PatchAbsolutePath, srcDir, relativeFileName));

                    if (!destFileInfo.Directory.Exists)
                    {
                        destFileInfo.Directory.Create();
                    }

                    File.Copy(f, destFileInfo.FullName, true);
                }

                FastZip fastZip = fastZip = AutofacContainer.Resolve<FastZip>();

                if (File.Exists(zipFilename))
                {
                    File.Delete(zipFilename);
                }

                fastZip.CreateZip(zipFilename, srcDirPath, true, null);
                Directory.Delete(srcDirPath, true);
            }
            catch (Exception e)
            {
                log.Error("BackupDb error", e);
            }
        }

        public virtual void SyncData()
        {
            string zipFilename = String.Empty;
            string srcDir = String.Empty;
            string srcDirPath = String.Empty;

            try
            {
                string date = DateTime.Now.ToString("yyyyMMdd");
                srcDir = "data" + date;
                srcDirPath = Path.Combine(Config.PatchAbsolutePath, srcDir);
                zipFilename = Path.Combine(Config.PatchAbsolutePath, srcDir + ".zip");

                HttpClient.DownloadFile(Config.App.RunMode.WebServer.Host, Config.App.RunMode.WebServer.Port,
                    Config.App.RunMode.WebServer.ContextPath, Config.App.RunMode.WebServer.Timeout, "/services/configs/sync", zipFilename);

                FastZip fastZip = fastZip = AutofacContainer.Resolve<FastZip>();
                fastZip.ExtractZip(zipFilename, srcDirPath, null);

                string[] files = Directory.GetFiles(srcDirPath, "*", SearchOption.AllDirectories);
                string relativeFilename = String.Empty;
                string dest = String.Empty;

                foreach (string f in files)
                {
                    relativeFilename = f.Substring(f.IndexOf(srcDirPath) + srcDirPath.Length + 1);
                    dest = Path.Combine(Config.AppRoot, relativeFilename);

                    File.Copy(f, dest, true);
                }

                Directory.Delete(srcDirPath, true);
                File.Delete(zipFilename);
            }
            catch (Exception e)
            {
                log.Error("SyncDb error", e);
            }
        }

        public virtual void DeleteLogs()
        {
            try
            {
                string logDir = Path.Combine(Config.AppRoot, "logs");
                string[] logFiles = Directory.GetFiles(logDir, "*", SearchOption.TopDirectoryOnly);

                int limit = Convert.ToInt32(DateTime.Now.AddDays(-7).ToString("yyyyMMdd"));
                Regex regex = new Regex(@"screen\d{8}|caller\d{8}|evaluator\d{8}|ic\d{8}|id\d{8}|mag\d{8}|peripheral\d{8}|printer\d{8}|app\d{8}|job\d{8}|update\d{8}", RegexOptions.Singleline);
                int day = 0;
                string path = String.Empty;

                foreach (string s in logFiles)
                {
                    if (regex.IsMatch(s))
                    {
                        day = Convert.ToInt32(s.Substring(s.Length - 8));

                        if (day < limit)
                        {
                            path = Path.Combine(logDir, s);
                            File.Delete(path);
                            log.InfoFormat("delete log file： {0}", path);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("DeleteLogs error", e);
            }
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
                jo["result"] = ErrorCode.Failure;
                log.Error("Callback Error", e);
            }
            finally
            {
                //if (null==scriptInvoker)
                //{
                //     scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
                //}

                //ScriptInvoker.ScriptInvoke(jo);
            }
        }
    }
}
