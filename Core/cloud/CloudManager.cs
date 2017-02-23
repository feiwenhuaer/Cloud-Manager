using SupDataDll;
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

namespace Core.Cloud
{
    public class CloudManager
    {
        const string Rg_url_idFolder = "(?<=\\/folders\\/)[A-Za-z0-9-_]+",//drive/folders/id_folder or drive.google.com/drive/u/0/folders/id
        Rg_url_idFolderOpen = "(?<=\\?id\\=)[A-Za-z0-9-_]+",
        Rg_url_idFile = "(?<=file\\/d\\/)[A-Za-z0-9-_]+";

        public ExplorerNode GetItemsList(ExplorerNode node)
        {
            switch (node.GetRoot().RootInfo.Type)
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
                    throw new UnknowCloudNameException("Error Unknow Cloud Type: " + node.GetRoot().RootInfo.Type.ToString());
            }
        }

        public Stream GetFileStream(ExplorerNode node, long Startpos = 0,long endpos =0,bool IsUpload = false)
        {
            switch (node.GetRoot().RootInfo.Type)
            {
                case CloudType.Dropbox:
                    return Dropbox.GetFileStream(node, Startpos, endpos);
                case CloudType.GoogleDrive:
                    return GoogleDrive.GetFileStream(node, Startpos, endpos);
                case CloudType.LocalDisk:
                    return LocalDisk.GetFileSteam(node, IsUpload, Startpos);
                default:
                    throw new UnknowCloudNameException("Error Unknow Cloud Type: " + node.GetRoot().RootInfo.Type.ToString());
            }
        }

        //public ListItemFileFolder GetListRecusive(string path, string id = null)
        //{
        //    ListItemFileFolder data = new ListItemFileFolder();
        //    data.path_raw = path.TrimEnd('/').TrimEnd('\\');
        //    AnalyzePath rp = new AnalyzePath(path);
        //    string Real_path = rp.GetPath();
        //    switch (rp.TypeCloud)
        //    {
        //        case CloudType.Dropbox:
        //            return Dropbox.ListFolderRecusive(Real_path, rp.Email);
        //        case CloudType.GoogleDrive:
        //            return GoogleDrive.GetListFolderRecusive(Real_path, id, rp.Email);
        //        default:
        //            throw new UnknowCloudNameException("Error Unknow Cloud Type: " + rp.TypeCloud.ToString());
        //    }
        //}

        public string CreateFolder(ExplorerNode node)
        {
            switch (node.GetRoot().RootInfo.Type)
            {
                case CloudType.Dropbox:
                    return Dropbox.CreateFolder(node);
                case CloudType.GoogleDrive:
                    return GoogleDrive.CreateFolder(node);
                case CloudType.LocalDisk:
                    return LocalDisk.CreateFolder(node);
                default:
                    throw new UnknowCloudNameException("Error Unknow Cloud Type: " + node.GetRoot().RootInfo.Type.ToString());
            }
        }

        #region Oauth
        public void Oauth(CloudType type)
        {
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
                    string[] scope = new string[] { Scope.Drive, Scope.DriveFile, Scope.DriveMetadata };
                    GoogleAPIOauth2 oauth_gd = new GoogleAPIOauth2(scope);
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
                        catch (Exception ex) { error = true; goto reoauthMega; }
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
            dynamic json = JsonConvert.DeserializeObject(client.GetCurrentAccount());
            string email = json.email;
            SaveToken(email, token, CloudType.Dropbox);
        }
        private void Oauth_gd_TokenCallBack(string token_)
        {
            if (!string.IsNullOrEmpty(token_))
            {
                TokenGoogleDrive token = JsonConvert.DeserializeObject<TokenGoogleDrive>(token_);
                if (token.IsError) throw new Exception("Accesstoken:" + token.access_token + ",RefreshToken:" + token.refresh_token);
                string token_text = JsonConvert.SerializeObject(token);
                DriveAPIHttprequestv2 client = new DriveAPIHttprequestv2(token);
                dynamic about = JsonConvert.DeserializeObject(client.About());
                string email = about.user.emailAddress;
                SaveToken(email, token_text, CloudType.GoogleDrive);
            }else throw new Exception("Oauth token GD failed.");
        }
        private void SaveToken(string email, string token, CloudType type)
        {
            if(AppSetting.settings.AddCloud(email, type, token, false)) AppSetting.UIMain.AddNewCloudToTV(email, type);
        }
        #endregion

