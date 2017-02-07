using System.IO;
using Core.Cloud;
using Core.StaticClass;
using SupDataDll;
using SupDataDll.UiInheritance;
using Core.Transfer;
using Cloud;

namespace Core
{
    public static class AppSetting
    {
        //can't static class (because event)
        public static Settings settings;//setting
        public static Login login;//login
        public static GroupsTransferManager TransferManager;//upload download
        public static CloudManager ManageCloud;
        public static Languages lang;
        //instance
        public static SettingUI UISetting;
        public static UILogin UILogin;
        public static UIMain UIMain;
        public static UIUC_TLV_ud uc_lv_ud_instance;
        public static UIClosing UIclose;
        public static OauthUI UIOauth;
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
