using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SupDataDll.Class.Mega;

namespace SupDataDll
{
   public class ManagerExplorerNodes
    {
        public ExplorerNode Root { get; set; }

        int index = -1;
        List<ExplorerNode> nodes = new List<ExplorerNode>();

        public ExplorerNode NodeWorking()
        {
            if (index == -1 || nodes.Count == 0) return null;
            else if (nodes.Count < index + 1) return null;
            else return nodes[index];
        }

        public ExplorerNode Back()
        {
            if (index > 0) { index--; return NodeWorking(); }
            else { nodes.Clear(); index = -1; return null; }
        }

        public ExplorerNode Next(ExplorerNode next = null)
        {
            if(next != null && next.GetRoot() == Root)
            {
                if (nodes.Count - 1 > index) nodes.RemoveRange(index + 1, nodes.Count - 1 - index);
                nodes.Add(next);
            }
            if (index < nodes.Count - 1) index++;
            else return null;
            return NodeWorking();
        }
    }

    public class ExplorerNode
    {
        public ExplorerNode() { }
        public ExplorerNode(RootNode root)
        {
            this.RootInfo = root;
        }
        public ExplorerNode(NodeInfo info)
        {
            this.Info = info;
        }
        public ExplorerNode(NodeInfo info, ExplorerNode parent)
        {
            this.Info = info;
            parent.AddChild(this);
        }

        
        #region field
        [JsonIgnore]
        List<ExplorerNode> child;
        [JsonIgnore]
        public List<ExplorerNode> Child
        {
            get { return child ?? (child = new List<ExplorerNode>()); }
            private set { child = value; }
        }

        [JsonIgnore]
        public ExplorerNode Parent
        {
            get { return parent; }
            set { parent = value; value.child.Add(this); }
        }
        [JsonIgnore]
        public NodeInfo Info { get { return info ?? (info = new NodeInfo()); } set { info = value; } }
        [JsonIgnore]
        public RootNode RootInfo { get { return rootinfo ?? (rootinfo = new RootNode()); } set { rootinfo = value; } }
        

        [JsonProperty]
        ExplorerNode parent;
        [JsonProperty]
        NodeInfo info;
        [JsonProperty]
        RootNode rootinfo;
        #endregion

        
        #region public method
        public void RemoveChild(ExplorerNode child)
        {
            this.Child.Remove(child);
            child.Parent = null;
        }

        public void AddChild(ExplorerNode child)
        {
            this.Child.Add(child);
            child.parent = this;
        }

        /// <summary>
        /// Clean all child, and add new Childs
        /// </summary>
        /// <param name="childs"></param>
        public void RenewChilds(List<ExplorerNode> childs)
        {
            this.Child.Clear();
            childs.ForEach(c => AddChild(c));
        }

        /// <summary>
        /// Get List Node from root to this.
        /// </summary>
        /// <returns></returns>
        public List<ExplorerNode> GetFullPath()
        {
            List<ExplorerNode> list = new List<ExplorerNode>();
            list.Add(this);
            ExplorerNode parent = this.parent;
            while (parent != null)
            {
                list.Insert(0, parent);
                parent = parent.Parent;
            }
            return list;
        }

        /// <summary>
        /// Get string path from Node.
        /// </summary>
        /// <param name="CloudExplorerType">Add [CloudType]:[Email] to head path string (Cloud only)</param>
        /// <param name="RemoveSpecialCharacter">For Dropbox or LocalDisk only</param>
        /// <returns></returns>
        public string GetFullPathString(bool CloudExplorerType = true,bool RemoveSpecialCharacter = false)
        {
            List<ExplorerNode> fullpathlist = GetFullPath();
            string path = "";
            switch(GetRoot().RootInfo.Type)
            {
                case CloudType.LocalDisk:
                    fullpathlist.ForEach(i => path += i.Info.Name + "\\");
                    while (path[path.Length - 1] == '\\') path = path.TrimEnd('\\');
                    if (path.IndexOf('\\') == -1) path += "\\";
                    break;
                case CloudType.Dropbox:
                case CloudType.GoogleDrive:
                case CloudType.Mega:
                    fullpathlist.RemoveAt(0);
                    if (CloudExplorerType) path = GetRoot().RootInfo.Type.ToString() + ":" + GetRoot().RootInfo.Email;
                    fullpathlist.ForEach(i => path += "/" + (RemoveSpecialCharacter ? RemoveSpecialChar(i.Info.Name) : i.Info.Name));
                    break;
                default:throw new Exception("Other cloud not support that type.");
            }
            return path;
        }

