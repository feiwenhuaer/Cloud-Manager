using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using CloudManagerGeneralLib.Class.Mega;
using System.ComponentModel;

namespace CloudManagerGeneralLib.Class
{
  public class ManagerHistoryItemNodes
  {
    public RootNode Root { get; set; }

    int index = -1;
    List<IItemNode> nodes = new List<IItemNode>();

    public IItemNode NodeWorking()
    {
      if (index == -1 || nodes.Count == 0) return null;
      else if (nodes.Count < index + 1) return null;
      else return nodes[index];
    }

    public IItemNode Back()
    {
      if (index > 0) { index--; return NodeWorking(); }
      else { nodes.Clear(); index = -1; return null; }
    }

    public IItemNode Next(IItemNode next = null)
    {
      if (next != null && next.GetRoot == Root)
      {
        if (nodes.Count - 1 > index) nodes.RemoveRange(index + 1, nodes.Count - 1 - index);
        nodes.Add(next);
      }
      if (index < nodes.Count - 1) index++;
      else return null;
      return NodeWorking();
    }
  }

  public interface IItemNode
  {
    List<IItemNode> Childs { get; }
    NodeInfo Info { get; set; }
    [JsonIgnore]
    IItemNode Parent { get; set; }
    RootNode GetRoot { get; }

    void RemoveChild(IItemNode child);
    void AddChild(IItemNode child);
    void RenewChilds(List<IItemNode> childs);
    List<IItemNode> GetFullPath();
    string GetFullPathString(bool CloudExplorerType = true, bool RemoveSpecialCharacter = false);
    IItemNode MakeNodeTo(IItemNode RootFrom, IItemNode RootTo);
    string GetExtension();
    IItemNode FindSameParent(IItemNode OtherNode);
  }

  public class ItemNode : IItemNode
  {
    public ItemNode() { }
    public ItemNode(NodeInfo info)
    {
      this.Info = info;
    }
    public ItemNode(NodeInfo info, IItemNode parent)
    {
      this.Info = info;
      parent.AddChild(this);
    }

    #region Field
    [JsonIgnore]
    NodeInfo info;
    [JsonIgnore]
    List<IItemNode> childs;
    [JsonIgnore]
    IItemNode parent;
    [JsonIgnore]
    public virtual IItemNode Parent
    {
      get { return parent; }
      set
      {
        parent = value;
        if (value != null) value.Childs.Add((IItemNode)this);
      }
    }
    /// <summary>
    /// Get List Node from root to this.
    /// </summary>
    /// <returns></returns>
    [JsonIgnore]
    public virtual RootNode GetRoot { get { return GetFullPath()[0] as RootNode; } }


    [JsonProperty]
    public NodeInfo Info { get { return info ?? (info = new NodeInfo()); } set { info = value; } }
    [JsonProperty]
    public List<IItemNode> Childs
    {
      get { return childs ?? (childs = new List<IItemNode>()); }
      private set { childs = value; }
    }
    #endregion

    #region Method
    public void RemoveChild(IItemNode child)
    {
      this.Childs.Remove(child);
      child.Parent = null;
    }
    public void AddChild(IItemNode child)
    {
      //this.Child.Add(child);
      child.Parent = (IItemNode)this;
    }
    /// <summary>
    /// Clean all child, and add new Childs
    /// </summary>
    /// <param name="childs"></param>
    public void RenewChilds(List<IItemNode> childs)
    {
      this.Childs.Clear();
      childs.ForEach(c => AddChild(c));
    }
    public List<IItemNode> GetFullPath()
    {
      List<IItemNode> FullPathArrayNode = new List<IItemNode>();
      FullPathArrayNode.Add((IItemNode)this);
      IItemNode parent = this.parent;
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
    public string GetFullPathString(bool CloudExplorerType = true, bool RemoveSpecialCharacter = false)
    {
      List<IItemNode> fullpathlist = GetFullPath();
      RootNode root = (fullpathlist[0] as RootNode);
      string path = "";
      switch (root.RootType.Type)
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
          if (CloudExplorerType) path = root.RootType.Type.ToString() + ":" + root.RootType.Email;
          fullpathlist.ForEach(i => path += "/" + (RemoveSpecialCharacter ? RemoveSpecialChar(i.Info.Name) : i.Info.Name));
          break;
        default: throw new Exception("Other cloud not support that type.");
      }
      return path;
    }
    public IItemNode MakeNodeTo(IItemNode RootFrom, IItemNode RootTo)
    {
      List<IItemNode> FullPathRootFrom = RootFrom.GetFullPath();
      List<IItemNode> NodeFullPath = this.GetFullPath();
      CloudType type_rootto = RootTo.GetRoot.RootType.Type;
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
    public IItemNode FindSameParent(IItemNode othernode)
    {
      List<IItemNode> list_other = othernode.GetFullPath();
      List<IItemNode> list_this = GetFullPath();
      IItemNode node = null;
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
    public static IItemNode GetNodeFromDiskPath(string path, long size = -1)
    {
      string[] path_split = path.Split('\\');

      IItemNode parent = new RootNode();
      ((RootNode)parent).RootType.Type = CloudType.LocalDisk;
      parent.Info.Name = path_split[0];
      for (int i = 1; i < path_split.Length; i++)
      {
        IItemNode node = new ItemNode();
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
    public static string RemoveSpecialChar(string input, char chrReplaceTo = '_')
    {
      listcannot.ForEach(c => input = input.Replace(c, chrReplaceTo));
      return input;
    }
    #endregion
  }

  public class RootNode : ItemNode
  {
    public RootNode() { }
    public RootNode(TypeNode RootType)
    {
      this.RootType = RootType;
    }
    public RootNode(TypeNode RootType, NodeInfo info)
    {
      this.RootType = RootType;
      this.Info = info;
    }

    [JsonIgnore]
    TypeNode RootType_;
    [JsonProperty]
    public TypeNode RootType
    {
      get
      {
        return RootType_ ?? (RootType_ = new TypeNode());
      }
      set
      {
        RootType_ = value;
      }
    }

    [JsonIgnore]
    public override IItemNode Parent
    {
      get
      {
        return null;
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    [JsonIgnore]
    public override RootNode GetRoot
    {
      get
      {
        return this;
      }
    }
  }

  public class NodeInfo : INotifyPropertyChanged
  {
    string name;


    public MegaKeyCrypto MegaCrypto { get; set; }
    public string Name { get { return name; } set { name = value; NotifyPropertyChange("Name"); } }
    public string ID { get; set; }
    public DateTime DateMod { get; set; }
    public long Size { get; set; }
    public string MimeType { get; set; }
    public Permission permission { get; set; }
    public byte[] Hash { get; set; }
    public HashType TypeHash { get; set; }


    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChange(string name)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
    #endregion
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
    Read, Write
  }
}
