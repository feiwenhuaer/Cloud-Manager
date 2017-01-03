using SupDataDll;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WpfUI.Class
{
    public class TreeviewDataItem
    {
        public string Name { get; set; }
        public ImageSource ImgSource { get; set; }
        public CloudName Type { get; set; }

        public TreeviewDataItem(string name, CloudName cloud_type, DiskType disk_type = DiskType.Cloud)
        {
            Name = name;
            if (cloud_type != CloudName.LocalDisk) ImgSource = Setting_UI.GetImage(ListBitmapImageResource.list_bm_cloud[(int)cloud_type]).Source;
            else
            {
                if (ListBitmapImageResource.list_bm_localdisk[(int)disk_type] != null) ImgSource = Setting_UI.GetImage(ListBitmapImageResource.list_bm_localdisk[(int)disk_type]).Source;
                else ImgSource = Setting_UI.GetImage(ListBitmapImageResource.list_bm_cloud[(int)cloud_type]).Source;
            }
            Type = cloud_type;
        }
    }
}
