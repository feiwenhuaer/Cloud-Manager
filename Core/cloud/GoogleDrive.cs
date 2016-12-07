using Core.EncodeDecode;
using GoogleDriveHttprequest;
using Newtonsoft.Json;
using SupDataDll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;


namespace Core.cloud
{
    internal static class GoogleDrive
    {
        private static object sync_GDcache = new object();
        private static List<GD_item> cache_gd = new List<GD_item>();
        private static object sync_root = new object();
        private static List<RootID> root = new List<RootID>();
        private static OrderByEnum[] en = { OrderByEnum.folder, OrderByEnum.title, OrderByEnum.createdDate };
        private static char[] listcannot = new char[] { '/', '\\', ':', '?', '*', '"', '<', '>', '|', '\'' };
        private static string[] mimeTypeGoogleRemove = new string[] {mimeType.audio, mimeType.drawing, mimeType.file,mimeType.form,mimeType.fusiontable,
            mimeType.map,mimeType.presentation,mimeType.script,mimeType.sites,mimeType.unknown,mimeType.video,mimeType.photo};
        
        private static DriveAPIHttprequestv2 GetAPIv2(string Email)
        {
            DriveAPIHttprequestv2 gdclient = new DriveAPIHttprequestv2(JsonConvert.DeserializeObject<TokenGoogleDrive>(AppSetting.settings.GetToken(Email, CloudName.GoogleDrive)));
            gdclient.Email = Email;
            gdclient.TokenRenewEvent += Gdclient_TokenRenewEvent;
            return gdclient;
        }

        public static ListItemFileFolder GetListFileFolder(string path,string Email, string id, bool folderonly = false,bool read_only = false)
        {
           // Console.WriteLine("token:" + token);
            ListItemFileFolder list = new ListItemFileFolder();
            DriveAPIHttprequestv2 gdclient = GetAPIv2(Email);

            string rootid = GetRootID(Email);
            string parent_ID = string.IsNullOrEmpty(rootid) ? "root" : rootid;
            GD_Files_list list_ = new GD_Files_list();
            if (!string.IsNullOrEmpty(id)) { parent_ID = id; goto mainrequest; }//got id parent
            if (path == "/" | string.IsNullOrEmpty(path)) { path = ""; goto mainrequest; }//id parrent = root

            parent_ID = GetIdOfPath(path, Email);// if not have parent id

        mainrequest: // list item in parent from parent_ID
            list_ = Search("'" + parent_ID + "' in parents and trashed=false", Email,parent_ID,read_only);
            if (parent_ID == "root") AddRoot(Email, list_.id);
            list = list_.Convert(parent_ID, folderonly, path);
            list.path_raw = CloudName.GoogleDrive.ToString() + ":" + Email + "/" + path;
            list.path_raw = list.path_raw.Replace("//", "/");
            list.id_folder = parent_ID;
            return list;
        }

        public static Stream GetFileStream(string id,string Email,long Startpos,long endpos)
        {
            DriveAPIHttprequestv2 gdclient = GetAPIv2(Email);
            return gdclient.Files_get(id,Startpos, endpos);
        }

        public static GD_Files_list Search(string query, string Email, string parent_id = null, bool read_only = false)
        {
            DriveAPIHttprequestv2 gdclient = GetAPIv2(Email);
            string temp = gdclient.Files_list(en, query);
            return Check_n_AddToCache(JsonConvert.DeserializeObject<GD_Files_list>(temp),Email, parent_id,read_only);
        }

        public static ListItemFileFolder GetListFolderRecusive(string path, string id,string Email)
        {
            DriveAPIHttprequestv2 gdclient = GetAPIv2(Email);
            ListItemFileFolder list = GetListFileFolder(path, Email, id, true);
            for (int i = 0; i < list.Items.Count; i++)
            {
                ListItemFileFolder list_ = GetListFileFolder(list.Items[i].path_display,Email, list.Items[i].id, true);
                list.Items.AddRange(list_.Items);
            }
            return list;
        }

