using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FormUI.UI.MainForm.PathNodes
{
    internal class LabelNode : Label
    {
        ExplorerNode node;
        public ExplorerNode Node { get { return node; } private set { node = value; ChangeText(); } }
        public LabelNode(ExplorerNode node) : base()
        {
            this.Node = node;
            this.MouseEnter += C_MouseEnter;
            this.MouseLeave += C_MouseLeave;
            C_MouseLeave(null, EventArgs.Empty);
        }

        void ChangeText()
        {
            if (node.Parent == null && node.NodeType.Type != CloudType.LocalDisk)//root
            {
                this.Text = node.NodeType.Type.ToString() + ":" + node.NodeType.Email;
            }
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
