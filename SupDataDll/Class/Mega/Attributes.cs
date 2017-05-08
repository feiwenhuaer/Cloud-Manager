using Newtonsoft.Json;

namespace CloudManagerGeneralLib.Class.Mega
{
    public class Attributes
    {
        public Attributes(string name)
        {
            this.Name = name;
        }

        [JsonProperty("n")]
        public string Name { get; set; }
    }
}
