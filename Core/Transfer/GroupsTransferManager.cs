using Core.StaticClass;
using Newtonsoft.Json;
using SupDataDll;
using SupDataDll.Class;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace Core.Transfer
{
    public delegate void updateclosingform(string text);
    public delegate void closeapp();
    public class GroupsTransferManager
    {
        public static int TimeRefresh = 500;
        public static int KillTheadTime = 15000000;

        List<ItemsTransferManager> GroupsWork;
        
        #region Start up app
        public Thread MainThread;
        //Start after login
        public void Start()
        {
            GroupsWork = new List<ItemsTransferManager>();
            ReadData();
            MainThread = new Thread(LoadMainThread);
            MainThread.Start();
        }

        long timestamp;
        bool Loop = true;
        public bool AuToStartGroupMode = true;
        public StatusUpDownApp status = StatusUpDownApp.Pause; //UploadDownloadItems
        private void LoadMainThread()
        {
            int count_group_running = 0;
            int count = 0;
            bool flag_shutdown = false;
            bool flag_waitUIcloseShow = false;
            timestamp = CurrentMillis.Millis;
            bool lockkillthr = false;
            while (Loop)
            {
                Thread.Sleep(100);
                if (status == StatusUpDownApp.Pause) continue;
                for (int i = 0; i < LoadGroupThreads.Count; i++)
                {
                    if (!LoadGroupThreads[i].IsAlive)
                    {
                        LoadGroupThreads.RemoveAt(i);
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
                        GroupsWork.ForEach(s =>
                        {
                            if (s.GroupData.status == StatusTransfer.Running) count_group_running++;
                            else if (s.GroupData.status == StatusTransfer.Started | s.GroupData.status == StatusTransfer.Waiting | s.GroupData.status == StatusTransfer.Loading) count++;
                        });

                        for (int i = 0; i < GroupsWork.Count; i++)
                        {
                            if (AuToStartGroupMode && GroupsWork[i].GroupData.status == StatusTransfer.Waiting && count_group_running < int.Parse(AppSetting.settings.GetSettingsAsString(SettingsKey.MaxGroupsDownload)))//auto
                            {
                                int.TryParse(AppSetting.settings.GetSettingsAsString(SettingsKey.MaxItemsInGroupDownload), out GroupsWork[i].GroupData.MaxItemsDownload);
                                GroupsWork[i].GroupData.status = StatusTransfer.Started;
                                count_group_running++;
                            }

                            if (GroupsWork[i].GroupData.status == StatusTransfer.Remove & GroupsWork[i].ThreadsItemLoadWork.Count == 0 &
                                GroupsWork[i].ItemsTransferWork.Count == 0)
                            {
                                AppSetting.uc_lv_ud_instance.RemoveGroup(this.GroupsWork[i].GroupData);
                                GroupsWork[i].GroupData.items.Clear();
                                this.GroupsWork.RemoveAt(i);
                                i--;
                                continue;
                            }

                            GroupsWork[i].ManagerItemsAndRefreshData();
                        }
                        bool ShutdownWhenDone = AppSetting.settings.GetSettingsAsBool(SettingsKey.ShutdownWhenDone);
                        if (!flag_shutdown && count_group_running > 0 && ShutdownWhenDone) flag_shutdown = true;
                        if (flag_shutdown && !ShutdownWhenDone) flag_shutdown = false;
                        if (flag_shutdown && ShutdownWhenDone && (count_group_running + count) == 0)
                        {
                            SaveData();
#if DEBUG
                            Console.WriteLine("shutdown");
#else

#endif
                            return;
                        }

                        if (CurrentMillis.Millis - timestamp > TimeRefresh)
                        {
                            timestamp = CurrentMillis.Millis;
                            AppSetting.uc_lv_ud_instance.RefreshAll();
                            SaveData();
                        }
                        #endregion
                        break;
                    case StatusUpDownApp.StopForClosingApp:
                        #region Stop
                        Eventupdateclosingform(AppSetting.lang.GetText(LanguageKey.CloseThread.ToString()));
                        if (!flag_waitUIcloseShow) { Thread.Sleep(1000); flag_waitUIcloseShow = true; }
                        int ItemsRunningCount = 0;
                        foreach (ItemsTransferManager group in GroupsWork)
                        {
                            switch (group.GroupData.status)
                            {
                                case StatusTransfer.Running:
                                case StatusTransfer.Started:
                                case StatusTransfer.Waiting: group.GroupData.status = StatusTransfer.Stop; break;
                                default:break;
                            }
                            if (lockkillthr) KillThreads(group.ThreadsItemLoadWork);
                            group.ManagerItemsAndRefreshData();//clean thread
                            group.GroupData.items.ForEach(s => 
                            {
                                if (s.status == StatusTransfer.Running) { s.status = StatusTransfer.Stop; ItemsRunningCount++; }
                            });
                            ItemsRunningCount += group.ThreadsItemLoadWork.Count;
#if DEBUG
                            Console.WriteLine("ItemsRunningCount:" + ItemsRunningCount.ToString());
#endif
                        }
                        ItemsRunningCount += this.LoadGroupThreads.Count;
                        //if (lockkillthr) KillThreads(threads);
                        if (ItemsRunningCount == 0) this.status = StatusUpDownApp.SavingData;
                        else if (CurrentMillis.Millis - timestamp > KillTheadTime & !lockkillthr) lockkillthr = true;
                        #endregion
                        break;
                    case StatusUpDownApp.SavingData:
                        #region SavingData
                        Eventupdateclosingform(AppSetting.lang.GetText(LanguageKey.SaveData.ToString()));
                        SaveData();
#if DEBUG
                        Console.WriteLine("GroupsTransferManager Thread Closed.");
#endif
                        Eventcloseapp();
                        #endregion
                        return;
                    default: break;
                }
            }
        }
        #endregion

        #region Add new items from UI
        public List<Thread> LoadGroupThreads = new List<Thread>();
        public void AddItems(List<ExplorerNode> items, ExplorerNode fromfolder, ExplorerNode savefolder, bool AreCut)
        {
            ItemsTransferManager gr = new ItemsTransferManager(items, fromfolder, savefolder, AreCut);
            GroupsWork.Add(gr);
            Thread thr = new Thread(GroupsWork[GroupsWork.IndexOf(gr)].LoadListItems);
            thr.Start();
            LoadGroupThreads.Add(thr);
        }
        #endregion

        #region load/reload UI -> add groups to treelistview
        public void LoadGroupToListView()
        {
            foreach (ItemsTransferManager gr in GroupsWork)
            {
                AppSetting.uc_lv_ud_instance.AddNewGroup(gr.GroupData);
            }
        }
        #endregion

        #region Kill thread
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
        #endregion

        #region Closing Form
        public void Exit()
        {
            timestamp = CurrentMillis.Millis;
        }
        public event updateclosingform Eventupdateclosingform;
        public event closeapp Eventcloseapp;
        #endregion

        #region Save/Read Data When Close/Open program
        string temp_jsonSaveData = "";
        public void ReadData()
        {
            if (ReadWriteData.Exists(ReadWriteData.File_DataUploadDownload))
            {
                var readerjson = ReadWriteData.Read(ReadWriteData.File_DataUploadDownload);
                if (readerjson != null)
                {
                    List<JsonDataSaveGroup> json_groups = JsonConvert.DeserializeObject<List<JsonDataSaveGroup>>(readerjson.ReadToEnd());
                    foreach (JsonDataSaveGroup json_group in json_groups)
                    {
                        ItemsTransferManager group = new ItemsTransferManager(json_group);
                        this.GroupsWork.Add(group);
                    }
                }
            }
        }
        public void SaveData()
        {
            List<JsonDataSaveGroup> json_groups = new List<JsonDataSaveGroup>();
            for (int i = 0; i < GroupsWork.Count; i++)
            {
                if (GroupsWork[i].GroupData.status == StatusTransfer.Loading) continue;
                JsonDataSaveGroup json_item = new JsonDataSaveGroup();
                json_item.fromfolder = GroupsWork[i].fromfolder;
                json_item.savefolder = GroupsWork[i].savefolder;
                json_item.Group = GroupsWork[i].GroupData;
                json_item.AreCut = GroupsWork[i].AreCut;
                json_groups.Add(json_item);
            }
            string json = JsonConvert.SerializeObject(json_groups);
            if (temp_jsonSaveData == json) return;
            else temp_jsonSaveData = json;
            ReadWriteData.Write(ReadWriteData.File_DataUploadDownload, Encoding.UTF8.GetBytes(json));
        }
        #endregion
    }
}
