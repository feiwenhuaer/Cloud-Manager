using CustomHttpRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Cloud;
using Cloud.Dropbox;

namespace Cloud.Dropbox
{
    public class DropboxRequestAPIv2
    {
        private string access_token;
        public string AccessToken { get { return access_token; } set { access_token = value; } }
        private string uid;
        public string Uid { get { return uid; } private set { uid = value; } }

        const int portListenMax = 22439;
        const int portListenMin = 22430;

        #region Constructors
        public DropboxRequestAPIv2() { }
        public DropboxRequestAPIv2(string access_token)
        {
            this.access_token = access_token;
        }
        #endregion
        
        #region authorsize
        public string GetAccessToken(string key_authorize, int port = -1)
        {
            dynamic json = JsonConvert.DeserializeObject(GetAccessToken_(key_authorize, port));
            this.access_token = json.access_token;
            return this.access_token;
        }

        private string GetAccessToken_(string key_authorize, int port = -1)
        {
            BuildURL build = new BuildURL("https://api.dropboxapi.com/1/oauth2/token");
            build.AddParameter("code", key_authorize);
            build.AddParameter("client_id", DropboxAppKey.ApiKey);
            build.AddParameter("client_secret", DropboxAppKey.ApiSecret);
            build.AddParameter("grant_type", "authorization_code");
            if (port != -1) build.AddParameter("redirect_uri", string.Format("http%3A%2F%2Flocalhost%3A{0}", port.ToString()));
            HttpRequest_ rq = new HttpRequest_(build.uri, TypeRequest.POST.ToString());
            rq.AddHeader("HOST: api.dropboxapi.com");
            rq.AddHeader("Content-Length: 0");
            return rq.GetTextDataResponse(true, true);
        }
        
        #endregion 
        HttpRequest_ rq;
        private T POST_Request<T>(string url,object obj_request = null,string[] headers = null,string ContentType = "application/json",bool upload = false)
        {
            Stream st = null;
            rq = new HttpRequest_(new Uri(url), "POST");
            rq.AddHeader("HOST", "api.dropboxapi.com");
            rq.AddHeader("Authorization", "Bearer " + access_token);

            if(headers != null) foreach (string header in headers) rq.AddHeader(header);
            if (obj_request != null && !string.IsNullOrEmpty(ContentType))
            {
                byte[] reqData;
                if (obj_request is byte[]) reqData = (byte[])obj_request;
                else reqData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj_request));

