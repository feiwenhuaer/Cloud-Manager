using System.Collections.Generic;

namespace SupDataDll.Class
{
    public class DeleteItems
    {
        public DeleteItems()
        {

        }
        public DeleteItems(ExplorerNode item)
        {
            Items.Add(item);
        }

        public DeleteItems(List<ExplorerNode> items)
        {
            Items.AddRange(items);
        }
        List<ExplorerNode> items = new List<ExplorerNode>();

        public List<ExplorerNode> Items { get { return items; } set { items = value; } }
        public bool PernamentDelete = false;
    }

}
