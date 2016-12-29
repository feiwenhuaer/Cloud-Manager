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
        List<TransferGroup> groups = new List<TransferGroup>();
        public int AddNewGroup(TransferGroup Group)
        {
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
            if (!Dispatcher.CheckAccess()) Dispatcher.Invoke(new Action(() => refresh()));
            else refresh();
        }

        void refresh()
        {
            TLV_done.treeList.Model = TLV_done.data;
            TLV_process.treeList.Model = TLV_process.data;
        }

        public void RemoveGroup(TransferGroup Group)
        {
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
