using SupDataDll;
using System;
using System.Windows.Forms;

namespace Form.UI
{
    public partial class DeleteConfirmForm : System.Windows.Forms.Form
    {
        public DeleteConfirmForm()
        {
            InitializeComponent();
            label1.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.DeleteConfirmForm_waning);
            CB_pernament.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.CB_pernament);
            BT_cancel.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.BT_cancel);
            BT_yes.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.BT_yes);
            this.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.DeleteConfirmForm_text);
        }
        public bool Delete = false;

        private void BT_yes_Click(object sender, EventArgs e)
        {
            Delete = true;
            this.Close();
        }

        private void BT_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
