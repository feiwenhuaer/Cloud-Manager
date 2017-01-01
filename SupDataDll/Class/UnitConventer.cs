using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupDataDll
{
    public static class UnitConventer
    {
        public static string[] unit_size = { "Byte", "Kb", "Mb", "Gb", "Tb" };
        public static string[] unit_speed = { "Byte/s", "Kb/s", "Mb/s", "Gb/s", "Tb/s" };

        public static string ConvertSize(decimal num, int round, string[] unit, int div = 1024)
        {
            if (num == 0) { return "0 " + unit[0]; }
            else if (num < 0) throw new Exception(num.ToString() + " < 0");
            for (double i = 0; i < unit.Length; i++)
            {
                decimal sizeitem = Math.Round(num / (decimal)Math.Pow(div, i), round);
                if (sizeitem == 0) return "0 " + unit[0];
                else if (sizeitem < 1)
                {
                    return Math.Round((num / (decimal)Math.Pow(div, i - 1)), round).ToString() + " " + unit[(int)i - 1];
                }
            }
            return Math.Round((decimal)num / (decimal)Math.Pow(div, unit.Length - 1), round).ToString() + " " + unit[unit.Length - 1];
        }
    }
}
