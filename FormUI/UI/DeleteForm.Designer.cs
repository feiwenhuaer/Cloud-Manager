namespace FormUI.UI
{
    partial class DeleteForm
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
            this.BT_cancel = new System.Windows.Forms.Button();
            this.CB_autoclose = new System.Windows.Forms.CheckBox();
            this.TB = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BT_cancel
            // 
            this.BT_cancel.Location = new System.Drawing.Point(391, 171);
            this.BT_cancel.Name = "BT_cancel";
            this.BT_cancel.Size = new System.Drawing.Size(75, 23);
            this.BT_cancel.TabIndex = 0;
            this.BT_cancel.Text = "Cancel";
            this.BT_cancel.UseVisualStyleBackColor = true;
            this.BT_cancel.Click += new System.EventHandler(this.BT_cancel_Click);
            // 
            // CB_autoclose
            // 
            this.CB_autoclose.AutoSize = true;
            this.CB_autoclose.Checked = true;
            this.CB_autoclose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_autoclose.Location = new System.Drawing.Point(12, 177);
            this.CB_autoclose.Name = "CB_autoclose";
            this.CB_autoclose.Size = new System.Drawing.Size(77, 17);
            this.CB_autoclose.TabIndex = 1;
            this.CB_autoclose.Text = "Auto Close";
            this.CB_autoclose.UseVisualStyleBackColor = true;
            this.CB_autoclose.CheckedChanged += new System.EventHandler(this.CB_autoclose_CheckedChanged);
            // 
            // TB
            // 
            this.TB.Location = new System.Drawing.Point(12, 12);
            this.TB.Multiline = true;
            this.TB.Name = "TB";
            this.TB.ReadOnly = true;
            this.TB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB.Size = new System.Drawing.Size(454, 153);
            this.TB.TabIndex = 4;
            // 
            // DeleteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 206);
            this.Controls.Add(this.TB);
            this.Controls.Add(this.CB_autoclose);
            this.Controls.Add(this.BT_cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "DeleteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DeleteForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DeleteForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_cancel;
        private System.Windows.Forms.CheckBox CB_autoclose;
        private System.Windows.Forms.TextBox TB;
    }
}