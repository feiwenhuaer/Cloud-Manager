using Aga.Controls.Tree;
using Newtonsoft.Json;
using SupDataDll.Class.Mega;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace SupDataDll
{
    #region ListItem (explorer)
    public class ExplorerListItem
    {
        public ExplorerNode node;
        public bool addToTV = false;
        public bool explandTV = false;
        public object TV_data;
        public object TV_node;
        public int indexLV_tab = -1;
    }
    #endregion

    #region Transfer
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
        
        public string UploadID = "";//for resume upload
        public string ErrorMsg { get { return errormsg; } set { errormsg = value.Replace("\r", "").Replace("\n", ""); } }
        [JsonIgnore]
        string errormsg = "";
        public long SizeWasTransfer = 0;//byte[] was transfer
        public int ChunkUploadSize = -1;// = -1 is download, >0 is chunk size upload
        public long SaveSizeTransferSuccess = 0;//Save size chunk upload success
        [JsonIgnore]
        public int byteread = 0;
        [JsonIgnore]
        public byte[] buffer;//buffer

        public DataCryptoMega dataCryptoMega;//for mega only
    }
    
    public class FileTransferInfo
    {
        public ExplorerNode node;
        [JsonIgnore]
        public Stream stream;
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
        public DeleteItems(ExplorerNode item)
        {
            Items.Add(item);
        }

        public DeleteItems(List<ExplorerNode> items)
        {
            Items.AddRange(items);
        }
        List<ExplorerNode> items = new List<ExplorerNode>();

        public List<ExplorerNode> Items { get; set; }
        public bool PernamentDelete = false;
    }
}
