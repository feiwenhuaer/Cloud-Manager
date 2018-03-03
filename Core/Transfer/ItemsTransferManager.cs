using Core.CloudSubClass;
using Core.StaticClass;
using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TqkLibs.CloudStorage.GoogleDrive;
using TqkLibs.CloudStorage.MegaNz;
using TqkLibs.CloudStorage.Dropbox;
using Newtonsoft.Json;
using System.Text;
using System.Runtime.InteropServices;

namespace Core.Transfer
{
  public delegate void updateclosingform(string text);
  public delegate void closeapp();
  public class ItemsTransferManager
  {
    #region Field
    //public bool AreCut { get; private set; }//delete form after transfer
    public List<Thread> ThreadsItemLoadWork { get; private set; } = new List<Thread>();
    public List<TransferBytes> ItemsTransferWork { get; private set; } = new List<TransferBytes>();
    public TransferListViewData ItemsTransfer { get; set; } = new TransferListViewData();
    public int TimeRefresh { get; set; } = 500;//500ms
    public int KillTheadTime { get; set; } = 15000000;//15 sec
    public Thread MainThread;
    string temp_jsonSaveData;
    public int ItemsWorkingLimit { get; set; } = 3;
    public StatusUpDownApp Status { get; set; } = StatusUpDownApp.Pause;

    public event updateclosingform Eventupdateclosingform;
    public event closeapp Eventcloseapp;

#if DEBUG
    public string timeformat = "hh:mm:ss:ffff";
#endif

    #endregion



    public void Start(Type type)
    {
      ReadData();      
      MainThread = new Thread(LoadMainThread);
      MainThread.Start(type);
    }

    void LoadMainThread(object type)
    {
      var assembly = ((Type)type).Assembly;
      var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
      var id = attribute.Value;
      bool createdNew;
      var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, id, out createdNew);
      var signaled = false;

      if (!createdNew)
      {
        waitHandle.Set();
        return;
      }
      var timer = new Timer(OnTimerElapsed, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(TimeRefresh));
      do
      {
        signaled = waitHandle.WaitOne(TimeSpan.FromMilliseconds(1000));
#if DEBUG
        Console.WriteLine(DateTime.Now.ToString(timeformat) + " | Core.Transfer.ItemsTransferManager.LoadMainThread(): signaled=" + signaled);
#endif
      } while (!signaled);
    }

    private void OnTimerElapsed(object state)
    {
#if DEBUG
      Console.WriteLine(DateTime.Now.ToString(timeformat) + " | Core.Transfer.ItemsTransferManager.OnTimerElapsed(): Timer elapsed.");      
#endif
      ThreadsItemLoadWork.CleanNotWorkingThread();      
      if(ItemsTransferWork.Count < ItemsWorkingLimit)
      {
        for(int i = 0; i < ItemsTransfer.ItemsBlinding.Count; i++)
        {
          if(ItemsTransfer.ItemsBlinding[i].status == StatusTransfer.Waiting)
          {
            Thread thr = new Thread(WorkThread);
            ItemsTransfer.ItemsBlinding[i].status = StatusTransfer.Running;
            thr.Start(ItemsTransfer.ItemsBlinding[i]);
            ThreadsItemLoadWork.Add(thr);
          }
        }
      }
    }



    public void Shutdown()
    {

    }

    #region Load items

    public void AddItems(List<IItemNode> items, IItemNode fromfolder, IItemNode savefolder, bool AreCut)
    {
      if (items.Count == 0) throw new Exception("List<NewTransferItem> items count = 0");
      if (fromfolder == null) throw new ArgumentNullException("fromfolder");
      if (savefolder == null) throw new ArgumentNullException("savefolder");
      
      foreach(IItemNode item in items)
      {
        if (item.Info.Size > 0) AddFile(item, fromfolder, savefolder, AreCut);
        else LoadFolder(item, fromfolder, savefolder, AreCut);
      }
    }
    void AddFile(IItemNode item, IItemNode fromfolder, IItemNode savefolder, bool AreCut)
    {
      TransferItem ud_item = new TransferItem();
      ud_item.From.node = item;
      ud_item.SizeString = UnitConventer.ConvertSize(item.Info.Size, 2, UnitConventer.unit_size);
      ud_item.status = StatusTransfer.Waiting;
      //To
      ud_item.To.node = item.MakeNodeTo(fromfolder,savefolder);
      ud_item.DataSource.From = item.GetFullPathString();
      ud_item.DataSource.To = ud_item.To.node.GetFullPathString();
      ud_item.DataSource.Status = ud_item.status.ToString();
      ItemsTransfer.ItemsBlinding.Add(ud_item);
    }
    void LoadFolder(IItemNode item, IItemNode fromfolder, IItemNode savefolder, bool AreCut)
    {
      foreach(IItemNode child in AppSetting.ManageCloud.GetItemsList(item).Childs)
      {
        if (item.Info.Size > 0) AddFile(item, fromfolder, savefolder, AreCut);
        else LoadFolder(item, fromfolder, savefolder, AreCut);
      }
    }
    #endregion

    #region Save/Read Data When Close/Open program
    public void ReadData()
    {
      if (ReadWriteData.Exists(ReadWriteData.File_DataUploadDownload))
      {
        var readerjson = ReadWriteData.Read(ReadWriteData.File_DataUploadDownload);
        if (readerjson != null)
        {
          try
          {
            this.ItemsTransfer = JsonConvert.DeserializeObject<TransferListViewData>(readerjson.ReadToEnd());
          }
          catch (Exception)
          {
            readerjson.Close();
            ReadWriteData.Delete(ReadWriteData.File_DataUploadDownload);
          }
        }
      }
    }
    
