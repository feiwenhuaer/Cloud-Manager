using CloudManagerGeneralLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CloudSubClass;
using Cloud.MegaNz;
using System.IO;
using static Core.CloudSubClass.MegaNz;
using System.Threading;
using Cloud.GoogleDrive;

namespace Core
{
#if DEBUG
    public static class TestClass
    {
        const string email = "games_tqk@yahoo.com.vn";
        const string filepath = @"E:\Music\music\mp3tagv270setup.exe";
        const string filepath2 = @"E:\Music\music\XviD4PSP_5.10.346.0_(2015-04-07)_rc34.2.exe";
        public static void Test()
        {
            //testWindowCutCopy();
            //Mega();
            //FileInfo info = new FileInfo(filepath);
            //info.MoveTo(@"E:\test_folder\mp3tagv270setup.exe");
            //return;
        }

        static void Mega()
        {
            MegaApiClient client = MegaNz.GetClient("tqk2811@yahoo.com.vn");
            IAccountInformation info = client.GetAccountInformation();
            Console.WriteLine(info.UsedQuota.ToString() + "/" + info.TotalQuota.ToString());
        }

        static void testWindowCutCopy()
        {
            bool a = FileOperationAPIWrapper.Copy(@"D:\SoftWare\Noto-hinted.zip", @"E:\temp\Noto-hinted.zip");
            Console.WriteLine(a);
            Thread.Sleep(10000);
        }
    }
#endif
}
