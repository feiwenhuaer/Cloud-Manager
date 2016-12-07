using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using Core.EncodeDecode;
using SupDataDll;
using System.Windows.Forms;

namespace Core
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
            if (StringToMD5.CreateMD5(Pass) != AppSetting.settings.GetSettingsAsString(SettingsKey.Admin_password)) return false;
            return true;
        }

        public LoginData ReadDataLogin()
        {
            FileInfo info = new FileInfo("login.dat");
            if (info.Exists)
            {
                byte[] buffer = new byte[info.Length - 1];
                byte[] crypt = new byte[1];
                FileStream fs = new FileStream("login.dat", FileMode.Open, FileAccess.Read);
                fs.Read(crypt, 0, 1);
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                buffer = AppSetting.Crypt(buffer, crypt[0]);
                MemoryStream mstream = new MemoryStream(buffer);
                StreamReader sreader = new StreamReader(mstream);
                LoginData data = JsonConvert.DeserializeObject<LoginData>(sreader.ReadToEnd());
                mstream.Close();
                sreader.Close();
                return data;
            }
            return null;
        }

        public bool WriteDataLogin(LoginData Data)
        {
            FileInfo info = new FileInfo("login.dat");
            if (info.Exists)
            {
                info.Delete();
            }

            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Data));
            FileStream fs = new FileStream("login.dat", FileMode.CreateNew, FileAccess.Write);
            Random rd = new Random();
            int val = rd.Next(1, 2 ^ 8 - 1);
            data = AppSetting.Crypt(data, val);
            fs.WriteByte((byte)val);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
            return true;
        }
    }
}
