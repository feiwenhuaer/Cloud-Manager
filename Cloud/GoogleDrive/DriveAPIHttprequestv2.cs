﻿using Cloud.GoogleDrive.Oauth;
using CustomHttpRequest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Cloud.GoogleDrive
{
    public delegate void TokenRenewCallback(TokenGoogleDrive token, string Email);
    public delegate void GD_LimitExceededDelegate();
    public class DriveAPIHttprequestv2
    {
        const string Host = "HOST: www.googleapis.com";
        const string uriAbout = "https://www.googleapis.com/drive/v2/about";
        const string uriFileList = "https://www.googleapis.com/drive/v2/files?orderBy={0}&corpus={1}&projection={2}&maxResults={3}&spaces={4}";
        const string uriFileGet = "https://www.googleapis.com/drive/v2/files/{0}?alt=media";
        const string uriFiles_insert_resumable_getUploadID = "https://www.googleapis.com/upload/drive/v2/files?uploadType={0}";
        const string uriDriveFile = "https://www.googleapis.com/drive/v2/files/";

        public int ReceiveTimeout { set { ReceiveTimeout_ = value; } get { return ReceiveTimeout; } }
        int ReceiveTimeout_ = 20000;
        
        public string Email { private get; set; }

        public bool acknowledgeAbuse { get { return acknowledgeAbuse_; } set { acknowledgeAbuse_ = value; } }
        bool acknowledgeAbuse_ = true;
        
        public TokenGoogleDrive GetToken { get { return token; } set { token = value; } }
        TokenGoogleDrive token;
        
        GD_LimitExceededDelegate limit;
        GoogleAPIOauth2 oauth;
        static object SyncRefreshToken = new object();
        static object SyncLimitExceeded = new object();
        
        public HttpRequest_ request { get; set; }
        public event TokenRenewCallback TokenRenewEvent;
        public bool Debug = false;

        #region Constructors
        public DriveAPIHttprequestv2(TokenGoogleDrive token, GD_LimitExceededDelegate LimitExceeded = null)
        {
            this.token = token;
            oauth = new GoogleAPIOauth2(token.refresh_token);
            this.limit = LimitExceeded;
        }
        #endregion

        private object Request(string url, TypeRequest typerequest, TypeReturn typereturn = TypeReturn.string_, byte[] bytedata = null, string[] moreheader = null)
        {
            request = new HttpRequest_(url, typerequest.ToString());
#if DEBUG
            request.debug = Debug;
#endif
            if (typereturn == TypeReturn.streamresponse_) request.ReceiveTimeout = this.ReceiveTimeout_;
            request.AddHeader(Host);
            request.AddHeader("Authorization", "Bearer " + token.access_token);
            if (moreheader != null) foreach (string h in moreheader) request.AddHeader(h);
            if ((typerequest == TypeRequest.POST || typerequest == TypeRequest.DELETE) && bytedata == null) request.AddHeader("Content-Length: 0");
            if (bytedata != null && (typerequest == TypeRequest.POST || typerequest == TypeRequest.PATCH))
            {
                request.AddHeader("Content-Length: " + bytedata.Length.ToString());
                Stream stream = request.SendHeader_And_GetStream();
                stream.Write(bytedata, 0, bytedata.Length);
                stream.Flush();
#if DEBUG
                Console.WriteLine("DriveAPIHttprequestv2: >>send data: " + Encoding.UTF8.GetString(bytedata));
#endif
            }
            try //get response
            {
                switch(typereturn)
                {
                    case TypeReturn.header_response: string temp = request.GetTextDataResponse(false, true); return request.HeaderReceive;
                    case TypeReturn.streamresponse_: return request.ReadHeaderResponse_and_GetStreamResponse(true, true);//get stream response
                    case TypeReturn.streamupload_: return request.SendHeader_And_GetStream();//get stream upload
                    case TypeReturn.string_:request.SendHeader_And_GetStream(); return request.GetTextDataResponse(true, true);//text data response
                    default:throw new Exception("Error typereturn.");
                }
            }
            catch (HttpException ex)
            {
                string textdata = request.TextDataResponse;
                GoogleDriveErrorMessage message;
                try { message = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleDriveErrorMessage>(ex.Message); }
                catch { throw ex; }// other message;
                switch(message.error.code)
                {
                    case 204: if (typerequest == TypeRequest.DELETE) return textdata; break;// delete result
                    case 401:
                        if (Monitor.TryEnter(SyncRefreshToken))
                        {
                            try
                            {
                                Monitor.Enter(SyncRefreshToken);
                                token = oauth.RefreshToken();
                                TokenRenewEvent.Invoke(token, this.Email);
                                return Request(url, typerequest, typereturn, bytedata, moreheader);
                            }
                            finally { Monitor.Exit(SyncRefreshToken); }
                        }
                        else
                        {
                            try { Monitor.Enter(SyncRefreshToken); return Request(url, typerequest, typereturn, bytedata, moreheader); }
                            finally { Monitor.Exit(SyncRefreshToken); }
                        }
                    case 403:
                        Error403 err = (Error403)Enum.Parse(typeof(Error403), message.error.errors[0].reason);
                        switch(err)
                        {
                            case Error403.forbidden:
                            case Error403.appNotAuthorizedToFile:
                            case Error403.domainPolicy:
                            case Error403.insufficientFilePermissions:
#if DEBUG
                                Console.WriteLine("DriveAPIHttprequestv2 Error403: " + textdata);
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
                                return Request(url, typerequest, typereturn, bytedata, moreheader);

                            case Error403.abuse://file malware or virut
                                if (acknowledgeAbuse_) return Request(url + "&acknowledgeAbuse=true", typerequest, typereturn, bytedata, moreheader); else break;
                            default: break;
                        }
                        break;
                    case 308: if(typerequest == TypeRequest.PUT && typereturn == TypeReturn.streamupload_) return request.GetStream(); else break;
                    default: break;
                }
                throw ex;
            }
        }

        public string About(bool includeSubscribed = true, long maxChangeIdCount = -1, long startChangeId = -1)
        {
            string parameters = "";
            if (!includeSubscribed) parameters += "includeSubscribed=false";
            if (!(maxChangeIdCount == -1)) if (parameters.Length > 0) parameters += "&maxChangeIdCount=" + maxChangeIdCount;
                else parameters = "maxChangeIdCount=" + maxChangeIdCount;
            if (!(startChangeId == -1)) if (parameters.Length > 0) parameters += "&startChangeId=" + startChangeId;
                else parameters = "startChangeId=" + startChangeId;
            return (string)Request(uriAbout, TypeRequest.GET, TypeReturn.string_,string.IsNullOrEmpty(parameters)? null:Encoding.UTF8.GetBytes(parameters));
        }
        
#region Files
        public string Files_list(OrderByEnum[] order, string q = null, CorpusEnum corpus = CorpusEnum.DEFAULT,
                                ProjectionEnum projection = ProjectionEnum.BASIC, string pageToken = null,
                                int maxResults = 1000, SpacesEnum spaces = SpacesEnum.drive)
        {
            string url = string.Format(
                uriFileList,
                HttpUtility.UrlEncode(OrderBy.Get(order), Encoding.UTF8),
                corpus.ToString(),
                projection.ToString(),
                maxResults.ToString(),
                spaces.ToString()
                );
            if (pageToken != null) url += "&pageToken=" + pageToken;
            if (q != null) url += "&q=" + HttpUtility.UrlEncode(q, Encoding.UTF8);
            return (string)Request(url, TypeRequest.GET);
        }

        public Stream Files_get(string fileId, long PosStart = 0, long endpos = 0)
        {
            string url = string.Format(uriFileGet, fileId);
            string[] moreheader = { "Range: bytes=" + PosStart.ToString() + "-" + endpos.ToString() };
            return (Stream)Request(url, TypeRequest.GET, TypeReturn.streamresponse_, null, (endpos != 0) ? moreheader : null);
        }

        public string Files_insert_resumable_getUploadID(string jsondata, string typefileupload, long filesize)
        {
            string url = string.Format(uriFiles_insert_resumable_getUploadID, uploadType.resumable.ToString());
            string[] moreheader = {
                                "Content-Type: application/json; charset=UTF-8",
                                "X-Upload-Content-Type: " + typefileupload,
                                "X-Upload-Content-Length: " + filesize.ToString() };
            string data = (string)Request(url, TypeRequest.POST, TypeReturn.header_response, Encoding.UTF8.GetBytes(jsondata), moreheader);
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

        public Stream Files_insert_resumable(string url_uploadid, long posstart, long posend, long filesize)
        {
            List<string> moreheader = new List<string>(){ "Content-Length: " + (posend - posstart + 1).ToString(),
                "Content-Type: image/jpeg",
                "Connection: keep-alive",
                "Content-Range: bytes " + posstart.ToString() + "-" + posend.ToString()+"/" + filesize.ToString()
            };
            return (Stream)Request(url_uploadid, TypeRequest.PUT, TypeReturn.streamupload_, null, moreheader.ToArray());
        }

        public string GetResponse_Files_insert_resumable()
        {
            string data = request.GetTextDataResponse(false, false);
#if DEBUG
            Console.WriteLine("DriveAPIHttprequestv2: " + data);
#endif
            return request.HeaderReceive;
        }

        public string Files_delete(string fileId)
        {
            return (string)Request(uriDriveFile + fileId + "?key=" + GoogleDriveAppKey.ApiKey, TypeRequest.DELETE, TypeReturn.string_, null, null);
        }

        public string CreateFolder(string name, string parent_id)
        {
            string data = "{\"mimeType\": \"application/vnd.google-apps.folder\", \"title\": \"" + name + "\", \"parents\": [{\"id\": \"" + parent_id + "\"}]}";
            return (string)Request(uriDriveFile, TypeRequest.POST, TypeReturn.string_, Encoding.UTF8.GetBytes(data),
                new string[] { "Content-Type: application/json" });
        }

        public string ItemTrash(string id)
        {
            return (string)Request(uriDriveFile + id + "/trash?fields=labels%2Ftrashed&key=" + GoogleDriveAppKey.ApiKey, TypeRequest.POST, TypeReturn.string_, null, null);
        }

        public string EditMetaData(string iditem, string json_data = null) //json_data = null is get metadata
        {
            byte[] buffer = null;
            if (!string.IsNullOrEmpty(json_data)) buffer = Encoding.UTF8.GetBytes(json_data);
            return (string)Request(uriDriveFile + iditem + "?key=" + GoogleDriveAppKey.ApiKey + "&alt=json",
                    TypeRequest.PATCH, TypeReturn.string_, buffer,
                    new string[] { "Content-Type: application/json" });
        }

#endregion
        
    }

}
