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
    public partial class FrmFaceCamera : Form
    {
        public static FrmFaceCamera instance;

        public static FrmFaceCamera Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new FrmFaceCamera();
                }
                return instance;
            }
        }

        private IFaceCamera faceCamera;
        public FrmFaceCamera()
        {
            InitializeComponent();
            faceCamera = AutofacContainer.ResolveNamed<IFaceCamera>("faceCamera");
        }

        private void FrmFaceCamera_Load(object sender, EventArgs e)
        {
            int ret = faceCamera.InitCamera(pictureBox.Handle);

            string retStr = faceCamera.DisplayDVR();
        }
    }
}
