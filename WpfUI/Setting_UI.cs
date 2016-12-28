using SupDataDll;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace WpfUI
{
    public class Setting_UI : SupDataDll.UiInheritance.SettingUI
    {
        public static System.Windows.Controls.Image GetImage(Bitmap bmp,double Width=16,double Height=16)
        {
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            image.Source = bi;
            image.Width = Width;
            image.Height = Height;
            return image;
        }

        public static System.Windows.Controls.Image GetImage(Icon ico, double Width = 16, double Height = 16)
        {
            return GetImage(ico.ToBitmap(), Width, Height);
        }
    }
}
