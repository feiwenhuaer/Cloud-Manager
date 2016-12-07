using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupDataDll
{
    public static class ClipBoard_
    {
        public static bool Clipboard = false;
        public static bool AreCut = false;
        public static string directory;
        public static List<UpDownloadItem> Items = new List<UpDownloadItem>();
        public static void Clear()
        {
            Items = new List<UpDownloadItem>();
            Clipboard = false;
            directory = string.Empty;
        }

        public static void Add(UpDownloadItem item)
        {
            Items.Add(item);
        }

        public static void Add(UpDownloadItem[] item)
        {
            Items.AddRange(item);
        }

        public static void Add(List<UpDownloadItem> item)
        {
            Items.AddRange(item);
        }
    }
}
