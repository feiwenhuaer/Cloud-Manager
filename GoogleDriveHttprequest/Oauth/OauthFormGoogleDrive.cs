using System;
using System.Threading;
using System.Windows.Forms;

namespace GoogleDriveHttprequest.Oauth
{
    internal partial class OauthFormGoogleDrive : Form
    {
        public OauthFormGoogleDrive(string url, string RedirectUrl)
        {
            InitializeComponent();
            this.url = url;
            this.RedirectUrl = RedirectUrl;
        }
        string url;
        string RedirectUrl;   
        private void OauthFormGoogleDrive_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(url);
            this.Activate();
        }

        bool flag = false;

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.ToString().IndexOf(RedirectUrl) == 0)
            {
                flag = true;
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (flag) this.Close();
        }
    }
}