        private static object sync_createfolder = new object();
        public static string CreateFolder(string Raw_path,string parentid = null,string Email = null,string name = null)
        {
            AnalyzePath rp = new AnalyzePath(Raw_path);
            DriveAPIHttprequestv2 gdclient = GetAPIv2(rp.Email);
            if (string.IsNullOrEmpty(rp.Parent)) return rp.GetPath();
            string parent_id = "";
            lock (sync_createfolder)
            {
                try
                {
                    if (string.IsNullOrEmpty(parentid))
                    {
                        parent_id = GetIdOfPath(rp.GetPath(), rp.Email);
                        return rp.GetPath();
                    }
                    else
                    {
                        foreach(var item in Search("'" + parentid + "' in parents and trashed=false",Email).Convert().Items)
                        {
                            if (item.Name == name) return name;
                        }
                        GD_item it = JsonConvert.DeserializeObject<GD_item>(gdclient.CreateFolder(name, parentid));
                        return it.title;
                    }
                }
                catch (GD_PathNotFoundException gd_ex)
                {
                    string temp = rp.GetPath().Remove(0,gd_ex.FoundPath.Length).TrimStart('/');
                    string[] temp_arr = temp.Split('/');
                    parent_id = gd_ex.IdPath;
                    string data = "";
                    for (int i = 0; i < temp_arr.Length; i++)
                    {
                        data = gdclient.CreateFolder(temp_arr[i], parent_id);//create folder
                        GD_item item = JsonConvert.DeserializeObject<GD_item>(data);
                        parent_id = item.id;
                    }
                    return (gd_ex.FoundPath + "/" + temp).Replace("//","/");
                }
            }
        }

        public static string GetIdOfPath(string path,string Email)
        {
            string root = GetRootID(Email);
            string parent_ID = string.IsNullOrEmpty(root) ? "root" : root;
            path = path.TrimEnd('/').TrimStart('/');
            if (string.IsNullOrEmpty(path)) return parent_ID;
            string parent_ID_temp = "";
            GD_Files_list list_;
            string[] path_arr = path.Split('/');
            for (int i = 0; i < path_arr.Length; i++)//get id path
            {
                parent_ID_temp = "";
                //find in cache
                lock (sync_GDcache)
                {
                    foreach (GD_item item in cache_gd)
                    {
                        if (Email != item.Email) continue;
                        if (item.title == path_arr[i])
                        {
                            foreach (GD_parent parent in item.parents)
                            {
                                if (parent.id == parent_ID)
                                { parent_ID_temp = item.id; break; }//found item id
                            }
                        }
                        if (!string.IsNullOrEmpty(parent_ID_temp)) break;
                    }
                }
                //find in cloud
                if (string.IsNullOrEmpty(parent_ID_temp))//list recusive in cloud if not found
                {
                    list_ = Search("'" + parent_ID + "' in parents and title = '" + path_arr[i] + "' and mimeType = 'application/vnd.google-apps.folder' and trashed=false", Email);
                    foreach (var item in list_.items)
                    {
                        if (item.title == path_arr[i])
                        { parent_ID_temp = item.id; break; }
                    }
                }
                if (string.IsNullOrEmpty(parent_ID_temp))
                {
                    string found_path = "";
                    for (int j = 0; j < i; j++)
                    {
                        found_path += "/" + path_arr[j];
                    }
                    if (string.IsNullOrEmpty(found_path)) found_path = "/";
                    throw new GD_PathNotFoundException("[GD_PathNotFoundException] Path not found: " + path){FoundPath = found_path,IdPath = parent_ID};
                }
                parent_ID = parent_ID_temp;
            }
            return parent_ID;
        }

