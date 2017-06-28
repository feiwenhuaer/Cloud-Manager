﻿using Cloud.MegaNz;
using CloudManagerGeneralLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using CustomHttpRequest;
using CloudManagerGeneralLib.Class.Mega;
using CloudManagerGeneralLib.Class;
using Core.StaticClass;

namespace Core.CloudSubClass
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

        public static ItemNode GetListFileFolder(ItemNode node)
        {
            node.Child.Clear();
            ItemNode root = node.GetRoot;
            MegaApiClient client = GetClient(root.NodeType.Email);

            if (root == node)
            {
                string ID = node.Info.ID;
                INode n = null;
                if (!string.IsNullOrEmpty(ID)) n = new MegaNzNode(ID);
                else
                {
                    n = GetRoot(root.NodeType.Email, NodeType.Root);
                    AppSetting.settings.SetRootID(root.NodeType.Email, CloudType.Mega, n.Id);
                }
                GetItems(client, n, node);
            }
            else
            {
                if (node.Info.Size != -1) throw new Exception("Can't explorer,this item is not folder.");
                MegaNzNode inode = new MegaNzNode(node.Info.Name, node.Info.ID, node.Parent.Info.ID, -1, NodeType.Directory, node.Info.DateMod);
                GetItems(client, inode, node);
            }
            return node;
        }
        
        public static Stream GetStream(ItemNode node,long start_pos = -1,long end_pos = -1,bool IsUpload = false, object DataEx = null)
        {
            if (node.Info.Size < 1) throw new Exception("Mega GetStream: Filesize <= 0.");
            MegaApiClient api = GetClient(node.GetRoot.NodeType.Email);
            MegaNzNode meganode = new MegaNzNode(node.Info.ID, node.Info.MegaCrypto);
            if (!IsUpload && start_pos >= 0)//download
            {
                long end = end_pos > 0 ? end_pos : node.Info.Size - 1;
                return api.Download(meganode, start_pos, end, DataEx);
            }
            else throw new Exception("Not Support Upload.");
        }

        public static void CreateFolder(ItemNode node)
        {
            MegaNzNode parent_meganode = new MegaNzNode(node.Parent.Info.ID);
            MegaApiClient client = GetClient(node.GetRoot.NodeType.Email);
            INode folder_meganode = client.CreateFolder(node.Info.Name, parent_meganode);
            node.Info.ID = folder_meganode.Id;
        }

        public static void AutoCreateFolder(ItemNode node)
        {
            List<ItemNode> list = node.GetFullPath();
            if (list[0].NodeType.Type != CloudType.Mega) throw new Exception("Mega only.");
            MegaApiClient client = GetClient(list[0].NodeType.Email);
            list.RemoveAt(0);
            foreach(ItemNode child in list)
            {
                if(string.IsNullOrEmpty(child.Info.ID))
                {
                    MegaNzNode m_p_node = new MegaNzNode(child.Parent.Info.ID);
                    INode c_node = client.GetNodes(m_p_node).Where(n => n.Name == child.Info.Name).First();//find
                    if(c_node == null) c_node = client.CreateFolder(child.Info.Name, m_p_node);//if not found -> create
                    child.Info.ID = c_node.Id;
                }
            }
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
                    foreach (INode n in client.GetNodes().Where<INode>(n => n.Type == NodeType.Root))
                    {
                        if (n.Type == NodeType.Root) return n;
                    }
                    break;
            }
            throw new Exception("Can't find " + type.ToString());
        }
        
        static void GetItems(MegaApiClient client,INode node, ItemNode Enode)
        {
            foreach (INode child in client.GetNodes(node))
            {
                ItemNode c = new ItemNode();
                c.Info.ID = child.Id;
                c.Info.Name = child.Name;
                c.Info.DateMod = child.LastModificationDate;
                c.Info.MegaCrypto = new MegaKeyCrypto(child as INodeCrypto);
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

        public class MegaNzNode : MegaKeyCrypto,INode
        {
            public MegaNzNode(string ID)
            {
                this.id = ID;
            }
            public MegaNzNode(string ID,long Size)
            {
                this.id = ID;
                this.size = Size;
            }
            
            
            public MegaNzNode(string ID, INodeCrypto keyDeCrypt) : this(null, ID, null, -1, NodeType.File, new DateTime(), keyDeCrypt)
            {
            }
            public MegaNzNode(string Name, string ID, string parentid, long Size, NodeType type, DateTime mod_d) : this(Name, ID, parentid, Size, type, mod_d, null)
            {
            }
            public MegaNzNode(string Name, string ID, string parentid, long Size, NodeType type, DateTime mod_d, INodeCrypto keyDeCrypt) : base(keyDeCrypt)
            {
                this.name = Name;
                this.id = ID;
                this.parentid = parentid;
                this.size = Size;
                this.type = type;
                this.mod_d = mod_d;
            }


            #region INode
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
            NodeType type;//default = 0 => File
            public NodeType Type
            {
                get
                {
                    return type;
                }
                set { type = value; }
            }

            public bool Equals(INode other)
            {
                throw new NotImplementedException();
            }
            #endregion
        }

        public static ItemNode GetItem(ItemNode node)
        {
            MegaApiClient client = GetClient(node.GetRoot.NodeType.Email);
            MegaNzNode inode = new MegaNzNode(node.Info.ID);
            GetItems(client, inode, node);
            return node;
        }
    }

    public class MegaUpload
    {
        HttpRequest_ request;
        public MegaUpload(Uri uri,int lengthUpload)
        {
            request = new HttpRequest_(uri, "POST");
            request.AddHeader("HOST", uri.Host);
            request.AddHeader("Content-Type", "application/octet-stream");
            request.AddHeader("Content-Length", lengthUpload.ToString());
        }

        public Stream MakeStreamUpload()
        {
            return request.SendHeader_And_GetStream();
        }

        public string ReadDataTextResponse()
        {
            return request.GetTextDataResponse(true, true);
        }
    }
}
