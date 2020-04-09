namespace khwkit_tools
{
    partial class MainForm
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
            this.gbSummary = new System.Windows.Forms.GroupBox();
            this.btnFetchSummary = new System.Windows.Forms.Button();
            this.txHwkitIp = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txSoftVersion = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txHwVersion = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txLoalIp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSetupTv = new System.Windows.Forms.Button();
            this.btnReportTvId = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txTvId = new System.Windows.Forms.TextBox();
            this.txSN = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tpIDReaderBtnGetConfig = new System.Windows.Forms.Button();
            this.tpIDReaderSaveConfig = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.gbSummary.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSummary
            // 
            this.gbSummary.Controls.Add(this.btnFetchSummary);
            this.gbSummary.Controls.Add(this.txHwkitIp);
            this.gbSummary.Controls.Add(this.label8);
            this.gbSummary.Controls.Add(this.txSoftVersion);
            this.gbSummary.Controls.Add(this.label7);
            this.gbSummary.Controls.Add(this.txHwVersion);
            this.gbSummary.Controls.Add(this.label6);
            this.gbSummary.Controls.Add(this.txLoalIp);
            this.gbSummary.Controls.Add(this.label3);
            this.gbSummary.Controls.Add(this.btnSetupTv);
            this.gbSummary.Controls.Add(this.btnReportTvId);
            this.gbSummary.Controls.Add(this.label2);
            this.gbSummary.Controls.Add(this.txTvId);
            this.gbSummary.Controls.Add(this.txSN);
            this.gbSummary.Controls.Add(this.label1);
            this.gbSummary.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbSummary.Location = new System.Drawing.Point(0, 0);
            this.gbSummary.Name = "gbSummary";
            this.gbSummary.Size = new System.Drawing.Size(936, 224);
            this.gbSummary.TabIndex = 0;
            this.gbSummary.TabStop = false;
            this.gbSummary.Text = "基本信息";
            // 
            // btnFetchSummary
            // 
            this.btnFetchSummary.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnFetchSummary.Location = new System.Drawing.Point(569, 33);
            this.btnFetchSummary.Name = "btnFetchSummary";
            this.btnFetchSummary.Size = new System.Drawing.Size(147, 27);
            this.btnFetchSummary.TabIndex = 14;
            this.btnFetchSummary.Text = "获取信息";
            this.btnFetchSummary.UseVisualStyleBackColor = true;
            this.btnFetchSummary.Click += new System.EventHandler(this.btnFetchSummary_Click);
            // 
            // txHwkitIp
            // 
            this.txHwkitIp.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txHwkitIp.Location = new System.Drawing.Point(257, 36);
            this.txHwkitIp.Name = "txHwkitIp";
            this.txHwkitIp.Size = new System.Drawing.Size(302, 22);
            this.txHwkitIp.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(167, 40);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 14);
            this.label8.TabIndex = 12;
            this.label8.Text = "外设服务IP:";
            // 
            // txSoftVersion
            // 
            this.txSoftVersion.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txSoftVersion.Location = new System.Drawing.Point(562, 114);
            this.txSoftVersion.Name = "txSoftVersion";
            this.txSoftVersion.ReadOnly = true;
            this.txSoftVersion.Size = new System.Drawing.Size(154, 22);
            this.txSoftVersion.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(432, 119);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(126, 14);
            this.label7.TabIndex = 10;
            this.label7.Text = "外设服务软件版本:";
            // 
            // txHwVersion
            // 
            this.txHwVersion.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txHwVersion.Location = new System.Drawing.Point(257, 114);
            this.txHwVersion.Name = "txHwVersion";
            this.txHwVersion.ReadOnly = true;
            this.txHwVersion.Size = new System.Drawing.Size(154, 22);
            this.txHwVersion.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(139, 119);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 14);
            this.label6.TabIndex = 8;
            this.label6.Text = "扩展版硬件版本:";
            // 
            // txLoalIp
            // 
            this.txLoalIp.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txLoalIp.Location = new System.Drawing.Point(562, 79);
            this.txLoalIp.Name = "txLoalIp";
            this.txLoalIp.ReadOnly = true;
            this.txLoalIp.Size = new System.Drawing.Size(154, 22);
            this.txLoalIp.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(503, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 14);
            this.label3.TabIndex = 6;
            this.label3.Text = "本机IP:";
            // 
            // btnSetupTv
            // 
            this.btnSetupTv.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnSetupTv.Location = new System.Drawing.Point(416, 156);
            this.btnSetupTv.Name = "btnSetupTv";
            this.btnSetupTv.Size = new System.Drawing.Size(143, 27);
            this.btnSetupTv.TabIndex = 5;
            this.btnSetupTv.Text = "安装Teamviewer";
            this.btnSetupTv.UseVisualStyleBackColor = true;
            this.btnSetupTv.Click += new System.EventHandler(this.btnSetupTv_Click);
            // 
            // btnReportTvId
            // 
            this.btnReportTvId.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnReportTvId.Location = new System.Drawing.Point(579, 156);
            this.btnReportTvId.Name = "btnReportTvId";
            this.btnReportTvId.Size = new System.Drawing.Size(139, 26);
            this.btnReportTvId.TabIndex = 4;
            this.btnReportTvId.Text = "上传TeamviewerId";
            this.btnReportTvId.UseVisualStyleBackColor = true;
            this.btnReportTvId.Click += new System.EventHandler(this.btnReportTvId_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(147, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 14);
            this.label2.TabIndex = 3;
            this.label2.Text = "Teamviewer ID:";
            // 
            // txTvId
            // 
            this.txTvId.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txTvId.Location = new System.Drawing.Point(258, 158);
            this.txTvId.Name = "txTvId";
            this.txTvId.Size = new System.Drawing.Size(154, 22);
            this.txTvId.TabIndex = 2;
            // 
            // txSN
            // 
            this.txSN.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txSN.Location = new System.Drawing.Point(257, 79);
            this.txSN.Name = "txSN";
            this.txSN.ReadOnly = true;
            this.txSN.Size = new System.Drawing.Size(154, 22);
            this.txSN.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(224, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "SN:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.tabControl1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 224);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(936, 594);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "配置";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.ItemSize = new System.Drawing.Size(100, 40);
            this.tabControl1.Location = new System.Drawing.Point(3, 18);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(930, 398);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 44);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(922, 350);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "系统信息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 44);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(922, 350);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "身份证阅读器";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox3.Location = new System.Drawing.Point(3, 72);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(414, 275);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "参数";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tpIDReaderSaveConfig);
            this.panel1.Controls.Add(this.tpIDReaderBtnGetConfig);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.comboBox2);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(916, 69);
            this.panel1.TabIndex = 10;
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.comboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.ItemHeight = 25;
            this.comboBox1.Location = new System.Drawing.Point(88, 15);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(252, 31);
            this.comboBox1.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 14);
            this.label4.TabIndex = 1;
            this.label4.Text = "品牌:";
            // 
            // comboBox2
            // 
            this.comboBox2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.comboBox2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.ItemHeight = 25;
            this.comboBox2.Location = new System.Drawing.Point(404, 14);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(252, 31);
            this.comboBox2.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(358, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 14);
            this.label5.TabIndex = 3;
            this.label5.Text = "型号:";
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 44);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(922, 350);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "收发卡机";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 44);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(922, 429);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "小票打印机";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 44);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(922, 429);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "二维码扫描器";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            this.tabPage6.Location = new System.Drawing.Point(4, 44);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(922, 429);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "房卡读写卡器";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // tabPage7
            // 
            this.tabPage7.Location = new System.Drawing.Point(4, 44);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(922, 429);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "PSB";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(417, 72);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(502, 275);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "功能测试";
            // 
            // tpIDReaderBtnGetConfig
            // 
            this.tpIDReaderBtnGetConfig.Location = new System.Drawing.Point(680, 13);
            this.tpIDReaderBtnGetConfig.Name = "tpIDReaderBtnGetConfig";
            this.tpIDReaderBtnGetConfig.Size = new System.Drawing.Size(108, 39);
            this.tpIDReaderBtnGetConfig.TabIndex = 5;
            this.tpIDReaderBtnGetConfig.Text = "获取配置";
            this.tpIDReaderBtnGetConfig.UseVisualStyleBackColor = true;
            // 
            // tpIDReaderSaveConfig
            // 
            this.tpIDReaderSaveConfig.Location = new System.Drawing.Point(794, 13);
            this.tpIDReaderSaveConfig.Name = "tpIDReaderSaveConfig";
            this.tpIDReaderSaveConfig.Size = new System.Drawing.Size(108, 39);
            this.tpIDReaderSaveConfig.TabIndex = 6;
            this.tpIDReaderSaveConfig.Text = "保存配置";
            this.tpIDReaderSaveConfig.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.richTextBox1);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox4.Location = new System.Drawing.Point(3, 422);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(930, 169);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "输出信息";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 18);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(924, 148);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 36);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 36);
            this.button1.TabIndex = 0;
            this.button1.Text = "状态检测";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(11, 95);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(167, 36);
            this.button2.TabIndex = 1;
            this.button2.Text = "读取二代证";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(11, 154);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(167, 36);
            this.button3.TabIndex = 2;
            this.button3.Text = "取消读取二代证";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(936, 818);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gbSummary);
            this.Font = new System.Drawing.Font("宋体", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            this.MinimumSize = new System.Drawing.Size(702, 526);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "自主机外设设置";
            this.gbSummary.ResumeLayout(false);
            this.gbSummary.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSummary;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txSN;
        private System.Windows.Forms.TextBox txTvId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnReportTvId;
        private System.Windows.Forms.Button btnSetupTv;
        private System.Windows.Forms.TextBox txLoalIp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txHwVersion;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txSoftVersion;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txHwkitIp;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnFetchSummary;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button tpIDReaderBtnGetConfig;
        private System.Windows.Forms.Button tpIDReaderSaveConfig;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}