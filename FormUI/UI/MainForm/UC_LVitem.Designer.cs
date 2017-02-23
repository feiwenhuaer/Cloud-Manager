namespace FormUI.UI.MainForm
{
    partial class UC_LVitem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UC_LVitem));
            this.CMS_LVitem = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.createFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyIDToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.dowloadSeletedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadFolderToHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadFileToHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TB_Path = new System.Windows.Forms.TextBox();
            this.PB_Next = new System.Windows.Forms.PictureBox();
            this.PB_Search = new System.Windows.Forms.PictureBox();
            this.PB_Back = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LV_item = new System.Windows.Forms.ListView();
            this.LV_CH_Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LV_CH_Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LV_CH_Size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LV_CH_DateMod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LV_CH_mimeType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LV_CH_Id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CMS_LVitem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Next)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Search)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Back)).BeginInit();
            this.SuspendLayout();
            // 
            // CMS_LVitem
            // 
            this.CMS_LVitem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator3,
            this.createFolderToolStripMenuItem,
            this.copyIDToClipboardToolStripMenuItem,
            this.toolStripSeparator2,
            this.dowloadSeletedToolStripMenuItem,
            this.uploadFolderToHereToolStripMenuItem,
            this.uploadFileToHereToolStripMenuItem});
            this.CMS_LVitem.Name = "CMS_LVitem";
            this.CMS_LVitem.Size = new System.Drawing.Size(193, 286);
            this.CMS_LVitem.Opening += new System.ComponentModel.CancelEventHandler(this.CMS_LVitem_Opening);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(189, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(189, 6);
            // 
            // createFolderToolStripMenuItem
            // 
            this.createFolderToolStripMenuItem.Name = "createFolderToolStripMenuItem";
            this.createFolderToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.createFolderToolStripMenuItem.Text = "Create Folder";
            this.createFolderToolStripMenuItem.Click += new System.EventHandler(this.createFolderToolStripMenuItem_Click);
            // 
            // copyIDToClipboardToolStripMenuItem
            // 
            this.copyIDToClipboardToolStripMenuItem.Name = "copyIDToClipboardToolStripMenuItem";
            this.copyIDToClipboardToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.copyIDToClipboardToolStripMenuItem.Text = "Copy ID To Clipboard";
            this.copyIDToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyIDToClipboardToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(189, 6);
            // 
            // dowloadSeletedToolStripMenuItem
            // 
            this.dowloadSeletedToolStripMenuItem.Name = "dowloadSeletedToolStripMenuItem";
            this.dowloadSeletedToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.dowloadSeletedToolStripMenuItem.Text = "Dowload Seleted";
            this.dowloadSeletedToolStripMenuItem.Click += new System.EventHandler(this.dowloadSeletedToolStripMenuItem_Click);
            // 
            // uploadFolderToHereToolStripMenuItem
            // 
            this.uploadFolderToHereToolStripMenuItem.Name = "uploadFolderToHereToolStripMenuItem";
            this.uploadFolderToHereToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.uploadFolderToHereToolStripMenuItem.Text = "Upload Folder To Here";
            this.uploadFolderToHereToolStripMenuItem.Click += new System.EventHandler(this.uploadFolderToHereToolStripMenuItem_Click);
            // 
            // uploadFileToHereToolStripMenuItem
            // 
            this.uploadFileToHereToolStripMenuItem.Name = "uploadFileToHereToolStripMenuItem";
            this.uploadFileToHereToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.uploadFileToHereToolStripMenuItem.Text = "Upload File To Here";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.TB_Path);
            this.splitContainer1.Panel1.Controls.Add(this.PB_Next);
            this.splitContainer1.Panel1.Controls.Add(this.PB_Search);
            this.splitContainer1.Panel1.Controls.Add(this.PB_Back);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1MinSize = 15;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.LV_item);
            this.splitContainer1.Size = new System.Drawing.Size(1143, 269);
            this.splitContainer1.SplitterDistance = 25;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.TabStop = false;
            // 
            // TB_Path
            // 
            //this.TB_Path.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.TB_Path.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.TB_Path.Location = new System.Drawing.Point(55, 0);
            //this.TB_Path.Margin = new System.Windows.Forms.Padding(0);
            //this.TB_Path.Name = "TB_Path";
            //this.TB_Path.Size = new System.Drawing.Size(1048, 22);
            //this.TB_Path.TabIndex = 5;
            //this.TB_Path.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TB_Path_KeyDown);
            // 
            // PB_Next
            // 
            this.PB_Next.Dock = System.Windows.Forms.DockStyle.Right;
            this.PB_Next.Image = ((System.Drawing.Image)(resources.GetObject("PB_Next.Image")));
            this.PB_Next.Location = new System.Drawing.Point(1103, 0);
            this.PB_Next.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Next.Name = "PB_Next";
            this.PB_Next.Size = new System.Drawing.Size(20, 25);
            this.PB_Next.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_Next.TabIndex = 7;
            this.PB_Next.TabStop = false;
            this.PB_Next.Click += new System.EventHandler(this.PB_Next_Click);
            // 
            // PB_Search
            // 
            this.PB_Search.Dock = System.Windows.Forms.DockStyle.Right;
            this.PB_Search.Image = ((System.Drawing.Image)(resources.GetObject("PB_Search.Image")));
            this.PB_Search.Location = new System.Drawing.Point(1123, 0);
            this.PB_Search.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Search.Name = "PB_Search";
            this.PB_Search.Size = new System.Drawing.Size(20, 25);
            this.PB_Search.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_Search.TabIndex = 8;
            this.PB_Search.TabStop = false;
            this.PB_Search.Click += new System.EventHandler(this.PB_Search_Click);
            // 
            // PB_Back
            // 
            this.PB_Back.Dock = System.Windows.Forms.DockStyle.Left;
            this.PB_Back.Image = ((System.Drawing.Image)(resources.GetObject("PB_Back.Image")));
            this.PB_Back.Location = new System.Drawing.Point(35, 0);
            this.PB_Back.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Back.Name = "PB_Back";
            this.PB_Back.Size = new System.Drawing.Size(20, 25);
            this.PB_Back.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_Back.TabIndex = 6;
            this.PB_Back.TabStop = false;
            this.PB_Back.Click += new System.EventHandler(this.PB_Back_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label1.Size = new System.Drawing.Size(35, 18);
            this.label1.TabIndex = 9;
            this.label1.Text = "Path :";
            // 
            // LV_item
            // 
            this.LV_item.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LV_CH_Name,
            this.LV_CH_Type,
            this.LV_CH_Size,
            this.LV_CH_DateMod,
            this.LV_CH_mimeType,
            this.LV_CH_Id});
            this.LV_item.ContextMenuStrip = this.CMS_LVitem;
            this.LV_item.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_item.FullRowSelect = true;
            this.LV_item.GridLines = true;
            this.LV_item.Location = new System.Drawing.Point(0, 0);
            this.LV_item.Margin = new System.Windows.Forms.Padding(0);
            this.LV_item.Name = "LV_item";
            this.LV_item.Size = new System.Drawing.Size(1143, 240);
            this.LV_item.TabIndex = 2;
            this.LV_item.UseCompatibleStateImageBehavior = false;
            this.LV_item.View = System.Windows.Forms.View.Details;
            this.LV_item.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LV_item_KeyDown);
            this.LV_item.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LV_item_MouseClick);
            this.LV_item.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LV_item_MouseDoubleClick);
            // 
            // LV_CH_Name
            // 
            this.LV_CH_Name.Name = "LV_CH_Name";
            this.LV_CH_Name.Text = "Name";
            this.LV_CH_Name.Width = 573;
            // 
            // LV_CH_Type
            // 
            this.LV_CH_Type.Name = "LV_CH_Type";
            this.LV_CH_Type.Text = "Type";
            this.LV_CH_Type.Width = 62;
            // 
            // LV_CH_Size
            // 
            this.LV_CH_Size.Name = "LV_CH_Size";
            this.LV_CH_Size.Text = "Size";
            this.LV_CH_Size.Width = 113;
            // 
            // LV_CH_DateMod
            // 
            this.LV_CH_DateMod.Name = "LV_CH_DateMod";
            this.LV_CH_DateMod.Text = "Date modified";
            this.LV_CH_DateMod.Width = 105;
            // 
            // LV_CH_mimeType
            // 
            this.LV_CH_mimeType.Name = "LV_CH_mimeType";
            this.LV_CH_mimeType.Text = "mimeType";
            this.LV_CH_mimeType.Width = 63;
            // 
            // LV_CH_Id
            // 
            this.LV_CH_Id.Name = "LV_CH_Id";
            this.LV_CH_Id.Text = "ID";
            this.LV_CH_Id.Width = 25;
            // 
            // UC_LVitem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Name = "UC_LVitem";
            this.Size = new System.Drawing.Size(1143, 269);
            this.Load += new System.EventHandler(this.UC_LVitem_Load);
            this.CMS_LVitem.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Next)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Search)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Back)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem dowloadSeletedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadFolderToHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadFileToHereToolStripMenuItem;
        public System.Windows.Forms.ContextMenuStrip CMS_LVitem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem copyIDToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createFolderToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox TB_Path;
        private System.Windows.Forms.PictureBox PB_Back;
        private System.Windows.Forms.PictureBox PB_Next;
        private System.Windows.Forms.PictureBox PB_Search;
        public System.Windows.Forms.ListView LV_item;
        private System.Windows.Forms.ColumnHeader LV_CH_Name;
        private System.Windows.Forms.ColumnHeader LV_CH_Type;
        private System.Windows.Forms.ColumnHeader LV_CH_Size;
        private System.Windows.Forms.ColumnHeader LV_CH_DateMod;
        private System.Windows.Forms.ColumnHeader LV_CH_mimeType;
        private System.Windows.Forms.ColumnHeader LV_CH_Id;
    }
}
