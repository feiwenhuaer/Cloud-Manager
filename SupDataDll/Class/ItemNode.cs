using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using CloudManagerGeneralLib.Class.Mega;

namespace CloudManagerGeneralLib.Class
{
   public class ManagerHistoryItemNodes
    {
        public ItemNode Root { get; set; }

        int index = -1;
        List<ItemNode> nodes = new List<ItemNode>();

        public ItemNode NodeWorking()
        {
            if (index == -1 || nodes.Count == 0) return null;
            else if (nodes.Count < index + 1) return null;
            else return nodes[index];
        }

        public ItemNode Back()
        {
            if (index > 0) { index--; return NodeWorking(); }
            else { nodes.Clear(); index = -1; return null; }
        }

        public ItemNode Next(ItemNode next = null)
        {
            if(next != null && next.GetRoot == Root)
            {
                if (nodes.Count - 1 > index) nodes.RemoveRange(index + 1, nodes.Count - 1 - index);
                nodes.Add(next);
            }
            if (index < nodes.Count - 1) index++;
            else return null;
            return NodeWorking();
        }
    }

    public class ItemNode
    {
        public ItemNode() { }
        public ItemNode(TypeNode root)
        {
            this.NodeType = root;
        }
        public ItemNode(NodeInfo info)
        {
            this.Info = info;
        }
        public ItemNode(NodeInfo info, ItemNode parent)
        {
            this.Info = info;
            parent.AddChild(this);
        }


        #region field
        [JsonProperty]
        ItemNode parent;
        [JsonProperty]
        NodeInfo info;
        [JsonProperty]
        TypeNode NodeType_;


        [JsonIgnore]
        List<ItemNode> child;
        [JsonIgnore]
        public List<ItemNode> Child
        {
            get { return child ?? (child = new List<ItemNode>()); }
            private set { child = value; }
        }

        [JsonIgnore]
        public ItemNode Parent
        {
            get { return parent; }
            set
            {
                if (NodeType_ != null) throw new Exception("Root can't get parent.");
                else { parent = value; value.child.Add(this); }
            }
        }
        [JsonIgnore]
        public NodeInfo Info { get { return info ?? (info = new NodeInfo()); } set { info = value; } }
        [JsonIgnore]
        public TypeNode NodeType
        {
            get
            {
                return NodeType_ ?? (NodeType_ = new TypeNode());
            }
            set
            {
                if (parent != null) throw new Exception("Root can't get parent.");
                else NodeType_ = value;
            }
        }
        
        [JsonIgnore]
        List<ItemNode> FullPathArrayNode;
        /// <summary>
        /// Get List Node from root to this.
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public ItemNode GetRoot { get { return GetFullPath()[0]; } }
        #endregion


        #region public method
        public void RemoveChild(ItemNode child)
        {
            this.Child.Remove(child);
            child.Parent = null;
        }

        public void AddChild(ItemNode child)
        {
            this.Child.Add(child);
            child.parent = this;
        }

        /// <summary>
        /// Clean all child, and add new Childs
        /// </summary>
        /// <param name="childs"></param>
        public void RenewChilds(List<ItemNode> childs)
        {
            this.Child.Clear();
            childs.ForEach(c => AddChild(c));
        }

        public List<ItemNode> GetFullPath()
        {
            List<ItemNode> FullPathArrayNode = new List<ItemNode>();
            FullPathArrayNode.Add(this);
            ItemNode parent = this.parent;
            while (parent != null)
            {
                FullPathArrayNode.Insert(0, parent);
                parent = parent.Parent;
            }
            return FullPathArrayNode;
        }
        
        /// <summary>
        /// Get string path from Node.
        /// </summary>
        /// <param name="CloudExplorerType">Add [CloudType]:[Email] to head path string (Cloud only)</param>
        /// <param name="RemoveSpecialCharacter">For Dropbox or LocalDisk only</param>
        /// <returns></returns>
        public string GetFullPathString(bool CloudExplorerType = true,bool RemoveSpecialCharacter = false)
        {
            List<ItemNode> fullpathlist = GetFullPath();
            string path = "";
            switch(GetRoot.NodeType.Type)
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
                    if (CloudExplorerType) path = GetRoot.NodeType.Type.ToString() + ":" + GetRoot.NodeType.Email;
                    fullpathlist.ForEach(i => path += "/" + (RemoveSpecialCharacter ? RemoveSpecialChar(i.Info.Name) : i.Info.Name));
                    break;
                default:throw new Exception("Other cloud not support that type.");
            }
            return path;
        }

        public ItemNode MakeNodeTo(ItemNode RootFrom, ItemNode RootTo)
        {
            List<ItemNode> FullPathRootFrom = RootFrom.GetFullPath();
            List<ItemNode> NodeFullPath = this.GetFullPath();
            CloudType type_rootto = RootTo.GetRoot.NodeType.Type;
            for (int i = NodeFullPath.IndexOf(RootFrom) + 1; i < NodeFullPath.Count; i++)
            {
                ItemNode node = new ItemNode();
                node.Info.Size = NodeFullPath[i].Info.Size;
                node.Info.Name = (type_rootto == CloudType.LocalDisk || type_rootto == CloudType.Dropbox) ? RemoveSpecialChar(NodeFullPath[i].Info.Name) : NodeFullPath[i].Info.Name;
                RootTo.AddChild(node);
                RootTo = node;
            }
            return RootTo;
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
        
        public ItemNode FindSameParent(ItemNode othernode)
        {
            List<ItemNode> list_other = othernode.GetFullPath();
            List<ItemNode> list_this = GetFullPath();
            ItemNode node = null;
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
        public static ItemNode GetNodeFromDiskPath(string path,long size = -1)
        {
            string[] path_split = path.Split('\\');

            ItemNode parent = new ItemNode();
            parent.NodeType.Type = CloudType.LocalDisk;
            parent.Info.Name = path_split[0];
            for (int i = 1; i< path_split.Length;i++)
            {
                ItemNode node = new ItemNode();
                node.Info.Name = path_split[i];
                parent.AddChild(node);
                parent = node;
            }
            if (size >= 0) parent.Info.Size = size;
            else
            {
                System.IO.FileInfo info = new System.IO.FileInfo(path);
                if (info.Exists) parent.Info.Size = info.Length;
            }
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
        public byte[] Hash { get; set; }
        public HashType TypeHash { get; set; }
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

    public enum HashType
    {
        None = 0,
        md5
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

    public class TypeNode
    {
        public string Email { get; set; }
        [JsonIgnore]
        CloudType type = CloudType.Folder;
        public CloudType Type { get { return type; } set { type = value; } }
        public Uri uri { get; set; }
    }

    public enum Permission
    {
        Owner = 0,
        Read,Write
    }
}
