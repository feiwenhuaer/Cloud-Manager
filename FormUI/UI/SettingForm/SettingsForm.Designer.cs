namespace FormUI.UI.SettingForm
{
    partial class SettingsForm
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
            this.main_setting1 = new main_setting();
            this.BT_Save = new System.Windows.Forms.Button();
            this.BT_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // main_setting1
            // 
            this.main_setting1.Location = new System.Drawing.Point(12, 12);
            this.main_setting1.Name = "main_setting1";
            this.main_setting1.Size = new System.Drawing.Size(518, 200);
            this.main_setting1.TabIndex = 0;
            // 
            // BT_Save
            // 
            this.BT_Save.Location = new System.Drawing.Point(297, 218);
            this.BT_Save.Name = "BT_Save";
            this.BT_Save.Size = new System.Drawing.Size(75, 23);
            this.BT_Save.TabIndex = 1;
            this.BT_Save.Text = "Save";
            this.BT_Save.UseVisualStyleBackColor = true;
            this.BT_Save.Click += new System.EventHandler(this.BT_Save_Click);
            // 
            // BT_Cancel
            // 
            this.BT_Cancel.Location = new System.Drawing.Point(411, 218);
            this.BT_Cancel.Name = "BT_Cancel";
            this.BT_Cancel.Size = new System.Drawing.Size(75, 23);
            this.BT_Cancel.TabIndex = 2;
            this.BT_Cancel.Text = "Cancel";
            this.BT_Cancel.UseVisualStyleBackColor = true;
            this.BT_Cancel.Click += new System.EventHandler(this.BT_Cancel_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 251);
            this.Controls.Add(this.BT_Cancel);
            this.Controls.Add(this.BT_Save);
            this.Controls.Add(this.main_setting1);
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SettingsForm";
            this.ResumeLayout(false);

        }

        #endregion

        private main_setting main_setting1;
        private System.Windows.Forms.Button BT_Save;
        private System.Windows.Forms.Button BT_Cancel;
    }
}