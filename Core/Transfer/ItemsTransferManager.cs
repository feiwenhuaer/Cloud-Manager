using Cloud.Dropbox;
using Cloud.GoogleDrive;
using Cloud.MegaNz;
using Core.CloudSubClass;
using Core.StaticClass;
using Newtonsoft.Json;
using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Core.Transfer
{
    class ItemsTransferManager
    {
        #region Field
        public bool AreCut { get; private set; }//delete from after transfer
        public List<Thread> ThreadsItemLoadWork { get; private set; }
        public TransferGroup GroupData { get; private set; }
        public List<TransferBytes> ItemsTransferWork { get; private set; }
        #endregion

        #region Declare
        List<ExplorerNode> items;
        public ExplorerNode fromfolder;
        public ExplorerNode savefolder;
        /// <summary>
        /// Load data from save file.
        /// </summary>
        /// <param name="group_json"></param>
        internal ItemsTransferManager(JsonDataSaveGroup group_json)
        {
            LoadField();
            this.GroupData = group_json.Group;
            this.fromfolder = group_json.fromfolder;
            this.AreCut = group_json.AreCut;
            this.savefolder = group_json.savefolder;

            this.GroupData.status = (group_json.Group.status == StatusTransfer.Done | group_json.Group.status == StatusTransfer.Error) ? group_json.Group.status : StatusTransfer.Stop;
            foreach (TransferItem item in this.GroupData.items)
            {
                item.Group = GroupData;
                if (item.status == StatusTransfer.Running) item.status = StatusTransfer.Stop;
                item.DataSource.Status = item.status.ToString();
                item.SizeString = UnitConventer.ConvertSize(item.From.node.Info.Size, 2, UnitConventer.unit_size);
            }
            this.GroupData.change = ChangeTLV.Done;
            RefreshGroupDataToShow(-1);
        }
        /// <summary>
        /// Load data from user input.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="fromfolder"></param>
        /// <param name="savefolder"></param>
        /// <param name="AreCut"></param>
        public ItemsTransferManager(List<ExplorerNode> items, ExplorerNode fromfolder, ExplorerNode savefolder, bool AreCut = false)
        {
            if (items.Count == 0) throw new Exception("List<NewTransferItem> items count = 0");
            if (fromfolder == null) throw new ArgumentNullException("fromfolder");
            if (savefolder == null) throw new ArgumentNullException("savefolder");
            LoadField();
            this.items = items;
            this.fromfolder = fromfolder;
            this.savefolder = savefolder;
            this.AreCut = AreCut;
        }
        void LoadField()
        {
            AreCut = false;
            ThreadsItemLoadWork = new List<Thread>();
            GroupData = new TransferGroup();
            ItemsTransferWork = new List<TransferBytes>();
        }
        #endregion

        #region Load List File Info
        public void LoadListItems()
        {
            this.GroupData.status = StatusTransfer.Loading;
            this.GroupData.DataSource.From = fromfolder.GetFullPathString();
            this.GroupData.DataSource.To = savefolder.GetFullPathString();
            this.GroupData.DataSource.Status = this.GroupData.status.ToString();
            this.GroupData.DataSource.Progress = "0/0";

            AppSetting.UIMain.UpdateGroup(GroupData, UpdateTransfer_TLVUD.Add);
            //string path = fromfolder.RootInfo.uri == null ? fromfolder.TypeCloud.ToString() + ":" + AppSetting.settings.GetDefaultCloud(fromfolder.TypeCloud) + "?id=" + fromfolder.ID : fromfolder.Path_Raw;
            foreach (ExplorerNode item in items)
            {
                if (item.Info.Size > 0) LoadFile(item);
                else ListAllItemInFolder(item);
            }
            GroupData.status = StatusTransfer.Waiting;
            items.Clear();// clear Declare memory
        }
        
        void ListAllItemInFolder(ExplorerNode node)
        {
            ExplorerNode list;
            try
            {
                list = AppSetting.ManageCloud.GetItemsList(node);
            }
            catch (UnauthorizedAccessException ex) { Console.WriteLine(ex.Message); return; }

            if(list.Child.Count == 0)//create empty folder
            {

            }else foreach (ExplorerNode ffitem in list.Child)
            {
                if (ffitem.Info.Size <= 0) ListAllItemInFolder(ffitem);
                else LoadFile(ffitem);
            }
        }
        
        void LoadFile(ExplorerNode node)
        {
            TransferItem ud_item = new TransferItem();
            //From
            ud_item.From.node = node;
            //group & UI
            GroupData.TotalFileLength += node.Info.Size;
            ud_item.SizeString = UnitConventer.ConvertSize(node.Info.Size, 2, UnitConventer.unit_size);
            ud_item.status = StatusTransfer.Waiting;
            //To
            ud_item.To.node = node.MakeNodeTo(this.fromfolder, this.savefolder);
            ud_item.DataSource.From = node.GetFullPathString();
            ud_item.DataSource.To = ud_item.To.node.GetFullPathString();
            ud_item.DataSource.Status = ud_item.status.ToString();
            
            ud_item.Group = GroupData;
            GroupData.items.Add(ud_item);
            GroupData.DataSource.Progress = "0/" + GroupData.items.Count.ToString();
            RefreshGroupDataToShow(0, 0);
        }
        #endregion

        public void ManagerItemsAndRefreshData()
        {
            //clean thread
            ThreadsItemLoadWork.CleanNotWorkingThread();

            switch (GroupData.status)
            {
                case StatusTransfer.Loading: return;
                case StatusTransfer.Started: GroupData.status = StatusTransfer.Running; GroupData.TimeStamp = CurrentMillis.Millis; break;
                case StatusTransfer.Remove: return;
            }

            if (GroupData.status != StatusTransfer.Running)
            {
                if (!string.IsNullOrEmpty(GroupData.DataSource.Speed)) GroupData.DataSource.Speed = "";
                if (!string.IsNullOrEmpty(GroupData.DataSource.Estimated)) GroupData.DataSource.Estimated = "";
            }

            #region Count & Remove
            int count_item_running = 0;
            int count_item_done = 0;
            int count_item_error = 0;
            int count_item_stop = 0;
            int count_item_remove = 0;
            for (int i = 0; i < GroupData.items.Count; i++)
            {
                if (GroupData.items[i].status == StatusTransfer.Remove)
                {
                    GroupData.items.RemoveAt(i);
                    i--;
                    count_item_remove++;
                    continue;
                }
                else switch (GroupData.items[i].status)
                    {
                        case StatusTransfer.Running: count_item_running++; break;
                        case StatusTransfer.Moved:
                        case StatusTransfer.Added:
                        case StatusTransfer.Done: count_item_done++; break;
                        case StatusTransfer.Error: count_item_error++; break;
                        case StatusTransfer.Stop: count_item_stop++; break;
                        default: break;
                    }

                //clear speed download when not running
                if (GroupData.items[i].status != StatusTransfer.Running)
                {
                    if (!string.IsNullOrEmpty(GroupData.items[i].DataSource.Speed)) GroupData.items[i].DataSource.Speed = "";
                    if (!string.IsNullOrEmpty(GroupData.items[i].DataSource.Estimated)) GroupData.items[i].DataSource.Estimated = "";
                }
            }
            #endregion

            #region Running group
            if (this.GroupData.status == StatusTransfer.Running)
            {
                if (this.GroupData.items.Count != 0 && count_item_done + count_item_error + count_item_stop != this.GroupData.items.Count)
                {
                    long Group_TotalTransfer = 0;
                    for (int i = 0; i < GroupData.items.Count; i++)//start item waiting and Started(force)
                    {
                        Group_TotalTransfer += GroupData.items[i].SizeWasTransfer;
                        #region start item force start
                        if (this.GroupData.items[i].status == StatusTransfer.Started && this.GroupData.status == StatusTransfer.Running)
                        {
                            Thread thr = new Thread(WorkThread);
                            this.GroupData.items[i].status = StatusTransfer.Running;
                            thr.Start(i);
                            ThreadsItemLoadWork.Add(thr);
                            count_item_running++;
                        }
                        #endregion

                        #region start item waiting
                        if (GroupData.items[i].status == StatusTransfer.Waiting && count_item_running < GroupData.MaxItemsDownload)
                        {
                            Thread thr = new Thread(WorkThread);
                            GroupData.items[i].status = StatusTransfer.Running;
                            GroupData.items[i].TimeStamp = Stopwatch.GetTimestamp();
                            thr.Start(i);
                            ThreadsItemLoadWork.Add(thr);
                            count_item_running++;
                        }
                        #endregion

                        //caculate speed & time left item
                        if (this.GroupData.items[i].status == StatusTransfer.Running) GroupData.items[i].CalSpeedAndTimeLeft();
                    }
                    //caculate speed & time left group
                    GroupData.CalSpeedAndTimeLeft(Group_TotalTransfer);
                }
                else
                {
                    if (count_item_done == this.GroupData.items.Count)
                    {
                        this.GroupData.status = StatusTransfer.Done;//Done group
                        if (this.AreCut)
                        {
                            //DeleteItems list = new DeleteItems();
                            //list.PernamentDelete = false;
                            //foreach (AddNewTransferItem item in items) list.items.Add(fromfolder.Path_Raw + (fromfolder.PathIsCloud ? "/" : "\\") + item.name);
                            //Thread thr = new Thread(AppSetting.ManageCloud.Delete);
                            //thr.Start(list);
                            throw new Exception("Transfer done, but not support delete items after copy now.");
                        }
                    }
                    else this.GroupData.status = StatusTransfer.Error;
                }
            }
            #endregion

            #region Change LV
            if (this.GroupData.change == ChangeTLV.Processing && (this.GroupData.status == StatusTransfer.Done ||
                this.GroupData.status == StatusTransfer.Error || this.GroupData.status == StatusTransfer.Stop))
                this.GroupData.change = ChangeTLV.ProcessingToDone;
            if (this.GroupData.change == ChangeTLV.Done && (this.GroupData.status == StatusTransfer.Started || this.GroupData.status == StatusTransfer.Running ||
                this.GroupData.status == StatusTransfer.Waiting || this.GroupData.status == StatusTransfer.Loading))
                this.GroupData.change = ChangeTLV.DoneToProcessing;
            #endregion

            RefreshGroupDataToShow(count_item_done, count_item_remove);
        }

        void RefreshGroupDataToShow(int count_item_done, int count_item_remove = 0)
        {
            if ((count_item_done != -1 && GroupData.DataSource != null && GroupData.DataSource.Progress.IndexOf("100% (") < 0 && GroupData.items.Count != 0) | count_item_remove != 0)
                GroupData.DataSource.Progress = Math.Round((double)count_item_done * 100 / GroupData.items.Count, 2).ToString() + "% (" + count_item_done.ToString() + "/" + GroupData.items.Count.ToString() + ")";
            GroupData.DataSource.Status = GroupData.status.ToString();
            for (int i = 0; i < GroupData.items.Count; i++)
            {
                GroupData.items[i].DataSource.Status = GroupData.items[i].status.ToString();
                if (GroupData.items[i].DataSource.Progress.IndexOf("100% (") < 0 & GroupData.items[i].From.node.Info.Size != 0)
                    GroupData.items[i].DataSource.Progress = Math.Round((double)GroupData.items[i].SizeWasTransfer * 100 / GroupData.items[i].From.node.Info.Size, 2).ToString() + "% (" + UnitConventer.ConvertSize(GroupData.items[i].SizeWasTransfer, 2, UnitConventer.unit_size) + "/" + GroupData.items[i].SizeString + ")";

                if (GroupData.items[i].ErrorMsg != GroupData.items[i].DataSource.Error) GroupData.items[i].DataSource.Error = GroupData.items[i].ErrorMsg;
            }
        }

        void WorkThread(object obj)
        {
            TransferItem item = GroupData.items[(int)obj];
            ExplorerNode root_from = fromfolder.GetRoot;
            ExplorerNode root_to = savefolder.GetRoot;
            try
            {
                if (root_from.NodeType.Type == root_to.NodeType.Type && root_from.NodeType.Type != CloudType.LocalDisk) SameAccountCloud(item);//cloud, inport file from other account same cloud
                else Transfer(item);//not same type
            }
            catch (Exception ex){ item.ErrorMsg = ex.Message + ex.StackTrace; item.status = StatusTransfer.Error; return; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="import">False is copy</param>
        void SameAccountCloud(TransferItem item,bool import = true)
        {
            throw new Exception("SameAccountCloud not support now.");
        }

        void Transfer(TransferItem item)
        {

#if DEBUG
            Console.WriteLine("Transfer items:" + item.From.node.GetFullPathString());
#endif
            int buffer_length = 32;//default
            int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.BufferSize), out buffer_length);//get buffer_length from setting
            item.buffer = item.From.node.GetRoot.NodeType.Type == CloudType.Mega ? new byte[buffer_length * 2048] : new byte[buffer_length * 1024];//create buffer

            ExplorerNode rootnodeto = item.To.node.GetRoot;

            item.byteread = 0;
            //this.group.items[x].UploadID = "";//resume
            item.SizeWasTransfer = item.OldTransfer = item.SaveSizeTransferSuccess;//resume
            item.ErrorMsg = "";//clear error
            item.TimeStamp = CurrentMillis.Millis;
            if (GroupData.status != StatusTransfer.Running) return;
            switch (rootnodeto.NodeType.Type)
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

                    DropboxRequestAPIv2 DropboxClient = Dropbox.GetAPIv2(rootnodeto.NodeType.Email);

                    if (string.IsNullOrEmpty(item.UploadID))//create upload id
                    {
                        item.byteread = item.From.stream.Read(item.buffer, 0, item.buffer.Length);
                        IDropbox_Request_UploadSessionAppend session = DropboxClient.upload_session_start(item.buffer, item.byteread);
                        item.UploadID = session.session_id;
                        item.SizeWasTransfer += item.byteread;
                    }
                    ItemsTransferWork.Add(new TransferBytes(item, this, DropboxClient));
                    return;
                #endregion

                case CloudType.GoogleDrive:
                    #region GoogleDrive
                    DriveAPIHttprequestv2 gdclient = GoogleDrive.GetAPIv2(rootnodeto.NodeType.Email);
                    GoogleDrive.CreateFolder(item.To.node.Parent);
                    int chunksizeGD = 5;//default
                    int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.GD_ChunksSize), out chunksizeGD);
                    item.ChunkUploadSize = chunksizeGD * 1024 * 1024;

                    if (string.IsNullOrEmpty(item.UploadID))//create upload id
                    {
                        if (string.IsNullOrEmpty(item.To.node.Parent.Info.ID)) throw new Exception("Can't get root id.");
                        string parentid = item.To.node.Parent.Info.ID;
                        string mimeType = Get_mimeType.Get_mimeType_From_FileExtension(item.To.node.GetExtension());
                        string jsondata = "{\"title\": \"" + item.From.node.Info.Name + "\", \"mimeType\": \"" + mimeType + "\", \"parents\": [{\"id\": \"" + parentid + "\"}]}";
                        item.UploadID = gdclient.Files.Insert_ResumableGetUploadID(jsondata, mimeType, item.From.node.Info.Size);
                    }
                    ItemsTransferWork.Add(new TransferBytes(item, this, gdclient));
                    return;
                #endregion

                case CloudType.Mega:
                    #region Mega
                    MegaApiClient MegaClient = MegaNz.GetClient(rootnodeto.NodeType.Email);
                    item.buffer = new byte[128 * 1024];
                    if (string.IsNullOrEmpty(item.UploadID))//create upload id
                    {
                        MegaNz.AutoCreateFolder(item.To.node.Parent); //auto create folder
                        item.UploadID = MegaClient.RequestUrlUpload(item.From.node.Info.Size);//Make Upload url
                    }
                    item.From.stream = MegaApiClient.MakeEncryptStreamForUpload(item.From.stream, item.From.node.Info.Size, item.dataCryptoMega);//make encrypt stream from file
                    ItemsTransferWork.Add(new TransferBytes(item, this, MegaClient));
                    return;
                    #endregion
            }
        }
    }
}