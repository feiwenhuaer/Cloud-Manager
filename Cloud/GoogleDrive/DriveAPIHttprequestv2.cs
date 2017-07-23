﻿using Cloud.GoogleDrive.Oauth;
using CustomHttpRequest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Newtonsoft.Json;

namespace Cloud.GoogleDrive
{
  public delegate void TokenRenewCallback(TokenGoogleDrive token);
  public delegate void GD_LimitExceededDelegate();
  public class DriveAPIHttprequestv2
  {
    const string Host = "HOST: www.googleapis.com";
    const string uriAbout = "https://www.googleapis.com/drive/v2/about";
    const string uriFileList = "https://www.googleapis.com/drive/v2/files?orderBy={0}&corpus={1}&projection={2}&maxResults={3}&spaces={4}";
    const string uriFileGet = "https://www.googleapis.com/drive/v2/files/{0}?alt=media";
    const string uriFiles_insert_resumable_getUploadID = "https://www.googleapis.com/upload/drive/v2/files?uploadType={0}";
    const string uriDriveFile = "https://www.googleapis.com/drive/v2/files/";
    const string Header_ContentTypeApplicationJson = "Content-Type: application/json";

    public int ReceiveTimeout { set { ReceiveTimeout_ = value; } get { return ReceiveTimeout; } }
    int ReceiveTimeout_ = 20000;

    public bool acknowledgeAbuse { get { return acknowledgeAbuse_; } set { acknowledgeAbuse_ = value; } }
    bool acknowledgeAbuse_ = true;

    public TokenGoogleDrive Token { get; set; }

    GD_LimitExceededDelegate limit;
    GoogleAPIOauth2 oauth;
    static object SyncRefreshToken = new object();
    static object SyncLimitExceeded = new object();

    public HttpRequest_ http_request { get; set; }
    public event TokenRenewCallback TokenRenewEvent;
#if DEBUG
    public bool Debug { get; set; } = false;
#endif

    #region Constructors
    public DriveAPIHttprequestv2(TokenGoogleDrive token, GD_LimitExceededDelegate LimitExceeded = null)
    {
      this.Token = token;
      oauth = new GoogleAPIOauth2(token);
      this.limit = LimitExceeded;
      GoogleDriveAppKey.Check();
      this.Files = new DriveFiles(this);
      this.About = new DriveAbout(this);
      this.Parent = new DriveParent(this);
      this.Extend = new DriveExtend(this);
    }
    #endregion

