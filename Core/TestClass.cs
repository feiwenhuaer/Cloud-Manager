using SupDataDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Cloud;
using Cloud.MegaNz;
using System.IO;
using static Core.Cloud.MegaNz;

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
            MegaApiClient client = MegaNz.GetClient(email);
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
            MegaNzNode node = new MegaNzNode(AppSetting.settings.GetRootID(email, CloudType.Mega));
            node.Type = NodeType.Root;
            client.Upload(fs, "testfile", node);
        }
    }
#endif
}
