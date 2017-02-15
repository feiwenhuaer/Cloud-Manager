using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupDataDll;
using Core.Transfer;

namespace Test
{
    class Test
    {
        static void Main(string[] args)
        {
            //string pf = @"E:\abcd";
            //string pt = "GoogleDrive:tqk2811@gmail.com/tqk2811@yahoo.com.vn/root";
            //AnalyzePath apf = new AnalyzePath(pf);
            //AnalyzePath apt = new AnalyzePath(pt);
            //while(true)
            //{
            //    Console.Write("Path:" + pf);
            //    string path = pf + Console.ReadLine();
            //    Console.WriteLine("Result:" + AnalyzePath.GetPathTo(new List<string>() { path }, apf, apt)[0]);
            //}

            GroupsTransferManager gtm = new GroupsTransferManager();
            gtm.ReadData();
            foreach(var group in gtm.GroupsWork)
            {
                foreach(var item in group.GroupData.items)
                {
                    Console.WriteLine("From:" + item.From.path);
                    Console.WriteLine("To:" + item.To.path);
                }
            }
            Console.ReadLine();
        }
    }
}
