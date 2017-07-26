using Cloud.GoogleDrive.Oauth;
using CustomHttpRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace Cloud.GoogleDrive
{
  public class DriveApiHttprequest
  {
    const string Host = "HOST: www.googleapis.com";
    const string Header_ContentTypeApplicationJson = "Content-Type: application/json";
    protected const string ApiUri = "https://www.googleapis.com/drive/";

    static object SyncRefreshToken = new object();
    static object SyncLimitExceeded = new object();

    GD_LimitExceededDelegate limit;
    GoogleAPIOauth2 oauth;
    int ReceiveTimeout_ = 20000;
    bool acknowledgeAbuse_ = true;

    public int ReceiveTimeout { set { ReceiveTimeout_ = value; } get { return ReceiveTimeout; } }
    public bool acknowledgeAbuse { get { return acknowledgeAbuse_; } set { acknowledgeAbuse_ = value; } }
    public TokenGoogleDrive Token { get; set; }
    public HttpRequest_ http_request { get; set; }
    public event TokenRenewCallback TokenRenewEvent;

    internal protected string version = "";

#if DEBUG
    public bool Debug { get; set; } = false;
#endif

    public DriveApiHttprequest(TokenGoogleDrive token, GD_LimitExceededDelegate LimitExceeded = null)
    {
      this.Token = token;
      oauth = new GoogleAPIOauth2(token);
      this.limit = LimitExceeded;
      GoogleDriveAppKey.Check();
    }


    #region Request
    internal RequestReturn Request<T>(string url, TypeRequest typerequest, byte[] bytedata = null, string[] moreheader = null)
    {
      RequestReturn result = new RequestReturn();
      http_request = new HttpRequest_(new Uri(ApiUri + version + url), typerequest.ToString());
#if DEBUG
      http_request.debug = Debug;
#endif
      if (typeof(T) == typeof(Stream) && bytedata == null && typerequest != TypeRequest.PUT) http_request.ReceiveTimeout = this.ReceiveTimeout_;
      http_request.AddHeader(Host);
      http_request.AddHeader("Authorization", "Bearer " + Token.access_token);
      if (moreheader != null) foreach (string h in moreheader) http_request.AddHeader(h);
      else if (bytedata != null && (typerequest == TypeRequest.POST || typerequest == TypeRequest.PATCH)) http_request.AddHeader(Header_ContentTypeApplicationJson);
      if ((typerequest == TypeRequest.POST || typerequest == TypeRequest.DELETE) && bytedata == null) http_request.AddHeader("Content-Length: 0");
      if (bytedata != null && (typerequest == TypeRequest.POST || typerequest == TypeRequest.PATCH))
      {
        http_request.AddHeader("Content-Length: " + bytedata.Length.ToString());
        Stream stream = http_request.SendHeader_And_GetStream();
        stream.Write(bytedata, 0, bytedata.Length);
        stream.Flush();
#if DEBUG
        Console.WriteLine("DriveAPIHttprequestv2: >>send data: " + Encoding.UTF8.GetString(bytedata));
#endif
      }
      try //get response
      {
        if (typeof(T) == typeof(string))
        {
          result.DataTextResponse = http_request.GetTextDataResponse(false, true);
          result.HeaderResponse = http_request.HeaderReceive;
        }
        else if (typeof(T) == typeof(Stream))
        {
          if (bytedata != null || typerequest == TypeRequest.PUT) result.stream = http_request.SendHeader_And_GetStream();//get stream upload
          else result.stream = http_request.ReadHeaderResponse_and_GetStreamResponse(true);//get stream response
        }
        else throw new Exception("Error typereturn.");
        return result;
      }
      catch (HttpException ex)
      {
        result.DataTextResponse = http_request.TextDataResponse;
        GoogleDriveErrorMessage message;
        try { message = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleDriveErrorMessage>(ex.Message); }
        catch { throw; }// other message;
        switch (message.error.code)
        {
          case 204: if (typerequest == TypeRequest.DELETE) return result; break;// delete result
          case 401:
            if (Monitor.TryEnter(SyncRefreshToken))
            {
#if DEBUG
              Console.WriteLine("DriveAPIHttprequestv2 Start Refresh Token {Email:" + Token.Email + ", Thread id:" + Thread.CurrentThread.ManagedThreadId + "}");
#endif
              try
              {
                Lock();
                Token = oauth.RefreshToken();
                TokenRenewEvent.Invoke(Token);
                return Request<T>(url, typerequest, bytedata, moreheader);
              }
              finally { Unlock(); }
            }
            else
            {
#if DEBUG
              Console.WriteLine("DriveAPIHttprequestv2 Start Wait Refresh Token {Email:" + Token.Email + ", Thread id:" + Thread.CurrentThread.ManagedThreadId + "}");
#endif
              try
              {
                Lock();
                return Request<T>(url, typerequest, bytedata, moreheader);
              }
              finally { Unlock(); }
            }
          case 403:
            Error403 err = (Error403)Enum.Parse(typeof(Error403), message.error.errors[0].reason);
            switch (err)
            {
              case Error403.forbidden:
              case Error403.appNotAuthorizedToFile:
              case Error403.domainPolicy:
              case Error403.insufficientFilePermissions:
#if DEBUG
                Console.WriteLine("DriveAPIHttprequestv2 Error403: " + result.DataTextResponse);
#endif
                break;

              case Error403.dailyLimitExceeded:
              case Error403.rateLimitExceeded:
              case Error403.sharingRateLimitExceeded:
              case Error403.userRateLimitExceeded:
                if (limit != null) limit.Invoke();
#if DEBUG
                Console.WriteLine("DriveAPIHttprequestv2 LimitExceeded: " + err.ToString());
#endif
                try { Monitor.Enter(SyncLimitExceeded); Thread.Sleep(5000); } finally { Monitor.Exit(SyncLimitExceeded); }
                return Request<T>(url, typerequest, bytedata, moreheader);

              case Error403.abuse://file malware or virut
                if (acknowledgeAbuse_) return Request<T>(url + "&acknowledgeAbuse=true", typerequest, bytedata, moreheader); else break;
              default: break;
            }
            break;
          case 308:
            if (typerequest == TypeRequest.PUT && typeof(T) == typeof(Stream))
            {
              result.stream = http_request.GetStream();
              return result;
            }
            else break;
          default: break;
        }
        throw;
      }
      catch (ThreadAbortException)
      {
        Unlock();
        throw;
      }
    }
    void Lock()
    {
#if DEBUG
      Console.WriteLine("DriveAPIHttprequestv2 Monitor Start Enter");
#endif
      Monitor.Enter(SyncRefreshToken);
#if DEBUG
      Console.WriteLine("DriveAPIHttprequestv2 Monitor Entered");
#endif
    }
    void Unlock()
    {
#if DEBUG
      Console.WriteLine("DriveAPIHttprequestv2 Monitor Start Exit");
#endif
      Monitor.Exit(SyncRefreshToken);
#if DEBUG
      Console.WriteLine("DriveAPIHttprequestv2 Monitor Exited");
#endif
    }
    #endregion

  }
}
