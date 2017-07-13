using Etier.IconHelper;
using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using WpfUI.Class;

namespace WpfUI.UI.Main.Lv_item
{
  /// <summary>
  /// Interaction logic for UC_Lv_item.xaml
  /// </summary>
  public partial class UC_Lv_item : System.Windows.Controls.UserControl
  {
    public Window main_window;
    public Thread thr;
    public UC_Lv_item()
    {
      InitializeComponent();
      this.Height = double.NaN;
      this.Width = double.NaN;
      LV_items.Height = double.NaN;
      LV_items.Width = double.NaN;

      managerehistory_itemnodes = new ManagerHistoryItemNodes();
      timeformat = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetSetting(SettingsKey.DATE_TIME_FORMAT);
      time_default = new DateTime();
      UILanguage();
      lv_data = new ObservableCollection<LV_data>();
      LV_items.ItemsSource = lv_data;
    }
    string timeformat;
    DateTime time_default;
    void UILanguage()
    {
      LoadContextMenuListview();
    }

    #region Binding Listview 
    ObservableCollection<LV_data> lv_data { get; set; }
    public void ShowDataToLV(IItemNode parent)
    {
      lv_data.Clear();
      foreach (ItemNode item in parent.Childs)
      {
        LV_data dt = new LV_data();
        dt.Node = item;

        if (item.Info.DateMod != time_default) dt.d_mod = item.Info.DateMod.ToString(timeformat);
        if (item.Info.Size >= 0)
        {
          dt.SizeString = UnitConventer.ConvertSize(item.Info.Size, 2, UnitConventer.unit_size);

          string extension = item.GetExtension();
          dt.ImgSource = Setting_UI.GetImage(
              item.GetRoot.RootType.Type == CloudType.LocalDisk ?
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

    public void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ComboBoxData data_add = null;
      ComboBoxData data_remove;
      if (e.AddedItems.Count > 0)
      {
        data_add = e.AddedItems[0] as ComboBoxData;
        if (data_add != null) if (data_add.Node == managerehistory_itemnodes.NodeWorking()) return;
      }
      if (e.RemovedItems.Count > 0 && data_add != null)
      {
        data_remove = e.RemovedItems[0] as ComboBoxData;
        if (data_remove != null)
        {
          if (data_remove.Node == managerehistory_itemnodes.NodeWorking())
          {
            managerehistory_itemnodes.Next(data_add.Node);
            ExplorerCurrentNode();
          }
        }
      }
    }

    #region Navigate
    public delegate void ListViewFolderDoubleClickCallBack(ExplorerListItem load);
    public event ListViewFolderDoubleClickCallBack EventListViewFolderDoubleClickCallBack;
    public ManagerHistoryItemNodes managerehistory_itemnodes;
    public void ExplorerCurrentNode(bool explandTV = false, bool addToTV = false, TreeViewDataModel DataItem = null, TreeViewItem TV_item = null)
    {
      ExplorerListItem load = new ExplorerListItem();
      load.node = managerehistory_itemnodes.NodeWorking();
      load.explandTV = explandTV;
      load.addToTV = addToTV;
      if (TV_item != null) load.TV_node = TV_item;
      if (DataItem != null) load.TV_data = DataItem;
      EventListViewFolderDoubleClickCallBack(load);
    }
    void Back()
    {
      if (managerehistory_itemnodes.Back() != null)
      {
        ExplorerCurrentNode();
      }
    }
    void Next()
    {
      if (managerehistory_itemnodes.Next() != null)
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
        managerehistory_itemnodes.Next(data.Node);
        ExplorerCurrentNode();
      }
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
        if (managerehistory_itemnodes.NodeWorking() == null) { data.IsEnabled = false; continue; }
        switch (data.Key)
        {
          case LanguageKey.TSMI_refresh: data.IsEnabled = true; break;
          case LanguageKey.TSMI_open: data.IsEnabled = !(selected_count != 1); break;
          case LanguageKey.TSMI_cut: data.IsEnabled = !(selected_count == 0); break;
          case LanguageKey.TSMI_copy: data.IsEnabled = !(selected_count == 0); break;
          case LanguageKey.TSMI_paste: data.IsEnabled = !(selected_count > 1 | !AppClipboard.Clipboard); break;
          case LanguageKey.TSMI_rename: data.IsEnabled = !(selected_count != 1); break;
          case LanguageKey.TSMI_delete: data.IsEnabled = !(selected_count == 0); break;
          case LanguageKey.TSMI_copyid: data.IsEnabled = !(selected_count != 1 || managerehistory_itemnodes.Root.RootType.Type == CloudType.LocalDisk); break;
          case LanguageKey.TSMI_downloadsellected: data.IsEnabled = !(selected_count == 0); break;
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
      AppClipboard.Clear();
      AppClipboard.AreCut = cut;
      AppClipboard.directory = managerehistory_itemnodes.NodeWorking();
      foreach (LV_data item in LV_items.SelectedItems) AppClipboard.Add(item.Node);
      AppClipboard.Clipboard = true;
    }
    void Paste()
    {
      IItemNode roottonode = managerehistory_itemnodes.NodeWorking();
      Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.TransferItems(AppClipboard.Items, AppClipboard.directory, roottonode, AppClipboard.AreCut);
    }
    void Rename()
    {
      LV_data data = LV_items.SelectedItem as LV_data;
      RenameItem rename = new RenameItem(data.Node);
      rename.Owner = main_window;
      rename.ShowDialog();
    }
    void Delete()
    {
      MessageBoxResult result = System.Windows.MessageBox.Show("Delete selected items\r\nAre you sure?\r\n\r\nYes: Send To Recycle Bin\r\nNo: Permanently Delete", "Confirm Delete", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
      if (result == MessageBoxResult.Cancel) return;
      DeleteItems items = new DeleteItems();
      foreach (LV_data data in LV_items.SelectedItems) items.Items.Add(data.Node);
      items.PernamentDelete = result == MessageBoxResult.No;
      Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.DeletePath(items);
    }
    void CreateFolder()
    {
      UICreateFolder ui_cf = new UICreateFolder(managerehistory_itemnodes.NodeWorking());
      ui_cf.Owner = this.main_window;
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
      List<IItemNode> listitems = new List<IItemNode>();
      foreach (LV_data item in LV_items.SelectedItems) listitems.Add(item.Node);
      Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.TransferItems(listitems, managerehistory_itemnodes.NodeWorking(), ItemNode.GetNodeFromDiskPath(fbd.SelectedPath), false);
    }
    void uploadfolder()
    {
      FolderBrowserDialog fbd = new FolderBrowserDialog();
      fbd.RootFolder = Environment.SpecialFolder.MyComputer;
      fbd.ShowNewFolderButton = true;
      DialogResult result = fbd.ShowDialog();
      if (result != DialogResult.OK | result != DialogResult.Yes) return;
      IItemNode node = ItemNode.GetNodeFromDiskPath(fbd.SelectedPath);
      Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.TransferItems(new List<IItemNode>() { node }, node.Parent, managerehistory_itemnodes.NodeWorking(), false);
    }
    void uploadfile()
    {
      OpenFileDialog ofd = new OpenFileDialog();
      ofd.Multiselect = true;
      ofd.Filter = PCPath.FilterAllFiles;
      ofd.InitialDirectory = PCPath.Mycomputer;
      DialogResult result = ofd.ShowDialog();
      if (result != DialogResult.OK | result != DialogResult.Yes) return;
      List<IItemNode> items = new List<IItemNode>();
      IItemNode root = ItemNode.GetNodeFromDiskPath(System.IO.Path.GetDirectoryName(ofd.FileNames[0]));
      foreach (string s in ofd.SafeFileNames)
      {
        IItemNode n = new ItemNode();
        n.Info.Name = s;
        root.AddChild(n);
        FileInfo info = new FileInfo(n.GetFullPathString());
        n.Info.Size = info.Length;
        n.Info.DateMod = info.LastWriteTime;
        items.Add(n);
      }
      Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.TransferItems(items, root, managerehistory_itemnodes.NodeWorking(), false);
    }

    #endregion

    #region ListView Event
    private void LV_items_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      LV_data data = LV_items.SelectedItem as LV_data;
      if (data == null) return;
      if (data.Node.Info.Size < 1)//folder
      {
        managerehistory_itemnodes.Next(data.Node);
        ExplorerCurrentNode();
      }
      else
      {
        if (data.Node.GetRoot.RootType.Type == CloudType.LocalDisk || string.IsNullOrEmpty(data.Node.GetRoot.RootType.Email))
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
  }
}