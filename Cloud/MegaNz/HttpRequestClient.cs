namespace Cloud.MegaNz
{
    using CustomHttpRequest;
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    public class HttpRequestClient : IHttpRequestClient
    {
        private const int DefaultResponseTimeout = Timeout.Infinite;

        private readonly int responseTimeout;
        private readonly string userAgent;

        public HttpRequestClient() : this(DefaultResponseTimeout)
        {
            this.BufferSize = MegaApiClient.DefaultBufferSize;
        }

        internal HttpRequestClient(int responseTimeout)
        {
            this.responseTimeout = responseTimeout;
            this.userAgent = this.GenerateUserAgent();
        }

        public int BufferSize { get; set; }

        public string PostRequestJson(Uri url, string jsonData)
        {
            using (MemoryStream jsonStream = new MemoryStream(jsonData.ToBytes()))
            {
                return this.PostRequest(url, jsonStream, "application/json");
            }
        }

        public string PostRequestRaw(Uri url, Stream dataStream)
        {
            return this.PostRequest(url, dataStream, "application/octet-stream");
        }
        HttpRequest_ request;
        public Stream PostRequestRaw(Uri uri, int contentlength, long Pos_Start = 0, long Pos_End = 0)
        {
            if (Pos_End < 0 || Pos_Start < 0) throw new Exception("Pos are <0.");
            if (Pos_End < Pos_Start) throw new Exception("Pos_End are < Pos_Start.");
            if ((int)(Pos_End - Pos_Start + 1) != contentlength) throw new Exception("contentlength is incorrect.");

            request = new HttpRequest_(uri, "POST");
            request.AddHeader("HOST", uri.Host);
            request.AddHeader("Content-Type", "application/octet-stream");
            request.AddHeader("Content-Length", contentlength.ToString());
            request.AddHeader("Range", "bytes=" + Pos_Start.ToString() + "-" + Pos_End.ToString());
            return request.SendHeader_And_GetStream();
        }

        public string GetDataResponseUpload()
        {
            if (request == null) throw new Exception("PostRequestRaw(,,,) is not work.");
            return request.GetTextDataResponse(true, true);
        }

        public Stream GetRequestRaw(Uri url)
        {
            HttpWebRequest request = this.CreateRequest(url);
            request.Method = "GET";

            return request.GetResponse().GetResponseStream();
        }

        private string PostRequest(Uri url, Stream dataStream, string contentType)
        {
            HttpWebRequest request = this.CreateRequest(url);
            request.ContentLength = dataStream.Length;
            request.Method = "POST";
            request.ContentType = contentType;

            using (Stream requestStream = request.GetRequestStream())
            {
                dataStream.Position = 0;
                dataStream.CopyTo(requestStream, this.BufferSize);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }

        private HttpWebRequest CreateRequest(Uri url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = this.responseTimeout;
            request.UserAgent = this.userAgent;

            return request;
        }

        private string GenerateUserAgent()
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            return string.Format("{0} v{1}", assemblyName.Name, assemblyName.Version.ToString(2));
        }
    }
}
