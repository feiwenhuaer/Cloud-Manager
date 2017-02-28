using Cloud.Dropbox;
using Newtonsoft.Json;
using SupDataDll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Threading;

namespace Core.Cloud
{
    internal static class Dropbox
    {
        #region field
        private static object sync_CreateFolder = new object();
        #endregion

        #region Public Method
        public static ExplorerNode GetListFileFolder(ExplorerNode node)
        {
            DropboxRequestAPIv2 client = GetAPIv2(node.GetRoot().RootInfo.Email);
            string path = node.GetFullPathString(false);
            string data = client.ListFolder(path).Replace("\".tag\":", "\"tag\":");
            ListFolder json;
            try
            {
                json = JsonConvert.DeserializeObject<ListFolder>(data);
            }
            catch { throw new Exception("Can't find folder: " + path + "\r\nError info:" + data); }
            node.Child.Clear();
            foreach (entrie ct in json.entries) if (ct.tag == "folder") new ExplorerNode(new NodeInfo() { Name = ct.name, Size = -1, ID = ct.id }, node);
            foreach (entrie ct in json.entries) if (ct.tag == "file") new ExplorerNode(new NodeInfo() { Name = ct.name, Size = ct.size, ID = ct.id, DateMod = DateTime.Parse(ct.client_modified) }, node);
            return node;
        }
        
        public static Stream GetFileStream(ExplorerNode node,long Startpos,long endpos)
        {
            DropboxRequestAPIv2 client = GetAPIv2(node.GetRoot().RootInfo.Email);
            return client.Download(node.GetFullPathString(false), Startpos, endpos);
        }
        
        public static string CreateFolder(ExplorerNode node)
        {
            if (node == node.GetRoot()) throw new Exception("Node is root.");
            DropboxRequestAPIv2 client = GetAPIv2(node.GetRoot().RootInfo.Email);
            dynamic json = JsonConvert.DeserializeObject(client.create_folder(node.GetFullPathString(false)));
            string path_display = json.path_display;
            return path_display;
        }

        public static bool Delete(ExplorerNode node, bool PernamentDelete)
        {
            DropboxRequestAPIv2 dropbox_client = GetAPIv2(node.GetRoot().RootInfo.Email);
            string path_display = "";
            string path = node.GetFullPathString(false);
            if (PernamentDelete)
            {
                dynamic json_response = JsonConvert.DeserializeObject(dropbox_client.permanently_delete(path));
                path_display = json_response.path_display;
            }
            else
            {
                dynamic json_response = JsonConvert.DeserializeObject(dropbox_client.delete(path));
                path_display = json_response.path_display;
            }
            if (path_display != path) return false;
            else return true;
        }

        public static bool Move(ExplorerNode nodemove, ExplorerNode newparent, string newname = null)
        {
            if (nodemove.GetRoot().RootInfo.Email != newparent.GetRoot().RootInfo.Email || nodemove.GetRoot().RootInfo.Type != newparent.GetRoot().RootInfo.Type) throw new Exception("Cloud not match.");
            DropboxRequestAPIv2 client = GetAPIv2(nodemove.GetRoot().RootInfo.Email);
            dynamic json = JsonConvert.DeserializeObject(client.move(nodemove.GetFullPathString(false), newparent.GetFullPathString(false) + "/" + newname == null ? nodemove.Info.Name : newname));
            return newparent.GetFullPathString(false) == (string)(json.path_display);
        }
        
        public static string AutoCreateFolder(ExplorerNode node)
        {
            if (node.Info.Size > 0) throw new Exception("Node is file.");
            DropboxRequestAPIv2 client = GetAPIv2(node.GetRoot().RootInfo.Email);
            try
            {
                Monitor.Enter(sync_CreateFolder);

                List<ExplorerNode> pathlist = node.GetFullPath();
                int i;
                for (i = 1; i < pathlist.Count; i++)
                {
                    try
                    {
                        client.ListFolder(pathlist[i].GetFullPathString(false));
                    }
                    catch (HttpException ex)
                    {
                        if (ex.ErrorCode == 409) break;
                        throw ex;
                    }
                }
                for (; i < pathlist.Count; i++) client.create_folder(pathlist[i].GetFullPathString(false));
                return pathlist[i - 1].GetFullPathString(false);
            }
            finally { Monitor.Exit(sync_CreateFolder); }
        }
        #endregion

        #region Private Method
        private static DropboxRequestAPIv2 GetAPIv2(string Email)
        {
            return new DropboxRequestAPIv2(AppSetting.settings.GetToken(Email, CloudType.Dropbox));
        }
        class ListFolder
        {
            public List<entrie> entries;
        }
        class entrie
        {
            public string id;
            public string tag;
            public string name;
            public string client_modified = "";
            public string path_display;
            public long size = -2;
        }
        #endregion
    }
}
