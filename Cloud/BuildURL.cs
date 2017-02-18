namespace Cloud
{
    internal class BuildURL
    {
        private string url;
        public string Url { get { return url; } }

        public BuildURL(string url)
        {
            this.url = url;
        }

        public void AddParameter(string key, string value)
        {
            if (url.IndexOf("?") > 0)
            {
                url += "&" + key + "=" + value;
                return;
            }
            url += "?" + key + "=" + value;
        }
    }
}
