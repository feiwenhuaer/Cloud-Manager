using System.Collections.Generic;
using System.Drawing;

namespace CloudManagerGeneralLib
{
    public static class ListBitmapImageResource
    {
        public static List<Bitmap> list_bm_cloud = new List<Bitmap>()//CloudName
        {
                CloudManagerGeneralLib.Properties.Resources.hard_drive_disk_icon_256x256,
                CloudManagerGeneralLib.Properties.Resources.folder_closed64x64,
                CloudManagerGeneralLib.Properties.Resources.Dropbox256x256,
                CloudManagerGeneralLib.Properties.Resources.Google_Drive_Icon256x256,
                CloudManagerGeneralLib.Properties.Resources.MegaSync
        };
        public static List<Bitmap> list_bm_localdisk = new List<Bitmap>()//DiskType
        {
            null,
            null,
            CloudManagerGeneralLib.Properties.Resources.usb,
            null,
            null,
            CloudManagerGeneralLib.Properties.Resources.cdrom_mount
        };
    }
    
}
