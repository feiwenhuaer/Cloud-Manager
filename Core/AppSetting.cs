using System.IO;
using Core.cloud;
using Core.StaticClass;
using SupDataDll;
using SupDataDll.UiInheritance;

namespace Core
{
    public static class AppSetting
    {
        public static void Load()
        {
            //load core
            settings = new Settings();//load settings
            settings.ReadSettings();
            login = new Login();//login
            ManageCloud = new CloudManager();//explorer
            Get_mimeType.Load();//mimeType (google drive upload)
            ud_items = new UploadDownloadItems();//transfer file
            lang = new Languages(settings.GetSettingsAsString(SettingsKey.lang));//load language
            LoadDllUI.Load();//load dll UI
            Reflection_UI.Load_Setting_UI();//Add handle to event
        }
        //can't static class (because event)
        public static Settings settings;//setting
        public static Login login;//login
        public static UploadDownloadItems ud_items;//upload download
        public static CloudManager ManageCloud;
        public static Languages lang;
        //instance
        public static UILogin UILogin;
        public static UIMain UIMain;
        public static UIUC_TLV_ud uc_lv_ud_instance;
        public static UIClosing UIclose;

        //pass
        public static string Pass;

        //other
        public static byte[] Crypt(byte[] Buffer,int val)
        {
            for (int i = 0; i < Buffer.Length; i++)
            {
                Buffer[i] ^= (byte)val;
            }
            return Buffer;
        }
        public static string RootDirectory = Directory.GetCurrentDirectory();
    }
}
