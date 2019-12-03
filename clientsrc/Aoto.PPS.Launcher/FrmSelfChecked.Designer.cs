namespace Aoto.PPS.Launcher
{
    partial class FrmSelfChecked
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSelfChecked));
            this.lblMessage = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.axPtrAx1 = new AxPtrAxLib.AxPtrAx();
            this.axIdcAx1 = new AxIdcAxLib.AxIdcAx();
            ((System.ComponentModel.ISupportInitialize)(this.axPtrAx1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axIdcAx1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMessage.Location = new System.Drawing.Point(19, 21);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(98, 14);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "开机自检中...";
            // 
            // txtMessage
            // 
            this.txtMessage.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtMessage.Location = new System.Drawing.Point(23, 54);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(0);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(311, 155);
            this.txtMessage.TabIndex = 2;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerDoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerRunWorkerCompleted);
            // 
            // axPtrAx1
            // 
            this.axPtrAx1.Enabled = true;
            this.axPtrAx1.Location = new System.Drawing.Point(275, 21);
            this.axPtrAx1.Name = "axPtrAx1";
            this.axPtrAx1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axPtrAx1.OcxState")));
            this.axPtrAx1.Size = new System.Drawing.Size(100, 50);
            this.axPtrAx1.TabIndex = 4;
            this.axPtrAx1.Visible = false;
            // 
            // axIdcAx1
            // 
            this.axIdcAx1.Enabled = true;
            this.axIdcAx1.Location = new System.Drawing.Point(150, 21);
            this.axIdcAx1.Name = "axIdcAx1";
            this.axIdcAx1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axIdcAx1.OcxState")));
            this.axIdcAx1.Size = new System.Drawing.Size(100, 50);
            this.axIdcAx1.TabIndex = 3;
            this.axIdcAx1.Visible = false;
            // 
            // FrmSelfChecked
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 233);
            this.Controls.Add(this.axPtrAx1);
            this.Controls.Add(this.axIdcAx1);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.lblMessage);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSelfChecked";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "取号终端系统";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmSelfCheckedLoad);
            ((System.ComponentModel.ISupportInitialize)(this.axPtrAx1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axIdcAx1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox txtMessage;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private AxIdcAxLib.AxIdcAx axIdcAx1;
        private AxPtrAxLib.AxPtrAx axPtrAx1;
    }
}