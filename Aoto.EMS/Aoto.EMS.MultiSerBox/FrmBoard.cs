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
    public partial class FrmBoard : Form
    {
        public static FrmBoard instance;

        public static FrmBoard Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new FrmBoard();
                }
                return instance;
            }
        }

        private ISignaturePlate writingBoard;

        public FrmBoard()
        {
            InitializeComponent();
            writingBoard = AutofacContainer.ResolveNamed<ISignaturePlate>("writingBoard");
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
            BoardPanel.Show();
            writingBoard.Initialize(BoardPanel.Handle, BoardPanel.ClientSize.Height, BoardPanel.ClientSize.Width);
        }

        private void FrmBoard_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
