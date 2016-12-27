using Core.cloud;
using Core.StaticClass;
using DropboxHttpRequest;
using GoogleDriveHttprequest;
using Newtonsoft.Json;
using SupDataDll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Web;

namespace Core
{
    public class UploadDownloadItems
    {
        public Thread MainThread;
        public StatusUpDownApp status = StatusUpDownApp.Pause; //UploadDownloadItems
        public List<GroupTransferItemsProcess> groups = new List<GroupTransferItemsProcess>();
        public List<Thread> threads = new List<Thread>();
        public bool AuToStartGroupMode = true;
        public bool Loop = true;
        long timestamp;
        public void Start()
        {
            MainThread = new Thread(LoadMainThread);
            MainThread.Start();
        }
        
        private void LoadMainThread()
        {
            int count_group_running = 0;
            int count = 0;
            bool flag_shutdown = false;
            timestamp = CurrentMillis.Millis;
            bool lockkillthr = false;
            ReadData();
            //int count_group_done = 0;
            while (Loop)
            {
                GC.Collect();
                Thread.Sleep(100);
                if (status == StatusUpDownApp.Pause) continue;
                for (int i = 0; i < threads.Count; i++)
                {
                    if (!threads[i].IsAlive)
                    {
                        threads.RemoveAt(i);
                        i--;
                    }
                }
                switch (status)//UploadDownloadItems
                {
                    case StatusUpDownApp.Start:
                        #region Start
                        //count
                        count_group_running = 0;
                        count = 0;
                        groups.ForEach(s => 
                        {
                            if (s.group.status == StatusUpDown.Running) count_group_running++;
                            else if (s.group.status == StatusUpDown.Started | s.group.status == StatusUpDown.Waiting | s.group.status == StatusUpDown.Loading) count++;
                        });

                        for (int i = 0; i < groups.Count; i++)
                        {
                            if (AuToStartGroupMode && groups[i].group.status == StatusUpDown.Waiting && count_group_running < int.Parse(AppSetting.settings.GetSettingsAsString(SettingsKey.MaxGroupsDownload)))//auto
                            {
                                int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.MaxItemsInGroupDownload),out groups[i].group.MaxItemsDownload);
                                groups[i].group.status = StatusUpDown.Started;
                                count_group_running++;
                            }

                            if (groups[i].group.status == StatusUpDown.Removing & groups[i].threads.Count == 0)
                            {
                                AppSetting.uc_lv_ud_instance.RemoveGroup(this.groups[i].group);
                                this.groups.RemoveAt(i);
                                i--;
                                continue;
                            }

                            groups[i].CheckStartDownloadItem();

                            if (!flag_shutdown && count_group_running > 0 && AppSetting.settings.GetSettingsAsString(SettingsKey.ShutdownWhenDone) == "1") flag_shutdown = true;
                            if (flag_shutdown && AppSetting.settings.GetSettingsAsString(SettingsKey.ShutdownWhenDone) == "0") flag_shutdown = false;
                            count = count_group_running + count;
                            if (flag_shutdown && AppSetting.settings.GetSettingsAsString(SettingsKey.ShutdownWhenDone) == "1" && count == 0)
                            {
                                SaveData();
                                Console.WriteLine("shutdown");
                                //Process.Start("shutdown.exe", "-s -t 30");
                                return;
                            }
                        }
                        if (CurrentMillis.Millis - timestamp > 500)
                        {
                            timestamp = CurrentMillis.Millis;
                            AppSetting.uc_lv_ud_instance.RefreshAll();
                        }
                        SaveData();
                        #endregion
                        break;
                    case StatusUpDownApp.StopForClosingApp:
                        #region Stop
                        Eventupdateclosingform(AppSetting.lang.GetText(LanguageKey.CloseThread.ToString()));
                        int thrcount = 0;
                        foreach (GroupTransferItemsProcess group in groups)
                        {
                            if (group.group.status == StatusUpDown.Running | group.group.status == StatusUpDown.Started |
                                group.group.status == StatusUpDown.Waiting ) group.group.status = StatusUpDown.Stop;
                            if (lockkillthr) KillThreads(group.threads);
                            group.CheckStartDownloadItem();//clean thread
                            thrcount += group.threads.Count;
                        }
                        thrcount += this.threads.Count;
                        //if (lockkillthr) KillThreads(threads);
                        if (thrcount == 0) this.status = StatusUpDownApp.SavingData;
                        else if (CurrentMillis.Millis - timestamp > 5000 & !lockkillthr) lockkillthr = true;
                        #endregion
                        break;
                    case StatusUpDownApp.SavingData:
                        #region SavingData
                        Eventupdateclosingform(AppSetting.lang.GetText(LanguageKey.SaveData.ToString()));
                        //Thread.Sleep(200);
                        SaveData();
                        Eventcloseapp();
                        #endregion
                        return;
                    default: break;
                }
            }
        }
        
        public void AddItems(List<UpDownloadItem> items, string fromfolder_raw, string savefolder_raw, bool AreCut)
        {
            GroupTransferItemsProcess gr = new GroupTransferItemsProcess(items, fromfolder_raw, savefolder_raw, AreCut);
            groups.Add(gr);
            Thread thr = new Thread(Loaditem);
            thr.Start(groups.IndexOf(gr));
            threads.Add(thr);
        }
        public void Loaditem(object obj)
        {
            groups[(int)obj].AddItems();
        }
        
        public void LoadGroupToListView()
        {
            foreach(GroupTransferItemsProcess gr in groups)
            {
                AppSetting.uc_lv_ud_instance.AddNewGroup(gr.group);
            }
        }
        
        void KillThreads(List<Thread> thrs)
        {
            for (int i = 0; i < thrs.Count; i++)
            {
                if (thrs[i] != null && thrs[i].IsAlive) KillThread(thrs[i]);
            }
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        void KillThread(Thread thr)
        {
            thr.Abort();
        }
        
        #region Closing Form
        public void Exit()
        {
            timestamp = CurrentMillis.Millis;
        }
        public delegate void updateclosingform(string text);
        public event updateclosingform Eventupdateclosingform;
        public delegate void closeapp();
        public event closeapp Eventcloseapp;
        #endregion

        #region Save/Read Data When Close/Open program
        public void ReadData()
        {
            if (ReadWriteData.Exists(ReadWriteData.File_DataUploadDownload))
            {
                Json_Data_UDGroup json_groups = JsonConvert.DeserializeObject<Json_Data_UDGroup>(ReadWriteData.Read(ReadWriteData.File_DataUploadDownload).ReadToEnd());
                foreach (Group_Json_Data_UDItem json_group in json_groups.groups)
                {
                    GroupTransferItemsProcess group = new GroupTransferItemsProcess(json_group);
                    this.groups.Add(group);
                }
            }
        }

        string temp_jsonSaveData = "";
        public void SaveData()
        {
            if (this.groups.Count == 0) return;
            Json_Data_UDGroup json_groups = new Json_Data_UDGroup();
            for (int i = 0; i < groups.Count;i++)
            {
                if (groups[i].group.status == StatusUpDown.Loading) continue;
                Group_Json_Data_UDItem json_item = new Group_Json_Data_UDItem();
                json_item.fromfolder_raw = groups[i].fromfolder.Path_Raw;
                json_item.savefolder_raw = groups[i].savefolder.Path_Raw;
                json_item.Group = groups[i].group;
                json_item.AreCut = groups[i].AreCut;
                json_groups.groups.Add(json_item);
            }
            string json = JsonConvert.SerializeObject(json_groups);
            if (temp_jsonSaveData == json) return;
            else temp_jsonSaveData = json;
            ReadWriteData.Write(ReadWriteData.File_DataUploadDownload, Encoding.UTF8.GetBytes(json));
        }
        #endregion
    }

    public class GroupTransferItemsProcess
    {
        public UD_group_work group = new UD_group_work();
        bool addGroup_toTLV = false;
        List<UpDownloadItem> items;
        public AnalyzePath fromfolder;
        public AnalyzePath savefolder;
        public bool AreCut = false;
        public List<Thread> threads = new List<Thread>();
        
        internal GroupTransferItemsProcess(Group_Json_Data_UDItem group_json)
        {
            addGroup_toTLV = true;
            this.group = group_json.Group;
            this.fromfolder = new AnalyzePath(group_json.fromfolder_raw);
            this.AreCut = group_json.AreCut;
            this.savefolder = new AnalyzePath(group_json.savefolder_raw);
            
            this.group.status = (group_json.Group.status == StatusUpDown.Done | group_json.Group.status == StatusUpDown.Error) ? group_json.Group.status : StatusUpDown.Stop;
            foreach(UD_item_work item in this.group.items)
            {
                if (item.status == StatusUpDown.Running) item.status = StatusUpDown.Stop;
                item.col[3] = item.status.ToString();
            }
            this.group.change = ChangeTLV.Done;
            RefreshGroup(-1);
        }
        public GroupTransferItemsProcess(List<UpDownloadItem> items, string fromfolder_raw, string savefolder_raw,bool AreCut = false)
        {
            if (items.Count == 0) throw new Exception("List<UpDownloadItem> items count = 0");
            if (string.IsNullOrEmpty(fromfolder_raw)) throw new ArgumentNullException(fromfolder_raw);
            if (string.IsNullOrEmpty(savefolder_raw)) throw new ArgumentNullException(savefolder_raw);
            this.items = items;
            this.fromfolder = new AnalyzePath(fromfolder_raw);
            this.savefolder = new AnalyzePath(savefolder_raw);
            this.AreCut = AreCut;
        }
        public void AddItems()
        {
            this.group.col = new List<string> { fromfolder.Path_Raw, savefolder.Path_Raw, this.group.status.ToString(), "0/0", "", "", "" };
            foreach (UpDownloadItem item in items)
            {
                if (item.type == Type_FileFolder.File)
                    group.items.Add(LoadFile(fromfolder.PathIsUrl? fromfolder.ID:fromfolder.Path_Raw,item.name,item.size, item.id));
                else
                {
                    //if(string.IsNullOrEmpty(item.name) & items.Count == 1){ListAllItemInFolder(rp_from.Path_Raw, "");break;}
                    group.items.AddRange(
                        ListAllItemInFolder(
                            fromfolder.PathIsUrl? fromfolder.ID + "/"+ item.name : fromfolder.Path_Raw + (fromfolder.PathIsCloud ? "/" : "\\") + item.name, 
                            item.id
                            ));
                }
            }
            group.status = StatusUpDown.Waiting;
        }
        private List<UD_item_work> ListAllItemInFolder(string path_rawItem,string id = "")
        {
            ListItemFileFolder list;
            List<UD_item_work> ud_items = new List<UD_item_work>();
            try
            {
                if(fromfolder.PathIsUrl) list = AppSetting.ManageCloud.GetItemsList("", id);
                else list = AppSetting.ManageCloud.GetItemsList(path_rawItem, id);//UnauthorizedAccessException
            }catch(UnauthorizedAccessException ex) { return ud_items; }
            
            foreach (FileFolder ffitem in list.Items)
            {
                if (ffitem.Size == -1)//folder size = -1
                    ud_items.AddRange(
                        ListAllItemInFolder(
                            fromfolder.PathIsUrl ? path_rawItem +"/"+ffitem.Name : path_rawItem + (fromfolder.PathIsCloud ? "/" : "\\") + ffitem.Name,
                            ffitem.id));
                else
                    ud_items.Add(
                        LoadFile(
                            path_rawItem,
                            ffitem.Name,
                            ffitem.Size,
                            ffitem.id));
            }
            return ud_items;
        }

        //Path_Parent : id/folder/folder or GD:a@gmail.com/folder/folder
        private UD_item_work LoadFile(string Path_Parent,string FileName,long size,string FileId)//Path_raw path parent folder of file
        {
            UD_item_work ud_item = new UD_item_work();
            //From
            ud_item.From.path = Path_Parent + (fromfolder.PathIsCloud? "/":"\\") + FileName;// id/filename or GD:a@gmail.com/filename
            ud_item.From.Fileid = FileId;
            ud_item.From.email = fromfolder.PathIsUrl ? AppSetting.settings.GetDefaultCloud(fromfolder.TypeCloud) : fromfolder.Email;// if url -> get defaul email
            ud_item.From.TypeCloud = fromfolder.TypeCloud;
            ud_item.From.Size = size;
            ud_item.From.filename = FileName;
            //group & UI
            group.TotalFileLength += size;
            ud_item.SizeString = UnitConventer.ConvertSize(size, 2, UnitConventer.unit_size);
            ud_item.status = StatusUpDown.Waiting;
            //To
            ud_item.To.filename = FileName;
            ud_item.To.path = AnalyzePath.GetPathTo(fromfolder.PathIsUrl ? ud_item.From.path:(new AnalyzePath(ud_item.From.path)).GetPath(), fromfolder, savefolder);//
            ud_item.To.email = savefolder.Email;
            ud_item.To.TypeCloud = savefolder.TypeCloud;
            ud_item.col = new List<string> { ud_item.From.path, ud_item.To.path, ud_item.status.ToString(), "", "", "", "" };
            Group_UD_item_EventAddDataToListview();
            return ud_item;
        }
        private void Group_UD_item_EventAddDataToListview()
        {
            if (AppSetting.ud_items.status == StatusUpDownApp.StopForClosingApp | AppSetting.ud_items.status == StatusUpDownApp.SavingData) return;
            if (!addGroup_toTLV)//& index == -1)
            {
                AppSetting.uc_lv_ud_instance.AddNewGroup(group);
                addGroup_toTLV = true;
                return;
            }
        }
        public void CheckStartDownloadItem()
        {
            //clean thread
            for (int i = 0; i < threads.Count; i++)
            {
                if (!threads[i].IsAlive)
                {
                    threads.RemoveAt(i);
                    i--;
                }
            }
            
            switch(group.status)
            {
                case StatusUpDown.Loading: return;
                case StatusUpDown.Started: group.status = StatusUpDown.Running; group.Timestamp = CurrentMillis.Millis; break;
                case StatusUpDown.Remove: group.status = StatusUpDown.Removing; return;
                case StatusUpDown.Removing: return;
            }

            #region Count
            int count_item_running = 0;
            int count_item_done = 0;
            int count_item_error = 0;
            int count_item_stop = 0;
            for (int i = 0; i < group.items.Count; i++)
            {
                if (group.items[i].status == StatusUpDown.Remove)
                {
                    group.items.RemoveAt(i);
                    i--;
                    break;
                }else switch (group.items[i].status)
                    {
                        case StatusUpDown.Running: count_item_running++; break;
                        case StatusUpDown.Done: count_item_done++; break;
                        case StatusUpDown.Error: count_item_error++; break;
                        case StatusUpDown.Stop: count_item_stop++; break;
                        default: break;
                    }
                
                //clear speed download when not running
                if (group.items[i].status != StatusUpDown.Running)
                {
                    if (!string.IsNullOrEmpty(group.items[i].col[4])) group.items[i].col[4] = "";
                    if (!string.IsNullOrEmpty(group.items[i].col[5])) group.items[i].col[5] = "";
                }
            }
            #endregion

            #region Running group
            if (this.group.status == StatusUpDown.Running && count_item_done + count_item_error + count_item_stop != this.group.items.Count)
            {
                long Group_TotalTransfer = 0;
                for (int i = 0; i < group.items.Count; i++)//start item waiting and Started(force)
                {
                    Group_TotalTransfer += group.items[i].Transfer;
                    if (this.group.items[i].status == StatusUpDown.Started)// start item force start
                    {
                        Thread thr = new Thread(WorkThread);
                        this.group.items[i].status = StatusUpDown.Running;
                        thr.Start(i);
                        threads.Add(thr);
                        count_item_running++;
                    }
                    if (group.items[i].status == StatusUpDown.Waiting && count_item_running < group.MaxItemsDownload)//start item waiting
                    {
                        Thread thr = new Thread(WorkThread);
                        group.items[i].status = StatusUpDown.Running;
                        group.items[i].Timestamp = Stopwatch.GetTimestamp();
                        thr.Start(i);
                        threads.Add(thr);
                        count_item_running++;
                    }
                    #region caculate speed & time left item
                    if (this.group.items[i].status == StatusUpDown.Running)
                    {
                        long size_transfer = group.items[i].Transfer - group.items[i].OldTransfer;
                        long time_milisec = CurrentMillis.Millis - group.items[i].Timestamp;
                        if (time_milisec != 0 & time_milisec >= 500)
                        {
                            //speed
                            group.items[i].Timestamp = CurrentMillis.Millis;
                            group.items[i].OldTransfer = group.items[i].Transfer;
                            decimal speed = (decimal)size_transfer * 1000 / time_milisec;
                            group.items[i].col[4] = UnitConventer.ConvertSize(speed, 2, UnitConventer.unit_speed);
                            //time 
                            if (speed != 0)
                            {
                                long length_left = group.items[i].From.Size - group.items[i].Transfer;
                                long secondleft = length_left / decimal.ToInt64(speed);
                                group.items[i].col[5] = CurrentMillis.GetTimeBySecond((int)secondleft);
                            }
                        }
                    }
                    #endregion
                }
                #region caculate speed & time left group
                long time_milisec_group = CurrentMillis.Millis - group.Timestamp;
                if (time_milisec_group != 0 & time_milisec_group >= 500)
                {
                    //speed
                    group.Timestamp = CurrentMillis.Millis;
                    decimal speed_group = (decimal)(Group_TotalTransfer - group.OldTransfer) * 1000 / time_milisec_group;
                    group.OldTransfer = Group_TotalTransfer;
                    group.col[4] = UnitConventer.ConvertSize(speed_group, 2, UnitConventer.unit_speed);
                    //time left
                    if (speed_group != 0)
                    {
                        long length_left_group = group.TotalFileLength - Group_TotalTransfer;
                        long secondleft_group = length_left_group / decimal.ToInt64(speed_group);
                        group.col[5] = CurrentMillis.GetTimeBySecond((int)secondleft_group);
                    }
                }
                #endregion
            }
            else
            {
                if (this.group.status == StatusUpDown.Running)
                {
                    if(this.AreCut && count_item_done == this.group.items.Count)// remove cut
                    {
                        DeleteItems list = new DeleteItems();
                        list.PernamentDelete = false;
                        foreach(UpDownloadItem item in items)
                        {
                            list.items.Add(fromfolder.Path_Raw + (fromfolder.PathIsCloud ? "/" : "\\") + item.name);
                        }
                        Thread thr = new Thread(AppSetting.ManageCloud.Delete);
                        thr.Start(list);
                    }
                    this.group.status = StatusUpDown.Done;//Done group
                }
            }
            #endregion

            #region Change LV
            if (this.group.change == ChangeTLV.Processing && (this.group.status == StatusUpDown.Done |
                this.group.status == StatusUpDown.Error | this.group.status == StatusUpDown.Stop))
                this.group.change = ChangeTLV.ProcessingToDone;
            if(this.group.change == ChangeTLV.Done && (this.group.status == StatusUpDown.Started | this.group.status == StatusUpDown.Running|
                this.group.status == StatusUpDown.Waiting | this.group.status == StatusUpDown.Loading ))
                this.group.change = ChangeTLV.DoneToProcessing;
            #endregion

            RefreshGroup(count_item_done);
        }

        private void RefreshGroup(int count_item_done)
        {
            if (count_item_done != -1 & group.col[3].IndexOf("100% (") < 0 & group.items.Count != 0)
                group.col[3] = Math.Round((double)count_item_done * 100 / group.items.Count, 2).ToString() + "% (" + count_item_done.ToString() + "/" + group.items.Count.ToString() + ")";
            group.col[2] = group.status.ToString();
            for (int i =0; i< group.items.Count; i++)
            {
                group.items[i].col[2] = group.items[i].status.ToString();
                if (group.items[i].col[3].IndexOf("100% (") < 0 & group.items[i].From.Size != 0)
                    group.items[i].col[3] = Math.Round((double)group.items[i].Transfer * 100 / group.items[i].From.Size, 2).ToString() + "% (" + UnitConventer.ConvertSize(group.items[i].Transfer,2,UnitConventer.unit_size) + "/" + group.items[i].SizeString + ")";
                group.items[i].col[6] = group.items[i].ErrorMsg;
            }
        }
        
        private void WorkThread(object obj)
        {
            int x = (int)obj;
            try{
                AnalyzePath rp_from;
                AnalyzePath rp_to = new AnalyzePath(group.items[x].To.path);

                if (!fromfolder.PathIsUrl)
                {
                    rp_from = new AnalyzePath(group.items[x].From.path);
                    group.items[x].From.stream = AppSetting.ManageCloud.GetFileStream(group.items[x].From.path,
                        group.items[x].From.Fileid,
                        rp_to.PathIsCloud,
                        group.items[x].TransferRequest,
                        group.items[x].From.Size - 1);
                }
                else
                {
                    group.items[x].From.stream = AppSetting.ManageCloud.GetFileStream("",
                        group.items[x].From.Fileid,
                        rp_to.PathIsCloud,
                        group.items[x].TransferRequest,
                        group.items[x].From.Size - 1,
                        fromfolder.TypeCloud,
                        AppSetting.settings.GetDefaultCloud(fromfolder.TypeCloud));
                }
                int buffer_length = 32;//default
                int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.BufferSize), out buffer_length);
                group.items[x].buffer = new byte[buffer_length * 1024];
                int byteread = 0;
                string token = "";
                if (rp_to.PathIsCloud) token = AppSetting.settings.GetToken(rp_to.Email, rp_to.TypeCloud);
                //this.group.items[x].UploadID = "";//remuse
                group.items[x].Transfer = group.items[x].TransferRequest;//remuse
                group.items[x].ErrorMsg = "";//clear error
                group.items[x].Timestamp = CurrentMillis.Millis;
                switch (rp_to.TypeCloud)
                {
                    case CloudName.LocalDisk:
                        #region LocalDisk
                        group.items[x].To.stream = AppSetting.ManageCloud.GetFileStream(group.items[x].To.path, null,false,group.items[x].Transfer);
                        //test 
                        do
                        {
                            byteread = group.items[x].From.stream.Read(group.items[x].buffer, 0, group.items[x].buffer.Length);
                            group.items[x].To.stream.Write(group.items[x].buffer, 0, byteread);
                            group.items[x].To.stream.Flush();
                            group.items[x].Transfer += byteread;
                            group.items[x].TransferRequest = group.items[x].Transfer;
                        } while (IsStillDownloading(x) && byteread != 0 && group.items[x].Transfer < group.items[x].From.Size);

                        if (!fromfolder.PathIsCloud) group.items[x].From.stream.Close();
                        if (!savefolder.PathIsCloud) group.items[x].To.stream.Close();

                        if (group.status == StatusUpDown.Remove) { group.status = StatusUpDown.Removing; return; }
                        if (group.items[x].status == StatusUpDown.Remove) { group.items[x].status = StatusUpDown.Removing; return; }

                        
                        if (group.status == StatusUpDown.Stop) group.items[x].status = StatusUpDown.Stop;
                        if (group.items[x].status == StatusUpDown.Stop){//if (info.Exists) info.Delete();
                            return;}

                        //check size
                        FileInfo info = new FileInfo(group.items[x].To.path);
                        if (info.Length != group.items[x].From.Size) group.items[x].status = StatusUpDown.Error;
                        else group.items[x].status = StatusUpDown.Done;
                        return;
                    #endregion

                    case CloudName.Dropbox:
                        #region Dropbox
                        int chunksizedb = 25;//default
                        int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.Dropbox_ChunksSize), out chunksizedb);
                        int size_db = chunksizedb * 1024*1024;
                        DropboxRequestAPIv2 client = new DropboxRequestAPIv2(token);
                        byteread = group.items[x].From.stream.Read(group.items[x].buffer, 0, group.items[x].buffer.Length);
                        db_createupload:
                        int loop = 0;
                        if (string.IsNullOrEmpty(group.items[x].UploadID))
                        {
                            dynamic json = JsonConvert.DeserializeObject(client.upload_session_start(group.items[x].buffer, byteread));
                            group.items[x].UploadID = json.session_id;
                            group.items[x].Transfer += byteread;
                            loop = (int)((group.items[x].From.Size - byteread) / (long)size_db);
                            if (((group.items[x].From.Size - byteread) % (long)size_db) != 0) loop++;
                        }
                        else
                        {
                            loop = (int)((group.items[x].From.Size - group.items[x].TransferRequest) / (long)size_db);
                            if (((group.items[x].From.Size - group.items[x].TransferRequest) % (long)size_db) != 0) loop++;
                        }

                        for (int i = 0; i < loop; i++)
                        {
                            try
                            {
                                group.items[x].To.stream = client.upload_session_append(group.items[x].UploadID,
                                    group.items[x].From.Size - group.items[x].Transfer > size_db ? size_db : group.items[x].From.Size - group.items[x].Transfer,
                                    group.items[x].Transfer);
                            }catch(HttpException http_ex) {
                                if (http_ex.ErrorCode == 404) { group.items[x].UploadID = ""; goto db_createupload; }
                                throw http_ex;
                            }
                            int temp_send = 0;
                            do
                            {
                                byteread = group.items[x].From.stream.Read(group.items[x].buffer, 0, (size_db - temp_send) > group.items[x].buffer.Length ? group.items[x].buffer.Length : (size_db - temp_send));
                                group.items[x].To.stream.Write(group.items[x].buffer, 0, byteread);
                                group.items[x].To.stream.Flush();
                                group.items[x].Transfer += byteread;
                                temp_send += byteread;
                            } while (IsStillDownloading(x) & temp_send != size_db & group.items[x].Transfer != group.items[x].From.Size);
                            client.GetResponse_upload_session_append();

                            if (group.status == StatusUpDown.Remove) { group.status = StatusUpDown.Removing; return; }
                            if (group.items[x].status == StatusUpDown.Remove) { group.items[x].status = StatusUpDown.Removing; return; }
                            if (group.status == StatusUpDown.Stop) group.items[x].status = StatusUpDown.Stop;
                            if (group.items[x].status != StatusUpDown.Stop) { if (!fromfolder.PathIsCloud) group.items[x].From.stream.Close(); return; }

                            group.items[x].TransferRequest = group.items[x].Transfer;
                        }
                        if (!fromfolder.PathIsCloud) group.items[x].From.stream.Close();
                        //create folder if not found
                        if(Dropbox.AutoCreateFolder(rp_to.GetPath(), rp_to.Email) != rp_to.GetPath())
                        {
                            group.items[x].ErrorMsg = "Failed to create folder: " + rp_to.GetPath(); 
                            group.items[x].status = StatusUpDown.Error;
                            return;
                        }
                        dynamic json_ = JsonConvert.DeserializeObject(client.upload_session_finish(null, group.items[x].UploadID, group.items[x].Transfer, rp_to.GetPath(), DropboxUploadMode.add));
                        long size = json_.size;
                        if (size == group.items[x].From.Size) group.items[x].status = StatusUpDown.Done;
                        else group.items[x].status = StatusUpDown.Error;
                        return;
                    #endregion

                    case CloudName.GoogleDrive:
                        #region GoogleDrive
                        DriveAPIHttprequestv2 gdclient = new DriveAPIHttprequestv2(JsonConvert.DeserializeObject<TokenGoogleDrive>(token));
                        gdclient.Email = rp_to.Email;
                        gdclient.TokenRenewEvent += GoogleDrive.Gdclient_TokenRenewEvent;
                        gdclient.Debug = true;

                        if (GoogleDrive.CreateFolder(rp_to.TypeCloud.ToString() + ":" + rp_to.Email + rp_to.Parent) != rp_to.Parent)
                            throw new Exception("Can't create folder: " + rp_to.TypeCloud.ToString() + ":" + rp_to.Email + rp_to.Parent);

                        string parentid = GoogleDrive.GetIdOfPath(rp_to.Parent, rp_to.Email);
                        int chunksizeGD = 5;//default
                        int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.GD_ChunksSize), out chunksizeGD);
                        int size_gd = chunksizeGD * 1024 * 1024;
                        string mimeType = Get_mimeType.Get_mimeType_From_FileExtension(fromfolder.GetExtensionFile());
                        string jsondata = "{\"title\": \"" + group.items[x].From.filename + "\", \"mimeType\": \""+ mimeType + "\", \"parents\": [{\"id\": \""+ parentid + "\"}]}";
                        #region gd_loop
                        int gd_loop = 0;
                        if (string.IsNullOrEmpty(group.items[x].UploadID))
                        {
                            group.items[x].UploadID = gdclient.Files_insert_resumable_getUploadID(jsondata, mimeType, group.items[x].From.Size);
                            gd_loop = (int)((group.items[x].From.Size) / (long)size_gd);
                            if (((group.items[x].From.Size) % (long)size_gd) != 0) gd_loop++;
                        }
                        else
                        {
                            gd_loop = (int)((group.items[x].From.Size - group.items[x].TransferRequest) / (long)size_gd);
                            if (((group.items[x].From.Size - group.items[x].TransferRequest) % (long)size_gd) != 0) gd_loop++;
                        }
                        #endregion

                        #region Processing
                        for (int i = 0; i < gd_loop; i++)
                        {
                            group.items[x].To.stream = gdclient.Files_insert_resumable( group.items[x].UploadID, 
                                                                        group.items[x].TransferRequest, 
                                                                        (i != gd_loop-1) ? group.items[x].TransferRequest + size_gd - 1 : group.items[x].From.Size - 1,
                                                                        group.items[x].From.Size);
                            int temp_send = 0;
                            int sizechunk = (i != gd_loop - 1) ? size_gd : (int)(group.items[x].From.Size - group.items[x].TransferRequest);
                            do
                            {
                                byteread = group.items[x].From.stream.Read(group.items[x].buffer, 0, sizechunk - temp_send > group.items[x].buffer.Length ? group.items[x].buffer.Length : sizechunk - temp_send);
                                group.items[x].To.stream.Write(group.items[x].buffer, 0, byteread);
                                group.items[x].To.stream.Flush();
                                group.items[x].Transfer += byteread;
                                temp_send += byteread;
                            } while (IsStillDownloading(x) && byteread != 0 && temp_send != sizechunk);

                            gdclient.GetResponse_Files_insert_resumable();//get header response
                            group.items[x].To.stream.Close();

                            if (group.items[x].status == StatusUpDown.Running) group.items[x].TransferRequest = group.items[x].Transfer;
                            if (group.status == StatusUpDown.Remove) { group.status = StatusUpDown.Removing; return; }
                            if (group.items[x].status == StatusUpDown.Remove) { group.items[x].status = StatusUpDown.Removing; return; }
                            if (group.status == StatusUpDown.Stop) group.items[x].status = StatusUpDown.Stop;
                            if (group.items[x].status == StatusUpDown.Stop) return;
                        }
                        group.items[x].From.stream.Close();
                        //if (!rp_from.IsCloud) StreamFrom.Close();
                        if (this.group.items[x].Transfer != this.group.items[x].From.Size) group.items[x].status = StatusUpDown.Error; 
                        else group.items[x].status = StatusUpDown.Done;
                        #endregion
                        return;
                        #endregion
                }
            }catch (Exception ex) { group.items[x].ErrorMsg = ex.Message; group.items[x].status = StatusUpDown.Error; return; }
        }

        bool IsStillDownloading(int x)
        {
            return ((group.status == StatusUpDown.Running || group.status == StatusUpDown.Waiting || group.status == StatusUpDown.Started) && group.items[x].status == StatusUpDown.Running);
        }
    }
    
    #region Json Save/Load Data
    public class Json_Data_UDGroup
    {
        public List<Group_Json_Data_UDItem> groups = new List<Group_Json_Data_UDItem>();
    }

    public class Group_Json_Data_UDItem
    {
        public string fromfolder_raw;
        public string savefolder_raw;
        public bool AreCut = false;
        public UD_group_work Group = new UD_group_work();
    }
    #endregion
    

    public class TransferByte
    {
        UD_item_work item;
        object clientTo;
        public TransferByte(UD_item_work item,object clientTo)
        {
            this.item = item;
            this.clientTo = clientTo;
            //Make Stream To
            if (item.ChunkUpload > 0) MakeNextChunkStreamTo(true);
            else this.item.To.stream = AppSetting.ManageCloud.GetFileStream(item.To.path, null, false, item.Transfer);
            //begin transfer
            item.status = StatusUpDown.Running;
            item.From.stream.BeginRead(item.buffer, 0, item.buffer.Length, new AsyncCallback(GetFrom), 0);
        }

        public void GetFrom(IAsyncResult result)
        {
            item.byteread = item.From.stream.EndRead(result);
            if (item.ChunkUpload < 0) item.To.stream.Seek(item.Transfer, SeekOrigin.Begin);//if download to disk then seek.
            item.To.stream.Write(item.buffer, 0, item.byteread);
            item.Transfer += item.byteread;
            if (item.Transfer == item.From.Size | item.status != StatusUpDown.Running)//transfer done/force stop.
            {
                if (item.status == StatusUpDown.Running) item.TransferRequest = item.Transfer;
                item.From.stream.Close();
                if (item.ChunkUpload > 0)//if upload
                {
                    switch(item.To.TypeCloud)
                    {
                        case CloudName.Dropbox: ((DropboxRequestAPIv2)clientTo).GetResponse_upload_session_append(); break;
                        case CloudName.GoogleDrive: ((DriveAPIHttprequestv2)clientTo).GetResponse_Files_insert_resumable(); break;
                        default: throw new Exception("Not support");
                    }
                }else item.To.stream.Close();//close file in disk
                item.status = StatusUpDown.Done;
                return;
            }

            if (item.ChunkUpload > 0)//if upload
            {
                int totalchunkupload = (int)result.AsyncState;
                totalchunkupload += item.byteread;
                if (totalchunkupload == item.ChunkUpload)
                {
                    MakeNextChunkStreamTo();
                }
                int nexbyteread = item.ChunkUpload - totalchunkupload >= item.buffer.Length ? item.buffer.Length : item.ChunkUpload - totalchunkupload;
                item.From.stream.BeginRead(item.buffer, 0, nexbyteread, new AsyncCallback(GetFrom), totalchunkupload);
            }
            else
            {
                item.TransferRequest = item.Transfer;
                item.From.stream.BeginRead(item.buffer, 0, item.buffer.Length, new AsyncCallback(GetFrom), 0);
            }
        }

        void MakeNextChunkStreamTo(bool CreateNew = false)
        {
            if (item.ChunkUpload <= 0) throw new Exception("Not upload type");
            long pos_end = item.Transfer + item.ChunkUpload - 1;
            if (pos_end >= item.From.Size) pos_end = item.From.Size - 1;

            switch (item.To.TypeCloud)
            {
                case CloudName.Dropbox:
                    if (!CreateNew) ((DropboxRequestAPIv2)clientTo).GetResponse_upload_session_append();//get data return from server
                    item.To.stream = ((DropboxRequestAPIv2)clientTo).upload_session_append(item.From.Fileid, pos_end - item.Transfer + 1, item.Transfer);
                    break;

                case CloudName.GoogleDrive:
                    if (!CreateNew) ((DriveAPIHttprequestv2)clientTo).GetResponse_Files_insert_resumable();//get data return from server
                    item.To.stream = ((DriveAPIHttprequestv2)clientTo).Files_insert_resumable(item.From.Fileid, item.Transfer, pos_end, item.From.Size);
                    break;


                default:throw new Exception("Not support.");
            }
            item.TransferRequest = item.Transfer;//
        }

    }
}
