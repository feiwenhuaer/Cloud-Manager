using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;
using System;
using System.Collections.Generic;
using System.IO;

namespace Core.CloudSubClass
{
    internal static class LocalDisk
    {
        public static IItemNode GetListFileFolder(IItemNode node)
        {
            string path = node.GetFullPathString();
            node.Childs.Clear();
            foreach (string item in Directory.GetDirectories(path))
            {
                DirectoryInfo info = new DirectoryInfo(item);
                if (CheckAttribute(info.Attributes, FileAttributes.System) | CheckAttribute(info.Attributes, FileAttributes.Offline)) continue;
                IItemNode f = new ItemNode();
                f.Info.Name = info.Name;
                f.Info.Size = -1;
                f.Info.DateMod = info.LastWriteTime;
                node.AddChild(f);
            }
            foreach (string item in Directory.GetFiles(path))
            {
                FileInfo info = new FileInfo(item);
                if (CheckAttribute(info.Attributes, FileAttributes.System) | CheckAttribute(info.Attributes, FileAttributes.Offline)) continue;
                IItemNode f = new ItemNode();
                f.Info.Name = info.Name;
                f.Info.Size = info.Length;
                f.Info.DateMod = info.LastWriteTime;
                node.AddChild(f);
            }
            return node;
        }

        static bool CheckAttribute(FileAttributes Item,FileAttributes compare)
        {
            return (Item & compare) == compare;
        }


        public static Stream GetFileSteam(IItemNode node, bool GetfileForUpload,long Startpos = 0)
        {
            string path = node.GetFullPathString();
            FileInfo info = new FileInfo(path);
            FileStream fs;
            
            if (GetfileForUpload)
            {
                if (!info.Exists) throw new FileNotFoundException("File not found",path);
                if (info.Length == 0) throw new Exception("File size = 0");
                fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                if(Startpos >=0) fs.Seek(Startpos, SeekOrigin.Begin);
                return fs;
            }
            else if (!info.Exists)
            {
                List<IItemNode> nodelist = node.GetFullPath();
                DirectoryInfo dinfo;
                for (int i = 1;i < nodelist.Count -1;i++ )
                {
                    dinfo = new DirectoryInfo(nodelist[i].GetFullPathString());
                    if (!dinfo.Exists) dinfo.Create();
                }
            }
            fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            if (Startpos >= 0) fs.Seek(Startpos, SeekOrigin.Begin);
            return fs;
        }

        public static bool Delete(ItemNode node, bool PernamentDelete)
        {
            string path = node.GetFullPathString();
            if (PernamentDelete)//delete
            {
                FileInfo info = new FileInfo(path);
                if (info.Exists)
                {
                    info.Delete();
                    return true;
                }
                else
                {
                    DirectoryInfo dinfo = new DirectoryInfo(path);
                    if (dinfo.Exists)
                    {
                        dinfo.Delete(true);
                        return true;
                    }
                }
                return false;
            }
            else//trash
            {
                return FileOperationAPIWrapper.SendRecycleBin(path);
            }
        }

        public static bool Move(IItemNode node, IItemNode newparent,string newname = null)
        {
            if (node.GetRoot.RootType.Type != CloudType.LocalDisk && newparent.GetRoot.RootType.Type != CloudType.LocalDisk) throw new Exception("CloudType is != LocalDisk.");
            string path_from = node.GetFullPathString();
            string path_to = newparent.GetFullPathString() + "\\" + newname == null ? node.Info.Name : newname;
            FileInfo info = new FileInfo(path_from);
            if (info.Exists) { info.MoveTo(path_to); return true; }
            DirectoryInfo dinfo = new DirectoryInfo(path_from);
            if (dinfo.Exists) { dinfo.MoveTo(path_to); return true; }
            return false;
        }

        public static void CreateFolder(IItemNode node)
        {
            DirectoryInfo dinfo = new DirectoryInfo(node.GetFullPathString());
            if (!dinfo.Exists) dinfo.Create();
        }

        public static void AutoCreateFolder(IItemNode node_folder_target)
        {
            List<IItemNode> nodelist = node_folder_target.GetFullPath();
            DirectoryInfo dinfo;
            for (int i = 1; i < nodelist.Count; i++)
            {
                dinfo = new DirectoryInfo(nodelist[i].GetFullPathString());
                if (!dinfo.Exists) dinfo.Create();
            }
        }

        public static IItemNode GetFileInfo(IItemNode node)
        {
            FileInfo info = new FileInfo(node.GetFullPathString());
            if(info.Exists)
            {
                node.Info.Size = info.Length;
                node.Info.DateMod = info.LastWriteTimeUtc;
                node.Info.permission = info.IsReadOnly ? Permission.Read : Permission.Owner;
            }
            return node;
        }
    }
}
