using System;
using System.Windows.Forms;
using SupDataDll.UiInheritance;
using SupDataDll;

namespace FormUI.UI
{
    public partial class LoginForm : System.Windows.Forms.Form, SupDataDll.UiInheritance.UILogin
    {
        public string user = "";
        public string pass = "";
        public bool autologin = false;

        #region interface

        public WindowState WindowState_
        {
            get
            {
                return (WindowState)(int)this.WindowState;
            }

            set
            {
                WindowState = (System.Windows.Forms.FormWindowState)(int)value;
            }
        }

        public bool ShowInTaskbar_
        {
            get
            {
                return ShowInTaskbar;
            }

            set
            {
                ShowInTaskbar = value;
            }
        }

        WindowState UILogin.WindowState_
        {
            get
            {
                return (WindowState)(int)this.WindowState;
            }

            set
            {
                this.WindowState = (FormWindowState)(int)value;
            }
        }

        public void Load_User(string User, string Pass, bool AutoLogin)
        {
            this.user = User;
            this.pass = Pass;
            this.autologin = AutoLogin;
        }

        public void ShowDialog_()
        {
            this.ShowDialog();
        }
        #endregion

        public LoginForm()
        {
            InitializeComponent();
        }

        #region Event Form
        private void Login_Load(object sender, EventArgs e)
        {
            TB_User.Text = this.user;
            TB_pass.Text = this.pass;
            CB_autologin.Checked = this.autologin;
            if (autologin == true)
            {
                Check(TB_User.Text, TB_pass.Text, CB_autologin.Checked);
            }
            LoadLanguage();
        }

        private void BT_cancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BT_Login_Click(object sender, EventArgs e)
        {
            Check(TB_User.Text, TB_pass.Text, CB_autologin.Checked);
        }

        private void TB_pass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                Check(TB_User.Text, TB_pass.Text, CB_autologin.Checked);
            }
        }
        #endregion

        public void Check(string user, string pass, bool autologin)
        {
            if (!Setting_UI.reflection_eventtocore._Login(user, pass, autologin))
            {
                MessageBox.Show("Login failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
                if (this.ShowInTaskbar == false) this.ShowInTaskbar = true;
            }
            else this.Close();
        }
        public void LoadLanguage()
        {
            this.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.Form_Text);
            LB_User.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.LB_User);
            LB_Pass.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.LB_pass);
            BT_Login.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.BT_Login);
            BT_cancel.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.BT_cancel);
            CB_autologin.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.CB_autologin);
        }
    }
}

