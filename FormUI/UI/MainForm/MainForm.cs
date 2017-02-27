using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Etier.IconHelper;
using SupDataDll;
using System.Threading;
using SupDataDll.UiInheritance;
using FormUI.UI.SettingForm;
using FormUI.UI.Oauth;

namespace FormUI.UI.MainForm
{
    public partial class MainForm : System.Windows.Forms.Form, SupDataDll.UiInheritance.UIMain
    {
        #region interface
        UserControl LV_Ud_control;
        public bool AreReloadUI
        {
            get
            {
                return Setting_UI.ReloadUI_Flag;
            }
        }
        public void load_uC_Lv_ud(UIUC_TLV_ud control)
        {
            LV_Ud_control = (UserControl)control;
            LV_Ud_control.Dock = DockStyle.Fill;
            if (InvokeRequired) Invoke(new Action(() => splitContainer1.Panel2.Controls.Add(LV_Ud_control)));
            else splitContainer1.Panel2.Controls.Add(LV_Ud_control);
        }
        public void AddNewCloudToTV(string email,CloudType type)
        {
            if (InvokeRequired) Invoke(new Action(() => AddNewCloudToTV_(email, type)));
            else AddNewCloudToTV_(email, type);
        }
        public void ShowDialog_()
        {
            this.ShowDialog();
        }
        public void FileSaveDialog(string InitialDirectory, string FileName, string Filter, ExplorerNode node)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = InitialDirectory;
            sfd.FileName = FileName;
            sfd.Filter = Filter;
            Invoke(new Action(() => 
            {
                DialogResult rs = sfd.ShowDialog();
                if (rs == DialogResult.OK || rs == DialogResult.Yes)
                {
                    ExplorerNode filesave = ExplorerNode.GetNodeFromDiskPath(sfd.FileName, node.Info.Size);
                    Setting_UI.reflection_eventtocore._AddItem(new List<ExplorerNode>() { node }, node.Parent, filesave.Parent, false);
                }
            }));
        }
        #endregion

        void AddNewCloudToTV_(string email, CloudType type)
        {
            TV_item.Nodes.Add(new TreeNode_(email, (int)type));
        }

        Icon icon_folder;
        string TimeFormat;
        
