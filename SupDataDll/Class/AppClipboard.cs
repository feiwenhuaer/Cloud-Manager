using CloudManagerGeneralLib.Class;
using System.Collections.Generic;

namespace CloudManagerGeneralLib
{
    public static class AppClipboard
    {
        public static bool Clipboard = false;
        public static bool AreCut = false;
        public static ExplorerNode directory;
        public static List<ExplorerNode> Items = new List<ExplorerNode>();
        public static void Clear()
        {
            Items = new List<ExplorerNode>();
            Clipboard = false;
            directory = null;
        }

        public static void Add(ExplorerNode item)
        {
            Items.Add(item);
        }

        public static void Add(ExplorerNode[] item)
        {
            Items.AddRange(item);
        }

        public static void Add(List<ExplorerNode> item)
        {
            Items.AddRange(item);
        }
    }
}
