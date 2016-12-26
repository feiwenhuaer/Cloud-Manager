﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SupDataDll;
using SupDataDll.UiInheritance;
using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;
using WpfUI.Class;
using WpfUI.UI.Main.Lv_item;
using WpfUI.UI.Main.Lv_ud;

namespace WpfUI.UI.Main
{
    /// <summary>
    /// Interaction logic for UIMan.xaml
    /// </summary>
    public partial class UIMain : Window, SupDataDll.UiInheritance.UIMain
    {
        bool reloadui = false;
        #region interface
        public bool AreReloadUI
        {
            get
            {
                return reloadui;
            }
        }

        public void AddNewCloudToTV(string email, CloudName type)
        {
            if (Dispatcher.CheckAccess())
                Dispatcher.Invoke(new Action(() => TreeObservableCollection.Add(new TreeViewDataModel(null) { DisplayData = new TreeviewDataItem(email, type) })));
            else TreeObservableCollection.Add(new TreeViewDataModel(null) { DisplayData = new TreeviewDataItem(email, type) });            
        }

        public void FileSaveDialog(string InitialDirectory, string FileName, string Filter, AnalyzePath rp, string filename, long filesize)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = InitialDirectory;
            sfd.FileName = FileName;
            sfd.Filter = Filter;

            this.Dispatcher.Invoke(new Action(() =>
            {
                if (sfd.ShowDialog().Value)
                {
                    UpDownloadItem uditem = new UpDownloadItem(filename, rp.ID, "", Type_FileFolder.File, filesize);
                    AnalyzePath ap = new AnalyzePath(sfd.FileName);
                    Setting_UI.reflection_eventtocore._AddItem(new List<UpDownloadItem>() { uditem }, rp.Path_Raw, ap.Parent, false);
                }
            }));
        }

        public void ShowDialog_()
        {
            this.ShowDialog();
        }

        public void load_uC_Lv_ud(SupDataDll.UiInheritance.UIUC_TLV_ud control)
        {
            UC_TLV_ud ct = (UC_TLV_ud)control;
            Grid_Lv_ud.Children.Add(ct);
            ct.Height = Double.NaN;
            ct.Width = Double.NaN;
        }

        #endregion

        public UIMain()
        {
            TreeObservableCollection = new ObservableCollection<TreeViewDataModel>();
            this.DataContext = this;
            InitializeComponent();
            TV_LoadDisk();
            TV_LoadCloud();
            Newtab();
            MenuItem_Cloud_Load();
            LoadLanguage();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Setting_UI.reflection_eventtocore._ExitApp();
        }
        void LoadLanguage()
        {
            LoadMenu_TV();
        }

