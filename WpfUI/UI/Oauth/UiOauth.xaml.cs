using SupDataDll.UiInheritance.Oauth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfUI.UI.Oauth
{
    /// <summary>
    /// Interaction logic for GoogleDrive.xaml
    /// </summary>
    public partial class UiOauth : Window, OauthUI
    {
        string url;
        string url_check;
        public UiOauth()
        {
            InitializeComponent();
        }

        #region interface
        public string CheckUrl
        {
            set
            {
                url_check = value;
            }
        }
        public string Url
        {
            set
            {
                url = value;
            }
        }

        public void ShowUI(object owner)
        {
            Thread thr = new Thread(ShowDiag);
            thr.Start();
        }
        #endregion

        void ShowDiag()
        {
            if(!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => this.ShowDialog()));
            }else this.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WB.Navigate(url);
        }

        private void WB_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Uri.ToString().IndexOf(url_check) == 0)
            {
                this.Close();
            }
        }
    }
}
