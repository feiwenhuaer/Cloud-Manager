namespace FormUI.UI.MainForm
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.TV_item = new System.Windows.Forms.TreeView();
            this.CMS_TVitem = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.dowloadSeletedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadFolderToHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadFileToHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.IMGList_TV = new System.Windows.Forms.ImageList(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.CMS_Tabcontrol = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeThisTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSSL_Upload = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSSL_download = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cloudToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.CMS_TVitem.SuspendLayout();
            this.CMS_Tabcontrol.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.TV_item);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            // 
            // TV_item
            // 
            this.TV_item.ContextMenuStrip = this.CMS_TVitem;
            resources.ApplyResources(this.TV_item, "TV_item");
            this.TV_item.ImageList = this.IMGList_TV;
            this.TV_item.Name = "TV_item";
            this.TV_item.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TV_item_NodeMouseClick);
            this.TV_item.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TV_item_NodeMouseDoubleClick);
            // 
            // CMS_TVitem
            // 
            this.CMS_TVitem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator2,
            this.dowloadSeletedToolStripMenuItem,
            this.uploadFolderToHereToolStripMenuItem,
            this.uploadFileToHereToolStripMenuItem});
            this.CMS_TVitem.Name = "CMS_LVitem";
            resources.ApplyResources(this.CMS_TVitem, "CMS_TVitem");
            this.CMS_TVitem.Opening += new System.ComponentModel.CancelEventHandler(this.CMS_TVitem_Opening);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // dowloadSeletedToolStripMenuItem
            // 
            this.dowloadSeletedToolStripMenuItem.Name = "dowloadSeletedToolStripMenuItem";
            resources.ApplyResources(this.dowloadSeletedToolStripMenuItem, "dowloadSeletedToolStripMenuItem");
            this.dowloadSeletedToolStripMenuItem.Click += new System.EventHandler(this.dowloadSeletedToolStripMenuItem_Click);
            // 
            // uploadFolderToHereToolStripMenuItem
            // 
            this.uploadFolderToHereToolStripMenuItem.Name = "uploadFolderToHereToolStripMenuItem";
            resources.ApplyResources(this.uploadFolderToHereToolStripMenuItem, "uploadFolderToHereToolStripMenuItem");
            this.uploadFolderToHereToolStripMenuItem.Click += new System.EventHandler(this.uploadFolderToHereToolStripMenuItem_Click);
            // 
            // uploadFileToHereToolStripMenuItem
            // 
            this.uploadFileToHereToolStripMenuItem.Name = "uploadFileToHereToolStripMenuItem";
            resources.ApplyResources(this.uploadFileToHereToolStripMenuItem, "uploadFileToHereToolStripMenuItem");
            this.uploadFileToHereToolStripMenuItem.Click += new System.EventHandler(this.uploadFileToHereToolStripMenuItem_Click);
            // 
            // IMGList_TV
            // 
            this.IMGList_TV.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IMGList_TV.ImageStream")));
            this.IMGList_TV.TransparentColor = System.Drawing.Color.Transparent;
            this.IMGList_TV.Images.SetKeyName(0, "kcmdevices32.ico");
            this.IMGList_TV.Images.SetKeyName(1, "Folder.ico");
            this.IMGList_TV.Images.SetKeyName(2, "Xenatt-Minimalism-App-dropbox32.ico");
            this.IMGList_TV.Images.SetKeyName(3, "Marcus-Roberto-Google-Play-Google-Drive32.ico");
            this.IMGList_TV.Images.SetKeyName(4, "mega32.ico");
            // 
            // tabControl1
            // 
            this.tabControl1.AccessibleRole = System.Windows.Forms.AccessibleRole.Alert;
            this.tabControl1.ContextMenuStrip = this.CMS_Tabcontrol;
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // CMS_Tabcontrol
            // 
            this.CMS_Tabcontrol.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewTabToolStripMenuItem,
            this.closeThisTabToolStripMenuItem});
            this.CMS_Tabcontrol.Name = "CMS_Tabcontrol";
            resources.ApplyResources(this.CMS_Tabcontrol, "CMS_Tabcontrol");
            this.CMS_Tabcontrol.Opening += new System.ComponentModel.CancelEventHandler(this.CMS_Tabcontrol_Opening);
            // 
            // addNewTabToolStripMenuItem
            // 
            this.addNewTabToolStripMenuItem.Name = "addNewTabToolStripMenuItem";
            resources.ApplyResources(this.addNewTabToolStripMenuItem, "addNewTabToolStripMenuItem");
            this.addNewTabToolStripMenuItem.Click += new System.EventHandler(this.addNewTabToolStripMenuItem_Click);
            // 
            // closeThisTabToolStripMenuItem
            // 
            this.closeThisTabToolStripMenuItem.Name = "closeThisTabToolStripMenuItem";
            resources.ApplyResources(this.closeThisTabToolStripMenuItem, "closeThisTabToolStripMenuItem");
            this.closeThisTabToolStripMenuItem.Click += new System.EventHandler(this.closeThisTabToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.TSSL_Upload,
            this.toolStripStatusLabel2,
            this.TSSL_download});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // TSSL_Upload
            // 
            this.TSSL_Upload.Name = "TSSL_Upload";
            resources.ApplyResources(this.TSSL_Upload, "TSSL_Upload");
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
            // 
            // TSSL_download
            // 
            this.TSSL_download.Name = "TSSL_download";
            resources.ApplyResources(this.TSSL_download, "TSSL_download");
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.cloudToolStripMenuItem,
            this.settingsToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // filesToolStripMenuItem
            // 
            this.filesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            resources.ApplyResources(this.filesToolStripMenuItem, "filesToolStripMenuItem");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            // 
            // cloudToolStripMenuItem
            // 
            this.cloudToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.cloudToolStripMenuItem.Name = "cloudToolStripMenuItem";
            resources.ApplyResources(this.cloudToolStripMenuItem, "cloudToolStripMenuItem");
            this.cloudToolStripMenuItem.DropDownOpening += new System.EventHandler(this.cloudToolStripMenuItem_DropDownOpening);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            resources.ApplyResources(this.addToolStripMenuItem, "addToolStripMenuItem");
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            resources.ApplyResources(this.removeToolStripMenuItem, "removeToolStripMenuItem");
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem1,
            this.uiToolStripMenuItem,
            this.languageToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.settingsToolStripMenuItem_DropDownOpening);
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            resources.ApplyResources(this.settingsToolStripMenuItem1, "settingsToolStripMenuItem1");
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.settingsToolStripMenuItem1_Click);
            // 
            // uiToolStripMenuItem
            // 
            this.uiToolStripMenuItem.Name = "uiToolStripMenuItem";
            resources.ApplyResources(this.uiToolStripMenuItem, "uiToolStripMenuItem");
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            resources.ApplyResources(this.languageToolStripMenuItem, "languageToolStripMenuItem");
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.CMS_TVitem.ResumeLayout(false);
            this.CMS_Tabcontrol.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cloudToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel TSSL_Upload;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel TSSL_download;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView TV_item;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ImageList IMGList_TV;
        public System.Windows.Forms.ContextMenuStrip CMS_TVitem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem dowloadSeletedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadFolderToHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadFileToHereToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip CMS_Tabcontrol;
        private System.Windows.Forms.ToolStripMenuItem addNewTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeThisTabToolStripMenuItem;
    }
}