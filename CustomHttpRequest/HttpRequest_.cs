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
    /// <summary>
    /// Get/Set url
    /// </summary>
    public Uri Uri { get; private set; }
    /// <summary>
    /// Get/Set RequestMethod
    /// </summary>
    public string RequestMethod { get; private set; }
    /// <summary>
    /// Get/Set ReceiveTimeout of tcpclient (default 30000ms)
    /// </summary>
    public int ReceiveTimeout { get; set; } = 30000;
    /// <summary>
    /// Get/Set SendTimeout of tcpclient (default 30000ms)
    /// </summary>
    public int SendTimeout { get; set; } = 30000;
    public List<HeaderField> HeadersSend { get; set; } = new List<HeaderField>();
    public List<HeaderField> HeadersReceived { get; set; } = new List<HeaderField>();
    public int ErrorCodeResponse { get; private set; } = -1;

    TcpClient tcp;
    SslStream sslStream;
    bool ssl = false;
    bool WasSendHeader = false;
    bool WasReceiveHeader = false;
    bool WasReceiveData = false;
    byte[] buffer_1 = new byte[1];
    byte[] buffer_header = new byte[8 * 1024];
    string base_field_headersend = "";// GET \abc.php  HTTP/1.1\r\n
    string data_text_response = "";

    public HttpRequest_(Uri uri, string RequestMethod)
    {
      if (uri == null) throw new ArgumentNullException("uri");
      if (string.IsNullOrEmpty(RequestMethod)) throw new ArgumentNullException("RequestMethod");
      MakeNewRequestHeader(uri, RequestMethod);
    }

    #region Connection & SSL
    void ConnectToHost()
    {
      if (tcp != null && tcp.Connected) return;
      try
      {
        tcp = new TcpClient();
        tcp.ReceiveTimeout = this.ReceiveTimeout;
        tcp.SendTimeout = this.SendTimeout;
        tcp.Client.NoDelay = true;
        this.ssl = this.Uri.ToString().ToLower().IndexOf("https://") == 0;
        tcp.Connect(this.Uri.Host, ssl ? 443 : 80);
        if (ssl)
        {
          sslStream = new SslStream(tcp.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
          try
          {
            sslStream.AuthenticateAsClient(this.Uri.Host);
          }
          catch (AuthenticationException)
          {
            tcp.Close();
            throw;
          }
        }
      }
      catch (Exception) { throw; }
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
    Stream GetStream()
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
    
    /// <summary>
    /// Make new request
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="RequestMethod"></param>
    public void MakeNewRequestHeader(Uri uri, string RequestMethod)
    {
      if (tcp != null && tcp.Connected) tcp.Close();
      this.RequestMethod = RequestMethod;
      this.Uri = uri;
      base_field_headersend = RequestMethod.ToUpper() + " " + this.Uri.PathAndQuery + " HTTP/1.1\r\n";
    }


    //send header
    public void SendHeader()
    {
      if (WasSendHeader) return;
      byte[] byte_send = Encoding.ASCII.GetBytes(base_field_headersend + HeadersSend.GetTextDataHeader() + "\r\n");
      GetStream().Write(byte_send,0,byte_send.Length);
      WasSendHeader = true;
    }

    public Stream UploadData()
    {
      if (!WasSendHeader) SendHeader();
      HeaderField content_length = HeadersSend.FindHeaderName("Content-Length");
      if (content_length != null)
      {
        long length = -1;
        if (long.TryParse(content_length.FieldData, out length)) return new UploadStream(GetStream(), length);
      }
      throw new Exception("Can't find Content-Length in RequestHeader.");
    }

    public void GetHeaderResponse()
    {
      if (!WasSendHeader) SendHeader();
      if (WasReceiveHeader) return;

      Stream st = GetStream();
      long byte_receive = 0;
      long byte_header_receive = 0;
      int byteread = 0;
      try
      {
        do //Receive header
        {
          byteread = st.Read(buffer_1, 0, buffer_1.Length);
          if (byteread <= 0) throw new EndOfStreamException("Error when receive header.");
          Array.Copy(buffer_1, 0, buffer_header, byte_receive, byteread);
          byte_receive += byteread;
          byte_header_receive += byteread;
        }
        while (!Find_CRLFCRLF(buffer_header, (int)byte_receive - 4));
      }
      catch (Exception) { throw; }
      string[] headers_response = Regex.Split(Encoding.UTF8.GetString(buffer_header, 0, (int)byte_header_receive).TrimStart(' '), "\r\n");
      ErrorCodeResponse = int.Parse(Regex.Split(headers_response[0], " ")[1]);//     HTTP/1.1 200 OK
      for (int i = 1; i < headers_response.Length - 2; i++) HeadersReceived.Add(HeaderField.Parse(headers_response[i]));
      WasReceiveHeader = true;

    }

    public Stream GetDataResponse()
    {
      if (!WasReceiveHeader) GetHeaderResponse();
      HeaderField content_length = HeadersReceived.FindHeaderName("Content-Length");
      if (content_length != null)
      {
        long length = -1;
        if (long.TryParse(content_length.FieldData, out length)) return new DownloadStream(GetStream(), length);
      }
      throw new Exception("Can't find Content-Length in ResponseHeader.");
    }

    public string GetTextResponse()
    {
      if (WasReceiveData) return data_text_response;
      if (!WasReceiveHeader) GetHeaderResponse();

      MethodTextTransfer mtt = MethodTextTransfer.None;
      HeaderField get_type = HeadersReceived.FindHeaderName("Content-Length");
      if (get_type != null) mtt = MethodTextTransfer.ContentLength;
      else mtt = MethodTextTransfer.Chunk;
      return ReadDataResponseText(mtt);
    }

    private string ReadDataResponseText(MethodTextTransfer mtt)
    {
      try
      {
        Stream stream = GetStream();
        int receive = 0;
        int byteread = 0;
        byte[] buffer;
        byte[] temp_buff = new byte[32];
        if (mtt == MethodTextTransfer.Chunk)
        {
          byte[] chunk_buffer = new byte[64];
          byte[] trash_buffer = new byte[2];
          buffer = new byte[5 * 1024 * 1024];//5Mb
          back:
          byteread = stream.Read(chunk_buffer, 0, 1);
          if (byteread <= 0) throw new EndOfStreamException("Can't get head.");
          if (chunk_buffer[0] == 48)//head chunk = char '0' then end
          {
            //stream.Read(chunk_buffer, 1, 4);
            data_text_response = Encoding.UTF8.GetString(buffer, 0, receive);
            tcp.Close();
            WasReceiveData = true;
            return data_text_response;
          }
          int offset = 1;
          while (offset < chunk_buffer.Length)//find head hex (ex: A2B5\r\n)
          {
            byteread = stream.Read(chunk_buffer, offset, 1);
            if (byteread <= 0) throw new EndOfStreamException("Find Head Error:" + Encoding.UTF8.GetString(chunk_buffer, 0, offset - 1));
            if (chunk_buffer[offset - 1] == 13 && chunk_buffer[offset] == 10) break;//if crlf
            offset += 1;
          }
          //parse hex to int
          int chunk_length = int.Parse(Encoding.ASCII.GetString(chunk_buffer, 0, offset - 1), System.Globalization.NumberStyles.HexNumber);
          int sub_receive = 0;
          do
          {
            byteread = stream.Read(temp_buff, 0, chunk_length - sub_receive < 32 ? chunk_length - sub_receive : 32);
            if (byteread <= 0) throw new EndOfStreamException("Reiceived chunk error, " + sub_receive + "bytes/" + chunk_length);
            Array.Copy(temp_buff, 0, buffer, receive, byteread);
            receive += byteread;
            sub_receive += byteread;
          } while (sub_receive != chunk_length);
          stream.Read(trash_buffer, 0, 2);
          goto back;
        }
        else if (mtt == MethodTextTransfer.ContentLength)
        {
          int ContentLengthResponse = int.Parse(HeadersReceived.FindHeaderName("Content-Length").FieldData);
          buffer = new byte[ContentLengthResponse];
          if (ContentLengthResponse != 0)
          {
            do
            {
              byteread = stream.Read(temp_buff, 0, temp_buff.Length);
              if (byteread <= 0) throw new EndOfStreamException("Reader " + receive + "bytes/" + buffer.Length);
              Array.Copy(temp_buff, 0, buffer, receive, byteread);
              receive += byteread;
            } while (receive != ContentLengthResponse);
          }
          data_text_response = Encoding.UTF8.GetString(buffer);
          tcp.Close();
          WasReceiveData = true;
          return data_text_response;
        }
        else return "";
      }
      catch (Exception) { throw; }
    }//read data text receive
    bool Find_CRLFCRLF(byte[] array, int start)
    {
      return (start < 0 || array.Length < 4) ? false : (array[start] == 13 && array[start + 1] == 10 && array[start + 2] == 13 && array[start + 3] == 10);
    }


    public void AddHeader(string HeaderLine)
    {
      HeadersSend.Add(HeaderField.Parse(HeaderLine));
    }
    public void AddHeader(string HeaderName,string HeaderData)
    {
      HeadersSend.Add(new HeaderField() { FieldName = HeaderName, FieldData = HeaderData });
    }
  }
}