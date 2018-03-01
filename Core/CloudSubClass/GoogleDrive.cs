using Core.StaticClass;
using Newtonsoft.Json;
using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using TqkLibs.CloudStorage.GoogleDrive;

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

    static List<TokenGoogleDrive> Tokens = new List<TokenGoogleDrive>();
    internal static TokenGoogleDrive GetToken(string Email)
    {
      foreach (TokenGoogleDrive token in Tokens) if (token.Email == Email) return token;
      TokenGoogleDrive tk = JsonConvert.DeserializeObject<TokenGoogleDrive>(AppSetting.settings.GetToken(Email, CloudType.GoogleDrive));
      Tokens.Add(tk);
      tk.EventTokenUpdate += Tk_EventTokenUpdate;
      return tk;
    }

    private static void Tk_EventTokenUpdate(TokenGoogleDrive token)
    {
      XmlNode cloud = AppSetting.settings.GetCloud(token.Email, CloudType.GoogleDrive);
      if (cloud != null) AppSetting.settings.ChangeToken(cloud, JsonConvert.SerializeObject(token));
      else throw new Exception("Can't save token.");
    }

    internal static void DeleteToken(string Email)
    {
      for (int i = 0; i < Tokens.Count; i++) if (Tokens[i].Email == Email) { Tokens.RemoveAt(i); return; }      
    }

    internal static DriveAPIv2 GetAPIv2(string Email, GD_LimitExceededDelegate LimitExceeded = null)
    {
      DriveAPIv2 gdclient = new DriveAPIv2(GetToken(Email));
      if (LimitExceeded != null) gdclient.LimitExceeded += LimitExceeded;
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
        List<Drivev2_File> list_ = Search("'" + parent_ID + "' in parents and trashed=false", Email);
        if (parent_ID == "root")//save root id
        {
          foreach (Drivev2_File item in list_)
          {
            foreach (Drivev2_Parent parent in item.parents)
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
          Drivev2_File item = GoogleDrive.GetMetadataItem(n);
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
      DriveAPIv2 gdclient = GetAPIv2(node.GetRoot.RootType.Email);
      return gdclient.Files.Get(node.Info.ID, Startpos, endpos);
    }

    public static List<Drivev2_File> Search(string query, string Email, string pageToken = null)
    {
      DriveAPIv2 gdclient = GetAPIv2(Email);
      Drivev2_Files_list list = gdclient.Files.List(en, query, pageToken);
      if (!string.IsNullOrEmpty(list.nextPageToken)) list.items.AddRange(Search(query, Email, list.nextPageToken));
      return list.items;
    }

    static object sync_createfolder = new object();
    public static void CreateFolder(IItemNode node)
    {
      string Email = node.GetRoot.RootType.Email;
      DriveAPIv2 gdclient = GetAPIv2(Email);
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
            Drivev2_File metadata = new Drivev2_File() { title = listnode[i].Info.Name };
            metadata.parents = new List<Drivev2_Parent>() { new Drivev2_Parent() { id = parent_id } };

            Drivev2_File folder = gdclient.Extend.CreateFolder(metadata);
            parent_id = listnode[i].Info.ID = folder.id;
          }
        }
      }
      finally { Monitor.Exit(sync_createfolder); }
    }

    public static bool ReNameItem(IItemNode node, string newname)
    {
      DriveAPIv2 gdclient = GetAPIv2(node.GetRoot.RootType.Email);
      Drivev2_File response = gdclient.Files.Patch(node.Info.ID, new Drivev2_File() { title = newname});
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
    public static Drivev2_File MoveItem(IItemNode nodemove, IItemNode newparent, string newname = null, bool copy = false)
    {
      if (newparent != null && nodemove.GetRoot.RootType.Type != newparent.GetRoot.RootType.Type) throw new Exception("TypeCloud not match.");
      DriveAPIv2 gdclient;
      if (!copy)
      {
        //Same account
        gdclient = GetAPIv2(nodemove.GetRoot.RootType.Email);
        if (newparent != null && nodemove.GetRoot.RootType.Email != newparent.GetRoot.RootType.Email) throw new Exception("Email not match.");
        Drivev2_File item = null;
        if (newparent == null & newname != null)//rename
        {
          item = new Drivev2_File();
          item.title = newname;
        }
        else//move
        {
          Drivev2_Parents_list parents = gdclient.Parent.List(nodemove.Info.ID);
          Drivev2_Parent found = parents.items.Find(p => p.id == nodemove.Parent.Info.ID);
          if (found != null) parents.items.Remove(found);
          parents.items.Add(new Drivev2_Parent() { id = newparent.Info.ID });
          gdclient.Parent.Insert(nodemove.Info.ID, parents.items);
        }
        return gdclient.Files.Patch(nodemove.Info.ID, item);
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

    public static Drivev2_File GetMetadataItem(IItemNode node)
    {
      DriveAPIv2 client = GetAPIv2(node.GetRoot.RootType.Email);
      return client.Files.Patch(node.Info.ID, null);
    }
    //trash/delete
    public static bool File_trash(IItemNode node, bool Permanently)
    {
      DriveAPIv2 gdclient = GetAPIv2(node.GetRoot.RootType.Email);
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

    public static List<IItemNode> Convert(this List<Drivev2_File> items, IItemNode parent)
    {
      List<IItemNode> list = new List<IItemNode>();
      foreach (Drivev2_File item in items)
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