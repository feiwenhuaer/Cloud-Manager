namespace FormUI.UI.SettingForm
{
    partial class main_setting
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.CB_AutoStartTransfer = new System.Windows.Forms.CheckBox();
      this.CB_shutdown = new System.Windows.Forms.CheckBox();
      this.NUD_group = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.NUD_item = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.CBB_lang = new System.Windows.Forms.ComboBox();
      this.CBB_ui = new System.Windows.Forms.ComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.NUD_buffer = new System.Windows.Forms.NumericUpDown();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.NUD_GDchunks = new System.Windows.Forms.NumericUpDown();
      this.label9 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.NUD_DBchunks = new System.Windows.Forms.NumericUpDown();
      this.label11 = new System.Windows.Forms.Label();
      this.TB_username = new System.Windows.Forms.TextBox();
      this.label12 = new System.Windows.Forms.Label();
      this.TB_oldpass = new System.Windows.Forms.TextBox();
      this.TB_newpass0 = new System.Windows.Forms.TextBox();
      this.label13 = new System.Windows.Forms.Label();
      this.TB_newpass1 = new System.Windows.Forms.TextBox();
      this.label14 = new System.Windows.Forms.Label();
      this.CB_autologin = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.NUD_group)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.NUD_item)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.NUD_buffer)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.NUD_GDchunks)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.NUD_DBchunks)).BeginInit();
      this.SuspendLayout();
      // 
      // CB_AutoStartTransfer
      // 
      this.CB_AutoStartTransfer.AutoSize = true;
      this.CB_AutoStartTransfer.Location = new System.Drawing.Point(3, 3);
      this.CB_AutoStartTransfer.Name = "CB_AutoStartTransfer";
      this.CB_AutoStartTransfer.Size = new System.Drawing.Size(109, 17);
      this.CB_AutoStartTransfer.TabIndex = 0;
      this.CB_AutoStartTransfer.Text = "Auto start transfer";
      this.CB_AutoStartTransfer.UseVisualStyleBackColor = true;
      // 
      // CB_shutdown
      // 
      this.CB_shutdown.AutoSize = true;
      this.CB_shutdown.Location = new System.Drawing.Point(3, 26);
      this.CB_shutdown.Name = "CB_shutdown";
      this.CB_shutdown.Size = new System.Drawing.Size(177, 17);
      this.CB_shutdown.TabIndex = 1;
      this.CB_shutdown.Text = "Shutdown computer when finish";
      this.CB_shutdown.UseVisualStyleBackColor = true;
      // 
      // NUD_group
      // 
      this.NUD_group.Location = new System.Drawing.Point(172, 133);
      this.NUD_group.Name = "NUD_group";
      this.NUD_group.Size = new System.Drawing.Size(44, 20);
      this.NUD_group.TabIndex = 2;
      this.NUD_group.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(2, 135);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(164, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "Max groups download same time:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(-1, 161);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(156, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "Max items download same time:";
      // 
      // NUD_item
      // 
      this.NUD_item.Location = new System.Drawing.Point(172, 159);
      this.NUD_item.Name = "NUD_item";
      this.NUD_item.Size = new System.Drawing.Size(44, 20);
      this.NUD_item.TabIndex = 4;
      this.NUD_item.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(-1, 78);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(58, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Language:";
      // 
      // CBB_lang
      // 
      this.CBB_lang.FormattingEnabled = true;
      this.CBB_lang.Location = new System.Drawing.Point(66, 75);
      this.CBB_lang.Name = "CBB_lang";
      this.CBB_lang.Size = new System.Drawing.Size(150, 21);
      this.CBB_lang.TabIndex = 7;
      // 
      // CBB_ui
      // 
      this.CBB_ui.FormattingEnabled = true;
      this.CBB_ui.Location = new System.Drawing.Point(66, 102);
      this.CBB_ui.Name = "CBB_ui";
      this.CBB_ui.Size = new System.Drawing.Size(150, 21);
      this.CBB_ui.TabIndex = 9;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(-1, 105);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(21, 13);
      this.label4.TabIndex = 8;
      this.label4.Text = "UI:";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(233, 5);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(59, 13);
      this.label5.TabIndex = 12;
      this.label5.Text = "Buffer size:";
      // 
      // NUD_buffer
      // 
      this.NUD_buffer.Location = new System.Drawing.Point(300, 3);
      this.NUD_buffer.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
      this.NUD_buffer.Minimum = new decimal(new int[] {
            16,
            0,
            0,
            0});
      this.NUD_buffer.Name = "NUD_buffer";
      this.NUD_buffer.Size = new System.Drawing.Size(61, 20);
      this.NUD_buffer.TabIndex = 11;
      this.NUD_buffer.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(367, 4);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(20, 13);
      this.label6.TabIndex = 13;
      this.label6.Text = "Kb";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(494, 30);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(22, 13);
      this.label7.TabIndex = 16;
      this.label7.Text = "Mb";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(233, 30);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(195, 13);
      this.label8.TabIndex = 15;
      this.label8.Text = "GoogleDrive upload length chunks size:";
      // 
      // NUD_GDchunks
      // 
      this.NUD_GDchunks.Location = new System.Drawing.Point(434, 28);
      this.NUD_GDchunks.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
      this.NUD_GDchunks.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
      this.NUD_GDchunks.Name = "NUD_GDchunks";
      this.NUD_GDchunks.Size = new System.Drawing.Size(54, 20);
      this.NUD_GDchunks.TabIndex = 14;
      this.NUD_GDchunks.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(494, 56);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(22, 13);
      this.label9.TabIndex = 19;
      this.label9.Text = "Mb";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(233, 56);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(176, 13);
      this.label10.TabIndex = 18;
      this.label10.Text = "Dropbox upload length chunks size:";
      // 
      // NUD_DBchunks
      // 
      this.NUD_DBchunks.Location = new System.Drawing.Point(434, 54);
      this.NUD_DBchunks.Maximum = new decimal(new int[] {
            150,
            0,
            0,
            0});
      this.NUD_DBchunks.Minimum = new decimal(new int[] {
            25,
            0,
            0,
            0});
      this.NUD_DBchunks.Name = "NUD_DBchunks";
      this.NUD_DBchunks.Size = new System.Drawing.Size(54, 20);
      this.NUD_DBchunks.TabIndex = 17;
      this.NUD_DBchunks.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(233, 83);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(60, 13);
      this.label11.TabIndex = 20;
      this.label11.Text = "UserName:";
      // 
      // TB_username
      // 
      this.TB_username.Location = new System.Drawing.Point(300, 80);
      this.TB_username.Name = "TB_username";
      this.TB_username.Size = new System.Drawing.Size(217, 20);
      this.TB_username.TabIndex = 21;
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(236, 107);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(51, 13);
      this.label12.TabIndex = 22;
      this.label12.Text = "Old pass:";
      // 
      // TB_oldpass
      // 
      this.TB_oldpass.Location = new System.Drawing.Point(300, 106);
      this.TB_oldpass.Name = "TB_oldpass";
      this.TB_oldpass.PasswordChar = '*';
      this.TB_oldpass.Size = new System.Drawing.Size(217, 20);
      this.TB_oldpass.TabIndex = 23;
      this.TB_oldpass.TextChanged += new System.EventHandler(this.TB_oldpass_TextChanged);
      // 
      // TB_newpass0
      // 
      this.TB_newpass0.Enabled = false;
      this.TB_newpass0.Location = new System.Drawing.Point(299, 132);
      this.TB_newpass0.Name = "TB_newpass0";
      this.TB_newpass0.PasswordChar = '*';
      this.TB_newpass0.Size = new System.Drawing.Size(217, 20);
      this.TB_newpass0.TabIndex = 25;
      this.TB_newpass0.TextChanged += new System.EventHandler(this.TB_newpass0_TextChanged);
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(239, 133);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(57, 13);
      this.label13.TabIndex = 24;
      this.label13.Text = "New pass:";
      // 
      // TB_newpass1
      // 
      this.TB_newpass1.Enabled = false;
      this.TB_newpass1.Location = new System.Drawing.Point(299, 158);
      this.TB_newpass1.Name = "TB_newpass1";
      this.TB_newpass1.PasswordChar = '*';
      this.TB_newpass1.Size = new System.Drawing.Size(217, 20);
      this.TB_newpass1.TabIndex = 27;
      this.TB_newpass1.TextChanged += new System.EventHandler(this.TB_newpass0_TextChanged);
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(236, 161);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(57, 13);
      this.label14.TabIndex = 28;
      this.label14.Text = "New pass:";
      // 
      // CB_autologin
      // 
      this.CB_autologin.AutoSize = true;
      this.CB_autologin.Location = new System.Drawing.Point(3, 50);
      this.CB_autologin.Name = "CB_autologin";
      this.CB_autologin.Size = new System.Drawing.Size(73, 17);
      this.CB_autologin.TabIndex = 29;
      this.CB_autologin.Text = "Auto login";
      this.CB_autologin.UseVisualStyleBackColor = true;
      // 
      // main_setting
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.CB_autologin);
      this.Controls.Add(this.label14);
      this.Controls.Add(this.TB_newpass1);
      this.Controls.Add(this.TB_newpass0);
      this.Controls.Add(this.label13);
      this.Controls.Add(this.TB_oldpass);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.TB_username);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.NUD_DBchunks);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.NUD_GDchunks);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.NUD_buffer);
      this.Controls.Add(this.CBB_ui);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.CBB_lang);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.NUD_item);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.NUD_group);
      this.Controls.Add(this.CB_shutdown);
      this.Controls.Add(this.CB_AutoStartTransfer);
      this.Name = "main_setting";
      this.Size = new System.Drawing.Size(520, 185);
      this.Load += new System.EventHandler(this.main_setting_Load);
      ((System.ComponentModel.ISupportInitialize)(this.NUD_group)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.NUD_item)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.NUD_buffer)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.NUD_GDchunks)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.NUD_DBchunks)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox CB_AutoStartTransfer;
        public System.Windows.Forms.CheckBox CB_shutdown;
        public System.Windows.Forms.NumericUpDown NUD_group;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.NumericUpDown NUD_item;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox CBB_lang;
        public System.Windows.Forms.ComboBox CBB_ui;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.NumericUpDown NUD_buffer;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.NumericUpDown NUD_GDchunks;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.NumericUpDown NUD_DBchunks;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox TB_username;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox TB_oldpass;
        private System.Windows.Forms.TextBox TB_newpass0;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox TB_newpass1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox CB_autologin;
    }
}