        #region TV
        public ObservableCollection<TreeViewDataModel> TreeObservableCollection { get; set; }
        private void TV_LoadDisk()
        {
            foreach (var drive in DriveInfo.GetDrives())
                TreeObservableCollection.Add(new TreeViewDataModel(null) { DisplayData = new TreeviewDataItem(drive.RootDirectory.ToString().Replace("\\", null), CloudName.LocalDisk) });
        }
        private void TV_LoadCloud()
        {
            foreach (CloudEmail_Type cloud in Setting_UI.reflection_eventtocore._GetListAccountCloud().account)
                TreeObservableCollection.Add(new TreeViewDataModel(null) { DisplayData = new TreeviewDataItem(cloud.Email, cloud.Type) });
        }
        TreeViewDataModel Get_TVDataMoldel(TreeViewItem item)
        {
            return item.Header as TreeViewDataModel;
        }
        public string GetRoot_TV(TreeViewDataModel pr)
        {
            string path = pr.DisplayData.Name;
            do
            {
                if (pr.Parent != null)
                {
                    pr = pr.Parent;
                    path = pr.DisplayData.Name + "\\" + path;
                }
                else
                {
                    switch (pr.DisplayData.Type)
                    {
                        case CloudName.Folder: throw new Exception("Root can't be folder.");
                        case CloudName.LocalDisk: return path;
                        default: return pr.DisplayData.Type.ToString() + ":" + path.Replace("\\", "/");
                    }
                }
            } while (true);
        }
        TreeViewItem Get_TreeViewItem(StackPanel stackpanel)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(stackpanel);
            while (!(parent is TreeViewItem))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as TreeViewItem;
        }
        private void StackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = Get_TreeViewItem(sender as StackPanel);
            TreeViewDataModel tv_datamodel = Get_TVDataMoldel(item);
            string path = GetRoot_TV(tv_datamodel);
            ((UC_Lv_item)((TabItem_)tabControl.Items[tabControl.SelectedIndex]).Content).Clear();
            ((UC_Lv_item)((TabItem_)tabControl.Items[tabControl.SelectedIndex]).Content).HistoryPathID.Add(new OldPathLV(null, path));
            ((UC_Lv_item)((TabItem_)tabControl.Items[tabControl.SelectedIndex]).Content).Next(e.ClickCount >= 2 ? true : false, true, tv_datamodel, item);
        }
        private void StackPanel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Get_TreeViewItem(sender as StackPanel).Focus();
        }

        #region TV menu 
        ObservableCollection<ContextMenuDataModel> menuitems_source;
        private void LoadMenu_TV()
        {
            menuitems_source = new ObservableCollection<ContextMenuDataModel>();
            menuitems_source.Add(new ContextMenuDataModel(LanguageKey.TSMI_cut));
            menuitems_source.Add(new ContextMenuDataModel(LanguageKey.TSMI_copy));
            menuitems_source.Add(new ContextMenuDataModel(LanguageKey.TSMI_paste));
            menuitems_source.Add(new ContextMenuDataModel(LanguageKey.TSMI_delete));
            menuitems_source.Add(null);
            menuitems_source.Add(new ContextMenuDataModel(LanguageKey.TSMI_downloadsellected));
            menuitems_source.Add(new ContextMenuDataModel(LanguageKey.TSMI_uploadfile));
            menuitems_source.Add(new ContextMenuDataModel(LanguageKey.TSMI_uploadfolder));
            treeView.ContextMenu.ItemsSource = menuitems_source;
        }
        private void treeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            TreeViewDataModel item_data = treeView.SelectedItem as TreeViewDataModel;
            TreeViewDataModel item_data_root = item_data;
            while (item_data_root.Parent != null)
            {
                item_data_root = item_data_root.Parent;
            }
            if (item_data_root.DisplayData.Type == CloudName.Folder) throw new Exception("Folder can't be root.");
            bool isroot = item_data_root == item_data;
            foreach (ContextMenuDataModel item in menuitems_source)
            {
                if (item == null) continue;
                switch (item.Key)
                {
                    case LanguageKey.TSMI_cut: if (isroot) item.IsEnabled = false; else item.IsEnabled = true; break;
                    case LanguageKey.TSMI_paste: if (ClipBoard_.Clipboard) item.IsEnabled = true; else item.IsEnabled = false; break;
                    case LanguageKey.TSMI_downloadsellected: if (item_data_root.DisplayData.Type == CloudName.LocalDisk) item.IsEnabled = false; else item.IsEnabled = true; break;
                    case LanguageKey.TSMI_uploadfile: if (item_data_root.DisplayData.Type == CloudName.LocalDisk) item.IsEnabled = false; else item.IsEnabled = true; break;
                    case LanguageKey.TSMI_uploadfolder: if (item_data_root.DisplayData.Type == CloudName.LocalDisk) item.IsEnabled = false; else item.IsEnabled = true; break;
                    default: continue;
                }
            }
            treeView.ContextMenu.ItemsSource = menuitems_source;
        }
        private void TV_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = sender as MenuItem;
            if (menuitem == null) return;
            ContextMenuDataModel data_menu = menuitem.DataContext as ContextMenuDataModel;
            if (data_menu == null) return;
            switch(data_menu.Key)
            {
                case LanguageKey.TSMI_cut: CutCopy(true); break;
                case LanguageKey.TSMI_copy: CutCopy(false); break;
                case LanguageKey.TSMI_paste: Paste(); break;
                case LanguageKey.TSMI_delete: Delete(); break;
                case LanguageKey.TSMI_downloadsellected: break;
                case LanguageKey.TSMI_uploadfile: break;
                case LanguageKey.TSMI_uploadfolder: break;
                default: return;
            }
        }

        private void CutCopy(bool AreCut)
        {
            ClipBoard_.Clear();
            ClipBoard_.AreCut = AreCut;
            TreeViewDataModel model = treeView.SelectedItem as TreeViewDataModel;
            UpDownloadItem item = new UpDownloadItem(model.DisplayData.Name, "", "", Type_FileFolder.Folder);
            string path = GetRoot_TV(model);
            AnalyzePath ap = new AnalyzePath(path);
            ClipBoard_.directory = ap.Parent;
            ClipBoard_.Add(item);
            ClipBoard_.Clipboard = true;
        }

        private void Paste()
        {
            if (!ClipBoard_.Clipboard) return;
            TreeViewDataModel model = treeView.SelectedItem as TreeViewDataModel;
            string savefolder = GetRoot_TV(model);
            Setting_UI.reflection_eventtocore._AddItem(ClipBoard_.Items, ClipBoard_.directory, savefolder, ClipBoard_.AreCut);
        }

        private void Delete()
        {
            TreeViewDataModel model = treeView.SelectedItem as TreeViewDataModel;
            switch(model.DisplayData.Type)
            {
                case CloudName.LocalDisk: return;
                case CloudName.Folder:
                    Thread thr = new Thread(Setting_UI.reflection_eventtocore._DeletePath);
                    Setting_UI.ManagerThreads.delete.Add(thr);
                    thr.Start(new DeleteItems(GetRoot_TV(model)) { PernamentDelete = false });
                    break;
                default:
                    if (Setting_UI.reflection_eventtocore._DeleteAccountCloud(model.DisplayData.Name, model.DisplayData.Type))
                    {
                        TreeObservableCollection.Remove(model);
                    }
                    else MessageBox.Show(this, "Error", "Remove cloud " + model.DisplayData.Type.ToString() + ":" + model.DisplayData.Name + " failed.", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
        
        #endregion
        #endregion

        #region TabControl
        void Newtab()
        {
            TabItem_ item = new TabItem_();
            item.Header = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.newtab);
            UC_Lv_item lvitem = new UC_Lv_item();
            lvitem.EventListViewFolderDoubleClickCallBack += Lvitem_EventListViewFolderDoubleClickCallBack;
            item.Content = lvitem;
            tabControl.Items.Add(item);
        }
        #endregion

        #region LVitem
        private void Lvitem_EventListViewFolderDoubleClickCallBack(ExplorerListItem load)
        {
            load.indexLV_tab = tabControl.SelectedIndex;
            try { if (((TabItem_)tabControl.Items[tabControl.SelectedIndex]).thr.IsAlive) ((TabItem_)tabControl.Items[tabControl.SelectedIndex]).thr.Abort(); } catch { }
            ((TabItem_)tabControl.Items[tabControl.SelectedIndex]).thr = new Thread(GetData_TV_LV);
            ((TabItem_)tabControl.Items[tabControl.SelectedIndex]).thr.Start(load);
        }
        #endregion


        //Set explorer data
        void GetData_TV_LV(object obj)
        {
            ExplorerListItem o = (ExplorerListItem)obj;
            bool exception = false;
            ListItemFileFolder list = new ListItemFileFolder();
            try
            {
                list = Setting_UI.reflection_eventtocore._ListIteamRequest(o.path, o.id);
            }
            catch (ThreadAbortException ex)
            {
                exception = true;
            }
            catch (IOException ex)
            {
                if (ex.Message == "The device is not ready.\r\n")
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    exception = true;
                }
                else
                {
                    Console.WriteLine("abort (IOException)");
                    exception = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                exception = true;
            }
            finally
            {
                string textboxpath = "";
                Dispatcher.Invoke(new Action(() => textboxpath = ((UC_Lv_item)((TabItem_)tabControl.Items[tabControl.SelectedIndex]).Content).textBox.Text));
                if (list == null | (exception & textboxpath == o.path.TrimEnd(new char[] { '/', '\\' })))
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (((tabControl.Items[tabControl.SelectedIndex] as TabItem_).Content as UC_Lv_item).HistoryPathID_index <= 0) ((tabControl.Items[tabControl.SelectedIndex] as TabItem_).Content as UC_Lv_item).textBox.Text = "";
                        else
                        {
                            ((tabControl.Items[tabControl.SelectedIndex] as TabItem_).Content as UC_Lv_item).HistoryPathID.RemoveAt(((tabControl.Items[tabControl.SelectedIndex] as TabItem_).Content as UC_Lv_item).HistoryPathID_index);
                            ((tabControl.Items[tabControl.SelectedIndex] as TabItem_).Content as UC_Lv_item).HistoryPathID_index--;
                            ((tabControl.Items[tabControl.SelectedIndex] as TabItem_).Content as UC_Lv_item).textBox.Text = ((tabControl.Items[tabControl.SelectedIndex] as TabItem_).Content as UC_Lv_item).HistoryPathID[((tabControl.Items[tabControl.SelectedIndex] as TabItem_).Content as UC_Lv_item).HistoryPathID_index].Path;
                        }
                    }
                    ));
                }
                //Dispatcher.Invoke(new Action(() => CMS_Tabcontrol.Enabled = true));
            }
            if (!exception & list != null)
            {
                Dispatcher.Invoke(new Action(() => SetData_TV_LV(o, list)));
            }
        }
        void SetData_TV_LV(ExplorerListItem load, ListItemFileFolder list)
        {
            bool iscloud = AnalyzePath.IsCloud(list.path_raw);
            if (load.addToTV && load.TV_data != null && load.TV_node != null)//add folder to tree view
            {
                ((TreeViewDataModel)load.TV_data).Childrens.Clear();
                foreach (FileFolder ff in list.Items)
                {
                    if (ff.Size != -1) continue;
                    TreeViewDataModel child = new TreeViewDataModel((TreeViewDataModel)load.TV_data) { DisplayData = new TreeviewDataItem(ff.Name, CloudName.Folder) };
                    ((TreeViewDataModel)load.TV_data).Childrens.Add(child);
                }
                if (load.explandTV) ((TreeViewItem)load.TV_node).ExpandSubtree();
            }
            ((tabControl.Items[tabControl.SelectedIndex] as TabItem_).Content as UC_Lv_item).ShowDataToLV(list.Items);

            string tabname = "";
            if (!string.IsNullOrEmpty(list.NameFolder)) tabname = list.NameFolder;
            else if (load.path.IndexOf('/') >= 0) { string[] splitPath = load.path.Split('/'); tabname = (string)splitPath.GetValue(splitPath.GetUpperBound(0)); }
            else if (load.path.IndexOf('\\') >= 0) { string[] splitPath = load.path.Split('\\'); tabname = (string)splitPath.GetValue(splitPath.GetUpperBound(0)); }
            else { tabname = load.path; }
            (tabControl.Items[tabControl.SelectedIndex] as TabItem_).Header = tabname;
            //(tabControl.Items[tabControl.SelectedIndex] as TabItem_).ToolTip = load.path;
            ((tabControl.Items[tabControl.SelectedIndex] as TabItem_).Content as UC_Lv_item).textBox.Text = load.path.TrimEnd(new char[] { '\\', '/' });
        }

        #region UI menu
        ObservableCollection<ContextMenuDataModel> CloudsAdd;
        private void MenuItem_Cloud_Load()
        {
            CloudsAdd = new ObservableCollection<ContextMenuDataModel>();
            CloudsAdd.Add(new ContextMenuDataModel(CloudName.Dropbox));
            CloudsAdd.Add(new ContextMenuDataModel(CloudName.GoogleDrive));
            Cloud_add.ItemsSource = CloudsAdd;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Cloud_remove.Items.Clear();
            foreach (CloudEmail_Type cloud in Setting_UI.reflection_eventtocore._GetListAccountCloud().account)
            {
                MenuItem item = new MenuItem();
                item.Header = cloud.Email;
                item.Icon = Setting_UI.GetImage(TreeviewDataItem.list_bm[(int)cloud.Type]);//.Source;
                item.Click += Remove_Click;
                Cloud_remove.Items.Add(item);
            }
        }
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Cloud_add_click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenuDataModel data = item.DataContext as ContextMenuDataModel;
            Setting_UI.reflection_eventtocore._ShowFormOauth(data.Type);
        }
        #endregion


    }
    public class TabItem_ : TabItem
    {
        public Thread thr;
    }
}