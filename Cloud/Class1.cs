using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud
{
    public static class Class1
    {
        public static Uri BuildUri(this string url)
        {
            return new Uri(url);
        }
    }
}
