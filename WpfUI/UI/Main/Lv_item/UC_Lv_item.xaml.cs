using Etier.IconHelper;
using SupDataDll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfUI;
using WpfUI.Class;

namespace WpfUI.UI.Main.Lv_item
{
    /// <summary>
    /// Interaction logic for UC_Lv_item.xaml
    /// </summary>
    public partial class UC_Lv_item : System.Windows.Controls.UserControl
    {
        public Thread thr;
        public UC_Lv_item()
        {
            InitializeComponent();
            this.Height = double.NaN;
            this.Width = double.NaN;
            LV_items.Height = double.NaN;
            LV_items.Width = double.NaN;

            image_back.Source = Setting_UI.GetImage(SupDataDll.Properties.Resources.back_icon).Source;
            image_next.Source = Setting_UI.GetImage(SupDataDll.Properties.Resources.next_icon).Source;
            image_search.Source = Setting_UI.GetImage(SupDataDll.Properties.Resources.search_64x64, image_search.Width, image_search.Height).Source;
            managerexplorernodes = new ManagerExplorerNodes();
            timeformat = Setting_UI.reflection_eventtocore._GetSetting(SettingsKey.DATE_TIME_FORMAT);
            time_default = new DateTime();
            UILanguage();
            lv_data = new ObservableCollection<LV_data>();
            LV_items.ItemsSource = lv_data;
        }
        string timeformat;
        DateTime time_default;
        void UILanguage()
        {
            label.Content = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TB_path);
            col_path.Width = new GridLength(label.Width + 2);
            LoadContextMenuListview();
        }

