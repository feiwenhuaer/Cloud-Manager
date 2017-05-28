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
using System.Windows.Shapes;

namespace WpfUI.UI
{
    /// <summary>
    /// Interaction logic for ChangeNumberItemsTransfer.xaml
    /// </summary>
    public partial class ChangeNumberItemsTransfer : Window
    {
        public ChangeNumberItemsTransfer(int num)
        {
            InitializeComponent();
            int max = 32;
            int.TryParse(Setting_UI.reflection_eventtocore.SettingAndLanguage.GetSetting(CloudManagerGeneralLib.SettingsKey.MaxItemsInGroupDownload),out max);
            n_ud.MaxValue = max;
            n_ud.MinValue = 1;
            Flags = false;
            n_ud.Number = num;
        }
        public bool Flags { get; private set; }
        public int Number { get { return n_ud.Number; } }

        private void BT_Save_Click(object sender, RoutedEventArgs e)
        {
            Flags = true;
            this.Close();
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