        public bool MoveItem(ExplorerNode node, ExplorerNode newparent, string newname = null, bool Copy = false)
        {
            if (node.GetRoot() == newparent.GetRoot())
            {
                bool flag = false;
                switch (node.GetRoot().RootInfo.Type)
                {
                    case CloudType.Dropbox: flag = Dropbox.Move(node, newparent, newname); break;
                    case CloudType.GoogleDrive: GoogleDrive.MoveItem(node, newparent, newname).parents.ForEach(s => { if (!flag && s.id == newparent.Info.ID) flag = true; }); break;
                    case CloudType.LocalDisk: flag = LocalDisk.Move(node, newparent, newname); break;
                    case CloudType.Mega:
                    default: throw new Exception("CloudType not support (" + node.GetRoot().RootInfo.Type.ToString() + ").");
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

        public void Delete(object items_)
        {
            DeleteItems items = items_ as DeleteItems;
            Type type_deleteform = LoadDllUI.GetTypeInterface(typeof(SupDataDll.UiInheritance.UIDelete));
            SupDataDll.UiInheritance.UIDelete deleteform = (SupDataDll.UiInheritance.UIDelete)Activator.CreateInstance(type_deleteform);
            CancelDelete cd = new CancelDelete();
            deleteform.EventCancel += cd.Deleteform_EventCancelDelegate;
            deleteform.EventClosing += cd.Deleteform_EventCloseForm;
            Thread thr = new Thread(deleteform.ShowDialog_);
            thr.Start();
            Thread.Sleep(500);
            foreach (ExplorerNode item in items.Items)
            {
                while (cd.cancel) { Thread.Sleep(100); if (cd.closedform) return; }
                if (cd.closedform) return;
                bool Iserror = false;
                deleteform.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Deleting.ToString()) + item);
                try
                {
                    switch (item.GetRoot().RootInfo.Type)
                    {
                        case CloudType.Dropbox:
                            if (!Dropbox.Delete(item, items.PernamentDelete)) Iserror = true;
                            break;
                        case CloudType.GoogleDrive:
                            if (!GoogleDrive.File_trash(item, items.PernamentDelete)) Iserror = true;
                            break;
                        case CloudType.LocalDisk:
                            if (!LocalDisk.Delete(item, items.PernamentDelete)) Iserror = true;
                            break;
                        default: throw new UnknowCloudNameException("Error Unknow Cloud Type: " + item.GetRoot().RootInfo.Type.ToString());
                    }
                    if (!Iserror) deleteform.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Deleted.ToString()) + "\r\n");
                    else
                    {
                        deleteform.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Error.ToString()) + "\r\n");
                        if (deleteform.AutoClose)
                        {
                            deleteform.SetAutoClose(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    deleteform.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Error.ToString()) + "\r\nMessage:" + ex.Message + "\r\n");
                    if (deleteform.AutoClose)
                    {
                        deleteform.SetAutoClose(false);
                    }
                }
            }
            if (deleteform.AutoClose) deleteform.Close_();
            else
            {
                deleteform.SetTextButtonCancel(AppSetting.lang.GetText(LanguageKey.BT_close.ToString()));
                while (!deleteform.AutoClose)
                {
                    Thread.Sleep(100);
                    if (cd.cancel)
                    {
                        deleteform.Close_();
                    }
                }
            }
        }

        class CancelDelete
        {
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
    }
}
