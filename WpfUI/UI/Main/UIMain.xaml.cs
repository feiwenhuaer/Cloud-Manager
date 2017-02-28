using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SupDataDll;
using Microsoft.Win32;
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
        #region interface
        public bool AreReloadUI
        {
            get
            {
                return Setting_UI.ReloadUI_Flag;
            }
        }
        public void AddNewCloudToTV(string email, CloudType type)
        {
            ExplorerNode n = new ExplorerNode(new RootNode() { Email = email, Type = type });
            Dispatcher.Invoke(new Action(() => TreeObservableCollection.Add(new TreeViewDataModel(null) { DisplayData = new TreeviewDataItem(n) })));
        }

        public void FileSaveDialog(string InitialDirectory, string FileName, string Filter, ExplorerNode node)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = InitialDirectory;
            sfd.FileName = FileName;
            sfd.Filter = Filter;

            this.Dispatcher.Invoke(new Action(() =>
            {
                if (sfd.ShowDialog().Value)
                {
                    Setting_UI.reflection_eventtocore._AddItem(new List<ExplorerNode>() { node }, node.Parent, ExplorerNode.GetNodeFromDiskPath(sfd.FileName).Parent, false);
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
            Setting_UI.ReloadUI_Flag = false;
            tabControl.SelectionChanged += TabControl_SelectionChanged;
            LoadImage();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Setting_UI.ManagerThreads.CloseAll();
            if (!Setting_UI.ReloadUI_Flag)
            {
                Setting_UI.ExitAPP_Flag = true;
                Setting_UI.reflection_eventtocore._ExitApp();
            }
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
            {
                ExplorerNode n = new ExplorerNode(new RootNode() { Type = CloudType.LocalDisk});
                n.Info.Name = drive.RootDirectory.ToString().TrimEnd('\\');
                TreeObservableCollection.Add(new TreeViewDataModel() { DisplayData = new TreeviewDataItem(n) });
            }
            
        }
        private void TV_LoadCloud()
        {
            foreach (CloudEmail_Type cloud in Setting_UI.reflection_eventtocore._GetListAccountCloud())
            {
                ExplorerNode n = new ExplorerNode(new RootNode() { Email = cloud.Email, Type = cloud.Type });
                TreeObservableCollection.Add(new TreeViewDataModel() { DisplayData = new TreeviewDataItem(n) });
            }
                
        }
        TreeViewDataModel Get_TVDataMoldel(TreeViewItem item)
        {
            return item.Header as TreeViewDataModel;
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
            ((UC_Lv_item)((TabItem)tabControl.Items[tabControl.SelectedIndex]).Content).managerexplorernodes.Root = tv_datamodel.DisplayData.Node.GetRoot();
            ((UC_Lv_item)((TabItem)tabControl.Items[tabControl.SelectedIndex]).Content).managerexplorernodes.Next(tv_datamodel.DisplayData.Node);
            ((UC_Lv_item)((TabItem)tabControl.Items[tabControl.SelectedIndex]).Content).ExplorerCurrentNode(false, true, tv_datamodel, item);
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
            if (item_data_root.DisplayData.Type == CloudType.Folder) throw new Exception("Folder can't be root.");
            bool isroot = item_data_root == item_data;
            foreach (ContextMenuDataModel item in menuitems_source)
            {
                if (item == null) continue;
                switch (item.Key)
                {
                    case LanguageKey.TSMI_cut: if (isroot) item.IsEnabled = false; else item.IsEnabled = true; break;
                    case LanguageKey.TSMI_paste: if (ClipBoard_.Clipboard) item.IsEnabled = true; else item.IsEnabled = false; break;
                    case LanguageKey.TSMI_downloadsellected: if (item_data_root.DisplayData.Type == CloudType.LocalDisk) item.IsEnabled = false; else item.IsEnabled = true; break;
                    case LanguageKey.TSMI_uploadfile: if (item_data_root.DisplayData.Type == CloudType.LocalDisk) item.IsEnabled = false; else item.IsEnabled = true; break;
                    case LanguageKey.TSMI_uploadfolder: if (item_data_root.DisplayData.Type == CloudType.LocalDisk) item.IsEnabled = false; else item.IsEnabled = true; break;
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
            ClipBoard_.directory = model.DisplayData.Node.Parent;
            ClipBoard_.Add(model.DisplayData.Node);
            ClipBoard_.Clipboard = true;
        }

        private void Paste()
        {
            if (!ClipBoard_.Clipboard) return;
            TreeViewDataModel model = treeView.SelectedItem as TreeViewDataModel;
            Setting_UI.reflection_eventtocore._AddItem(ClipBoard_.Items, ClipBoard_.directory, model.DisplayData.Node, ClipBoard_.AreCut);
        }

        private void Delete()
        {
            TreeViewDataModel model = treeView.SelectedItem as TreeViewDataModel;
            MessageBoxResult result;
            switch (model.DisplayData.Type)
            {
                case CloudType.LocalDisk: return;
                case CloudType.Folder:
                    result = System.Windows.MessageBox.Show(    this,
                                                                "Are you want to remove " + model.DisplayData.Node.Info.Name,
                                                                "Confirm",
                                                                MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes) return;
                    Thread thr = new Thread(Setting_UI.reflection_eventtocore._DeletePath);
                    Setting_UI.ManagerThreads.delete.Add(thr);
                    thr.Start(new DeleteItems(model.DisplayData.Node) { PernamentDelete = false });
                    break;

                default:
                    result = System.Windows.MessageBox.Show(   this,
                                                                                "Are you want to remove " + model.DisplayData.Type.ToString() + ":" + model.DisplayData.Node.RootInfo.Email,
                                                                                "Confirm",
                                                                                MessageBoxButton.YesNo,MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes) return;
                    if (Setting_UI.reflection_eventtocore._DeleteAccountCloud(model.DisplayData.Node.RootInfo.Email, model.DisplayData.Type))
                    {
                        TreeObservableCollection.Remove(model);
                    }
                    else System.Windows.MessageBox.Show(    this, 
                                                            "Remove cloud " + model.DisplayData.Type.ToString() + ":" + model.DisplayData.Node.Info.Name + " failed.",
                                                            "Error"
                                                            , MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
        
        #endregion
        #endregion

        #region TabControl
        void Newtab()
        {
            TabItem item = new TabItem();
            ComboBoxHeader cbbh = new ComboBoxHeader();
            UC_Lv_item lvitem = new UC_Lv_item();

            cbbh.IsEnabled = false;
            item.Header = cbbh;
            cbbh.comboBox.SelectionChanged += lvitem.ComboBox_SelectionChanged;

            lvitem.EventListViewFolderDoubleClickCallBack += Lvitem_EventListViewFolderDoubleClickCallBack;
            item.Content = lvitem;
            tabControl.Items.Add(item);
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem item;
            ComboBoxHeader cbb;
            if (e.AddedItems.Count > 0)
            {
                item = e.AddedItems[0] as TabItem;
                if (item != null)
                {
                    cbb = item.Header as ComboBoxHeader;
                    if (cbb != null) cbb.IsEnabled = true;
                }
            }
            if (e.RemovedItems.Count > 0)
            {
                item = e.RemovedItems[0] as TabItem;
                if (item != null)
                {
                    cbb = item.Header as ComboBoxHeader;
                    if (cbb != null) cbb.IsEnabled = false;
                }
            }
        }
        private void ContextMenu_newtab_Click(object sender, RoutedEventArgs e)
        {
            Newtab();
            if (ContextMenu_closetab.IsEnabled == false) ContextMenu_closetab.IsEnabled = true;
        }
        private void ContextMenu_closetab_Click(object sender, RoutedEventArgs e)
        {
            tabControl.Items.Remove(tabControl.Items[tabControl.SelectedIndex]);
            GC.Collect();
            if (tabControl.Items.Count == 1) ContextMenu_closetab.IsEnabled = false;
        }
        #endregion

        #region LVitem
        private void Lvitem_EventListViewFolderDoubleClickCallBack(ExplorerListItem load)
        {
            load.indexLV_tab = tabControl.SelectedIndex;
            Thread thr = ((UC_Lv_item)((TabItem)tabControl.Items[tabControl.SelectedIndex]).Content).thr;
            try { if (thr.IsAlive) thr.Abort(); } catch { }
            thr = new Thread(GetData_TV_LV);
            thr.Start(load);
        }
        #endregion

        #region Image Navigate Click
        void LoadImage()
        {
            image_back.Source = Setting_UI.GetImage(SupDataDll.Properties.Resources.back_icon).Source;
            image_next.Source = Setting_UI.GetImage(SupDataDll.Properties.Resources.next_icon).Source;
            image_search.Source = Setting_UI.GetImage(SupDataDll.Properties.Resources.search_64x64, image_search.Width, image_search.Height).Source;
        }

        private void image_back_MouseUp(object sender, MouseButtonEventArgs e)
        {
            UC_Lv_item lv = ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Content as UC_Lv_item;
            if(lv!= null) if (lv.managerexplorernodes.Back() != null) lv.ExplorerCurrentNode();
        }
        private void image_next_MouseUp(object sender, MouseButtonEventArgs e)
        {
            UC_Lv_item lv = ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Content as UC_Lv_item;
            if (lv != null) if (lv.managerexplorernodes.Next() != null) lv.ExplorerCurrentNode();
        }
        private void image_search_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show(this,"Not support.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        //Set explorer data
        void GetData_TV_LV(object obj)
        {
            ExplorerListItem o = (ExplorerListItem)obj;
            bool exception = false;
            bool flag_arenull = false;
            try
            {
                if (Setting_UI.reflection_eventtocore._ListIteamRequest(o.node) == null) flag_arenull = true;
            }
            catch (ThreadAbortException)
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
                if (flag_arenull || exception )
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        ((tabControl.Items[tabControl.SelectedIndex] as TabItem).Content as UC_Lv_item).managerexplorernodes.Back();
                    }
                    ));
                }
                //Dispatcher.Invoke(new Action(() => CMS_Tabcontrol.Enabled = true));
            }
            if (!exception & !flag_arenull)
            {
                Dispatcher.Invoke(new Action(() => SetData_TV_LV(o)));
            }
        }
        void SetData_TV_LV(ExplorerListItem load)
        {
            if (load.addToTV && load.TV_data != null && load.TV_node != null)//add folder to tree view
            {
                ((TreeViewDataModel)load.TV_data).Childrens.Clear();
                foreach (ExplorerNode n in load.node.Child)
                {
                    if (n.Info.Size > 0) continue;
                    TreeViewDataModel child = new TreeViewDataModel((TreeViewDataModel)load.TV_data) { DisplayData = new TreeviewDataItem(n)};
                    ((TreeViewDataModel)load.TV_data).Childrens.Add(child);
                }
                if (load.explandTV) ((TreeViewItem)load.TV_node).ExpandSubtree();
            }
            ((tabControl.Items[load.indexLV_tab] as TabItem).Content as UC_Lv_item).ShowDataToLV(load.node);
            ((ComboBoxHeader)(tabControl.Items[load.indexLV_tab] as TabItem).Header).Node = load.node;
            //(tabControl.Items[tabControl.SelectedIndex] as TabItem_).ToolTip = load.path;
            //((tabControl.Items[tabControl.SelectedIndex] as TabItem).Content as UC_Lv_item).textBox.Text = load.path.TrimEnd(new char[] { '\\', '/' });
        }


        #region UI menu
        ObservableCollection<ContextMenuDataModel> CloudsAdd;
        ObservableCollection<ContextMenuDataModel> CloudsRemove;
        private void MenuItem_Cloud_Load()
        {
            CloudsAdd = new ObservableCollection<ContextMenuDataModel>();
            CloudsAdd.Add(new ContextMenuDataModel(CloudType.Dropbox));
            CloudsAdd.Add(new ContextMenuDataModel(CloudType.GoogleDrive));
            Cloud_add.ItemsSource = CloudsAdd;
        }
        private void MenuCloud_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            CloudsRemove = new ObservableCollection<ContextMenuDataModel>();
            foreach (CloudEmail_Type cloud in Setting_UI.reflection_eventtocore._GetListAccountCloud())
            {
                CloudsRemove.Add(new ContextMenuDataModel(cloud.Email, cloud.Type));
            }
            Cloud_remove.ItemsSource = CloudsRemove;
        }
        private void Cloud_add_click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenuDataModel data = item.DataContext as ContextMenuDataModel;
            Setting_UI.reflection_eventtocore._ShowFormOauth(data.Type);
        }
        private void Cloud_remove_click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenuDataModel data = item.DataContext as ContextMenuDataModel;
            MessageBoxResult result = System.Windows.MessageBox.Show(this, "Are you want to remove " + data.Type.ToString() + ":" + data.Text, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            if (data.Type == CloudType.Folder || data.Type == CloudType.LocalDisk) throw new Exception("Can remove cloud only.");
            if (Setting_UI.reflection_eventtocore._DeleteAccountCloud(data.Text, data.Type))
                foreach (TreeViewDataModel tv_data in TreeObservableCollection)
                    if (tv_data.DisplayData.Node.Info.Name == data.Text && tv_data.DisplayData.Type == data.Type) { TreeObservableCollection.Remove(tv_data); return; }
        }

        ObservableCollection<ContextMenuDataModel> uis;
        ObservableCollection<ContextMenuDataModel> langs;
        private void MenuSetting_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            uis = new ObservableCollection<ContextMenuDataModel>();
            langs = new ObservableCollection<ContextMenuDataModel>();
            string ui_default = Setting_UI.reflection_eventtocore._GetSetting(SettingsKey.UI_dll_file);
            string lang_default = Setting_UI.reflection_eventtocore._GetSetting(SettingsKey.lang);

            foreach (string s in GetList_UI_n_lang.GetListUiFile()) uis.Add(new ContextMenuDataModel(s) { IsEnabled = s == ui_default ? false : true });
            foreach (string s in GetList_UI_n_lang.GetListLangFile()) langs.Add(new ContextMenuDataModel(s) { IsEnabled = s == lang_default ? false : true });

            MenuChangeUI.ItemsSource = uis;
            MenuChangeLang.ItemsSource = langs;
        }
        private void ChangeUI_click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenuDataModel data = item.DataContext as ContextMenuDataModel;
            Setting_UI.reflection_eventtocore._SetSetting(SettingsKey.UI_dll_file, data.Text);
            Setting_UI.reflection_eventtocore._SaveSetting();
            Setting_UI.ReloadUI_Flag = true;
            this.Close();
        }
        private void ChangeLang_click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenuDataModel data = item.DataContext as ContextMenuDataModel;
            Setting_UI.reflection_eventtocore._SetSetting(SettingsKey.lang, data.Text);
            Setting_UI.reflection_eventtocore._SaveSetting();
            LoadLanguage();
        }
        #endregion
    }
}