using CloudManagerGeneralLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CloudSubClass;
using System.IO;
using System.Threading;
using Core.StaticClass;
using Newtonsoft.Json;
using TqkLibs.CloudStorage.GoogleDrive;
using TqkLibs.CloudStorage;
using TqkLibs.CloudStorage.MegaNz;

namespace Core
{
#if DEBUG
  public static class TestClass
  {
    const string email = "games_tqk@yahoo.com.vn";
    const string filepath = @"E:\Music\music\mp3tagv270setup.exe";
    const string filepath2 = @"E:\Music\music\XviD4PSP_5.10.346.0_(2015-04-07)_rc34.2.exe";
    const string email_gd = "HaDT.46C3@dhtm.edu.vn";//tqk2811@gmail.com
    public static void Test()
    {
      //testWindowCutCopy();
      //Mega();
      //FileInfo info = new FileInfo(filepath);
      //info.MoveTo(@"E:\test_folder\mp3tagv270setup.exe");
      //return;
      //Drivev3();
      //TestDrive2Copy();
      //Drive2Checkquotas();
      //Drive2Test();
      CryptTest();
    }

    #region Mega
    static void Mega()
    {
      MegaApiClient client = MegaNz.GetClient("tqk2811@yahoo.com.vn");
      IAccountInformation info = client.GetAccountInformation();
      Console.WriteLine(info.UsedQuota.ToString() + "/" + info.TotalQuota.ToString());
    }
    #endregion

    #region Disk
    static void testWindowCutCopy()
    {
      bool a = FileOperationAPIWrapper.Copy(@"D:\SoftWare\Noto-hinted.zip", @"E:\temp\Noto-hinted.zip");
      Console.WriteLine(a);
      Thread.Sleep(10000);
    }
    #endregion

    static TokenGoogleDrive Token;
    static void Drivev3()
    {
      Token = GoogleDrive.GetToken(email_gd);
      DriveAPIv3 v3 = new DriveAPIv3(Token);
      string data = v3.About_Get();
      Console.WriteLine(data);
    }

    #region Drive 2 Test
    #region Drive API
    private static void TokenRenewEvent(TokenGoogleDrive token)
    {
      Token = token;
      AppSetting.settings.ChangeToken(AppSetting.settings.GetCloud(token.Email, CloudType.GoogleDrive), JsonConvert.SerializeObject(token));
    }
    static DriveAPIv2 GetAPIv2(string email)
    {
      return GoogleDrive.GetAPIv2(email);
    }
    #endregion
    static void Drive2Test()
    {
      //TestDrive2Copy();
      //Drive2Checkquotas();
      //Drive2Test_FileList();
    }

    static void TestDrive2Copy()
    {
      //file 0Bx154iMNwuyWUUJEUTNRMnAwc0k           folder 0B2T-102UejylQmwxSnFGN3RsLWM      
      DriveAPIv2 v2 =GetAPIv2(email_gd);
      Drivev2_File f = v2.Files.Copy("0Bx154iMNwuyWUUJEUTNRMnAwc0k", "0B2T-102UejylQmwxSnFGN3RsLWM");
      return;
    }
    static void Drive2Checkquotas()
    {
      DriveAPIv2 v2 = GetAPIv2(email_gd);
      //var about = v2.About.Get();
      //string about_string = JsonConvert.SerializeObject(about, JsonSetting._settings_serialize);
      var file = v2.Files.Patch("0B2T-102UejylMXZpbUwtUU13c2c");
      string file_string = JsonConvert.SerializeObject(file, JsonSetting._settings_serialize);
      return;
    }
    static void Drive2Test_FileList()
    {
      //DriveAPIHttprequestv2 v2 = GetAPIv2(email_gd);
      //var list = v2.Files.List(GoogleDrive.en, "'0B2T-102UejylMXZpbUwtUU13c2c' in parents and trashed=false");
      var list = ReadFile<Drivev2_Files_list>("F:\\IDrive2_Files array.json");
      string list_string = JsonConvert.SerializeObject(list.items, JsonSetting._settings_serialize);      
    }
    #endregion

    static T ReadFile<T>(string path)
    {
      FileStream fo = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
      byte[] buffer = new byte[fo.Length];
      fo.Read(buffer, 0, buffer.Length);
      fo.Close();
      return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(buffer), JsonSetting._settings_deserialize);
    }

    public static void CryptTest()
    {
      var key = TqkLibs.CloudStorage.MegaNz.Crypto.GenerateRsaKey();
      Console.WriteLine();
      Console.WriteLine(key.xmlPublic + " | " + key.xmlPrivate);
      byte[] encrypt = Crypto.RSAEncrypt(Encoding.UTF8.GetBytes("tqk2811"), key.xmlPublic);
      Console.WriteLine(Encoding.UTF8.GetString(Crypto.RSADecrypt(encrypt, key.xmlPrivate)));
    }
  }
#endif
}
