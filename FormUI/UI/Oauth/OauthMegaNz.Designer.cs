namespace FormUI.UI.Oauth
{
    partial class OauthMegaNz
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
            this.TB_email = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TB_pass = new System.Windows.Forms.TextBox();
            this.BT_Authencation = new System.Windows.Forms.Button();
            this.BT_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TB_email
            // 
            this.TB_email.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_email.Location = new System.Drawing.Point(135, 13);
            this.TB_email.Name = "TB_email";
            this.TB_email.Size = new System.Drawing.Size(240, 26);
            this.TB_email.TabIndex = 0;
            this.TB_email.TextChanged += new System.EventHandler(this.TB_pass_TextChanged);
            this.TB_email.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TB_pass_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(58, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Email:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "PassWord:";
            // 
            // TB_pass
            // 
            this.TB_pass.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_pass.Location = new System.Drawing.Point(135, 50);
            this.TB_pass.Name = "TB_pass";
            this.TB_pass.PasswordChar = '*';
            this.TB_pass.Size = new System.Drawing.Size(240, 26);
            this.TB_pass.TabIndex = 3;
            this.TB_pass.TextChanged += new System.EventHandler(this.TB_pass_TextChanged);
            this.TB_pass.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TB_pass_KeyDown);
            // 
            // BT_Authencation
            // 
            this.BT_Authencation.Enabled = false;
            this.BT_Authencation.Location = new System.Drawing.Point(29, 108);
            this.BT_Authencation.Name = "BT_Authencation";
            this.BT_Authencation.Size = new System.Drawing.Size(85, 25);
            this.BT_Authencation.TabIndex = 4;
            this.BT_Authencation.Text = "Authencation";
            this.BT_Authencation.UseVisualStyleBackColor = true;
            this.BT_Authencation.Click += new System.EventHandler(this.BT_Authencation_Click);
            // 
            // BT_Cancel
            // 
            this.BT_Cancel.Location = new System.Drawing.Point(290, 108);
            this.BT_Cancel.Name = "BT_Cancel";
            this.BT_Cancel.Size = new System.Drawing.Size(85, 25);
            this.BT_Cancel.TabIndex = 5;
            this.BT_Cancel.Text = "Cancel";
            this.BT_Cancel.UseVisualStyleBackColor = true;
            this.BT_Cancel.Click += new System.EventHandler(this.BT_Cancel_Click);
            // 
            // OauthMegaNz
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 145);
            this.Controls.Add(this.BT_Cancel);
            this.Controls.Add(this.BT_Authencation);
            this.Controls.Add(this.TB_pass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TB_email);
            this.Name = "OauthMegaNz";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OauthMegaNz";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TB_email;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TB_pass;
        private System.Windows.Forms.Button BT_Authencation;
        private System.Windows.Forms.Button BT_Cancel;
    }
}