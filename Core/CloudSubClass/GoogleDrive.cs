﻿using Cloud.GoogleDrive;
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

namespace Core.CloudSubClass
{
    internal static class GoogleDrive
    {
        const string Rg_url_idFolder = "(?<=\\/folders\\/)[A-Za-z0-9-_]+",//drive/folders/id_folder or drive.google.com/drive/u/0/folders/id
        Rg_url_idFolderOpen = "(?<=\\?id\\=)[A-Za-z0-9-_]+",
        Rg_url_idFile = "(?<=file\\/d\\/)[A-Za-z0-9-_]+";
        
        static OrderByEnum[] en = { OrderByEnum.folder, OrderByEnum.title, OrderByEnum.createdDate };
        internal static List<string> mimeTypeGoogleRemove = new List<string>() {mimeType.audio, mimeType.drawing, mimeType.file,mimeType.form,mimeType.fusiontable,
            mimeType.map,mimeType.presentation,mimeType.script,mimeType.sites,mimeType.unknown,mimeType.video,mimeType.photo,mimeType.spreadsheet,mimeType.document};

        internal static DriveAPIHttprequestv2 GetAPIv2(string Email, GD_LimitExceededDelegate LimitExceeded = null)
        {
            DriveAPIHttprequestv2 gdclient = new DriveAPIHttprequestv2(JsonConvert.DeserializeObject<TokenGoogleDrive>(AppSetting.settings.GetToken(Email, CloudType.GoogleDrive)), LimitExceeded);
            if(string.IsNullOrEmpty(gdclient.Token.Email) || gdclient.Token.Email != Email) gdclient.Token.Email = Email;
            gdclient.TokenRenewEvent += Gdclient_TokenRenewEvent;
            return gdclient;
        }

