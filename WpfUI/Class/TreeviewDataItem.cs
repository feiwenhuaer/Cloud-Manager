using SupDataDll;
using SupDataDll.Class;
using System.Windows.Media;

namespace WpfUI.Class
{
    public class TreeviewDataItem
    {
        ExplorerNode node;
        public ExplorerNode Node { get { return node; } set { node = value; Update(); } }
        public ImageSource ImgSource { get; private set; }
        public CloudType Type { get; private set; }

        public string Name { get; private set; }

        public TreeviewDataItem(ExplorerNode Node)
        {
            this.Node = Node;
        }

        void Update()
        {
            this.Type = Node.RootInfo.Type;
            ImgSource = Setting_UI.GetImage(ListBitmapImageResource.list_bm_cloud[(int)this.Type]).Source;
            switch (this.Type)
            {
                case CloudType.Folder: 
                case CloudType.LocalDisk: this.Name = Node.Info.Name; break;
                default: this.Name = Node.RootInfo.Email; break;
            }
        }
    }
}
