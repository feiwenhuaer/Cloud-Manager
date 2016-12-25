using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CustomHttpRequest
{
    public class HttpRequest_
    {
        public bool debug = false;
        Uri uri;

        public HttpRequest_(string url, string RequestMethod)
        {
            if (string.IsNullOrEmpty(url)) throw new NullReferenceException("url can't be null or empty");
            this.url = url;
            this.requestmethod = RequestMethod;
            MakeNewHeader(url, RequestMethod);
        }

        private string url;
        /// <summary>
        /// Get/Set url
        /// </summary>
        public string URL { get { return url; } set { url = value; } }

        /// <summary>
        /// Get/Set RequestMethod
        /// </summary>
        public string RequestMethod { get { return requestmethod; } set { requestmethod = value; } }
        string requestmethod;

        /// <summary>
        /// Get Data Respone
        /// </summary>
        public string TextDataResponse { get { return textdataresponse; } }
        string textdataresponse;


        #region Connection & SSL
        /// <summary>
        /// Get/Set ReceiveTimeout of tcpclient (default 10000ms)
        /// </summary>
        public int ReceiveTimeout { get { return receive_timeout; } set { receive_timeout = value; } }
        int receive_timeout = 10000;

        /// <summary>
        /// Get/Set SendTimeout of tcpclient (default 10000ms)
        /// </summary>
        public int SendTimeout { get { return send_timeout; } set { send_timeout = value; } }
        int send_timeout = 10000;

        public TcpClient tcp = new TcpClient();
        SslStream sslStream;
        bool ssl = false;
        void ConnectToHost()
        {
            if (tcp.Connected) return;
            try
            {
                tcp = new TcpClient();
                tcp.ReceiveTimeout = receive_timeout;
                tcp.SendTimeout = send_timeout;
                tcp.Client.NoDelay = true;
                uri = new Uri(this.url);
                ssl = url.ToLower().IndexOf("https://") == 0;
                tcp.Connect(uri.Host, ssl ? 443 : 80);
                debugwrite("TcpClient Connected", "uri.Host: " + uri.Host + ", RemoteEndPoint: " + tcp.Client.RemoteEndPoint.ToString());
                if (ssl)
                {
                    sslStream = new SslStream(tcp.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                    try
                    {
                        sslStream.AuthenticateAsClient(uri.Host);
                    }
                    catch (AuthenticationException e)
                    {
                        tcp.Close();
                        throw e;
                    }
                }
            }catch(Exception ex) { throw ex; }
        }
        bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) return true;
            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
        }
        
        /// <summary>
        /// Get stream from tcpclient
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            ConnectToHost();
            if (!ssl)
            {
                return tcp.GetStream();
            }
            else
            {
                return sslStream;
            }
        }
        #endregion

        
        #region Count byte send/receive
        long byte_receive = 0;
        long byte_send = 0;
        long byte_header_receive = 0;
        long byte_header_send = 0;
        public long ByteDataReceive { get { return byte_receive - byte_header_receive; } }
        public long ByteDataSend { get { return byte_send - byte_header_send; } }
        public long TotalByteSend { get { return byte_send; } }
        public long TotalByteReceive { get { return byte_receive; } }
        #endregion



        #region HeaderSend
        bool WasSendHeader = false;
        public string HeaderSend { get { return header_send; } set { header_send = value; } }
        string header_send = "";

        public void AddHeader(string HeadName, string Data)
        {
            header_send += HeadName + ": " + Data + "\r\n";
        }
        public void AddHeader(string Header)
        {
            header_send += Header + "\r\n";
        }
        public void AddHeader(string[] Header)
        {
            foreach (string h in Header)
            {
                header_send += h + "\r\n";
            }
        }

        /// <summary>
        /// Make new request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="RequestMethod"></param>
        public void MakeNewHeader(string url, string RequestMethod)
        {
            if (tcp.Connected) tcp.Close();
            uri = new Uri(url);
            header_send = RequestMethod.ToUpper() + " " + uri.PathAndQuery + " HTTP/1.1\r\n";
        }
        #endregion



        #region HeaderReceive
        bool WasReceiveHeader = false;

        /// <summary>
        /// Get HeaderReceive
        /// </summary>
        public string HeaderReceive { get { return header_receive; } }
        string header_receive = "";

        /// <summary>
        /// Get status code.
        /// </summary>
        /// <returns></returns>
        public int GetCodeResponse()
        {
            if(WasReceiveHeader) return int.Parse(Regex.Split(GetHeaderResponse()[0], " ")[1]);
            return -1;
        }

        /// <summary>
        /// Get size data will receive.
        /// </summary>
        /// <returns></returns>
        public long GetContentLengthResponse()
        {
            if (!WasReceiveHeader) return -1;
            List<string> ls = GetHeaderDataResponse("content-length");
            string length = ls.Count == 0 ? null : ls[0];
            return length == null ? (long)-1 : long.Parse(length);
        }

        /// <summary>
        /// Get Array String Header Response
        /// </summary>
        /// <returns></returns>
        public string[] GetHeaderResponse()
        {
            if (!string.IsNullOrEmpty(header_receive) & WasReceiveHeader)
            {
                return Regex.Split(header_receive, "\r\n");
            }
            throw new Exception("Header is null");
        }
        
        public List<string> GetHeaderDataResponse(string HeaderName)
        {
            List<string> data = new List<string>();
            foreach (string h in GetHeaderResponse())
            {
                if (h.ToLower().IndexOf(HeaderName.ToLower() + ": ") >= 0)
                {
                    data.Add(Regex.Split(h, ": ")[1]);
                }
            }
            return data;
        }
        #endregion
        

        void ResetWasSendReceive()
        {
            WasReceiveHeader = false;
            WasSendHeader = false;
            header_receive = "";
            textdataresponse = "";
        }

        #region Send
        private void SendHeader(Stream stream)
        {
            if (WasSendHeader) return;
            byte[] req = Encoding.UTF8.GetBytes(header_send + "\r\n");
            stream.Write(req, 0, req.Length);
            WasSendHeader = true;
            debugwrite("send header", header_send);
        }
        /// <summary>
        /// Send header and Get Stream Upload.
        /// </summary>
        /// <returns></returns>
        public Stream SendHeader_And_GetStream()
        {
            Stream stream = GetStream();
            SendHeader(stream);
            debugwrite("Stream response status: read:", stream.CanRead.ToString() + ", write:" + stream.CanWrite.ToString());
            return stream;
        }
        #endregion

        #region Receive
        byte[] buffer = new byte[1];
        byte[] buffer_header = new byte[8 * 1024];

        /// <summary>
        /// Send header -> Receive header response
        /// </summary>
        /// <param name="SendDataHeader"></param>
        /// <param name="CheckStatusCode">HttpException, Ignore 200 and 206</param>
        /// <returns>Data Stream Download</returns>
        public Stream ReadHeaderResponse_and_GetStreamResponse(bool SendDataHeader = true, bool CheckStatusCode = false)
        {
            Stream st;
            st = GetStream();
            byte_receive = 0;
            byte_send = 0;
            byte_header_receive = 0;
            byte_header_send = 0;
            if (SendDataHeader) SendHeader(st);
            if (!WasReceiveHeader)
            {
                int byteread = 0;
                try
                {
                    do //Receive header
                    {
                        byteread = st.Read(buffer, 0, buffer.Length);
                        Array.Copy(buffer, 0, buffer_header, byte_receive, byteread);
                        byte_receive += byteread;
                        byte_header_receive += byteread;
                        if (byte_receive < 4) continue;
                        if (buffer_header[(int)byte_receive - 4] == 13 && buffer_header[(int)byte_receive - 3] == 10 &&
                            buffer_header[(int)byte_receive - 2] == 13 && buffer_header[(int)byte_receive - 1] == 10) { break; }
                    } while (true);
                }
                catch (Exception ex) { throw ex; }
                header_receive = Encoding.UTF8.GetString(buffer_header, 0, (int)byte_header_receive).TrimStart(' ');
                debugwrite("header response.", header_receive);
                WasReceiveHeader = true;
            }

            if (!CheckStatusCode) return st;
            else
            {
                int statuscode = GetCodeResponse();
                if (statuscode == 200 | statuscode == 206) return st;
                textdataresponse = ReadDataResponseText(CheckMethodTransfer());
                throw new HttpException(statuscode, "Error code "+ statuscode.ToString()+"\r\n"+textdataresponse);
            }
        }
        
        public void ReadHeaderResponse_and_GetStreamResponse(AsyncCallback callback, bool SendDataHeader = true, bool CheckStatusCode = false)
        {
            Stream st = GetStream();
            byte_receive = 0;
            byte_send = 0;
            byte_header_receive = 0;
            byte_header_send = 0;
            if (SendDataHeader) SendHeader(st);
            if (!WasReceiveHeader) st.BeginRead(buffer, 0, buffer.Length, BeginRead, callback);
        }

        void BeginRead(IAsyncResult asyncResult)
        {
            Stream st = GetStream();
            int byteread = st.EndRead(asyncResult);
            buffer_header[byte_receive] = buffer[0];
            byte_receive += byteread;
            //check
            if(byte_receive > 4)
            {
                if (buffer_header[(int)byte_receive - 4] == 13 && buffer_header[(int)byte_receive - 3] == 10 &&
                            buffer_header[(int)byte_receive - 2] == 13 && buffer_header[(int)byte_receive - 1] == 10)
                {
                    header_receive = Encoding.UTF8.GetString(buffer_header, 0, (int)byte_receive-4).TrimStart(' ');
                    WasReceiveHeader = true;

                    ReceiveHeaderIAsyncResult a = new ReceiveHeaderIAsyncResult(st);
                    ((AsyncCallback)asyncResult.AsyncState).Invoke(a);
                    return;
                }
            }
            st.BeginRead(buffer, 0, buffer.Length, BeginRead, null);
        }


        /// <summary>
        /// Send header -> Receive header -> Read text data response.
        /// </summary>
        /// <param name="AutoDirect"></param>
        /// <param name="CheckStatusCode"></param>
        /// <returns></returns>
        public string GetTextDataResponse(bool AutoDirect = true,bool CheckStatusCode = false)
        {
            if (!WasSendHeader) SendHeader_And_GetStream();
            bool redirect = false;
        ReRequest:
            List<string> HeaderOldRequest = new List<string>(Regex.Split(header_send, "\r\n"));
            HeaderOldRequest.RemoveAt(0);
            HeaderOldRequest = RemoveOldDataHeader(HeaderOldRequest);

            ReadHeaderResponse_and_GetStreamResponse();
            
            ReadDataResponseText(CheckMethodTransfer());
            int response_code = GetCodeResponse();
            if (response_code == 200) return textdataresponse;
            if (response_code == 301 | response_code == 302) redirect = true; else redirect = false;
            if (AutoDirect & redirect)
            {
                MakeNewHeader(GetHeaderDataResponse("location")[0], this.requestmethod);
                MoveDataHeaderResponeToRequest(HeaderOldRequest);
                ResetWasSendReceive();
                goto ReRequest;
            }
            if (!CheckStatusCode) return textdataresponse;
            throw new HttpException(response_code, "Error code " + response_code.ToString() +"\r\n"+ textdataresponse);
        }

        /// <summary>
        /// Read text data response.
        /// </summary>
        /// <returns></returns>
        public string ReadDataResponseText()
        {
            if (!WasReceiveHeader) throw new NullReferenceException("header_receive: " + header_receive);
            return ReadDataResponseText(CheckMethodTransfer());
        }

        private string ReadDataResponseText(MethodTransfer mt)
        {
            try
            {
                Stream stream = GetStream();
                int receive = 0;
                int byteread = 0;
                byte[] buffer;
                byte[] temp_buff = new byte[32];
                if (mt == MethodTransfer.Chunk)
                {
                    byte[] chunk_buffer = new byte[18];
                    byte[] trash_buffer = new byte[2];
                    buffer = new byte[5 * 1024 * 1024];
                back:
                    stream.Read(chunk_buffer, 0, 1);
                    if (chunk_buffer[0] == 48)
                    {
                        //stream.Read(chunk_buffer, 1, 4);
                        textdataresponse = Encoding.UTF8.GetString(buffer, 0, receive);
                        tcp.Close();
                        debugwrite("TCP closed, Data text response:", textdataresponse);
                        return textdataresponse;
                    }
                    int offset = 1;
                    while (offset < chunk_buffer.Length)
                    {
                        stream.Read(chunk_buffer, offset, 1);
                        if (chunk_buffer[offset - 1] == 13 && chunk_buffer[offset] == 10) break;//if crlf
                        offset += 1;
                    }
                    int chunk_length = int.Parse(Encoding.ASCII.GetString(chunk_buffer, 0, offset - 1), System.Globalization.NumberStyles.HexNumber);
                    int sub_receive = 0;
                    do
                    {
                        byteread = stream.Read(temp_buff, 0, chunk_length - sub_receive < 32 ? chunk_length - sub_receive : 32);
                        Array.Copy(temp_buff, 0, buffer, receive, byteread);
                        receive += byteread;
                        sub_receive += byteread;
                    } while (sub_receive != chunk_length);
                    stream.Read(trash_buffer, 0, 2);
                    goto back;
                }
                else if (mt == MethodTransfer.ContentLength)
                {
                    int ContentLengthResponse = (int)GetContentLengthResponse();
                    buffer = new byte[ContentLengthResponse];
                    if (ContentLengthResponse != 0)
                    {

                        do
                        {
                            byteread = stream.Read(temp_buff, 0, temp_buff.Length);
                            Array.Copy(temp_buff, 0, buffer, receive, byteread);
                            receive += byteread;
                        } while (receive != GetContentLengthResponse());
                    }
                    textdataresponse = Encoding.UTF8.GetString(buffer);
                    debugwrite("TCP closed, Data text response", textdataresponse);
                    tcp.Close();
                    return textdataresponse;
                }
                else return "";
            }catch(Exception ex) { throw ex; }
        }//read data text receive
        #endregion
        
        #region SubMethod check,data processing,...
        private List<string> RemoveOldDataHeader(List<string> headerlist)
        {
            for (int i = 0; i < headerlist.Count; i++)
            {
                foreach (string h in ListHeaderRemove)
                {
                    if (headerlist[i].ToLower().IndexOf(h.ToLower()) >= 0)
                    {
                        headerlist.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
            return headerlist;
        }
        private void MoveDataHeaderResponeToRequest(List<string> HeaderOldRequest)
        {
            AddHeader(HeaderOldRequest.ToArray());
            foreach (string h in GetHeaderResponse())
            {
                var rg = Regex.Split(h, ": ");
                foreach (ParseHeader c in ListHeaderSetWhenRedirect)
                {
                    if (rg[0].ToLower().IndexOf(c.ReceiveName) >= 0)
                    {
                        AddHeader(c.SendName, rg[1]);
                    }
                }
            }
        }
        private MethodTransfer CheckMethodTransfer()
        {
            long ContentLength_ = GetContentLengthResponse();
            if (ContentLength_ == -1)
            {
                List<string> Transfer_Encoding = GetHeaderDataResponse("Transfer-Encoding".ToLower());
                if (Transfer_Encoding.Count == 0) return MethodTransfer.None;
                return MethodTransfer.Chunk;
            }
            else return MethodTransfer.ContentLength;
        }

        public List<ParseHeader> ListHeaderSetWhenRedirect = new List<ParseHeader>() {
            new ParseHeader() { ReceiveName = "set-cookie", SendName = "Cookie" }
        };

        public List<string> ListHeaderRemove = new List<string>() { "host" };
        #endregion
        
        private void debugwrite(string title,string data)
        {
            if(debug)
            {
                Console.WriteLine(">> " + title + "\r\n" + data);
            }
        }
    }
}
