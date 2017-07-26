using System.IO;
using Core.CloudSubClass;
using CloudManagerGeneralLib.UiInheritance;
using Core.Transfer;
using Cloud;
using System.Threading;
using Core.Class;

namespace Core.StaticClass
{
  public static class AppSetting
  {
    public static Thread MainThread;
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
    public static UIClosing UIclose;
    public static IOauth UIOauth;
    //pass
    public static string Pass;

    //other
    public static void LoadAPIKey()
    {
      DropboxAppKey.ApiKey = Properties.Resources.DropboxApiKey;
      DropboxAppKey.ApiSecret = Properties.Resources.DropboxApiSecret;
      GoogleDriveAppKey.ApiKey = Properties.Resources.GoogleDriveApiKey;
      GoogleDriveAppKey.Clientsecret = Properties.Resources.GoogleDriveClientsecret;
      GoogleDriveAppKey.ClientID = Properties.Resources.GoogleDriveClientID;
      MegaNzAppKey.ApiKey = Properties.Resources.MegaNzApiKey;
    }
    public static void CloseOauthUI()
    {
      if (UIOauth != null) UIOauth.CloseUI();
    }
    public static byte[] Crypt(byte[] Buffer, int val)
    {
      for (int i = 0; i < Buffer.Length; i++)
      {
        Buffer[i] ^= (byte)val;
      }
      return Buffer;
    }
    public static string RootDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
  }
}