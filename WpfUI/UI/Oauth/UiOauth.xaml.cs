using Cloud;
using System;
using System.Threading;
using System.Windows;

namespace WpfUI.UI.Oauth
{
    /// <summary>
    /// Interaction logic for GoogleDrive.xaml
    /// </summary>
    public partial class UiOauth : Window, OauthUI
    {
        bool isclosed = false;
        string url;
        string url_check;
        public UiOauth()
        {
            InitializeComponent();
            this.Closed += UiOauth_Closed;
        }

        private void UiOauth_Closed(object sender, EventArgs e)
        {
            isclosed = true;
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

        public void CloseUI()
        {
            if(!isclosed)
            {
                this.Close();
                isclosed = true;
            }
        }
    }
}
