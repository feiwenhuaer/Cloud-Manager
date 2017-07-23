using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Cloud.MegaNz;

namespace CloudManagerGeneralLib.Class
{
    public class DataTLV
    {
        public DataTLV()
        {
            Col = new List<string>() { "", "", "", "", "", "", "" };
        }

        public List<string> Col { get; set; }

        public string From { get { return Col[0]; } set { Col[0] = value; } }
        public string To { get { return Col[1]; } set { Col[1] = value; } }
        public string Status { get { return Col[2]; } set { Col[2] = value; } }
        public string Progress { get { return Col[3]; } set { Col[3] = value; } }
        public string Speed { get { return Col[4]; } set { Col[4] = value; } }
        public string Estimated { get { return Col[5]; } set { Col[5] = value; } }
        public string Error { get { return Col[6]; } set { Col[6] = value; } }
    }

    public abstract class Transfer
    {
        public Transfer()
        {
            DataSource = new DataTLV();
        }
        [JsonIgnore]
        public long TimeStamp = 0;
        public DataTLV DataSource { get; set; }
        public StatusTransfer status = StatusTransfer.Waiting;
        [JsonIgnore]
        public StatusTransfer CheckChangeStatus = StatusTransfer.Waiting;
        public long OldTransfer = 0;//for caculate speed
    }
    public class TransferGroup : Transfer
    {
        public TransferGroup()
        {
            
        }
        // MaxItemDownload
        public int MaxItemsDownload = 2;
        //TreeListView
        public string Name = "";//TreeListView Form
        //Speed & timeleft
        public long TotalFileLength = 0;
        //Status
        public ChangeTLV change = ChangeTLV.Processing;//for change TLV
        //Items
        [JsonIgnore]
        List<TransferItem> _items = new List<TransferItem>();
        public List<TransferItem> items { get { return _items; } private set { _items = value; } }
        
        public void AddTransferItem(TransferItem item)
        {
            item.Group = this;
            items.Add(item);
        }
    }
    public class TransferItem : Transfer
    {
        [JsonIgnore]
        public TransferGroup Group { get; set; }

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
        public IItemNode node;
        [JsonIgnore]
        public Stream stream;
    }

}
