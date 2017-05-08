using Core.StaticClass;
using CloudManagerGeneralLib;
using System.IO;
using System.Xml;

namespace Core.Class
{
    public class Languages
    {
        XmlDocument xmlLanguages;
        string filepath;
        XmlNodeList langlist;
        public Languages(string FileName)
        {
            xmlLanguages = new XmlDocument();
            filepath = AppSetting.RootDirectory + "\\lang\\" + FileName;
            Load();
        }

        void SaveFile(bool defaulfile =false)
        {
            if (!Directory.Exists(AppSetting.RootDirectory + "\\lang")) Directory.CreateDirectory(AppSetting.RootDirectory + "\\lang");
            if (defaulfile) xmlLanguages.Save(AppSetting.RootDirectory + "\\lang\\eng.xml");
            else xmlLanguages.Save(filepath);
        }
        void Load()
        {
            xmlLanguages.LoadXml(global::Core.Properties.Resources.eng);
            if (!File.Exists(AppSetting.RootDirectory + "\\lang\\eng.xml")) SaveFile(true);
            if (File.Exists(filepath))
            {
                xmlLanguages.Load(filepath);
            }
            langlist = xmlLanguages.DocumentElement.SelectSingleNode("LANG").ChildNodes;
        }
        string Reload()
        {
            xmlLanguages = new XmlDocument();
            filepath = AppSetting.RootDirectory + "\\lang\\" + AppSetting.settings.GetSettingsAsString(SettingsKey.lang);
            Load();
            return string.Empty;
        }
        
        public string GetText(string Key)
        {
            if (Key == LanguageKey.ReloadLang.ToString()) return Reload();
            string returnValue = "Error";
            foreach (XmlNode lang in langlist)
            {
                if (lang.NodeType == XmlNodeType.Comment || lang.Attributes["NAME"].Value != Key) continue;
                returnValue = lang.Attributes["VALUE"].Value;
                break;
            }
            return returnValue;
        }
    }
}
