using Newtonsoft.Json;

namespace SupDataDll.Class.Mega
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
