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
    public class ItemsTransferManager
    {
        public bool AreCut = false;//delete from after transfer
        public List<Thread> ThreadsItemLoadWork = new List<Thread>();
        public TransferGroup GroupData = new TransferGroup();
        public List<TransferBytes> ItemsTransferWork = new List<TransferBytes>();

        #region Declare
        List<AddNewTransferItem> items;
        public AnalyzePath fromfolder;
        public AnalyzePath savefolder;
        //load from save data
        internal ItemsTransferManager(JsonDataSaveGroup group_json)
        {
            this.GroupData = group_json.Group;
            this.fromfolder = new AnalyzePath(group_json.fromfolder_raw);
            this.AreCut = group_json.AreCut;
            this.savefolder = new AnalyzePath(group_json.savefolder_raw);

            this.GroupData.status = (group_json.Group.status == StatusTransfer.Done | group_json.Group.status == StatusTransfer.Error) ? group_json.Group.status : StatusTransfer.Stop;
            foreach (TransferItem item in this.GroupData.items)
            {
                if (item.status == StatusTransfer.Running) item.status = StatusTransfer.Stop;
                item.col[3] = item.status.ToString();
                item.SizeString = UnitConventer.ConvertSize(item.From.Size, 2, UnitConventer.unit_size);
                item.From.ap = new AnalyzePath(item.From.path);
                item.To.ap = new AnalyzePath(item.To.path);
            }
            this.GroupData.change = ChangeTLV.Done;
            RefreshGroupDataToShow(-1);
        }
        //load from user
        public ItemsTransferManager(List<AddNewTransferItem> items, string fromfolder_raw, string savefolder_raw, bool AreCut = false)
        {
            if (items.Count == 0) throw new Exception("List<NewTransferItem> items count = 0");
            if (string.IsNullOrEmpty(fromfolder_raw)) throw new ArgumentNullException(fromfolder_raw);
            if (string.IsNullOrEmpty(savefolder_raw)) throw new ArgumentNullException(savefolder_raw);
            this.items = items;
            this.fromfolder = new AnalyzePath(fromfolder_raw);
            this.savefolder = new AnalyzePath(savefolder_raw);
            this.AreCut = AreCut;
        }
        #endregion

        #region Load List File Info
        public void LoadListItems()
        {
            this.GroupData.status = StatusTransfer.Loading;
            this.GroupData.col = new List<string> { fromfolder.Path_Raw, savefolder.Path_Raw, this.GroupData.status.ToString(), "0/0", "", "", "" };
            AppSetting.uc_lv_ud_instance.AddNewGroup(GroupData);
            string path = fromfolder.PathIsUrl ? fromfolder.TypeCloud.ToString() + ":" + AppSetting.settings.GetDefaultCloud(fromfolder.TypeCloud) + "?id=" + fromfolder.ID : fromfolder.Path_Raw;
            foreach (AddNewTransferItem item in items)
            {
                if (item.type == Type_FileFolder.File) LoadFile(path, item.name, item.size, item.id);
                else ListAllItemInFolder(path + (fromfolder.PathIsCloud ? "/" : "\\") + item.name, item.id);
            }
            GroupData.status = StatusTransfer.Waiting;
            items.Clear();// clear Declare memory
        }


        void ListAllItemInFolder(string path_rawItem, string id = "")
        {
            ListItemFileFolder list;
            try
            {
                if (fromfolder.PathIsUrl) list = AppSetting.ManageCloud.GetItemsList("", id);
                else list = AppSetting.ManageCloud.GetItemsList(path_rawItem, id);//UnauthorizedAccessException
            }
            catch (UnauthorizedAccessException) { return; }

            foreach (FileFolder ffitem in list.Items)
            {
                if (ffitem.Size == -1) ListAllItemInFolder(fromfolder.PathIsUrl ? path_rawItem + "/" + ffitem.Name : path_rawItem + (fromfolder.PathIsCloud ? "/" : "\\") + ffitem.Name, ffitem.id);
                else LoadFile(path_rawItem, ffitem.Name, ffitem.Size, ffitem.id);
            }
        }

        //Path_Parent : GD:default@gmail.com?id=id/folder/folder or GD:a@gmail.com/folder/folder
        void LoadFile(string Path_Parent, string FileName, long size, string FileId)//Path_raw path parent folder of file
        {
            TransferItem ud_item = new TransferItem();
            //From
            ud_item.From.path = Path_Parent + (fromfolder.PathIsCloud ? "/" : "\\") + FileName;
            ud_item.From.ap = new AnalyzePath(ud_item.From.path);
            ud_item.From.Fileid = FileId;
            ud_item.From.Size = size;
            //group & UI
            GroupData.TotalFileLength += size;
            ud_item.SizeString = UnitConventer.ConvertSize(size, 2, UnitConventer.unit_size);
            ud_item.status = StatusTransfer.Waiting;
            //To
            ud_item.To.path = AnalyzePath.GetPathTo(fromfolder.PathIsUrl ? ud_item.From.ap.Path_Raw : ud_item.From.ap.GetPath(), fromfolder, savefolder);
            ud_item.To.ap = new AnalyzePath(ud_item.To.path);
            ud_item.col = new List<string> { ud_item.From.ap.Path_Raw, ud_item.To.ap.Path_Raw, ud_item.status.ToString(), "", "", "", "" };
            GroupData.items.Add(ud_item);
            GroupData.col[3] = "0/" + GroupData.items.Count.ToString();
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
            if (this.GroupData.status == StatusTransfer.Running && this.GroupData.items.Count !=0 && 
                count_item_done + count_item_error + count_item_stop != this.GroupData.items.Count)
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
                                long length_left = GroupData.items[i].From.Size - GroupData.items[i].Transfer;
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
                if (this.GroupData.status == StatusTransfer.Running)
                {
                    #region Remove Items_From If Cut (if done 100%)
                    if (this.AreCut && count_item_done == this.GroupData.items.Count)
                    {
                        DeleteItems list = new DeleteItems();
                        list.PernamentDelete = false;
                        foreach (AddNewTransferItem item in items)
                        {
                            list.items.Add(fromfolder.Path_Raw + (fromfolder.PathIsCloud ? "/" : "\\") + item.name);
                        }
                        Thread thr = new Thread(AppSetting.ManageCloud.Delete);
                        thr.Start(list);
                    }
                    #endregion
                    this.GroupData.status = StatusTransfer.Done;//Done group
                }
            }
            #endregion

            #region Change LV
            if (this.GroupData.change == ChangeTLV.Processing && (this.GroupData.status == StatusTransfer.Done |
                this.GroupData.status == StatusTransfer.Error | this.GroupData.status == StatusTransfer.Stop))
                this.GroupData.change = ChangeTLV.ProcessingToDone;
            if (this.GroupData.change == ChangeTLV.Done && (this.GroupData.status == StatusTransfer.Started | this.GroupData.status == StatusTransfer.Running |
                this.GroupData.status == StatusTransfer.Waiting | this.GroupData.status == StatusTransfer.Loading))
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
                if (GroupData.items[i].col[3].IndexOf("100% (") < 0 & GroupData.items[i].From.Size != 0)
                    GroupData.items[i].col[3] = Math.Round((double)GroupData.items[i].Transfer * 100 / GroupData.items[i].From.Size, 2).ToString() + "% (" + UnitConventer.ConvertSize(GroupData.items[i].Transfer, 2, UnitConventer.unit_size) + "/" + GroupData.items[i].SizeString + ")";
                
                if (GroupData.items[i].ErrorMsg != GroupData.items[i].col[6]) GroupData.items[i].col[6] = GroupData.items[i].ErrorMsg;
            }
        }

        void WorkThread(object obj)
        {
            int x = (int)obj;
            try
            {
                Console.WriteLine("Transfer items:"+GroupData.items[x].From.ap.Path_Raw);
                #region CreateStreamFrom
                if (!fromfolder.PathIsUrl)
                {
                    //group.items[x].From.ap = new AnalyzePath(group.items[x].From.path);
                    GroupData.items[x].From.stream = AppSetting.ManageCloud.GetFileStream(GroupData.items[x].From.ap.Path_Raw,
                        GroupData.items[x].From.Fileid,
                        GroupData.items[x].To.ap.PathIsCloud,
                        GroupData.items[x].TransferRequest,
                        GroupData.items[x].From.Size - 1);
                }
                else
                {
                    GroupData.items[x].From.stream = AppSetting.ManageCloud.GetFileStream("",
                        GroupData.items[x].From.Fileid,
                        GroupData.items[x].To.ap.PathIsCloud,
                        GroupData.items[x].TransferRequest,
                        GroupData.items[x].From.Size - 1,
                        fromfolder.TypeCloud,
                        AppSetting.settings.GetDefaultCloud(fromfolder.TypeCloud));
                }
                #endregion

                int buffer_length = 32;//default
                int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.BufferSize), out buffer_length);//get buffer_length from setting
                GroupData.items[x].buffer = new byte[buffer_length * 1024];//create buffer

                GroupData.items[x].byteread = 0;
                string token = "";
                if (GroupData.items[x].To.ap.PathIsCloud) token = AppSetting.settings.GetToken(GroupData.items[x].To.ap.Email, GroupData.items[x].To.ap.TypeCloud);
                //this.group.items[x].UploadID = "";//remuse
                GroupData.items[x].Transfer = GroupData.items[x].OldTransfer = GroupData.items[x].TransferRequest;//remuse
                GroupData.items[x].ErrorMsg = "";//clear error
                GroupData.items[x].Timestamp = CurrentMillis.Millis;
                if (GroupData.status != StatusTransfer.Running) return;
                switch (GroupData.items[x].To.ap.TypeCloud)
                {
                    case CloudName.LocalDisk:
                        #region LocalDisk
                        ItemsTransferWork.Add(new TransferBytes(GroupData.items[x], this));
                        return;
                    #endregion

                    case CloudName.Dropbox:
                        #region Dropbox

                        int chunksizedb = 25;//default
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

                    case CloudName.GoogleDrive:
                        #region GoogleDrive
                        DriveAPIHttprequestv2 gdclient = new DriveAPIHttprequestv2(JsonConvert.DeserializeObject<TokenGoogleDrive>(token));
                        gdclient.Email = GroupData.items[x].To.ap.Email;
                        gdclient.TokenRenewEvent += GoogleDrive.Gdclient_TokenRenewEvent;

                        if (GoogleDrive.CreateFolder(GroupData.items[x].To.ap.TypeCloud.ToString() + ":" + GroupData.items[x].To.ap.Email + GroupData.items[x].To.ap.Parent) != GroupData.items[x].To.ap.Parent)
                            throw new Exception("Can't create folder: " + GroupData.items[x].To.ap.TypeCloud.ToString() + ":" + GroupData.items[x].To.ap.Email + GroupData.items[x].To.ap.Parent);
                        
                        int chunksizeGD = 5;//default
                        int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.GD_ChunksSize), out chunksizeGD);
                        GroupData.items[x].ChunkUploadSize = chunksizeGD * 1024 * 1024;
                        
                        if (string.IsNullOrEmpty(GroupData.items[x].UploadID))//create upload id
                        {
                            string parentid = GoogleDrive.GetIdOfPath(GroupData.items[x].To.ap.Parent, GroupData.items[x].To.ap.Email);
                            string mimeType = Get_mimeType.Get_mimeType_From_FileExtension(GroupData.items[x].To.ap.GetExtensionFile());
                            string jsondata = "{\"title\": \"" + GroupData.items[x].From.ap.NameLastItem + "\", \"mimeType\": \"" + mimeType + "\", \"parents\": [{\"id\": \"" + parentid + "\"}]}";
                            GroupData.items[x].UploadID = gdclient.Files_insert_resumable_getUploadID(jsondata, mimeType, GroupData.items[x].From.Size);
                        }
                        ItemsTransferWork.Add(new TransferBytes(GroupData.items[x], this, gdclient));
                        return;
                        #endregion
                }
            }
            catch (Exception ex)
            { GroupData.items[x].ErrorMsg = ex.Message +ex.StackTrace; GroupData.items[x].status = StatusTransfer.Error; return; }
        }
    }
}