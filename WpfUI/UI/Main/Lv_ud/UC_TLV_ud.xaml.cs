using SupDataDll;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public int AddNewGroup(UD_group_work Group)
        {
            Group.col[2] = Group.status.ToString();
            if (Group.change == ChangeTLV.Processing)
            {
                if (TLV_process.data == null) TLV_process.data = new UD_data_WPF(Group);
                else TLV_process.data.Add(Group);
            }
            else
            {
                if (TLV_done.data == null) TLV_done.data = new UD_data_WPF(Group);
                else TLV_done.data.Add(Group);
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
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    TLV_process.treeList.Model = TLV_process.data;
                    TLV_done.treeList.Model = TLV_done.data;
                }));
            }
            else
            {
                TLV_process.treeList.Model = TLV_process.data;
                TLV_done.treeList.Model = TLV_done.data;
            }
        }

        public void RemoveGroup(UD_group_work Group)
        {
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
