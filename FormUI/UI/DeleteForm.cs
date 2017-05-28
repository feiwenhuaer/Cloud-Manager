using CloudManagerGeneralLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CloudManagerGeneralLib.UiInheritance;

namespace FormUI.UI
{
    public partial class DeleteForm : System.Windows.Forms.Form, CloudManagerGeneralLib.UiInheritance.UIDelete
    {
        #region interface
        bool autoclose = true;
        public event CancelDelegate EventCancel;
        public event ClosingDelegate EventClosing;

        public bool AutoClose
        {
            get
            {
                return autoclose;
            }
        }

        public void Show_(object owner)
        {
            this.Owner = (Form)owner;
            this.Show();
        }

        public void SetTextButtonCancel(string text)
        {
            if (InvokeRequired) this.Invoke(new Action(() => BT_cancel.Text = text));
            else BT_cancel.Text = text;
        }

        public void Close_()
        {
            if (InvokeRequired) this.Invoke(new Action(() => this.Close()));
            else this.Close();
        }

        public void UpdateText(string text)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => TB.Text += text));
            }
            else
            {
                TB.Text += text;
            }

        }
        public void SetAutoClose(bool c)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => { CB_autoclose.Checked = c; autoclose = c; }));
            }
            else
            {
                CB_autoclose.Checked = c;
                autoclose = c;
            }
        }
        #endregion

        public DeleteForm()
        {
            InitializeComponent();
            CB_autoclose.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.DeleteForm_CB_autoclose);
            BT_cancel.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.BT_cancel);
            this.Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(LanguageKey.DeleteForm_text);
        }

        #region Event Form
        private void BT_cancel_Click(object sender, EventArgs e)
        {
            EventCancel();
        }

        private void CB_autoclose_CheckedChanged(object sender, EventArgs e)
        {
            autoclose = CB_autoclose.Checked;
        }

        private void DeleteForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            autoclose = true;
            EventClosing();
        }
        #endregion
    }
}

