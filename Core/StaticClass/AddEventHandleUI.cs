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

      instance.AccountsAndCloud.EventGetListAccountCloud += AppSetting.settings.GetListAccountCloud;
      instance.SettingAndLanguage.EventSetSetting += AppSetting.settings.SetSettingAsString;
      instance.SettingAndLanguage.EventSaveSetting += AppSetting.settings.SaveSettings;
      instance.SettingAndLanguage.EventGetSetting += AppSetting.settings.GetSettingsAsString;
      instance.AccountsAndCloud.EventDeleteAccountCloud += AppSetting.settings.RemoveCloud;
      instance.AccountsAndCloud.EventChangeUserPass += AppSetting.settings.ChangeUserPass;

      instance.AccountsAndCloud.EventShowFormOauth += AppSetting.ManageCloud.Oauth;
      instance.ExplorerAndManagerFile.EventDeletePath += AppSetting.ManageCloud.Delete;
      instance.ExplorerAndManagerFile.EventCreateFolder += AppSetting.ManageCloud.CreateFolder;
      instance.ExplorerAndManagerFile.EventMoveItem += AppSetting.ManageCloud.MoveItem;
      instance.ExplorerAndManagerFile.EventGetChildNode += AppSetting.ManageCloud.GetItemsList;

      instance.ExplorerAndManagerFile.EventTransferItems += AppSetting.TransferManager.AddItems;
      instance.EventExitAppCallBack += AppSetting.TransferManager.Shutdown;

      instance.SettingAndLanguage.EventGetTextLanguage += AppSetting.lang.GetText;

      instance.AccountsAndCloud.EventLogin += AppSetting.login.Login_EventLoginCallBack;
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
      AppSetting.TransferManager.ItemsTransfer = AppSetting.UIMain.ItemsTransfer;
    }
  }
}