using CloudManagerGeneralLib;
using System.IO;
using Newtonsoft.Json;
using System;
using Core.StaticClass;
using System.Threading;
using Cloud;
using Cloud.Dropbox.Oauth;
using Cloud.GoogleDrive;
using Cloud.GoogleDrive.Oauth;
using Cloud.Dropbox;
using Cloud.MegaNz.Oauth;
using Cloud.MegaNz;
using System.Text.RegularExpressions;
using CloudManagerGeneralLib.Class;
using System.Collections.Generic;

namespace Core.CloudSubClass
{
    public class CloudManager
    {
        const string Rg_url_idFolder = "(?<=\\/folders\\/)[A-Za-z0-9-_]+",//drive/folders/id_folder or drive.google.com/drive/u/0/folders/id
        Rg_url_idFolderOpen = "(?<=\\?id\\=)[A-Za-z0-9-_]+",
        Rg_url_idFile = "(?<=file\\/d\\/)[A-Za-z0-9-_]+";

        const string require_main = "Require main thread for create and show UI.";
        const string require_child = "This is long task, use main thread will make UI not responding.";
        public static void CheckThread(bool isMain)
        {
            if ((Thread.CurrentThread == AppSetting.MainThread) != isMain) throw new Exception(isMain ? require_main : require_child);
        }


        public ItemNode GetItemsList(ItemNode node)
        {
            CheckThread(false);
            switch (node.GetRoot.NodeType.Type)
            {
                case CloudType.Dropbox:
                    return Dropbox.GetListFileFolder(node);
                case CloudType.GoogleDrive:
                    return GoogleDrive.GetListFileFolder(node);
                case CloudType.LocalDisk:
                    return LocalDisk.GetListFileFolder(node);
                case CloudType.Mega:
                    return MegaNz.GetListFileFolder(node);
                default:
                    throw new UnknowCloudNameException("Error Unknow Cloud Type: " + node.GetRoot.NodeType.Type.ToString());
            }
        }

        public Stream GetFileStream(ItemNode node, long Startpos = -1,long endpos =-1,bool IsUpload = false,object DataEx = null)
        {
            switch (node.GetRoot.NodeType.Type)
            {
                case CloudType.Dropbox:
                    return Dropbox.GetFileStream(node, Startpos, endpos);//download only
                case CloudType.GoogleDrive:
                    return GoogleDrive.GetFileStream(node, Startpos, endpos);//download only
                case CloudType.LocalDisk:
                    return LocalDisk.GetFileSteam(node, IsUpload, Startpos);//upload/download
                case CloudType.Mega:
                    return MegaNz.GetStream(node, Startpos, endpos, IsUpload, DataEx);//
                default:
                    throw new UnknowCloudNameException("Error Unknow Cloud Type: " + node.GetRoot.NodeType.Type.ToString());
            }
        }

        public void CreateFolder(ItemNode node)
        {
            CheckThread(false);
            switch (node.GetRoot.NodeType.Type)
            {
                case CloudType.Dropbox:
                    Dropbox.CreateFolder(node);
                    break;
                case CloudType.GoogleDrive:
                    GoogleDrive.CreateFolder(node);
                    break;
                case CloudType.LocalDisk:
                    LocalDisk.CreateFolder(node);
                    break;
                case CloudType.Mega:
                    MegaNz.CreateFolder(node);
                    break;
                default:
                    throw new UnknowCloudNameException("Error Unknow Cloud Type: " + node.GetRoot.NodeType.Type.ToString());
            }
        }

