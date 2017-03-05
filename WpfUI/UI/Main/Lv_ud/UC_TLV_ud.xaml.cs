using SupDataDll;
using SupDataDll.Class;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace WpfUI.UI.Main.Lv_ud
{
    /// <summary>
    /// Interaction logic for UCLV_ud.xaml
    /// </summary>
    public partial class UC_TLV_ud : UserControl, SupDataDll.UiInheritance.UIUC_TLV_ud
    {
        object uimain;
        #region interface
        public object UIMain
        {
            set
            {
                uimain = value;
            }
        }
        List<TransferGroup> groups = new List<TransferGroup>();
        public int AddNewGroup(TransferGroup Group)
        {
            if (Setting_UI.ExitAPP_Flag) return -1;
            Group.col[2] = Group.status.ToString();
            if (groups.IndexOf(Group) >= 0) refresh();
            else
            {
                groups.Add(Group);
                if (Group.change == ChangeTLV.Processing)
                {
                    TLV_process.data.Add(Group);
                }
                else
                {
                    TLV_done.data.Add(Group);
                }
            }
            return 0;
        }

        public void LoadLanguage()
        {
            TI_process.Header = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TP_processing);
            TI_complete.Header = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TP_done);
        }

        public void RefreshAll()
        {
            if (Setting_UI.ExitAPP_Flag) return;
            if (!Dispatcher.CheckAccess()) Dispatcher.Invoke(new Action(() => refresh()));
            else refresh();
        }

        void refresh()
        {
            foreach(TransferGroup item in groups)
            {
                if (item.change == ChangeTLV.DoneToProcessing)
                {
                    TLV_done.data.Remove(item);
                    TLV_process.data.Add(item);
                    item.change = ChangeTLV.Processing;
                }

                if (item.change == ChangeTLV.ProcessingToDone)
                {
                    TLV_process.data.Remove(item);
                    TLV_done.data.Add(item);
                    item.change = ChangeTLV.Done;
                }
            }
            TLV_done.treeList.Model = TLV_done.data;
            TLV_process.treeList.Model = TLV_process.data;
        }

        public void RemoveGroup(TransferGroup Group)
        {
            if (Setting_UI.ExitAPP_Flag) return;
            groups.Remove(Group);
            if (Group.change == ChangeTLV.Processing) TLV_process.data.Remove(Group);
            else TLV_done.data.Remove(Group);
        }

        #endregion

        public UC_TLV_ud()
        {
            InitializeComponent();
        }
    }
}
