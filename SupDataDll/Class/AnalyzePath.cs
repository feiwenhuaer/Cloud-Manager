//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace SupDataDll
//{
//    public class AnalyzePath
//    {
//        #region Field
//        public string Email { get { if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("Email"); else return email; } }
//        private string email = "";

//        public string Path_Raw { get { return path_raw; } }
//        private string path_raw = "";
//        private string pathcloud = "";

//        public TypePath TypePath { get { return typepath; } }
//        private TypePath typepath = TypePath.Cloud;

//        public string Query { get { if (string.IsNullOrEmpty(q)) throw new ArgumentNullException("Query"); else return q; } }
//        private string q = "";

//        public CloudType TypeCloud { get { return type; } }
//        private CloudType type = CloudType.LocalDisk;

//        public bool PathIsCloud { get { return path_is_cloud; } }
//        private bool path_is_cloud = false;

//        public bool PathIsUrl { get { return isurl; } }
//        bool isurl = false;

//        public string Parent { get { if (string.IsNullOrEmpty(path_parent)) throw new ArgumentNullException("Parent"); else return path_parent; } }
//        private string path_parent = "";

//        public string NameLastItem { get { if (string.IsNullOrEmpty(namelastitem)) throw new ArgumentNullException("NameLastItem"); else return namelastitem; } }
//        private string namelastitem = "";

//        public string ID { get { if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("ID"); else return id; } }
//        private string id = "";

//        public bool IsFolder { get { return isFolder; } }
//        private bool isFolder = true;

//        public bool HaveParent { get { return haveparent; } }
//        private bool haveparent = false;
//        #endregion

//        #region const
//        private const string Rg_cloud = "^\\w+:(\\w|\\.|@)+",//GoogleDrive:tqk2811@gmail.com
//        Rg_CloudName = "^\\w+",//GoogleDrive[:
//        Rg_email = "(?<=:)(\\w|\\.)+@(\\w|\\.)+",// :]tqk2.sa_dsa@abc.com 
//        Rg_PathCloud = "\\/.+$", // GoogleDrive:tqk2811@gmail.com]/abc/def/ad
//        Rg_LastItem = "((?!\\/).)+$",//ad fas  ((?!\/).)+$
//        Rg_url_GD = "^https:\\/\\/drive\\.google\\.com",//https://drive.google.com......
//        Rg_url_idFolder = "(?<=\\/folders\\/)[A-Za-z0-9-_]+",//drive/folders/id_folder or drive.google.com/drive/u/0/folders/id
//        Rg_url_idFolderOpen = "(?<=\\?id\\=)[A-Za-z0-9-_]+",
//        Rg_url_idFile = "(?<=file\\/d\\/)[A-Za-z0-9-_]+",
//        Rg_cloud_id = "(?<=id=)\\S+",
//        Rg_cloud_query = "(?<=q=).+",
//        Rg_Disk_LastItem = "(?<=\\\\)(.(?!\\\\))+$";
//        #endregion

//        #region Static 
//        public static List<string> GetPathTo(List<string> pathform, AnalyzePath rootfrom, AnalyzePath rootto)
//        {
//            List<string> pathto = new List<string>();
//            foreach (string from in pathform)
//            {
//                pathto.Add(GetPathTo(from, rootfrom, rootto));
//            }
//            return pathto;
//        }
//        /// <summary>
//        /// If rootfrom is url -> pathfrom format: IDparent/filename
//        /// </summary>
//        /// <param name="pathfrom">GD:email?id=xab/abc/def.txt or \abc\def.txt or IDparent/folder.../filename (rootfrom is url)</param>
//        /// <param name="rootfrom"></param>
//        /// <param name="rootto"></param>
//        /// <returns></returns>
//        public static string GetPathTo(string pathfrom, AnalyzePath rootfrom, AnalyzePath rootto)
//        {
//            string temp = rootto.PathIsCloud ? "/" : "\\";
//            string de_temp = rootto.PathIsCloud ? "\\" : "/";
//            // if from is url
//            if (IsUrl(rootfrom.Path_Raw) && (rootfrom.TypePath == TypePath.UrlFolder | rootfrom.TypePath == TypePath.UrlFile))
//            {
//                Regex rg = new Regex("(?<=\\?id\\=).+$");
//                Match m = rg.Match(pathfrom);
//                if(m.Success)
//                {
//                    string[] pathfrom_arr = m.Value.Split('/');
//                    if (pathfrom_arr[0] != rootfrom.ID) throw new Exception("id:" + pathfrom_arr[0] + " not indexof " + rootfrom.Path_Raw);// check id
//                    string temppath = m.Value.Remove(0, pathfrom_arr[0].Length + 1);// remove id
//                    return RemoveDup((rootto.path_raw + temp + temppath).Replace(de_temp, temp), temp); // ->E:\testfolder\folder...\filename.txt
//                }
//            }
            
