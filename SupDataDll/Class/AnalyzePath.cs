using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SupDataDll
{
    public class AnalyzePath
    {
        #region Field
        public string Email { get { return email; } }
        private string email = "";

        public string Path_Raw { get { return path_raw; } }
        private string path_raw = "";
        private string pathcloud = "";

        public TypePath TypePath { get { return typepath; } }
        private TypePath typepath = TypePath.Cloud;

        public string Query { get { return q; } }
        private string q = "";

        public CloudName TypeCloud { get { return type; } }
        private CloudName type = CloudName.LocalDisk;

        public bool PathIsCloud { get { return path_is_cloud; } }
        private bool path_is_cloud = false;

        public bool PathIsUrl { get { return IsUrl(path_raw); } }

        public string Parent { get { return path_parent; } }
        private string path_parent = "";

        public string NameLastItem { get { return namelastitem; } }
        private string namelastitem = "";

        public string ID { get { return id; } }
        private string id = "";

        public bool IsFolder { get { return isFolder; } set { isFolder = value; } }
        private bool isFolder = true;
        #endregion

        #region const
        private const string Rg_cloud = "^\\w+:(\\w|\\.|@)+",//GoogleDrive:tqk2811@gmail.com
        Rg_cloudname = "^\\w+",//GoogleDrive[:
        Rg_email = "(?<=:)(\\w|\\.)+@(\\w|\\.)+",// :]tqk2.sa_dsa@abc.com 
        Rg_pathcloud = "\\/.+$", // GoogleDrive:tqk2811@gmail.com]/abc/def/ad
        Rg_lastitem = "((?!\\/)\\S)+$",//ad
        Rg_url_GD = "^https:\\/\\/drive\\.google\\.com",
        Rg_url_idfolder = "(?<=drive\\/folders\\/)[A-Z0-9]\\w+",
        Rg_url_idfolderopen = "(?<=\\?id\\=)[A-Z0-9]\\w+",
        Rg_url_idfile = "(?<=file\\/d\\/)[A-Z0-9]\\w+",
        Rg_cloud_id = "(?<=id=)\\w+",
        Rg_cloud_query = "(?<=q=).+",
        Rg_Disk_lastitem = "(?<=\\\\)(.(?!\\\\))+$";

        #endregion

        #region Static 
        public static List<string> GetPathTo(List<string> pathform, AnalyzePath rootfrom, AnalyzePath rootto)
        {
            List<string> pathto = new List<string>();
            foreach (string from in pathform)
            {
                pathto.Add(GetPathTo(from, rootfrom, rootto));
            }
            return pathto;
        }
        /// <summary>
        /// If rootfrom is url -> pathfrom format: IDparent/filename
        /// </summary>
        /// <param name="pathfrom">/abc/def.txt or \abc\def.txt or IDparent/folder.../filename (rootfrom is url)</param>
        /// <param name="rootfrom"></param>
        /// <param name="rootto"></param>
        /// <returns></returns>
        public static string GetPathTo(string pathfrom, AnalyzePath rootfrom, AnalyzePath rootto)
        {
            string temp = rootto.PathIsCloud ? "/" : "\\";
            string de_temp = rootto.PathIsCloud ? "\\" : "/";
            // if from is url
            if (IsUrl(rootfrom.Path_Raw) & rootfrom.TypePath == TypePath.UrlFolder)// id/folder.../filename.txt, Url, E:\testfolder
            {
                string[] pathfrom_arr = pathfrom.Split('/');
                if (pathfrom_arr[0] != rootfrom.ID) throw new Exception("id:" + pathfrom_arr[0] +" not indexof " + rootfrom.Path_Raw);// check id
                string temppath = pathfrom.Remove(0, pathfrom_arr[0].Length + 1);// remove id
                return RemoveDup((rootto.path_raw + temp + temppath).Replace(de_temp, temp), temp); // ->E:\testfolder\folder...\filename.txt
            }

            if (IsUrl(rootfrom.Path_Raw) & rootfrom.TypePath == TypePath.UrlFile)// id/folder.../filename.txt, Url, E:\testfolder
            {
                string[] pathfrom_arr = pathfrom.Split('/');
                if (pathfrom_arr[0] != rootfrom.ID) throw new Exception("id:" + pathfrom_arr[0] + " not indexof " + rootfrom.Path_Raw);// check id
                if (pathfrom_arr.Length != 2) throw new Exception("pathfrom format error: " + pathfrom);
                return RemoveDup((rootto.path_raw + temp + pathfrom_arr[1]).Replace(de_temp,temp), temp);
            }   
            
            //if from root folder (cloud)
            if (string.IsNullOrEmpty(rootfrom.GetPath()))// Ex: /abc.txt ,  , E:\newfolder
            {
                return RemoveDup((rootto.Path_Raw + temp + pathfrom).Replace(de_temp, temp), temp);// -> E:\newfolder\abc.txt
            }
            //not root folder
            if (pathfrom.IndexOf(rootfrom.GetPath()) >= 0)// Ex: /folder/abc.txt, /folder, E:\newfolder
            {

                string newpath = rootto.Path_Raw + temp + pathfrom.Remove(0,rootfrom.GetPath().Length).Replace(de_temp, temp);// ->E:\newfolder\abc.txt
                newpath = RemoveDup(newpath, temp);

                #region create folder in Disk
                if (!IsCloud(newpath))
                {
                    FileInfo info = new FileInfo(newpath);
                    if (info.Exists)
                    {
                        string extension = info.Extension;
                        string filename = info.Name.Remove(info.Name.Length - extension.Length, extension.Length);
                        string newfilepatch = "";
                        FileInfo info_;
                        int i = 1;
                        while (true)
                        {
                            newfilepatch = info.DirectoryName + "\\" + filename + " (" + i.ToString() + ")" + extension;
                            info_ = new FileInfo(newfilepatch);
                            if (info_.Exists) { i++; continue; }
                            else { newpath = newfilepatch; break; }
                        }
                    }
                }
                #endregion

                return newpath;
            }

            throw new Exception(pathfrom + " not index of " + rootfrom.Path_Raw);
        }

        static string RemoveDup(string newpath,string temp)
        {
            while (newpath.IndexOf(temp + temp) >= 0)
            {
                newpath = newpath.Replace(temp + temp, temp);
            }
            return newpath;
        }
        public static bool IsCloud(string path)
        {
            if (path.IndexOf("\\") >= 0)
            {
                string[] pathsplit = path.TrimStart('\\').Split('\\');
                if (pathsplit.Length > 0 && pathsplit[0].IndexOf('@') < 0) return false;
            }
            if (path.IndexOf('/') >= 0 & path.IndexOf('\\') < 0) return true;
            if (!string.IsNullOrEmpty(path) && path.Length > 1 && path[path.Length - 1] == ':') return false;
            if (path.IndexOf('@') > 0) return true;
            throw new Exception("Path error: " + path);
        }

        public static bool IsUrl(string path)
        {
            if (path.IndexOf("http") >= 0) return true;
            return false;
        }
        #endregion

        #region Method
        public AnalyzePath(string path)
        {
            path_raw = path.TrimEnd('/').TrimEnd('\\').Replace("\\\\","\\");
            path_is_cloud = IsCloud(path_raw);
            if (!IsUrl(path)) path_raw = path_raw.Replace("//", "/");
            GetPath_();
        }

        private void GetDiskPath()
        {
            if (path_raw.Length == 2) path_raw += "\\";
            Regex rg = new Regex(Rg_Disk_lastitem);
            Match match = rg.Match(path_raw);
            if (match.Success)
            {
                namelastitem = match.Value;
                if(namelastitem.Length>0) path_parent = path_raw.Remove(path_raw.Length - namelastitem.Length, namelastitem.Length).TrimEnd('\\');
            }
        }

        private void GetRealCloudPath()
        {
            Regex rg;
            Match match;

            #region GoogleDrive:abc@gmail.com / Dropbox:abc@yahoo.com
            rg = new Regex(Rg_cloud);
            match = rg.Match(path_raw);
            if (match.Success)
            {
                rg = new Regex(Rg_cloudname);
                match = rg.Match(path_raw);
                if (match.Success) type = (CloudName)Enum.Parse(typeof(CloudName), match.Value); //CloudName

                rg = new Regex(Rg_email);
                match = rg.Match(path_raw);
                if (match.Success) email = match.Value;//email

                rg = new Regex(Rg_pathcloud);
                match = rg.Match(path_raw);
                if (match.Success)
                {
                    pathcloud = match.Value;//path cloud
                    rg = new Regex(Rg_lastitem);
                    match = rg.Match(path_raw);
                    if (match.Success) {
                        namelastitem = match.Value;//name last item
                        if (pathcloud.TrimEnd('/').TrimStart('/').Length != namelastitem.Length)
                        {
                            path_parent = pathcloud.Remove(pathcloud.Length - namelastitem.Length - 1, namelastitem.Length + 1);
                        }
                    }
                    return;
                }
                else
                {
                    rg = new Regex(Rg_cloud_id);// GD:abc@gmail.com?id=ads
                    match = rg.Match(path_raw);
                    if(match.Success)
                    {
                        id = match.Value;
                        pathcloud = path_raw;
                        typepath = TypePath.CloudID;
                        return;
                    }

                    rg = new Regex(Rg_cloud_query);// GD:abc@gmail.com?q='abc' in parent
                    match = rg.Match(path_raw);
                    if(match.Success)
                    {
                        q = match.Value;
                        typepath = TypePath.CloudQuery;
                        pathcloud = path_raw;
                        return;
                    }
                }
                return;
            }
            #endregion

            #region https://drive.google.com
            rg = new Regex(Rg_url_GD);
            match = rg.Match(path_raw);
            if (match.Success)
            {
                type = CloudName.GoogleDrive;

                rg = new Regex(Rg_url_idfolder);
                match = rg.Match(path_raw);
                if (match.Success)
                {
                    id = match.Value;
                    pathcloud = path_raw;
                    typepath = TypePath.UrlFolder;
                    return;
                }

                rg = new Regex(Rg_url_idfolderopen);
                match = rg.Match(path_raw);
                if (match.Success)
                {
                    this.id = match.Value;
                    pathcloud = path_raw;
                    typepath = TypePath.UrlFolder;
                    return;
                }

                rg = new Regex(Rg_url_idfile);
                match = rg.Match(path_raw);
                if (match.Success)
                {
                    this.id = match.Value;
                    isFolder = false;
                    pathcloud = path_raw;
                    typepath = TypePath.UrlFile;
                    return;
                }
            }
            #endregion
            throw new Exception("Path error: " + path_raw);
        }

        public string GetExtensionFile()
        {
            if (NameLastItem.IndexOf('.') >= 0)
            {
                string[] temp_arr = NameLastItem.Split('.');
                return temp_arr[temp_arr.Length - 1];
            }
            return NameLastItem;
        }

        public string GetPath()
        {
            return (path_is_cloud ? pathcloud : path_raw);
        }
        private void GetPath_()
        {
            if (path_is_cloud) GetRealCloudPath();
            else GetDiskPath();
        }

        public string ReplaceIDUrl(string newid)
        {
            return pathcloud.Replace(id, newid);
        }

        public string AddRawChildPath(string name)
        {
            return path_raw + (IsCloud(path_raw) ? "/" : "\\") + name;
        }

        public AnalyzePath GetParent()
        {
            if (string.IsNullOrEmpty(Parent)) return null;
            else
            {
                return new AnalyzePath(Parent);
            }
        }
        #endregion
    }
}
