using Aga.Controls.Tree;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace SupDataDll
{
    #region Account
    public class CloudEmail_Type
    {
        public string Email;
        public CloudName Type;
    }
    #endregion

    #region ListItem (explorer)
    public class ExplorerListItem
    {
        public string path;
        public bool addToTV = false;
        public bool explandTV = false;
        public object TV_data;
        public object TV_node;
        public string id;
        public int indexLV_tab = -1;
    }

    public class OldPathLV
    {
        public OldPathLV(string ID, string Path)
        {
            this.ID = ID;
            this.Path = Path;
        }
        public string ID { get; set; }
        public string Path { get; set; }
    }

    public class ListItemFileFolder
    {
        public List<FileFolder> Items = new List<FileFolder>();
        public string path_raw;
        public string id_folder;
        public string NameFolder = "";
    }
    
    public class FileFolder
    {
        public string Name;
        public DateTime Time_mod;
        public long Size = -1;
        public string mimeType;
        public List<string> parentid = new List<string>();
        public string id;
        public string path_display;
    }
    #endregion

    #region Transfer
    public class AddNewTransferItem
    {
        public AddNewTransferItem(string name,string id,string mimeType, Type_FileFolder type,long size = -1)
        {
            this.name = name;
            this.id = id;
            this.mimeType = mimeType;
            this.type = type;
            this.size = size;
        }
        public string name;
        public string id;
        public long size = -1;
        public string mimeType;
        public Type_FileFolder type;
    }
    
    public class TransferGroup: Transfer
    {
        // MaxItemDownload
        public int MaxItemsDownload = 2;
        //TreeListView
        public string Name = "";//TreeListView Form
        //Speed & timeleft
        public long TotalFileLength = 0;
        //Status
        public ChangeTLV change = ChangeTLV.Processing;//for change TLV
        //Items
        public List<TransferItem> items = new List<TransferItem>();
    }

    public class TransferItem: Transfer
    {
        [JsonIgnore]
        public string SizeString = "";
        public FileTransferInfo From = new FileTransferInfo();
        public FileTransferInfo To = new FileTransferInfo();
        public string UploadID = "";//for remuse upload
        public string ErrorMsg = "";
        public long Transfer = 0;//byte[] was transfer
        public int ChunkUploadSize = -1;// = -1 is download, >0 is chunk size upload
        public long TransferRequest = 0;//Save pos chunk upload success
        [JsonIgnore]
        public int byteread = 0;
        [JsonIgnore]
        public byte[] buffer;//buffer
    }

    public abstract class Transfer
    {
        [JsonIgnore]
        public long Timestamp = 0;
        public List<string> col { get; set; }
        public StatusTransfer status = StatusTransfer.Waiting;
        [JsonIgnore]
        public StatusTransfer CheckChangeStatus = StatusTransfer.Waiting;
        public long OldTransfer = 0;//for caculate speed
    }
    
    public class FileTransferInfo
    {
        [JsonIgnore]
        public AnalyzePath ap;
        [JsonIgnore]
        public Stream stream;
        public string Fileid;
        public string path;
        public long Size = -1;
    }

    public class TransferDataTLVWPF: ITreeModel
    {
        ObservableCollection<TransferGroup> groups_ = new ObservableCollection<TransferGroup>();
        public void Add(TransferGroup ud_group)
        {
            if (groups_.IndexOf(ud_group) < 0) groups_.Add(ud_group);
        }
        public void Remove(TransferGroup ud_group)
        {
            if (groups_.IndexOf(ud_group) > -1) groups_.Remove(ud_group);
        }


        public IEnumerable GetChildren(object parent)
        {
            var pr = parent as TransferGroup;
            if (parent == null)
            {
                foreach (TransferGroup group in groups_)
                {
                    yield return group;
                }
            }
            else if (pr != null)
            {
                foreach (TransferItem item in pr.items)
                {
                    yield return item;
                }
            }
        }

        public bool HasChildren(object parent)
        {
            return parent is TransferGroup;
        }
    }

    #endregion

    public class DeleteItems
    {
        public DeleteItems()
        {

        }
        public DeleteItems(string item)
        {
            items.Add(item);
        }

        public DeleteItems(List<string> items)
        {
            this.items.AddRange(items);
        }
        public List<string> items = new List<string>();
        public bool PernamentDelete = false;
    }
}