        public static ItemNode GetListFileFolder(ItemNode node, bool folderonly = false,bool read_only = false)
        {
            bool uri = false;
            ItemNode root = node.GetRoot;
            string Email = root.NodeType.Email;
            string parent_ID = null;
            string url = null;
            Regex rg;
            Match match;
            if (string.IsNullOrEmpty(Email)) { Email = AppSetting.settings.GetDefaultCloud(CloudType.GoogleDrive); uri = true; }

            #region Get parent_ID
            if (uri)//folder url
            {
                if (root.NodeType.uri != null)
                {
                    url = root.NodeType.uri.ToString();
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
                GD_Files_list list_ = Search("'" + parent_ID + "' in parents and trashed=false", Email);
                if (parent_ID == "root")//save root id
                {
                    foreach(GD_item item in list_.items)
                    {
                        foreach(GD_parent parent in item.parents)
                        {
                            if (parent.isRoot) { parent_ID = parent.id; break; }
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
                if(match.Success)
                {
                    ItemNode n = new ItemNode();
                    n.Info.ID = match.Value;
                    n.NodeType.Email = Email;
                    GD_item item = GoogleDrive.GetMetadataItem(n);
                    n.Info.Size = item.fileSize;
                    n.Info.Name = item.title;
                    n.Info.MimeType = item.mimeType;
                    AppSetting.UIMain.FileSaveDialog(PCPath.Mycomputer, item.title, PCPath.FilterAllFiles, n);
                    return null;
                }
            }
            #endregion

            throw new Exception("Can't Analyze Data Input.");
        }

        public static Stream GetFileStream(ItemNode node, long Startpos = -1,long endpos = -1)
        {
            DriveAPIHttprequestv2 gdclient = GetAPIv2(node.GetRoot.NodeType.Email);
            return gdclient.Files.Get(node.Info.ID, Startpos, endpos);
        }

        public static GD_Files_list Search(string query, string Email,string pageToken = null)
        {
            DriveAPIHttprequestv2 gdclient = GetAPIv2(Email);
            GD_Files_list list = JsonConvert.DeserializeObject<GD_Files_list>(gdclient.Files.List(en, query, pageToken));
            if(!string.IsNullOrEmpty(list.nextPageToken)) list.items.AddRange(Search(query, Email, list.nextPageToken).items);
            list.nextPageToken = null;
            return list;
        }

        static object sync_createfolder = new object();
        public static void CreateFolder(ItemNode node)
        {
            string Email = node.GetRoot.NodeType.Email;
            DriveAPIHttprequestv2 gdclient = GetAPIv2(Email);
            string parent_id = "";
            try
            {
                List<ItemNode> listnode = node.GetFullPath();
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
                        List<ItemNode> listsearchnode = Search("'" + parent_id + "' in parents" +
                            " and trashed=false" +
                            " and title='" + listnode[i].Info.Name.Replace("'", "\\'") + "'" +
                            " and mimeType = 'application/vnd.google-apps.folder'", Email).Convert(node);
                        if (listsearchnode.Count == 0) create = true;
                        else parent_id = listsearchnode[0].Info.ID;
                    }

                    if (create) gdclient.Extend.CreateFolder(listnode[i].Info.Name, parent_id);
                }
            }
            finally { Monitor.Exit(sync_createfolder); }
        }

        public static bool ReNameItem(ItemNode node,string newname)
        {
            DriveAPIHttprequestv2 gdclient = GetAPIv2(node.GetRoot.NodeType.Email);
            string json = "{\"title\": \"" + newname + "\"}";
            string response = gdclient.Files.Patch(node.Info.ID, json);
            dynamic json_ = JsonConvert.DeserializeObject(response);
            string name = json_.title;
            if (name == newname) return true;
            else return false;
        }

        public static GD_item MoveItem(ItemNode nodemove, ItemNode newparent,string newname = null,bool copy = false)
        {
            //Same account
            if (nodemove.GetRoot.NodeType.Email != newparent.GetRoot.NodeType.Email) throw new Exception("Email not match.");
            if (nodemove.GetRoot.NodeType.Type != newparent.GetRoot.NodeType.Type) throw new Exception("TypeCloud not match.");

            DriveAPIHttprequestv2 gdclient = GetAPIv2(nodemove.GetRoot.NodeType.Email);
            GD_item item_metadata = JsonConvert.DeserializeObject<GD_item>(gdclient.Files.Patch(nodemove.Info.ID));
            if (nodemove.Parent != newparent)
            {
                if (!copy) foreach (GD_parent parent in item_metadata.parents) if (parent.id == nodemove.Parent.Info.ID)
                                                                                {
                                                                                    item_metadata.parents.Remove(parent);
                                                                                    break;
                                                                                }
                bool isroot = false;
                if (AppSetting.settings.GetCloudRootNode(gdclient.Token.Email, CloudType.GoogleDrive).Info.ID == newparent.Parent.Info.ID) isroot = true;
                item_metadata.parents.Add(new GD_parent() { id = newparent.Parent.Info.ID, isRoot = isroot });
            }
            if (newname != null) item_metadata.title = newname;
            return JsonConvert.DeserializeObject<GD_item>(gdclient.Files.Patch(nodemove.Info.ID,JsonConvert.SerializeObject(item_metadata)));
        }

        public static GD_item GetMetadataItem(ItemNode node)
        {
            DriveAPIHttprequestv2 client = GetAPIv2(node.GetRoot.NodeType.Email);
            return JsonConvert.DeserializeObject<GD_item>(client.Files.Patch(node.Info.ID, null));
        }
        //trash/delete
        public static bool File_trash(ItemNode node, bool Permanently)
        {
            DriveAPIHttprequestv2 gdclient = GetAPIv2(node.GetRoot.NodeType.Email);
            if (node == node.GetRoot) throw new Exception("Can't delete root.");
            if (Permanently)
            {
                gdclient.Files.Delete(node.Info.ID);
                return true;
            }
            else
            {
                dynamic json = JsonConvert.DeserializeObject(gdclient.Files.Trash(node.Info.ID));
                return true;
            }
        }
        
        public static void Gdclient_TokenRenewEvent(TokenGoogleDrive token)
        {
            XmlNode cloud = AppSetting.settings.GetCloud(token.Email,CloudType.GoogleDrive);
            if (cloud != null) AppSetting.settings.ChangeToken(cloud, JsonConvert.SerializeObject(token));
            else throw new Exception("Can't save token.");
        }
        
    }
    public class GD_Files_list
    {
        public string id;
        public string nextPageToken;
        public List<GD_item> items = new List<GD_item>();
        public List<ItemNode> Convert(ItemNode parent)
        {
            List<ItemNode> list = new List<ItemNode>();
            foreach (GD_item item in items)
            {
                bool add = true;
                GoogleDrive.mimeTypeGoogleRemove.ForEach(m => { if (item.mimeType == m) add = false; });
                if (add) list.Add(
                                    new ItemNode(
                                        new NodeInfo() { Name = item.title, MimeType = item.mimeType, ID = item.id, Size = item.fileSize, DateMod = DateTime.Parse(item.modifiedDate) }, 
                                        parent)
                                  );
                
            }
            return list;
        }
    }
    public class GD_item
    {
        public string title;
        public string modifiedDate;
        public List<GD_parent> parents = new List<GD_parent>();
        public string mimeType;
        public GD_label labels = new GD_label();
        public long fileSize = -1;
        public string id;
        public string fileExtension;
        public string fullFileExtension;
        public string md5Checksum;
        public string Email;
        public permissionsResource userPermission;
        public List<permissionsResource> permissions;
    }

    public class GD_parent
    {
        public bool isRoot = false;
        public string id;
    }

    public class GD_label
    {
        public bool starred = false;
        public bool hidden = false;
        public bool trashed = false;
        public bool restricted = false;
        public bool viewed = true;
    }

    public class RootID
    {
        public string id;
        public string email;
    }

    public class GD_PathNotFoundException : Exception
    {
        public string FoundPath { get; set; }
        public string IdPath { get; set; }
        public GD_PathNotFoundException(string message):base(message)
        {

        }
    }

    public class permissionsResource
    {
        public string id;
        public string name;
        public rolePermissions role;
        public List<rolePermissions> additionalRoles;
        public typePermissions type;
    }

    public enum rolePermissions
    {
        owner, reader, writer
    }

    public enum typePermissions
    {
        user,group,domain,anyone
    }
}
