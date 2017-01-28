using Aga.Controls.Tree;
using SupDataDll;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WpfUI.Class;

namespace WpfUI.UI.Main.Lv_ud
{
    /// <summary>
    /// Interaction logic for UC_TLV.xaml
    /// </summary>
    public partial class UC_TLV :UserControl
    {
        public TransferDataTLVWPF data;

        public UC_TLV()
        {
            InitializeComponent();
            this.DataContext = this;
            data = new TransferDataTLVWPF();
            treeList.Model = data;
            Load_TLVmenu();
        }

        private void treeList_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            treeList.SelectedItems.Clear();
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
            treeList.ContextMenu.ItemsSource = menuitems_source;
        }
        
        List<TransferGroup> groups;
        List<TransferItem> items;
        private void treeList_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            start.IsEnabled = false;
            waiting.IsEnabled = false;
            stop.IsEnabled = false;
            error.IsEnabled = false;
            remove.IsEnabled = false;
            numberOfParallelDownloads.IsEnabled = false;

            var seleecteditems = treeList.SelectedItems;
            if(seleecteditems!= null)
            {
                if (seleecteditems.Count > 0) remove.IsEnabled = true;
                groups = new List<TransferGroup>();
                items = new List<TransferItem>();
                foreach (var item in seleecteditems)
                {
                    TreeNode node = item as TreeNode;
                    TransferGroup tg = node.Tag as TransferGroup;
                    TransferItem ti = node.Tag as TransferItem;
                    if (tg != null)//group
                    {
                        groups.Add(tg);
                        SetIsEnableMenuTLV(tg as Transfer);
                        foreach (TransferItem child in tg.items) SetIsEnableMenuTLV(child as Transfer);
                    }//item
                    else
                    {
                        items.Add(ti);
                        SetIsEnableMenuTLV(ti as Transfer);
                    }
                }
                if(groups.Count == 1 & items.Count == 0) numberOfParallelDownloads.IsEnabled = true;
            }
        }
        void SetIsEnableMenuTLV(Transfer t)
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
            if (model != null && model.Key == LanguageKey.TSMI_numberOfParallelDownloads) ChangeNumberOfParallelDownloads();
            else
            {
                switch(model.Key)
                {
                    case LanguageKey.TSMI_start: ChangeStatus(StatusTransfer.Started); break;
                    case LanguageKey.TSMI_waiting: ChangeStatus(StatusTransfer.Waiting); break;
                    case LanguageKey.TSMI_stop: ChangeStatus(StatusTransfer.Stop); break;
                    case LanguageKey.TSMI_remove:
                        MessageBoxResult result = MessageBox.Show("Are you sure to remove?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if(result == MessageBoxResult.Yes) ChangeStatus(StatusTransfer.Remove);
                        break;
                    case LanguageKey.TSMI_forcestart: ErrorSetForce(StatusTransfer.Started); break;
                    case LanguageKey.TSMI_forcewaiting: ErrorSetForce(StatusTransfer.Waiting); break;
                }
            }
            groups.Clear();
            items.Clear();
        }
               
        void ChangeNumberOfParallelDownloads()
        {
            ChangeNumberItemsTransfer u = new ChangeNumberItemsTransfer(groups[0].MaxItemsDownload);
            u.ShowDialog();
            if(u.Flags)
            {
                groups[0].MaxItemsDownload = u.Number;
            }
        }

        void ChangeStatus(StatusTransfer val)
        {
            foreach (TransferItem it in items)
            {
                if (val == StatusTransfer.Started && (it.status == StatusTransfer.Stop | it.status == StatusTransfer.Waiting | it.status == StatusTransfer.Error))
                {
                    it.status = val;
                    TransferGroup pr = GetParentItem(it);
                    if (pr != null && (pr.status != StatusTransfer.Running | pr.status != StatusTransfer.Loading | pr.status != StatusTransfer.Remove)) pr.status = val;
                }
                else
                    //set Stop child
                    if (val == StatusTransfer.Stop && (it.status != StatusTransfer.Done | it.status != StatusTransfer.Error | it.status != StatusTransfer.Stop)) it.status = val;
                else
                    //set Waiting child
                    if (val == StatusTransfer.Waiting && it.status != StatusTransfer.Done) it.status = val;
                else
                    if (val == StatusTransfer.Remove) it.status = val;
            }

            foreach(TransferGroup gr in groups)
            {
                if (val == StatusTransfer.Started && (gr.status != StatusTransfer.Running)) gr.status = val;
                else
                   if (val == StatusTransfer.Stop && (gr.status != StatusTransfer.Done | gr.status != StatusTransfer.Loading | gr.status != StatusTransfer.Stop)) gr.status = val;
                else
                   if (val == StatusTransfer.Waiting && (gr.status != StatusTransfer.Done | gr.status != StatusTransfer.Remove)) gr.status = val;
                else
                   if (val == StatusTransfer.Remove) gr.status = val;
            }
        }

        void ErrorSetForce(StatusTransfer val)
        {
            foreach (var gr in groups)
            {
                foreach (var it in gr.items)
                {
                    if (it.status == StatusTransfer.Error)
                    {
                        it.status = val;
                    }
                }
            }
        }

        TransferGroup GetParentItem(TransferItem item)
        {
            foreach (TreeNode node in treeList.Nodes)
            {
                foreach (TransferItem it in (node.Tag as TransferGroup).items) if (it == item) return node.Tag as TransferGroup;
            }
            return null;
        }
        #endregion
    }
}
