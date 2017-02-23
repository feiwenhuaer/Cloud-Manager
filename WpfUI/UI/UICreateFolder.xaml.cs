using SupDataDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    /// Interaction logic for UICreateFolder.xaml
    /// </summary>
    public partial class UICreateFolder : Window
    {
        ExplorerNode parent;
        public UICreateFolder(ExplorerNode parent)
        {
            InitializeComponent();
            BT_create.Visibility = Visibility.Hidden;
            this.parent = parent;
        }

        private void BT_create_Click(object sender, RoutedEventArgs e)
        {
            DoCreateFolder();
        }
        private void BT_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex rg = new Regex("^[^\\/\\\\:<>\\|\"*?]+$");// ^[^\\/\\\\:<>\\|\"*?]+$
            Match match = rg.Match(textBox.Text);
            if (match.Success) BT_create.Visibility = Visibility.Visible;
            else BT_create.Visibility = Visibility.Hidden;
        }

        void DoCreateFolder()
        {
            Thread thr = new Thread(CreateFolder);
            Setting_UI.ManagerThreads.CleanThr();
            Setting_UI.ManagerThreads.createfolder.Add(thr);
            thr.Start();
        }
        void CreateFolder()
        {
            ExplorerNode n = new ExplorerNode();
            n.Info.Name = textBox.Text;
            parent.AddChild(n);
            Setting_UI.reflection_eventtocore._CreateFolder(n);
            Dispatcher.Invoke(new Action(() => this.Close()));
        }
    }
}
