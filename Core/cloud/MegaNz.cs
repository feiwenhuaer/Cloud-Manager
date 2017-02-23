using Cloud.MegaNz;
using SupDataDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Core.StaticClass;
using System.Threading;

namespace Core.Cloud
{
    internal static class MegaNz
    {
        public static MegaApiClient GetClient(string email)
        {
            MegaApiClient client = new MegaApiClient();
            MegaApiClient.AuthInfos authinfo = JsonConvert.DeserializeObject<MegaApiClient.AuthInfos>(AppSetting.settings.GetToken(email, CloudType.Mega));
            client.Login(authinfo);
            return client;
        }

        static INode GetRoot(string Email,NodeType type)
        {
            switch (type)
            {
                case NodeType.Directory:
                case NodeType.File:
                case NodeType.Inbox:
                    throw new Exception("That isn't root.");
                case NodeType.Root:
                case NodeType.Trash:
                    MegaApiClient client = GetClient(Email);
                    IEnumerable<INode> node = client.GetNodes();
                    foreach (INode n in node)
                    {
                        if (n.Type == NodeType.Root) return n;
                    }
                    break;
            }
            throw new Exception("Can't find " + type.ToString());
        }
        
        public static ExplorerNode GetListFileFolder(ExplorerNode node)
        {
            node.Child.Clear();
            ExplorerNode root = node.GetRoot();
            MegaApiClient client = GetClient(root.RootInfo.Email);
            if (root == node) GetItems(client, GetRoot(root.RootInfo.Email, NodeType.Root), node);
            else
            {
                if (node.Info.Size != -1) throw new Exception("Can't explorer,this item is not folder.");
                MegaNzNode inode = new MegaNzNode(node.Info.Name,node.Info.ID,node.Parent.Info.ID,-1,NodeType.Directory,node.Info.DateMod);
                GetItems(client, inode, node);
            }
            return node;
        }

        static void GetItems(MegaApiClient client,INode node, ExplorerNode Enode)
        {
            foreach (INode child in client.GetNodes(node))
            {
                ExplorerNode c = new ExplorerNode();
                c.Info.ID = child.Id;
                c.Info.Name = child.Name;
                c.Info.DateMod = child.LastModificationDate;
                switch (child.Type)
                {
                    case NodeType.File:
                        c.Info.Size = child.Size;
                        Enode.AddChild(c);
                        break;
                    case NodeType.Directory:
                        c.Info.Size = -1;
                        Enode.AddChild(c);
                        break;
                    default: break;
                }
            }
        }

        class MegaNzNode : INode
        {
            public MegaNzNode(string Name,string ID,string parentid,long Size, NodeType type, DateTime mod_d)
            {
                this.name = Name;
                this.id = ID;
                this.parentid = parentid;
                this.size = Size;
                this.type = type;
                this.mod_d = mod_d;
            }
            string id;
            public string Id
            {
                get
                {
                    return id;
                }
            }
            DateTime mod_d;
            public DateTime LastModificationDate
            {
                get
                {
                    return mod_d;
                }
            }
            string name;
            public string Name
            {
                get
                {
                    return name;
                }
            }
            string owner;
            public string Owner
            {
                get
                {
                    return owner;
                }
            }
            string parentid;
            public string ParentId
            {
                get
                {
                    return parentid;
                }
            }
            long size;
            public long Size
            {
                get
                {
                    return size;
                }
            }
            NodeType type;
            public NodeType Type
            {
                get
                {
                    return type;
                }
            }

            public bool Equals(INode other)
            {
                throw new NotImplementedException();
            }
        }
    }

}
