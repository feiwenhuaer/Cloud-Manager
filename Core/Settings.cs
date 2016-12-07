using Core.EncodeDecode;
using Core.StaticClass;
using SupDataDll;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Core
{
    public class Settings
    {
        private XmlDocument xmlSettings;
        private object SyncSetting = new object();
        public object SyncPass = new object();
        public void ReadSettings()
        {
            xmlSettings = new XmlDocument();
            try
            {
                if (File.Exists(AppSetting.RootDirectory + "\\" + "Settings.dat"))
                {
                    lock (SyncSetting)
                    {
                        xmlSettings.Load(ReadWriteData.Read("Settings.dat"));
                    }
                }
                else
                {
                    xmlSettings.LoadXml(global::Core.Properties.Resources.SettingDefault);
                    SaveSettings();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("read setting error");
            }
        }

        public void SaveSettings()
        {
            lock (SyncSetting)
            {
                try
                {

                    MemoryStream Stream = new MemoryStream();
                    TextWriter TxtWriter = new StreamWriter(Stream, Encoding.UTF8);
                    xmlSettings.Save(TxtWriter);
                    ReadWriteData.Write("Settings.dat", Stream.GetBuffer());
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Can't write setting\r\n" + Ex.Message);
                }
            }
        }

        public string GetSettingsAsString(SettingsKey Key)
        {
            return GetSettingsAsString_FromString(Key.ToString());
        }

        public string GetSettingsAsString_FromString(string Key)
        {
            XmlNodeList SettingsList = xmlSettings.DocumentElement.SelectSingleNode("SETTINGS").ChildNodes;
            string returnValue = string.Empty;
            foreach (XmlNode Setting in SettingsList)
            {
                if (Setting.Attributes["NAME"].Value != Key) continue;
                returnValue = Setting.Attributes["VALUE"].Value;
                break;
            }
            return returnValue;
        }

        public void SetSettingAsString(SettingsKey Key, string Data)
        {
            SetSettingAsString_FromString(Key.ToString(), Data);
        }

        public void SetSettingAsString_FromString(string Key, string Data)
        {
            XmlNode SettingsNode = xmlSettings.DocumentElement.SelectSingleNode("SETTINGS");
            foreach (XmlNode Setting in SettingsNode.ChildNodes)
            {
                if (Setting.Attributes["NAME"].Value != Key) continue;
                Setting.Attributes["VALUE"].Value = Data;
                return;
            }
            XmlNode NewSetting = xmlSettings.CreateElement("KEY");
            XmlAttribute Attrib = xmlSettings.CreateAttribute("NAME");
            Attrib.Value = Key;
            NewSetting.Attributes.Append(Attrib);
            Attrib = xmlSettings.CreateAttribute("VALUE");
            Attrib.Value = Data;
            NewSetting.Attributes.Append(Attrib);
            SettingsNode.AppendChild(NewSetting);
        }

        public ListAccountCloud GetListAccountCloud()
        {
            ListAccountCloud list = new ListAccountCloud();
            foreach (XmlNode node in GetCloudDataList())
            {
                list.account.Add(new CloudEmail_Type()
                {
                    Email = node.Attributes["Email"].Value,
                    Type = (CloudName)Enum.Parse(typeof(CloudName), node.Attributes["CloudName"].Value)
                });
            }
            return list;
        }

        public XmlNodeList GetCloudDataList()
        {
            return xmlSettings.DocumentElement.SelectSingleNode("UserAccount").ChildNodes;
        }

        internal XmlNode GetCloud(string Email, CloudName cloudname)
        {
            foreach (XmlNode node in GetCloudDataList())
            {
                if (node.Attributes["Email"].Value == Email & node.Attributes["CloudName"].Value == cloudname.ToString())
                    return node;
            }
            return null;
        }

        internal string GetToken(string Email, CloudName cloudname)
        {
            XmlNode node = GetCloud(Email, cloudname);
            if (node == null) return null;
            if (node.Attributes["EncryptToken"].Value == "0") return node.Attributes["Token"].Value;
            else
            {
                string token;
                try
                {
                    Monitor.Enter(SyncPass);
                    token = Crypto.DecryptStringAES(node.Attributes["Token"].Value, AppSetting.Pass);
                }
                finally { Monitor.Exit(SyncPass); }
                return token;
            }
        }

        public bool AddCloud(string Email, CloudName cloudname, string token, bool AreEncrypt, bool IsDefault = false)
        {
            if (GetToken(Email, cloudname) == null)
            {
                XmlNode NewSetting = xmlSettings.CreateElement("Cloud");
                XmlAttribute Attrib = xmlSettings.CreateAttribute("Email");
                Attrib.Value = Email;
                NewSetting.Attributes.Append(Attrib);

                Attrib = xmlSettings.CreateAttribute("CloudName");
                Attrib.Value = cloudname.ToString();
                NewSetting.Attributes.Append(Attrib);

                Attrib = xmlSettings.CreateAttribute("Token");
                try
                {
                    Monitor.Enter(SyncPass);
                    if (!AreEncrypt & !string.IsNullOrEmpty(AppSetting.Pass)) Attrib.Value = Crypto.EncryptStringAES(token, AppSetting.Pass);
                    else Attrib.Value = token;
                }
                finally {Monitor.Exit(SyncPass); }
                NewSetting.Attributes.Append(Attrib);

                Attrib = xmlSettings.CreateAttribute("EncryptToken");
                Attrib.Value = string.IsNullOrEmpty(AppSetting.Pass) ? "0" : "1";
                NewSetting.Attributes.Append(Attrib);

                Attrib = xmlSettings.CreateAttribute("Default");
                Attrib.Value = IsDefault ? "1" : "0";
                NewSetting.Attributes.Append(Attrib);

                xmlSettings.DocumentElement.SelectSingleNode("UserAccount").AppendChild(NewSetting);
                SaveSettings();
                return true;
            }
            return false;
        }

        public string GetDefaultCloud(CloudName cloudname)
        {
            foreach (XmlNode node in GetCloudDataList())
            {
                if (node.Attributes["CloudName"].Value == cloudname.ToString() && node.Attributes["Default"].Value == "1") return node.Attributes["Email"].Value;
            }
            foreach (XmlNode node in GetCloudDataList())
            {
                if (node.Attributes["CloudName"].Value == cloudname.ToString() && node.Attributes["Default"].Value == "0")
                {
                    node.Attributes["Default"].Value = "1";
                    SaveSettings();
                    return node.Attributes["Email"].Value;
                }
            }
            return null;
        }

        public bool SetDefaultCloud(string Email, CloudName cloudname)
        {
            XmlNode n = GetCloud(Email, cloudname);
            if (n != null)
            {
                foreach (XmlNode node in GetCloudDataList())
                {
                    if (node.Attributes["CloudName"].Value == cloudname.ToString() && node.Attributes["Default"].Value == "1") node.Attributes["Default"].Value = "0";
                }
                n.Attributes["Default"].Value = "1";
                return true;
            }
            return false;
        }

        public bool RemoveCloud(string Email, CloudName cloudname)
        {
            XmlNode cloud = GetCloud(Email, cloudname);
            if (cloud != null)
            {
                xmlSettings.DocumentElement.SelectSingleNode("UserAccount").RemoveChild(cloud);
                SaveSettings();
                return true;
            }
            return false;
        }

        internal void ChangeToken(XmlNode cloud, string newtoken,string newpass = null)
        {
            if (cloud != null)
            {
                try
                {
                    Monitor.Enter(AppSetting.settings.SyncPass);
                    cloud.Attributes["Token"].Value = Crypto.EncryptStringAES(newtoken, newpass == null ? AppSetting.Pass : newpass);
                    cloud.Attributes["EncryptToken"].Value = "1";
                    AppSetting.settings.SaveSettings();
                    return;
                }
                finally { Monitor.Exit(AppSetting.settings.SyncPass); }
            }
            throw new Exception("Can't save token");
        }

        public bool ChangeUserPass(string user, string pass, string newpass)
        {
            bool flag = false;
            if (StringToMD5.CreateMD5(pass) != GetSettingsAsString(SettingsKey.Admin_password)) return flag;
            else
            {
                if (!string.IsNullOrEmpty(user))
                {
                    SetSettingAsString(SettingsKey.Admin_user, user);
                    flag = true;
                }

                if (!string.IsNullOrEmpty(newpass))
                {
                    foreach (XmlNode node in GetCloudDataList())
                    {
                        string token = GetToken(node.Attributes["Email"].Value, (CloudName)Enum.Parse(typeof(CloudName), node.Attributes["CloudName"].Value));
                        if (!string.IsNullOrEmpty(token)) ChangeToken(node, token, newpass);
                    }
                    SetSettingAsString(SettingsKey.Admin_password, StringToMD5.CreateMD5(newpass));
                    SaveSettings();
                    AppSetting.Pass = newpass;
                    flag = true;
                }
                return flag;
            }

        }
    }
}