//            //if from root folder (cloud)
//            if (string.IsNullOrEmpty(rootfrom.GetPath()))// Ex: /abc.txt ,  , E:\newfolder
//            {
//                return RemoveDup((rootto.Path_Raw + temp + pathfrom).Replace(de_temp, temp), temp);// -> E:\newfolder\abc.txt
//            }
//            //not root folder
//            if (pathfrom.IndexOf(rootfrom.GetPath()) >= 0)// Ex: /folder/abc.txt, /folder, E:\newfolder
//            {

//                string newpath = rootto.Path_Raw + temp + pathfrom.Remove(0,rootfrom.GetPath().Length).Replace(de_temp, temp);// ->E:\newfolder\abc.txt
//                newpath = RemoveDup(newpath, temp);

//                #region create folder in Disk
//                if (!IsCloud(newpath))
//                {
//                    FileInfo info = new FileInfo(newpath);
//                    if (info.Exists)
//                    {
//                        string extension = info.Extension;
//                        string filename = info.Name.Remove(info.Name.Length - extension.Length, extension.Length);
//                        string newfilepatch = "";
//                        FileInfo info_;
//                        int i = 1;
//                        while (true)
//                        {
//                            newfilepatch = info.DirectoryName + "\\" + filename + " (" + i.ToString() + ")" + extension;
//                            info_ = new FileInfo(newfilepatch);
//                            if (info_.Exists) { i++; continue; }
//                            else { newpath = newfilepatch; break; }
//                        }
//                    }
//                }
//                #endregion

//                return newpath;
//            }

//            throw new Exception(pathfrom + " not index of " + rootfrom.Path_Raw);
//        }

//        static string RemoveDup(string newpath,string temp)// replace \\ -> \ or // -> /
//        {
//            while (newpath.IndexOf(temp + temp) >= 0)
//            {
//                newpath = newpath.Replace(temp + temp, temp);
//            }
//            return newpath;
//        }

//        public static bool IsCloud(string path)
//        {
//            if (path.IndexOf("\\") >= 0 & path.IndexOf("/") >= 0) throw new Exception("Path error: " + path + ".");
//            string[] pathsplit;
//            if (path.IndexOf("\\") >= 0)//local
//            {
//                pathsplit = path.TrimStart('\\').Split('\\');
//                if (pathsplit.Length > 0 && pathsplit[0].Length > 1 && pathsplit[0][1] == ':') return false;// E:\ C:\ ( DiskName:\ )
//            }

//            if (path.IndexOf('/') >= 0)//cloud
//            {
//                pathsplit = path.TrimStart('/').Split('/');
//                if (pathsplit.Length > 0 )
//                {
//                    Regex rg = new Regex(Rg_cloud);
//                    Match m = rg.Match(pathsplit[0]);
//                    if (m.Success) return true;// [NameCloud]:[Email]
//                }
//            }
//            if (path.IndexOf('@') > 0 && path.IndexOf(':') > 0) return true;

//            if (path.Length > 1 && path[1] == ':') return false;
//            throw new Exception("Path error: " + path);
//        }

//        public static bool IsUrl(string path)
//        {
//            if (path.ToLower().IndexOf("http") == 0) return true;
//            else return false;
//        }
//        #endregion


//        #region Method
//        public AnalyzePath(string path)
//        {
//            if (string.IsNullOrEmpty(path)) throw new Exception("Path is null.");
//            path_raw = path.TrimEnd('/').TrimEnd('\\').TrimStart('/').TrimStart('\\');
//            path_raw = RemoveDup(path_raw, "/");
//            path_raw = RemoveDup(path_raw, "\\");

//            path_is_cloud = IsCloud(path_raw);
//            isurl = IsUrl(path_raw);

//            if (path_is_cloud) GetRealCloudPath();
//            else GetDiskPath();
//        }

