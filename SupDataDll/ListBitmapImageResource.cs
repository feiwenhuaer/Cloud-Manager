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
    public enum CloudName
    {
        LocalDisk = 0,
        Folder = 1,
        Dropbox = 2,
        GoogleDrive = 3,
        Mega = 4,
        MediaFire = 5
    }
    public enum DiskType
    {
        Unknown = 0,
        NoRootDirectory = 1,
        Removable = 2,
        Fixed = 3,
        Network = 4,
        CDRom = 5,
        Ram = 6,
        Cloud = 7
    }
}
