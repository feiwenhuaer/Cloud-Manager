using Cloud.Dropbox;
using Cloud.GoogleDrive;
using Core.Cloud;
using Core.StaticClass;
using Newtonsoft.Json;
using SupDataDll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Core.Transfer
{
    class ItemsTransferManager
    {
        public bool AreCut = false;//delete from after transfer
        public List<Thread> ThreadsItemLoadWork = new List<Thread>();
        public TransferGroup GroupData = new TransferGroup();
        public List<TransferBytes> ItemsTransferWork = new List<TransferBytes>();

        #region Declare
        List<ExplorerNode> items;
        public ExplorerNode fromfolder;
        public ExplorerNode savefolder;
        //load from save data
        internal ItemsTransferManager(JsonDataSaveGroup group_json)
        {
            this.GroupData = group_json.Group;
            this.fromfolder = group_json.fromfolder;
            this.AreCut = group_json.AreCut;
            this.savefolder = group_json.savefolder;

            this.GroupData.status = (group_json.Group.status == StatusTransfer.Done | group_json.Group.status == StatusTransfer.Error) ? group_json.Group.status : StatusTransfer.Stop;
            foreach (TransferItem item in this.GroupData.items)
            {
                if (item.status == StatusTransfer.Running) item.status = StatusTransfer.Stop;
                item.col[3] = item.status.ToString();
                item.SizeString = UnitConventer.ConvertSize(item.From.node.Info.Size, 2, UnitConventer.unit_size);
            }
            this.GroupData.change = ChangeTLV.Done;
            RefreshGroupDataToShow(-1);
        }
        //load from user
        public ItemsTransferManager(List<ExplorerNode> items, ExplorerNode fromfolder, ExplorerNode savefolder, bool AreCut = false)
        {
            if (items.Count == 0) throw new Exception("List<NewTransferItem> items count = 0");
            if (fromfolder == null) throw new ArgumentNullException("fromfolder");
            if (savefolder == null) throw new ArgumentNullException("savefolder");
            this.items = items;
            this.fromfolder = fromfolder;
            this.savefolder = savefolder;
            this.AreCut = AreCut;
        }
        #endregion

        #region Load List File Info
        public void LoadListItems()
        {
            this.GroupData.status = StatusTransfer.Loading;
            this.GroupData.col = new List<string> { fromfolder.GetFullPathString(), savefolder.GetFullPathString(), this.GroupData.status.ToString(), "0/0", "", "", "" };
            AppSetting.uc_lv_ud_instance.AddNewGroup(GroupData);
            //string path = fromfolder.RootInfo.uri == null ? fromfolder.TypeCloud.ToString() + ":" + AppSetting.settings.GetDefaultCloud(fromfolder.TypeCloud) + "?id=" + fromfolder.ID : fromfolder.Path_Raw;
            foreach (ExplorerNode item in items)
            {
                if (item.Info.Size >0) LoadFile(item);
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

            foreach (ExplorerNode ffitem in list.Child)
            {
                if (ffitem.Info.Size <=0) ListAllItemInFolder(ffitem);
                else LoadFile(ffitem);
            }
        }

        //Path_Parent : GD:default@gmail.com?id=id/folder/folder or GD:a@gmail.com/folder/folder
        void LoadFile(ExplorerNode node)//Path_raw path parent folder of file
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
            ud_item.col = new List<string> { node.GetFullPathString(), ud_item.To.node.GetFullPathString(), ud_item.status.ToString(), "", "", "", "" };
            GroupData.items.Add(ud_item);
            GroupData.col[3] = "0/" + GroupData.items.Count.ToString();
            RefreshGroupDataToShow(0, 0);
        }
        #endregion

        public void ManagerItemsAndRefreshData()
        {
            //clean thread
            for (int i = 0; i < ThreadsItemLoadWork.Count; i++)
            {
                if (!ThreadsItemLoadWork[i].IsAlive)
                {
                    ThreadsItemLoadWork.RemoveAt(i);
                    i--;
                }
            }

            switch (GroupData.status)
            {
                case StatusTransfer.Loading: return;
                case StatusTransfer.Started: GroupData.status = StatusTransfer.Running; GroupData.Timestamp = CurrentMillis.Millis; break;
                case StatusTransfer.Remove: return;
            }

            if (GroupData.status != StatusTransfer.Running)
            {
                if (!string.IsNullOrEmpty(GroupData.col[4])) GroupData.col[4] = "";
                if (!string.IsNullOrEmpty(GroupData.col[5])) GroupData.col[5] = "";
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
                        case StatusTransfer.Done: count_item_done++; break;
                        case StatusTransfer.Error: count_item_error++; break;
                        case StatusTransfer.Stop: count_item_stop++; break;
                        default: break;
                    }

                //clear speed download when not running
                if (GroupData.items[i].status != StatusTransfer.Running)
                {
                    if (!string.IsNullOrEmpty(GroupData.items[i].col[4])) GroupData.items[i].col[4] = "";
                    if (!string.IsNullOrEmpty(GroupData.items[i].col[5])) GroupData.items[i].col[5] = "";
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
                        Group_TotalTransfer += GroupData.items[i].Transfer;
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
                            GroupData.items[i].Timestamp = Stopwatch.GetTimestamp();
                            thr.Start(i);
                            ThreadsItemLoadWork.Add(thr);
                            count_item_running++;
                        }
                        #endregion

                        #region caculate speed & time left item
                        if (this.GroupData.items[i].status == StatusTransfer.Running)
                        {
                            long size_transfer = GroupData.items[i].Transfer - GroupData.items[i].OldTransfer;
                            long time_milisec = CurrentMillis.Millis - GroupData.items[i].Timestamp;
                            if (time_milisec != 0 & time_milisec >= GroupsTransferManager.TimeRefresh)
                            {
                                //speed
                                GroupData.items[i].Timestamp = CurrentMillis.Millis;
                                GroupData.items[i].OldTransfer = GroupData.items[i].Transfer;
                                decimal speed = (decimal)size_transfer * 1000 / time_milisec;
                                GroupData.items[i].col[4] = UnitConventer.ConvertSize(speed, 2, UnitConventer.unit_speed);
                                //time 
                                if (speed != 0)
                                {
                                    long length_left = GroupData.items[i].From.node.Info.Size - GroupData.items[i].Transfer;
                                    long secondleft = decimal.ToInt64(((decimal)length_left / speed));
                                    GroupData.items[i].col[5] = CurrentMillis.GetTimeBySecond((int)secondleft);
                                }
                            }
                        }
                        #endregion
                    }
                    #region caculate speed & time left group
                    long time_milisec_group = CurrentMillis.Millis - GroupData.Timestamp;
                    if (time_milisec_group != 0 & time_milisec_group >= GroupsTransferManager.TimeRefresh)
                    {
                        //speed
                        GroupData.Timestamp = CurrentMillis.Millis;
                        decimal speed_group = ((decimal)(Group_TotalTransfer - GroupData.OldTransfer)) * 1000 / time_milisec_group;
                        GroupData.OldTransfer = Group_TotalTransfer;
                        GroupData.col[4] = UnitConventer.ConvertSize(speed_group, 2, UnitConventer.unit_speed);
                        //time left
                        if (speed_group != 0)
                        {
                            long length_left_group = GroupData.TotalFileLength - Group_TotalTransfer;
                            long secondleft_group = length_left_group / decimal.ToInt64(speed_group);
                            GroupData.col[5] = CurrentMillis.GetTimeBySecond((int)secondleft_group);
                        }
                    }
                    #endregion
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

            RefreshGroupDataToShow(count_item_done,count_item_remove);
        }

        void RefreshGroupDataToShow(int count_item_done,int count_item_remove = 0)
        {
            if ((count_item_done != -1 && GroupData.col[3].IndexOf("100% (") < 0 && GroupData.items.Count != 0) | count_item_remove !=0)
                GroupData.col[3] = Math.Round((double)count_item_done * 100 / GroupData.items.Count, 2).ToString() + "% (" + count_item_done.ToString() + "/" + GroupData.items.Count.ToString() + ")";
            GroupData.col[2] = GroupData.status.ToString();
            for (int i = 0; i < GroupData.items.Count; i++)
            {
                GroupData.items[i].col[2] = GroupData.items[i].status.ToString();
                if (GroupData.items[i].col[3].IndexOf("100% (") < 0 & GroupData.items[i].From.node.Info.Size != 0)
                    GroupData.items[i].col[3] = Math.Round((double)GroupData.items[i].Transfer * 100 / GroupData.items[i].From.node.Info.Size, 2).ToString() + "% (" + UnitConventer.ConvertSize(GroupData.items[i].Transfer, 2, UnitConventer.unit_size) + "/" + GroupData.items[i].SizeString + ")";
                
                if (GroupData.items[i].ErrorMsg != GroupData.items[i].col[6]) GroupData.items[i].col[6] = GroupData.items[i].ErrorMsg;
            }
        }

        void WorkThread(object obj)
        {
            int x = (int)obj;
            try
            {
#if DEBUG
                Console.WriteLine("Transfer items:"+GroupData.items[x].From.node.GetFullPathString());
#endif
                GroupData.items[x].From.stream = AppSetting.ManageCloud.GetFileStream(
                    GroupData.items[x].From.node,
                    GroupData.items[x].TransferRequest,
                    GroupData.items[x].From.node.Info.Size - 1,
                    GroupData.items[x].To.node.GetRoot().RootInfo.Type != CloudType.LocalDisk);
                
                int buffer_length = 32;//default
                int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.BufferSize), out buffer_length);//get buffer_length from setting
                GroupData.items[x].buffer = new byte[buffer_length * 1024];//create buffer

                ExplorerNode rootnodeto = GroupData.items[x].To.node.GetRoot();

                GroupData.items[x].byteread = 0;
                string token = "";
                if (!string.IsNullOrEmpty(rootnodeto.RootInfo.Email)) token = AppSetting.settings.GetToken(rootnodeto.RootInfo.Email, rootnodeto.RootInfo.Type);
                //this.group.items[x].UploadID = "";//remuse
                GroupData.items[x].Transfer = GroupData.items[x].OldTransfer = GroupData.items[x].TransferRequest;//remuse
                GroupData.items[x].ErrorMsg = "";//clear error
                GroupData.items[x].Timestamp = CurrentMillis.Millis;
                if (GroupData.status != StatusTransfer.Running) return;
                switch (rootnodeto.RootInfo.Type)
                {
                    case CloudType.LocalDisk:
                        #region LocalDisk
                        ItemsTransferWork.Add(new TransferBytes(GroupData.items[x], this));
                        return;
                    #endregion

                    case CloudType.Dropbox:
                        #region Dropbox
                        int chunksizedb = 25;//default 25Mb
                        int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.Dropbox_ChunksSize), out chunksizedb);
                        GroupData.items[x].ChunkUploadSize = chunksizedb * 1024 * 1024;

                        DropboxRequestAPIv2 client = new DropboxRequestAPIv2(token);

                        if (string.IsNullOrEmpty(GroupData.items[x].UploadID))//create upload id
                        {
                            GroupData.items[x].byteread = GroupData.items[x].From.stream.Read(GroupData.items[x].buffer, 0, GroupData.items[x].buffer.Length);
                            dynamic json = JsonConvert.DeserializeObject(client.upload_session_start(GroupData.items[x].buffer, GroupData.items[x].byteread));
                            GroupData.items[x].UploadID = json.session_id;
                            GroupData.items[x].Transfer += GroupData.items[x].byteread;
                        }
                        ItemsTransferWork.Add(new TransferBytes(GroupData.items[x], this, client));
                        return;
                    #endregion

                    case CloudType.GoogleDrive:
                        #region GoogleDrive
                        DriveAPIHttprequestv2 gdclient = new DriveAPIHttprequestv2(JsonConvert.DeserializeObject<TokenGoogleDrive>(token));
                        gdclient.Email = rootnodeto.RootInfo.Email;
                        gdclient.TokenRenewEvent += GoogleDrive.Gdclient_TokenRenewEvent;

                        GoogleDrive.CreateFolder(GroupData.items[x].To.node.Parent);
                        int chunksizeGD = 5;//default
                        int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.GD_ChunksSize), out chunksizeGD);
                        GroupData.items[x].ChunkUploadSize = chunksizeGD * 1024 * 1024;
                        
                        if (string.IsNullOrEmpty(GroupData.items[x].UploadID))//create upload id
                        {
                            string parentid = GroupData.items[x].To.node.Parent.Info.ID;
                            string mimeType = Get_mimeType.Get_mimeType_From_FileExtension(GroupData.items[x].To.node.Info.GetExtensionFile());
                            string jsondata = "{\"title\": \"" + GroupData.items[x].From.node.Info.Name + "\", \"mimeType\": \"" + mimeType + "\", \"parents\": [{\"id\": \"" + parentid + "\"}]}";
                            GroupData.items[x].UploadID = gdclient.Files_insert_resumable_getUploadID(jsondata, mimeType, GroupData.items[x].From.node.Info.Size);
                        }
                        ItemsTransferWork.Add(new TransferBytes(GroupData.items[x], this, gdclient));
                        return;
                    #endregion
                    case CloudType.Mega:

                        return;
                }
            }
            catch (Exception ex)
            { GroupData.items[x].ErrorMsg = ex.Message +ex.StackTrace; GroupData.items[x].status = StatusTransfer.Error; return; }
        }
    }
}