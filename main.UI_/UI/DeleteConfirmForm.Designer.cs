namespace Form.UI
{
    partial class DeleteConfirmForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.CB_pernament = new System.Windows.Forms.CheckBox();
            this.BT_yes = new System.Windows.Forms.Button();
            this.BT_cancel = new System.Windows.Forms.Button();
            this.TB = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, -2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(344, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Are you sure you want to delete it?";
            // 
            // CB_pernament
            // 
            this.CB_pernament.AutoSize = true;
            this.CB_pernament.Location = new System.Drawing.Point(13, 184);
            this.CB_pernament.Name = "CB_pernament";
            this.CB_pernament.Size = new System.Drawing.Size(114, 17);
            this.CB_pernament.TabIndex = 1;
            this.CB_pernament.Text = "Permanent  Delete";
            this.CB_pernament.UseVisualStyleBackColor = true;
            // 
            // BT_yes
            // 
            this.BT_yes.Location = new System.Drawing.Point(13, 201);
            this.BT_yes.Name = "BT_yes";
            this.BT_yes.Size = new System.Drawing.Size(75, 23);
            this.BT_yes.TabIndex = 2;
            this.BT_yes.Text = "Yes";
            this.BT_yes.UseVisualStyleBackColor = true;
            this.BT_yes.Click += new System.EventHandler(this.BT_yes_Click);
            // 
            // BT_cancel
            // 
            this.BT_cancel.Location = new System.Drawing.Point(255, 201);
            this.BT_cancel.Name = "BT_cancel";
            this.BT_cancel.Size = new System.Drawing.Size(75, 23);
            this.BT_cancel.TabIndex = 3;
            this.BT_cancel.Text = "Cancel";
            this.BT_cancel.UseVisualStyleBackColor = true;
            this.BT_cancel.Click += new System.EventHandler(this.BT_cancel_Click);
            // 
            // TB
            // 
            this.TB.Location = new System.Drawing.Point(1, 25);
            this.TB.Multiline = true;
            this.TB.Name = "TB";
            this.TB.ReadOnly = true;
            this.TB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB.Size = new System.Drawing.Size(358, 153);
            this.TB.TabIndex = 4;
            // 
            // DeleteConfirmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 227);
            this.Controls.Add(this.TB);
            this.Controls.Add(this.BT_cancel);
            this.Controls.Add(this.BT_yes);
            this.Controls.Add(this.CB_pernament);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "DeleteConfirmForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DeleteConfirmForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BT_yes;
        private System.Windows.Forms.Button BT_cancel;
        public System.Windows.Forms.CheckBox CB_pernament;
        public System.Windows.Forms.TextBox TB;
    }
}