    #region Request
    private RequestReturn Request<T>(string url, TypeRequest typerequest, byte[] bytedata = null, string[] moreheader = null)
    {
      RequestReturn result = new RequestReturn();
      http_request = new HttpRequest_(new Uri(url), typerequest.ToString());
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
      catch (ThreadAbortException ex_thr_abort)
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

    #region DriveFiles
    public DriveFiles Files { get; private set; }
    public class DriveFiles
    {
      DriveAPIHttprequestv2 client;
      public DriveFiles(DriveAPIHttprequestv2 client)
      {
        this.client = client;
      }

      /// <summary>
      /// Download file
      /// </summary>
      /// <param name="fileId"></param>
      /// <param name="PosStart"></param>
      /// <param name="endpos"></param>
      /// <returns>Stream</returns>
      public Stream Get(string fileId, long PosStart = -1, long endpos = -1)
      {
        string url = string.Format(uriFileGet, fileId);
        string[] moreheader = { "Range: bytes=" + PosStart.ToString() + "-" + endpos.ToString() };
        return client.Request<Stream>(url, TypeRequest.GET, null, (endpos < 0) ? moreheader : null).stream;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="jsonMetaData"></param>
      /// <param name="typefileupload"></param>
      /// <param name="filesize"></param>
      /// <returns>Url upload</returns>
      public string Insert_Resumable_GetUploadID(string jsonMetaData, string typefileupload, long filesize)
      {
        string url = string.Format(uriFiles_insert_resumable_getUploadID, uploadType.resumable.ToString());
        string[] moreheader = {
                                "Content-Type: application/json; charset=UTF-8",
                                "X-Upload-Content-Type: " + typefileupload,
                                "X-Upload-Content-Length: " + filesize.ToString() };
        var reponse = client.Request<string>(url, TypeRequest.POST, Encoding.UTF8.GetBytes(jsonMetaData), moreheader);
        string data = reponse.HeaderResponse;
        string[] arrheader = Regex.Split(data, "\r\n");
        foreach (string h in arrheader)
        {
          if (h.ToLower().IndexOf("location: ") >= 0)
          {
            string location = Regex.Split(h, ": ")[1];
            if (string.IsNullOrEmpty(location)) throw new NullReferenceException(data);
            return location;
          }
        }
        throw new Exception("Can't get data: \r\n" + data);
      }
      /// <summary>
      /// Continue upload
      /// </summary>
      /// <param name="url_uploadid"></param>
      /// <param name="posstart"></param>
      /// <param name="posend"></param>
      /// <param name="filesize"></param>
      /// <returns></returns>
      public Stream Insert_Resumable(string url_uploadid, long posstart, long posend, long filesize)
      {
        if (posstart < 0 & posend < 0) throw new Exception("Pos can't be < 0.");
        List<string> moreheader = new List<string>(){ "Content-Length: " + (posend - posstart + 1).ToString(),
                "Content-Type: image/jpeg","Connection: keep-alive",
                "Content-Range: bytes " + posstart.ToString() + "-" + posend.ToString()+"/" + filesize.ToString()};
        return client.Request<Stream>(url_uploadid, TypeRequest.PUT, null, moreheader.ToArray()).stream;
      }

      /// <summary>
      /// Get response after Insert_Resumable
      /// </summary>
      /// <param name="GetMetaData">if false then return HeaderReceive</param>
      /// <returns>HeaderReceive or DataReceive</returns>
      public RequestReturn Insert_Resumable_Response(bool GetMetaData = false)
      {
        RequestReturn rr = new RequestReturn();
        rr.DataTextResponse = client.http_request.GetTextDataResponse(false, true);
#if DEBUG
        Console.WriteLine("DriveAPIHttprequestv2: " + rr.DataTextResponse);
#endif
        rr.HeaderResponse = client.http_request.HeaderReceive;
        return rr;;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="json_filemetadata"></param>
      /// <returns>ItemMetadata</returns>
      public Drive2_File Insert_MetadataRequest(string json_filemetadata)
      {
        return JsonConvert.DeserializeObject<Drive2_File>(client.Request<string>(uriDriveFile, TypeRequest.POST, Encoding.UTF8.GetBytes(json_filemetadata)).DataTextResponse);
      }
      
      public Drive2_File Patch(string id, string json_metadata = null)
      {
        byte[] buffer = null;
        if (!string.IsNullOrEmpty(json_metadata)) buffer = Encoding.UTF8.GetBytes(json_metadata);
        return client.Request<string>(uriDriveFile + id + "?key=" + GoogleDriveAppKey.ApiKey + "&alt=json",
                TypeRequest.PATCH, buffer).GetObjectResponse<Drive2_File>();
      }
      
      public Drive2_File Copy(string file_id, string parent_id)
      {
        string post_data = "{\"parents\": [{\"id\": \"" + parent_id + "\"}]}";
        return client.Request<string>("https://www.googleapis.com/drive/v2/files/" + file_id + "/copy", TypeRequest.POST, Encoding.UTF8.GetBytes(post_data)).GetObjectResponse<Drive2_File>();
      }

      public Drive2_File Delete(string fileId)
      {
        return client.Request<string>(uriDriveFile + fileId + "?key=" + GoogleDriveAppKey.ApiKey, TypeRequest.DELETE, null, null).GetObjectResponse<Drive2_File>();
      }

      public Drive2_Files_list List(OrderByEnum[] order, string query = null, string pageToken = null,
          CorpusEnum corpus = CorpusEnum.DEFAULT, ProjectionEnum projection = ProjectionEnum.BASIC,
              int maxResults = 1000, SpacesEnum spaces = SpacesEnum.drive)
      {
        string url = string.Format(uriFileList, HttpUtility.UrlEncode(order.Get(), Encoding.UTF8), corpus.ToString(),
        projection.ToString(), maxResults.ToString(), spaces.ToString());
        if (pageToken != null) url += "&pageToken=" + pageToken;
        if (query != null) url += "&q=" + HttpUtility.UrlEncode(query, Encoding.UTF8);
        return client.Request<string>(url, TypeRequest.GET).GetObjectResponse<Drive2_Files_list>();
      }

      //public string Touch()
      //{

      //}

      public Drive2_File Trash(string id)
      {
        return client.Request<string>(uriDriveFile + id + "/trash?fields=labels%2Ftrashed&key=" + GoogleDriveAppKey.ApiKey, TypeRequest.POST, null, null).GetObjectResponse<Drive2_File>();
      }

      public Drive2_File UnTrash(string id)
      {
        return client.Request<string>(uriDriveFile + id + "/untrash", TypeRequest.POST).GetObjectResponse<Drive2_File>();
      }

      //public string Watch()
      //{

      //}

      public void EmptyTrash()
      {
        client.Request<string>(uriDriveFile + "trash", TypeRequest.DELETE);
      }

      //public string GenerateIds()
      //{

      //}
    }
    
    #endregion

    #region DriveAbout
    public DriveAbout About { get; private set; }
    public class DriveAbout
    {
      DriveAPIHttprequestv2 client;
      public DriveAbout(DriveAPIHttprequestv2 client)
      {
        this.client = client;
      }

      public Drive2_About Get(bool includeSubscribed = true, long maxChangeIdCount = -1, long startChangeId = -1)
      {
        string parameters = "";
        if (!includeSubscribed) parameters += "includeSubscribed=false";
        if (!(maxChangeIdCount == -1)) if (parameters.Length > 0) parameters += "&maxChangeIdCount=" + maxChangeIdCount;
          else parameters = "maxChangeIdCount=" + maxChangeIdCount;
        if (!(startChangeId == -1)) if (parameters.Length > 0) parameters += "&startChangeId=" + startChangeId;
          else parameters = "startChangeId=" + startChangeId;
        return client.Request<string>(uriAbout, TypeRequest.GET, string.IsNullOrEmpty(parameters) ? null : Encoding.UTF8.GetBytes(parameters)).GetObjectResponse<Drive2_About>();
      }
    }
    #endregion

    #region DriveParent
    public DriveParent Parent { get; private set; }
    public class DriveParent
    {
      DriveAPIHttprequestv2 client;
      public DriveParent(DriveAPIHttprequestv2 client)
      {
        this.client = client;
      }

      public void Delete(string fileid, string parentid)
      {
        client.Request<string>(uriDriveFile + fileid + "/parents/" + parentid, TypeRequest.DELETE);
      }

      public Drive2_Parent Get(string fileid, string parentid)
      {
        return client.Request<string>(uriDriveFile + fileid + "/parents/" + parentid, TypeRequest.GET).GetObjectResponse<Drive2_Parent>();
      }

      /// <summary>
      /// Adds a parent folder for a file.
      /// </summary>
      /// <param name="fileid"></param>
      /// <param name="json_metadata">Json data parent</param>
      /// <returns></returns>
      public Drive2_Parent Insert(string fileid, string json_metadata)
      {
        return client.Request<string>(uriDriveFile + fileid + "/parents?alt=json&key=" + GoogleDriveAppKey.ApiKey,
            TypeRequest.POST, Encoding.UTF8.GetBytes(json_metadata)).GetObjectResponse<Drive2_Parent>();
      }

      public Drive2_Parents_list List(string fileid)
      {
        return client.Request<string>(uriDriveFile + fileid + "/parents", TypeRequest.GET).GetObjectResponse<Drive2_Parents_list>(); ;
      }
    }
    #endregion

    #region DriveExtend
    public DriveExtend Extend { get; private set; }
    public class DriveExtend
    {
      DriveAPIHttprequestv2 client;
      public DriveExtend(DriveAPIHttprequestv2 client)
      {
        this.client = client;
      }

      public Drive2_File CreateFolder(string name, List<string> parent_id)
      {

        string json_data = "{\"mimeType\": \"application/vnd.google-apps.folder\", \"title\": \"" + name + "\", \"parents\": [{\"id\": \"" + parent_id + "\"}]}";
        return CreateFolder(json_data);
      }

      public Drive2_File CreateFolder(string name, string parent_id)
      {
        string json_data = "{\"mimeType\": \"application/vnd.google-apps.folder\", \"title\": \"" + name + "\", \"parents\": [{\"id\": \"" + parent_id + "\"}]}";
        return CreateFolder(json_data);
      }
      public Drive2_File CreateFolder(string metadata)
      {
        return client.Files.Insert_MetadataRequest(metadata);
      }
    }
    #endregion
  }
  
}
