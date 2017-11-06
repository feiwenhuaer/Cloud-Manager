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
  public delegate void TokenRenewCallback(TokenGoogleDrive token);
  public delegate void GD_LimitExceededDelegate();
  public class DriveApiHttprequest
  {
    #region const/readonly
    const string Host = "HOST: www.googleapis.com";
    const string Header_ContentTypeApplicationJson = "Content-Type: application/json";
    internal protected readonly string ApiUri = "";
    static readonly Type typestream = typeof(Stream);
    static readonly Type typestring = typeof(string);
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
      Type typeT = typeof(T);
      Uri uri;
      int LimitExceededCount = 0;
      if (!Uri.TryCreate(ApiUri + url, UriKind.RelativeOrAbsolute, out uri)) Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);
      request:
      http_request = new HttpRequest_(uri, typerequest.ToString());
      byte[] buffer_json_post_data = null;
      if (post_data != null) buffer_json_post_data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(post_data, JsonSetting._settings_serialize));
      if (typeT == typestream && post_data == null && typerequest != TypeRequest.PUT) http_request.ReceiveTimeout = this.ReceiveTimeout;
      http_request.AddHeader(Host);
      http_request.AddHeader("Authorization", "Bearer " + Token.access_token);
      if (moreheader != null) foreach (string h in moreheader) http_request.AddHeader(h);
      else if (post_data != null && (typerequest == TypeRequest.POST || typerequest == TypeRequest.PATCH)) http_request.AddHeader(Header_ContentTypeApplicationJson);
      if ((typerequest == TypeRequest.POST || typerequest == TypeRequest.DELETE) && post_data == null) http_request.AddHeader("Content-Length", "0");
      if (post_data != null && (typerequest == TypeRequest.POST || typerequest == TypeRequest.PATCH))
      {
        http_request.AddHeader("Content-Length", buffer_json_post_data.Length.ToString());
        Stream stream = http_request.UploadData();
        stream.Write(buffer_json_post_data, 0, buffer_json_post_data.Length);
        stream.Flush();
      }
      try //get response
      {
        if (typeT == typestring)
        {
          result.DataTextResponse = http_request.GetTextResponse();
          result.HeaderResponse = http_request.HeadersReceived.GetTextDataHeader();
        }
        else if (typeT == typestream)
        {
          if (post_data != null || typerequest == TypeRequest.PUT) result.stream = http_request.UploadData();//get stream upload
          else result.stream = http_request.GetDataResponse();//get stream response
        }
        else throw new Exception("Error typereturn.");
        int error_code = http_request.ErrorCodeResponse;
        if (error_code == 200 | error_code == 206) return result;
        else throw new HttpException(error_code, http_request.GetTextResponse());
      }
      catch (HttpException ex)
      {
        result.DataTextResponse = http_request.GetTextResponse();
        GoogleDriveErrorMessage message;
        try { message = JsonConvert.DeserializeObject<GoogleDriveErrorMessage>(ex.Message); }
        catch { throw; }// other message;
        switch (message.error.code)
        {
          case 204: if (typerequest == TypeRequest.DELETE) return result; break;// delete result
          case 401: oauth.RefreshToken(); goto request;
          case 403:
            Error403 err = (Error403)Enum.Parse(typeof(Error403), message.error.errors[0].reason);
            switch (err)
            {
              case Error403.forbidden:
              case Error403.appNotAuthorizedToFile:
              case Error403.domainPolicy:
              case Error403.insufficientFilePermissions:
                break;

              case Error403.dailyLimitExceeded:
              case Error403.rateLimitExceeded:
              case Error403.sharingRateLimitExceeded:
              case Error403.userRateLimitExceeded:
                if (LimitExceeded != null) LimitExceeded.Invoke();
                Thread.Sleep(5000);
                LimitExceededCount++;
                if (LimitExceededCount > 10) throw;
                goto request;

              case Error403.abuse://file malware or virut
                if (acknowledgeAbuse)
                {
                  url += "&acknowledgeAbuse=true";
                  goto request;
                }
                else break;
              default: break;
            }
            break;
          case 308:
            if (typerequest == TypeRequest.PUT && typeT == typestream)
            {
              result.stream = http_request.UploadData();
              return result;
            }
            else break;
          default: break;
        }
        throw;
      }
    }
    #endregion
    internal enum DriveApiVersion
    {
      v2, v3
    }
  }
}
