namespace FormUI.UI.Oauth
{
    partial class FormSellectOauth
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
            this.PB_GoogleDrive = new System.Windows.Forms.PictureBox();
            this.PB_Mega = new System.Windows.Forms.PictureBox();
            this.PB_Dropbox = new System.Windows.Forms.PictureBox();
            this.BT_Cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PB_GoogleDrive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mega)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Dropbox)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_GoogleDrive
            // 
            this.PB_GoogleDrive.Image = CloudManagerGeneralLib.Properties.Resources.Google_Drive_Icon256x256;
            this.PB_GoogleDrive.Location = new System.Drawing.Point(12, 12);
            this.PB_GoogleDrive.Name = "PB_GoogleDrive";
            this.PB_GoogleDrive.Size = new System.Drawing.Size(128, 128);
            this.PB_GoogleDrive.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_GoogleDrive.TabIndex = 0;
            this.PB_GoogleDrive.TabStop = false;
            this.PB_GoogleDrive.Click += new System.EventHandler(this.PB_GoogleDrive_Click);
            // 
            // PB_Mega
            // 
            this.PB_Mega.Image = CloudManagerGeneralLib.Properties.Resources.MegaSync;
            this.PB_Mega.Location = new System.Drawing.Point(146, 12);
            this.PB_Mega.Name = "PB_Mega";
            this.PB_Mega.Size = new System.Drawing.Size(128, 128);
            this.PB_Mega.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_Mega.TabIndex = 1;
            this.PB_Mega.TabStop = false;
            this.PB_Mega.Click += new System.EventHandler(this.PB_Mega_Click);
            // 
            // PB_Dropbox
            // 
            this.PB_Dropbox.Image = CloudManagerGeneralLib.Properties.Resources.Dropbox256x256;
            this.PB_Dropbox.Location = new System.Drawing.Point(280, 12);
            this.PB_Dropbox.Name = "PB_Dropbox";
            this.PB_Dropbox.Size = new System.Drawing.Size(128, 128);
            this.PB_Dropbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_Dropbox.TabIndex = 2;
            this.PB_Dropbox.TabStop = false;
            this.PB_Dropbox.Click += new System.EventHandler(this.PB_Dropbox_Click);
            // 
            // BT_Cancel
            // 
            this.BT_Cancel.Location = new System.Drawing.Point(171, 158);
            this.BT_Cancel.Name = "BT_Cancel";
            this.BT_Cancel.Size = new System.Drawing.Size(75, 23);
            this.BT_Cancel.TabIndex = 3;
            this.BT_Cancel.Text = "Cancel";
            this.BT_Cancel.UseVisualStyleBackColor = true;
            this.BT_Cancel.Click += new System.EventHandler(this.BT_Cancel_Click);
            // 
            // FormOauth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(424, 193);
            this.Controls.Add(this.BT_Cancel);
            this.Controls.Add(this.PB_Dropbox);
            this.Controls.Add(this.PB_Mega);
            this.Controls.Add(this.PB_GoogleDrive);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOauth";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormOauth";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.PB_GoogleDrive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mega)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Dropbox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PB_GoogleDrive;
        private System.Windows.Forms.PictureBox PB_Mega;
        private System.Windows.Forms.PictureBox PB_Dropbox;
        private System.Windows.Forms.Button BT_Cancel;
    }
}