        #region Oauth
        public void Oauth(CloudType type)
        {
            CheckThread(true);
            Type type_oauthUI;
            switch (type)
            {
                case CloudType.Dropbox:
                    DropboxOauthv2 oauth_dropbox = new DropboxOauthv2();
                    oauth_dropbox.TokenCallBack += Oauth_dropbox_TokenCallBack;

                    type_oauthUI = LoadDllUI.GetTypeInterface(typeof(UIinterfaceDB));
                    AppSetting.UIOauth = (OauthUI)Activator.CreateInstance(type_oauthUI);

                    oauth_dropbox.GetCode(AppSetting.UIOauth, AppSetting.UIMain);
                    break;


                case CloudType.GoogleDrive:
                    GoogleAPIOauth2 oauth_gd = new GoogleAPIOauth2();
                    oauth_gd.TokenCallBack += Oauth_gd_TokenCallBack;

                    type_oauthUI = LoadDllUI.GetTypeInterface(typeof(UIinterfaceGD));
                    AppSetting.UIOauth = (OauthUI)Activator.CreateInstance(type_oauthUI);

                    oauth_gd.GetCode(AppSetting.UIOauth, AppSetting.UIMain);
                    break;


                case CloudType.Mega:
                    type_oauthUI = LoadDllUI.GetTypeInterface(typeof(UIinterfaceMegaNz));
                    UIinterfaceMegaNz mega = (UIinterfaceMegaNz)Activator.CreateInstance(type_oauthUI);
                    bool error = false;
                    reoauthMega:
                    if (!error) mega.ShowDialog_();
                    else mega.ShowError("Wrong email or password.");
                    if (mega.Success)
                    {
                        MegaApiClient.AuthInfos oauthinfo = MegaApiClient.GenerateAuthInfos(mega.Email, mega.Pass);
                        MegaApiClient client = new MegaApiClient();
                        try
                        {
                            client.Login(oauthinfo);
                        }
                        catch (Exception) { error = true; goto reoauthMega; }
                        SaveToken(mega.Email, JsonConvert.SerializeObject(oauthinfo), CloudType.Mega);
                    }
                    break;


                default: throw new Exception("Not support");
            }
        }
        private void Oauth_dropbox_TokenCallBack(string token)
        {
            Console.WriteLine("dropbox token call back");
            DropboxRequestAPIv2 client = new DropboxRequestAPIv2(token);
            IDropbox_Response_GetCurrentAccount account = client.GetCurrentAccount();
            SaveToken(account.email, token, CloudType.Dropbox);
        }
        private void Oauth_gd_TokenCallBack(string token_)
        {
            if (!string.IsNullOrEmpty(token_))
            {
                TokenGoogleDrive token = JsonConvert.DeserializeObject<TokenGoogleDrive>(token_);
                if (token.IsError) throw new Exception("Accesstoken:" + token.access_token + ",RefreshToken:" + token.refresh_token);
                string token_text = JsonConvert.SerializeObject(token);
                DriveAPIHttprequestv2 client = new DriveAPIHttprequestv2(token);
                dynamic about = JsonConvert.DeserializeObject(client.About.Get());
                string email = about.user.emailAddress;
                SaveToken(email, token_text, CloudType.GoogleDrive);
            }else throw new Exception("Oauth token GD failed.");
        }
        private void SaveToken(string email, string token, CloudType type)
        {
            if (AppSetting.settings.AddCloud(email, type, token, false)) AppSetting.UIMain.AddNewCloudToTV(new ItemNode(new TypeNode() { Email = email, Type = type }));
        }
        #endregion
        
        public bool MoveItem(ItemNode node, ItemNode newparent, string newname = null, bool Copy = false)
        {
            CheckThread(false);
            if (node.GetRoot == newparent.GetRoot)
            {
                bool flag = false;
                switch (node.GetRoot.NodeType.Type)
                {
                    case CloudType.Dropbox: flag = Dropbox.Move(node, newparent, newname); break;
                    case CloudType.GoogleDrive: GoogleDrive.MoveItem(node, newparent, newname).parents.ForEach(s => { if (!flag && s.id == newparent.Info.ID) flag = true; }); break;
                    case CloudType.LocalDisk: flag = LocalDisk.Move(node, newparent, newname); break;
                    case CloudType.Mega:
                    default: throw new Exception("CloudType not support (" + node.GetRoot.NodeType.Type.ToString() + ").");
                }
                if (flag)
                {
                    node.Parent.RemoveChild(node);
                    newparent.AddChild(node);
                }
                return flag;
            }
            else throw new Exception("Root not match.");
        }

