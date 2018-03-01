using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TqkLibs.CloudStorage.MegaNz;

namespace WpfUI.UI.Oauth
{
    /// <summary>
    /// Interaction logic for OauthMega.xaml
    /// </summary>
    public partial class OauthMega : Window, UIinterfaceMegaNz
    {
        public OauthMega()
        {
            InitializeComponent();
        }

        public string Email
        {
            get
            {
                return this.TB_UserName.Text;
            }
        }

        public string Pass
        {
            get
            {
                return this.PB_.Password;
            }
        }
        bool success = false;
        public bool Success
        {
            get
            {
                return success;
            }
        }

        public void ShowDialog_()
        {
            if (Dispatcher.CheckAccess())
            {
                this.ShowDialog();
            }
            else Dispatcher.Invoke(new Action(() => { this.ShowDialog(); }));
        }

        public void ShowError(string message)
        {
            if (Dispatcher.CheckAccess())
            {
                ShowError_(message);
            }
            else Dispatcher.Invoke(new Action(() => { ShowError_(message); }));
        }

        void ShowError_(string message)
        {
            System.Windows.MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BT_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BT_Auth_Click(object sender, RoutedEventArgs e)
        {
            success = true;
            this.Close();
        }

        private void TB_UserName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (BT_Auth.IsEnabled)
                {
                    success = true;
                    this.Close();
                }
            }
        }

        private void TB_UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckTextInput();
        }

        private void PB__PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckTextInput();
        }

        void CheckTextInput()
        {
            if (string.IsNullOrEmpty(TB_UserName.Text) || string.IsNullOrEmpty(PB_.Password)) BT_Auth.IsEnabled = false;
            else BT_Auth.IsEnabled = true;
        }
    }
}
