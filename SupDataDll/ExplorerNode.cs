using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
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
        ExplorerNode parent;
        [JsonIgnore]
        NodeInfo info;
        [JsonIgnore]
        RootNode rootinfo;

        [JsonIgnore] 
        public List<ExplorerNode> Child
        {
            get { return child ?? (child = new List<ExplorerNode>()); }
            private set { child = value; }
        }

        [JsonProperty]
        public ExplorerNode Parent
        {
            get { return parent; }
            set { parent = value; value.child.Add(this); }
        }
        [JsonProperty]
        public NodeInfo Info { get { return info ?? (info = new NodeInfo()); } set { info = value; } }
        [JsonProperty]
        public RootNode RootInfo { get { return rootinfo ?? (rootinfo = new RootNode()); } set { rootinfo = value; } }
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

        public void RenewChilds(List<ExplorerNode> childs)
        {
            this.Child.Clear();
            childs.ForEach(c => AddChild(c));
        }

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
        
        public string GetFullPathString(bool CloudExplorerType = true)
        {
            List<ExplorerNode> fullpathlist = GetFullPath();
            string path = "";
            switch(GetRoot().RootInfo.Type)
            {
                case CloudType.LocalDisk:
                    fullpathlist.ForEach(i => path += i.Info.Name + "\\");
                    path.TrimEnd('\\');
                    break;
                case CloudType.Dropbox:
                case CloudType.GoogleDrive:
                case CloudType.Mega:
                    fullpathlist.RemoveAt(0);
                    if (CloudExplorerType) path = GetRoot().RootInfo.Type.ToString() + ":" + GetRoot().RootInfo.Email;
                    fullpathlist.ForEach(i => path += "/" + RemoveSpecialChar(i.Info.Name));
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

        public string GetExtension()
        {
            string[] splitPath = this.Info.Name.Split(new Char[] { '.' });
            string extension = (string)splitPath.GetValue(splitPath.GetUpperBound(0));
            if (string.IsNullOrEmpty(extension)) extension = this.Info.Name;
            return extension;
        }
        #endregion

        #region Static
        [JsonIgnore]
        static List<char> listcannot = new List<char>() { '/', '\\', ':', '?', '*', '"', '<', '>', '|' };
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
        public static string RemoveSpecialChar(string input)
        {
            listcannot.ForEach(c => input = input.Replace(c, '_'));
            return input;
        }
        #endregion
    }


    public class NodeInfo
    {
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
        public CloudType Type { get; set; }
    }
}
