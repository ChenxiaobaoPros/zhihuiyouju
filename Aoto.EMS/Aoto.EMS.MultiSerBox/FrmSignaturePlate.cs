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
    public partial class FrmSignaturePlate : Form
    {
        public static FrmSignaturePlate instance;

        public static FrmSignaturePlate Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new FrmSignaturePlate();
                }
                return instance;
            }
        }

        private ISignaturePlate writingBoard;

        public FrmSignaturePlate()
        {
            InitializeComponent();
            
            writingBoard = AutofacContainer.ResolveNamed<ISignaturePlate>("signaturePlate");
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            writingBoard.SaveBoard();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.DoEvents();
            writingBoard.Clear();
        }

        private void FrmBoard_Load(object sender, EventArgs e)
        {
            //this.Left = 0;
            //this.Top = 0;
            this.Left = 1920 - Width;
            this.Top = 1080 - Height;
            BoardPanel.Show();
            writingBoard.Initialize(BoardPanel.Handle, BoardPanel.ClientSize.Height, BoardPanel.ClientSize.Width);
        }

        private void FrmBoard_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FrmBoard_MouseLeave(object sender, EventArgs e)
        {

        }
    }
}
