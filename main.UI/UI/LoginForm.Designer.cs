namespace Form.UI
{
    partial class LoginForm
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
            this.LB_User = new System.Windows.Forms.Label();
            this.LB_Pass = new System.Windows.Forms.Label();
            this.TB_User = new System.Windows.Forms.TextBox();
            this.TB_pass = new System.Windows.Forms.TextBox();
            this.BT_Login = new System.Windows.Forms.Button();
            this.BT_cancel = new System.Windows.Forms.Button();
            this.CB_autologin = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // LB_User
            // 
            this.LB_User.AutoSize = true;
            this.LB_User.Location = new System.Drawing.Point(12, 24);
            this.LB_User.MinimumSize = new System.Drawing.Size(80, 0);
            this.LB_User.Name = "LB_User";
            this.LB_User.Size = new System.Drawing.Size(80, 13);
            this.LB_User.TabIndex = 0;
            this.LB_User.Text = "User:";
            // 
            // LB_Pass
            // 
            this.LB_Pass.AutoSize = true;
            this.LB_Pass.Location = new System.Drawing.Point(12, 49);
            this.LB_Pass.MinimumSize = new System.Drawing.Size(80, 0);
            this.LB_Pass.Name = "LB_Pass";
            this.LB_Pass.Size = new System.Drawing.Size(80, 13);
            this.LB_Pass.TabIndex = 1;
            this.LB_Pass.Text = "Pass:";
            // 
            // TB_User
            // 
            this.TB_User.Location = new System.Drawing.Point(97, 21);
            this.TB_User.Name = "TB_User";
            this.TB_User.Size = new System.Drawing.Size(140, 20);
            this.TB_User.TabIndex = 0;
            this.TB_User.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_pass_KeyPress);
            // 
            // TB_pass
            // 
            this.TB_pass.Location = new System.Drawing.Point(97, 47);
            this.TB_pass.Name = "TB_pass";
            this.TB_pass.PasswordChar = '*';
            this.TB_pass.Size = new System.Drawing.Size(140, 20);
            this.TB_pass.TabIndex = 1;
            this.TB_pass.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_pass_KeyPress);
            // 
            // BT_Login
            // 
            this.BT_Login.Location = new System.Drawing.Point(15, 96);
            this.BT_Login.Name = "BT_Login";
            this.BT_Login.Size = new System.Drawing.Size(95, 23);
            this.BT_Login.TabIndex = 3;
            this.BT_Login.Text = "Login";
            this.BT_Login.UseVisualStyleBackColor = true;
            this.BT_Login.Click += new System.EventHandler(this.BT_Login_Click);
            // 
            // BT_cancel
            // 
            this.BT_cancel.Location = new System.Drawing.Point(141, 96);
            this.BT_cancel.Name = "BT_cancel";
            this.BT_cancel.Size = new System.Drawing.Size(96, 23);
            this.BT_cancel.TabIndex = 4;
            this.BT_cancel.Text = "Cancel";
            this.BT_cancel.UseVisualStyleBackColor = true;
            this.BT_cancel.Click += new System.EventHandler(this.BT_cancel_Click);
            // 
            // CB_autologin
            // 
            this.CB_autologin.AutoSize = true;
            this.CB_autologin.Location = new System.Drawing.Point(97, 73);
            this.CB_autologin.Name = "CB_autologin";
            this.CB_autologin.Size = new System.Drawing.Size(74, 17);
            this.CB_autologin.TabIndex = 2;
            this.CB_autologin.Text = "AutoLogin";
            this.CB_autologin.UseVisualStyleBackColor = true;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 127);
            this.Controls.Add(this.CB_autologin);
            this.Controls.Add(this.BT_cancel);
            this.Controls.Add(this.BT_Login);
            this.Controls.Add(this.TB_pass);
            this.Controls.Add(this.TB_User);
            this.Controls.Add(this.LB_Pass);
            this.Controls.Add(this.LB_User);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Login_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LB_User;
        private System.Windows.Forms.Label LB_Pass;
        private System.Windows.Forms.TextBox TB_User;
        private System.Windows.Forms.TextBox TB_pass;
        private System.Windows.Forms.Button BT_Login;
        private System.Windows.Forms.Button BT_cancel;
        private System.Windows.Forms.CheckBox CB_autologin;
    }
}