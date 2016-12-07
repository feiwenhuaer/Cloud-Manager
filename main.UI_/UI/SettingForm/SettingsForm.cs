using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Form.UI
{
    public partial class SettingsForm : System.Windows.Forms.Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }
        bool save = false;
        public bool IsChangeUI { get { return main_setting1.ChangeUI; } }
        public bool IsChangeLang { get { return main_setting1.ChangeLang; } }
        public bool IsSave { get { return save; } }

        private void BT_Save_Click(object sender, EventArgs e)
        {
            main_setting1.Save();
            save = true;
            this.Close();
        }
        private void BT_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
