using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Etier.IconHelper;
using CloudManagerGeneralLib;
using System.Threading;
using CloudManagerGeneralLib.UiInheritance;
using FormUI.UI.SettingForm;
using FormUI.UI.Oauth;
using CloudManagerGeneralLib.Class;

namespace FormUI.UI.MainForm
{
    public partial class MainForm : Form, UIMain
    {
        #region interface
        public bool AreReloadUI
        {
            get
            {
                return Setting_UI.ReloadUI_Flag;
            }
        }

        public void AddNewCloudToTV(ItemNode newnode)
        {
            if (InvokeRequired) Invoke(new Action(() => AddNewCloudToTV_(newnode)));
            else AddNewCloudToTV_(newnode);
        }
        public void ShowDialog_()
        {
            this.ShowDialog();
        }

        public void UpdateGroup(TransferGroup Group, UpdateTransfer_TLVUD type)
        {
            switch (type)
            {
                case UpdateTransfer_TLVUD.Add: uC_Lv_ud1.AddNewGroup(Group); break;
                case UpdateTransfer_TLVUD.Remove: uC_Lv_ud1.RemoveGroup(Group); break;
                case UpdateTransfer_TLVUD.Refresh: uC_Lv_ud1.RefreshAll(); break;
            }
        }

        public void FileSaveDialog(string InitialDirectory, string FileName, string Filter, ItemNode node)
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
                    ItemNode filesave = ItemNode.GetNodeFromDiskPath(sfd.FileName, node.Info.Size);
                    Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.TransferItems(new List<ItemNode>() { node }, node.Parent, filesave.Parent, false);
                }
            }));
        }
        public void ShowChildUI(object UI, bool ShowDialog,bool Owner)
        {
            Invoke(new Action(() =>
            {
                if (ShowDialog)
                {
                    if (Owner) ((System.Windows.Forms.Form)UI).ShowDialog(this);
                    else ((System.Windows.Forms.Form)UI).ShowDialog();
                }
                else
                {
                    if (Owner) ((System.Windows.Forms.Form)UI).Show(this);
                    else ((System.Windows.Forms.Form)UI).Show();
                }
            }));
        }
        public T CreateUI<T>(Type type)
        {
            return (T)Activator.CreateInstance(type);
        }
        #endregion

        void AddNewCloudToTV_(ItemNode newnode)
        {
            TV_item.Nodes.Add(new TreeNode_(newnode));
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
            TimeFormat = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetSetting(SettingsKey.DATE_TIME_FORMAT);
            LoadLanguage();
            AddNewTabControl();
            TV_item.BeginUpdate();
            foreach (var drive in DriveInfo.GetDrives())
                TV_item.Nodes.Add(new TreeNode_(drive.RootDirectory.ToString().Replace("\\", null), 0));

            foreach (ItemNode cloud in Setting_UI.reflection_eventtocore.AccountsAndCloud.GetListAccountCloud())
                TV_item.Nodes.Add(new TreeNode_(cloud));
            TV_item.EndUpdate();

            this.Activate();
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Setting_UI.ManagerThreads.CloseAll();
            if (!Setting_UI.ReloadUI_Flag)
            {
                Setting_UI.ExitAPP_Flag = true;
                Setting_UI.reflection_eventtocore.ExitApp();
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
            string newtab = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.newtab);
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
            Setting_UI.reflection_eventtocore.SettingAndLanguage.SaveSetting();
            Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.ReloadLang);
            //Menu ToolStrip
            cloudToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.ToolStrip_cloud);
            settingsToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.ToolStrip_settings);
            settingsToolStripMenuItem1.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.ToolStrip_settings_setting);
            addToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.ToolStrip_cloud_add);
            removeToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.ToolStrip_cloud_remove);

            cutToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.TSMI_cut);
            copyToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.TSMI_copy);
            pasteToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.TSMI_paste);
            deleteToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.TSMI_delete);
            dowloadSeletedToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.TSMI_downloadsellected);
            uploadFileToHereToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.TSMI_uploadfile);
            uploadFolderToHereToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.TSMI_uploadfolder);

            addNewTabToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.addtab);
            closeThisTabToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.removetab);

            uC_Lv_ud1.LoadLanguage();

            for (int i = 0; i < tabControl1.Controls.Count;i++)
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
            list_UCLVitem[tabControl1.SelectedIndex].managerhistory_itemnodes.Root = TN.explorernode.GetRoot;
            list_UCLVitem[tabControl1.SelectedIndex].managerhistory_itemnodes.Next(TN.explorernode);
            list_UCLVitem[tabControl1.SelectedIndex].ExplorerCurrentNode(explandTV, true, TN);
        }
        private void CMS_TVitem_Opening(object sender, CancelEventArgs e)
        {
            if (AppClipboard.Clipboard) pasteToolStripMenuItem.Enabled = true;
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
            AppClipboard.Clear();
            AppClipboard.AreCut = AreCut;
            AppClipboard.directory = ((TreeNode_)TV_item.SelectedNode).explorernode.Parent;
            AppClipboard.Add(((TreeNode_)TV_item.SelectedNode).explorernode);
            AppClipboard.Clipboard = true;
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.TransferItems( AppClipboard.Items, 
                                                        AppClipboard.directory, 
                                                        ((TreeNode_)TV_item.SelectedNode).explorernode, 
                                                        AppClipboard.AreCut);
            if (AppClipboard.AreCut) AppClipboard.Clipboard = false;
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
                        Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.DeletePath(deleteitems);
                    }
                    break;
                default://cloud
                    d.TB.Text = ((CloudType)TV_item.SelectedNode.ImageIndex).ToString() + ":" + TV_item.SelectedNode.Text;
                    d.CB_pernament.Enabled = false;
                    d.ShowDialog(this);
                    if (d.Delete)
                    {
                        TV_item.Nodes.Remove(TV_item.SelectedNode);
                        Setting_UI.reflection_eventtocore.AccountsAndCloud.DeleteAccountCloud(TV_item.SelectedNode.Text, (CloudType)TV_item.SelectedNode.ImageIndex);
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
                List<ItemNode> list_item_from = new List<ItemNode>();
                ItemNode node = ItemNode.GetNodeFromDiskPath(TV_item.SelectedNode.FullPath);
                list_item_from.Add(node);
                Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.TransferItems(list_item_from, node.Parent,
                    ItemNode.GetNodeFromDiskPath(fbd.SelectedPath), false);
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

                List<ItemNode> list_item_from = new List<ItemNode>();
                ItemNode node = ItemNode.GetNodeFromDiskPath(fbd.SelectedPath);
                list_item_from.Add(node);
                Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.TransferItems(list_item_from, node.Parent,
                    ((UC_LVitem)tabControl1.SelectedTab.Controls[0]).managerhistory_itemnodes.NodeWorking(), false);
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
                List<ItemNode> list_item_from = new List<ItemNode>();
                
                string root = Path.GetDirectoryName(ofd.FileNames[0]);
                ItemNode rootnode = ItemNode.GetNodeFromDiskPath(root);
                foreach (string a in ofd.SafeFileNames)
                {
                    FileInfo info = new FileInfo(root + "\\" + a);
                    ItemNode n = new ItemNode();
                    n.Info.Name = a;
                    n.Info.Size = info.Length;
                    rootnode.AddChild(n);
                    list_item_from.Add(n);
                }
                Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.TransferItems(list_item_from, rootnode, 
                    ((UC_LVitem)tabControl1.SelectedTab.Controls[0]).managerhistory_itemnodes.NodeWorking(), false);
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
                if (Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.ListIteamRequest(o.node) == null) returnnull = true;
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
                                                        list_UCLVitem[o.indexLV_tab].managerhistory_itemnodes.Back();
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
                foreach (ItemNode c in load.node.Child)
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
                foreach (ItemNode c in load.node.Child)
                {
                    if (c.Info.Size > 0) continue;
                    string datetime = "";
                    if (c.Info.DateMod != temp) { try { datetime = c.Info.DateMod.ToString(TimeFormat); } catch { } }

                    ListItem_LV.Add(new ItemLV() { str = new string[] { c.Info.Name, "Folder", string.Empty, datetime, c.Info.MimeType, c.Info.ID }, icon = icon_folder });
                }
                foreach (ItemNode c in load.node.Child)
                {
                    if (c.Info.Size < 1) continue;
                    string extension = c.GetExtension();                    
                    ListItem_LV.Add(new ItemLV() {
                        str = new string[] { c.Info.Name, "File",c.Info.Size.ToString(), c.Info.DateMod.ToString(TimeFormat), c.Info.MimeType, c.Info.ID },
                        icon = c.GetRoot.NodeType.Type == CloudType.LocalDisk ? 
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
            string dll_now = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetSetting(SettingsKey.UI_dll_file);
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
            string lang_now = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetSetting(SettingsKey.lang);
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
            Setting_UI.reflection_eventtocore.SettingAndLanguage.SetSetting(SettingsKey.lang, item.Text);
            LoadLanguage();
        }
        //change ui
        private void UI_Item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Setting_UI.reflection_eventtocore.SettingAndLanguage.SetSetting(SettingsKey.UI_dll_file, item.Text);
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
            foreach (ItemNode cloud in Setting_UI.reflection_eventtocore.AccountsAndCloud.GetListAccountCloud())
            {
                ToolStripMenuItem item = new ToolStripMenuItem(cloud.NodeType.Type.ToString()+":"+ cloud.NodeType.Email);
                item.Click += RemoveCloudItem_Click;
                removeToolStripMenuItem.DropDownItems.Add(item);
            }
        }
        private void RemoveCloudItem_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            string[] cloud = item.Text.Split(':');
            CloudType type = (CloudType)Enum.Parse(typeof(CloudType), cloud[0]);
            Setting_UI.reflection_eventtocore.AccountsAndCloud.DeleteAccountCloud(cloud[1], type);
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
