using Cloud.GoogleDrive.Oauth;
using CustomHttpRequest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using System.Dynamic;

namespace Cloud.GoogleDrive
{
  
  public class DriveAPIHttprequestv2: DriveApiHttprequest
  {
    internal const string uriFileList = "files?orderBy={0}&corpus={1}&projection={2}&maxResults={3}&spaces={4}";
    internal const string uriFileGet = "files/{0}?alt=media";
    internal const string uriFiles_insert_resumable_getUploadID = "https://www.googleapis.com/upload/drive/v2/files?uploadType={0}";
    internal const string uriDriveFile = "files/";

    #region Constructors
    public DriveAPIHttprequestv2(TokenGoogleDrive token):base(token,DriveApiVersion.v2)
    {
      this.Files = new DriveFiles(this);
      this.About = new DriveAbout(this);
      this.Parent = new DriveParent(this);
      this.Extend = new DriveExtend(this);
    }
    #endregion

    #region DriveAbout
    public DriveAbout About { get; private set; }
    public class DriveAbout
    {
      const string uriAbout = "about";
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
      public IRequestReturn Insert_Resumable_Response(bool GetMetaData = false)
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
        return client.Request<string>(uriDriveFile + file_id + "/copy", TypeRequest.POST, Encoding.UTF8.GetBytes(post_data)).GetObjectResponse<Drive2_File>();
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
      public Drive2_Parent Insert(string fileid, List<Drive2_Parent> parents)
      {
        return client.Request<string>(uriDriveFile + fileid + "/parents?alt=json&key=" + GoogleDriveAppKey.ApiKey,
            TypeRequest.POST, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(parents,JsonSetting._settings_serialize))).GetObjectResponse<Drive2_Parent>();
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

      public Drive2_File CreateFolder(string name, List<Drive2_Parent> parents_id)
      {
        Drive2_File f = new Drive2_File();
        f.mimeType = "application/vnd.google-apps.folder";
        f.title = name;
        f.parents = parents_id;
        f.etag = "test";
        string json = JsonConvert.SerializeObject(f, JsonSetting._settings_serialize);
        return CreateFolder(json);
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
