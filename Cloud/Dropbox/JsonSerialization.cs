using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Converters;
using System.Collections;

namespace Cloud.Dropbox
{
    #region interface
    
    #region interface Response
    public interface IDropbox_Response_GetCurrentAccount
    {
        string account_id { get; }
        IDropbox_Response_GetCurrentAccount_name name { get; }
        string email { get;}
        bool email_verified { get;}
        bool disabled { get;}
        string locale { get; }
        string referral_link { get; }
        bool is_paired { get; }
        IDropbox_tag_string account_type { get; }
        string country { get;  }
        IDropbox_Response_GetCurrentAccount_team team { get;}
        string team_member_id { get; }
        string profile_photo_url { get; }
    }
    public interface IDropbox_Response_GetCurrentAccount_name
    {
        string given_name { get; }
        string surname { get; }
        string familiar_name { get; }
        string display_name { get; }
        string abbreviated_name { get; }
    }
    public interface IDropbox_Response_GetCurrentAccount_team
    {
        string id { get; }
        string name { get; }
        IDropbox_Response_GetCurrentAccount_Team_SharingPolicies sharing_policies { get; }
    }
    public interface IDropbox_Response_GetCurrentAccount_Team_SharingPolicies
    {
        IDropbox_tag_string Dropbox_tag_string { get; }
        IDropbox_tag_string shared_folder_join_policy { get; }
        IDropbox_tag_string shared_link_create_policy { get; }
    }
    public interface IDropbox_Response_GetSpaceUsage
    {
        long used { get; }
        IDropbox_Response_GetSpaceUsage_Allocation allocation { get; }
    }
    public interface IDropbox_Response_GetSpaceUsage_Allocation: IDropbox_tag_string
    {
        long allocated { get; set; }
    }
    public interface IDropbox_Response_MetaData : IDropbox_tag_string
    {
        string name { get; set; }
        string id { get; set; }
        string server_modified { get; set; }
        string client_modified { get; set; }
        long size { get; set; }
        string path_display { get; set; }
        bool has_explicit_shared_members { get; set; }
        string content_hash { get; set; }
        IDropbox_sharing_info sharing_info { get; }
        IEnumerable<IDropbox_property_groups> property_groups { get; }
    }
    public interface IDropbox_Response_ListFolder
    {
        IEnumerable<IDropbox_Response_MetaData> entries { get; }
        string cursor { get; set; }
        bool has_more { get; set; }
    }
    public interface IDropbox_Response_Error
    {
        string error_summary { get; set; }
        IDropbox_tag_string error { get; }
    }
    #endregion

    #region interface Request
    public interface IDropbox_Request_Metadata : IDropbox_Path
    {
        bool include_media_info { get; set; }
        bool include_deleted { get; set; }
        bool include_has_explicit_shared_members { get; set; }
    }
    public interface IDropbox_Request_ListFolder : IDropbox_Request_Metadata
    {
        bool recursive { get; set; }
    }
    public interface IDropbox_Request_ListFolderContinue
    {
        string cursor { get; set; }
    }
    public interface IDropbox_Request_MoveCopy
    {
        string from_path { get; set; }
        string to_path { get; set; }
        bool allow_shared_folder { get; set; }
        bool autorename { get; set; }
    }
    public interface IDropbox_Request_GetThumbnail : IDropbox_Path
    {
        string format { get; set; }
        Dropboxthumbnail size { get; set; }
    }
    public interface IDropbox_Request_UploadSessionAppend
    {
        string session_id { get; set; }
        long offset { get; set; }
    }
    public interface IDropbox_Request_UploadSessionFinish
    {
        IDropbox_Request_UploadSessionAppend cursor { get; set; }
        IDropbox_upload commit { get; set; }
    }
    #endregion



    public interface IDropbox_tag_string
    {
        string tag { get; set; }
    }
    public interface IDropbox_Path
    {
        /// <summary>
        /// Path to file/folder or id or rev (create folder use path only)
        /// Ex: /abc/def.pdf or id:abc123 or rev:def456
        /// </summary>
        string path { get; set; }
    }
    public interface IDropbox_upload : IDropbox_Path, IDropbox_upload_file_autorename_mute
    {
        Dropbox_WriteMode mode { get; set; }
    }
    #region Metadata field
    public interface IDropbox_sharing_info
    {
        bool read_only { get; set; }
        string parent_shared_folder_id { get; set; }
        bool traverse_only { get; set; }
        bool no_access { get; set; }
    }
    public interface IDropbox_property_groups
    {
        string template_id { get; set; }
        IDropbox_property_groups_field fields { get; }
    }
    public interface IDropbox_property_groups_field
    {
        string name { get; set; }
        string value { get; set; }
    }
    #endregion
    public interface IDropbox_upload_file_autorename_mute
    {
        bool autorename { get; set; }
        bool mute { get; set; }
    }
    public enum Dropbox_WriteMode
    {
        add = 0,
        overwrite,
        update
    }
    #endregion


    #region Class
    internal class Dropbox_Response_GetCurrentAccount : IDropbox_Response_GetCurrentAccount
    {
        public string account_id { get; set; }

        [JsonProperty("account_type")]
        Dropbox_tag_string account_type_;
        [JsonIgnore]
        public IDropbox_tag_string account_type { get { return account_type_; } }

        public string country { get; set; }

        public bool disabled { get; set; }

        public string email { get; set; }

        public bool email_verified { get; set; }