        //long task (need thread)
        public void Delete(DeleteItems items)
        {
            CheckThread(true);
            Type type_deleteform = LoadDllUI.GetTypeInterface(typeof(CloudManagerGeneralLib.UiInheritance.UIDelete));
            DeleteWork dw = new DeleteWork(items.Items, AppSetting.UIMain.CreateUI<CloudManagerGeneralLib.UiInheritance.UIDelete>(type_deleteform), items.PernamentDelete);
            dw.Start();
        }

        public class DeleteWork
        {
            List<ItemNode> items;
            CloudManagerGeneralLib.UiInheritance.UIDelete ui;
            bool PernamentDelete = false;
            public DeleteWork(List<ItemNode> items, CloudManagerGeneralLib.UiInheritance.UIDelete ui, bool PernamentDelete = false)
            {
                if (ui == null) throw new Exception("UI is null.");
                if (items == null || items.Count == 0) throw new Exception("Need >= 1 item.");
                this.items = items;
                this.ui = ui;
                this.PernamentDelete = PernamentDelete;
                this.ui.EventCancel += Deleteform_EventCancelDelegate;
                this.ui.EventClosing += Deleteform_EventCloseForm;
            }

            public void Start()
            {
                Thread thr = new Thread(work);
                ui.Show_();
                thr.Start();
            }

            void work()
            {
                Thread.Sleep(500);
                foreach (ItemNode item in items)
                {
                    while (cancel) { Thread.Sleep(100); if (closedform) return; }
                    if (closedform) return;
                    bool Iserror = false;
                    ui.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Deleting.ToString()) + item);
                    try
                    {
                        switch (item.GetRoot.NodeType.Type)
                        {
                            case CloudType.Dropbox:
                                if (!Dropbox.Delete(item, PernamentDelete)) Iserror = true;
                                break;
                            case CloudType.GoogleDrive:
                                if (!GoogleDrive.File_trash(item, PernamentDelete)) Iserror = true;
                                break;
                            case CloudType.LocalDisk:
                                if (!LocalDisk.Delete(item, PernamentDelete)) Iserror = true;
                                break;
                            case CloudType.Mega:
                            default: throw new UnknowCloudNameException("Error Unknow Cloud Type: " + item.GetRoot.NodeType.Type.ToString());
                        }
                        if (!Iserror) ui.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Deleted.ToString()) + "\r\n");
                        else
                        {
                            ui.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Error.ToString()) + "\r\n");
                            if (ui.AutoClose)
                            {
                                ui.SetAutoClose(false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ui.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Error.ToString()) + "\r\nMessage:" + ex.Message + "\r\n");
                        if (ui.AutoClose) ui.SetAutoClose(false);
                    }
                }
                if (ui.AutoClose) ui.Close_();
                else
                {
                    ui.SetTextButtonCancel(AppSetting.lang.GetText(LanguageKey.BT_close.ToString()));
                    while (!ui.AutoClose)
                    {
                        Thread.Sleep(100);
                        if (cancel) ui.Close_();
                    }
                }
            }
            
            public bool cancel = false;
            public bool closedform = false;
            public void Deleteform_EventCancelDelegate()
            {
                cancel = !cancel;
            }
            public void Deleteform_EventCloseForm()
            {
                closedform = !closedform;
            }
        }


        public ItemNode GetFileInfo(ItemNode node)
        {
            switch (node.GetRoot.NodeType.Type)
            {
                case CloudType.Dropbox:
                    return Dropbox.GetMetaData(node);
                case CloudType.GoogleDrive:
                    GD_item item = GoogleDrive.GetMetadataItem(node);
                    node.Info.Size = item.fileSize;
                    node.Info.Name = item.title;
                    node.Info.DateMod = DateTime.Parse(item.modifiedDate);
                    return node;
                case CloudType.LocalDisk:
                    return LocalDisk.GetFileInfo(node);
                case CloudType.Mega:
                    return MegaNz.GetItem(node);
                default:
                    throw new UnknowCloudNameException("Error Unknow Cloud Type: " + node.GetRoot.NodeType.Type.ToString());
            }
        }
    }
}
