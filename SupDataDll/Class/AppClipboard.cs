using CloudManagerGeneralLib.Class;
using System.Collections.Generic;

namespace CloudManagerGeneralLib
{
    public static class AppClipboard
    {
        public static bool Clipboard = false;
        public static bool AreCut = false;
        public static ItemNode directory;
        public static List<ItemNode> Items = new List<ItemNode>();
        public static void Clear()
        {
            Items = new List<ItemNode>();
            Clipboard = false;
            directory = null;
        }

        public static void Add(ItemNode item)
        {
            Items.Add(item);
        }

        public static void Add(ItemNode[] item)
        {
            Items.AddRange(item);
        }

        public static void Add(List<ItemNode> item)
        {
            Items.AddRange(item);
        }
    }
}
