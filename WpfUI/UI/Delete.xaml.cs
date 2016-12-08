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
    public partial class Delete : Window, UIDelete
    {
        public Delete()
        {
            InitializeComponent();
            this.Closing += Delete_Closing;
        }

        private void Delete_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EventClosing();
        }

        bool autoclose = false;

        #region interface
        public event CancelDelegate EventCancel;
        public event ClosingDelegate EventClosing;

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
                    autoclose = c;
                    checkBox.IsChecked = c;
                }));
            }
            else
            {
                autoclose = c;
                checkBox.IsChecked = c;
            }
        }

        public void SetTextButtonCancel(string text)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => button.Content = text));
            }
            else button.Content = text;
        }

        public void ShowDialog_()
        {
            this.ShowDialog();
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
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            autoclose = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            autoclose = false;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            EventCancel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            autoclose = true;
        }
        #endregion
    }
}

