using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CloudManagerGeneralLib;

namespace FormUI.UI.SettingForm
{
    public partial class main_setting : UserControl
    {
        public main_setting()
        {
            InitializeComponent();
        }
        public bool CanSave = false;
        private void main_setting_Load(object sender, EventArgs e)
        {
            ReadToForm();
        }

        private void TB_oldpass_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TB_oldpass.Text)) { TB_newpass0.Enabled = false; TB_newpass1.Enabled = false; TB_newpass0.Text = ""; TB_newpass1.Text = ""; }
            else { TB_newpass0.Enabled = true; TB_newpass1.Enabled = true; }
        }

        private void TB_newpass0_TextChanged(object sender, EventArgs e)
        {
            if (TB_newpass1.Text != TB_newpass0.Text | TB_newpass0.Text == TB_oldpass.Text)
            {
                CanSave = false;
            }
            else CanSave = true;
        }
        string langfilename;
        string uifilename;
        public bool ChangeUI = false;
        public bool ChangeLang = false;

        void ReadToForm()
        {
            CB_AutoStartTransfer.Checked = Setting_UI.reflection_eventtocore.GetSetting(SettingsKey.AutoStartTransfer) == "1" ? true : false;
            CB_autologin.Checked = Setting_UI.reflection_eventtocore.GetSetting(SettingsKey.AutoLogin) == "1" ? true : false;
            CB_shutdown.Checked = Setting_UI.reflection_eventtocore.GetSetting(SettingsKey.ShutdownWhenDone) == "1" ? true : false;

            CBB_lang.Items.AddRange(GetList_UI_n_lang.GetListLangFile().ToArray());
            langfilename = Setting_UI.reflection_eventtocore.GetSetting(SettingsKey.lang);
            CBB_lang.SelectedText = langfilename;
            CBB_ui.Items.AddRange(GetList_UI_n_lang.GetListUiFile().ToArray());
            uifilename = Setting_UI.reflection_eventtocore.GetSetting(SettingsKey.UI_dll_file);
            CBB_ui.SelectedText = uifilename;

            decimal MaxGroupsDownload = 2;
            decimal MaxItemsInGroupDownload = 2;
            decimal BufferSize = 32;
            decimal GD_ChunksSize = 5;
            decimal Dropbox_ChunksSize = 25;
            Decimal.TryParse(Setting_UI.reflection_eventtocore.GetSetting(SettingsKey.MaxGroupsDownload), out MaxGroupsDownload);
            Decimal.TryParse(Setting_UI.reflection_eventtocore.GetSetting(SettingsKey.MaxItemsInGroupDownload), out MaxItemsInGroupDownload);
            Decimal.TryParse(Setting_UI.reflection_eventtocore.GetSetting(SettingsKey.BufferSize), out BufferSize);
            Decimal.TryParse(Setting_UI.reflection_eventtocore.GetSetting(SettingsKey.GD_ChunksSize), out GD_ChunksSize);
            Decimal.TryParse(Setting_UI.reflection_eventtocore.GetSetting(SettingsKey.Dropbox_ChunksSize), out Dropbox_ChunksSize);
            try { NUD_group.Value = MaxGroupsDownload; } catch { }
            try { NUD_item.Value = MaxItemsInGroupDownload; } catch { }
            try { NUD_buffer.Value = BufferSize; } catch { }
            try { NUD_GDchunks.Value = GD_ChunksSize; } catch { }
            try { NUD_DBchunks.Value = Dropbox_ChunksSize; } catch { }
        }

        public void Save()
        {
            if (!ChangeUser_Pass())
            {
                MessageBox.Show("Can't set new user or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Setting_UI.reflection_eventtocore.SetSetting(SettingsKey.AutoStartTransfer, CB_AutoStartTransfer.Checked ? "1" : "0");
            Setting_UI.reflection_eventtocore.SetSetting(SettingsKey.AutoLogin, CB_autologin.Checked ? "1" : "0");
            Setting_UI.reflection_eventtocore.SetSetting(SettingsKey.ShutdownWhenDone, CB_shutdown.Checked ? "1" : "0");

            if (!string.IsNullOrEmpty(CBB_lang.Text) && CBB_lang.Text != langfilename)
            {
                Setting_UI.reflection_eventtocore.SetSetting(SettingsKey.lang, CBB_lang.Text);
                ChangeUI = true;
            }
            if (!string.IsNullOrEmpty(CBB_ui.Text) && CBB_ui.Text != uifilename)
            {
                Setting_UI.reflection_eventtocore.SetSetting(SettingsKey.UI_dll_file, CBB_ui.Text);
                ChangeLang = true;
            }

            Setting_UI.reflection_eventtocore.SetSetting(SettingsKey.MaxGroupsDownload, NUD_group.Value.ToString());
            Setting_UI.reflection_eventtocore.SetSetting(SettingsKey.MaxItemsInGroupDownload, NUD_item.Value.ToString());
            Setting_UI.reflection_eventtocore.SetSetting(SettingsKey.BufferSize, NUD_buffer.Value.ToString());
            Setting_UI.reflection_eventtocore.SetSetting(SettingsKey.GD_ChunksSize, NUD_GDchunks.Value.ToString());
            Setting_UI.reflection_eventtocore.SetSetting(SettingsKey.Dropbox_ChunksSize, NUD_DBchunks.Value.ToString());
            Setting_UI.reflection_eventtocore.SaveSetting();
        }

        public bool ChangeUser_Pass()
        {
            if (!string.IsNullOrEmpty(TB_newpass0.Text) & !CanSave)
            {
                return false;
            }

            if (CanSave)
            {
                return Setting_UI.reflection_eventtocore.ChangeUserPass(TB_username.Text, TB_oldpass.Text, TB_newpass0.Text);
            }
            return true;
        }
    }
}

