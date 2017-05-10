using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.StaticClass
{
    public static class CurrentMillis
    {
        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>Get extra long current timestamp</summary>
        public static long Millis { get { return (long)((DateTime.UtcNow - Jan1St1970).TotalMilliseconds); } }

        public static string GetTimeBySecond(long second)
        {
            long sec = second % (long)60;
            long minute = (second - sec) / 60;
            long min = minute % 60;
            long hour = (minute - min) / 60;
            long hr = hour % 24;
            long days = (hour - hr) / 24;
            string text = "";
            if (days != 0) text += days.ToString() + " days ";
            text += hr.ToString() + ":" + min.ToString() + ":" + sec.ToString();
            return text;
        }
    }
}
