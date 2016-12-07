using Aga.Controls.Tree;
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
using System.Collections;

namespace WPF.UI
{
    /// <summary>
    /// Interaction logic for LV_ud.xaml
    /// </summary>
    public partial class UCLV_ud : UserControl
    {
        public UD_data_WPF data;

        public UCLV_ud()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            treeList.UpdateLayout();
        }
    }
}

