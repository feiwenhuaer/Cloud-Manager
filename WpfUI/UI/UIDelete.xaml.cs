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
using SupDataDll.UiInheritance;

namespace WpfUI.UI
{
    /// <summary>
    /// Interaction logic for Delete.xaml
    /// </summary>
    public partial class UIDelete : Window, SupDataDll.UiInheritance.UIDelete
    {
        public UIDelete()
        {
            InitializeComponent();
            checkBox.Checked += CheckBox_Checked;
            checkBox.Unchecked += CheckBox_Unchecked;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            autoclose = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            autoclose = true;
        }

        #region interface
        public event CancelDelegate EventCancel;
        public event ClosingDelegate EventClosing;
        bool autoclose = true;
        public bool AutoClose
        {
            get
            {
                return autoclose;
            }
        }
        public void Close_()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => this.Close()));
            }
            else this.Close();
        }

        public void SetAutoClose(bool c)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    checkBox.IsChecked = c;
                }));
            }
            else checkBox.IsChecked = c;
        }

        public void SetTextButtonCancel(string text)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => button.Content = text));
            }
            else button.Content = text;
        }

        public void Show_(object owner = null)
        {
            if (owner != null) this.Owner = (Window)owner;
            this.Show();
        }

        public void UpdateText(string text)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => textBox.Text += text));
            }
            else textBox.Text += text;
        }
        #endregion

        #region event

        private void button_Click(object sender, RoutedEventArgs e)
        {
            EventCancel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            checkBox.IsChecked = true;
            EventClosing();
        }
        #endregion
    }
}

