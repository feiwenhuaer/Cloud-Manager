using System.Collections.Generic;

namespace Cloud.GoogleDrive.JsonClass
{
    public class GD_Files_list
    {
        public string nextPageToken;
        public List<GD_ItemMetaData> items = new List<GD_ItemMetaData>();
    }

    public class GD_ItemMetaData
    {
        public string title { get; set; }
        public string modifiedDate { get; set; }
        public List<GD_ParentMetaData> parents { get; set; } = new List<GD_ParentMetaData>();
        public string mimeType { get; set; }
        public GD_LabelMetaData labels { get; set; } = new GD_LabelMetaData();
        public long fileSize { get; set; } = -1;
        public string id { get; set; }
        //public string originalFilename;
        public string fileExtension { get; set; }
        public string fullFileExtension { get; set; }
    }

    public class GD_ParentMetaData
    {
        public string parentLink;
        public bool isRoot = false;
        public string id;
    }

    public class GD_LabelMetaData
    {
        public bool starred = false;
        public bool hidden = false;
        public bool trashed = false;
        public bool restricted = false;
        public bool viewed = true;
    }
}
