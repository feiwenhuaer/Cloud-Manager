using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupDataDll
{
    public class Reflection_EventToCore
    {
        public bool _ChangeUserPass(string user, string pass, string newpass)
        {
            return EventChangeUserPass(user, pass, newpass);
        }
        public delegate bool ChangeUserPass(string user, string pass, string newpass);
        public event ChangeUserPass EventChangeUserPass;

        /// <summary>
        /// Add list item for download upload
        /// </summary>
        /// <param name="items">List Items</param>
        /// <param name="fromfolder">From</param>
        /// <param name="savefolder">To</param>
        /// <param name="AreCut">Cut or Copy (Cut will delete items in From folder)</param>
        public void _AddItem(List<AddNewTransferItem> items, string fromfolder, string savefolder, bool AreCut)
        {
            EventAddItem(items, fromfolder, savefolder, AreCut);
        }
        public delegate void AddItem(List<AddNewTransferItem> items, string fromfolder, string savefolder, bool AreCut);
        public event AddItem EventAddItem;
        /// <summary>
        /// SetSetting
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="Data">Value to set</param>
        public void _SetSetting(SettingsKey Key, string Data)
        {
            EventSetSetting(Key, Data);
        }
        public delegate void SetSetting(SettingsKey Key, string Data);
        public event SetSetting EventSetSetting;
        /// <summary>
        /// Save Setting
        /// </summary>
        public void _SaveSetting()
        {
            EventSaveSetting();
        }
        public delegate void SaveSetting();
        public event SaveSetting EventSaveSetting;
        /// <summary>
        /// Read Setting
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string _GetSetting(SettingsKey Key)
        {
            return EventGetSetting(Key);
        }
        public delegate string GetSetting(SettingsKey Key);
        public event GetSetting EventGetSetting;
        /// <summary>
        /// Read language
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string _GetTextLanguage(string Key)
        {
            return EventGetTextLanguage(Key);
        }
        /// <summary>
        /// Read language
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string _GetTextLanguage(LanguageKey Key)
        {
            return EventGetTextLanguage(Key.ToString());
        }
        public delegate string GetTextLanguage(string Key);
        public event GetTextLanguage EventGetTextLanguage;
        /// <summary>
        /// Delete Cloud Account
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool _DeleteAccountCloud(string Email, CloudName type)
        {
            return EventDeleteAccountCloud(Email, type);
        }
        public delegate bool DeleteAccountCloud(string Email, CloudName type);
        public event DeleteAccountCloud EventDeleteAccountCloud;
        /// <summary>
        /// Get List Cloud Account
        /// </summary>
        /// <returns></returns>
        public List<CloudEmail_Type> _GetListAccountCloud()
        {
            return EventGetListAccountCloud();
        }
        public delegate List<CloudEmail_Type> GetListAccountCloud();
        public event GetListAccountCloud EventGetListAccountCloud;
        /// <summary>
        /// Oauth for add cloud account
        /// </summary>
        /// <param name="type"></param>
        public void _ShowFormOauth(CloudName type)
        {
            EventShowFormOauth(type);
        }
        public delegate void ShowFormOauth(CloudName type);
        public event ShowFormOauth EventShowFormOauth;
        /// <summary>
        /// Explorer
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ListItemFileFolder _ListIteamRequest(string path, string id)
        {
            return EventListIteamRequest(path, id);
        }
        public delegate ListItemFileFolder ListIteamRequest(string path, string id);
        public event ListIteamRequest EventListIteamRequest;
        /// <summary>
        /// Send data login to core
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Pass"></param>
        /// <param name="AutoLogin"></param>
        /// <returns></returns>
        public bool _Login(string User, string Pass, bool AutoLogin)
        {
            return EventLogin(User, Pass, AutoLogin);
        }
        public delegate bool Login(string User, string Pass, bool AutoLogin);
        public event Login EventLogin;

        /// <summary>
        /// Move/Rename Disk/Dropbox : path_from,path_to |
        /// Move GoogleDrive: { path_from,path_to,id} or { Email,id,type,parent_id_from,parent_id_to} |
        /// Rename GoogleDrive: Email,id,type,newname
        /// </summary>
        /// <param name="path_from"></param>
        /// <param name="path_to"></param>
        /// <param name="id"></param>
        /// <param name="parent_id_from"></param>
        /// <param name="parent_id_to"></param>
        /// <param name="newname"></param>
        /// <param name="Email"></param>
        /// <param name="type"></param>
        /// <param name="Copy"></param>
        /// <returns></returns>
        public bool _MoveItem(string path_from, string path_to, string id, string parent_id_from, string parent_id_to, string newname, string Email, CloudName type = CloudName.Folder, bool Copy = false)
        {
            return EventMoveItem(path_from,path_to,id,parent_id_from,parent_id_to,newname,Email,type,Copy);
        }
        public delegate bool RenameItem(string path_from, string path_to, string id, string parent_id_from, string parent_id_to, string newname, string Email, CloudName type = CloudName.Folder, bool Copy = false);
        public event RenameItem EventMoveItem;
        /// <summary>
        /// Delete List items
        /// </summary>
        /// <param name="items">Class SupDataDll.DeleteItems</param>
        public void _DeletePath(object items)
        {
            EventDeletePath(items);
        }//object DeleteItems
        public delegate void DeletePath(object items);//object DeleteItems
        public event DeletePath EventDeletePath;
        /// <summary>
        /// Create Folder (if parentid != null then path will dismiss)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parentid"></param>
        /// <param name="Email"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string _CreateFolder(string path, string parentid = null, string Email = null, string name = null)
        {
            return EventCreateFolder(path, parentid, Email, name);
        }
        public delegate string CreateFolder(string path, string parentid = null, string Email = null, string name = null);
        public event CreateFolder EventCreateFolder;

        //exit
        private bool Exitting = false;
        /// <summary>
        /// Close App
        /// </summary>
        public void _ExitApp()
        {
            if (!Exitting)
            {
                EventExitAppCallBack.Invoke();
                Exitting = true;
            }
        }
        public delegate void ExitAppCallBack();
        public event ExitAppCallBack EventExitAppCallBack;
    }

}
