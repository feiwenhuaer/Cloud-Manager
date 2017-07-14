using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cloud.GoogleDrive
{
  public class Drive_About
  {
    public string kind { get; set; }
    public string etag { get; set; }
    public string selfLink { get; set; }
    public string name { get; set; }
    public Drive_User user { get; set; }
    public long quotaBytesTotal { get; set; }
    public long quotaBytesUsed { get; set; }
    public long quotaBytesUsedAggregate { get; set; }
    public long quotaBytesUsedInTrash { get; set; }
    public string quotaType { get; set; }
    public List<Drive_About_quotaBytesByService> quotaBytesByService { get; set; }
    public long largestChangeId { get; set; }
    public long remainingChangeIds { get; set; }
    public string rootFolderId { get; set; }
    public string domainSharingPolicy { get; set; }
    public string permissionId { get; set; }
    public List<Drive_About_Format> importFormats { get; set; }
    public List<Drive_About_Format> exportFormats { get; set; }
    public List<Drive_About_additionalRoleInfo> additionalRoleInfo { get; set; }
    public List<Drive_About_feature> features { get; set; }
    public List<Drive_About_maxUploadSize> maxUploadSizes { get; set; }
    public bool isCurrentAppInstalled { get; set; }
    public string languageCode { get; set; }
    public List<string> folderColorPalette { get; set; }
    public List<Drive_About_teamDriveTheme> teamDriveThemes { get; set; }
  }

  public class Drive_About_quotaBytesByService
  {
    public string serviceName { get; set; }
    public long bytesUsed { get; set; }
  }
  public class Drive_About_Format
  {
    public string source { get; set; }
    public List<string> targets { get; set; }
  }
  public class Drive_About_additionalRoleInfo
  {
    public string type { get; set; }
    public List<Drive_About_roleSet> roleSets { get; set; }
  }
  public class Drive_About_roleSet
  {
    public string primaryRole { get; set; }
    public List<string> additionalRoles { get; set; }
  }
  public class Drive_About_feature
  {
    public string featureName { get; set; }
    public double featureRate { get; set; }
  }
  public class Drive_About_maxUploadSize
  {
    public string type { get; set; }
    public long size { get; set; }
  }
  public class Drive_About_teamDriveTheme
  {
    public string id { get; set; }
    public string backgroundImageLink { get; set; }
    public string colorRgb { get; set; }
  }


  public class Drive_Parent_List
  {
    public string kind { get; set; }
    public string etag { get; set; }
    public string selfLink { get; set; }
    public List<DriveItemMetadata_parent> items { get; set; }
  }

  public class Drive_Files_list
  {
    public string kind { get; set; }
    public string etag { get; set; }
    public string selfLink { get; set; }
    public string nextPageToken { get; set; }
    public string nextLink { get; set; }
    public bool incompleteSearch { get; set; }
    public List<DriveItemMetadata_Item> items { get; set; }
  }  

  #region metadata
  public class DriveItemMetadata_Item
  {
    public string kind { get; set; }
    public string id { get; set; }
    public string etag { get; set; }
    public string selfLink { get; set; }
    public string webContentLink { get; set; }
    public string webViewLink { get; set; }
    public string alternateLink { get; set; }
    public string embedLink { get; set; }
    //openWithLinks { (key): string }
    public string defaultOpenWithLink { get; set; }
    public string iconLink { get; set; }
    public bool hasThumbnail { get; set; }
    public string thumbnailLink { get; set; }
    public long thumbnailVersion { get; set; }
    public DriveItemMetadata_thumbnail thumbnail { get; set; }
    public string title { get; set; }
    public string mimeType { get; set; }
    public string description { get; set; }
    public DriveItemMetadata_label labels { get; set; }
    public DateTime createdDate { get; set; }
    public DateTime modifiedDate { get; set; }
    public DateTime modifiedByMeDate { get; set; }
    public DateTime lastViewedByMeDate { get; set; }
    public DateTime markedViewedByMeDate { get; set; }
    public DateTime sharedWithMeDate { get; set; }
    public long version { get; set; }
    public Drive_User sharingUser { get; set; }
    public List<DriveItemMetadata_parent> parents { get; set; }
    public string downloadUrl { get; set; }
    //exportLinks { (key): string }
    public DriveItemMetadata_indexableText indexableText { get; set; }
    public DriveItemMetadata_permission userPermission { get; set; }
    public List<DriveItemMetadata_permission> permissions { get; set; }
    public bool hasAugmentedPermissions { get; set; }
    public string originalFilename { get; set; }
    public string fileExtension { get; set; }
    public string fullFileExtension { get; set; }
    public string md5Checksum { get; set; }
    public long fileSize { get; set; }
    public long quotaBytesUsed { get; set; }
    public List<string> ownerNames { get; set; }
    public List<Drive_User> owners { get; set; }
    public string teamDriveId { get; set; }
    public string lastModifyingUserName { get; set; }
    public Drive_User lastModifyingUser { get; set; }
    public bool ownedByMe { get; set; }
    public DriveItemMetadata_capability capabilities { get; set; }
    public bool editable { get; set; }
    public bool canComment { get; set; }
    public bool canReadRevisions { get; set; }
    public bool shareable { get; set; }
    public bool copyable { get; set; }
    public bool writersCanShare { get; set; }
    public bool shared { get; set; }
    public bool explicitlyTrashed { get; set; }
    public Drive_User trashingUser { get; set; }
    public DateTime trashedDate { get; set; }
    public bool appDataContents { get; set; }
    public string headRevisionId { get; set; }
    public List<DriveItemMetadata_property> properties { get; set; }
    public string folderColorRgb { get; set; }
    public DriveItemMetadata_imageMediaMetadata imageMediaMetadata { get; set; }
    public DriveItemMetadata_videoMediaMetadata videoMediaMetadata { get; set; }
    public List<string> spaces { get; set; }
    public bool isAppAuthorized { get; set; }
  }
  public class DriveItemMetadata_thumbnail
  {
    public byte[] image { get; set; }
    public string mimeType { get; set; }
  }
  public class DriveItemMetadata_picture
  {
    public string url { get; set; }
  }
  public class DriveItemMetadata_indexableText
  {
    public string text { get; set; }
  }
  public class DriveItemMetadata_label
  {
    public bool starred { get; set; }
    public bool hidden { get; set; }
    public bool trashed { get; set; }
    public bool restricted { get; set; }
    public bool viewed { get; set; }
  }
  public class DriveItemMetadata_parent
  {
    public string kind { get; set; }
    public string id { get; set; }
    public string selfLink { get; set; }
    public string parentLink { get; set; }
    public bool isRoot { get; set; }
  }
  public class DriveItemMetadata_permission
  {
    public string kind { get; set; }
    public string etag { get; set; }
    public string id { get; set; }
    public string selfLink { get; set; }
    public string name { get; set; }
    public string emailAddress { get; set; }
    public string domain { get; set; }
    public string role { get; set; }
    public List<string> additionalRoles { get; set; }
    public string type { get; set; }
    public string value { get; set; }
    public string authKey { get; set; }
    public bool withLink { get; set; }
    public string photoLink { get; set; }
    public DateTime expirationDate { get; set; }
    public List<DriveItemMetadata_teamDrivePermissionDetail> teamDrivePermissionDetails { get; set; }
    public bool deleted { get; set; }
  }
  public class DriveItemMetadata_teamDrivePermissionDetail
  {
    public string teamDrivePermissionType { get; set; }
    public string role { get; set; }
    public List<string> additionalRoles { get; set; }
    public string inheritedFrom { get; set; }
    public bool inherited { get; set; }
  }
  public class DriveItemMetadata_capability
  {
    public bool canAddChildren { get; set; }
    public bool canChangeRestrictedDownload { get; set; }
    public bool canComment { get; set; }
    public bool canCopy { get; set; }
    public bool canDelete { get; set; }
    public bool canDownload { get; set; }
    public bool canEdit { get; set; }
    public bool canListChildren { get; set; }
    public bool canMoveItemIntoTeamDrive { get; set; }
    public bool canMoveTeamDriveItem { get; set; }
    public bool canReadRevisions { get; set; }
    public bool canReadTeamDrive { get; set; }
    public bool canRemoveChildren { get; set; }
    public bool canRename { get; set; }
    public bool canShare { get; set; }
    public bool canTrash { get; set; }
    public bool canUntrash { get; set; }
  }
  public class DriveItemMetadata_property
  {
    public string kind { get; set; }
    public string etag { get; set; }
    public string selfLink { get; set; }
    public string visibility { get; set; }
    public string value { get; set; }
  }
  public class DriveItemMetadata_imageMediaMetadata
  {
    public int width { get; set; }
    public int height { get; set; }
    public int rotation { get; set; }
    public DriveItemMetadata_location location { get; set; }
    public string date { get; set; }
    public string cameraMake { get; set; }
    public string cameraModel { get; set; }
    public float exposureTime { get; set; }
    public float aperture { get; set; }
    public bool flashUsed { get; set; }
    public float focalLength { get; set; }
    public int isoSpeed { get; set; }
    public string meteringMode { get; set; }
    public string sensor { get; set; }
    public string exposureMode { get; set; }
    public string colorSpace { get; set; }
    public string whiteBalance { get; set; }
    public float exposureBias { get; set; }
    public float maxApertureValue { get; set; }
    public int subjectDistance { get; set; }
    public string lens { get; set; }
  }
  public class DriveItemMetadata_videoMediaMetadata
  {
    public int width { get; set; }
    public int height { get; set; }
    public long durationMillis { get; set; }
  }
  public class DriveItemMetadata_location
  {
    public double latitude { get; set; }
    public double longitude { get; set; }
    public double altitude { get; set; }
  }
  //public class key
  //{

  //}
  public class Drive_User
  {
    public string kind { get; set; }
    public string displayName { get; set; }
    public DriveItemMetadata_picture picture { get; set; }
    public bool isAuthenticatedUser { get; set; }
    public string permissionId { get; set; }
    public string emailAddress { get; set; }
  }
  #endregion
}
