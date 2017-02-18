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
    public partial class ClosingForm : System.Windows.Forms.Form, SupDataDll.UiInheritance.UIClosing
    {
        public ClosingForm()
        {
            InitializeComponent();
        }

        public void updatedata(string text)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => this.label1.Text = text));
            }
            else this.label1.Text = text;
        }

        public void Close_()
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => this.Close()));
            }
            else this.Close();
        }

        public void ShowDialog_()
        {
            this.ShowDialog();
        }
    }
}
