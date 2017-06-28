using System.Collections.Generic;

namespace CloudManagerGeneralLib.Class
{
    public class DeleteItems
    {
        public DeleteItems()
        {

        }
        public DeleteItems(ItemNode item)
        {
            Items.Add(item);
        }

        public DeleteItems(List<ItemNode> items)
        {
            Items.AddRange(items);
        }
        List<ItemNode> items = new List<ItemNode>();

        public List<ItemNode> Items { get { return items; } set { items = value; } }
        public bool PernamentDelete = false;
    }

}
