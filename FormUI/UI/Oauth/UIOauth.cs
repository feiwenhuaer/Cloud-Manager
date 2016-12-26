using SupDataDll.UiInheritance.Oauth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FormUI.UI.Oauth
{
    public partial class UIOauth : Form, OauthUI
    {
        string url;
        string url_check;
        public UIOauth()
        {
            InitializeComponent();
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

        public void ShowUI(object owner)
        {
            Thread thr = new Thread(ShowDiag);
            thr.Start();
        }

        private void ShowDiag()
        {
            if (InvokeRequired) this.Invoke(new Action(() => this.ShowDialog()));
            else this.ShowDialog();
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