        #region MainForm
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            Setting_UI.ReloadUI_Flag = false;
            icon_folder = IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Closed);
            TimeFormat = Setting_UI.reflection_eventtocore._GetSetting(SettingsKey.DATE_TIME_FORMAT);
            LoadLanguage();
            AddNewTabControl();
            TV_item.BeginUpdate();
            foreach (var drive in DriveInfo.GetDrives())
                TV_item.Nodes.Add(new TreeNode_(drive.RootDirectory.ToString().Replace("\\", null), 0));

            foreach (CloudEmail_Type cloud in Setting_UI.reflection_eventtocore._GetListAccountCloud())
                TV_item.Nodes.Add(new TreeNode_(cloud.Email, (int)cloud.Type));
            TV_item.EndUpdate();

            this.Activate();
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Setting_UI.ManagerThreads.CloseAll();
            if (!Setting_UI.ReloadUI_Flag)
            {
                Setting_UI.ExitAPP_Flag = true;
                Setting_UI.reflection_eventtocore._ExitApp();
            }
        }
        #endregion

        #region TabControl LV
        private List<UC_LVitem> list_UCLVitem = new List<UC_LVitem>();
        
        private void AddNewTabControl()
        {
            UC_LVitem ucitem = new UC_LVitem();
            ucitem.Dock = DockStyle.Fill;
            ucitem.MainForm = this;
            ucitem.EventListViewFolderDoubleClickCallBack += Ucitem_EventListViewFolderDoubleClickCallBack;
            list_UCLVitem.Add(ucitem);
            ucitem.LoadLanguage();
            TabPage tabpage = new TabPage();
            tabpage.Controls.Add(list_UCLVitem[list_UCLVitem.Count -1]);
            tabpage.Location = new System.Drawing.Point(4, 22);
            tabpage.Padding = new System.Windows.Forms.Padding(3);
            tabpage.Size = new System.Drawing.Size(672, 296);
            tabpage.TabIndex = tabControl1.Controls.Count;
            string newtab = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.newtab);
            tabpage.Text = newtab;
            tabpage.ToolTipText = newtab;
            tabpage.UseVisualStyleBackColor = true;
            tabControl1.Controls.Add(tabpage);
        }

        private void addNewTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewTabControl();
        }
        private void closeThisTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseTabControl(tabControl1.SelectedIndex);
        }
        
        private void CMS_Tabcontrol_Opening(object sender, CancelEventArgs e)
        {
            if (!CMS_Tabcontrol.Enabled) e.Cancel = true;
            if (tabControl1.TabCount == 1)
            {
                CMS_Tabcontrol.Items[1].Enabled = false;
            }
            else
            {
                CMS_Tabcontrol.Items[1].Enabled = true;
            }
        }
        private void CloseTabControl(int index)
        {
            var control = tabControl1.Controls[index];
            tabControl1.Controls.RemoveAt(index);
            list_UCLVitem.RemoveAt(index);
            control.Dispose();
        }
        #endregion

        //language
        private void LoadLanguage()
        {
            Setting_UI.reflection_eventtocore._SaveSetting();
            Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.ReloadLang);
            //Menu ToolStrip
            cloudToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.ToolStrip_cloud);
            settingsToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.ToolStrip_settings);
            settingsToolStripMenuItem1.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.ToolStrip_settings_setting);
            addToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.ToolStrip_cloud_add);
            removeToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.ToolStrip_cloud_remove);

            cutToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_cut);
            copyToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_copy);
            pasteToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_paste);
            deleteToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_delete);
            dowloadSeletedToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_downloadsellected);
            uploadFileToHereToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_uploadfile);
            uploadFolderToHereToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_uploadfolder);

            addNewTabToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.addtab);
            closeThisTabToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.removetab);

            ((SupDataDll.UiInheritance.UIUC_TLV_ud)LV_Ud_control).LoadLanguage();

            for(int i = 0; i < tabControl1.Controls.Count;i++)
            {
                ((UC_LVitem)((TabPage)tabControl1.Controls[i]).Controls[0]).LoadLanguage();
            }
        }

        #region Event TV 
        private void TV_item_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OpenPath(false, e);
        }
        private void TV_item_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OpenPath(true, e);
        }
        private void OpenPath(bool explandTV, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                if (e.Button == MouseButtons.Right) TV_item.SelectedNode = e.Node;
                return;
            }
            TreeNode_ TN = e.Node as TreeNode_;
            if (TN == null) return;
            list_UCLVitem[tabControl1.SelectedIndex].managerexplorernodes.Root = TN.explorernode.GetRoot();
            list_UCLVitem[tabControl1.SelectedIndex].managerexplorernodes.Next(TN.explorernode);
            list_UCLVitem[tabControl1.SelectedIndex].ExplorerCurrentNode(explandTV, true, TN);
        }
        private void CMS_TVitem_Opening(object sender, CancelEventArgs e)
        {
            if (ClipBoard_.Clipboard) pasteToolStripMenuItem.Enabled = true;
            else pasteToolStripMenuItem.Enabled = false;
            bool areCloud = GetRootParent(TV_item.SelectedNode).Text.IndexOf('@') >= 0;
            dowloadSeletedToolStripMenuItem.Enabled = areCloud;
            uploadFileToHereToolStripMenuItem.Enabled = areCloud;
            uploadFolderToHereToolStripMenuItem.Enabled = areCloud;

            try
            {
                TreeNode root = TV_item.SelectedNode.Parent.Parent;//folder
                cutToolStripMenuItem.Enabled = true;
                copyToolStripMenuItem.Enabled = true;
                deleteToolStripMenuItem.Enabled = true;
                dowloadSeletedToolStripMenuItem.Enabled = areCloud;
            }
            catch
            {
                cutToolStripMenuItem.Enabled = false;
                copyToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = areCloud;
                dowloadSeletedToolStripMenuItem.Enabled = false;
            }
            
        }
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutCopyTV(true);
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutCopyTV(false);
        }
        private void CutCopyTV(bool AreCut)
        {
            ClipBoard_.Clear();
            ClipBoard_.AreCut = AreCut;
            ClipBoard_.directory = ((TreeNode_)TV_item.SelectedNode).explorernode.Parent;
            ClipBoard_.Add(((TreeNode_)TV_item.SelectedNode).explorernode);
            ClipBoard_.Clipboard = true;
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting_UI.reflection_eventtocore._AddItem( ClipBoard_.Items, 
                                                        ClipBoard_.directory, 
                                                        ((TreeNode_)TV_item.SelectedNode).explorernode, 
                                                        ClipBoard_.AreCut);
            if (ClipBoard_.AreCut) ClipBoard_.Clipboard = false;
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteConfirmForm d = new DeleteConfirmForm();
            switch (TV_item.SelectedNode.ImageIndex)
            {
                case 0://disk
                    break;
                case 1://folder
                    TreeNode_ parent_node = GetRootParent(TV_item.SelectedNode);
                    if (parent_node.ImageIndex == 0) d.TB.Text = TV_item.SelectedNode.FullPath;//disk
                    else d.TB.Text = ((CloudType)parent_node.ImageIndex).ToString() + ":" + TV_item.SelectedNode.FullPath;
                    d.ShowDialog(this);
                    if (d.Delete)
                    {
                        DeleteItems deleteitems = new DeleteItems() { PernamentDelete = d.CB_pernament.Checked };
                        deleteitems.Items.Add(((TreeNode_)TV_item.SelectedNode).explorernode);

                        Thread thr = new Thread(Setting_UI.reflection_eventtocore._DeletePath);
                        Setting_UI.ManagerThreads.delete.Add(thr);
                        thr.Start(deleteitems);
                        Setting_UI.ManagerThreads.CleanThr();
                    }
                    break;
                default://cloud
                    d.TB.Text = ((CloudType)TV_item.SelectedNode.ImageIndex).ToString() + ":" + TV_item.SelectedNode.Text;
                    d.CB_pernament.Enabled = false;
                    d.ShowDialog(this);
                    if (d.Delete)
                    {
                        TV_item.Nodes.Remove(TV_item.SelectedNode);
                        Setting_UI.reflection_eventtocore._DeleteAccountCloud(TV_item.SelectedNode.Text, (CloudType)TV_item.SelectedNode.ImageIndex);
                    }
                    break;
            }
        }
        private void dowloadSeletedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.ShowNewFolderButton = true;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK | result == DialogResult.Yes)
            {
                List<ExplorerNode> list_item_from = new List<ExplorerNode>();
                ExplorerNode node = ExplorerNode.GetNodeFromDiskPath(TV_item.SelectedNode.FullPath);
                list_item_from.Add(node);
                Setting_UI.reflection_eventtocore._AddItem(list_item_from, node.Parent,
                    ExplorerNode.GetNodeFromDiskPath(fbd.SelectedPath), false);
            }
        }
        private void uploadFolderToHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.ShowNewFolderButton = true;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK | result == DialogResult.Yes)
            {

                List<ExplorerNode> list_item_from = new List<ExplorerNode>();
                ExplorerNode node = ExplorerNode.GetNodeFromDiskPath(fbd.SelectedPath);
                list_item_from.Add(node);
                Setting_UI.reflection_eventtocore._AddItem(list_item_from, node.Parent,
                    ((UC_LVitem)tabControl1.SelectedTab.Controls[0]).managerexplorernodes.NodeWorking(), false);
            }
        }
        private void uploadFileToHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = PCPath.FilterAllFiles;
            ofd.InitialDirectory = PCPath.Mycomputer;
            
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK | result == DialogResult.Yes)
            {
                List<ExplorerNode> list_item_from = new List<ExplorerNode>();
                
                string root = Path.GetDirectoryName(ofd.FileNames[0]);
                ExplorerNode rootnode = ExplorerNode.GetNodeFromDiskPath(root);
                foreach (string a in ofd.SafeFileNames)
                {
                    FileInfo info = new FileInfo(root + "\\" + a);
                    ExplorerNode n = new ExplorerNode();
                    n.Info.Name = a;
                    n.Info.Size = info.Length;
                    rootnode.AddChild(n);
                    list_item_from.Add(n);
                }
                Setting_UI.reflection_eventtocore._AddItem(list_item_from, rootnode, 
                    ((UC_LVitem)tabControl1.SelectedTab.Controls[0]).managerexplorernodes.NodeWorking(), false);
            }
        }
        private TreeNode_ GetRootParent(TreeNode node)
        {
            TreeNode_ parent = (TreeNode_)node;
            while (parent.Parent != null)
            {
                parent = (TreeNode_)parent.Parent;
            }
            return parent;
        }
        #endregion

        //update TV, TB, LV item
        private void Ucitem_EventListViewFolderDoubleClickCallBack(ExplorerListItem load)
        {
            load.indexLV_tab = tabControl1.SelectedIndex;
            CMS_Tabcontrol.Enabled = false;
            try { ((tabControl1.Controls[tabControl1.SelectedIndex] as TabPage).Controls[0] as UC_LVitem).threxplorer.Abort(); } catch { }
            ((tabControl1.Controls[tabControl1.SelectedIndex] as TabPage).Controls[0] as UC_LVitem).threxplorer = new Thread(GetList_AddItemTo_LVnTV_);
            ((tabControl1.Controls[tabControl1.SelectedIndex] as TabPage).Controls[0] as UC_LVitem).threxplorer.Start(load);
        }
        public void GetList_AddItemTo_LVnTV_(object obj)
        {
            ExplorerListItem o = (ExplorerListItem)obj;
            bool exception = false;
            bool returnnull = false;
            try
            {
                if (Setting_UI.reflection_eventtocore._ListIteamRequest(o.node) == null) returnnull = true;
            }
            catch(ThreadAbortException)
            {
                exception = true;
            }
            catch(IOException ex)
            {
                if (ex.Message == "The device is not ready.\r\n")
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    exception = true;
                }
                else
                {
                    Console.WriteLine("abort (IOException)");
                    exception = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                exception = true;
            }
            finally
            {
                if (returnnull || exception) Invoke(new Action(() =>
                                                    {
                                                        list_UCLVitem[o.indexLV_tab].managerexplorernodes.Back();
                                                    }));
                Invoke(new Action(() => CMS_Tabcontrol.Enabled = true));
            }
            if (!exception & !returnnull) Invoke(new Action(() => SetData_GetList_AddItemTo_LVnTV_(o)));
        }
        public void SetData_GetList_AddItemTo_LVnTV_(ExplorerListItem load)
        {
            if (load.addToTV && load.TV_node != null)//add folder to tree view
            {
                ((TreeNode)load.TV_node).Nodes.Clear();
                foreach (ExplorerNode c in load.node.Child)
                {
                    if (c.Info.Size >0) continue;
                    TreeNode_ child = new TreeNode_(c);
                    ((TreeNode_)load.TV_node).Nodes.Add(child);
                }
                if (load.explandTV) ((TreeNode)load.TV_node).Expand();
            }
            // Add LV tab index
            if (load.indexLV_tab != -1)
            {
                List<ItemLV> ListItem_LV = new List<ItemLV>();
                DateTime temp = new DateTime(0);
                foreach (ExplorerNode c in load.node.Child)
                {
                    if (c.Info.Size > 0) continue;
                    string datetime = "";
                    if (c.Info.DateMod != temp) { try { datetime = c.Info.DateMod.ToString(TimeFormat); } catch { } }

                    ListItem_LV.Add(new ItemLV() { str = new string[] { c.Info.Name, "Folder", string.Empty, datetime, c.Info.MimeType, c.Info.ID }, icon = icon_folder });
                }
                foreach (ExplorerNode c in load.node.Child)
                {
                    if (c.Info.Size < 1) continue;
                    string extension = c.GetExtension();                    
                    ListItem_LV.Add(new ItemLV() {
                        str = new string[] { c.Info.Name, "File",c.Info.Size.ToString(), c.Info.DateMod.ToString(TimeFormat), c.Info.MimeType, c.Info.ID },
                        icon = c.GetRoot().RootInfo.Type == CloudType.LocalDisk ? 
                            IconReader.GetFileIcon(c.GetFullPathString(), IconReader.IconSize.Small,false) : //some large file make slow.
                            IconReader.GetFileIcon("." + extension, IconReader.IconSize.Small, false)});
                    
                }
                list_UCLVitem[load.indexLV_tab].AddListViewItem(ListItem_LV);
            }

            //set tab text name
            tabControl1.TabPages[load.indexLV_tab].Text = load.node.Info.Name;

            //set tooltip tab
            tabControl1.TabPages[load.indexLV_tab].ToolTipText = load.node.GetFullPathString();

            //set PathNode
            list_UCLVitem[load.indexLV_tab].pathUC1.Node = load.node;

            //OldPathLV inpath = new OldPathLV();
            //inpath.ID = list.id_folder;
            //inpath.Path = loaditemthread.path.TrimEnd(new char[] { '\\', '/' });
            ////set old Path TextBox
            //list_UCLVitem[loaditemthread.indexLV_tab].oldpathlv.Add(inpath);
        }

        #region ToolStripMenu
        //open setting form
        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SettingsForm setting = new SettingsForm();
            setting.ShowDialog();
            if(setting.IsSave)
            {
                if (setting.IsChangeLang) LoadLanguage();
                if(setting.IsChangeUI)
                {
                    Setting_UI.ReloadUI_Flag = true;
                    this.Close();
                }
            }
        }
        //load menu ui & lang
        private void settingsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //ui
            uiToolStripMenuItem.DropDownItems.Clear();
            string dll_now = Setting_UI.reflection_eventtocore._GetSetting(SettingsKey.UI_dll_file);
            foreach (string file in GetList_UI_n_lang.GetListUiFile())
            {
                ToolStripMenuItem item = new ToolStripMenuItem(file);
                item.Size = new System.Drawing.Size(152, 22);
                if (dll_now == file) { item.Checked = true; item.Enabled = false; }
                else item.Click += UI_Item_Click;
                uiToolStripMenuItem.DropDownItems.Add(item);
            }
            //lang
            languageToolStripMenuItem.DropDownItems.Clear();
            string lang_now = Setting_UI.reflection_eventtocore._GetSetting(SettingsKey.lang);
            foreach (string file in GetList_UI_n_lang.GetListLangFile())
            {
                ToolStripMenuItem item = new ToolStripMenuItem(file);
                item.Size = new System.Drawing.Size(152, 22);
                if (lang_now == file) { item.Checked = true; item.Enabled = false; }
                else item.Click += Lang_Item_Click;
                languageToolStripMenuItem.DropDownItems.Add(item);
            }
        }//menu setting (ui & lang)
        //change language
        private void Lang_Item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Setting_UI.reflection_eventtocore._SetSetting(SettingsKey.lang, item.Text);
            LoadLanguage();
        }
        //change ui
        private void UI_Item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Setting_UI.reflection_eventtocore._SetSetting(SettingsKey.UI_dll_file, item.Text);
            Setting_UI.ReloadUI_Flag = true;
            this.Close();
        }
        //add cloud
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSellectOauth oauth = new FormSellectOauth();
            //oauth.Show(this);
            oauth.ShowDialog(this);
        }
        private void cloudToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            removeToolStripMenuItem.DropDownItems.Clear();
            foreach (CloudEmail_Type cloud in Setting_UI.reflection_eventtocore._GetListAccountCloud())
            {
                ToolStripMenuItem item = new ToolStripMenuItem(cloud.Type.ToString()+":"+ cloud.Email);
                item.Click += RemoveCloudItem_Click;
                removeToolStripMenuItem.DropDownItems.Add(item);
            }
        }
        private void RemoveCloudItem_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            string[] cloud = item.Text.Split(':');
            CloudType type = (CloudType)Enum.Parse(typeof(CloudType), cloud[0]);
            Setting_UI.reflection_eventtocore._DeleteAccountCloud(cloud[1], type);
            foreach(TreeNode node in TV_item.Nodes)
            {
                if(node.Text == cloud[1] & node.ImageIndex == (int)type)
                {
                    TV_item.Nodes.Remove(node);
                    return;
                }
            }
        }
        #endregion
    }
}
