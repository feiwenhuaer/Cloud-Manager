using CloudManagerGeneralLib.Class;
using System.Collections.Generic;

namespace CloudManagerGeneralLib
{
    public static class AppClipboard
    {
        public static bool Clipboard = false;
        public static bool AreCut = false;
        public static IItemNode directory;
        public static List<IItemNode> Items = new List<IItemNode>();
        public static void Clear()
        {
            Items = new List<IItemNode>();
            Clipboard = false;
            directory = null;
        }

        public static void Add(IItemNode item)
        {
            Items.Add(item);
        }

        public static void Add(IItemNode[] item)
        {
            Items.AddRange(item);
        }

        public static void Add(List<IItemNode> item)
        {
            Items.AddRange(item);
        }
    }
}
