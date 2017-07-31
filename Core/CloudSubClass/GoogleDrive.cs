using Cloud.GoogleDrive;
using Core.StaticClass;
using Newtonsoft.Json;
using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using static Cloud.GoogleDrive.DriveAPIHttprequestv2;
using Cloud;

namespace Core.CloudSubClass
{
  internal static class GoogleDrive
  {
    const string Rg_url_idFolder = "(?<=\\/folders\\/)[A-Za-z0-9-_]+",//drive/folders/id_folder or drive.google.com/drive/u/0/folders/id
    Rg_url_idFolderOpen = "(?<=\\?id\\=)[A-Za-z0-9-_]+",
    Rg_url_idFile = "(?<=file\\/d\\/)[A-Za-z0-9-_]+";

    public static OrderByEnum[] en = { OrderByEnum.folder, OrderByEnum.title, OrderByEnum.createdDate };
    internal static List<string> GoogleAppsmimeTypeGoogleRemove = new List<string>() {GoogleAppsmimeType.audio, GoogleAppsmimeType.drawing, GoogleAppsmimeType.file,GoogleAppsmimeType.form,GoogleAppsmimeType.fusiontable,
            GoogleAppsmimeType.map,GoogleAppsmimeType.presentation,GoogleAppsmimeType.script,GoogleAppsmimeType.sites,GoogleAppsmimeType.unknown,GoogleAppsmimeType.video,GoogleAppsmimeType.photo,GoogleAppsmimeType.spreadsheet,GoogleAppsmimeType.document};

    internal static DriveAPIHttprequestv2 GetAPIv2(string Email, GD_LimitExceededDelegate LimitExceeded = null)
    {
      DriveAPIHttprequestv2 gdclient = new DriveAPIHttprequestv2(JsonConvert.DeserializeObject<TokenGoogleDrive>(AppSetting.settings.GetToken(Email, CloudType.GoogleDrive)));
      if (LimitExceeded != null) gdclient.LimitExceeded += LimitExceeded;
      if (string.IsNullOrEmpty(gdclient.Token.Email) || gdclient.Token.Email != Email) gdclient.Token.Email = Email;
      gdclient.TokenRenewEvent += Gdclient_TokenRenewEvent;
      return gdclient;
    }

    public static IItemNode GetListFileFolder(IItemNode node, bool folderonly = false, bool read_only = false)
    {
      bool uri = false;
      RootNode root = node.GetRoot;
      string Email = root.RootType.Email;
      string parent_ID = null;
      string url = null;
      Regex rg;
      Match match;
      if (string.IsNullOrEmpty(Email)) { Email = AppSetting.settings.GetDefaultCloud(CloudType.GoogleDrive); uri = true; }

      #region Get parent_ID
      if (uri)//folder url
      {
        if (root.RootType.uri != null)
        {
          url = root.RootType.uri.ToString();
          rg = new Regex(Rg_url_idFolder);
          match = rg.Match(url);
          if (match.Success) parent_ID = match.Value;
          else
          {
            rg = new Regex(Rg_url_idFolderOpen);
            match = rg.Match(url);
            if (match.Success) parent_ID = match.Value;
          }
        }
      }
      else//explorer node
      {
        parent_ID = "root";//root
        if (!string.IsNullOrEmpty(node.Info.ID)) parent_ID = node.Info.ID;//id root or id node
      }
      #endregion

      #region Get Child Node Data
      if (!string.IsNullOrEmpty(parent_ID))//if found id is folder
      {
        List<Drive2_File> list_ = Search("'" + parent_ID + "' in parents and trashed=false", Email);
        if (parent_ID == "root")//save root id
        {
          foreach (Drive2_File item in list_)
          {
            foreach (Drive2_Parent parent in item.parents)
            {
              if (parent.isRoot ?? false) { parent_ID = parent.id; break; }
            }
            if (parent_ID != "root") break;
          }
          node.Info.ID = parent_ID;
          AppSetting.settings.SetRootID(Email, CloudType.GoogleDrive, parent_ID);
        }
        node.RenewChilds(list_.Convert(node));
        return node;
      }
      else if (string.IsNullOrEmpty(url))//if id from url
      {
        rg = new Regex(Rg_url_idFile);
        match = rg.Match(url);
        if (match.Success)
        {
          RootNode n = new RootNode();
          n.Info.ID = match.Value;
          n.RootType.Email = Email;
          Drive2_File item = GoogleDrive.GetMetadataItem(n);
          n.Info.Size = item.fileSize ?? -1;
          n.Info.Name = item.title;
          n.Info.MimeType = item.mimeType;
          AppSetting.UIMain.FileSaveDialog(PCPath.Mycomputer, item.title, PCPath.FilterAllFiles, n);
          return null;
        }
      }
      #endregion

      throw new Exception("Can't Analyze Data Input.");
    }

    public static Stream GetFileStream(IItemNode node, long Startpos = -1, long endpos = -1)
    {
      DriveAPIHttprequestv2 gdclient = GetAPIv2(node.GetRoot.RootType.Email);
      return gdclient.Files.Get(node.Info.ID, Startpos, endpos);
    }

    public static List<Drive2_File> Search(string query, string Email, string pageToken = null)
    {
      DriveAPIHttprequestv2 gdclient = GetAPIv2(Email);
      Drive2_Files_list list = gdclient.Files.List(en, query, pageToken);
      if (!string.IsNullOrEmpty(list.nextPageToken)) list.items.AddRange(Search(query, Email, list.nextPageToken));
      return list.items;
    }

