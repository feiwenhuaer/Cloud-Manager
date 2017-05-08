using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudManagerGeneralLib
{
    public static class PCPath
    {
        public static readonly string Mycomputer = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer); //"::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
        public const string FilterAllFiles = "All files (*.*)|*.*";
    }
}
