using Newtonsoft.Json;
using SupDataDll.Class.Mega;
using System.Collections.Generic;
using System.IO;

namespace SupDataDll.Class
{
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
    public class TransferGroup : Transfer
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
    public class TransferItem : Transfer
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

}