    static object sync_createfolder = new object();
    public static void CreateFolder(IItemNode node)
    {
      string Email = node.GetRoot.RootType.Email;
      DriveAPIHttprequestv2 gdclient = GetAPIv2(Email);
      string parent_id = "";
      try
      {
        List<IItemNode> listnode = node.GetFullPath();
        Monitor.Enter(sync_createfolder);
        int i;
        for (i = listnode.Count - 1; i > 0; i--)
        {
          if (!string.IsNullOrEmpty(listnode[i].Info.ID)) { parent_id = listnode[i].Info.ID; break; }
        }
        i++;
        bool create = false;
        for (; i < listnode.Count; i++)
        {
          if (!create)
          {
            List<IItemNode> listsearchnode = Search("'" + parent_id + "' in parents" +
                " and trashed=false" +
                " and title='" + listnode[i].Info.Name.Replace("'", "\\'") + "'" +
                " and mimeType = 'application/vnd.google-apps.folder'", Email).Convert(node);
            if (listsearchnode.Count == 0) create = true;
            else parent_id = listnode[i].Info.ID = listsearchnode[0].Info.ID;
          }

          if (create)
          {
            Drive2_File folder = gdclient.Extend.CreateFolder(listnode[i].Info.Name, parent_id);
            parent_id = listnode[i].Info.ID = folder.id;
          }
        }
      }
      finally { Monitor.Exit(sync_createfolder); }
    }

    public static bool ReNameItem(IItemNode node, string newname)
    {
      DriveAPIHttprequestv2 gdclient = GetAPIv2(node.GetRoot.RootType.Email);
      string json = "{\"title\": \"" + newname + "\"}";
      Drive2_File response = gdclient.Files.Patch(node.Info.ID, json);
      if (response.title == newname) return true;
      else return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodemove">Item move</param>
    /// <param name="newparent">if cut then parent, copy then item</param>
    /// <param name="newname">rename (cut)</param>
    /// <param name="copy"></param>
    /// <returns></returns>
    public static Drive2_File MoveItem(IItemNode nodemove, IItemNode newparent, string newname = null, bool copy = false)
    {
      if (newparent != null && nodemove.GetRoot.RootType.Type != newparent.GetRoot.RootType.Type) throw new Exception("TypeCloud not match.");
      DriveAPIHttprequestv2 gdclient;
      if (!copy)
      {
        //Same account
        gdclient = GetAPIv2(nodemove.GetRoot.RootType.Email);
        if (newparent != null && nodemove.GetRoot.RootType.Email != newparent.GetRoot.RootType.Email) throw new Exception("Email not match.");
        Drive2_File item = null;
        if (newparent == null & newname != null)//rename
        {
          item = new Drive2_File();
          item.title = newname;
        }
        else//move
        {
          Drive2_Parents_list parents = gdclient.Parent.List(nodemove.Info.ID);
          Drive2_Parent found = parents.items.Find(p => p.id == nodemove.Parent.Info.ID);
          if (found != null) parents.items.Remove(found);
          parents.items.Add(new Drive2_Parent() { id = newparent.Info.ID });
          gdclient.Parent.Insert(nodemove.Info.ID, parents.items);
        }
        return gdclient.Files.Patch(nodemove.Info.ID, item == null ? null : JsonConvert.SerializeObject(item));
      }
      else
      {
        CreateFolder(newparent.Parent);
        if(string.IsNullOrEmpty(newparent.Parent.Info.ID))
        {
          Console.WriteLine("error");
        }
        return GetAPIv2(newparent.GetRoot.RootType.Email).Files.Copy(nodemove.Info.ID, newparent.Parent.Info.ID);
      }
    }

    public static Drive2_File GetMetadataItem(IItemNode node)
    {
      DriveAPIHttprequestv2 client = GetAPIv2(node.GetRoot.RootType.Email);
      return client.Files.Patch(node.Info.ID, null);
    }
    //trash/delete
    public static bool File_trash(IItemNode node, bool Permanently)
    {
      DriveAPIHttprequestv2 gdclient = GetAPIv2(node.GetRoot.RootType.Email);
      if (node == node.GetRoot) throw new Exception("Can't delete root.");
      if (Permanently)
      {
        gdclient.Files.Delete(node.Info.ID);
        return true;
      }
      else
      {
        gdclient.Files.Trash(node.Info.ID);
        return true;
      }
    }


    public static void Gdclient_TokenRenewEvent(TokenGoogleDrive token)
    {
      XmlNode cloud = AppSetting.settings.GetCloud(token.Email, CloudType.GoogleDrive);
      if (cloud != null) AppSetting.settings.ChangeToken(cloud, JsonConvert.SerializeObject(token));
      else throw new Exception("Can't save token.");
    }

    public static List<IItemNode> Convert(this List<Drive2_File> items, IItemNode parent)
    {
      List<IItemNode> list = new List<IItemNode>();
      foreach (Drive2_File item in items)
      {
        bool add = true;
        GoogleAppsmimeTypeGoogleRemove.ForEach(m => { if (item.mimeType == m) add = false; });
        if (add) list.Add(
                            new ItemNode(
                                new NodeInfo()
                                {
                                  Name = item.title,
                                  MimeType = item.mimeType,
                                  ID = item.id,
                                  Size = item.fileSize ?? -1,
                                  DateMod = item.modifiedDate ?? DateTime.Now
                                },
                                parent)
                          );

      }
      return list;
    }
  }
  public class GD_PathNotFoundException : Exception
  {
    public string FoundPath { get; set; }
    public string IdPath { get; set; }
    public GD_PathNotFoundException(string message) : base(message)
    {

    }
  }

  public enum rolePermissions
  {
    owner, reader, writer
  }

  public enum typePermissions
  {
    user, group, domain, anyone
  }
}