        #region Binding Listview 
        ObservableCollection<LV_data> lv_data { get; set; }
        public void ShowDataToLV(ExplorerNode parent)
        {
            lv_data.Clear();
            foreach (ExplorerNode item in parent.Child)
            {
                LV_data dt = new LV_data();
                dt.Node = item;
                if (item.Info.DateMod != time_default) dt.d_mod = item.Info.DateMod.ToString(timeformat);
                if (item.Info.Size > 0)
                {
                    dt.SizeString = UnitConventer.ConvertSize(item.Info.Size, 2, UnitConventer.unit_size);

                    string extension = item.GetExtension();
                    dt.ImgSource = Setting_UI.GetImage(
                                                        item.GetRoot().RootInfo.Type == CloudType.LocalDisk ?
                                                            IconReader.GetFileIcon(item.GetFullPathString(), IconReader.IconSize.Small, false) ://some large file make slow.
                                                            IconReader.GetFileIcon("." + extension, IconReader.IconSize.Small, false)
                        ).Source;
                }
                else
                {
                    dt.SizeString = "-1";
                    dt.ImgSource = Setting_UI.GetImage(IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Closed)).Source;
                }
                lv_data.Add(dt);
            }
        }
        #endregion

        #region Navigate
        public delegate void ListViewFolderDoubleClickCallBack(ExplorerListItem load);
        public event ListViewFolderDoubleClickCallBack EventListViewFolderDoubleClickCallBack;
        public ManagerExplorerNodes managerexplorernodes;
        public void ExplorerCurrentNode(bool explandTV = false, bool addToTV = false, TreeViewDataModel DataItem = null, TreeViewItem TV_item = null)
        {
            ExplorerListItem load = new ExplorerListItem();
            load.node = managerexplorernodes.NodeWorking();
            load.explandTV = explandTV;
            load.addToTV = addToTV;
            if (TV_item != null) load.TV_node = TV_item;
            if (DataItem != null) load.TV_data = DataItem;
            EventListViewFolderDoubleClickCallBack(load);
        }
        void Back()
        {
            if (managerexplorernodes.Back() != null)
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

        void OpenItemLV()
        {
            if (LV_items.SelectedItems.Count != 1) return;
            LV_data data = LV_items.SelectedItem as LV_data;
            if (data != null)
            {
                managerexplorernodes.Next(data.Node);
                ExplorerCurrentNode();
            }
        }
        #endregion

        #region Image Navigate Click
        private void image_back_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Back();
        }
        private void image_next_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Next();
        }
        private void image_search_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
        #endregion

        #region ContextMenu Listview
        ObservableCollection<ContextMenuDataModel> menuitemsource;
        public void LoadContextMenuListview()
        {
            menuitemsource = new ObservableCollection<ContextMenuDataModel>();
            menuitemsource.Add(new ContextMenuDataModel(LanguageKey.TSMI_refresh));
            menuitemsource.Add(new ContextMenuDataModel(LanguageKey.TSMI_open));
            menuitemsource.Add(null);
            menuitemsource.Add(new ContextMenuDataModel(LanguageKey.TSMI_cut));
            menuitemsource.Add(new ContextMenuDataModel(LanguageKey.TSMI_copy));
            menuitemsource.Add(new ContextMenuDataModel(LanguageKey.TSMI_paste));
            menuitemsource.Add(new ContextMenuDataModel(LanguageKey.TSMI_rename));
            menuitemsource.Add(new ContextMenuDataModel(LanguageKey.TSMI_delete));
            menuitemsource.Add(null);
            menuitemsource.Add(new ContextMenuDataModel(LanguageKey.TSMI_createfolder));
            menuitemsource.Add(new ContextMenuDataModel(LanguageKey.TSMI_copyid));
            menuitemsource.Add(null);
            menuitemsource.Add(new ContextMenuDataModel(LanguageKey.TSMI_downloadsellected));
            menuitemsource.Add(new ContextMenuDataModel(LanguageKey.TSMI_uploadfolder));
            menuitemsource.Add(new ContextMenuDataModel(LanguageKey.TSMI_uploadfile));
            LV_items.ContextMenu.ItemsSource = menuitemsource;
        }
        private void LVitems_ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            int selected_count = LV_items.SelectedItems.Count;
            foreach (ContextMenuDataModel data in LV_items.ContextMenu.Items)
            {
                if (data == null) continue;
                if (managerexplorernodes.NodeWorking() == null) { data.IsEnabled = false; continue; }
                switch (data.Key)
                {
                    case LanguageKey.TSMI_refresh: data.IsEnabled = true; break;
                    case LanguageKey.TSMI_open: if (selected_count != 1) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_cut: if (selected_count == 0) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_copy: if (selected_count == 0) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_paste: if (selected_count > 1 | !ClipBoard_.Clipboard) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_rename: if (selected_count != 1) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_delete: if (selected_count == 0) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_copyid: if (selected_count != 1 || managerexplorernodes.Root.RootInfo.Type == CloudType.LocalDisk) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_downloadsellected: if (selected_count == 0) data.IsEnabled = false; else data.IsEnabled = true; break;
                    default: continue;
                }
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuitem = sender as System.Windows.Controls.MenuItem;
            if (menuitem != null && menuitem.Header != null && menuitem.DataContext is ContextMenuDataModel)
            {
                ContextMenuDataModel menu_data_blind = menuitem.DataContext as ContextMenuDataModel;
                switch (menu_data_blind.Key)
                {
                    case LanguageKey.TSMI_refresh: ExplorerCurrentNode(); return;
                    case LanguageKey.TSMI_open: OpenItemLV(); return;
                    case LanguageKey.TSMI_cut: CutCopy(true); return;
                    case LanguageKey.TSMI_copy: CutCopy(false); return;
                    case LanguageKey.TSMI_paste: Paste(); return;
                    case LanguageKey.TSMI_rename: Rename(); return;
                    case LanguageKey.TSMI_delete: Delete(); return;
                    case LanguageKey.TSMI_createfolder: CreateFolder(); return;
                    case LanguageKey.TSMI_copyid: System.Windows.Clipboard.SetText((LV_items.SelectedItem as LV_data).Node.Info.ID); return;
                    case LanguageKey.TSMI_downloadsellected: DownloadSelected(); return;
                    case LanguageKey.TSMI_uploadfolder: uploadfolder(); return;
                    case LanguageKey.TSMI_uploadfile: uploadfile(); return;
                    default: throw new Exception("Not found MenuItem: " + menu_data_blind.Key.ToString());
                }
            }
        }
        void CutCopy(bool cut)
        {
            ClipBoard_.Clear();
            ClipBoard_.AreCut = cut;
            ClipBoard_.directory = managerexplorernodes.NodeWorking();
            foreach (LV_data item in LV_items.SelectedItems) ClipBoard_.Add(item.Node);
            ClipBoard_.Clipboard = true;
        }
        void Paste()
        {
            ExplorerNode roottonode = managerexplorernodes.NodeWorking();
            Setting_UI.reflection_eventtocore._AddItem(ClipBoard_.Items, ClipBoard_.directory, roottonode, ClipBoard_.AreCut);
        }
        void Rename()
        {
            LV_data data = LV_items.SelectedItem as LV_data;
            RenameItem rename = new RenameItem(data.Node);
            rename.Show();
        }
        void Delete(bool PernamentDelete = false)
        {
            DeleteItems items = new DeleteItems();
            foreach (LV_data data in LV_items.SelectedItems) items.Items.Add(data.Node);
            items.PernamentDelete = PernamentDelete;
            Thread thr = new Thread(Setting_UI.reflection_eventtocore._DeletePath);
            Setting_UI.ManagerThreads.delete.Add(thr);
            thr.Start(items);
        }
        void CreateFolder()
        {
            UICreateFolder ui_cf = new UICreateFolder(managerexplorernodes.NodeWorking());
            ui_cf.ShowDialog();
        }
        void DownloadSelected()
        {
            if (LV_items.SelectedIndex < 1) return;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.ShowNewFolderButton = true;
            DialogResult result = fbd.ShowDialog();
            if (result != DialogResult.OK | result != DialogResult.Yes) return;
            List<ExplorerNode> listitems = new List<ExplorerNode>();
            foreach (LV_data item in LV_items.SelectedItems) listitems.Add(item.Node);
            Setting_UI.reflection_eventtocore._AddItem(listitems, managerexplorernodes.NodeWorking(), ExplorerNode.GetNodeFromDiskPath(fbd.SelectedPath), false);
        }
        void uploadfolder()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.ShowNewFolderButton = true;
            DialogResult result = fbd.ShowDialog();
            if (result != DialogResult.OK | result != DialogResult.Yes) return;
            ExplorerNode node = ExplorerNode.GetNodeFromDiskPath(fbd.SelectedPath);

            Setting_UI.reflection_eventtocore._AddItem(new List<ExplorerNode>() { node }, node.Parent, managerexplorernodes.NodeWorking(), false);
        }
        void uploadfile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = PCPath.FilterAllFiles;
            ofd.InitialDirectory = PCPath.Mycomputer;
            DialogResult result = ofd.ShowDialog();
            if (result != DialogResult.OK | result != DialogResult.Yes) return;
            List<ExplorerNode> items = new List<ExplorerNode>();
            ExplorerNode root = ExplorerNode.GetNodeFromDiskPath(System.IO.Path.GetDirectoryName(ofd.FileNames[0]));
            foreach (string s in ofd.SafeFileNames)
            {
                ExplorerNode n = new ExplorerNode();
                n.Info.Name = s;
                root.AddChild(n);
                FileInfo info = new FileInfo(n.GetFullPathString());
                n.Info.Size = info.Length;
                n.Info.DateMod = info.LastWriteTime;
                items.Add(n);
            }
            Setting_UI.reflection_eventtocore._AddItem(items, root, managerexplorernodes.NodeWorking(), false);
        }

        #endregion

        #region ListView Event
        private void LV_items_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LV_data data = LV_items.SelectedItem as LV_data;
            if (data == null) return;
            if (data.Node.Info.Size < 1)//folder
            {
                managerexplorernodes.Next(data.Node);
                ExplorerCurrentNode();
            }
            else
            {
                if (data.Node.GetRoot().RootInfo.Type == CloudType.LocalDisk || string.IsNullOrEmpty(data.Node.GetRoot().RootInfo.Email))
                {
                    System.Diagnostics.Process.Start(data.Node.GetFullPathString());
                }
                else
                {
                    //
                }
            }
        }

        private void LV_items_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LV_items.SelectedItems.Clear();
        }
        #endregion

        private void textBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //    if(e.Key == Key.Enter)
            //    {
            //        OldPathLV nextpath = new OldPathLV("", textBox.Text);
            //        Clear();
            //        HistoryPathID.Add(nextpath);
            //        Next();
            //    }
        }
    }
}