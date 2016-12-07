using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FormUI.UI
{
    public partial class CreateFolderForm : System.Windows.Forms.Form
    {

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

        public string Path;
        public string Id;
        public string Email;
        public CreateFolderForm()
        {
            InitializeComponent();
            BT_Create.Enabled = false;
        }

        private void CreateFolderForm_Load(object sender, EventArgs e)
        {
            this.BackColor = Setting_UI.Background;
            CenterToParent();
            label1.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.CreateFolderForm_name);
            BT_Create.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.BT_create);
            BT_Cancel.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.BT_cancel);
        }

        private void BT_Create_Click(object sender, EventArgs e)
        {
            DoCreateFolder();
        }

        private void BT_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Regex rg = new Regex("^[^\\/\\\\:<>\\|\"*?]+$");// ^[^\\/\\\\:<>\\|\"*?]+$
            Match match = rg.Match(textBox1.Text);
            if (match.Success) BT_Create.Enabled = true;
            else BT_Create.Enabled = false;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) DoCreateFolder();
        }

        void DoCreateFolder()
        {
            Thread thr = new Thread(CreateFolder);
            Setting_UI.ManagerThreads.CleanThr();
            Setting_UI.ManagerThreads.createfolder.Add(thr);
            thr.Start();
        }
        void CreateFolder()
        {
            AnalyzePath ap = new AnalyzePath(Path);
            string temp = ap.AddRawChildPath(textBox1.Text);
            Setting_UI.reflection_eventtocore._CreateFolder(temp, Id, Email, textBox1.Text);
            Invoke(new Action(() => this.Close()));
        }

    }
}
