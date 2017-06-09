using System.Windows.Forms;
using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;

namespace FormUI.UI.MainForm
{
    internal class TreeNode_ : TreeNode
    {
        public ExplorerNode explorernode { get; private set; }
        public TreeNode_(string text, int imageIndex) : base(text, imageIndex, imageIndex)
        {
            explorernode = new ExplorerNode();
            if ((CloudType)imageIndex != CloudType.LocalDisk)
            {
                explorernode.NodeType.Email = text;
                explorernode.NodeType.Type = (CloudType)imageIndex;
            }
            else
            {
                explorernode.Info.Name = text;
                explorernode.Info.Size = -1;
                explorernode.NodeType.Type = CloudType.LocalDisk;
            }
        }
        public TreeNode_(ExplorerNode node)
        {
            this.Text = (node.NodeType.Type != CloudType.Folder && node.NodeType.Type != CloudType.LocalDisk) ? node.NodeType.Email : node.Info.Name;
            this.ImageIndex = this.SelectedImageIndex = (int)node.NodeType.Type;//(int)CloudType.Folder;
            this.explorernode = node;
        }
    }
}