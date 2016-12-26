﻿using SupDataDll.UiInheritance;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.StaticClass
{
    public static class Reflection_UI
    {
        public static object instance_ListAccountCloud;
        public static void Load_Setting_UI()
        {
            //set instance
            #region Create instance UI 
            Type Login = LoadDllUI.GetTypeInterface(typeof(UILogin));
            AppSetting.UILogin = (UILogin)Activator.CreateInstance(Login);
            #endregion

            //add event handler
            #region AddEventHandler reflection_eventtocore
            Type Type_setting = LoadDllUI.GetTypeInterface(typeof(SupDataDll.UiInheritance.UI));
            Type Type_reflection_eventtocore = Type_setting.BaseType.GetField("reflection_eventtocore").FieldType;
            object instance_reflection_eventtocore = Type_setting.BaseType.GetField("reflection_eventtocore").GetValue(null);
            List<object[]> listloadevent = new List<object[]>() {
            new object[] { Type_reflection_eventtocore, "EventAddItem",AppSetting.ud_items,"AddItems", instance_reflection_eventtocore }
            ,new object[] { Type_reflection_eventtocore, "EventGetListAccountCloud", AppSetting.settings, "GetListAccountCloud", instance_reflection_eventtocore }

            ,new object[] { Type_reflection_eventtocore, "EventSetSetting", AppSetting.settings, "SetSettingAsString", instance_reflection_eventtocore }
            ,new object[] { Type_reflection_eventtocore, "EventSaveSetting", AppSetting.settings,"SaveSettings", instance_reflection_eventtocore }
            ,new object[] { Type_reflection_eventtocore, "EventGetSetting", AppSetting.settings, "GetSettingsAsString", instance_reflection_eventtocore }

            ,new object[] { Type_reflection_eventtocore, "EventGetTextLanguage", AppSetting.lang, "GetText", instance_reflection_eventtocore }
            ,new object[] { Type_reflection_eventtocore, "EventRenameItem", AppSetting.ManageCloud, "RenameItem", instance_reflection_eventtocore }
            ,new object[] { Type_reflection_eventtocore, "EventDeleteAccountCloud", AppSetting.settings, "RemoveCloud", instance_reflection_eventtocore }
            ,new object[] { Type_reflection_eventtocore ,"EventListIteamRequest",AppSetting.ManageCloud, "GetItemsList", instance_reflection_eventtocore }
            ,new object[] { Type_reflection_eventtocore, "EventLogin", AppSetting.login, "Login_EventLoginCallBack", instance_reflection_eventtocore}
            ,new object[] { Type_reflection_eventtocore, "EventExitAppCallBack", AppSetting.ud_items, "Exit", instance_reflection_eventtocore }
            ,new object[] { Type_reflection_eventtocore, "EventShowFormOauth", AppSetting.ManageCloud, "Oauth", instance_reflection_eventtocore }
            ,new object[] { Type_reflection_eventtocore, "EventDeletePath", AppSetting.ManageCloud, "Delete", instance_reflection_eventtocore }
            ,new object[] { Type_reflection_eventtocore, "EventCreateFolder",AppSetting.ManageCloud, "CreateFolder", instance_reflection_eventtocore }
            ,new object[] { Type_reflection_eventtocore, "EventChangeUserPass",AppSetting.settings, "ChangeUserPass", instance_reflection_eventtocore }
            };
            foreach (object[] item in listloadevent)
            {
                EventInfo Event_ = ((Type)item[0]).GetEvent((string)item[1]);
                Delegate Event_handler = Delegate.CreateDelegate(Event_.EventHandlerType, item[2], (string)item[3]);
                Event_.AddEventHandler(item[4], Event_handler);
            }
            #endregion
        }

        public static void Load_UIMain()
        {
            Type Main = LoadDllUI.GetTypeInterface(typeof(UIMain));
            AppSetting.UIMain = (UIMain)Activator.CreateInstance(Main);

            Type uc_lv_ud = LoadDllUI.GetTypeInterface(typeof(UIUC_TLV_ud));
            AppSetting.uc_lv_ud_instance = (UIUC_TLV_ud)Activator.CreateInstance(uc_lv_ud);

            AppSetting.UIMain.load_uC_Lv_ud(AppSetting.uc_lv_ud_instance);
        }
    }
}
