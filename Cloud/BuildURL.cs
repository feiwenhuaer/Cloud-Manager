using System;

namespace Cloud
{
    internal class BuildURL
    {
        private string url;
        public string Url { get { return url; } private set { url = value; uri = new Uri(value); } }

        public Uri uri { get; private set; }

        public BuildURL(string url)
        {
            this.Url = url;
        }

        public void AddParameter(string key, string value)
        {
            if (Url.IndexOf("?") > 0)
            {
                Url += "&" + key + "=" + value;
                return;
            }
            Url += "?" + key + "=" + value;
        }
    }
}
