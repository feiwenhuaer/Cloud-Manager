using Aga.Controls.Tree;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace SupDataDll
{
    #region Account
    public class ListAccountCloud
    {
        public List<CloudEmail_Type> account = new List<CloudEmail_Type>();
    }

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
    public class NewTransferItem
    {
        public NewTransferItem(string name,string id,string mimeType, Type_FileFolder type,long size = -1)
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
    
    public class TransferGroup
    {
        // MaxItemDownload
        public int MaxItemsDownload = 2;
        //TreeListView
        public string Name = "";//TreeListView Form
        public List<string> col { get; set; }
        //Speed & timeleft
        public long TotalFileLength = 0;
        public long OldTransfer = 0;
        public long Timestamp = 0;
        //Status
        public StatusTransfer status = StatusTransfer.Loading;
        public StatusTransfer CheckChangeStatus = StatusTransfer.Loading;
        public ChangeTLV change = ChangeTLV.Processing;
        //Items
        public List<TransferItem> items = new List<TransferItem>();
    }

    public class TransferItem
    {
        //Show UI
        public List<string> col { get; set; }
        public string SizeString = "";
        [JsonIgnore]
        public long Timestamp = 0;
        public UD_item_work_info From = new UD_item_work_info();
        public UD_item_work_info To = new UD_item_work_info();
        public StatusTransfer status = StatusTransfer.Waiting;
        public StatusTransfer CheckChangeStatus = StatusTransfer.Waiting;
        public string UploadID = "";//for remuse upload
        public string ErrorMsg = "";
        public long Transfer = 0;//byte[] was transfer
        public long OldTransfer = 0;//for caculate speed
        public int ChunkUploadSize = -1;// = -1 is download, >0 is chunk size upload
        public long TransferRequest = 0;//Save pos chunk upload success
        [JsonIgnore]
        public Thread item_work;
        [JsonIgnore]
        public int byteread = 0;
        [JsonIgnore]
        public byte[] buffer;//buffer
    }

    
    public class UD_item_work_info
    {
        [JsonIgnore]
        public AnalyzePath ap;
        [JsonIgnore]
        public Stream stream;
        public string Fileid;
        public string path;
        public long Size = -1;
    }

    public class UD_data_WPF : ITreeModel
    {
        ObservableCollection<TransferGroup> ud_groups = new ObservableCollection<TransferGroup>();

        public UD_data_WPF(TransferGroup group)
        {
            ud_groups.Add(group);
        }


        public void Add(TransferGroup ud_group)
        {
            ud_groups.Add(ud_group);
        }
        public void Remove(TransferGroup ud_group)
        {
            if (ud_groups.IndexOf(ud_group) > -1)
                ud_groups.Remove(ud_group);
        }


        public IEnumerable GetChildren(object parent)
        {
            var pr = parent as TransferGroup;
            if (parent == null)
            {
                foreach (TransferGroup group in ud_groups)
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
