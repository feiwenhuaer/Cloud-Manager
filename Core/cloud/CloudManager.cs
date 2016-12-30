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

namespace Core.cloud
{
    public class CloudManager
    {
        public ListItemFileFolder GetItemsList(string path, string id)
        {
            ListItemFileFolder data = new ListItemFileFolder();
            data.path_raw = path.TrimEnd('/').TrimEnd('\\');
            AnalyzePath rp = new AnalyzePath(path);
            switch (rp.TypeCloud)
            {
                case CloudName.Dropbox:
                    return Dropbox.GetListFileFolder(rp.GetPath(), rp.Email);
                case CloudName.GoogleDrive:
                    #region GD
                    switch (rp.TypePath)
                    {
                        case TypePath.Cloud:
                        case TypePath.CloudID:
                            return GoogleDrive.GetListFileFolder(rp.GetPath(), rp.Email, id);
                        case TypePath.UrlFolder:
                            string email = AppSetting.settings.GetDefaultCloud(CloudName.GoogleDrive);
                            dynamic j = JsonConvert.DeserializeObject(GoogleDrive.GetMetadataItem(email, id));
                            ListItemFileFolder fd = GoogleDrive.GetListFileFolder(null, email, id, false, true);
                            fd.NameFolder = j.title;
                            return fd;
                        case TypePath.CloudQuery:
                            var a = GoogleDrive.Search(rp.Query, AppSetting.settings.GetDefaultCloud(CloudName.GoogleDrive), null, true).Convert();
                            a.path_raw = path;
                            a.NameFolder = "Search";
                            return a;
                        case TypePath.UrlFile:
                            GD_item item = JsonConvert.DeserializeObject<GD_item>(GoogleDrive.GetMetadataItem(AppSetting.settings.GetDefaultCloud(rp.TypeCloud), rp.ID));
                            AppSetting.UIMain.FileSaveDialog("::{20D04FE0-3AEA-1069-A2D8-08002B30309D}", item.title, "All files (*.*)|*.*", rp, item.title, item.fileSize);
                            return null;
                        default: throw new Exception("Not support now");
                    }
                #endregion
                case CloudName.LocalDisk:
                    return LocalDisk.GetListFileFolder(rp.GetPath());
                default:
                    throw new UnknowCloudNameException("Error Unknow Cloud Type: " + rp.TypeCloud.ToString());
            }
        }

        public Stream GetFileStream(string path, string id, bool IsUpload = false,long Startpos = 0,long endpos =0,CloudName type = CloudName.LocalDisk,string email = null)
        {
            AnalyzePath rp;
            if (!string.IsNullOrEmpty(path))
            {
                rp = new AnalyzePath(path);
                switch (rp.TypeCloud)
                {
                    case CloudName.Dropbox:
                        return Dropbox.GetFileStream(rp.GetPath(), rp.Email, Startpos, endpos);
                    case CloudName.GoogleDrive:
                        return GoogleDrive.GetFileStream(id, rp.Email, Startpos, endpos);
                    case CloudName.LocalDisk:
                        return LocalDisk.GetFileSteam(path, IsUpload, Startpos);
                    default:
                        throw new UnknowCloudNameException("Error Unknow Cloud Type: " + rp.TypeCloud.ToString());
                }
            }
            else
            {
                switch(type)
                {
                    case CloudName.GoogleDrive:
                        return GoogleDrive.GetFileStream(id, email, Startpos, endpos);
                    default:
                        throw new UnknowCloudNameException("Error Unknow Cloud Type: " + type.ToString());
                }
            }
        }

        public ListItemFileFolder GetListRecusive(string path, string id = null)
        {
            ListItemFileFolder data = new ListItemFileFolder();
            data.path_raw = path.TrimEnd('/').TrimEnd('\\');
            AnalyzePath rp = new AnalyzePath(path);
            string Real_path = rp.GetPath();
            switch (rp.TypeCloud)
            {
                case CloudName.Dropbox:
                    return Dropbox.ListFolder(Real_path, rp.Email);
                case CloudName.GoogleDrive:
                    return GoogleDrive.GetListFolderRecusive(Real_path, id, rp.Email);
                default:
                    throw new UnknowCloudNameException("Error Unknow Cloud Type: " + rp.TypeCloud.ToString());
            }
        }

        public string CreateFolder(string path,string parentid= null,string Email = null, string name = null)
        {
            AnalyzePath rp = new AnalyzePath(path);
            string Real_path = rp.GetPath();
            switch (rp.TypeCloud)
            {
                case CloudName.Dropbox:
                    return Dropbox.CreateFolder(Real_path, rp.Email);
                case CloudName.GoogleDrive:
                    return GoogleDrive.CreateFolder(path,parentid,Email,name);
                case CloudName.LocalDisk:
                    return LocalDisk.CreateFolder(path);
                default:
                    throw new UnknowCloudNameException("Error Unknow Cloud Type: " + rp.TypeCloud.ToString());
            }
        }

