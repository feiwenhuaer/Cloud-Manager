using SupDataDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WpfUI
{
    public class LV_data
    {
        public ImageSource ImgSource { get; set; }
        public string Name { get; set; }
        public Type_FileFolder Type { get; set; }
        public long Size { get; set; }
        public string mimeType { get; set; }
        public string id { get; set; }
        public string d_mod { get; set; }
    }
}
