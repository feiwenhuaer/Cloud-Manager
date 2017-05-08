using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using CloudManagerGeneralLib;
using Etier.IconHelper;
using CloudManagerGeneralLib.Class;

namespace FormUI.UI.MainForm
{
    public partial class UC_LVitem : UserControl
    {
        public System.Windows.Forms.Form MainForm;
        private ImageList _SmallImageList = new ImageList();
        private ImageList _LargeImageList = new ImageList();
        public IconListManager _IconListManager;
        public Thread threxplorer;
        
        int index_collumn_name;
        int index_collumn_Type;
        int index_collumn_Id;
        int index_cullumn_size;
        int index_cullumn_mimeType;
        
        public UC_LVitem()
        {
            InitializeComponent();
        }

        private void UC_LVitem_Load(object sender, EventArgs e)
        {
            _SmallImageList.ColorDepth = ColorDepth.Depth32Bit;
            _LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
            _SmallImageList.ImageSize = new System.Drawing.Size(16, 16);
            _LargeImageList.ImageSize = new System.Drawing.Size(32, 32);
            LV_item.SmallImageList = _SmallImageList;
            LV_item.LargeImageList = _LargeImageList;
            //thr_openpath = new Thread(OpenPath);

            index_collumn_name = LV_item.Columns["LV_CH_Name"].Index;
            index_collumn_Type = LV_item.Columns["LV_CH_Type"].Index;
            index_collumn_Id = LV_item.Columns["LV_CH_Id"].Index;
            index_cullumn_size = LV_item.Columns["LV_CH_Size"].Index;
            index_cullumn_mimeType = LV_item.Columns["LV_CH_mimeType"].Index;
            managerexplorernodes = new ManagerExplorerNodes();
            //TB_Path.Text = "https://drive.google.com/drive/u/0/folders/0B-yiWN2AF_cIeHZaTWVsU2duSVU";
            pathUC1.EventNodePathClick += PathUC1_EventNodePathClick;
        }

        private void PathUC1_EventNodePathClick(ExplorerNode nodeclick)
        {
            managerexplorernodes.Next(nodeclick);
            ExplorerCurrentNode();
        }

        public void AddListViewItem(List<ItemLV> list)
        {
            LV_item.Items.Clear();
            _IconListManager = new IconListManager(_SmallImageList, _LargeImageList);
            foreach (ItemLV item in list)
            {
                LV_item.Items.Add(new ListViewItem(item.str, _IconListManager.AddIcon(item.icon)));
            }
        }

        public void LoadLanguage()
        {
            label1.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TB_path);

            for (int i = 0; i < LV_item.Columns.Count; i++)
            {
                LV_item.Columns[i].Text = Setting_UI.reflection_eventtocore.GetTextLanguage("LVitem_Columns_" + i.ToString());
            }

            refreshToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_refresh);
            openToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_open);
            cutToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_cut);
            copyToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_copy);
            pasteToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_paste);
            renameToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_rename);
            deleteToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_delete);
            createFolderToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_createfolder);
            copyIDToClipboardToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_copyid);
            dowloadSeletedToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_downloadsellected);
            uploadFileToHereToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_uploadfile);
            uploadFolderToHereToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_uploadfolder);
        }

        #region ListView Event
        private void LV_item_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //MessageBox.Show(LV_item.SelectedItems[0].SubItems[5].Text);
                //Clipboard.SetText(LV_item.SelectedItems[0].SubItems[5].Text);
            }
        }
        private void LV_item_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (LV_item.SelectedItems.Count != 1) return;
                OpenItemLV();
            }
        }
        private void LV_item_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Back:Back(); break;
                case Keys.Delete:
                case Keys.Control | Keys.C:
                case Keys.Control | Keys.X:
                case Keys.Control | Keys.V:
                default:return;
            }
        }
        #endregion

        #region PictureBox
        private void PB_Back_Click(object sender, EventArgs e)
        {
            Back();
        }
        private void PB_Next_Click(object sender, EventArgs e)
        {
            Next();
        }
        private void PB_Search_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Navigate
        public delegate void ListViewFolderDoubleClickCallBack(ExplorerListItem load);
        public event ListViewFolderDoubleClickCallBack EventListViewFolderDoubleClickCallBack;
        public ManagerExplorerNodes managerexplorernodes;
        public void ExplorerCurrentNode(bool explandTV = false, bool addToTV = false, TreeNode Tnode = null)
        {
            ExplorerListItem load = new ExplorerListItem();
            load.node = managerexplorernodes.NodeWorking();
            load.explandTV = explandTV;
            load.addToTV = addToTV;
            if (Tnode != null) load.TV_node = Tnode;
            EventListViewFolderDoubleClickCallBack(load);
        }
        void Back()
        {
            if(managerexplorernodes.Back() != null)
            {
                ExplorerCurrentNode();
            }
        }
        void Next()
        {
            if (managerexplorernodes.Next() != null)
            {
                ExplorerCurrentNode();
            }
        }

        private void OpenItemLV()
        {
            if (LV_item.SelectedItems.Count != 1) return;
            ExplorerNode find = FindNodeLV(LV_item.SelectedItems[0]);
            if (find != null)
            {
                if (find.Info.Size > 0)//file
                {
                    if (find.GetRoot().RootInfo.Type != CloudType.LocalDisk)//cloud
                    {
                        MessageBox.Show("Not support now.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else//disk
                    {
                        System.Diagnostics.Process.Start(find.GetFullPathString());
                    }
                }
                else//folder
                {
                    managerexplorernodes.Next(find);
                    ExplorerCurrentNode();
                }
            }
        }
        #endregion

        //private void TB_Path_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        AnalyzePath ap = new AnalyzePath(TB_Path.Text);
        //        Clear();
        //        HistoryPathID.Add(new OldPathLV(string.Empty, TB_Path.Text));
        //        Next();
        //    }
        //}

        #region Menu R click
        private void CMS_LVitem_Opening(object sender, CancelEventArgs e)
        {
            if(managerexplorernodes.NodeWorking() != null) refreshToolStripMenuItem.Enabled = true;
            else refreshToolStripMenuItem.Enabled = false;

            copyIDToClipboardToolStripMenuItem.Enabled = false;
            openToolStripMenuItem.Enabled = false;
            renameToolStripMenuItem.Enabled = false;
            switch(LV_item.SelectedItems.Count)
            {
                case 0:
                    cutToolStripMenuItem.Enabled = false;
                    copyToolStripMenuItem.Enabled = false;
                    dowloadSeletedToolStripMenuItem.Enabled = false;
                    deleteToolStripMenuItem.Enabled = false;
                    pasteToolStripMenuItem.Enabled = AppClipboard.Clipboard;
                    if (managerexplorernodes.NodeWorking() != null) SetUpload_TSMI(true);
                    break;

                case 1:
                    openToolStripMenuItem.Enabled = true;
                    renameToolStripMenuItem.Enabled = true;
                    copyIDToClipboardToolStripMenuItem.Enabled = true;
                    pasteToolStripMenuItem.Enabled = AppClipboard.Clipboard;
                    if (managerexplorernodes.NodeWorking() != null) SetUpload_TSMI(true);
                    SetCutCopyDeleteDownload_TSMI();
                    break;

                default: // >1
                    pasteToolStripMenuItem.Enabled = false;
                    if (managerexplorernodes.NodeWorking() == null) SetUpload_TSMI(false);
                    SetCutCopyDeleteDownload_TSMI();
                    break; 
            }
        }
        void SetUpload_TSMI(bool flag)
        {
            uploadFileToHereToolStripMenuItem.Enabled = flag;
            uploadFolderToHereToolStripMenuItem.Enabled = flag;
        }
        void SetCutCopyDeleteDownload_TSMI()
        {
            cutToolStripMenuItem.Enabled = true;
            copyToolStripMenuItem.Enabled = true;
            deleteToolStripMenuItem.Enabled = true;
            if (!string.IsNullOrEmpty(managerexplorernodes.Root.RootInfo.Email)) dowloadSeletedToolStripMenuItem.Enabled = true;
            else dowloadSeletedToolStripMenuItem.Enabled = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenItemLV();
        }
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddClipboard_(true);
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddClipboard_(false);
        }
        void AddClipboard_(bool AreCut)
        {
            AppClipboard.Clear();
            AppClipboard.AreCut = AreCut;
            AppClipboard.directory = managerexplorernodes.NodeWorking();
            bool AreCloud = !string.IsNullOrEmpty(managerexplorernodes.Root.RootInfo.Email);
            foreach (ListViewItem item in LV_item.SelectedItems)
            {
                ExplorerNode node = FindNodeLV(item);
                if (node != null) AppClipboard.Add(node);
            }
            AppClipboard.Clipboard = true;
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExplorerNode rootto = managerexplorernodes.NodeWorking();
            if (LV_item.SelectedItems.Count == 1)
            {
                ExplorerNode find = FindNodeLV(LV_item.SelectedItems[0]);
                if (find != null) rootto = find;
            }
            Setting_UI.reflection_eventtocore.TransferItems(AppClipboard.Items, AppClipboard.directory, rootto, AppClipboard.AreCut);
        }
        ExplorerNode FindNodeLV(ListViewItem item)
        {
            foreach (ExplorerNode n in managerexplorernodes.NodeWorking().Child)
            {
                if (string.IsNullOrEmpty(n.Info.ID))// disk
                {
                    if (n.Info.Name == item.SubItems[index_collumn_name].Text) return n;
                }
                else
                {
                    if(n.Info.ID == item.SubItems[index_collumn_Id].Text) return n;
                }
            }
            return null;
        }
        
        private void dowloadSeletedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.ShowNewFolderButton = true;
            DialogResult result = fbd.ShowDialog(MainForm);
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                List<ExplorerNode> list_item_from = new List<ExplorerNode>();
                foreach (ListViewItem item in LV_item.SelectedItems)
                {
                    ExplorerNode find = FindNodeLV(item);
                    if(find != null) list_item_from.Add(find);
                }
                Setting_UI.reflection_eventtocore.TransferItems(list_item_from, managerexplorernodes.NodeWorking(), ExplorerNode.GetNodeFromDiskPath(fbd.SelectedPath), false);
            }
        }
        private void uploadFolderToHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.ShowNewFolderButton = true;
            DialogResult result = fbd.ShowDialog(MainForm);
            if (result == DialogResult.OK | result == DialogResult.Yes)
            {
                List<ExplorerNode> list_item_from = new List<ExplorerNode>();
                ExplorerNode node = ExplorerNode.GetNodeFromDiskPath(fbd.SelectedPath.TrimEnd('\\'));
                list_item_from.Add(node);

                ExplorerNode rootto = managerexplorernodes.NodeWorking();
                if(LV_item.SelectedItems.Count == 1)
                {
                    ExplorerNode find = FindNodeLV(LV_item.SelectedItems[0]);
                    if (find != null && find.Info.Size <= 0) rootto = find;
                }
                Setting_UI.reflection_eventtocore.TransferItems(list_item_from, node.Parent, rootto, false);
            }
        }
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExplorerCurrentNode();
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string item = "";
            List<ExplorerNode> item_arr = new List<ExplorerNode>();
            for (int i = 0; i < LV_item.SelectedItems.Count; i++)
            {
                ExplorerNode find = FindNodeLV(LV_item.SelectedItems[i]);
                if (find != null)
                {
                    item_arr.Add(find);
                    item += find.Info.Name + "\r\n";
                }
            }
            DeleteConfirmForm f = new DeleteConfirmForm();
            f.TB.Text = item;
            f.ShowDialog(this);
            if (f.Delete)
            {
                DeleteItems items = new DeleteItems() { Items = item_arr, PernamentDelete = f.CB_pernament.Checked };
                Setting_UI.reflection_eventtocore.DeletePath(items);
            }
        }
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //RenameItem rename = new RenameItem(TB_Path.Text + (AnalyzePath.IsCloud(TB_Path.Text) ? "/" : "\\") + LV_item.SelectedItems[0].Text,
            //    LV_item.SelectedItems[0].SubItems[index_collumn_Id].Text, LV_item.SelectedItems[0].Text);
            //rename.Show(MainForm);
        }
        private void copyIDToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LV_item.SelectedItems.Count == 0) { throw new Exception("LV_item.SelectedItems.Count == 0"); }
            Clipboard.SetText(LV_item.SelectedItems[0].SubItems[index_collumn_Id].Text);
        }
        private void createFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateFolderForm f = new CreateFolderForm(managerexplorernodes.NodeWorking());
            f.Show(MainForm);
        }
        #endregion
    }

    public class ItemLV
    {
        public string[] str;
        public Icon icon;
    }
}
