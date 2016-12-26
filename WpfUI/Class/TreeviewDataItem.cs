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
        public static List<Bitmap> list_bm = new List<Bitmap>()
        {
                WpfUI.Properties.Resources.hard_drive_disk_icon_256x256,
                WpfUI.Properties.Resources.folder_closed64x64,
                WpfUI.Properties.Resources.Dropbox256x256,
                WpfUI.Properties.Resources.Google_Drive_Icon256x256,
                WpfUI.Properties.Resources.MegaSync
        };
        public string Name { get; set; }
        public ImageSource ImgSource { get; set; }
        public CloudName Type { get; set; }

        public TreeviewDataItem(string name, CloudName type)
        {
            Name = name;
            ImgSource = Setting_UI.GetImage(list_bm[(int)type]).Source;
            Type = type;
        }
    }
}
