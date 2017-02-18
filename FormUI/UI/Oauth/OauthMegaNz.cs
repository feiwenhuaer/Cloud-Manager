using Cloud.MegaNz.Oauth;
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
    public partial class OauthMegaNz : Form, UIinterfaceMegaNz
    {
        public string Email
        {
            get
            {
                return this.TB_email.Text;
            }
        }

        public string Pass
        {
            get
            {
                return this.TB_pass.Text;
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

        public OauthMegaNz()
        {
            InitializeComponent();
        }

        private void BT_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TB_pass_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TB_email.Text) || string.IsNullOrEmpty(TB_pass.Text)) BT_Authencation.Enabled = false;
            else BT_Authencation.Enabled = true;
        }

        private void BT_Authencation_Click(object sender, EventArgs e)
        {
            success = true;
            this.Close();
        }

        public void ShowDialog_()
        {
            if (InvokeRequired) Invoke(new Action(() => this.ShowDialog()));
            else this.ShowDialog();
        }

        public void ShowError(string message)
        {
            if (InvokeRequired) Invoke(new Action(() => showerror(message)));
            else showerror(message);
        }

        void showerror(string message)
        {
            MessageBox.Show(message, "Authencation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.ShowDialog();
        }

        private void TB_pass_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (BT_Authencation.Enabled)
                {
                    success = true;
                    this.Close();
                }
            }
        }
    }
}
