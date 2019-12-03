namespace Aoto.EMS.MultiSerBox
{
    partial class FrmFinger
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
            this.btnRegister = new System.Windows.Forms.Button();
            this.picRFiger = new System.Windows.Forms.PictureBox();
            this.labMessage = new System.Windows.Forms.Label();
            this.labFinger = new System.Windows.Forms.Label();
            this.picLFinger = new System.Windows.Forms.PictureBox();
            this.labCheckMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picRFiger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLFinger)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(12, 96);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(138, 59);
            this.btnRegister.TabIndex = 0;
            this.btnRegister.Text = "注册";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.BtnRegister_Click);
            // 
            // picRFiger
            // 
            this.picRFiger.Location = new System.Drawing.Point(175, 12);
            this.picRFiger.Name = "picRFiger";
            this.picRFiger.Size = new System.Drawing.Size(231, 245);
            this.picRFiger.TabIndex = 1;
            this.picRFiger.TabStop = false;
            // 
            // labMessage
            // 
            this.labMessage.AutoSize = true;
            this.labMessage.Location = new System.Drawing.Point(173, 302);
            this.labMessage.Name = "labMessage";
            this.labMessage.Size = new System.Drawing.Size(41, 12);
            this.labMessage.TabIndex = 2;
            this.labMessage.Text = "label1";
            // 
            // labFinger
            // 
            this.labFinger.AutoSize = true;
            this.labFinger.Location = new System.Drawing.Point(173, 275);
            this.labFinger.Name = "labFinger";
            this.labFinger.Size = new System.Drawing.Size(41, 12);
            this.labFinger.TabIndex = 3;
            this.labFinger.Text = "label1";
            // 
            // picLFinger
            // 
            this.picLFinger.Location = new System.Drawing.Point(702, 69);
            this.picLFinger.Name = "picLFinger";
            this.picLFinger.Size = new System.Drawing.Size(231, 245);
            this.picLFinger.TabIndex = 5;
            this.picLFinger.TabStop = false;
            // 
            // labCheckMessage
            // 
            this.labCheckMessage.AutoSize = true;
            this.labCheckMessage.Location = new System.Drawing.Point(801, 332);
            this.labCheckMessage.Name = "labCheckMessage";
            this.labCheckMessage.Size = new System.Drawing.Size(41, 12);
            this.labCheckMessage.TabIndex = 6;
            this.labCheckMessage.Text = "label1";
            // 
            // FrmFinger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 475);
            this.Controls.Add(this.labCheckMessage);
            this.Controls.Add(this.picLFinger);
            this.Controls.Add(this.labFinger);
            this.Controls.Add(this.labMessage);
            this.Controls.Add(this.picRFiger);
            this.Controls.Add(this.btnRegister);
            this.Name = "FrmFinger";
            this.Text = "FrmFinger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmFinger_FormClosing);
            this.Load += new System.EventHandler(this.FrmFinger_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picRFiger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLFinger)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.PictureBox picRFiger;
        private System.Windows.Forms.Label labMessage;
        private System.Windows.Forms.Label labFinger;
        private System.Windows.Forms.PictureBox picLFinger;
        private System.Windows.Forms.Label labCheckMessage;
    }
}