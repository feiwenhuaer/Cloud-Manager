using System.Collections.Generic;

namespace Cloud.Dropbox
{
    public class BuildJson
    {
        private string json = null;
        public string GetJson { get { return "{" + json + "}"; } }

        public void AddChildNodes(string Nodes, string Value)
        {
            if (json != null)
            {
                json += ", \"" + Nodes + "\": " + Value;
            }
            else
            {
                json = "\"" + Nodes + "\": " + Value;
            }
        }

        public void AddChildStringNodes(string Nodes, string Value)
        {
            if (json != null)
            {
                json += ", \"" + Nodes + "\": \"" + Value + "\"";
            }
            else
            {
                json = "\"" + Nodes + "\": \"" + Value + "\"";
            }
        }

        public void AddListChildNodes(string Nodes, List<string> ListValue)
        {
            if (json != null)
            {
                json += "\"" + Nodes + "\": [" + GetValueFromList(ListValue) + "]";
            }
            else
            {
                json = "\"" + Nodes + "\": [" + GetValueFromList(ListValue) + "]";
            }
        }

        private string GetValueFromList(List<string> ListValue)
        {
            string data = null;
            foreach (string Value in ListValue)
            {
                if (data != null)
                {
                    data += ",";
                }
                data += Value;
            }
            return data;
        }
    }
}
