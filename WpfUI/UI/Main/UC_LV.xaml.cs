using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WpfUI.Class;

namespace WpfUI.UI.Main
{
  /// <summary>
  /// Interaction logic for UC_TLV.xaml
  /// </summary>
  public partial class UC_LV : UserControl
  {
    public TransferListViewData Datas { get; set; } = new TransferListViewData();
    public UC_LV()
    {
      InitializeComponent();
      //this.DataContext = this;
      lv.ItemsSource = Datas.ItemsBlinding;
      Load_TLVmenu();
    }

    private void lv_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      lv.SelectedItems.Clear();
    }

    #region Menu TLV
    ObservableCollection<ContextMenuDataModel> menuitems_source;
    ContextMenuDataModel change;
    ContextMenuDataModel start;
    ContextMenuDataModel waiting;
    ContextMenuDataModel stop;
    ContextMenuDataModel error;
    ContextMenuDataModel remove;
    ContextMenuDataModel numberOfParallelDownloads;
    void Load_TLVmenu()
    {
      menuitems_source = new ObservableCollection<ContextMenuDataModel>();
      change = new ContextMenuDataModel(LanguageKey.TSMI_ChangeStatus);
      start = new ContextMenuDataModel(LanguageKey.TSMI_start);
      waiting = new ContextMenuDataModel(LanguageKey.TSMI_waiting);
      stop = new ContextMenuDataModel(LanguageKey.TSMI_stop);
      error = new ContextMenuDataModel(LanguageKey.TSMI_error);
      remove = new ContextMenuDataModel(LanguageKey.TSMI_remove);
      numberOfParallelDownloads = new ContextMenuDataModel(LanguageKey.TSMI_numberOfParallelDownloads);

      change.Child.Add(start);
      change.Child.Add(waiting);
      change.Child.Add(stop);
      change.Child.Add(error);

      error.Child.Add(new ContextMenuDataModel(LanguageKey.TSMI_forcestart));
      error.Child.Add(new ContextMenuDataModel(LanguageKey.TSMI_forcewaiting));
      menuitems_source.Add(change);
      menuitems_source.Add(numberOfParallelDownloads);
      menuitems_source.Add(null);
      menuitems_source.Add(remove);
      lv.ContextMenu.ItemsSource = menuitems_source;
    }

    List<TransferItem> items;
    private void lv_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
      start.IsEnabled = false;
      waiting.IsEnabled = false;
      stop.IsEnabled = false;
      error.IsEnabled = false;
      remove.IsEnabled = false;
      numberOfParallelDownloads.IsEnabled = false;

      var seleecteditems = lv.SelectedItems;
      if (seleecteditems != null)
      {
        if (seleecteditems.Count > 0) remove.IsEnabled = true;
        items = new List<TransferItem>();
        foreach (var item in seleecteditems)
        {
          //TreeNode node = item as TreeNode;
          //TransferGroup tg = node.Tag as TransferGroup;
          //TransferItem ti = node.Tag as TransferItem;
          
          //  items.Add(ti);
          //  SetIsEnableMenuTLV(ti as Transfer);
        }
      }
    }
    void SetIsEnableMenuTLV(TransferItem t)
    {
      switch (t.status)
      {
        case StatusTransfer.Waiting: start.IsEnabled = true; stop.IsEnabled = true; break;
        case StatusTransfer.Running: stop.IsEnabled = true; waiting.IsEnabled = true; break;
        case StatusTransfer.Stop: start.IsEnabled = true; waiting.IsEnabled = true; break;
        case StatusTransfer.Error: error.IsEnabled = true; start.IsEnabled = true; waiting.IsEnabled = true; break;
        default: break;
      }
    }


    private void TV_MenuItem_Click(object sender, RoutedEventArgs e)
    {
      MenuItem menu = sender as MenuItem;
      ContextMenuDataModel model = menu.DataContext as ContextMenuDataModel;
      if (model != null && model.Key == LanguageKey.TSMI_numberOfParallelDownloads) ;//ChangeNumberOfParallelDownloads();
      else
      {
        switch (model.Key)
        {
          case LanguageKey.TSMI_start: ChangeStatus(StatusTransfer.Started); break;
          case LanguageKey.TSMI_waiting: ChangeStatus(StatusTransfer.Waiting); break;
          case LanguageKey.TSMI_stop: ChangeStatus(StatusTransfer.Stop); break;
          case LanguageKey.TSMI_remove:
            MessageBoxResult result = MessageBox.Show("Are you sure to remove?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) ChangeStatus(StatusTransfer.Remove);
            break;
          //case LanguageKey.TSMI_forcestart: ErrorSetForce(StatusTransfer.Started); break;
          //case LanguageKey.TSMI_forcewaiting: ErrorSetForce(StatusTransfer.Waiting); break;
        }
      }
      items.Clear();
    }

    void ChangeStatus(StatusTransfer val)
    {
      foreach (TransferItem it in items)
      {
        if (val == StatusTransfer.Started && (it.status == StatusTransfer.Stop || it.status == StatusTransfer.Waiting || it.status == StatusTransfer.Error)) it.status = val;
        else
            //set Stop child
            if (val == StatusTransfer.Stop && (it.status != StatusTransfer.Done || it.status != StatusTransfer.Error || it.status != StatusTransfer.Stop)) it.status = val;
        else
            //set Waiting child
            if (val == StatusTransfer.Waiting && it.status != StatusTransfer.Done) it.status = val;
        else
            if (val == StatusTransfer.Remove) it.status = val;
      }
    }
    #endregion
  }
}