    public void SaveData()
    {
      string json = JsonConvert.SerializeObject(ItemsTransfer);
      if (temp_jsonSaveData == json) return;
      else temp_jsonSaveData = json;
      ReadWriteData.Write(ReadWriteData.File_DataUploadDownload, Encoding.UTF8.GetBytes(json));
    }
    #endregion
   
    void WorkThread(object obj)
    {
      TransferItem item = (TransferItem)obj;
      
      RootNode root_from = item.From.Root.GetRoot;
      RootNode root_to = item.To.Root.GetRoot;
      try
      {
        if (root_from.RootType.Type == root_to.RootType.Type && root_from.RootType.Type != CloudType.LocalDisk) SameAccountCloud(item);//cloud, inport file from other account same cloud
        else Transfer(item);//not same type
      }
      catch (Exception ex) { item.DataSource.Error = ex.Message + ex.StackTrace; item.status = StatusTransfer.Error; return; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="import">False is copy</param>
    void SameAccountCloud(TransferItem item, bool import = true)
    {
      switch (item.From.node.GetRoot.RootType.Type)
      {
        case CloudType.GoogleDrive:
          Drivev2_File f = GoogleDrive.MoveItem(item.From.node,item.To.node,null,true);

          foreach (var p in f.parents) if (p.id == item.To.node.Parent.Info.ID) item.status = StatusTransfer.Done;
                                      else item.status = StatusTransfer.Error;
          break;
        default: throw new Exception("SameAccountCloud not support now.");
      }      
    }

    void Transfer(TransferItem item)
    {

#if DEBUG
      Console.WriteLine("Transfer items:" + item.From.node.GetFullPathString());
#endif
      int buffer_length = 32;//default
      int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.BufferSize), out buffer_length);//get buffer_length from setting
      item.buffer = item.From.node.GetRoot.RootType.Type == CloudType.Mega ? new byte[buffer_length * 2048] : new byte[buffer_length * 1024];//create buffer

      ItemNode rootnodeto = item.To.node.GetRoot;

      item.byteread = 0;
      //this.group.items[x].UploadID = "";//resume
      item.SizeWasTransfer = item.OldTransfer = item.SaveSizeTransferSuccess;//resume
      item.DataSource.Error = string.Empty;//clear error
      item.TimeStamp = CurrentMillis.Millis;
      switch (rootnodeto.GetRoot.RootType.Type)
      {
        case CloudType.LocalDisk:
          #region LocalDisk
          ItemsTransferWork.Add(new TransferBytes(item, this));
          return;
        #endregion

        case CloudType.Dropbox:
          #region Dropbox
          int chunksizedb = 25;//default 25Mb
          int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.Dropbox_ChunksSize), out chunksizedb);
          item.ChunkUploadSize = chunksizedb * 1024 * 1024;

          DropboxRequestAPIv2 DropboxClient = Dropbox.GetAPIv2(rootnodeto.GetRoot.RootType.Email);

          if (string.IsNullOrEmpty(item.UploadID))//create upload id
          {
            item.byteread = item.From.stream.Read(item.buffer, 0, item.buffer.Length);
            IDropbox_Request_UploadSessionAppend session = DropboxClient.upload_session_start(item.buffer, item.byteread);
            item.UploadID = session.session_id;
            item.SizeWasTransfer += item.byteread;
          }
          ItemsTransferWork.Add(new TransferBytes(item, DropboxClient));
          return;
        #endregion

        case CloudType.GoogleDrive:
          #region GoogleDrive
          DriveAPIv2 gdclient = GoogleDrive.GetAPIv2(rootnodeto.GetRoot.RootType.Email);
          GoogleDrive.CreateFolder(item.To.node.Parent);
          int chunksizeGD = 5;//default
          int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.GD_ChunksSize), out chunksizeGD);
          item.ChunkUploadSize = chunksizeGD * 1024 * 1024;

          if (string.IsNullOrEmpty(item.UploadID))//create upload id
          {
            if (string.IsNullOrEmpty(item.To.node.Parent.Info.ID)) throw new Exception("Can't get root id.");
            string parentid = item.To.node.Parent.Info.ID;
            string mimeType = Get_mimeType.Get_mimeType_From_FileExtension(item.To.node.GetExtension());

            Drivev2_File f_metadata = new Drivev2_File() {  title = item.From.node.Info.Name, mimeType = mimeType};
            f_metadata.parents = new List<Drivev2_Parent>() { new Drivev2_Parent() { id = parentid } };

            item.UploadID = gdclient.Files.Insert_Resumable_GetUploadID(f_metadata, mimeType, item.From.node.Info.Size);
          }
          ItemsTransferWork.Add(new TransferBytes(item, gdclient));
          return;
        #endregion

        case CloudType.Mega:
          #region Mega
          MegaApiClient MegaClient = MegaNz.GetClient(rootnodeto.GetRoot.RootType.Email);
          item.buffer = new byte[128 * 1024];
          if (string.IsNullOrEmpty(item.UploadID))//create upload id
          {
            MegaNz.AutoCreateFolder(item.To.node.Parent); //auto create folder
            item.UploadID = MegaClient.RequestUrlUpload(item.From.node.Info.Size);//Make Upload url
          }
          item.From.stream = MegaApiClient.MakeEncryptStreamForUpload(item.From.stream, item.From.node.Info.Size, item.dataCryptoMega);//make encrypt stream from file
          ItemsTransferWork.Add(new TransferBytes(item, MegaClient));
          return;
          #endregion
      }
    }
  }
}