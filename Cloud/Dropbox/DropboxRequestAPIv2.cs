using CustomHttpRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

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
        public void GetAccessToken(string key_authorize, int port = -1)
        {
            dynamic json = JsonConvert.DeserializeObject(GetAccessToken_(key_authorize, port));
            this.access_token = json.access_token;
        }

        private string GetAccessToken_(string key_authorize, int port = -1)
        {
            BuildURL build = new BuildURL("https://api.dropboxapi.com/1/oauth2/token");
            build.AddParameter("code", key_authorize);
            build.AddParameter("client_id", DropboxAppKey.ApiKey);
            build.AddParameter("client_secret", DropboxAppKey.ApiSecret);
            build.AddParameter("grant_type", "authorization_code");
            if (port != -1) build.AddParameter("redirect_uri", string.Format("http%3A%2F%2Flocalhost%3A{0}", port.ToString()));
            HttpRequest_ rq = new HttpRequest_(build.Url, TypeRequest.POST.ToString());
            rq.AddHeader("HOST: api.dropboxapi.com");
            rq.AddHeader("Content-Length: 0");
            return rq.GetTextDataResponse(true, true);
        }

        public string DisableAccessToken()
        {
            if (string.IsNullOrEmpty(access_token)) { throw new System.ArgumentException("AccessToken can't be null"); }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.dropboxapi.com/1/disable_access_token?access_token=" + access_token);
            request.Method = "POST";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string result = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return result;
        }
        #endregion 

        private string RequestUrlv2(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            WebHeaderCollection myWebHeaderCollection = request.Headers;
            myWebHeaderCollection.Add("Authorization", "Bearer " + access_token);
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string result = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return result;
        }


        #region info_account // done
        public string GetCurrentAccount()
        {
            return RequestUrlv2("https://api.dropboxapi.com/2/users/get_current_account");
        }

        public string GetSpaceUsage()
        {
            return RequestUrlv2("https://api.dropboxapi.com/2/users/get_space_usage");
        }
        #endregion

        #region path 
        private string RequestUrl2(string url, byte[] reqData)
        {
            HttpRequest_ rq = new HttpRequest_(url, "POST");
            rq.AddHeader("HOST", "api.dropboxapi.com");
            rq.AddHeader("Authorization", "Bearer " + access_token);
            rq.AddHeader("Content-Type", "application/json");

            if (reqData != null)
            {
                rq.AddHeader("Content-Length", reqData.Length.ToString());
                Stream st = rq.SendHeader_And_GetStream();
                st.Write(reqData, 0, reqData.Length);
                st.Flush();
                rq.ReadHeaderResponse_and_GetStreamResponse(false, true);
                return rq.ReadDataResponseText();
            }
            else
            {
                rq.AddHeader("Content-Length", "0");
                rq.ReadHeaderResponse_and_GetStreamResponse(false, true);
                return rq.ReadDataResponseText();
            }
        }

        #region my account
        public string GetMetadata(string path, Boolean include_media_info = false)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);
            build.AddChildNodes("include_media_info", include_media_info.ToString().ToLower());
            Console.WriteLine(build.GetJson);
            return RequestUrl2("https://api.dropboxapi.com/2/files/get_metadata", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string ListFolder(string path, bool include_media_info = false, bool recursive = false, bool include_deleted = false, bool include_has_explicit_shared_members = false)
        {
            string data = string.Format("\"path\": \"{0}\",\"recursive\": {1},\"include_media_info\": {2},\"include_deleted\": {3},\"include_has_explicit_shared_members\": {4}",
                path, recursive.ToString().ToLower(), include_media_info.ToString().ToLower(), include_deleted.ToString().ToLower(), include_has_explicit_shared_members.ToString().ToLower());

            return RequestUrl2("https://api.dropboxapi.com/2/files/list_folder", Encoding.UTF8.GetBytes("{" + data + "}"));
        }

        public string list_folder_continue(string cursor)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("cursor", cursor);
            return RequestUrl2("https://api.dropboxapi.com/2/files/list_folder/continue", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string list_folder_get_latest_cursor(string path, Boolean include_media_info = false, Boolean recursive = false, Boolean include_deleted = false)
        {
            BuildJson build = new BuildJson();

            build.AddChildStringNodes("path", path);
            build.AddChildNodes("recursive", recursive.ToString().ToLower());
            build.AddChildNodes("include_media_info", include_media_info.ToString().ToLower());
            build.AddChildNodes("include_deleted", include_deleted.ToString().ToLower());
            return RequestUrl2("https://api.dropboxapi.com/2/files/list_folder/get_latest_cursor", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string search(string path, string search, long max_results, DropboxSearchMode searchmode, int start = 0)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);
            build.AddChildStringNodes("query", search);
            build.AddChildNodes("start", start.ToString());
            build.AddChildNodes("max_results", max_results.ToString());
            build.AddChildStringNodes("mode", searchmode.ToString());
            return RequestUrl2("https://api.dropboxapi.com/2/files/search", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string create_folder(string path)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);
            return RequestUrl2("https://api.dropboxapi.com/2/files/create_folder", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string delete(string path)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);
            return RequestUrl2("https://api.dropboxapi.com/2/files/delete", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string permanently_delete(string path)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);
            return RequestUrl2("https://api.dropboxapi.com/2/files/permanently_delete", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string copy(string from_path, string to_path)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("from_path", from_path);
            build.AddChildStringNodes("to_path", to_path);
            return RequestUrl2("https://api.dropboxapi.com/2/files/copy", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string move(string from_path, string to_path)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("from_path", from_path);
            build.AddChildStringNodes("to_path", to_path);
            return RequestUrl2("https://api.dropboxapi.com/2/files/move", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string get_thumbnail(string path, Dropboxthumbnail size, DropboxImageFormat format)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);
            build.AddChildStringNodes("format", format.ToString());
            build.AddChildStringNodes("size", size.ToString());
            return RequestUrl2("https://content.dropboxapi.com/2/files/get_thumbnail", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string get_preview(string path)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);
            return RequestUrl2("https://content.dropboxapi.com/2/files/get_preview", Encoding.UTF8.GetBytes(build.GetJson));
        }
        #endregion

        #region revisions
        public string list_revisions(string path, int limit)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);
            build.AddChildNodes("limit", limit.ToString());
            return RequestUrl2("https://api.dropboxapi.com/2/files/list_revisions", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string restore(string path, string rev)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);
            build.AddChildStringNodes("rev", rev);
            return RequestUrl2("https://api.dropboxapi.com/2/files/restore", Encoding.UTF8.GetBytes(build.GetJson));
        }
        #endregion

        #region sharing  //// not doneeeeeeeee
        public string get_shared_links(string path)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/get_shared_links", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string create_shared_link(string path, Boolean short_url)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);
            build.AddChildNodes("short_url", short_url.ToString().ToLower());
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/create_shared_link", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string revoke_shared_link(string url)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("url", url);
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/revoke_shared_link", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string sharing_get_folder_metadata(string shared_folder_id, Boolean include_membership = true)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("shared_folder_id", shared_folder_id);
            build.AddChildNodes("include_membership", include_membership.ToString().ToLower());
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/get_folder_metadata", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string sharing_list_folders(Boolean include_membership = false)
        {
            BuildJson build = new BuildJson();
            build.AddChildNodes("include_membership", include_membership.ToString().ToLower());
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/list_folders", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string sharing_update_folder_policy()
        {
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/update_folder_policy", null);
        }
        #endregion

        #region beta sharing ~50%
        public string sharing_check_job_status(string async_job_id)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("async_job_id", async_job_id);
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/check_job_status", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string sharing_check_share_job_status(string async_job_id)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("async_job_id", async_job_id);
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/check_share_job_status", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string sharing_share_folder(string path, DropboxMemberPolicy member_policy = DropboxMemberPolicy.team,
            DropboxAclUpdatePolicy acl_update_policy = DropboxAclUpdatePolicy.editors, DropboxSharedLinkPolicy shared_link_policy = DropboxSharedLinkPolicy.members
            , Boolean force_async = false)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);
            build.AddChildStringNodes("member_policy", member_policy.ToString());
            build.AddChildStringNodes("acl_update_policy", acl_update_policy.ToString());
            build.AddChildStringNodes("shared_link_policy", shared_link_policy.ToString());
            build.AddChildNodes("force_async", force_async.ToString().ToLower());
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/share_folder", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string sharing_unshare_folder(string shared_folder_id, Boolean leave_a_copy = true)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("shared_folder_id", shared_folder_id);
            build.AddChildNodes("leave_a_copy", leave_a_copy.ToString().ToLower());
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/unshare_folder", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string sharing_transfer_folder(string shared_folder_id, string to_dropbox_id)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("shared_folder_id", shared_folder_id);
            build.AddChildStringNodes("to_dropbox_id", to_dropbox_id);
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/transfer_folder", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string sharing_mount_folder(string shared_folder_id)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("shared_folder_id", shared_folder_id);
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/mount_folder", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string sharing_unmount_folder(string shared_folder_id)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("shared_folder_id", shared_folder_id);
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/unmount_folder", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string sharing_create_shared_link_with_settings(string path, DropboxRequestedVisibility requested_visibility, DateTime expires, string link_password = null)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);

            BuildJson settings = new BuildJson();
            settings.AddChildStringNodes("requested_visibility", requested_visibility.ToString().ToLower());
            if (requested_visibility == DropboxRequestedVisibility.password)
            {
                settings.AddChildStringNodes("link_password", link_password);
            }
            settings.AddChildStringNodes("expires", expires.ToLongTimeString());
            build.AddChildNodes("settings", settings.GetJson);

            return RequestUrl2("https://api.dropboxapi.com/2/sharing/create_shared_link_with_settings", Encoding.UTF8.GetBytes(build.GetJson));

        }
        public string sharing_create_shared_link_with_settings(string path, DropboxRequestedVisibility requested_visibility, string link_password = null)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("path", path);

            BuildJson settings = new BuildJson();
            settings.AddChildStringNodes("requested_visibility", requested_visibility.ToString().ToLower());
            if (requested_visibility == DropboxRequestedVisibility.password)
            {
                settings.AddChildStringNodes("link_password", link_password);
            }

            build.AddChildNodes("settings", settings.GetJson);

            return RequestUrl2("https://api.dropboxapi.com/2/sharing/create_shared_link_with_settings", Encoding.UTF8.GetBytes(build.GetJson));
        }


        #region share member 0%
        public string sharing_add_folder_member(string shared_folder_id, List<DropboxSharingMember> members, Boolean quiet = false, string custom_message = "Dropbox httprequest by tqk2811")
        {
            BuildJson build = new BuildJson();
            List<string> list_member = new List<string>();// {} , {}, {}

            foreach (DropboxSharingMember member in members)
            {
                BuildJson build_member_n_access_level = new BuildJson(); // {{},{}}

                BuildJson build_member = new BuildJson();//{}
                build_member.AddChildStringNodes(".tag", member.tag.ToString());
                if (member.tag == DropboxTag.email)
                {
                    build_member.AddChildStringNodes("email", member.email);
                }
                else
                {
                    build_member.AddChildStringNodes("dropbox_id", member.dropbox_id);
                }


                BuildJson build_access_level = new BuildJson();//{}
                build_access_level.AddChildStringNodes(".tag", member.access_level.ToString());

                build_member_n_access_level.AddChildNodes("member", build_member.GetJson);
                build_member_n_access_level.AddChildNodes("access_level", build_access_level.GetJson);

                list_member.Add(build_member_n_access_level.GetJson);
            }
            build.AddChildStringNodes("shared_folder_id", shared_folder_id);
            build.AddListChildNodes("members", list_member);
            build.AddChildNodes("quiet", quiet.ToString().ToLower());
            build.AddChildStringNodes("custom_message", custom_message);
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/add_folder_member", Encoding.UTF8.GetBytes(build.GetJson));
        }

        private string getjsonMember(DropboxSharingMember member)
        {
            BuildJson build_member = new BuildJson();
            build_member.AddChildStringNodes(".tag", member.tag.ToString());
            if (member.tag == DropboxTag.email)
            {
                build_member.AddChildStringNodes("email", member.email);
            }
            else
            {
                build_member.AddChildStringNodes("dropbox_id", member.dropbox_id);
            }
            return build_member.GetJson;
        }

        public string sharing_remove_folder_member(string shared_folder_id, DropboxSharingMember member, Boolean leave_a_copy = false)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("shared_folder_id", shared_folder_id);
            build.AddChildNodes("member", getjsonMember(member));
            build.AddChildNodes("leave_a_copy", leave_a_copy.ToString().ToLower());
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/remove_folder_member", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string sharing_update_folder_member(string shared_folder_id, DropboxSharingMember member, DropboxSharingAccess_level access_level)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("shared_folder_id", shared_folder_id);
            build.AddChildNodes("member", getjsonMember(member));
            build.AddChildStringNodes("access_level", access_level.ToString());
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/update_folder_member", Encoding.UTF8.GetBytes(build.GetJson));
        }

        public string sharing_relinquish_folder_membership(string shared_folder_id)
        {
            BuildJson build = new BuildJson();
            build.AddChildStringNodes("shared_folder_id", shared_folder_id);
            return RequestUrl2("https://api.dropboxapi.com/2/sharing/relinquish_folder_membership", Encoding.UTF8.GetBytes(build.GetJson));
        }



        #endregion
        #endregion
        #endregion

        #region up/download
        public Stream Download(string path, long startpos = 0, long endpos = 0, int timeout = 2147483647)// unsupport multi
        {
            custom_request = new HttpRequest_("https://content.dropboxapi.com/2/files/download", "POST");
            custom_request.AddHeader("HOST: content.dropboxapi.com");
            custom_request.AddHeader("Authorization", "Bearer " + access_token);
            custom_request.AddHeader("Dropbox-API-Arg", "{\"path\": \"" + path + "\"}");
            if (endpos > 0)
            {
                custom_request.AddHeader("Range", "bytes=" + startpos.ToString() + "-" + endpos.ToString());
            }
            return custom_request.ReadHeaderResponse_and_GetStreamResponse(true, true);
        }

        public string upload_session_start(byte[] buffer, int buffer_length = -1, bool close = false)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://content.dropboxapi.com/2/files/upload_session/start");
            request.Method = "POST";
            request.ContentType = "application/octet-stream";
            //request.ContentType += "text/plain; charset=dropbox-cors-hack";
            request.ContentLength = buffer_length == -1 ? buffer.Length : buffer_length;
            WebHeaderCollection myWebHeaderCollection = request.Headers;
            myWebHeaderCollection.Add("Authorization", "Bearer " + access_token);
            myWebHeaderCollection.Add("Dropbox-API-Arg", "{\"close\": " + close.ToString().ToLower() + "}");
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(buffer, 0, buffer_length == -1 ? buffer.Length : buffer_length);
            }
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string result = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return result;
        }
        public HttpRequest_ custom_request;
        public Stream upload_session_append(string session_id, long length_chunk, long offset)
        {
            custom_request = new HttpRequest_("https://content.dropboxapi.com/2/files/upload_session/append", "POST");
            custom_request.AddHeader("HOST", "content.dropboxapi.com");
            custom_request.AddHeader("Content-Type", "application/octet-stream");
            custom_request.AddHeader("Content-Length", length_chunk.ToString());
            custom_request.AddHeader("Authorization", "Bearer " + access_token);

            BuildJson build = new BuildJson();
            build.AddChildStringNodes("session_id", session_id);
            build.AddChildNodes("offset", offset.ToString());
            custom_request.AddHeader("Dropbox-API-Arg", build.GetJson);

            return custom_request.SendHeader_And_GetStream();
        }

        public string GetResponse_upload_session_append()
        {
            return custom_request.GetTextDataResponse(false, false);
        }

        public string upload_session_append(byte[] buffer, string session_id, long offset, int buffer_length = -1, int timeout = 2147483647)// can't multi
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://content.dropboxapi.com/2/files/upload_session/append");
            request.Method = "POST";
            request.Timeout = timeout;
            request.ContentType = "application/octet-stream";
            request.ContentLength = buffer_length == -1 ? buffer.Length : buffer_length;
            WebHeaderCollection myWebHeaderCollection = request.Headers;
            myWebHeaderCollection.Add("Authorization", "Bearer " + access_token);

            BuildJson build = new BuildJson();
            build.AddChildStringNodes("session_id", session_id);
            build.AddChildNodes("offset", offset.ToString());
            myWebHeaderCollection.Add("Dropbox-API-Arg", build.GetJson);

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(buffer, 0, buffer_length == -1 ? buffer.Length : buffer_length);
            }
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string result = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return result;
        }

        public string upload_session_append_v2(byte[] buffer, string session_id, long offset, int buffer_length = -1, bool close = false, int timeout = 2147483647)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://content.dropboxapi.com/2/files/upload_session/append_v2");
            request.Method = "POST";
            request.Timeout = timeout;
            request.ContentType = "application/octet-stream";
            request.ContentLength = buffer_length == -1 ? buffer.Length : buffer_length;
            WebHeaderCollection myWebHeaderCollection = request.Headers;
            myWebHeaderCollection.Add("Authorization", "Bearer " + access_token);

            BuildJson cursor = new BuildJson();
            cursor.AddChildStringNodes("session_id", session_id);
            cursor.AddChildNodes("offset", offset.ToString());

            BuildJson apiarg = new BuildJson();
            apiarg.AddChildNodes("cursor", cursor.GetJson);
            apiarg.AddChildNodes("close", close.ToString().ToLower());

            myWebHeaderCollection.Add("Dropbox-API-Arg", apiarg.GetJson);

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(buffer, 0, buffer_length == -1 ? buffer.Length : buffer_length);
            }
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string result = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return result;
        }

        private string EncodeUnicode(string input)
        {
            string str = "";
            foreach (char chr in input)
            {
                if (((ushort)chr) < 127) str += chr;
                else str += "\\u" + ((ushort)chr).ToString("X");
            }
            return str;
        }

        public string upload_session_finish(byte[] buffer, string session_id, long offset, string path, DropboxUploadMode mode, int buffer_length = -1, bool autorename = true, bool mute = false, int timeout = 2147483647)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://content.dropboxapi.com/2/files/upload_session/finish");
            request.Method = "POST";
            request.Timeout = timeout;
            request.ContentType = "application/octet-stream";
            //request.TransferEncoding = "";

            WebHeaderCollection myWebHeaderCollection = request.Headers;
            myWebHeaderCollection.Add("Authorization", "Bearer " + access_token);

            BuildJson cursor = new BuildJson();
            cursor.AddChildStringNodes("session_id", session_id);
            cursor.AddChildNodes("offset", offset.ToString());

            BuildJson commit = new BuildJson();
            commit.AddChildStringNodes("path", EncodeUnicode(path));
            commit.AddChildStringNodes("mode", mode.ToString());
            commit.AddChildNodes("autorename", autorename.ToString().ToLower());
            commit.AddChildNodes("mute", mute.ToString().ToLower());

            BuildJson apiarg = new BuildJson();
            apiarg.AddChildNodes("cursor", cursor.GetJson);
            apiarg.AddChildNodes("commit", commit.GetJson);

            myWebHeaderCollection.Add("Dropbox-API-Arg", apiarg.GetJson);
            request.ContentLength = 0;
            if (buffer != null)
            {
                request.ContentLength = buffer_length == -1 ? buffer.Length : buffer_length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(buffer, 0, buffer_length == -1 ? buffer.Length : buffer_length);
                }
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string result = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return result;
        }

        public string upload_session_finish_batch(string[] session_id, long[] offset, string[] path, DropboxUploadMode[] mode, Boolean autorename = true, Boolean mute = false, int timeout = 2147483647)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.dropboxapi.com/2/files/upload_session/finish_batch");
            request.Method = "POST";
            request.Timeout = timeout;
            request.ContentType = "application/octet-stream";

            WebHeaderCollection myWebHeaderCollection = request.Headers;
            myWebHeaderCollection.Add("Authorization", "Bearer " + access_token);

            //"{\"entries\": [{\"cursor\": {\"session_id\": \"1234faaf0678bcde\",\"offset\": 0},
            //                \"commit\": {\"path\": \"/Homework/math/Matrices.txt\",
            //                              \"mode\": {\".tag\": \"add\"},\"autorename\": true,\"mute\": false}}]}"
            //{"entries": [{"cursor": {"session_id": "AAAAAAAAAUM2ht5IKQGVVw", "offset": 10}, 
            //                  "commit": {"path": "/a_test.txt", "mode": {".tag": "update"}, "autorename": true, "mute": false}}]}
            BuildJson data = new BuildJson();

            List<string> entries = new List<string>();
            int index = 0;
            foreach (string session_id_ in session_id)
            {
                BuildJson entrie = new BuildJson();
                BuildJson cursor = new BuildJson();
                cursor.AddChildStringNodes("session_id", session_id[index]);
                cursor.AddChildNodes("offset", offset[index].ToString());

                BuildJson commit = new BuildJson();
                commit.AddChildStringNodes("path", EncodeUnicode(path[index]));

                BuildJson mode_ = new BuildJson();
                mode_.AddChildStringNodes(".tag", mode[index].ToString());
                commit.AddChildNodes("mode", mode_.GetJson);
                commit.AddChildNodes("autorename", autorename.ToString().ToLower());
                commit.AddChildNodes("mute", mute.ToString().ToLower());

                entrie.AddChildNodes("cursor", cursor.GetJson);
                entrie.AddChildNodes("commit", commit.GetJson);
                entries.Add(entrie.GetJson);
            }
            data.AddListChildNodes("entries", entries);

            byte[] buffer = Encoding.UTF8.GetBytes(data.GetJson);
            request.ContentLength = buffer.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(buffer, 0, buffer.Length);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string result = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return result;
        }

        public string upload(byte[] buffer, string path, int buffer_length = -1, DropboxUploadMode mode = DropboxUploadMode.add, bool autorename = true, bool mute = false, int timeout = Int32.MaxValue)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://content.dropboxapi.com/2/files/upload");
            request.Method = "POST";
            request.Timeout = timeout;
            request.ContentType = "application/octet-stream";
            request.ContentLength = buffer_length == -1 ? buffer.Length : buffer_length;
            WebHeaderCollection myWebHeaderCollection = request.Headers;
            myWebHeaderCollection.Add("Authorization", "Bearer " + access_token);
            myWebHeaderCollection.Add("Dropbox-API-Arg", "{\"path\": \"" + EncodeUnicode(path) + "\",\"mode\": \"" + mode.ToString() + "\",\"autorename\": " + autorename.ToString().ToLower() + ",\"mute\": " + mute.ToString().ToLower() + "}");
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(buffer, 0, buffer_length == -1 ? buffer.Length : buffer_length);
            }
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string result = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return result;
        }
        #endregion
    }
}
