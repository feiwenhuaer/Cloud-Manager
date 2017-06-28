using CloudManagerGeneralLib.Class;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace FormUI.UI
{
    public partial class RenameItem : Form
    {
        public RenameItem(ItemNode node)
        {
            InitializeComponent();
            this.node = node;
        }

        #region Move Form
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        private void pnlMain_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void pnlMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void pnlMain_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        #endregion

        ItemNode node;

        private void BT_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TB_newname_TextChanged(object sender, EventArgs e)
        {
            if (TB_oldname.Text == TB_newname.Text) BT_change.Enabled = false;
            else BT_change.Enabled = true;
        }

        private void BT_change_Click(object sender, EventArgs e)
        {
            DoRename();
        }

        void DoRename()
        {
            Thread thr = new Thread(Rename);
            Setting_UI.ManagerThreads.rename.Add(thr);
            Setting_UI.ManagerThreads.CleanThr();
            thr.Start();
        }
        void Rename()
        {
            //Setting_UI.reflection_eventtocore._MoveItem()
            //AnalyzePath ap = new AnalyzePath(raw_path);
            //if (ap.TypeCloud == CloudType.GoogleDrive ?
            //            Setting_UI.reflection_eventtocore._MoveItem(null, null, id, null, null, TB_newname.Text, ap.Email, CloudType.GoogleDrive):
            //            Setting_UI.reflection_eventtocore._MoveItem(ap.AddRawChildPath(oldname), ap.AddRawChildPath(TB_newname.Text), id, null, null, null, null)                        
            //    )
            //{
            //    Invoke(new Action(() =>
            //    {
            //        MessageBox.Show(this, "Rename successful", "Message response", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        this.Close();
            //    }));

            //}
            //else
            //{
            //    bool flag = false;
            //    Invoke(new Action(() =>
            //    {
            //        DialogResult result = MessageBox.Show(this, "Rename Error", "Message response", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            //        if (result == DialogResult.Retry) flag = true;
            //        else this.Close();
            //    }));
            //    if (flag) Rename();
            //}
        }

        private void RenameItem_Load(object sender, EventArgs e)
        {
            TB_oldname.Text = TB_newname.Text = this.node.Info.Name;
            CenterToParent();
            this.ActiveControl = TB_newname;
            this.BackColor = Setting_UI.Background;
        }

        private void TB_newname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && BT_change.Enabled == true) DoRename();
        }
    }
}