        #region Oauth
        public void Oauth(CloudName type)
        {
            Type type_oauthUI;
            OauthUI instanceUI;
            switch (type)
            {
                case CloudName.Dropbox:
                    DropboxOauthv2 oauth_dropbox = new DropboxOauthv2();
                    oauth_dropbox.TokenCallBack += Oauth_dropbox_TokenCallBack;

                    type_oauthUI = LoadDllUI.GetTypeInterface(typeof(UIinterfaceDB));
                    instanceUI = (OauthUI)Activator.CreateInstance(type_oauthUI);

                    oauth_dropbox.GetCode(instanceUI, AppSetting.UIMain);
                    break;
                case CloudName.GoogleDrive:
                    string[] scope = new string[] { Scope.Drive, Scope.DriveFile, Scope.DriveMetadata };
                    GoogleAPIOauth2 oauth_gd = new GoogleAPIOauth2(scope);
                    oauth_gd.TokenCallBack += Oauth_gd_TokenCallBack;

                    type_oauthUI = LoadDllUI.GetTypeInterface(typeof(UIinterfaceGD));
                    instanceUI = (OauthUI)Activator.CreateInstance(type_oauthUI);

                    oauth_gd.GetCode(instanceUI, AppSetting.UIMain);
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
            SaveToken(email, token, CloudName.Dropbox);
        }
        private void Oauth_gd_TokenCallBack(TokenGoogleDrive token)
        {
            if (token.IsError) throw new Exception("Accesstoken:" + token.access_token + ",RefreshToken:" + token.refresh_token);
            string token_text = JsonConvert.SerializeObject(token);
            DriveAPIHttprequestv2 client = new DriveAPIHttprequestv2(token);
            dynamic about = JsonConvert.DeserializeObject(client.About());
            string email = about.user.emailAddress;
            SaveToken(email, token_text, CloudName.GoogleDrive);
        }
        private void SaveToken(string email, string token, CloudName type)
        {
            if(AppSetting.settings.AddCloud(email, type, token, false)) AppSetting.UIMain.AddNewCloudToTV(email, type);
        }
        #endregion

        public bool RenameItem(string path,string newname, string id = null)
        {
            if (string.IsNullOrEmpty(path) && id == null) throw new ArgumentNullException("path");
            try
            {
                AnalyzePath rp = new AnalyzePath(path);
                string Real_path = rp.GetPath();
                switch (rp.TypeCloud)
                {
                    case CloudName.Dropbox:
                        return Dropbox.Rename(path, newname);
                    case CloudName.GoogleDrive:
                        return GoogleDrive.ReNameItem(newname, id, rp.Email);
                    case CloudName.LocalDisk:
                        return LocalDisk.Rename(rp.Parent, rp.NameLastItem, newname);
                    default:
                        throw new UnknowCloudNameException("Error Unknow Cloud Type: " + rp.TypeCloud.ToString());
                }
            }
            catch{ return false; }
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
            foreach (string item in items.items)
            {
                while (cd.cancel) { Thread.Sleep(100); if (cd.closedform) return; }
                if (cd.closedform) return;
                bool Iserror = false;
                deleteform.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Deleting.ToString()) + item);
                try {
                    AnalyzePath rp = new AnalyzePath(item);
                    string Real_path = "";
                    if (rp.PathIsCloud) Real_path = rp.GetPath();
                    switch (rp.TypeCloud)
                    {
                        case CloudName.Dropbox:
                            if (!Dropbox.Delete(Real_path, AppSetting.settings.GetToken(rp.Email, rp.TypeCloud), items.PernamentDelete)) Iserror = true;
                            break;
                        case CloudName.GoogleDrive:
                            if (!GoogleDrive.File_trash(Real_path, "", AppSetting.settings.GetToken(rp.Email, rp.TypeCloud), rp.Email, items.PernamentDelete)) Iserror = true;
                            break;
                        case CloudName.LocalDisk:
                            if (!LocalDisk.Delete(item, items.PernamentDelete)) Iserror = true;
                            break;
                        default: throw new UnknowCloudNameException("Error Unknow Cloud Type: " + rp.TypeCloud.ToString());
                    }
                    if (!Iserror) deleteform.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Deleted.ToString())+ "\r\n");
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
                    if(cd.cancel)
                    {
                        deleteform.Close_();
                    }
                }
            }
        }

    }
    public class CancelDelete
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
