using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomHttpRequest
{
  public class HeaderField
  {
    public string FieldName { get; set; }
    public string FieldData { get; set; }

    public static HeaderField Parse(string header_line)
    {
      int index = header_line.IndexOf(": ");
      if (index > 0) return new HeaderField()
      {
        FieldName = header_line.Substring(0, index),
        FieldData = header_line.Substring(index + 2, header_line.Length - index - 2)
      };
      else throw new Exception("Error parse header, check your input again: " + header_line);
    }    
  }


  public static class Extension_
  {
    public static string GetTextDataHeader(this List<HeaderField> Headers)
    {
      string text_header = "";
      foreach (HeaderField f in Headers) text_header += f.FieldName + ": " + f.FieldData + "\r\n";
      return text_header;
    }

    public static List<HeaderField> SetHeaderRequestFromResponse(this List<HeaderField> Headers)
    {
      List<HeaderField> list = new List<HeaderField>();
      foreach (HeaderField f in Headers)
        if (f.FieldName.ToLower().IndexOf("set-") == 0)
          list.Add(new HeaderField() { FieldData = f.FieldData, FieldName = f.FieldName.Substring(4, f.FieldName.Length - 4) });
      return list;
    }

    public static HeaderField FindHeaderName(this List<HeaderField> Headers, string Name)
    {
      foreach (HeaderField h in Headers) if (h.FieldName.ToLower().IndexOf(Name.ToLower()) >= 0) return h;
      return null;
    }
  }
}
