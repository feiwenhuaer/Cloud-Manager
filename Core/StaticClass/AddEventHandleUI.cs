using CloudManagerGeneralLib;
using CloudManagerGeneralLib.UiInheritance;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.StaticClass
{
    public static class AddEventHandleUI
    {
        public static void Load_Setting_UI()
        {
            //add event handler
            #region AddEventHandler reflection_eventtocore
            Type Type_setting = LoadDllUI.GetTypeInterface(typeof(SettingUI));
            Type Type_reflection_eventtocore = Type_setting.BaseType.GetField("reflection_eventtocore").FieldType;
            RequestToCore instance = Type_setting.BaseType.GetField("reflection_eventtocore").GetValue(null) as RequestToCore;

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
            AppSetting.TransferManager.LoadGroupToListView();
        }
    }
}