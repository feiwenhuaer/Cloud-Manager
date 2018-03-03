using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using TqkLibs.CloudStorage.MegaNz;

namespace CloudManagerGeneralLib.Class
{
  public class TransferListViewData
  {
    public ObservableCollection<TransferItem> ItemsBlinding { get; set; } = new ObservableCollection<TransferItem>();
  }

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
    public string Error { get { return Col[6]; } set { Col[6] = value.Replace("\r","").Replace("\n",""); } }
  }
  
  //public class TransferGroup : Transfer
  //{
  //    public TransferGroup()
  //    {

  //    }
  //    // MaxItemDownload
  //    public int MaxItemsDownload = 2;
  //    //TreeListView
  //    public string Name = "";//TreeListView Form
  //    //Speed & timeleft
  //    public long TotalFileLength = 0;
  //    //Status
  //    public ChangeTLV change = ChangeTLV.Processing;//for change TLV
  //    //Items
  //    [JsonIgnore]
  //    List<TransferItem> _items = new List<TransferItem>();
  //    public List<TransferItem> items { get { return _items; } private set { _items = value; } }

  //    public void AddTransferItem(TransferItem item)
  //    {
  //        item.Group = this;
  //        items.Add(item);
  //    }
  //}
  public class TransferItem
  {
    public TransferItem()
    {
      DataSource = new DataTLV();
    }

    [JsonIgnore]
    public long TimeStamp = 0;
    public DataTLV DataSource { get; set; }// blinding to listview
    public StatusTransfer status { get; set; } = StatusTransfer.Waiting;
    public long OldTransfer { get; set; } = 0;//for calculate speed
    public long SizeWasTransfer { get; set; } = 0;//byte[] was transfer
    public int ChunkUploadSize { get; set; } = -1;// = -1 is download, >0 is chunk size upload
    public long SaveSizeTransferSuccess { get; set; } = 0;//Save size chunk upload success
    public bool AreCut { get; set; } = false;
    [JsonIgnore]
    public string SizeString { get; set; } = "";
    public FileTransferInfo From { get; set; } = new FileTransferInfo();
    public FileTransferInfo To { get; set; } = new FileTransferInfo();

    public string UploadID { get; set; } = "";//for resume upload

    [JsonIgnore]
    public int byteread { get; set; } = 0;
    [JsonIgnore]
    public byte[] buffer { get; set; }//buffer

    public DataCryptoMega dataCryptoMega { get; set; }//for mega only

  }
  public class FileTransferInfo
  {
    public IItemNode Root { get; set; }
    public IItemNode node { get; set; }
    [JsonIgnore]
    public Stream stream;
  }

}
