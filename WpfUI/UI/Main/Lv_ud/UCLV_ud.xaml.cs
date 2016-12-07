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
    public partial class UCLV_ud : UserControl, SupDataDll.UiInheritance.UIUC_Lv_ud
    {
        List<UD_group_work> ud = new List<UD_group_work>();
        #region interface
        public object UIMain
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public int AddNewGroup(UD_group_work Group)
        {
            Group.col[2] = Group.status.ToString();
            if (Group.change == ChangeTLV.Processing)
            {
                if (LV_process.data == null) LV_process.data = new UD_data_WPF(Group);
                else LV_process.data.Add(Group);
            }
            else
            {
                if (LV_done.data == null) LV_done.data = new UD_data_WPF(Group);
                else LV_done.data.Add(Group);
            }
            return 0;
        }

        public void LoadLanguage()
        {
            throw new NotImplementedException();
        }

        public void RefreshAll()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    LV_process.treeList.Model = LV_process.data;
                    LV_done.treeList.Model = LV_done.data;
                    //LV_process.Refresh();
                    //LV_done.Refresh();
                }));
            }
            else
            {
                LV_process.treeList.Model = LV_process.data;
                LV_done.treeList.Model = LV_done.data;
                //LV_process.Refresh();
                //LV_done.Refresh();
            }
        }

        public void RemoveGroup(UD_group_work Group)
        {
            if (Group.change == ChangeTLV.Processing) LV_process.data.Remove(Group);
            else LV_done.data.Remove(Group);
        }

        #endregion

        public UIUC_Lv_ud()
        {
            InitializeComponent();
        }
    }
}
