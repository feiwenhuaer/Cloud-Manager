using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CloudManagerGeneralLib
{
    public static class GetList_UI_n_lang
    {
        public static List<string> GetListUiFile()
        {
            List<string> list = new List<string>();
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*UI.dll"))
            {
                FileInfo info = new FileInfo(file);
                list.Add(info.Name);
            }
            return list;
        }

        public static List<string> GetListLangFile()
        {
            List<string> list = new List<string>();
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory() + "\\lang", "*.xml"))
            {
                FileInfo info = new FileInfo(file);
                list.Add(info.Name);
            }
            return list;
        }
    }
}