                rq.AddHeader("Content-Type", ContentType);
                rq.AddHeader("Content-Length", reqData.Length.ToString());
                st = rq.SendHeader_And_GetStream();
                st.Write(reqData, 0, reqData.Length);
                st.Flush();
            }
            else
            {
                rq.AddHeader("Content-Length", "0");
                st = rq.SendHeader_And_GetStream();
            }

           
            //receiver
            if (typeof(T) == typeof(Stream))
            {
                if (!upload) st = rq.ReadHeaderResponse_and_GetStreamResponse(true);//download
                return (T)(object)st;//upload
            }
            else
            {
                rq.ReadHeaderResponse_and_GetStreamResponse(true);
                string text_response = rq.ReadDataResponseText();//.Replace("\".tag\":", "\"tag\":");
                if (typeof(T) == typeof(string)) return (T)(object)text_response;
                else
                {
                    T data = JsonConvert.DeserializeObject<T>(text_response);
                    if (data != null) return data;
                    else
                    {
                        if (typeof(T) == typeof(Dropbox_Response_Error)) throw new DropboxJsonResponseException(data as IDropbox_Response_Error);
                        else throw new Exception(text_response);
                    }
                }
            }
        }
        
        public IDropbox_Response_GetCurrentAccount GetCurrentAccount()
        {
            return POST_Request<Dropbox_Response_GetCurrentAccount>("https://api.dropboxapi.com/2/users/get_current_account");
        }
        public IDropbox_Response_GetSpaceUsage GetSpaceUsage()
        {
            return POST_Request<Dropbox_Response_GetSpaceUsage>("https://api.dropboxapi.com/2/users/get_space_usage");
        }        
        public IDropbox_Response_MetaData GetMetadata(IDropbox_Request_Metadata data_request)
        {
            return POST_Request<Dropbox_Response_MetaData>("https://api.dropboxapi.com/2/files/get_metadata", data_request);
        }
        public IDropbox_Response_ListFolder ListFolder(IDropbox_Request_ListFolder data_request)
        {
            return POST_Request<Dropbox_Response_ListFolder>("https://api.dropboxapi.com/2/files/list_folder", data_request);
        }
        public IDropbox_Response_ListFolder list_folder_continue(IDropbox_Request_ListFolderContinue data_request)
        {
            return POST_Request<Dropbox_Response_ListFolder>("https://api.dropboxapi.com/2/files/list_folder/continue", data_request);
        }        
        public IDropbox_Response_MetaData create_folder(IDropbox_Path data_request)
        {
            return POST_Request<Dropbox_Response_MetaData>("https://api.dropboxapi.com/2/files/create_folder", data_request);
        }
        public IDropbox_Response_MetaData delete(IDropbox_Path data_request)
        {
            return POST_Request<Dropbox_Response_MetaData>("https://api.dropboxapi.com/2/files/delete", data_request);
        }
        public void permanently_delete(IDropbox_Path data_request)
        {
            POST_Request<IDropbox_Response_Error>("https://api.dropboxapi.com/2/files/permanently_delete", data_request);
        }
        public IDropbox_Response_MetaData copy(IDropbox_Request_MoveCopy data_request)
        {
            return POST_Request<Dropbox_Response_MetaData>("https://api.dropboxapi.com/2/files/copy", data_request);
        }
        public IDropbox_Response_MetaData move(IDropbox_Request_MoveCopy data_request)
        {
            return POST_Request<Dropbox_Response_MetaData>("https://api.dropboxapi.com/2/files/move", data_request);
        }
        public IDropbox_Response_MetaData get_thumbnail(IDropbox_Request_GetThumbnail data_request)
        {
            return POST_Request<Dropbox_Response_MetaData>("https://content.dropboxapi.com/2/files/get_thumbnail", data_request);
        }
       

        #region up/download
        public Stream Download(IDropbox_Path path, long startpos = -1, long endpos = -1, int timeout = 2147483647)// unsupport multi
        {
            List<string> headers = new List<string>() { "Dropbox-API-Arg: " + JsonConvert.SerializeObject(path) };
            if (endpos > 0) headers.Add("Range: bytes=" + startpos.ToString() + "-" + endpos.ToString());
            return POST_Request<Stream>("https://content.dropboxapi.com/2/files/download", null, headers.ToArray(), null);
        }

        public IDropbox_Request_UploadSessionAppend upload_session_start(byte[] buffer, int buffer_length = -1, bool close = false)
        {
            List<string> headers = new List<string>() { "Dropbox-API-Arg: {\"close\": " + close.ToString().ToLower() + "}" };
            byte[] buffer_ = null;
            if (buffer_length > 0)
            {
                buffer_ = new byte[buffer_length];
                Array.Copy(buffer, 0, buffer_, 0, buffer_length);
            }
            else buffer_ = buffer;
            IDropbox_Request_UploadSessionAppend session_append = POST_Request<Dropbox_Request_UploadSessionAppend>("https://content.dropboxapi.com/2/files/upload_session/start",
                buffer_, headers.ToArray(), "application/octet-stream", false);
            session_append.offset = buffer_.Length;
            return session_append;
        }
        public Stream upload_session_append(IDropbox_Request_UploadSessionAppend session, long length_chunk)
        {
            List<string> headers = new List<string>() {
                "Content-Length: " + length_chunk.ToString(),
                "Dropbox-API-Arg: " + JsonConvert.SerializeObject(session)
            };
            return POST_Request<Stream>("https://content.dropboxapi.com/2/files/upload_session/append", null, headers.ToArray(), "application/octet-stream", true);
        }
        public string GetResponse_upload_session_append()
        {
            return rq.GetTextDataResponse(false, true);
        }
        public IDropbox_Response_MetaData upload_session_finish(IDropbox_Request_UploadSessionFinish session)
        {
            List<string> headers = new List<string>()
            {
                "Dropbox-API-Arg: "+ JsonConvert.SerializeObject(session)
            };
            return POST_Request<Dropbox_Response_MetaData>("https://content.dropboxapi.com/2/files/upload_session/finish", null, headers.ToArray(),
                 "application/octet-stream", false);
        }
        #endregion
    }
}
