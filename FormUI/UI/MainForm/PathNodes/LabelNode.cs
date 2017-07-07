using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FormUI.UI.MainForm.PathNodes
{
    internal class LabelNode : Label
    {
        IItemNode node;
        public IItemNode Node { get { return node; } private set { node = value; ChangeText(); } }
        public LabelNode(IItemNode node) : base()
        {
            this.Node = node;
            this.MouseEnter += C_MouseEnter;
            this.MouseLeave += C_MouseLeave;
            C_MouseLeave(null, EventArgs.Empty);
        }

        void ChangeText()
        {
            RootNode root = node as RootNode;
            if (root != null && root.RootType.Type != CloudType.LocalDisk) this.Text = root.RootType.Type.ToString() + ":" + root.RootType.Email;//root
            else this.Text = node.Info.Name;
        }
        private void C_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = System.Drawing.SystemColors.ControlLight;
        }

        private void C_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.DarkGray;
        }
    }
}
