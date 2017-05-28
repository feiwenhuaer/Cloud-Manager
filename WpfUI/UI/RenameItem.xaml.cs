using CloudManagerGeneralLib.Class;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfUI.UI
{
    /// <summary>
    /// Interaction logic for RenameItem.xaml
    /// </summary>
    public partial class RenameItem : Window
    {
        public RenameItem(ExplorerNode node)
        {
            InitializeComponent();
            TB_newname.Text = TB_oldname.Text = node.Info.Name;
            this.node = node;
        }
        ExplorerNode node;

        private void BT_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BT_change_Click(object sender, RoutedEventArgs e)
        {
            DoRename();
        }

        private void TB_newname_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && BT_change.Visibility == Visibility.Visible) DoRename();
        }

        private void TB_newname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_newname.Text == TB_oldname.Text) BT_change.Visibility = Visibility.Hidden;
            else BT_change.Visibility = Visibility.Visible;
        }
        void DoRename()
        {
            Thread thr = new Thread(Rename);
            Setting_UI.ManagerThreads.rename.Add(thr);
            Setting_UI.ManagerThreads.CleanThr();
            thr.Start();
        }

        void Rename()
        {
            if (Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.RenameItem(node,TB_newname.Text))
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(this, "Rename successful.", "Message response",MessageBoxButton.OK,MessageBoxImage.Information);
                    this.Close();
                }));

            }
            else
            {
                bool flag = false;
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBoxResult result = MessageBox.Show(this, "Rename Error\r\nPress Ok to retry.", "Message response", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    if (result == MessageBoxResult.OK) flag = true;
                    else this.Close();
                }));
                if (flag) Rename();
            }
        }

    }
}
