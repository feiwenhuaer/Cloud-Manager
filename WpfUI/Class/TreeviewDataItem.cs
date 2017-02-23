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
        public ExplorerNode Node { get; set; }
        public ImageSource ImgSource { get; set; }
        public CloudType Type { get; set; }

        public TreeviewDataItem(ExplorerNode Node)
        {
            this.Type = Node.GetRoot().RootInfo.Type;
            //if (Node.GetRoot().RootInfo.Type != CloudType.LocalDisk)
                ImgSource = Setting_UI.GetImage(ListBitmapImageResource.list_bm_cloud[(int)this.Type]).Source;
            //else
            //{
            //    if (ListBitmapImageResource.list_bm_localdisk[(int)disk_type] != null) ImgSource = Setting_UI.GetImage(ListBitmapImageResource.list_bm_localdisk[(int)disk_type]).Source;
            //    else ImgSource = Setting_UI.GetImage(ListBitmapImageResource.list_bm_cloud[(int)cloud_type]).Source;
            //}
            //Type = cloud_type;
        }
    }
}
