using Cloud.Dropbox;
using Newtonsoft.Json;
using SupDataDll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Core.cloud
{
    internal static class Dropbox
    {
        public static ListItemFileFolder GetListFileFolder(string path,string Email)
        {
            if (path == "/") path = "";
            ListItemFileFolder list = new ListItemFileFolder();
            DropboxRequestAPIv2 client = GetAPIv2(Email);
            string data = client.ListFolder(path).Replace("\".tag\":", "\"tag\":");
            ListFolder json;
            try
            {
                json = JsonConvert.DeserializeObject<ListFolder>(data);
            }
            catch { throw new Exception("Can't find folder: " + path + "\r\nError info:" + data); }
            foreach (entrie ct in json.entries)
            {
                if (ct.tag == "folder")
                {
                    FileFolder item = new FileFolder();
                    item.Name = ct.name;
                    item.Size = -1;
                    item.id = ct.id;
                    list.Items.Add(item);
                }
            }
            foreach (entrie ct in json.entries)
            {
                if (ct.tag == "file")
                {
                    FileFolder item = new FileFolder();
                    item.Name = ct.name;
                    item.Size = ct.size;
                    item.id = ct.id;
                    item.Time_mod = DateTime.Parse(ct.client_modified);
                    list.Items.Add(item);
                }
            }
            list.path_raw = CloudName.Dropbox.ToString() + ":" + Email + "/" + path;
            list.path_raw = list.path_raw.Replace("//", "/");
            return list;
        }

        private static DropboxRequestAPIv2 GetAPIv2(string Email)
        {
            return new DropboxRequestAPIv2(AppSetting.settings.GetToken(Email, CloudName.Dropbox));
        }

        public static Stream GetFileStream(string path,string Email,long Startpos,long endpos)
        {
            DropboxRequestAPIv2 client = GetAPIv2(Email);
            return client.Download(path,Startpos, endpos);
        }

        public static ListItemFileFolder ListFolder(string path, string Email)
        {
            if (path == "/") path = "";
            ListItemFileFolder list = new ListItemFileFolder();
            DropboxRequestAPIv2 client = GetAPIv2(Email);

            string data = client.ListFolder(path, false, true, false, true).Replace("\".tag\":", "\"tag\":");
            ListFolder json = JsonConvert.DeserializeObject<ListFolder>(data);
            foreach (entrie ct in json.entries)
            {
                if (ct.tag == "folder")
                {
                    FileFolder item = new FileFolder();
                    item.Name = ct.name;
                    item.Size = -1;
                    item.id = ct.id;
                    list.Items.Add(item);
                    item.path_display = ct.path_display;
                }
            }
            return list;
        }

        public static string CreateFolder(string path,string Email)
        {
            if (path == "/") path = "";
            ListItemFileFolder list = new ListItemFileFolder();
            DropboxRequestAPIv2 client = GetAPIv2(Email);
            string data = client.create_folder(path);
            dynamic json = JsonConvert.DeserializeObject(data);
            string path_display = json.path_display;
            return path_display;
        }

        public static bool Delete(string path,string Email,bool PernamentDelete)
        {
            DropboxRequestAPIv2 dropbox_client = GetAPIv2(Email);
            string path_display = "";
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

        public static bool Rename(string path_raw,string newname)
        {
            AnalyzePath ap = new AnalyzePath(path_raw);
            DropboxRequestAPIv2 client = GetAPIv2(ap.Email);
            dynamic json = JsonConvert.DeserializeObject(client.move(ap.GetPath(), ap.Parent +"/" + newname));
            return (ap.Parent + "/" + newname) == (string)(json.path_display);
        }

        private static object sync_CreateFolder = new object();

        public static string AutoCreateFolder(string path, string Email)
        {
            DropboxRequestAPIv2 client = GetAPIv2(Email);
            lock (sync_CreateFolder)
            {
                string[] path_arr = path.TrimEnd('/').TrimStart('/').Split('/');
                int index_path_Exist = -1;
                string path_ = "";
                for (int i = path_arr.Length - 1; i >= 0; i--)
                {
                    path_ = "";
                    for (int j = 0; j <= i; j++)
                    {
                        path_ += "/" + path_arr[j];
                    }
                    try
                    {
                        client.ListFolder(path_);
                        index_path_Exist = i;
                        break;
                    }
                    catch (HttpException ex)
                    {
                        if (ex.ErrorCode == 409)
                        {
                            continue;
                        }
                    }
                }
                for (int i = index_path_Exist + 1; i < path_arr.Length; i++)
                {
                    path_ = "";
                    for (int j = 0; j <= i; j++)
                    {
                        path_ += "/" + path_arr[j];
                    }
                    client.create_folder(path_);
                }
                return path_;
            }
        }
    }

    public class ListFolder
    {
        public List<entrie> entries;
    }

    public class entrie
    {
        public string id;
        public string tag;
        public string name;
        public string client_modified = "";
        public string path_display;
        public long size = -2;
    }
}
