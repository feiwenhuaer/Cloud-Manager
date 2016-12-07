using System;
using System.Drawing;
using System.Windows.Forms;
namespace Form.UI
{
    public partial class FormOauth : System.Windows.Forms.Form
    {
        public FormOauth()
        {
            InitializeComponent();
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

        
        private void BT_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PB_Dropbox_Click(object sender, EventArgs e)
        {
            Setting_UI.reflection_eventtocore._ShowFormOauth(SupDataDll.CloudName.Dropbox);
            this.Close();
        }
        private void PB_GoogleDrive_Click(object sender, EventArgs e)
        {
            Setting_UI.reflection_eventtocore._ShowFormOauth(SupDataDll.CloudName.GoogleDrive);
            this.Close();
        }
        private void PB_Mega_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not support now", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //Setting_UI.reflection_eventtocore.ShowFormOauth_(SupDataDll.CloudName.Mega);
            //this.Close();
        }
    }
}
