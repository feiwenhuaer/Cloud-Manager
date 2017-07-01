using Cloud;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormUI.UI.Oauth
{
    public partial class UIOauth : Form, IOauth
    {
        bool isclosed = false;
        string url;
        string url_check;

        public event UriResponse EventUriResponse;

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
            if (!isclosed)
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
#if DEBUG
            Console.WriteLine("webBrowser1 Navigate url:" + url);
#endif
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString().IndexOf(url_check) == 0)
            {
                this.Close();
                EventUriResponse(e.Url);
            }
        }
    }
}
