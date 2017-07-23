using System;
using System.Collections.Generic;

namespace Cloud
{
  public class JsonBuilder
  {
    public JsonBuilder() { }
    public JsonBuilder(JsonItem item)
    {
      Items.Add(item);
    }

    public List<JsonItem> Items { get; set; } = new List<JsonItem>();


    public string GetJson()
    {
      if (Items.Count == 0) throw new Exception("JsonItem is empty.");
      string json = "";
      foreach (JsonItem item in Items)
      {
        if (!item.Check()) throw new Exception("JsonItem is empty.");
        if (!string.IsNullOrEmpty(item.Data))
        {
          json += (string.IsNullOrEmpty(json) ? "" : ",") + "\"" + item.Value + "\": \"" + item.Data + "\"";
        }
        else
        {
          string array_item = "";
          foreach (JsonBuilder build in item.ArrayData) array_item += (string.IsNullOrEmpty(array_item) ? "" : ",") + build.GetJson();
          json += (string.IsNullOrEmpty(json) ? "" : ",") + "\"" + item.Value + "\": [" + array_item + "]";
        }
      }
      return "{" + json + "}";
    }
  }

  public class JsonItem
  {
    string ex = "Accept only Data or ArrayData";
    string data;
    List<JsonBuilder> ArrayData_ = new List<JsonBuilder>();

    public string Value { get; set; }
    public string Data { get { return data; } set { if (ArrayData_.Count != 0) throw new Exception(ex); else data = value; } }
    public List<JsonBuilder> ArrayData { get { return ArrayData_; } set { if (string.IsNullOrEmpty(Data)) ArrayData_ = value; else throw new Exception(ex); } }

    public bool Check()
    {
      if (string.IsNullOrEmpty(Value) | (string.IsNullOrEmpty(Value) & ArrayData.Count == 0)) return false;
      return true;
    }
  }
}
