using Aoto.EMS.Infrastructure;
using Aoto.EMS.Peripheral;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aoto.EMS.MultiSerBox
{
    public partial class FrmHighMeter : Form
    {
        public static FrmHighMeter instance;

        public static FrmHighMeter Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new FrmHighMeter();
                }
                return instance;
            }
        }

        private IHighMeter highMeter;
        public FrmHighMeter()
        {
            InitializeComponent();
            highMeter = AutofacContainer.ResolveNamed<IHighMeter>("highMeter");
        }

        private void FrmHighMeter_Load(object sender, EventArgs e)
        {
            //this.Left = 0;
            //this.Top = 0;
            this.Left = 1920 - Width;
            this.Top = 1080 - Height;
            pictureBox.Show();
            highMeter.Initialize(pictureBox.Handle);
        }


    }
}
