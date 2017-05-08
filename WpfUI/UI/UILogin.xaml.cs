using CloudManagerGeneralLib;
using System.Windows;
using System.Windows.Input;
namespace WpfUI.UI
{
    /// <summary>
    /// Interaction logic for UILogin.xaml
    /// </summary>
    public partial class UILogin : Window, CloudManagerGeneralLib.UiInheritance.UILogin
    {
        public string user = "";
        public string pass = "";
        public bool autologin = false;
        public UILogin()
        {
            InitializeComponent();
        }

        #region interface
        public bool ShowInTaskbar_
        {
            get
            {
                return this.ShowInTaskbar;
            }

            set
            {
                this.ShowInTaskbar = value;
            }
        }

        public CloudManagerGeneralLib.UiInheritance.WindowState WindowState_
        {
            get
            {
                return (CloudManagerGeneralLib.UiInheritance.WindowState)(int)this.WindowState;
            }

            set
            {
                this.WindowState = (System.Windows.WindowState)(int)value;
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

        public void Check(string user, string pass, bool autologin)
        {
            if (!Setting_UI.reflection_eventtocore.Login(user, pass, autologin))
            {
                MessageBox.Show("Login failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (this.WindowState == System.Windows.WindowState.Minimized) this.WindowState = System.Windows.WindowState.Normal;
                if (this.ShowInTaskbar == false) this.ShowInTaskbar = true;
            }
            else this.Close();
        }
        public void LoadLanguage()
        {
            this.Title = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.Form_Text);
            LB_user.Content = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.LB_User);
            LB_pass.Content = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.LB_pass);
            BT_login.Content = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.BT_Login);
            BT_cancel.Content = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.BT_cancel);
            CB_autologin.Content = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.CB_autologin);
        }

        #region Event UI
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            TB_user.Text = this.user;
            PB_pass.Password = this.pass;
            CB_autologin.IsChecked = this.autologin;
            if (autologin == true & CB_autologin.IsChecked != null)
            {
                Check(TB_user.Text, PB_pass.Password, CB_autologin.IsChecked.Value);
            }
            LoadLanguage();
        }
        private void BT_login_Click(object sender, RoutedEventArgs e)
        {
            Check(TB_user.Text, PB_pass.Password, CB_autologin.IsChecked.Value);
        }

        private void BT_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void TB_pass_KeyDown(object sender, KeyEventArgs e)
        {
            Check(TB_user.Text, PB_pass.Password, CB_autologin.IsChecked.Value);
        }
        #endregion

    }
}
