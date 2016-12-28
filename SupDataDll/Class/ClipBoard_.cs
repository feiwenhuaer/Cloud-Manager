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
        public static List<NewTransferItem> Items = new List<NewTransferItem>();
        public static void Clear()
        {
            Items = new List<NewTransferItem>();
            Clipboard = false;
            directory = string.Empty;
        }

        public static void Add(NewTransferItem item)
        {
            Items.Add(item);
        }

        public static void Add(NewTransferItem[] item)
        {
            Items.AddRange(item);
        }

        public static void Add(List<NewTransferItem> item)
        {
            Items.AddRange(item);
        }
    }
}
