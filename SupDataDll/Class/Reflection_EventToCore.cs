using CloudManagerGeneralLib.Class;
using System.Collections.Generic;

namespace CloudManagerGeneralLib
{
    public delegate bool ChangeUserPass(string user, string pass, string newpass);
    public delegate void TransferItems(List<ExplorerNode> items, ExplorerNode fromfolder, ExplorerNode savefolder, bool AreCut);
    public delegate void SaveSetting();
    public delegate string GetSetting(SettingsKey Key);
    public delegate string GetTextLanguage(string Key);
    public delegate bool DeleteAccountCloud(string Email, CloudType type);
    public delegate List<ExplorerNode> GetListAccountCloud();
    public delegate void ShowFormOauth(CloudType type);
    public delegate ExplorerNode GetChildNode(ExplorerNode node);
    public delegate bool LoginApp(string User, string Pass, bool AutoLogin);
    public delegate bool RenameItem(ExplorerNode node, ExplorerNode newparent, string newname = null, bool Copy = false);
    public delegate void DeletePath(DeleteItems items);
    public delegate void CreateFolder(ExplorerNode node);
    public delegate void ExitAppCallBack();
    public delegate void SetSetting(SettingsKey Key, string Data);
    public class Reflection_EventToCore
    {
        public string test()
        {
            return EventChangeUserPass.GetType().Name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="newpass"></param>
        /// <returns></returns>
        public bool ChangeUserPass(string user, string pass, string newpass)
        {
            return EventChangeUserPass(user, pass, newpass);
        }
        public event ChangeUserPass EventChangeUserPass;
        /// <summary>
        /// Add list item for download upload
        /// </summary>
        /// <param name="items">List Items</param>
        /// <param name="fromfolder">From</param>
        /// <param name="savefolder">To</param>
        /// <param name="AreCut">Cut or Copy (Cut will delete items in From folder)</param>
        public void TransferItems(List<ExplorerNode> items, ExplorerNode fromfolder, ExplorerNode savefolder, bool AreCut)
        {
            EventTransferItems(items, fromfolder, savefolder, AreCut);
        }
        public event TransferItems EventTransferItems;
        /// <summary>
        /// SetSetting
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="Data">Value to set</param>
        public void SetSetting(SettingsKey Key, string Data)
        {
            EventSetSetting(Key, Data);
        }
        public event SetSetting EventSetSetting;
        /// <summary>
        /// Save Setting
        /// </summary>
        public void SaveSetting()
        {
            EventSaveSetting();
        }
        public event SaveSetting EventSaveSetting;
        /// <summary>
        /// Read Setting
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string GetSetting(SettingsKey Key)
        {
            return EventGetSetting(Key);
        }
        public event GetSetting EventGetSetting;
        /// <summary>
        /// Read language
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string GetTextLanguage(string Key)
        {
            return EventGetTextLanguage(Key);
        }
        /// <summary>
        /// Read language
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string GetTextLanguage(LanguageKey Key)
        {
            return EventGetTextLanguage(Key.ToString());
        }
        public event GetTextLanguage EventGetTextLanguage;
        /// <summary>
        /// Delete Cloud Account
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool DeleteAccountCloud(string Email, CloudType type)
        {
            return EventDeleteAccountCloud(Email, type);
        }
        public event DeleteAccountCloud EventDeleteAccountCloud;
        /// <summary>
        /// Get List Cloud Account
        /// </summary>
        /// <returns></returns>
        public List<ExplorerNode> GetListAccountCloud()
        {
            return EventGetListAccountCloud();
        }
        public event GetListAccountCloud EventGetListAccountCloud;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void ShowFormOauth(CloudType type)
        {
            EventShowFormOauth(type);
        }
        public event ShowFormOauth EventShowFormOauth;
        /// <summary>
        /// Explorer, return parent.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ExplorerNode ListIteamRequest(ExplorerNode node)
        {
            return EventGetChildNode(node);
        }
        public event GetChildNode EventGetChildNode;
        /// <summary>
        /// Send data login to core
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Pass"></param>
        /// <param name="AutoLogin"></param>
        /// <returns></returns>
        public bool Login(string User, string Pass, bool AutoLogin)
        {
            return EventLogin(User, Pass, AutoLogin);
        }
        public event LoginApp EventLogin;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="newparent"></param>
        /// <param name="newname"></param>
        /// <param name="Copy"></param>
        /// <returns></returns>
        public bool RenameItem(ExplorerNode node,string newname)
        {
            return EventMoveItem(node, null, newname, false);
        }
        public event RenameItem EventMoveItem;
        /// <summary>
        /// Delete List items
        /// </summary>
        /// <param name="items">Class CloudManagerGeneralLib.DeleteItems</param>
        public void DeletePath(DeleteItems items)
        {
            EventDeletePath(items);
        }
        public event DeletePath EventDeletePath;
        /// <summary>
        /// Create folder node
        /// </summary>
        /// <returns></returns>
        public void CreateFolder(ExplorerNode node)
        {
            EventCreateFolder(node);
        }
        public event CreateFolder EventCreateFolder;

        //exit
        private bool Exitting = false;
        /// <summary>
        /// Close App
        /// </summary>
        public void ExitApp()
        {
            if (!Exitting)
            {
                EventExitAppCallBack.Invoke();
                Exitting = true;
            }
        }
        public event ExitAppCallBack EventExitAppCallBack;
    }

}
