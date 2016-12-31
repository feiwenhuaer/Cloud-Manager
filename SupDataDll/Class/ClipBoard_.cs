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
        public static List<AddNewTransferItem> Items = new List<AddNewTransferItem>();
        public static void Clear()
        {
            Items = new List<AddNewTransferItem>();
            Clipboard = false;
            directory = string.Empty;
        }

        public static void Add(AddNewTransferItem item)
        {
            Items.Add(item);
        }

        public static void Add(AddNewTransferItem[] item)
        {
            Items.AddRange(item);
        }

        public static void Add(List<AddNewTransferItem> item)
        {
            Items.AddRange(item);
        }
    }
}
