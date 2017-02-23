using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SupDataDll
{
    public static class ListBitmapImageResource
    {
        public static List<Bitmap> list_bm_cloud = new List<Bitmap>()//CloudName
        {
                SupDataDll.Properties.Resources.hard_drive_disk_icon_256x256,
                SupDataDll.Properties.Resources.folder_closed64x64,
                SupDataDll.Properties.Resources.Dropbox256x256,
                SupDataDll.Properties.Resources.Google_Drive_Icon256x256,
                SupDataDll.Properties.Resources.MegaSync
        };
        public static List<Bitmap> list_bm_localdisk = new List<Bitmap>()//DiskType
        {
            null,
            null,
            SupDataDll.Properties.Resources.usb,
            null,
            null,
            SupDataDll.Properties.Resources.cdrom_mount
        };
    }
    
}
