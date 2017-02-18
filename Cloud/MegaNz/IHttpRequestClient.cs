namespace Cloud.MegaNz
{
  using System;
  using System.IO;

  public interface IHttpRequestClient
  {
    int BufferSize { get; set; }

    string PostRequestJson(Uri url, string jsonData);

    string PostRequestRaw(Uri url, Stream dataStream);

    Stream GetRequestRaw(Uri url);
  }
}
