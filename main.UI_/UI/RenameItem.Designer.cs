namespace Form.UI
{
    partial class RenameItem
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
            this.TB_oldname = new System.Windows.Forms.TextBox();
            this.TB_newname = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BT_change = new System.Windows.Forms.Button();
            this.BT_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Old name: ";
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseDown);
            this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseMove);
            this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseUp);
            // 
            // TB_oldname
            // 
            this.TB_oldname.Location = new System.Drawing.Point(79, 6);
            this.TB_oldname.Name = "TB_oldname";
            this.TB_oldname.ReadOnly = true;
            this.TB_oldname.Size = new System.Drawing.Size(255, 20);
            this.TB_oldname.TabIndex = 1;
            // 
            // TB_newname
            // 
            this.TB_newname.Location = new System.Drawing.Point(79, 32);
            this.TB_newname.Name = "TB_newname";
            this.TB_newname.Size = new System.Drawing.Size(255, 20);
            this.TB_newname.TabIndex = 2;
            this.TB_newname.TextChanged += new System.EventHandler(this.TB_newname_TextChanged);
            this.TB_newname.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TB_newname_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "New name:";
            this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseDown);
            this.label2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseMove);
            this.label2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseUp);
            // 
            // BT_change
            // 
            this.BT_change.Enabled = false;
            this.BT_change.Location = new System.Drawing.Point(60, 58);
            this.BT_change.Name = "BT_change";
            this.BT_change.Size = new System.Drawing.Size(75, 23);
            this.BT_change.TabIndex = 4;
            this.BT_change.Text = "Change";
            this.BT_change.UseVisualStyleBackColor = true;
            this.BT_change.Click += new System.EventHandler(this.BT_change_Click);
            // 
            // BT_cancel
            // 
            this.BT_cancel.Location = new System.Drawing.Point(195, 58);
            this.BT_cancel.Name = "BT_cancel";
            this.BT_cancel.Size = new System.Drawing.Size(75, 23);
            this.BT_cancel.TabIndex = 5;
            this.BT_cancel.Text = "Cancel";
            this.BT_cancel.UseVisualStyleBackColor = true;
            this.BT_cancel.Click += new System.EventHandler(this.BT_cancel_Click);
            // 
            // RenameItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 91);
            this.Controls.Add(this.BT_cancel);
            this.Controls.Add(this.BT_change);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TB_newname);
            this.Controls.Add(this.TB_oldname);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenameItem";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RenameItem";
            this.Load += new System.EventHandler(this.RenameItem_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TB_newname;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BT_change;
        private System.Windows.Forms.Button BT_cancel;
        private System.Windows.Forms.TextBox TB_oldname;
    }
}