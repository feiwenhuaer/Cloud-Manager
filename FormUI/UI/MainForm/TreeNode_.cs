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
                explorernode.RootInfo.Email = text;
                explorernode.RootInfo.Type = (CloudType)imageIndex;
            }
            else
            {
                explorernode.Info.Name = text;
                explorernode.Info.Size = -1;
                explorernode.RootInfo.Type = CloudType.LocalDisk;
            }
        }
        public TreeNode_(ExplorerNode node)
        {
            this.Text = node.Info.Name;
            this.ImageIndex = this.SelectedImageIndex = (int)CloudType.Folder;
            this.explorernode = node;
        }
    }
}