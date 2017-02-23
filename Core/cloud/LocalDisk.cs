using SupDataDll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Core.Cloud
{
    internal static class LocalDisk
    {
        public static ExplorerNode GetListFileFolder(ExplorerNode node)
        {
            string path = node.GetFullPathString();
            node.Child.Clear();
            foreach (string item in Directory.GetDirectories(path))
            {
                DirectoryInfo info = new DirectoryInfo(item);
                if (CheckAttribute(info.Attributes, FileAttributes.System) | CheckAttribute(info.Attributes, FileAttributes.Offline)) continue;
                ExplorerNode f = new ExplorerNode();
                f.Info.Name = info.Name;
                f.Info.Size = -1;
                f.Info.DateMod = info.LastWriteTimeUtc;
                node.AddChild(f);
            }
            foreach (string item in Directory.GetFiles(path))
            {
                FileInfo info = new FileInfo(item);
                if (CheckAttribute(info.Attributes, FileAttributes.System) | CheckAttribute(info.Attributes, FileAttributes.Offline)) continue;
                ExplorerNode f = new ExplorerNode();
                f.Info.Name = info.Name;
                f.Info.Size = info.Length;
                f.Info.DateMod = info.LastWriteTimeUtc;
                node.AddChild(f);
            }
            return node;
        }

        static bool CheckAttribute(FileAttributes Item,FileAttributes compare)
        {
            return (Item & compare) == compare;
        }


        public static Stream GetFileSteam(ExplorerNode node, bool GetfileForUpload,long Startpos)
        {
            string path = node.GetFullPathString();
            FileInfo info = new FileInfo(path);
            FileStream fs;
            
            if (GetfileForUpload)
            {
                if (!info.Exists) throw new FileNotFoundException("File not found",path);
                if (info.Length == 0) throw new Exception("File size = 0");
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                fs.Seek(Startpos, SeekOrigin.Begin);
                return fs;
            }
            else if (!info.Exists)
            {
                List<ExplorerNode> nodelist = node.GetFullPath();
                DirectoryInfo dinfo;
                for (int i = 1;i < nodelist.Count -1;i++ )
                {
                    dinfo = new DirectoryInfo(nodelist[i].GetFullPathString());
                    if (!dinfo.Exists) dinfo.Create();
                }
            }
            fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            fs.Seek(Startpos, SeekOrigin.Begin);
            return fs;
        }

        public static bool Delete(ExplorerNode node, bool PernamentDelete)
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
                return FileOperationAPIWrapper.Send(path);
            }
        }

        public static bool Move(ExplorerNode node, ExplorerNode newparent,string newname = null)
        {
            if (node.GetRoot().RootInfo.Type != CloudType.LocalDisk && newparent.GetRoot().RootInfo.Type != CloudType.LocalDisk) throw new Exception("CloudType is != LocalDisk.");
            string path_from = node.GetFullPathString();
            string path_to = newparent.GetFullPathString() + "\\" + newname == null ? node.Info.Name : newname;
            FileInfo info = new FileInfo(path_from);
            if (info.Exists) { info.MoveTo(path_to); return true; }
            DirectoryInfo dinfo = new DirectoryInfo(path_from);
            if (dinfo.Exists) { dinfo.MoveTo(path_to); return true; }
            return false;
        }

        public static string CreateFolder(ExplorerNode node)
        {
            DirectoryInfo dinfo = new DirectoryInfo(node.GetFullPathString());
            if (!dinfo.Exists) dinfo.Create();
            return dinfo.FullName;
        }
    }

    public class FileOperationAPIWrapper
    {
        /// <summary>
        /// Possible flags for the SHFileOperation method.
        /// </summary>
        [Flags]
        public enum FileOperationFlags : ushort
        {
            /// <summary>
            /// Do not show a dialog during the process
            /// </summary>
            FOF_SILENT = 0x0004,
            /// <summary>
            /// Do not ask the user to confirm selection
            /// </summary>
            FOF_NOCONFIRMATION = 0x0010,
            /// <summary>
            /// Delete the file to the recycle bin.  (Required flag to send a file to the bin
            /// </summary>
            FOF_ALLOWUNDO = 0x0040,
            /// <summary>
            /// Do not show the names of the files or folders that are being recycled.
            /// </summary>
            FOF_SIMPLEPROGRESS = 0x0100,
            /// <summary>
            /// Surpress errors, if any occur during the process.
            /// </summary>
            FOF_NOERRORUI = 0x0400,
            /// <summary>
            /// Warn if files are too big to fit in the recycle bin and will need
            /// to be deleted completely.
            /// </summary>
            FOF_WANTNUKEWARNING = 0x4000,
        }
        /// <summary>
        /// File Operation Function Type for SHFileOperation
        /// </summary>
        public enum FileOperationType : uint
        {
            /// <summary>
            /// Move the objects
            /// </summary>
            FO_MOVE = 0x0001,
            /// <summary>
            /// Copy the objects
            /// </summary>
            FO_COPY = 0x0002,
            /// <summary>
            /// Delete (or recycle) the objects
            /// </summary>
            FO_DELETE = 0x0003,
            /// <summary>
            /// Rename the object(s)
            /// </summary>
            FO_RENAME = 0x0004,
        }
        /// <summary>
        /// SHFILEOPSTRUCT for SHFileOperation from COM
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEOPSTRUCT
        {

            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public FileOperationType wFunc;
            public string pFrom;
            public string pTo;
            public FileOperationFlags fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        /// <summary>
        /// Send file to recycle bin
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        /// <param name="flags">FileOperationFlags to add in addition to FOF_ALLOWUNDO</param>
        public static bool Send(string path, FileOperationFlags flags)
        {
            try
            {
                var fs = new SHFILEOPSTRUCT
                {
                    wFunc = FileOperationType.FO_DELETE,
                    pFrom = path + '\0' + '\0',
                    fFlags = FileOperationFlags.FOF_ALLOWUNDO | flags
                };
                SHFileOperation(ref fs);
                return true;
                //return fs.fAnyOperationsAborted;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Send file to recycle bin.  Display dialog, display warning if files are too big to fit (FOF_WANTNUKEWARNING)
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        public static bool Send(string path)
        {
            return Send(path, FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_WANTNUKEWARNING);
        }

        /// <summary>
        /// Send file silently to recycle bin.  Surpress dialog, surpress errors, delete if too large.
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        public static bool MoveToRecycleBin(string path)
        {
            return Send(path, FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_NOERRORUI | FileOperationFlags.FOF_SILENT);
        }

        private static bool deleteFile(string path, FileOperationFlags flags)
        {
            try
            {
                var fs = new SHFILEOPSTRUCT
                {
                    wFunc = FileOperationType.FO_DELETE,
                    pFrom = path + '\0' + '\0',
                    fFlags = flags
                };
                SHFileOperation(ref fs);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DeleteCompletelySilent(string path)
        {
            return deleteFile(path,
                              FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_NOERRORUI |
                              FileOperationFlags.FOF_SILENT);
        }
    }
}