        public bool is_paired { get; set; }

        public string locale { get; set; }

        [JsonProperty("name")]
        Dropbox_Response_GetCurrentAccount_name name_;
        [JsonIgnore]
        public IDropbox_Response_GetCurrentAccount_name name { get { return name_; } }

        public string profile_photo_url { get; set; }

        public string referral_link { get; set; }

        [JsonProperty("team")]
        Dropbox_Response_GetCurrentAccount_team team_;
        [JsonIgnore]
        public IDropbox_Response_GetCurrentAccount_team team { get; set; }

        public string team_member_id { get; set; }
    }
    internal class Dropbox_Response_GetCurrentAccount_name : IDropbox_Response_GetCurrentAccount_name
    {
        public string abbreviated_name { get; set; }
        public string display_name { get; set; }

        public string familiar_name { get; set; }

        public string given_name { get; set; }

        public string surname { get; set; }
    }
    internal class Dropbox_Response_GetCurrentAccount_team : IDropbox_Response_GetCurrentAccount_team
    {
        public string id { get; set; }

        public string name { get; set; }

        [JsonProperty("sharing_policies")]
        Dropbox_Response_GetCurrentAccount_Team_SharingPolicies sharing_policies_;
        [JsonIgnore]
        public IDropbox_Response_GetCurrentAccount_Team_SharingPolicies sharing_policies { get { return sharing_policies_; } }
    }
    internal class Dropbox_Response_GetCurrentAccount_Team_SharingPolicies : IDropbox_Response_GetCurrentAccount_Team_SharingPolicies
    {
        [JsonProperty("Dropbox_tag_string")]
        Dropbox_tag_string Dropbox_tag_string_;
        [JsonIgnore]
        public IDropbox_tag_string Dropbox_tag_string { get { return Dropbox_tag_string_; } }

        [JsonProperty("shared_folder_join_policy")]
        Dropbox_tag_string shared_folder_join_policy_;
        [JsonIgnore]
        public IDropbox_tag_string shared_folder_join_policy { get { return shared_folder_join_policy_; } }

        [JsonProperty("shared_link_create_policy")]
        Dropbox_tag_string shared_link_create_policy_;
        [JsonIgnore]
        public IDropbox_tag_string shared_link_create_policy { get { return shared_link_create_policy_; } }
    }

    internal class Dropbox_Response_GetSpaceUsage : IDropbox_Response_GetSpaceUsage
    {
        [JsonProperty("allocation")]
        Dropbox_Response_GetSpaceUsage_Allocation allocation_;
        [JsonIgnore]
        public IDropbox_Response_GetSpaceUsage_Allocation allocation { get { return allocation_; } }

        public long used { get; set; }
    }
    internal class Dropbox_Response_GetSpaceUsage_Allocation : IDropbox_Response_GetSpaceUsage_Allocation
    {
        public long allocated { get; set; }

        public string tag { get; set; }
    }



    internal class Dropbox_Response_ListFolder : IDropbox_Response_ListFolder
    {
        [JsonProperty("entries")]
        List<Dropbox_Response_MetaData> entries_;
        [JsonIgnore]
        public IEnumerable<IDropbox_Response_MetaData> entries
        {
            get
            {
                foreach (IDropbox_Response_MetaData item in entries_) yield return item;
            }
        }
        public bool has_more { get; set; }
        public string cursor { get; set; }
    }
    internal class Dropbox_Response_MetaData : IDropbox_Response_MetaData
    {
        public string client_modified { get; set; }
        public string content_hash { get; set; }
        public bool has_explicit_shared_members { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string path_display { get; set; }

        [JsonProperty("property_groups")]
        List<Dropbox_property_groups> property_groups_;
        [JsonIgnore]
        public IEnumerable<IDropbox_property_groups> property_groups
        {
            get
            {
                foreach (IDropbox_property_groups item in property_groups_) yield return item;
            }
        }
        public string server_modified { get; set; }

        [JsonProperty("sharing_info")]
        Dropbox_sharing_info sharing_info_;
        [JsonIgnore]
        public IDropbox_sharing_info sharing_info { get { return sharing_info_; } }

        public long size { get; set; }

        [JsonProperty(".tag")]
        public string tag { get; set; }
    }
    internal class Dropbox_property_groups : IDropbox_property_groups
    {
        [JsonProperty("fields")]
        Dropbox_property_groups_field fields_;
        [JsonIgnore]
        public IDropbox_property_groups_field fields{ get { return fields_; } }
        public string template_id { get; set; }
    }
    internal class Dropbox_property_groups_field : IDropbox_property_groups_field
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    internal class Dropbox_sharing_info : IDropbox_sharing_info
    {
        public bool no_access { get; set; }
        public string parent_shared_folder_id { get; set; }
        public bool read_only { get; set; }
        public bool traverse_only { get; set; }
    }

    internal class Dropbox_Response_Error : IDropbox_Response_Error
    {
        [JsonProperty("error")]
        Dropbox_tag_string error_;
        public IDropbox_tag_string error
        {
            get
            {
                return error_;
            }
        }

        public string error_summary { get; set; }
    }
    internal class Dropbox_tag_string : IDropbox_tag_string
    {
        [JsonProperty(".tag")]
        public string tag { get; set; }
    }



    internal class Dropbox_Request_UploadSessionAppend : IDropbox_Request_UploadSessionAppend
    {
        public long offset { get; set; }
        public string session_id { get; set; }
    }

    #endregion
}
