namespace Form.UI
{
    partial class UC_Lv_ud
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
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TP_processing = new System.Windows.Forms.TabPage();
            this.TP_done = new System.Windows.Forms.TabPage();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.waitingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.errorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceWaitingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numberOfParallelDownloadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl1.Controls.Add(this.TP_processing);
            this.tabControl1.Controls.Add(this.TP_done);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(849, 165);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.TP_processing.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TP_processing.Location = new System.Drawing.Point(23, 4);
            this.TP_processing.Name = "tabPage1";
            this.TP_processing.Padding = new System.Windows.Forms.Padding(3);
            this.TP_processing.Size = new System.Drawing.Size(822, 157);
            this.TP_processing.TabIndex = 0;
            this.TP_processing.Text = "Processing";
            this.TP_processing.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.TP_done.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TP_done.Location = new System.Drawing.Point(23, 4);
            this.TP_done.Name = "tabPage2";
            this.TP_done.Padding = new System.Windows.Forms.Padding(3);
            this.TP_done.Size = new System.Drawing.Size(822, 157);
            this.TP_done.TabIndex = 1;
            this.TP_done.Text = "Done";
            this.TP_done.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeStatusToolStripMenuItem,
            this.numberOfParallelDownloadsToolStripMenuItem,
            this.toolStripSeparator1,
            this.removeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(235, 98);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // changeStatusToolStripMenuItem
            // 
            this.changeStatusToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.waitingToolStripMenuItem,
            this.errorToolStripMenuItem});
            this.changeStatusToolStripMenuItem.Name = "changeStatusToolStripMenuItem";
            this.changeStatusToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.changeStatusToolStripMenuItem.Text = "Change Status";
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // waitingToolStripMenuItem
            // 
            this.waitingToolStripMenuItem.Name = "waitingToolStripMenuItem";
            this.waitingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.waitingToolStripMenuItem.Text = "Waiting";
            this.waitingToolStripMenuItem.Click += new System.EventHandler(this.waitingToolStripMenuItem_Click);
            // 
            // errorToolStripMenuItem
            // 
            this.errorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.forceStartToolStripMenuItem,
            this.forceWaitingToolStripMenuItem});
            this.errorToolStripMenuItem.Name = "errorToolStripMenuItem";
            this.errorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.errorToolStripMenuItem.Text = "Error";
            // 
            // forceStartToolStripMenuItem
            // 
            this.forceStartToolStripMenuItem.Name = "forceStartToolStripMenuItem";
            this.forceStartToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.forceStartToolStripMenuItem.Text = "Force Start";
            this.forceStartToolStripMenuItem.Click += new System.EventHandler(this.forceStartToolStripMenuItem_Click);
            // 
            // forceWaitingToolStripMenuItem
            // 
            this.forceWaitingToolStripMenuItem.Name = "forceWaitingToolStripMenuItem";
            this.forceWaitingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.forceWaitingToolStripMenuItem.Text = "Force Waiting";
            this.forceWaitingToolStripMenuItem.Click += new System.EventHandler(this.forceWaitingToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(231, 6);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // numberOfParallelDownloadsToolStripMenuItem
            // 
            this.numberOfParallelDownloadsToolStripMenuItem.Name = "numberOfParallelDownloadsToolStripMenuItem";
            this.numberOfParallelDownloadsToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.numberOfParallelDownloadsToolStripMenuItem.Text = "Number of parallel downloads";
            this.numberOfParallelDownloadsToolStripMenuItem.Click += new System.EventHandler(this.numberOfParallelDownloadsToolStripMenuItem_Click);
            // 
            // UC_Lv_ud
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "UC_Lv_ud";
            this.Size = new System.Drawing.Size(849, 165);
            this.tabControl1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TP_processing;
        private System.Windows.Forms.TabPage TP_done;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeStatusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem waitingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem errorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceWaitingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numberOfParallelDownloadsToolStripMenuItem;
    }
}
