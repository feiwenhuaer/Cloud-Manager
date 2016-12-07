using SupDataDll;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WPF.UI
{
    public class TreeviewDataItem
    {
        static List<Bitmap> list_bm = new List<Bitmap>()
        {
                Properties.Resources.hard_drive_disk_icon_256x256,
                Properties.Resources.folder_closed64x64,
                Properties.Resources.Dropbox256x256,
                Properties.Resources.Google_Drive_Icon256x256,
                Properties.Resources.MegaSync
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
