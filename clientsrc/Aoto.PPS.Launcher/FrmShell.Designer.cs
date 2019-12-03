namespace Aoto.PPS.Launcher
{
    partial class FrmShell
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmShell));
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.button1 = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.axPtrAx2Print = new AxPtrAxLib.AxPtrAx();
            this.axEMVICAx2Mifare = new AxEMVICAxLib.AxEMVICAx();
            this.axIdcAx2Mifare = new AxIdcAxLib.AxIdcAx();
            ((System.ComponentModel.ISupportInitialize)(this.axPtrAx2Print)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axEMVICAx2Mifare)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axIdcAx2Mifare)).BeginInit();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.ScrollBarsEnabled = false;
            this.webBrowser.Size = new System.Drawing.Size(592, 299);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.WebBrowserNavigated);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(44, 97);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "签到";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(422, 20);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(170, 247);
            this.richTextBox2.TabIndex = 3;
            this.richTextBox2.Text = "";
            this.richTextBox2.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(44, 133);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "取号1";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(44, 162);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "退出";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(161, 97);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "关闭外设";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(161, 133);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 10;
            this.button5.Text = "打开外设";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(279, 97);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 11;
            this.button6.Text = "打印";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Visible = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // axPtrAx2Print
            // 
            this.axPtrAx2Print.Enabled = true;
            this.axPtrAx2Print.Location = new System.Drawing.Point(296, 20);
            this.axPtrAx2Print.Name = "axPtrAx2Print";
            this.axPtrAx2Print.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axPtrAx2Print.OcxState")));
            this.axPtrAx2Print.Size = new System.Drawing.Size(100, 50);
            this.axPtrAx2Print.TabIndex = 6;
            this.axPtrAx2Print.Visible = false;
            // 
            // axEMVICAx2Mifare
            // 
            this.axEMVICAx2Mifare.Enabled = true;
            this.axEMVICAx2Mifare.Location = new System.Drawing.Point(161, 20);
            this.axEMVICAx2Mifare.Name = "axEMVICAx2Mifare";
            this.axEMVICAx2Mifare.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axEMVICAx2Mifare.OcxState")));
            this.axEMVICAx2Mifare.Size = new System.Drawing.Size(100, 50);
            this.axEMVICAx2Mifare.TabIndex = 5;
            this.axEMVICAx2Mifare.Visible = false;
            // 
            // axIdcAx2Mifare
            // 
            this.axIdcAx2Mifare.Enabled = true;
            this.axIdcAx2Mifare.Location = new System.Drawing.Point(44, 20);
            this.axIdcAx2Mifare.Name = "axIdcAx2Mifare";
            this.axIdcAx2Mifare.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axIdcAx2Mifare.OcxState")));
            this.axIdcAx2Mifare.Size = new System.Drawing.Size(100, 50);
            this.axIdcAx2Mifare.TabIndex = 4;
            this.axIdcAx2Mifare.Visible = false;
            this.axIdcAx2Mifare.CardInserted += new System.EventHandler(this.axIdcAx2Mifare_CardInserted);
            this.axIdcAx2Mifare.ReadRawDataComplete += new AxIdcAxLib._DIdcAxEvents_ReadRawDataCompleteEventHandler(this.axIdcAx2Mifare_ReadRawDataComplete);
            this.axIdcAx2Mifare.CardTaken += new System.EventHandler(this.axIdcAx2Mifare_CardTaken);
            // 
            // FrmShell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 299);
            this.ControlBox = false;
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.axPtrAx2Print);
            this.Controls.Add(this.axEMVICAx2Mifare);
            this.Controls.Add(this.axIdcAx2Mifare);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.webBrowser);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmShell";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "潍坊银行填单机系统";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmShellClosed);
            this.Load += new System.EventHandler(this.FrmShellLoad);
            ((System.ComponentModel.ISupportInitialize)(this.axPtrAx2Print)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axEMVICAx2Mifare)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axIdcAx2Mifare)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private AxIdcAxLib.AxIdcAx axIdcAx2Mifare;
        private AxEMVICAxLib.AxEMVICAx axEMVICAx2Mifare;
        private AxPtrAxLib.AxPtrAx axPtrAx2Print;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
       
    }
}