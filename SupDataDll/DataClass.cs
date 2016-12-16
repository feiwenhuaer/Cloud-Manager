using Aga.Controls.Tree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SupDataDll
{
    #region Account
    public class ListAccountCloud
    {
        public List<CloudEmail_Type> account = new List<CloudEmail_Type>();
    }

    public class CloudEmail_Type
    {
        public string Email;
        public CloudName Type;
    }
    #endregion

    #region ListItem (explorer)
    public class ExplorerListItem
    {
        public string path;
        public bool addToTV = false;
        public bool explandTV = false;
        public object TV_data;
        public object TV_node;
        public string id;
        public int indexLV_tab = -1;
    }

    public class OldPathLV
    {
        public OldPathLV(string ID, string Path)
        {
            this.ID = ID;
            this.Path = Path;
        }
        public string ID { get; set; }
        public string Path { get; set; }
    }

    public class ListItemFileFolder
    {
        public List<FileFolder> Items = new List<FileFolder>();
        public string path_raw;
        public string id_folder;
        public string NameFolder = "";
    }
    
    public class FileFolder
    {
        public string Name;
        public DateTime Time_mod;
        public long Size = -1;
        public string mimeType;
        public List<string> parentid = new List<string>();
        public string id;
        public string path_display;
    }
    #endregion

    #region Transfer
    public class UpDownloadItem
    {
        public UpDownloadItem(string name,string id,string mimeType, Type_FileFolder type,long size = -1)
        {
            this.name = name;
            this.id = id;
            this.mimeType = mimeType;
            this.type = type;
            this.size = size;
        }
        public string name;
        public string id;
        public long size = -1;
        public string mimeType;
        public Type_FileFolder type;
    }
    
    public class UD_group_work
    {
        // MaxItemDownload
        public int MaxItemsDownload = 2;
        //TreeListView
        public string Name = "";
        public List<string> col { get; set; }
        //Speed & timeleft
        public long TotalFileLength = 0;
        public long OldTransfer = 0;
        public long Timestamp = 0;
        //Status
        public StatusUpDown status = StatusUpDown.Loading;
        public StatusUpDown CheckChangeStatus = StatusUpDown.Loading;
        public ChangeTLV change = ChangeTLV.Processing;
        //Items
        public List<UD_item_work> items = new List<UD_item_work>();
    }


    public class UD_item_work
    {
        public List<string> col { get; set; }
        public long Timestamp = 0;
        public UD_item_work_info From = new UD_item_work_info();
        public UD_item_work_info To = new UD_item_work_info();
        public StatusUpDown status = StatusUpDown.Waiting;
        public StatusUpDown CheckChangeStatus = StatusUpDown.Waiting;
        public string UploadID = "";
        public string ErrorMsg = "";
        public long Transfer = 0;
        public long OldTransfer = 0;
        public long TransferRequest = 0;
        public Thread item_work;
        public string SizeString = "";
    }

    public class UD_data_WPF : ITreeModel
    {
        List<UD_group_work> ud_groups = new List<UD_group_work>();

        public UD_data_WPF(UD_group_work group)
        {
            ud_groups.Add(group);
        }

        public void Add(UD_group_work ud_group)
        {
            ud_groups.Add(ud_group);
        }

        public void Remove(UD_group_work ud_group)
        {
            if (ud_groups.IndexOf(ud_group) > -1)
                ud_groups.Remove(ud_group);
        }

        public IEnumerable GetChildren(object parent)
        {
            var pr = parent as UD_group_work;
            if (parent == null)
            {
                foreach (UD_group_work group in ud_groups)
                {
                    yield return group;
                }
            }
            else if (pr != null)
            {
                foreach (UD_item_work item in pr.items)
                {
                    yield return item;
                }
            }
        }

        public bool HasChildren(object parent)
        {
            return parent is UD_group_work;
        }
    }

    public class UD_item_work_info
    {
        public string filename;
        public string path;
        public string Fileid;
        public CloudName TypeCloud;
        public string email;
        public long Size = -1;
    }
    #endregion
    
    public class DeleteItems
    {
        public DeleteItems()
        {

        }
        public DeleteItems(string item)
        {
            items.Add(item);
        }

        public DeleteItems(List<string> items)
        {
            this.items.AddRange(items);
        }
        public List<string> items = new List<string>();
        public bool PernamentDelete;
    }
}
