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
        bool closeflag = false;

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
        
        public void ShowDialog_()
        {
            this.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
#if DEBUG
            Console.WriteLine("ClosingForm: timer1_tick");
#endif
            if (closeflag) this.Close();
        }

        public void Close_()
        {
            closeflag = true;
        }
    }
}
