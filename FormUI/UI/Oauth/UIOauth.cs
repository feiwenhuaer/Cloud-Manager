using Cloud;
using System;
using System.Threading;
using System.Windows.Forms;

namespace FormUI.UI.Oauth
{
    public partial class UIOauth : Form, OauthUI
    {
        bool isclosed = false;
        string url;
        string url_check;
        public UIOauth()
        {
            InitializeComponent();
            this.FormClosed += UIOauth_FormClosed;
        }

        private void UIOauth_FormClosed(object sender, FormClosedEventArgs e)
        {
            isclosed = true;
        }

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

        public void CloseUI()
        {
            if(!isclosed)
            {
                this.Close();
                isclosed = true;
            }
        }

        public void ShowUI(object owner)
        {
            if (owner != null) this.Show((Form)owner);
            else this.Show();
        }

        private void UIOauth_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(url);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if(e.Url.ToString().IndexOf(url_check) == 0)
            {
                this.Close();
            }
        }
    }
}
