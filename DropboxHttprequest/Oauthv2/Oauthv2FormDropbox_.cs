using System;
using System.Threading;
using System.Windows.Forms;

namespace DropboxHttpRequest
{
    internal partial class Oauthv2FormDropbox : Form
    {
        private string url;
        private string RedirectUrl;
        bool flag = false;
        public Oauthv2FormDropbox(string url,string RedirectUrl)
        {
            InitializeComponent();
            this.url = url;
            this.RedirectUrl = RedirectUrl;
        }

        private void Oauthv2FormDropbox_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(url);
        }

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
