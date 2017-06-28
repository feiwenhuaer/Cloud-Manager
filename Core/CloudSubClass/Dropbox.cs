using Cloud.Dropbox;
using Newtonsoft.Json;
using CloudManagerGeneralLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Threading;
using CloudManagerGeneralLib.Class;
using Core.StaticClass;

namespace Core.CloudSubClass
{
    internal static class Dropbox
    {
        #region field
        private static object sync_CreateFolder = new object();
        #endregion

        #region Public Method
        public static ItemNode GetListFileFolder(ItemNode node)
        {
            DropboxRequestAPIv2 client = GetAPIv2(node.GetRoot.NodeType.Email);

            IDropbox_Response_ListFolder response = client.ListFolder(new Dropbox_Request_ListFolder(node.GetFullPathString(false)));
            node.Child.Clear();
            foreach (IDropbox_Response_MetaData metadata in response.entries)
                if (metadata.tag == "folder")
                    new ItemNode(
                        new NodeInfo() { Name = metadata.name, Size = -1, ID = metadata.id }, node);

            foreach (IDropbox_Response_MetaData metadata in response.entries)
                if (metadata.tag == "file")
                    new ItemNode(
                        new NodeInfo() { Name = metadata.name, Size = metadata.size, ID = metadata.id, DateMod = DateTime.Parse(metadata.client_modified) }, node);
            return node;
        }
        
        public static Stream GetFileStream(ItemNode node,long Startpos = -1,long endpos = -1)
        {
            DropboxRequestAPIv2 client = GetAPIv2(node.GetRoot.NodeType.Email);
            return client.Download(new Dropbox_path(node.GetFullPathString(false)), Startpos, endpos);
        }
        
        public static void CreateFolder(ItemNode node)
        {
            if (node == node.GetRoot) throw new Exception("Node is root.");
            DropboxRequestAPIv2 client = GetAPIv2(node.GetRoot.NodeType.Email);
            IDropbox_Response_MetaData metadata = client.create_folder(new Dropbox_path(node.GetFullPathString(false)));
            node.Info.ID = metadata.id;
        }

        public static bool Delete(ItemNode node, bool PernamentDelete)
        {
            DropboxRequestAPIv2 dropbox_client = GetAPIv2(node.GetRoot.NodeType.Email);
            string path = node.GetFullPathString(false);
            if (PernamentDelete)
            {
                dropbox_client.permanently_delete(new Dropbox_path(path));
                return true;
            }
            else
            {
                IDropbox_Response_MetaData metadata = dropbox_client.delete(new Dropbox_path(path));
                return true;
            }
        }

        public static bool Move(ItemNode nodemove, ItemNode newparent, string newname = null)
        {
            if (nodemove.GetRoot.NodeType.Email != newparent.GetRoot.NodeType.Email || nodemove.GetRoot.NodeType.Type != newparent.GetRoot.NodeType.Type) throw new Exception("Cloud not match.");
            DropboxRequestAPIv2 client = GetAPIv2(nodemove.GetRoot.NodeType.Email);
            IDropbox_Response_MetaData metadata = client.move(
                new Dropbox_Request_MoveCopy(   nodemove.GetFullPathString(false), 
                                                newparent.GetFullPathString(false) + "/" + newname == null ? nodemove.Info.Name : newname
                                            ));
            return newparent.GetFullPathString(false) == metadata.path_display;
        }
        
        public static string AutoCreateFolder(ItemNode node)
        {
            if (node.Info.Size > 0) throw new Exception("Node is file.");
            DropboxRequestAPIv2 client = GetAPIv2(node.GetRoot.NodeType.Email);
            try
            {
                Monitor.Enter(sync_CreateFolder);

                List<ItemNode> pathlist = node.GetFullPath();
                int i;
                for (i = 1; i < pathlist.Count; i++)
                {
                    try
                    {
                        client.ListFolder(new Dropbox_Request_ListFolder(pathlist[i].GetFullPathString(false)));
                    }
                    catch (HttpException ex)
                    {
                        if (ex.ErrorCode == 409) break;
                        throw ex;
                    }
                }
                for (; i < pathlist.Count; i++) client.create_folder(new Dropbox_path(pathlist[i].GetFullPathString(false)));
                return pathlist[i - 1].GetFullPathString(false);
            }
            finally { Monitor.Exit(sync_CreateFolder); }
        }

        public static ItemNode GetMetaData(ItemNode node)
        {
            DropboxRequestAPIv2 client = GetAPIv2(node.GetRoot.NodeType.Email);
            IDropbox_Response_MetaData metadata = client.GetMetadata(
                new Dropbox_Request_Metadata(string.IsNullOrEmpty(node.Info.ID) ? node.GetFullPathString(false, true) : "id:" + node.Info.ID));
            node.Info.Name = metadata.name;
            node.Info.DateMod = DateTime.Parse(metadata.server_modified);
            node.Info.Size = metadata.size;
            return node;
        }
        #endregion

        #region Private
        internal static DropboxRequestAPIv2 GetAPIv2(string Email)
        {
            return new DropboxRequestAPIv2(AppSetting.settings.GetToken(Email, CloudType.Dropbox));
        }
        #endregion


        

    }
    internal class Dropbox_Request_ListFolder : Cloud.Dropbox.IDropbox_Request_ListFolder
    {
        public Dropbox_Request_ListFolder(string path,bool include_media_info =false, bool include_deleted = false, bool include_has_explicit_shared_members = false, bool recursive = false)
        {
            this.path = path;
            this.include_deleted = include_deleted;
            this.include_has_explicit_shared_members = include_has_explicit_shared_members;
            this.recursive = recursive;
            this.include_media_info = include_media_info;
        }
        
        public bool include_deleted { get; set; }
        public bool include_has_explicit_shared_members { get; set; }
        public bool include_media_info { get; set; }
        public string path { get; set; }
        public bool recursive { get; set; }

    }

    internal class Dropbox_Request_Metadata : Cloud.Dropbox.IDropbox_Request_Metadata
    {
        public Dropbox_Request_Metadata(string path, bool include_deleted = false, bool include_media_info = false, bool include_has_explicit_shared_members = false)
        {
            this.path = path;
            this.include_media_info = include_media_info;
            this.include_has_explicit_shared_members = include_has_explicit_shared_members;
            this.include_deleted = include_deleted;
        }
        
        public bool include_deleted { get; set; }
        public bool include_has_explicit_shared_members { get; set; }
        public bool include_media_info { get; set; }
        public string path { get; set; }
    }

    internal class Dropbox_path : IDropbox_Path
    {
        public Dropbox_path(string path)
        {
            this.path = path;
        }

        public string path { get; set; }
    }

    internal class Dropbox_Request_MoveCopy : Cloud.Dropbox.IDropbox_Request_MoveCopy
    {
        public Dropbox_Request_MoveCopy(string from_path, string to_path, bool autorename = false, bool allow_shared_folder = false)
        {
            this.allow_shared_folder = allow_shared_folder;
            this.autorename = autorename;
            this.from_path = from_path;
            this.to_path = to_path;
        }

        public bool allow_shared_folder { get; set; }
        public bool autorename { get; set; }
        public string from_path { get; set; }
        public string to_path { get; set; }
    }
}
