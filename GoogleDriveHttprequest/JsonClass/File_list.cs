using System.Collections.Generic;

namespace GoogleDriveHttprequest.JsonClass
{
    public class GD_Files_list
    {
        public string nextPageToken;
        public List<GD_item> items = new List<GD_item>();
    }

    public class GD_item
    {
        public string title;
        public string modifiedDate;
        public List<GD_parent> parents = new List<GD_parent>();
        public string mimeType;
        public GD_label labels = new GD_label();
        public long fileSize = -1;
        public string id;
        //public string originalFilename;
        public string fileExtension;
        public string fullFileExtension;
    }

    public class GD_parent
    {
        public string parentLink;
        public bool isRoot = false;
        public string id;
    }

    public class GD_label
    {
        public bool starred = false;
        public bool hidden = false;
        public bool trashed = false;
        public bool restricted = false;
        public bool viewed = true;
    }
}
