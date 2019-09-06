namespace SmartCardExampleCode {
    partial class frmMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pbBannerLogo = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblBanner = new System.Windows.Forms.Label();
            this.gbJob = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbImage = new System.Windows.Forms.CheckBox();
            this.lblImage = new System.Windows.Forms.Label();
            this.tbIPAddress = new System.Windows.Forms.TextBox();
            this.cboJobType = new System.Windows.Forms.ComboBox();
            this.lblJobType = new System.Windows.Forms.Label();
            this.lblIPAddress = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.lblMsg = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbBannerLogo)).BeginInit();
            this.gbJob.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbBannerLogo
            // 
            this.pbBannerLogo.BackColor = System.Drawing.Color.Black;
            this.pbBannerLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbBannerLogo.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.pbBannerLogo.Enabled = false;
            this.pbBannerLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbBannerLogo.Image")));
            this.pbBannerLogo.Location = new System.Drawing.Point(731, 18);
            this.pbBannerLogo.Margin = new System.Windows.Forms.Padding(0);
            this.pbBannerLogo.Name = "pbBannerLogo";
            this.pbBannerLogo.Size = new System.Drawing.Size(96, 37);
            this.pbBannerLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbBannerLogo.TabIndex = 89;
            this.pbBannerLogo.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.Font = new System.Drawing.Font("Arial", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(458, 28);
            this.label1.TabIndex = 92;
            this.label1.Text = "Smart Card Developer Demos 2.16.0004";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBanner
            // 
            this.lblBanner.BackColor = System.Drawing.Color.Black;
            this.lblBanner.Location = new System.Drawing.Point(0, 0);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Size = new System.Drawing.Size(907, 68);
            this.lblBanner.TabIndex = 88;
            this.lblBanner.Text = "label1";
            // 
            // gbJob
            // 
            this.gbJob.Controls.Add(this.label2);
            this.gbJob.Controls.Add(this.cbImage);
            this.gbJob.Controls.Add(this.lblImage);
            this.gbJob.Controls.Add(this.tbIPAddress);
            this.gbJob.Controls.Add(this.cboJobType);
            this.gbJob.Controls.Add(this.lblJobType);
            this.gbJob.Controls.Add(this.lblIPAddress);
            this.gbJob.Location = new System.Drawing.Point(10, 80);
            this.gbJob.Name = "gbJob";
            this.gbJob.Size = new System.Drawing.Size(883, 80);
            this.gbJob.TabIndex = 0;
            this.gbJob.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Location = new System.Drawing.Point(500, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(257, 18);
            this.label2.TabIndex = 8;
            this.label2.Text = "Enter an IP Address for testing EoE";
            // 
            // cbImage
            // 
            this.cbImage.AutoSize = true;
            this.cbImage.Location = new System.Drawing.Point(840, 32);
            this.cbImage.Name = "cbImage";
            this.cbImage.Size = new System.Drawing.Size(18, 17);
            this.cbImage.TabIndex = 4;
            this.cbImage.UseVisualStyleBackColor = true;
            // 
            // lblImage
            // 
            this.lblImage.AutoSize = true;
            this.lblImage.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImage.Location = new System.Drawing.Point(780, 30);
            this.lblImage.Name = "lblImage";
            this.lblImage.Size = new System.Drawing.Size(54, 18);
            this.lblImage.TabIndex = 7;
            this.lblImage.Text = "Image:";
            // 
            // tbIPAddress
            // 
            this.tbIPAddress.Location = new System.Drawing.Point(375, 30);
            this.tbIPAddress.Name = "tbIPAddress";
            this.tbIPAddress.Size = new System.Drawing.Size(118, 22);
            this.tbIPAddress.TabIndex = 3;
            // 
            // cboJobType
            // 
            this.cboJobType.FormattingEnabled = true;
            this.cboJobType.Items.AddRange(new object[] {
            "MIFARE",
            "PROX",
            "ATMEL",
            "UHF"});
            this.cboJobType.Location = new System.Drawing.Point(90, 30);
            this.cboJobType.Name = "cboJobType";
            this.cboJobType.Size = new System.Drawing.Size(170, 24);
            this.cboJobType.TabIndex = 1;
            this.cboJobType.Text = "MIFARE";
            this.cboJobType.SelectedIndexChanged += new System.EventHandler(this.cboJobType_SelectedIndexChanged);
            // 
            // lblJobType
            // 
            this.lblJobType.AutoSize = true;
            this.lblJobType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblJobType.Location = new System.Drawing.Point(10, 30);
            this.lblJobType.Name = "lblJobType";
            this.lblJobType.Size = new System.Drawing.Size(76, 18);
            this.lblJobType.TabIndex = 4;
            this.lblJobType.Text = "Job Type:";
            // 
            // lblIPAddress
            // 
            this.lblIPAddress.AutoSize = true;
            this.lblIPAddress.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIPAddress.ForeColor = System.Drawing.Color.Black;
            this.lblIPAddress.Location = new System.Drawing.Point(280, 30);
            this.lblIPAddress.Name = "lblIPAddress";
            this.lblIPAddress.Size = new System.Drawing.Size(87, 18);
            this.lblIPAddress.TabIndex = 0;
            this.lblIPAddress.Text = "IP Address:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnExit);
            this.groupBox1.Controls.Add(this.btnRun);
            this.groupBox1.Controls.Add(this.lblMsg);
            this.groupBox1.Location = new System.Drawing.Point(10, 165);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(883, 65);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(793, 21);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 25);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(712, 21);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 25);
            this.btnRun.TabIndex = 7;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // lblMsg
            // 
            this.lblMsg.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMsg.Location = new System.Drawing.Point(20, 24);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(680, 20);
            this.lblMsg.TabIndex = 2;
            this.lblMsg.Text = "lblMsg";
            this.lblMsg.Visible = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(908, 241);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbJob);
            this.Controls.Add(this.pbBannerLogo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblBanner);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pbBannerLogo)).EndInit();
            this.gbJob.ResumeLayout(false);
            this.gbJob.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbBannerLogo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblBanner;
        private System.Windows.Forms.GroupBox gbJob;
        private System.Windows.Forms.TextBox tbIPAddress;
        private System.Windows.Forms.Label lblIPAddress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.CheckBox cbImage;
        private System.Windows.Forms.Label lblImage;
        private System.Windows.Forms.ComboBox cboJobType;
        private System.Windows.Forms.Label lblJobType;
        private System.Windows.Forms.Label label2;
    }
}

