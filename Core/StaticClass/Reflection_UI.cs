using CloudManagerGeneralLib;
using CloudManagerGeneralLib.UiInheritance;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.StaticClass
{
    public static class Reflection_UI
    {
        public static void Load_Setting_UI()
        {
            //add event handler
            #region AddEventHandler reflection_eventtocore
            Type Type_setting = LoadDllUI.GetTypeInterface(typeof(SettingUI));
            Type Type_reflection_eventtocore = Type_setting.BaseType.GetField("reflection_eventtocore").FieldType;
            Reflection_EventToCore instance = Type_setting.BaseType.GetField("reflection_eventtocore").GetValue(null) as Reflection_EventToCore;

            instance.EventGetListAccountCloud += AppSetting.settings.GetListAccountCloud;
            instance.EventSetSetting += AppSetting.settings.SetSettingAsString;
            instance.EventSaveSetting += AppSetting.settings.SaveSettings;
            instance.EventGetSetting += AppSetting.settings.GetSettingsAsString;
            instance.EventDeleteAccountCloud += AppSetting.settings.RemoveCloud;
            instance.EventChangeUserPass += AppSetting.settings.ChangeUserPass;

            instance.EventShowFormOauth += AppSetting.ManageCloud.Oauth;
            instance.EventDeletePath += AppSetting.ManageCloud.Delete;
            instance.EventCreateFolder += AppSetting.ManageCloud.CreateFolder;
            instance.EventMoveItem += AppSetting.ManageCloud.MoveItem;
            instance.EventGetChildNode += AppSetting.ManageCloud.GetItemsList;

            instance.EventTransferItems += AppSetting.TransferManager.AddItems;
            instance.EventExitAppCallBack += AppSetting.TransferManager.Exit;

            instance.EventGetTextLanguage += AppSetting.lang.GetText;

            instance.EventLogin += AppSetting.login.Login_EventLoginCallBack;
            #endregion
        }

        public static void CreateInstanceLogin()
        {
            Type Login = LoadDllUI.GetTypeInterface(typeof(UILogin));
            AppSetting.UILogin = (UILogin)Activator.CreateInstance(Login);
        }

        public static void Load_UIMain()
        {
            Type Main = LoadDllUI.GetTypeInterface(typeof(UIMain));
            AppSetting.UIMain = (UIMain)Activator.CreateInstance(Main);

            Type uc_lv_ud = LoadDllUI.GetTypeInterface(typeof(UIUC_TLV_ud));
            AppSetting.uc_lv_ud_instance = (UIUC_TLV_ud)Activator.CreateInstance(uc_lv_ud);

            AppSetting.UIMain.load_uC_Lv_ud(AppSetting.uc_lv_ud_instance);
            AppSetting.TransferManager.LoadGroupToListView();
        }
    }
}