        public ExplorerNode MakeNodeTo(ExplorerNode RootFrom, ExplorerNode RootTo)
        {
            List<ExplorerNode> FullPathRootFrom = RootFrom.GetFullPath();
            List<ExplorerNode> NodeFullPath = this.GetFullPath();
            CloudType type_rootto = RootTo.GetRoot().RootInfo.Type;
            for (int i = NodeFullPath.IndexOf(RootFrom) + 1; i < NodeFullPath.Count; i++)
            {
                ExplorerNode node = new ExplorerNode();
                node.Info.Size = NodeFullPath[i].Info.Size;
                node.Info.Name = (type_rootto == CloudType.LocalDisk || type_rootto == CloudType.Dropbox) ? RemoveSpecialChar(NodeFullPath[i].Info.Name) : NodeFullPath[i].Info.Name;
                RootTo.AddChild(node);
                RootTo = node;
            }
            return RootTo;
        }
        
        public ExplorerNode GetRoot()
        {
            return GetFullPath()[0];
        }

        /// <summary>
        /// Get File Extension
        /// </summary>
        /// <returns></returns>
        public string GetExtension()
        {
            string[] splitPath = this.Info.Name.Split(new Char[] { '.' });
            string extension = (string)splitPath.GetValue(splitPath.GetUpperBound(0));
            if (string.IsNullOrEmpty(extension)) extension = this.Info.Name;
            return extension;
        }
        
        public ExplorerNode FindSameParent(ExplorerNode othernode)
        {
            List<ExplorerNode> list_other = othernode.GetFullPath();
            List<ExplorerNode> list_this = GetFullPath();
            ExplorerNode node = null;
            int max = list_other.Count <= list_this.Count ? list_other.Count : list_this.Count;
            for (int i = 0; i < max; i++)
            {
                if (list_other[i] == list_this[i]) node = list_this[i];
                else break;
            }
            return node;
        }
        #endregion

        #region Static
        [JsonIgnore]
        static List<char> listcannot = new List<char>() { '/', '\\', ':', '?', '*', '"', '<', '>', '|' };

        /// <summary>
        /// Auto Create Node From Path In Disk
        /// </summary>
        /// <param name="path"></param>
        /// <param name="size"> <1 is folder</param>
        /// <returns></returns>
        public static ExplorerNode GetNodeFromDiskPath(string path,long size = -1)
        {
            string[] path_split = path.Split('\\');

            ExplorerNode parent = new ExplorerNode();
            parent.RootInfo.Type = CloudType.LocalDisk;
            parent.Info.Name = path_split[0];
            for (int i = 1; i< path_split.Length;i++)
            {
                ExplorerNode node = new ExplorerNode();
                node.Info.Name = path_split[i];
                parent.AddChild(node);
                parent = node;
            }
            if (size > 0) parent.Info.Size = size;
            return parent;
        }

        /// <summary>
        /// Replace { '/', '\\', ':', '?', '*', '"', '<', '>', '|' }  to chrReplaceTo
        /// </summary>
        /// <param name="input"></param>
        /// <param name="chrReplaceTo"></param>
        /// <returns></returns>
        public static string RemoveSpecialChar(string input,char chrReplaceTo = '_')
        {
            listcannot.ForEach(c => input = input.Replace(c, chrReplaceTo));
            return input;
        }
        #endregion
    }


    public class NodeInfo
    {
        public MegaKeyCrypto MegaCrypto { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
        public DateTime DateMod { get; set; }
        public long Size { get; set; }
        public string MimeType { get; set; }
        public Permission permission { get; set; }
        public string GetExtensionFile()
        {
            if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException("Name");
            if (Name.IndexOf('.') >= 0)
            {
                string[] temp_arr = Name.Split('.');
                return temp_arr[temp_arr.Length - 1];
            }
            else return Name;
        }
    }

    public class MegaKeyCrypto : INodeCrypto
    {
        public MegaKeyCrypto()
        {

        }
        public MegaKeyCrypto(INodeCrypto CryptoKey)
        {
            if (CryptoKey == null) return;
            this.key = CryptoKey.Key;
            this.sharedkey = CryptoKey.SharedKey;
            this.iv = CryptoKey.Iv;
            this.metamac = CryptoKey.MetaMac;
            this.fullkey = CryptoKey.FullKey;
        }
        byte[] key;
        public byte[] Key
        {
            get
            {
                return key;
            }
        }
        byte[] sharedkey;
        public byte[] SharedKey
        {
            get
            {
                return sharedkey;
            }
        }
        byte[] iv;
        public byte[] Iv
        {
            get
            {
                return iv;
            }
        }
        byte[] metamac;
        public byte[] MetaMac
        {
            get
            {
                return metamac;
            }
        }
        byte[] fullkey;
        public byte[] FullKey
        {
            get
            {
                return fullkey;
            }
        }
    }

    public class RootNode: CloudEmail_Type
    {
        public Uri uri { get; set; }
    }

    public enum Permission
    {
        Owner = 0,
        Read,Write
    }
    public class CloudEmail_Type
    {
        public string Email { get; set; }
        CloudType type = CloudType.Folder;
        public CloudType Type { get { return type; } set { type = value; } }
    }
}
