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

        public void AddNewCloudToTV(string email,CloudName type)
        {
            if (InvokeRequired) Invoke(new Action(() => AddNewCloudToTV_(email, type)));
            else AddNewCloudToTV_(email, type);
        }
        
        public void ShowDialog_()
        {
            this.ShowDialog();
        }

        public void FileSaveDialog(string InitialDirectory, string FileName, string Filter, AnalyzePath rp,string filename,long filesize)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = InitialDirectory;
            sfd.FileName = FileName;
            sfd.Filter = Filter;
            Invoke(new Action(() => 
            {
                DialogResult rs = sfd.ShowDialog();
                if (rs == DialogResult.OK | rs == DialogResult.Yes)
                {
                    AddNewTransferItem uditem = new AddNewTransferItem(filename,rp.ID,"", Type_FileFolder.File,filesize);
                    AnalyzePath ap = new AnalyzePath(sfd.FileName);
                    Setting_UI.reflection_eventtocore._AddItem(new List<AddNewTransferItem>() { uditem }, rp.Path_Raw, ap.Parent, false);
                }
            }));
        }
        #endregion

        void AddNewCloudToTV_(string email, CloudName type)
        {
            TreeNode nodes = new TreeNode(email);
            nodes.ImageIndex = (int)type;
            nodes.SelectedImageIndex = nodes.ImageIndex;
            TV_item.Nodes.Add(nodes);
        }
        
        #region MainForm
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            Setting_UI.ReloadUI_Flag = false;
            LoadLanguage();
            AddNewTabControl();
            TV_item.BeginUpdate();
            foreach (var drive in DriveInfo.GetDrives())
            {
                TreeNode nodes = new TreeNode(drive.RootDirectory.ToString().Replace("\\", null));
                nodes.ImageIndex = 0;
                nodes.SelectedImageIndex = 0;
                nodes.SelectedImageIndex = nodes.ImageIndex;
                TV_item.Nodes.Add(nodes);
            }
            foreach (CloudEmail_Type cloud in Setting_UI.reflection_eventtocore._GetListAccountCloud())
            {
                TreeNode nodes = new TreeNode(cloud.Email);
                nodes.ImageIndex = (int)cloud.Type;
                nodes.SelectedImageIndex = nodes.ImageIndex;
                TV_item.Nodes.Add(nodes);
            }
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
            //int i = 0;
            //foreach (Control ct in tabControl1.Controls)
            //{
            //    ct.TabIndex = i;
            //    i++;
            //}
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
            OpenPath(true,e);
        }
        private void OpenPath(bool explandTV, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                if (e.Button == MouseButtons.Right) TV_item.SelectedNode = e.Node;
                return;
            }
            //clean next history
            for (int i = list_UCLVitem[tabControl1.SelectedIndex].HistoryPathID_index + 1; i < list_UCLVitem[tabControl1.SelectedIndex].HistoryPathID.Count; i++)
            {
                list_UCLVitem[tabControl1.SelectedIndex].HistoryPathID.RemoveAt(i);
            }
            int img_index = GetRootParent(e.Node).ImageIndex;
            OldPathLV pathnext = new OldPathLV(null, img_index > 1 ? ((CloudName)img_index).ToString() + ":" + e.Node.FullPath.Replace("\\", "/") : e.Node.FullPath);
            list_UCLVitem[tabControl1.SelectedIndex].Clear();
            list_UCLVitem[tabControl1.SelectedIndex].HistoryPathID.Add(pathnext);
            list_UCLVitem[tabControl1.SelectedIndex].Next(explandTV,true,e.Node);
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
            CutCopy(true);
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutCopy(false);
        }
        private void CutCopy(bool AreCut)
        {
            ClipBoard_.Clear();
            ClipBoard_.AreCut = AreCut;

            AddNewTransferItem item = new AddNewTransferItem(TV_item.SelectedNode.Text,"","", Type_FileFolder.Folder);

            TreeNode parent_node = GetRootParent(TV_item.SelectedNode);
            bool arecloud = parent_node.Text.IndexOf('@') >= 0 ? true : false;
            CloudName type = (CloudName)parent_node.ImageIndex;

            string path = arecloud ? type.ToString() + ":" + TV_item.SelectedNode.FullPath.Replace('\\', '/') : TV_item.SelectedNode.FullPath;
            AnalyzePath rp = new AnalyzePath(path);
            ClipBoard_.directory = arecloud ? type.ToString() +":"+ rp.Parent : rp.Parent;

            ClipBoard_.Add(item);
            ClipBoard_.Clipboard = true;
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode parent_node = GetRootParent(TV_item.SelectedNode);
            bool arecloud = parent_node.Text.IndexOf('@') >= 0 ? true : false;
            CloudName type = (CloudName)parent_node.ImageIndex;
            string path = arecloud ? type.ToString() + ":" + TV_item.SelectedNode.FullPath.Replace('\\', '/') : TV_item.SelectedNode.FullPath;

            Setting_UI.reflection_eventtocore._AddItem(ClipBoard_.Items, ClipBoard_.directory, path, ClipBoard_.AreCut);
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
                    TreeNode parent_node = GetRootParent(TV_item.SelectedNode);
                    if (parent_node.ImageIndex == 0) d.TB.Text = TV_item.SelectedNode.FullPath;//disk
                    else d.TB.Text = ((CloudName)parent_node.ImageIndex).ToString() + ":" + TV_item.SelectedNode.FullPath;
                    d.ShowDialog(this);
                    if (d.Delete)
                    {
                        DeleteItems items = new DeleteItems() { items = new List<string>(){ d.TB.Text }, PernamentDelete = d.CB_pernament.Checked };
                        Thread thr = new Thread(Setting_UI.reflection_eventtocore._DeletePath);
                        Setting_UI.ManagerThreads.delete.Add(thr);
                        thr.Start(items);
                        Setting_UI.ManagerThreads.CleanThr();
                    }
                    break;
                default://cloud
                    d.TB.Text = ((CloudName)TV_item.SelectedNode.ImageIndex).ToString() + ":" + TV_item.SelectedNode.FullPath;
                    d.CB_pernament.Enabled = false;
                    d.ShowDialog(this);
                    if (d.Delete)
                    {
                        TV_item.Nodes.Remove(TV_item.SelectedNode);
                        Setting_UI.reflection_eventtocore._DeleteAccountCloud(TV_item.SelectedNode.FullPath, (CloudName)TV_item.SelectedNode.ImageIndex);
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
                List<AddNewTransferItem> list_item_from = new List<AddNewTransferItem>();
                list_item_from.Add(new AddNewTransferItem(TV_item.SelectedNode.Text, "", "", Type_FileFolder.Folder));                
                Setting_UI.reflection_eventtocore._AddItem(list_item_from, ((CloudName)GetRootParent(TV_item.SelectedNode).ImageIndex).ToString() +":"+ TV_item.SelectedNode.Parent.FullPath.Replace('\\','/'),
                    fbd.SelectedPath,false);
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
                List<AddNewTransferItem> list_item_from = new List<AddNewTransferItem>();
                list_item_from.Add(new AddNewTransferItem(TV_item.SelectedNode.Text, "", "", Type_FileFolder.Folder));
                Setting_UI.reflection_eventtocore._AddItem(list_item_from, fbd.SelectedPath,((CloudName)GetRootParent(TV_item.SelectedNode).ImageIndex).ToString() + ":" + TV_item.SelectedNode.Parent.FullPath.Replace('\\', '/'),false);
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
                List<AddNewTransferItem> list_item_from = new List<AddNewTransferItem>();
                string root = Path.GetDirectoryName(ofd.FileNames[0]);
                foreach (string a in ofd.SafeFileNames)
                {
                    FileInfo info = new FileInfo(root + "\\" + a);
                    AddNewTransferItem item = new AddNewTransferItem(a,"","", Type_FileFolder.File, info.Length);
                    list_item_from.Add(item);
                }
                Setting_UI.reflection_eventtocore._AddItem(list_item_from, root,
                    ((CloudName)GetRootParent(TV_item.SelectedNode).ImageIndex).ToString() + ":" + TV_item.SelectedNode.FullPath.Replace('\\', '/'),false);
            }
        }
        private TreeNode GetRootParent(TreeNode node)
        {
            TreeNode parent = node;
            while (parent.Parent != null)
            {
                parent = parent.Parent;
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
            ListItemFileFolder list = new ListItemFileFolder();
            try
            {
                list = Setting_UI.reflection_eventtocore._ListIteamRequest(o.path, o.id);
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
                if (list == null | (exception & list_UCLVitem[o.indexLV_tab].TB_Path.Text.TrimEnd(new char[] { '/', '\\' }) == o.path.TrimEnd(new char[] { '/', '\\' })))
                {
                    Invoke(new Action(() =>
                        {
                            if (list_UCLVitem[o.indexLV_tab].HistoryPathID_index <= 0) list_UCLVitem[o.indexLV_tab].TB_Path.Text = "";
                            else
                            {
                                list_UCLVitem[o.indexLV_tab].HistoryPathID.RemoveAt(list_UCLVitem[o.indexLV_tab].HistoryPathID_index);
                                list_UCLVitem[o.indexLV_tab].HistoryPathID_index--;
                                list_UCLVitem[o.indexLV_tab].TB_Path.Text = list_UCLVitem[o.indexLV_tab].HistoryPathID[list_UCLVitem[o.indexLV_tab].HistoryPathID_index].Path;
                            }
                        }
                    ));
                }
                Invoke(new Action(() => CMS_Tabcontrol.Enabled = true));
            }
            if (!exception & list != null)
            {
                Invoke(new Action(() => SetData_GetList_AddItemTo_LVnTV_(o, list)));
            }
        }
        public void SetData_GetList_AddItemTo_LVnTV_(ExplorerListItem loaditemthread, ListItemFileFolder list)
        {
            string TimeFormat = Setting_UI.reflection_eventtocore._GetSetting(SettingsKey.DATE_TIME_FORMAT);
            bool iscloud = AnalyzePath.IsCloud(list.path_raw);
            if (loaditemthread.addToTV)//add folder to tree view
            {
                ((TreeNode)loaditemthread.TV_node).Nodes.Clear();
                foreach (FileFolder ff in list.Items)
                {
                    if (ff.Size != -1) continue;
                    TreeNode node = new TreeNode(ff.Name);
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                    ((TreeNode)loaditemthread.TV_node).Nodes.Add(node);
                }
                if (loaditemthread.explandTV) ((TreeNode)loaditemthread.TV_node).Expand();
            }
            // Add LV tab index
            if (loaditemthread.indexLV_tab != -1)
            {
                List<ItemLV> ListItem_LV = new List<ItemLV>();
                DateTime temp = new DateTime();
                Icon icon_folder = IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Closed);
                foreach (FileFolder ff in list.Items)
                {
                    if (ff.Size != -1) continue;
                    string datetime = "";
                    if (ff.Time_mod != temp) { try { datetime = ff.Time_mod.ToString(TimeFormat); } catch { } }
                    ListItem_LV.Add(new ItemLV() { str = new string[] { ff.Name, "Folder", string.Empty, datetime, ff.mimeType, ff.id }, icon = icon_folder });
                }
                foreach (FileFolder ff in list.Items)
                {
                    if (ff.Size == -1) continue;
                    string[] splitPath = ff.Name.Split(new Char[] { '.' });
                    string extension = (string)splitPath.GetValue(splitPath.GetUpperBound(0));
                    if (string.IsNullOrEmpty(extension)) extension = ff.Name;
                    
                    ListItem_LV.Add(new ItemLV() { str = new string[] { ff.Name, "File",ff.Size.ToString(), ff.Time_mod.ToString(TimeFormat), ff.mimeType, ff.id },
                                    icon = iscloud ? IconReader.GetFileIcon("." + extension, IconReader.IconSize.Small, false) : null,
                                    filepath = iscloud ? string.Empty : (list.path_raw + "\\" + ff.Name).Replace("\\\\","\\")});
                    
                }
                list_UCLVitem[loaditemthread.indexLV_tab].AddListViewItem(ListItem_LV);
            }
            //set tab text name
            string tabname = "";
            if (!string.IsNullOrEmpty(list.NameFolder)) tabname = list.NameFolder;
            else if (loaditemthread.path.IndexOf('/') >= 0) { string[] splitPath = loaditemthread.path.Split('/'); tabname = (string)splitPath.GetValue(splitPath.GetUpperBound(0)); }
            else if (loaditemthread.path.IndexOf('\\') >= 0) { string[] splitPath = loaditemthread.path.Split('\\'); tabname = (string)splitPath.GetValue(splitPath.GetUpperBound(0)); }
            else { tabname = loaditemthread.path; }
            tabControl1.TabPages[loaditemthread.indexLV_tab].Text = tabname;
            //set tooltip tab
            tabControl1.TabPages[loaditemthread.indexLV_tab].ToolTipText = loaditemthread.path;
            //set Path TextBox
            list_UCLVitem[loaditemthread.indexLV_tab].TB_Path.Text = loaditemthread.path.TrimEnd(new char[] { '\\', '/' });

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
            CloudName type = (CloudName)Enum.Parse(typeof(CloudName), cloud[0]);
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
