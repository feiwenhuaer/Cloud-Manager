﻿namespace Cloud.MegaNz
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

        public Stream GetRequestRaw(Uri url, long start_pos = -1, long end_pos = -1)
        {
            HttpWebRequest request = this.CreateRequest(url);
            request.Method = "GET";
            if (start_pos >= 0 && end_pos >= 0 && start_pos <= end_pos) request.AddRange(start_pos, end_pos);
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
