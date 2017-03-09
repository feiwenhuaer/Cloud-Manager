using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using SupDataDll;
using Core.StaticClass;
using SupDataDll.Crypt;

namespace Core.Class
{
    public class LoginData
    {
        public string User;
        public string Pass;
    }

    public class Login
    {
        public void Load(string[] arg)
        {
            string user = "";
            string pass = "";
            bool autologin = false;
            if (arg.Length > 0)
            {
                user = arg[0];
                if (arg.Length > 1) { pass = arg[1]; }
                if (arg.Length > 2)
                {
                    autologin = arg[2] == "0" ? false : true;
                    AppSetting.settings.SetSettingAsString(SettingsKey.AutoLogin, arg[2] == "0" ? "0" : "1");
                }
            }
            else
            {
                LoginData data = ReadDataLogin();
                if(data != null)
                {
                    user = data.User;
                    pass = data.Pass;
                    autologin = true;
                }
            }
            AppSetting.UILogin.Load_User(user, pass, autologin);
        }

        public bool Login_EventLoginCallBack(string User, string Pass, bool AutoLogin)
        {
            if (CheckAccount(User, Pass))
            {
                if(AutoLogin)
                {
                    WriteDataLogin(new LoginData() { Pass = Pass, User = User });
                    AppSetting.settings.SetSettingAsString(SettingsKey.AutoLogin, "1");
                    AppSetting.settings.SaveSettings();
                }
                //create mainform
                if (AppSetting.UILogin.WindowState_ != SupDataDll.UiInheritance.WindowState.Minimized) AppSetting.UILogin.WindowState_ = SupDataDll.UiInheritance.WindowState.Minimized;
                if (AppSetting.UILogin.ShowInTaskbar_ == true) AppSetting.UILogin.ShowInTaskbar_ = false;
                AppSetting.Pass = Pass;
                return true;
            }
            else return false;
        }
        
        public bool CheckAccount(string User, string Pass)
        {
            if (User != AppSetting.settings.GetSettingsAsString(SettingsKey.Admin_user)) return false;
            if (Md5.CreateMD5String(Pass) != AppSetting.settings.GetSettingsAsString(SettingsKey.Admin_password)) return false;
            return true;
        }

        public LoginData ReadDataLogin()
        {
            if (ReadWriteData.Exists(ReadWriteData.File_Login))
            {
                TextReader reader = ReadWriteData.Read(ReadWriteData.File_Login);
                LoginData data = JsonConvert.DeserializeObject<LoginData>(reader.ReadToEnd());
                reader.Close();
                return data;
            }
            return null;
        }

        public bool WriteDataLogin(LoginData Data)
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Data));
            ReadWriteData.Write(ReadWriteData.File_Login, data);
            return true;
        }
    }
}
