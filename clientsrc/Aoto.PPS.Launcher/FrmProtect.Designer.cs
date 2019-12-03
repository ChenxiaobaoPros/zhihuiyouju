namespace Aoto.PPS.Launcher
{
    partial class FrmProtect
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStateTxt = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblProtextMess = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExportKey = new System.Windows.Forms.Button();
            this.btnActive = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblStateTxt);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.lblProtextMess);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(546, 140);
            this.panel1.TabIndex = 0;
            // 
            // lblStateTxt
            // 
            this.lblStateTxt.AutoSize = true;
            this.lblStateTxt.Font = new System.Drawing.Font("微软雅黑", 14.25F);
            this.lblStateTxt.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblStateTxt.Location = new System.Drawing.Point(194, 91);
            this.lblStateTxt.Name = "lblStateTxt";
            this.lblStateTxt.Size = new System.Drawing.Size(145, 25);
            this.lblStateTxt.TabIndex = 4;
            this.lblStateTxt.Text = "状态：未激活。";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Aoto.PPS.Launcher.Properties.Resources.bt;
            this.pictureBox1.Location = new System.Drawing.Point(14, 14);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(45, 35);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // lblProtextMess
            // 
            this.lblProtextMess.AutoSize = true;
            this.lblProtextMess.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblProtextMess.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblProtextMess.Location = new System.Drawing.Point(126, 91);
            this.lblProtextMess.Name = "lblProtextMess";
            this.lblProtextMess.Size = new System.Drawing.Size(297, 25);
            this.lblProtextMess.TabIndex = 2;
            this.lblProtextMess.Text = "激活码文件不对，请联系管理员。";
            this.lblProtextMess.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(35, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(508, 50);
            this.label1.TabIndex = 0;
            this.label1.Text = "     您的软件许可需要激活,请导出txt文本后发送给管理员，\r\n以获取激活文件。如已有激活文件，请点击激活继续。";
            // 
            // btnExportKey
            // 
            this.btnExportKey.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnExportKey.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.btnExportKey.Location = new System.Drawing.Point(73, 146);
            this.btnExportKey.Name = "btnExportKey";
            this.btnExportKey.Size = new System.Drawing.Size(97, 53);
            this.btnExportKey.TabIndex = 1;
            this.btnExportKey.Text = "导出txt文本";
            this.btnExportKey.UseVisualStyleBackColor = true;
            this.btnExportKey.Click += new System.EventHandler(this.btnExportKey_Click);
            // 
            // btnActive
            // 
            this.btnActive.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnActive.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnActive.Location = new System.Drawing.Point(225, 146);
            this.btnActive.Name = "btnActive";
            this.btnActive.Size = new System.Drawing.Size(97, 53);
            this.btnActive.TabIndex = 1;
            this.btnActive.Text = "激 活";
            this.btnActive.UseVisualStyleBackColor = true;
            this.btnActive.Click += new System.EventHandler(this.btnActive_Click);
            // 
            // btnClose
            // 
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClose.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.btnClose.Location = new System.Drawing.Point(377, 146);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(97, 53);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "退 出";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FrmProtect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 206);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnActive);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnExportKey);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmProtect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnActive;
        private System.Windows.Forms.Button btnExportKey;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblProtextMess;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblStateTxt;
    }
}