using Newtonsoft.Json;

namespace Cloud.MegaNz
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