//        private void GetDiskPath()
//        {
//            if (path_raw.Length == 2) path_raw += "\\"; // C: -> C:\
//            Regex rg = new Regex(Rg_Disk_LastItem);
//            Match match = rg.Match(path_raw);
//            if (match.Success)
//            {
//                namelastitem = match.Value;
//                if (namelastitem.Length > 0)
//                {
//                    haveparent = true;
//                    path_parent = path_raw.Remove(path_raw.Length - namelastitem.Length, namelastitem.Length).TrimEnd('\\');
//                }
//            }
//        }

//        private void GetRealCloudPath()
//        {
//            Regex rg;
//            Match match;

//            #region GoogleDrive:abc@gmail.com / Dropbox:abc@yahoo.com
//            rg = new Regex(Rg_cloud);//check cloud type [CloudName]:[Email]/..../...
//            match = rg.Match(path_raw);
//            if (match.Success)
//            {
//                rg = new Regex(Rg_CloudName);
//                match = rg.Match(path_raw);
//                if (match.Success) type = (CloudType)Enum.Parse(typeof(CloudType), match.Value); //CloudName

//                rg = new Regex(Rg_email);
//                match = rg.Match(path_raw);
//                if (match.Success) email = match.Value;//Email

//                rg = new Regex(Rg_PathCloud);//[Cloudname:Email]/.../..../.../.../...
//                match = rg.Match(path_raw);
//                if (match.Success)
//                {
//                    pathcloud = match.Value;//path cloud /abc/def/ghi/....

//                    rg = new Regex(Rg_LastItem);
//                    match = rg.Match(path_raw);
//                    if (match.Success)
//                    {
//                        namelastitem = match.Value;//name last item
//                        if (pathcloud.TrimEnd('/').TrimStart('/').Length != namelastitem.Length | !string.IsNullOrEmpty(namelastitem))
//                        {
//                            haveparent = true;
//                            path_parent =  pathcloud.Remove(pathcloud.Length - namelastitem.Length - 1, namelastitem.Length + 1);
//                        }
//                    }
//                    return;
//                }
//                else
//                {
//                    rg = new Regex(Rg_cloud_id);// GD:abc@gmail.com?id=ads
//                    match = rg.Match(path_raw);
//                    if(match.Success)
//                    {
//                        id = match.Value;
//                        typepath = TypePath.CloudID;
//                        return;
//                    }

//                    rg = new Regex(Rg_cloud_query);// GD:abc@gmail.com?q='abc' in parent
//                    match = rg.Match(path_raw);
//                    if(match.Success)
//                    {
//                        q = match.Value;
//                        typepath = TypePath.CloudQuery;
//                        return;
//                    }
//                }
//                return;
//            }
//            #endregion

//            #region https://drive.google.com
//            rg = new Regex(Rg_url_GD);
//            match = rg.Match(path_raw);
//            if (match.Success)
//            {
//                type = CloudType.GoogleDrive;
//                pathcloud = path_raw;

//                rg = new Regex(Rg_url_idFolder);
//                match = rg.Match(path_raw);
//                if (match.Success)
//                {
//                    id = match.Value;
//                    typepath = TypePath.UrlFolder;
//                    return;
//                }

//                rg = new Regex(Rg_url_idFolderOpen);
//                match = rg.Match(path_raw);
//                if (match.Success)
//                {
//                    this.id = match.Value;
//                    typepath = TypePath.UrlFolder;
//                    return;
//                }

//                rg = new Regex(Rg_url_idFile);
//                match = rg.Match(path_raw);
//                if (match.Success)
//                {
//                    this.id = match.Value;
//                    isFolder = false;
//                    typepath = TypePath.UrlFile;
//                    return;
//                }
//            }
//            #endregion
            
//            throw new Exception("Path error: " + path_raw);
//        }

//        public string GetExtensionFile()
//        {
//            if (NameLastItem.IndexOf('.') >= 0)
//            {
//                string[] temp_arr = NameLastItem.Split('.');
//                return temp_arr[temp_arr.Length - 1];
//            }else return NameLastItem;
//        }

//        public string GetPath()
//        {
//            return (path_is_cloud ? pathcloud : path_raw);
//        }

//        public string ReplaceIDUrl(string newid)
//        {
//            return pathcloud.Replace(id, newid);
//        }

//        public string AddRawChildPath(string name)
//        {
//            return path_raw + (IsCloud(path_raw) ? "/" : "\\") + name;
//        }

//        public AnalyzePath GetParent()
//        {
//            if (path_is_cloud) return new AnalyzePath(type.ToString() + ":" + email.ToString() + "/" + Parent);
//            return new AnalyzePath(Parent);

//        }
//        #endregion
//    }
//}
