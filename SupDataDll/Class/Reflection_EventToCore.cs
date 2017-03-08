using SupDataDll.Class;
using System.Collections.Generic;

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
        public void _AddItem(List<ExplorerNode> items, ExplorerNode fromfolder, ExplorerNode savefolder, bool AreCut)
        {
            EventAddItem(items, fromfolder, savefolder, AreCut);
        }
        public delegate void AddItem(List<ExplorerNode> items, ExplorerNode fromfolder, ExplorerNode savefolder, bool AreCut);
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
        public bool _DeleteAccountCloud(string Email, CloudType type)
        {
            return EventDeleteAccountCloud(Email, type);
        }
        public delegate bool DeleteAccountCloud(string Email, CloudType type);
        public event DeleteAccountCloud EventDeleteAccountCloud;
        /// <summary>
        /// Get List Cloud Account
        /// </summary>
        /// <returns></returns>
        public List<ExplorerNode> _GetListAccountCloud()
        {
            return EventGetListAccountCloud();
        }
        public delegate List<ExplorerNode> GetListAccountCloud();
        public event GetListAccountCloud EventGetListAccountCloud;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void _ShowFormOauth(CloudType type)
        {
            EventShowFormOauth(type);
        }
        public delegate void ShowFormOauth(CloudType type);
        public event ShowFormOauth EventShowFormOauth;
        /// <summary>
        /// Explorer, return parent.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ExplorerNode _ListIteamRequest(ExplorerNode node)
        {
            return EventGetChildNode(node);
        }
        public delegate ExplorerNode GetChildNode(ExplorerNode node);
        public event GetChildNode EventGetChildNode;
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
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="newparent"></param>
        /// <param name="newname"></param>
        /// <param name="Copy"></param>
        /// <returns></returns>
        public bool _RenameItem(ExplorerNode node,string newname)
        {
            return EventMoveItem(node, null, newname, false);
        }
        public delegate bool RenameItem(ExplorerNode node, ExplorerNode newparent, string newname = null, bool Copy = false);
        public event RenameItem EventMoveItem;
        /// <summary>
        /// Delete List items
        /// </summary>
        /// <param name="items">Class SupDataDll.DeleteItems</param>
        public void _DeletePath(DeleteItems items)
        {
            EventDeletePath(items);
        }
        public delegate void DeletePath(DeleteItems items);
        public event DeletePath EventDeletePath;
        /// <summary>
        /// Create folder node
        /// </summary>
        /// <returns></returns>
        public void _CreateFolder(ExplorerNode node)
        {
            EventCreateFolder(node);
        }
        public delegate void CreateFolder(ExplorerNode node);
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
