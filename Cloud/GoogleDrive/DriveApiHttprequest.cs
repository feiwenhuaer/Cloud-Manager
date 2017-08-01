﻿using Cloud.GoogleDrive.Oauth;
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
  public delegate void TokenRenewCallback(TokenGoogleDrive token);
  public delegate void GD_LimitExceededDelegate();
  public class DriveApiHttprequest
  {
    #region const/readonly
    const string Host = "HOST: www.googleapis.com";
    const string Header_ContentTypeApplicationJson = "Content-Type: application/json";
    internal protected readonly string ApiUri = "";
    #endregion

    #region sync
    static object SyncRefreshToken = new object();
    static object SyncLimitExceeded = new object();
    #endregion

    #region public event & properties
    /// <summary>
    /// Timeout for socket (default 2000ms)
    /// </summary>
    public int ReceiveTimeout { get; set; } = 2000;

    /// <summary>
    /// Still download if file have virus/malware (default true)
    /// </summary>
    public bool acknowledgeAbuse { get; set; } = true;

    /// <summary>
    /// Token
    /// </summary>
    public TokenGoogleDrive Token { get { return token; } set { if (!value.CheckToken()) throw new Exception("Token error"); else token = value; } }

    public event TokenRenewCallback TokenRenewEvent;

    public event GD_LimitExceededDelegate LimitExceeded;
    #endregion

    #region private & internal field
    GoogleAPIOauth2 oauth;
    TokenGoogleDrive token;
    internal protected HttpRequest_ http_request;
    #endregion

#if DEBUG
    public bool Debug { get; set; } = false;
#endif

    #region Constructors
    internal DriveApiHttprequest(TokenGoogleDrive token, DriveApiVersion version)
    {
      GoogleDriveAppKey.Check();//check key api
      this.Token = token;
      this.ApiUri = "https://www.googleapis.com/drive/" + version + "/";
      oauth = new GoogleAPIOauth2(token);
    }
    #endregion

    #region Request
    internal RequestReturn Request<T>(string url, TypeRequest typerequest, object post_data = null, string[] moreheader = null)
    {
      RequestReturn result = new RequestReturn();
      Uri uri;
      if (!Uri.TryCreate(ApiUri + url, UriKind.RelativeOrAbsolute, out uri)) Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);
      http_request = new HttpRequest_(uri, typerequest.ToString());
#if DEBUG
      http_request.debug = Debug;
#endif
      byte[] buffer_json_post_data = null;
      if (post_data != null) buffer_json_post_data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(post_data, JsonSetting._settings_serialize));
      if (typeof(T) == typeof(Stream) && post_data == null && typerequest != TypeRequest.PUT) http_request.ReceiveTimeout = this.ReceiveTimeout;
      http_request.AddHeader(Host);
      http_request.AddHeader("Authorization", "Bearer " + Token.access_token);
      if (moreheader != null) foreach (string h in moreheader) http_request.AddHeader(h);
      else if (post_data != null && (typerequest == TypeRequest.POST || typerequest == TypeRequest.PATCH)) http_request.AddHeader(Header_ContentTypeApplicationJson);
      if ((typerequest == TypeRequest.POST || typerequest == TypeRequest.DELETE) && post_data == null) http_request.AddHeader("Content-Length: 0");
      if (post_data != null && (typerequest == TypeRequest.POST || typerequest == TypeRequest.PATCH))
      {
        http_request.AddHeader("Content-Length: " + buffer_json_post_data.Length.ToString());
        Stream stream = http_request.SendHeader_And_GetStream();
        stream.Write(buffer_json_post_data, 0, buffer_json_post_data.Length);
        stream.Flush();
#if DEBUG
        Console.WriteLine("DriveAPIHttprequestv2: >>send data: " + Encoding.UTF8.GetString(buffer_json_post_data));
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
          if (post_data != null || typerequest == TypeRequest.PUT) result.stream = http_request.SendHeader_And_GetStream();//get stream upload
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
                return Request<T>(url, typerequest, post_data, moreheader);
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
                return Request<T>(url, typerequest, post_data, moreheader);
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
                if (LimitExceeded != null) LimitExceeded.Invoke();
#if DEBUG
                Console.WriteLine("DriveAPIHttprequestv2 LimitExceeded: " + err.ToString());
#endif
                try { Monitor.Enter(SyncLimitExceeded); Thread.Sleep(5000); } finally { Monitor.Exit(SyncLimitExceeded); }
                return Request<T>(url, typerequest, post_data, moreheader);

              case Error403.abuse://file malware or virut
                if (acknowledgeAbuse) return Request<T>(url + "&acknowledgeAbuse=true", typerequest, post_data, moreheader); else break;
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

  internal enum DriveApiVersion
  {
    v2,v3
  }
}
