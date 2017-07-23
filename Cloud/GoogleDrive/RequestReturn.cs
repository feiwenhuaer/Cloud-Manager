using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cloud.GoogleDrive
{
  public class RequestReturn
  {
    public string HeaderResponse { get; internal set; }
    public string DataTextResponse { get; internal set; }
    public Stream stream { get; internal set; }
    public T GetObjectResponse<T>()
    {
      try
      {
        return JsonConvert.DeserializeObject<T>(this.DataTextResponse);
      }
      catch (Exception)
      {
        return default(T);
      }
    }
  }
}
