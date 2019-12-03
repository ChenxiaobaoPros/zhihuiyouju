using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Aoto.PPS.Infrastructure.Configuration;
using Aoto.PPS.Infrastructure.Utils;
using log4net;

namespace Aoto.PPS.Launcher
{
    public partial class FrmProtect : Form
    {
        private static ILog log = LogManager.GetLogger("app");
        private static FrmProtect instance;

        public FrmProtect()
        {
            InitializeComponent();

            Text = Config.App.Name + " " + Config.App.Version;

            if (File.Exists(Config.AppIconFilePath))
            {
                Icon = new Icon(Config.AppIconFilePath);
            }
        }

        public static FrmProtect Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new FrmProtect();
                }

                return instance;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ClassStyle = param.ClassStyle | 0x200;
                return param;
            }
        }


        /// <summary>
        /// 打开文件控件
        /// </summary>
        private void ShowOpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "激活码文件|*.dat";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            
            // 激活标记
            bool activeSign = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"TempPPF\protectppf.dat")))
                    {
                        File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"TempPPF\protectppf.dat"));
                    }

                    activeSign = CodeRegister.VerifyDeviceCode(openFileDialog.FileName);

                    File.SetAttributes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"TempPPF\protectppf.dat"), FileAttributes.Hidden);

                }
                catch
                {
                    activeSign = false;
                    log.DebugFormat("读取激活文件异常!");
                }

                if (!activeSign)
                {
                    lblStateTxt.Visible = false;
                    lblProtextMess.Visible = true;
                }
                else
                {
                    // 效验成功
                    this.DialogResult = DialogResult.Yes;
                    this.Close();
                }

            }
        }

        /// <summary>
        /// 保存文件控件
        /// </summary>
        private void ShowSaveFileDialog()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "key文件（*.txt）|";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            //默认文件名称
            sfd.FileName = Text + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.IO.FileStream fs = (System.IO.FileStream)sfd.OpenFile();
                    StreamWriter swWriter = new StreamWriter(fs,Encoding.UTF8);
                    //写入数据
                    string deviceInfo=DeviceInfo.Instance().CpuID + DeviceInfo.Instance().MacAddress + DeviceInfo.Instance().DiskID + DeviceInfo.Instance().SystemType;

                    string deviceInfoMd5=CodeRegister.GetMd5("南京奥拓电子" + deviceInfo);

                    string curMachineCode = CodeRegister.GetMd5(DateTime.Now.ToString()) + "_" + deviceInfoMd5;

                    string devEds = CodeRegister.Encrypt("0587aoto南京奥拓", curMachineCode);

                    swWriter.WriteLine(devEds);
                    swWriter.Close();
                    fs.Close();
                }
                catch
                {
                    log.DebugFormat("key文件写入异常!");
                }

                log.DebugFormat("导出key文件：" + sfd.FileName+" 完毕.");
            }
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            ShowOpenFileDialog();
        }

        private void btnExportKey_Click(object sender, EventArgs e)
        {
            ShowSaveFileDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }






    }
}
