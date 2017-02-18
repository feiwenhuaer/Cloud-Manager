using SupDataDll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormUI.UI
{
    public partial class ChangeNumberItemsTransfer : System.Windows.Forms.Form
    {
        public ChangeNumberItemsTransfer(int numberitems)
        {
            InitializeComponent();
            this.numberitems = numberitems;
            label1.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.ChangeNumberItemDownload);
            BT_Save.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.BT_save);
            BT_Cancel.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.BT_cancel);
            numericUpDown1.Value = numberitems;
        }
        private int numberitems = 2;
        public int NumberItems { get { return numberitems; } }

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


        private void BT_Save_Click(object sender, EventArgs e)
        {
            numberitems = Decimal.ToInt32(numericUpDown1.Value);
            this.Close();
        }

        private void BT_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChangeNumberItemDownload_Load(object sender, EventArgs e)
        {
            this.BackColor = Setting_UI.Background;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (Decimal.ToInt32(numericUpDown1.Value) == numberitems) BT_Save.Enabled = false;
            else BT_Save.Enabled = true;
        }
    }
}
