using System.Windows.Forms;
using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;

namespace FormUI.UI.MainForm
{
    internal class TreeNode_ : TreeNode
    {
        public IItemNode ExplorerNode { get; private set; }
        public TreeNode_(string text, int imageIndex) : base(text, imageIndex, imageIndex)
        {
            RootNode explorernode = new RootNode();
            if ((CloudType)imageIndex != CloudType.LocalDisk)
            {
                explorernode.RootType.Email = text;
                explorernode.RootType.Type = (CloudType)imageIndex;
            }
            else
            {
                explorernode.Info.Name = text;
                explorernode.Info.Size = -1;
                explorernode.RootType.Type = CloudType.LocalDisk;
            }
            this.ExplorerNode = explorernode;
        }
        public TreeNode_(IItemNode node)
        {
            this.Text = ((node is RootNode) && (node as RootNode).RootType.Type != CloudType.LocalDisk) ? (node as RootNode).RootType.Email : node.Info.Name;
            this.ImageIndex = this.SelectedImageIndex = (node is RootNode) ? (int)(node as RootNode).RootType.Type : (int)CloudType.Folder;//(int)CloudType.Folder;
            this.ExplorerNode = node;
        }
    }
}