        private static GD_Files_list Check_n_AddToCache(GD_Files_list list_,string Email,string parent_id = null,bool read_only =false)
        {
            //delete mimeType item not support
            for (int i = 0; i < list_.items.Count; i++)
            {
                list_.items[i].Email = Email;
                if (list_.items[i].mimeType == mimeType.folder) continue;
                foreach (string item in mimeTypeGoogleRemove)
                {
                    if (list_.items[i].mimeType == item)
                    {
                        list_.items.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            //remove deleted item id on cache
            if (parent_id != null)
            {

                if (parent_id == "root")
                {
                    foreach(var id_pr in list_.items[0].parents)
                    {
                        if(id_pr.isRoot)
                        {
                            parent_id = id_pr.id;
                            break;
                        }
                    }
                }
                list_.id = parent_id;
                List<string> foundid = new List<string>();
                lock (sync_GDcache) //get list id in this parent on cache
                {
                    foreach (GD_item item in cache_gd)
                    {
                        if (Email != item.Email) continue;
                        foreach (GD_parent itemparent in item.parents)
                        {
                            if (parent_id == itemparent.id)
                            {
                                foundid.Add(item.id);
                                break;
                            }
                        }
                    }
                }
                if (foundid.Count > 0 && foundid.Count > list_.items.Count)//delete/remove parent not found in cache
                {
                    foreach (string item in foundid)
                    {
                        bool delete = true;
                        foreach (GD_item gditem in list_.items)
                        {
                            if (item == gditem.id)
                            {
                                if (gditem.parents.Count == 1)
                                {
                                    delete = false;//delete item
                                    break;
                                }
                                else
                                {
                                    lock (sync_GDcache)
                                    {
                                        foreach (GD_item gditem_cache in cache_gd)
                                        {
                                            if (Email != gditem_cache.Email) continue;
                                            foreach (GD_parent gdparent_cache in gditem_cache.parents)
                                            {
                                                if (gdparent_cache.id == parent_id)
                                                {
                                                    gditem.parents.Remove(gdparent_cache);//remove parent
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (delete) cache_gd.RemoveAt(GetIndexByID(item));
                    }
                }
            }


            if (list_.items.Count == 0)//if not found
            {
                return list_;
            }

            #region check wrong path & duplicate & auto rename
            List<string> ListIDItemCheckDuplicate;
            foreach (var item in list_.items)
            {
                if (!read_only)
                {
                    //check wrong path
                    string temppath = item.title;
                    foreach (char itemcannot in listcannot)
                    {
                        if (item.title.IndexOf(itemcannot) >= 0)
                        {
                            temppath = temppath.Replace(itemcannot, '_');
                        }
                    }
                    if (temppath != item.title)//autorename if wrong path
                    {
                        ReNameItem(temppath, item.id, Email);
                        item.title = temppath;
                    }
                    //check duplicate item
                    ListIDItemCheckDuplicate = new List<string>();
                    foreach (var item_ in list_.items)
                    {
                        if (item.title == item_.title && item.id != item_.id)
                        {
                            ListIDItemCheckDuplicate.Add(item_.id);
                        }
                    }
                    //rename duplicate item 2nd -> 
                    for (int j = 0; j < ListIDItemCheckDuplicate.Count; j++)
                    {
                        int z = 1;
                        bool found_duplicate = false;
                        while (true)
                        {
                            foreach (var item_ in list_.items)
                            {
                                if (item_.title == (item.title + "(" + z.ToString() + ")")) { found_duplicate = true; break; }
                                else found_duplicate = false;
                            }
                            if (found_duplicate) z++;
                            else break;
                        }
                        ReNameItem(item.title + "(" + z.ToString() + ")", ListIDItemCheckDuplicate[j], Email);
                        for (int k = 0; k < list_.items.Count; k++)
                        {
                            if (list_.items[k].id == ListIDItemCheckDuplicate[j])
                            {
                                list_.items[k].title = list_.items[k].title + "(" + z.ToString() + ")";
                                break;
                            }
                        }
                    }
                }
                //add item to cache
                AddAndEditGDcache(item);
                
            }
            #endregion

            GC.Collect();
            return list_;
        }

        private static string GetTitleByID(string id, string Email = "")
        {
            lock (sync_GDcache)
            {
                foreach (GD_item item in cache_gd)
                {
                    if ((string.IsNullOrEmpty(Email) ? true : Email == item.Email) && item.id == id) return item.title;
                }
            }
            return null;
        }

        private static int GetIndexByID(string id, string Email = "")
        {
            int index = 0;
            lock (sync_GDcache)
            {
                foreach (GD_item item in cache_gd)
                {
                    if ((string.IsNullOrEmpty(Email) ? true : Email == item.Email) && item.id == id) return index;
                    index++;
                }
            }
            return -1;
        }

        private static void AddAndEditGDcache(GD_item item_)
        {
            if (string.IsNullOrEmpty(item_.Email)) throw new ArgumentNullException(item_.Email);
            int index = 0;
            bool addnew = true;
            lock (sync_GDcache)
            {
                foreach (GD_item check in cache_gd)
                {
                    if (item_.Email == check.Email && check.id == item_.id && check.title == item_.title)
                    {
                        addnew = false;
                        break;
                    }
                    index++;
                }
                if (addnew) cache_gd.Add(item_);
                else cache_gd[index] = item_;
            }
        }
        
        public static bool ReNameItem(string newname, string id,string Email)
        {
            DriveAPIHttprequestv2 gdclient = GetAPIv2(Email);
            string json = "{\"title\": \"" + newname + "\"}";
            string response = gdclient.EditMetaData(id, json);
            dynamic json_ = JsonConvert.DeserializeObject(response);
            string name = json_.title;
            if (name == newname) return true;
            else return false;
        }

        public static string MoveItem(DriveAPIHttprequestv2 gdclient, string iditem, string idoldparent, string idnewparent, bool copy = false)
        {
            GD_item item_metadata = JsonConvert.DeserializeObject<GD_item>(gdclient.EditMetaData(iditem));
            if (!copy) foreach (GD_parent parent in item_metadata.parents)
                {
                    if (parent.id == idoldparent)
                    {
                        item_metadata.parents.Remove(parent);
                        break;
                    }
                }
            bool isroot = false;
            foreach (RootID r in root)
            {
                if (r.id == idnewparent) isroot = true;
            }
            item_metadata.parents.Add(new GD_parent() { id = idnewparent, isRoot = isroot });
            return gdclient.EditMetaData(JsonConvert.SerializeObject(item_metadata));
        }

        private static string GetRootID(string email)
        {
            lock (sync_root)
            {
                foreach (RootID item in root)
                {
                    if (item.email == email) return item.id;
                }
            }
            return null;
        }

        private static void AddRoot(string email, string id)
        {
            lock (sync_root)
            {
                foreach (RootID item in root)
                {
                    if (item.email == email && item.id == id) return;
                }
                root.Add(new RootID() { email = email, id = id });
            }
        }

        public static string GetMetadataItem(string email,string id)
        {
            DriveAPIHttprequestv2 client = GetAPIv2(email);
            return client.EditMetaData(id, null);
        }
        //trash/delete
        public static bool File_trash(string path, string id, string token, string Email,bool Permanently)
        {
            ListItemFileFolder list = new ListItemFileFolder();
            TokenGoogleDrive tk = JsonConvert.DeserializeObject<TokenGoogleDrive>(token);
            DriveAPIHttprequestv2 gdclient = new DriveAPIHttprequestv2(tk);
            gdclient.Email = Email;
            gdclient.TokenRenewEvent += Gdclient_TokenRenewEvent;

            if (string.IsNullOrEmpty(id))
            {
                if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(path);
                id = GetIdOfPath(path,Email);
            }

            if(string.IsNullOrEmpty(id)) throw new ArgumentNullException(id);
            if (Permanently)
            {
                gdclient.Files_delete(id);
                return true;
            }
            else
            {
                dynamic json = JsonConvert.DeserializeObject(gdclient.ItemTrash(id));
                return true;
            }
        }
        
        public static void Gdclient_TokenRenewEvent(TokenGoogleDrive token, string Email)
        {
            string json = JsonConvert.SerializeObject(token);
            XmlNode cloud = AppSetting.settings.GetCloud(Email,CloudName.GoogleDrive);
            if (cloud != null) AppSetting.settings.ChangeToken(cloud, json);
            else throw new Exception("Can't save token");
        }
    }

    public class GD_Files_list
    {
        public string id;
        public string nextPageToken;
        public List<GD_item> items = new List<GD_item>();
        public ListItemFileFolder Convert(string parent_ID= null,bool folderonly = false,string path = null)
        {
            ListItemFileFolder list = new ListItemFileFolder();
            foreach(GD_item item in items)
            {
                if (item.labels.hidden & item.labels.trashed) continue;
                FileFolder ff = new FileFolder();
                ff.id = item.id;
                ff.Name = item.title;
                ff.Time_mod = DateTime.Parse(item.modifiedDate);
                ff.mimeType = item.mimeType;
                if (parent_ID == "root") ff.parentid.Add(id);
                else foreach (GD_parent item_ in item.parents)
                    {
                        ff.parentid.Add(item_.id);
                    }
                if (!string.IsNullOrEmpty(path)) ff.path_display = path + "/" + item.title;
                if (folderonly) { list.Items.Add(ff); continue; }
                else
                {
                    switch (item.mimeType)
                    {
                        case mimeType.folder: break;
                        case mimeType.spreadsheet: continue;
                        case mimeType.document: continue;
                        //case mimeType.spreadsheet: ff.Size = 0; ff.Name += "." + AppSetting.settings.GetSettingsAsString(SettingsKey.mimeType_spreadsheet); break;
                        //case mimeType.document: ff.Size = 0; ff.Name += "." + AppSetting.settings.GetSettingsAsString(SettingsKey.mimeType_document); break;
                        default: ff.Size = item.fileSize; break;
                    }
                    list.Items.Add(ff);
                }
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
        public long fileSize = 0;
        public string id;
        public string fileExtension;
        public string fullFileExtension;

        public string Email;
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
}
