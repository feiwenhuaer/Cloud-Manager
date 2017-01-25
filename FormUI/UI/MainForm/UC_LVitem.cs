using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using SupDataDll;
using Etier.IconHelper;

namespace FormUI.UI.MainForm
{
    public partial class UC_LVitem : UserControl
    {
        public System.Windows.Forms.Form MainForm;
        private ImageList _SmallImageList = new ImageList();
        private ImageList _LargeImageList = new ImageList();
        public IconListManager _IconListManager;
        public Thread threxplorer;

        public List<OldPathLV> HistoryPathID = new List<OldPathLV>();
        public int HistoryPathID_index = -1;

        int index_collumn_name;
        int index_collumn_Type;
        int index_collumn_Id;
        int index_cullumn_size;
        int index_cullumn_mimeType;

        public delegate void ListViewFolderDoubleClickCallBack(ExplorerListItem load);
        public event ListViewFolderDoubleClickCallBack EventListViewFolderDoubleClickCallBack;

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

            TB_Path.Text = "https://drive.google.com/drive/u/0/folders/0B-yiWN2AF_cIeHZaTWVsU2duSVU";
        }

        public void AddListViewItem(List<ItemLV> list)
        {
            LV_item.Items.Clear();
            foreach (ItemLV item in list)
            {
                _IconListManager = new IconListManager(_SmallImageList, _LargeImageList);
                ListViewItem lv_item = new ListViewItem(item.str, string.IsNullOrEmpty(item.filepath) ? _IconListManager.AddIcon(item.icon) : _IconListManager.AddFileIcon(item.filepath));
                LV_item.Items.Add(lv_item);
            }
        }

        public void LoadLanguage()
        {
            label1.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TB_path);

            for (int i = 0; i < LV_item.Columns.Count; i++)
            {
                LV_item.Columns[i].Text = Setting_UI.reflection_eventtocore._GetTextLanguage("LVitem_Columns_" + i.ToString());
            }

            refreshToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_refresh);
            openToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_open);
            cutToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_cut);
            copyToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_copy);
            pasteToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_paste);
            renameToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_rename);
            deleteToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_delete);
            createFolderToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_createfolder);
            copyIDToClipboardToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_copyid);
            dowloadSeletedToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_downloadsellected);
            uploadFileToHereToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_uploadfile);
            uploadFolderToHereToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_uploadfolder);
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
                Open();
            }
        }
        private void LV_item_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                Back();
            }
            if (e.KeyCode == Keys.Delete)
            {

            }
            if (e.KeyCode == (Keys.Control | Keys.C))
            {

            }
            if (e.KeyCode == (Keys.Control | Keys.X))
            {

            }
            if (e.KeyCode == (Keys.Control | Keys.V))
            {

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
        public void Next(bool explandTV = false, bool addToTV = false, TreeNode node = null)
        {

            if (HistoryPathID_index < HistoryPathID.Count - 1)
            {
                HistoryPathID_index++;
                ExplorerListItem load = new ExplorerListItem();
                load.path = HistoryPathID[HistoryPathID_index].Path;
                load.id = HistoryPathID[HistoryPathID_index].ID;
                load.explandTV = explandTV;
                load.addToTV = addToTV;
                if (node != null) load.TV_node = node;
                EventListViewFolderDoubleClickCallBack(load);
            }
        }

        public void Clear()
        {
            if (HistoryPathID_index < 0) return;
            for (int i = HistoryPathID_index + 1; i < HistoryPathID.Count; i++)
            {
                HistoryPathID.RemoveAt(i);
                i--;
            }
        }

        public void Back()
        {
            if (HistoryPathID_index > 0)
            {
                HistoryPathID_index--;
                ExplorerListItem load = new ExplorerListItem();
                load.path = HistoryPathID[HistoryPathID_index].Path;
                load.id = HistoryPathID[HistoryPathID_index].ID;
                EventListViewFolderDoubleClickCallBack(load);
            }
        }

        private void Open()
        {
            if (LV_item.SelectedItems.Count != 1) return;
            if (LV_item.SelectedItems[0].SubItems[index_collumn_Type].Text == "Folder")
            {
                string path = "";
                string id = LV_item.SelectedItems[0].SubItems[5].Text;


                if (!AnalyzePath.IsUrl(TB_Path.Text)) path = TB_Path.Text + (AnalyzePath.IsCloud(TB_Path.Text) ? "/" : "\\") + LV_item.SelectedItems[0].SubItems[0].Text;
                else
                {
                    AnalyzePath rp = new AnalyzePath(TB_Path.Text);
                    path = rp.ReplaceIDUrl(id);
                }

                Clear();
                HistoryPathID.Add(new OldPathLV(id, path));
                Next();
                return;
            }
            if (LV_item.SelectedItems[0].SubItems[index_collumn_Type].Text == "File")
            {
                if (AnalyzePath.IsCloud(TB_Path.Text))
                {
                    //download file 
                    return;
                }
                else
                {
                    System.Diagnostics.Process.Start(TB_Path.Text + "\\" + LV_item.SelectedItems[0].SubItems[0].Text);
                }
                return;
            }
        }
        #endregion

        private void TB_Path_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AnalyzePath ap = new AnalyzePath(TB_Path.Text);
                Clear();
                HistoryPathID.Add(new OldPathLV(ap.ID, TB_Path.Text));
                Next();
            }
        }

        #region Menu R click
        private void CMS_LVitem_Opening(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(TB_Path.Text)) refreshToolStripMenuItem.Enabled = false;
            else refreshToolStripMenuItem.Enabled = true;

            copyIDToClipboardToolStripMenuItem.Enabled = false;
            openToolStripMenuItem.Enabled = false;
            renameToolStripMenuItem.Enabled = false;

            if (LV_item.SelectedItems.Count == 0) // off all
            {
                //createFolderToolStripMenuItem.Enabled = true;
                cutToolStripMenuItem.Enabled = false;
                copyToolStripMenuItem.Enabled = false;
                dowloadSeletedToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = false;
            }
            else// >0
            {
                //createFolderToolStripMenuItem.Enabled = false;
                cutToolStripMenuItem.Enabled = true;
                copyToolStripMenuItem.Enabled = true;
                deleteToolStripMenuItem.Enabled = true;
                if (AnalyzePath.IsCloud(TB_Path.Text)) dowloadSeletedToolStripMenuItem.Enabled = true;
                else dowloadSeletedToolStripMenuItem.Enabled = false;
            }

            //openToolStripMenuItem
            if (LV_item.SelectedItems.Count == 1)
            {
                openToolStripMenuItem.Enabled = true;
                renameToolStripMenuItem.Enabled = true;
                copyIDToClipboardToolStripMenuItem.Enabled = true;
            }

            //uploadFileToHereToolStripMenuItem
            if (LV_item.SelectedItems.Count <= 1 & !string.IsNullOrEmpty(TB_Path.Text))
            {
                uploadFileToHereToolStripMenuItem.Enabled = true;
                uploadFolderToHereToolStripMenuItem.Enabled = true;
            }
            else
            {
                uploadFileToHereToolStripMenuItem.Enabled = false;
                uploadFolderToHereToolStripMenuItem.Enabled = false;
            }
            //pasteToolStripMenuItem

            if (LV_item.SelectedItems.Count > 1) pasteToolStripMenuItem.Enabled = false;
            else pasteToolStripMenuItem.Enabled = ClipBoard_.Clipboard;
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
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
            ClipBoard_.Clear();
            ClipBoard_.AreCut = AreCut;
            ClipBoard_.directory = TB_Path.Text;
            bool AreCloud = AnalyzePath.IsCloud(TB_Path.Text);
            foreach (ListViewItem item in LV_item.SelectedItems)
            {
                Type_FileFolder type = (Type_FileFolder)Enum.Parse(typeof(Type_FileFolder), item.SubItems[index_collumn_Type].Text);
                ClipBoard_.Add(new AddNewTransferItem(item.SubItems[index_collumn_name].Text,
                                                  item.SubItems[index_collumn_Id].Text,
                                                  item.SubItems[index_cullumn_mimeType].Text,
                                                  type,
                                                  type == Type_FileFolder.File ? long.Parse(item.SubItems[index_cullumn_size].Text) : -1
                                                  )
                                );
            }
            ClipBoard_.Clipboard = true;
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnalyzePath rp = new AnalyzePath(TB_Path.Text);
            string path = TB_Path.Text;
            if (LV_item.SelectedItems.Count == 1)
            {
                Type_FileFolder type = (Type_FileFolder)Enum.Parse(typeof(Type_FileFolder), LV_item.SelectedItems[0].SubItems[index_collumn_Type].Text);
                if (type == Type_FileFolder.Folder)
                {
                    path += rp.PathIsCloud ? "/" : "\\" + LV_item.SelectedItems[0].Text;
                }
            }
            Setting_UI.reflection_eventtocore._AddItem(ClipBoard_.Items, ClipBoard_.directory, path, ClipBoard_.AreCut);
        }
        private void dowloadSeletedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.ShowNewFolderButton = true;
            DialogResult result = fbd.ShowDialog(MainForm);
            if (result == DialogResult.OK | result == DialogResult.Yes)
            {
                List<AddNewTransferItem> list_item_from = new List<AddNewTransferItem>();
                foreach (ListViewItem item in LV_item.SelectedItems)
                {
                    Type_FileFolder type = (Type_FileFolder)Enum.Parse(typeof(Type_FileFolder), item.SubItems[index_collumn_Type].Text);
                    AddNewTransferItem dl_item = new AddNewTransferItem(item.SubItems[index_collumn_name].Text,
                                                                item.SubItems[index_collumn_Id].Text,
                                                                item.SubItems[index_cullumn_mimeType].Text,
                                                                type,
                                                                type == Type_FileFolder.File ? long.Parse(item.SubItems[index_cullumn_size].Text) : -1
                                                                );
                    list_item_from.Add(dl_item);
                }
                Setting_UI.reflection_eventtocore._AddItem(list_item_from, TB_Path.Text, fbd.SelectedPath, false);
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
                List<AddNewTransferItem> list_item_from = new List<AddNewTransferItem>();
                string[] pathfrom_arr = fbd.SelectedPath.TrimEnd('\\').Split('\\');
                string parent_directory_from = "";
                for (int i = 0; i < pathfrom_arr.Length - 1; i++)
                {
                    parent_directory_from += pathfrom_arr[i] + "\\";
                }
                if (string.IsNullOrEmpty(parent_directory_from)) parent_directory_from = pathfrom_arr[0];

                AddNewTransferItem dl_item = new AddNewTransferItem(string.IsNullOrEmpty(parent_directory_from) ? "" : pathfrom_arr[pathfrom_arr.Length - 1],
                                                            "",
                                                            "",
                                                            Type_FileFolder.Folder
                                                            );
                list_item_from.Add(dl_item);
                Setting_UI.reflection_eventtocore._AddItem(list_item_from, parent_directory_from.TrimEnd('\\'),
                        LV_item.SelectedItems.Count == 0 ? TB_Path.Text : TB_Path.Text + "/" + LV_item.SelectedItems[0].Text, false);
            }
        }
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistoryPathID_index--;
            Next();
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnalyzePath rp = new AnalyzePath(TB_Path.Text);
            string item = "";
            List<string> item_arr = new List<string>();
            for (int i = 0; i < LV_item.SelectedItems.Count; i++)
            {
                item += TB_Path.Text + (rp.PathIsCloud ? "/" : "\\") + LV_item.SelectedItems[i].Text + "\r\n";
                item_arr.Add(TB_Path.Text + (rp.PathIsCloud ? "/" : "\\") + LV_item.SelectedItems[i].Text);
            }
            DeleteConfirmForm f = new DeleteConfirmForm();
            f.TB.Text = item;
            f.ShowDialog(this);
            if (f.Delete)
            {
                DeleteItems items = new DeleteItems() { items = item_arr, PernamentDelete = f.CB_pernament.Checked };
                Thread thr = new Thread(Setting_UI.reflection_eventtocore._DeletePath);
                Setting_UI.ManagerThreads.delete.Add(thr);
                Setting_UI.ManagerThreads.CleanThr();
                thr.Start(items);
            }
        }
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenameItem rename = new RenameItem(TB_Path.Text + (AnalyzePath.IsCloud(TB_Path.Text) ? "/" : "\\") + LV_item.SelectedItems[0].Text,
                LV_item.SelectedItems[0].SubItems[index_collumn_Id].Text, LV_item.SelectedItems[0].Text);
            rename.Show(MainForm);
        }
        private void copyIDToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LV_item.SelectedItems.Count == 0) { throw new Exception("LV_item.SelectedItems.Count == 0"); }
            Clipboard.SetText(LV_item.SelectedItems[0].SubItems[index_collumn_Id].Text);
        }
        private void createFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateFolderForm f = new CreateFolderForm();
            AnalyzePath ap = new AnalyzePath(TB_Path.Text);
            f.Path = HistoryPathID[HistoryPathID_index].Path;
            f.Id = HistoryPathID[HistoryPathID_index].ID;
            f.Show(MainForm);
        }
        #endregion
    }

    public class ItemLV
    {
        public string[] str;
        public Icon icon;
        public string filepath = "";
    }
}
