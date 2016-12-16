using Etier.IconHelper;
using SupDataDll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
    public partial class UC_Lv_item : UserControl
    {
        public UC_Lv_item()
        {
            InitializeComponent();
            this.Height = double.NaN;
            this.Width = double.NaN;
            LV_items.Height = double.NaN;
            LV_items.Width = double.NaN;

            image_back.Source = Setting_UI.GetImage(Properties.Resources.back_icon).Source;
            image_next.Source = Setting_UI.GetImage(Properties.Resources.next_icon).Source;
            image_search.Source = Setting_UI.GetImage(Properties.Resources.search_64x64, image_search.Width, image_search.Height).Source;
            UILanguage();
            lv_data = new ObservableCollection<LV_data>();
            LV_items.ItemsSource = lv_data;
        }

        void UILanguage()
        {
            label.Content = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TB_path);
            col_path.Width = new GridLength(label.Width + 2);
            LoadContextMenuListview();
        }

        #region Binding Listview 
        ObservableCollection<LV_data> lv_data { get; set; }
        public void ShowDataToLV(List<FileFolder> data)
        {
            lv_data.Clear();
            string timeformat = Setting_UI.reflection_eventtocore._GetSetting(SettingsKey.DATE_TIME_FORMAT);
            foreach (FileFolder item in data)
            {
                LV_data dt = new LV_data();
                dt.Name = item.Name;
                dt.mimeType = item.mimeType;
                dt.id = item.id;
                dt.d_mod = item.Time_mod.ToString(timeformat);
                dt.Size = item.Size;
                if (item.Size > 0)
                {
                    dt.Type = Type_FileFolder.File;
                    string[] splitPath = item.Name.Split(new Char[] { '.' });
                    string extension = (string)splitPath.GetValue(splitPath.GetUpperBound(0));
                    dt.ImgSource = Setting_UI.GetImage(IconReader.GetFileIcon("." + extension, IconReader.IconSize.Small, false)).Source;
                }
                else
                {
                    dt.Type = Type_FileFolder.Folder;
                    dt.ImgSource = Setting_UI.GetImage(IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Closed)).Source;
                }
                lv_data.Add(dt);
            }
        }
        #endregion

        #region Navigate
        public delegate void ListViewFolderDoubleClickCallBack(ExplorerListItem load);
        public event ListViewFolderDoubleClickCallBack EventListViewFolderDoubleClickCallBack;
        public List<OldPathLV> HistoryPathID = new List<OldPathLV>();
        public int HistoryPathID_index = -1;
        public void Next(bool explandTV = false, bool addToTV = false, TreeViewDataModel DataItem = null, TreeViewItem TV_item = null)
        {
            if (HistoryPathID_index < HistoryPathID.Count - 1)
            {
                HistoryPathID_index++;
                ExplorerListItem load = new ExplorerListItem();
                load.path = HistoryPathID[HistoryPathID_index].Path;
                load.id = HistoryPathID[HistoryPathID_index].ID;
                load.explandTV = explandTV;
                load.addToTV = addToTV;
                if (DataItem != null) load.TV_data = DataItem;
                if (TV_item != null) load.TV_node = TV_item;
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
            if (LV_items.SelectedItems.Count != 1) return;
            LV_data item = LV_items.SelectedItems[0] as LV_data;
            if (item.Type == Type_FileFolder.Folder)
            {
                string path = "";
                if (AnalyzePath.IsUrl(textBox.Text))
                {
                    AnalyzePath rp = new AnalyzePath(textBox.Text);
                    path = rp.ReplaceIDUrl(item.id);
                }
                else
                {
                    path = textBox.Text + (AnalyzePath.IsCloud(textBox.Text) ? "/" : "\\") + item.Name;
                }
                OldPathLV nextpath = new OldPathLV(item.id, path);
                Clear();
                HistoryPathID.Add(nextpath);
                Next();
            }
            else
            {
                if (AnalyzePath.IsCloud(textBox.Text))
                {
                    //download file 
                    return;
                }
                else
                {
                    System.Diagnostics.Process.Start(textBox.Text + "\\" + item.Name);
                }
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

        private void Lvitem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LV_data data = (sender as ListViewItem).DataContext as LV_data;
            if (data.Type == Type_FileFolder.Folder)
            {
                Clear();
                OldPathLV op_lv = new OldPathLV(data.id, textBox.Text + (AnalyzePath.IsCloud(textBox.Text) ? "/" : "\\") + data.Name);
                HistoryPathID.Add(op_lv);
                Next();
            }
        }

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
                if (HistoryPathID_index == -1) { data.IsEnabled = false; continue; }
                switch (data.Key)
                {
                    case LanguageKey.TSMI_open: if (selected_count != 1) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_cut: if (selected_count == 0) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_copy: if (selected_count == 0) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_paste: if (selected_count > 1 | !ClipBoard_.Clipboard) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_rename: if (selected_count != 1) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_delete: if (selected_count == 0) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_copyid: if (selected_count != 1) data.IsEnabled = false; else data.IsEnabled = true; break;
                    case LanguageKey.TSMI_downloadsellected: if (selected_count == 0) data.IsEnabled = false; else data.IsEnabled = true; break;
                    default: continue;
                }
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = sender as MenuItem;
            if (menuitem != null && menuitem.Header != null && menuitem.DataContext is ContextMenuDataModel)
            {
                ContextMenuDataModel menu_data_blind = menuitem.DataContext as ContextMenuDataModel;
                switch (menu_data_blind.Key)
                {
                    case LanguageKey.TSMI_refresh: HistoryPathID_index--; Next(); break;
                    case LanguageKey.TSMI_open: Open(); break;
                    case LanguageKey.TSMI_cut: Add_Clipboard(true); break;
                    case LanguageKey.TSMI_copy: Add_Clipboard(false); break;
                    case LanguageKey.TSMI_paste: Paste(); break;
                    case LanguageKey.TSMI_rename: break;
                    case LanguageKey.TSMI_delete: break;
                    case LanguageKey.TSMI_createfolder: break;
                    case LanguageKey.TSMI_copyid: break;
                    case LanguageKey.TSMI_downloadsellected: break;
                    case LanguageKey.TSMI_uploadfolder: break;
                    case LanguageKey.TSMI_uploadfile: break;
                    default: return;
                }
            }
        }
        void Add_Clipboard(bool cut)
        {
            ClipBoard_.Clear();
            ClipBoard_.AreCut = cut;
            ClipBoard_.directory = textBox.Text;
            foreach (LV_data item in LV_items.SelectedItems)
            {
                UpDownloadItem ud_item = new UpDownloadItem(item.Name, item.id, item.mimeType, item.Type, item.Size);
                ClipBoard_.Add(ud_item);
            }
            ClipBoard_.Clipboard = true;
        }
        void Paste()
        {
            if (AnalyzePath.IsUrl(textBox.Text)) return;
            string savefolder = "";
            if (LV_items.SelectedItems.Count == 0) savefolder = textBox.Text;
            else
            {
                savefolder = textBox.Text + (AnalyzePath.IsCloud(textBox.Text) ? "/" : "\\") + (LV_items.SelectedItems[0] as LV_data).Name;
            }
            Setting_UI.reflection_eventtocore._AddItem(ClipBoard_.Items, ClipBoard_.directory, savefolder, ClipBoard_.AreCut);
        }
        #endregion
    }
}
