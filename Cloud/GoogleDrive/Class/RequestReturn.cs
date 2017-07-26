using Newtonsoft.Json;
using System;
using System.IO;

namespace Cloud.GoogleDrive
{
  internal class RequestReturn: IRequestReturn
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
