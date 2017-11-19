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
    //const string Rg_url_idFolder = "(?<=\\/folders\\/)[A-Za-z0-9-_]+",//drive/folders/id_folder or drive.google.com/drive/u/0/folders/id
    //Rg_url_idFolderOpen = "(?<=\\?id\\=)[A-Za-z0-9-_]+",
    //Rg_url_idFile = "(?<=file\\/d\\/)[A-Za-z0-9-_]+";

    #region Check Thread
    const string require_main = "Require main thread for create and show UI.";
    const string require_child = "This is long task, use main thread will make UI not-responding.";
    static void CheckThread(bool isMain)
    {
      if ((Thread.CurrentThread == AppSetting.MainThread) != isMain) throw new Exception(isMain ? require_main : require_child);
    }
    #endregion

    //List item in folder
    public IItemNode GetItemsList(IItemNode node)
    {
      CheckThread(false);
      switch (node.GetRoot.RootType.Type)
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
          throw new UnknowCloudNameException("Error Unknow Cloud Type: " + node.GetRoot.RootType.Type.ToString());
      }
    }

    //download file
    public Stream GetFileStream(IItemNode node, long Startpos = -1, long endpos = -1, bool IsUpload = false, object DataEx = null)
    {
      switch (node.GetRoot.RootType.Type)
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
          throw new UnknowCloudNameException("Error Unknow Cloud Type: " + node.GetRoot.RootType.Type.ToString());
      }
    }

    //create folder
    public void CreateFolder(IItemNode node)
    {
      CheckThread(false);
      switch (node.GetRoot.RootType.Type)
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
          throw new UnknowCloudNameException("Error Unknow Cloud Type: " + node.GetRoot.RootType.Type.ToString());
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
          AppSetting.UIOauth = (IOauth)Activator.CreateInstance(type_oauthUI);
          oauth_dropbox.GetCode(AppSetting.UIOauth, AppSetting.UIMain);
          break;

        case CloudType.GoogleDrive:
          GoogleAPIOauth2 oauth_gd = new GoogleAPIOauth2();
          oauth_gd.TokenCallBack += Oauth_gd_TokenCallBack;
          type_oauthUI = LoadDllUI.GetTypeInterface(typeof(UIinterfaceGD));
          AppSetting.UIOauth = (IOauth)Activator.CreateInstance(type_oauthUI);
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
        Drive2_About about = client.About.Get();
        SaveToken(about.user.emailAddress, token_text, CloudType.GoogleDrive);
      }
      else throw new Exception("Oauth token GD failed.");
    }
    private void SaveToken(string email, string token, CloudType type)
    {
      if (AppSetting.settings.AddCloud(email, type, token, false)) AppSetting.UIMain.AddNewCloudToTV(new RootNode(new TypeNode() { Email = email, Type = type }));
    }
    #endregion

    //Rename: MoveItem(node,null,newname,false)
    //Cut: MoveItem(node,newparent,null,false)
    //Copy: MoveItem(node,newparent,null,true)
    //
    public bool MoveItem(IItemNode node, IItemNode newparent = null, string newname = null, bool Copy = false)
    {
      CheckThread(false);
      if ((newparent != null && node.GetRoot == newparent.GetRoot) | newparent == null)
      {
        bool flag = false;
        switch (node.GetRoot.RootType.Type)
        {
          case CloudType.Dropbox: flag = Dropbox.Move(node, newparent, newname); break;
          case CloudType.GoogleDrive:
            Drive2_File item = GoogleDrive.MoveItem(node, newparent, newname);
            if (item.title == newname) flag = true;
            item.parents.ForEach(s => { if (!flag && newparent != null && s.id == newparent.Info.ID) flag = true; });
            break;
          case CloudType.LocalDisk: flag = LocalDisk.Move(node, newparent, newname); break;
          case CloudType.Mega:
          default: throw new Exception("CloudType not support (" + node.GetRoot.RootType.Type.ToString() + ").");
        }
        if (flag)
        {
          if (newparent != null)
          {
            node.Parent.RemoveChild(node);
            newparent.AddChild(node);
          }
          else node.Info.Name = newname;
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
      DeleteTask dw = new DeleteTask(items.Items, AppSetting.UIMain.CreateUI<CloudManagerGeneralLib.UiInheritance.UIDelete>(type_deleteform), items.PernamentDelete);
      dw.Start();
    }
    
    public IItemNode GetFileInfo(IItemNode node)
    {
      switch (node.GetRoot.RootType.Type)
      {
        case CloudType.Dropbox:
          return Dropbox.GetMetaData(node);
        case CloudType.GoogleDrive:
          Drive2_File item = GoogleDrive.GetMetadataItem(node);
          node.Info.Size = item.fileSize ?? -1;
          node.Info.Name = item.title;
          node.Info.DateMod = item.modifiedDate ?? DateTime.Now;
          return node;
        case CloudType.LocalDisk:
          return LocalDisk.GetFileInfo(node);
        case CloudType.Mega:
          return MegaNz.GetItem(node);
        default:
          throw new UnknowCloudNameException("Error Unknow Cloud Type: " + node.GetRoot.RootType.Type.ToString());
      }
    }
  }
}