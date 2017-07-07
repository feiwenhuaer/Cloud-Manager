using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CloudManagerGeneralLib.Class;

namespace FormUI.UI.MainForm.PathNodes
{
    public delegate void NodePathClick(IItemNode nodeclick);
    public partial class PathUC : UserControl
    {
        public PathUC()
        {
            InitializeComponent();
        }

        public event NodePathClick EventNodePathClick;

        public PathUC(ItemNode node)
        {
            InitializeComponent();
            this.Node = node;
        }
        IItemNode oldnode;
        IItemNode node;
        public IItemNode Node { get { return node; } set { node = value; Make(); } }

        List<LabelNode> list_n_uc = new List<LabelNode>();
        int list_n_uc_HideIndex = 1;
        bool up = true;
        void Make()
        {
            if (node == oldnode) return;
            List<IItemNode> list = node.GetFullPath();
            if (oldnode != null)//old node
            {
                IItemNode sameparent = oldnode.FindSameParent(node);//find sameparent (new node and old node)
                if (sameparent != null)
                {
                    if (list.Count < oldnode.GetFullPath().Count) up = false;//explorer to child or parent?
                    int index = list_n_uc.IndexOf(list_n_uc.Find(uc => uc.Node == sameparent));//find index sameparent at list showing
                    if (index < list_n_uc.Count && list_n_uc.Count >= 1)//remove from (index +1) to (Count -1) at list showing
                    {
                        int i = index + 1;
                        while (i < list_n_uc.Count)
                        {
                            this.Controls.Remove(list_n_uc[i]);
                            list_n_uc.RemoveAt(i);
                        }
                    }
                    list.RemoveRange(0, list.IndexOf(sameparent) + 1);//remove from [root to sameparent] of newlist (need from (index +1) to  (Count -1))


                    //while (list_n_uc[list_n_uc.Count-1].Location.X + list_n_uc[list_n_uc.Count-1].Width > this.Width)
                    //{

                    //}
                }
                else//if not sameparent then clear all
                {
                    list_n_uc.ForEach(n => this.Controls.Remove(n));
                    list_n_uc.Clear();
                }
            }
            foreach (IItemNode n in list)//add 
            {
                LabelNode n_uc = new LabelNode(n);
                n_uc.Dock = DockStyle.Left;
                n_uc.AutoSize = true;
                n_uc.Margin = new Padding(2, 0, 2, 0);
                n_uc.Click += N_uc_Click;
                n_uc.Padding = new Padding(0);
                list_n_uc.Add(n_uc);
                this.Controls.Add(n_uc);
                n_uc.BringToFront();
                while (n_uc.Location.X + n_uc.Width > this.Width)
                {
                    try
                    {
                        list_n_uc[list_n_uc_HideIndex].Hide();
                        list_n_uc_HideIndex++;
                    }
                    catch { break; }
                }
            }
            oldnode = node;
        }

        private void N_uc_Click(object sender, EventArgs e)
        {
            if (EventNodePathClick != null) EventNodePathClick(((LabelNode)((Control)sender).Parent).Node);
        }
        int oldsize = 0;
        private void PathUC_Resize(object sender, EventArgs e)
        {
            if (list_n_uc.Count < 3) return;
            //if(this.Size.Width > oldsize)
            //{
            //    list_n_uc[list_n_uc.Count - 1].Show();
            //}
            //else
            //{

            //}
        }
    }
}
