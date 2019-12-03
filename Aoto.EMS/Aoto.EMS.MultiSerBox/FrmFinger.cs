using Aoto.EMS.Infrastructure;
using Aoto.EMS.Infrastructure.ComponentModel;
using Aoto.EMS.Peripheral;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aoto.EMS.MultiSerBox
{
    public partial class FrmFinger : Form
    {
        public static FrmFinger instance;

        public static FrmFinger Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new FrmFinger();
                }
                return instance;
            }
        }
        private IFinger finger;

        public FrmFinger()
        {
            InitializeComponent();
            finger = AutofacContainer.ResolveNamed<IFinger>("finger");
            finger.RunCompletedEvent  += ShowFinger;
        }
        /// <summary>
        /// 将数组转换成彩色图片
        /// </summary>
        /// <param name="rawValues">图像的byte数组</param>
        /// <param name="width">图像的宽</param>
        /// <param name="height">图像的高</param>
        /// <returns>Bitmap对象</returns>
        public Bitmap ToColorBitmap(byte[] bytelist, int width, int height)
        {
            //// 申请目标位图的变量，并将其内存区域锁定
            try
            {
                MemoryStream ms1 = new MemoryStream(bytelist);
                Bitmap bitmap = (Bitmap)Image.FromStream(ms1);
                ms1.Close();
                return bitmap;

            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
        public StringBuilder regStringBuilder;

        private void ShowFinger(object sender, RunCompletedEventArgs e)
        {
            if (finger.FingerType == FingerType.RegisterFinger)
            {
                this.Invoke((EventHandler)delegate {
                    picRFiger.Image = ToColorBitmap((byte[])e.Result, picLFinger.ClientSize.Width, picLFinger.ClientSize.Height);
                    labFinger.Text = sender.ToString();
                    stringBuilders.Add((StringBuilder)sender);
                    if (stringBuilders.Count == 1)
                    {
                        labMessage.Text = "请再次录入指纹";
                    }
                    else if (stringBuilders.Count == 2)
                    {
                        labMessage.Text = "请三次录入指纹";
                    }
                    else if (stringBuilders.Count == 3)
                    {
                        regStringBuilder = finger.MakeFeatureToTemplate(stringBuilders);
                        if (regStringBuilder != null)
                        {
                            labMessage.Text = "录入指纹成功";
                            finger.FingerType = FingerType.ShowFinger;
                        }
                    }

                });
            }
            else
            {
                this.Invoke((EventHandler)delegate {
                    picLFinger.Image = ToColorBitmap((byte[])e.Result, picLFinger.ClientSize.Width, picLFinger.ClientSize.Height);
                    labCheckMessage.Text = sender.ToString();
                });
            }
        }
        private void FrmFinger_Load(object sender, EventArgs e)
        {
            finger.Initialize();
        }
        List<StringBuilder> stringBuilders = new List<StringBuilder>();
        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (finger.FingerType == FingerType.RegisterFinger)
            {
                if (MessageBox.Show("当前正在注册指纹,确认退出吗?退出后将不保存未完成指纹信息", "操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    stringBuilders.Clear();
                }
            }
            else
            {
                finger.FingerType = FingerType.RegisterFinger;
                labMessage.Text = "请录入指纹";
            }

        }

        private void FrmFinger_FormClosing(object sender, FormClosingEventArgs e)
        {
            finger.Dispose();
        }
    }
}
