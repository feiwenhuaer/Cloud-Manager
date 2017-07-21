using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cloud.GoogleDrive
{
  public class Drive_About
  {
    [JsonIgnore]
    public string kind { get; set; }
    [JsonIgnore]
    public string etag { get; set; }
    [JsonIgnore]
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
    /// <summary>
    /// The type of file. This is always drive#file.	
    /// </summary>
    [JsonIgnore]
    public string kind { get; set; }

    /// <summary>
    /// The ID of the file.
    /// </summary>
    public string id { get; set; }

    /// <summary>
    /// ETag of the file.	
    /// </summary>
    [JsonIgnore]
    public string etag { get; set; }
    
    /// <summary>
    /// A link back to this file.	
    /// </summary>
    [JsonIgnore]
    public string selfLink { get; set; }
    
    /// <summary>
    /// A link for downloading the content of the file in a browser using cookie based authentication. In cases where the content is shared publicly, the content can be downloaded without any credentials.	
    /// </summary>
    [JsonIgnore]
    public string webContentLink { get; set; }
    
    /// <summary>
    /// A link only available on public folders for viewing their static web assets (HTML, CSS, JS, etc) via Google Drive's Website Hosting.	
    /// </summary>
    [JsonIgnore]
    public string webViewLink { get; set; }
    
    /// <summary>
    /// A link for opening the file in a relevant Google editor or viewer.	
    /// </summary>
    [JsonIgnore]
    public string alternateLink { get; set; }
    
    /// <summary>
    /// A link for embedding the file.	
    /// </summary>
    [JsonIgnore]
    public string embedLink { get; set; }
    //openWithLinks { (key): string }
    
    /// <summary>
    /// A link to open this file with the user's default app for this file. Only populated when the drive.apps.readonly scope is used.	
    /// </summary>
    [JsonIgnore]
    public string defaultOpenWithLink { get; set; }
    
    /// <summary>
    /// A link to the file's icon.	
    /// </summary>
    [JsonIgnore]
    public string iconLink { get; set; }
    
    /// <summary>
    /// Whether this file has a thumbnail. This does not indicate whether the requesting app has access to the thumbnail. To check access, look for the presence of the thumbnailLink field.	
    /// </summary>
    [JsonIgnore]
    public bool hasThumbnail { get; set; }
    
    /// <summary>
    /// A short-lived link to the file's thumbnail. Typically lasts on the order of hours. Only populated when the requesting app can access the file's content.	
    /// </summary>
    [JsonIgnore]
    public string thumbnailLink { get; set; }
    
    /// <summary>
    /// The thumbnail version for use in thumbnail cache invalidation.	
    /// </summary>
    [JsonIgnore]
    public long thumbnailVersion { get; set; }
    
    /// <summary>
    /// A thumbnail for the file. This will only be used if Drive cannot generate a standard thumbnail.	
    /// </summary>
    [JsonIgnore]
    public DriveItemMetadata_thumbnail thumbnail { get; set; }
    
    /// <summary>
    /// The title of this file. Note that for immutable items such as the top level folders of Team Drives, My Drive root folder, and Application Data folder the title is constant.	
    /// </summary>
    public string title { get; set; }
    
    /// <summary>
    /// The MIME type of the file. This is only mutable on update when uploading new content. This field can be left blank, and the mimetype will be determined from the uploaded content's MIME type.	
    /// </summary>
    public string mimeType { get; set; }
    
    /// <summary>
    /// A short description of the file.	
    /// </summary>
    public string description { get; set; }
    
    /// <summary>
    /// A group of labels for the file.	
    /// </summary>
    public DriveItemMetadata_label labels { get; set; }
    
    /// <summary>
    /// Create time for this file (formatted RFC 3339 timestamp).	
    /// </summary>
    [JsonIgnore]
    public DateTime createdDate { get; set; }
    
    /// <summary>
    /// Last time this file was modified by anyone (formatted RFC 3339 timestamp). This is only mutable on update when the setModifiedDate parameter is set.	
    /// </summary>
    public DateTime modifiedDate { get; set; }
    
    /// <summary>
    /// Last time this file was modified by the user (formatted RFC 3339 timestamp). Note that setting modifiedDate will also update the modifiedByMe date for the user which set the date.	
    /// </summary>
    [JsonIgnore]
    public DateTime modifiedByMeDate { get; set; }
    
    /// <summary>
    /// Last time this file was viewed by the user (formatted RFC 3339 timestamp).	
    /// </summary>
    public DateTime lastViewedByMeDate { get; set; }
    
    /// <summary>
    /// Deprecated
    /// </summary>
    public DateTime markedViewedByMeDate { get; set; }
    
    /// <summary>
    /// Time at which this file was shared with the user (formatted RFC 3339 timestamp).	
    /// </summary>
    [JsonIgnore]
    public DateTime sharedWithMeDate { get; set; }
    
    /// <summary>
    /// A monotonically increasing version number for the file. This reflects every change made to the file on the server, even those not visible to the requesting user.	
    /// </summary>
    [JsonIgnore]
    public long version { get; set; }
    
    /// <summary>
    /// User that shared the item with the current user, if available.	
    /// </summary>
    [JsonIgnore]
    public Drive_User sharingUser { get; set; }
    
    /// <summary>
    /// Collection of parent folders which contain this file. Setting this field will put the file in all of the provided folders.On insert, if no folders are provided, the file will be placed in the default root folder.
    /// </summary>
    public List<DriveItemMetadata_parent> parents { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonIgnore]
    public string downloadUrl { get; set; }
    //exportLinks { (key): string }

    /// <summary>
    /// Indexable text attributes for the file. This property can only be written, and is not returned by files.get. For more information, see https://developers.google.com/drive/v3/web/practices#custom_thumbnails_and_indexable_text
    /// </summary>
    public DriveItemMetadata_indexableText indexableText { get; set; }

    /// <summary>
    /// The permissions for the authenticated user on this file.	
    /// </summary>
    public DriveItemMetadata_permission userPermission { get; set; }

    /// <summary>
    /// The list of permissions for users with access to this file. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnore]////////////////////////////////
    public List<DriveItemMetadata_permission> permissions { get; set; }

    /// <summary>
    /// Whether any users are granted file access directly on this file. This field is only populated for Team Drive files.	
    /// </summary>
    [JsonIgnore]
    public bool hasAugmentedPermissions { get; set; }

    /// <summary>
    /// The original filename of the uploaded content if available, or else the original value of the title field. This is only available for files with binary content in Drive.	
    /// </summary>
    public string originalFilename { get; set; }

    /// <summary>
    /// The final component of fullFileExtension with trailing text that does not appear to be part of the extension removed. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnore]
    public string fileExtension { get; set; }

    /// <summary>
    /// The full file extension; extracted from the title. May contain multiple concatenated extensions, such as "tar.gz". Removing an extension from the title does not clear this field; however, changing the extension on the title does update this field. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnore]
    public string fullFileExtension { get; set; }

    /// <summary>
    /// An MD5 checksum for the content of this file. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnore]
    public string md5Checksum { get; set; }

    /// <summary>
    /// The size of the file in bytes. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnore]
    public long fileSize { get; set; } = -1;

    /// <summary>
    /// The number of quota bytes used by this file.	
    /// </summary>
    [JsonIgnore]
    public long quotaBytesUsed { get; set; }

    /// <summary>
    /// Name(s) of the owner(s) of this file. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnore]
    public List<string> ownerNames { get; set; }

    /// <summary>
    /// The owner(s) of this file. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnore]
    public List<Drive_User> owners { get; set; }

    /// <summary>
    /// ID of the Team Drive the file resides in.	
    /// </summary>
    [JsonIgnore]
    public string teamDriveId { get; set; }

    /// <summary>
    /// Name of the last user to modify this file.	
    /// </summary>
    [JsonIgnore]
    public string lastModifyingUserName { get; set; }

    /// <summary>
    /// The last user to modify this file.	
    /// </summary>
    [JsonIgnore]
    public Drive_User lastModifyingUser { get; set; }

    /// <summary>
    /// Whether the file is owned by the current user. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnore]
    public bool ownedByMe { get; set; }

    /// <summary>
    /// Capabilities the current user has on this file. Each capability corresponds to a fine-grained action that a user may take.
    /// </summary>
    [JsonIgnore]
    public DriveItemMetadata_capability capabilities { get; set; }

    /// <summary>
    /// Deprecated: use capabilities/canEdit.	
    /// </summary>
    [JsonIgnore]
    public bool editable { get; set; }

    /// <summary>
    /// Deprecated: use capabilities/canComment.
    /// </summary>
    [JsonIgnore]
    public bool canComment { get; set; }

    /// <summary>
    /// Deprecated: use capabilities/canReadRevisions.	
    /// </summary>
    [JsonIgnore]
    public bool canReadRevisions { get; set; }

    /// <summary>
    /// Deprecated: use capabilities/canShare.	
    /// </summary>
    [JsonIgnore]
    public bool shareable { get; set; }

    /// <summary>
    /// Deprecated: use capabilities/canCopy.	
    /// </summary>
    [JsonIgnore]
    public bool copyable { get; set; }

    /// <summary>
    /// Whether writers can share the document with other users. Not populated for Team Drive files.	
    /// </summary>
    public bool writersCanShare { get; set; }

    /// <summary>
    /// Whether the file has been shared. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnore]
    public bool shared { get; set; }

    /// <summary>
    /// Whether this file has been explicitly trashed, as opposed to recursively trashed.	
    /// </summary>
    [JsonIgnore]
    public bool explicitlyTrashed { get; set; }

    /// <summary>
    /// If the file has been explicitly trashed, the user who trashed it. Only populated for Team Drive files.	
    /// </summary>
    [JsonIgnore]
    public Drive_User trashingUser { get; set; }

    /// <summary>
    /// The time that the item was trashed (formatted RFC 3339 timestamp). Only populated for Team Drive files.	
    /// </summary>
    [JsonIgnore]
    public DateTime trashedDate { get; set; }

    /// <summary>
    /// Whether this file is in the Application Data folder.	
    /// </summary>
    [JsonIgnore]
    public bool appDataContents { get; set; }

    /// <summary>
    /// The ID of the file's head revision. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnore]
    public string headRevisionId { get; set; }

    /// <summary>
    /// The list of properties.	
    /// </summary>
    [JsonIgnore]
    public List<DriveItemMetadata_property> properties { get; set; }

    /// <summary>
    /// Folder color as an RGB hex string if the file is a folder. The list of supported colors is available in the folderColorPalette field of the About resource. If an unsupported color is specified, it will be changed to the closest color in the palette. Not populated for Team Drive files.	
    /// </summary>
    public string folderColorRgb { get; set; }

    /// <summary>
    /// Metadata about image media. This will only be present for image types, and its contents will depend on what can be parsed from the image content.	
    /// </summary>
    [JsonIgnore]
    public DriveItemMetadata_imageMediaMetadata imageMediaMetadata { get; set; }

    /// <summary>
    /// Metadata about video media. This will only be present for video types.	
    /// </summary>
    [JsonIgnore]
    public DriveItemMetadata_videoMediaMetadata videoMediaMetadata { get; set; }

    /// <summary>
    /// The list of spaces which contain the file. Supported values are 'drive', 'appDataFolder' and 'photos'.	
    /// </summary>
    [JsonIgnore]
    public List<string> spaces { get; set; }

    /// <summary>
    /// Whether the file was created or opened by the requesting app.	
    /// </summary>
    [JsonIgnore]
    public bool isAppAuthorized { get; set; }
  }
  public class DriveItemMetadata_thumbnail
  {
    /// <summary>
    /// The URL-safe Base64 encoded bytes of the thumbnail image. It should conform to RFC 4648 section 5.	
    /// </summary>
    [JsonIgnore]
    public byte[] image { get; set; }
    /// <summary>
    /// The MIME type of the thumbnail.	
    /// </summary>
    [JsonIgnore]
    public string mimeType { get; set; }
  }
  public class DriveItemMetadata_picture
  {
    /// <summary>
    /// A URL that points to a profile picture of this user.	
    /// </summary>
    [JsonIgnore]
    public string url { get; set; }
  }
  public class DriveItemMetadata_indexableText
  {
    /// <summary>
    /// The text to be indexed for this file.	
    /// </summary>
    public string text { get; set; }
  }
  public class DriveItemMetadata_label
  {
    /// <summary>
    /// Whether this file is starred by the user.	
    /// </summary>
    public bool starred { get; set; }

    /// <summary>
    /// Deprecated
    /// </summary>
    public bool hidden { get; set; }

    /// <summary>
    /// Whether this file has been trashed. This label applies to all users accessing the file; however, only owners are allowed to see and untrash files.	
    /// </summary>
    public bool trashed { get; set; }

    /// <summary>
    /// Whether viewers and commenters are prevented from downloading, printing, and copying this file.	
    /// </summary>
    public bool restricted { get; set; }

    /// <summary>
    /// Whether this file has been viewed by this user.	
    /// </summary>
    public bool viewed { get; set; }
  }
  public class DriveItemMetadata_parent
  {
    /// <summary>
    /// This is always drive#parent Reference.	
    /// </summary>
    [JsonIgnore]
    public string kind { get; set; }
    /// <summary>
    /// The ID of the parent.	
    /// </summary>
    [JsonIgnore]
    public string id { get; set; }
    /// <summary>
    /// A link back to this reference.	
    /// </summary>
    [JsonIgnore]
    public string selfLink { get; set; }
    /// <summary>
    /// A link to the parent.	
    /// </summary>
    [JsonIgnore]
    public string parentLink { get; set; }
    /// <summary>
    /// Whether or not the parent is the root folder.	
    /// </summary>
    [JsonIgnore]
    public bool isRoot { get; set; }
  }
  public class DriveItemMetadata_permission
  {
    /// <summary>
    /// This is always drive#permission.	
    /// </summary>
    [JsonIgnore]
    public string kind { get; set; }

    /// <summary>
    /// The ETag of the permission.	
    /// </summary>
    [JsonIgnore]
    public string etag { get; set; }

    /// <summary>
    /// The ID of the user this permission refers to, and identical to the permissionId in the About and Files resources. When making a drive.permissions.insert request, exactly one of the id or value fields must be specified unless the permission type is anyone, in which case both id and value are ignored.	
    /// </summary>
    public string id { get; set; }

    /// <summary>
    /// A link back to this permission.	
    /// </summary>
    [JsonIgnore]
    public string selfLink { get; set; }

    /// <summary>
    /// The name for this permission.	
    /// </summary>
    [JsonIgnore]
    public string name { get; set; }

    /// <summary>
    /// The email address of the user or group this permission refers to. This is an output-only field which is present when the permission type is user or group.	
    /// </summary>
    [JsonIgnore]
    public string emailAddress { get; set; }

    /// <summary>
    /// The domain name of the entity this permission refers to. This is an output-only field which is present when the permission type is user, group or domain.	
    /// </summary>
    [JsonIgnore]
    public string domain { get; set; }

    /// <summary>
    /// The primary role for this user. While new values may be supported in the future
    /// </summary>
    public DriveItemMetadata_PermissionRole role { get; set; }

    /// <summary>
    /// Additional roles for this user. Only commenter is currently allowed, though more may be supported in the future.	
    /// </summary>
    public List<string> additionalRoles { get; set; }

    /// <summary>
    /// The account type
    /// </summary>
    public DriveItemMetadata_PermissionType type { get; set; }

    /// <summary>
    /// The email address or domain name for the entity. This is used during inserts and is not populated in responses. When making a drive.permissions.insert request, exactly one of the id or value fields must be specified unless the permission type is anyone, in which case both id and value are ignored.	
    /// </summary>
    public string value { get; set; }

    /// <summary>
    /// The authkey parameter required for this permission.	
    /// </summary>
    [JsonIgnore]
    public string authKey { get; set; }

    /// <summary>
    /// Whether the link is required for this permission.	
    /// </summary>
    public bool withLink { get; set; }

    /// <summary>
    /// A link to the profile photo, if available.	
    /// </summary>
    [JsonIgnore]
    public string photoLink { get; set; }

    /// <summary>
    /// The time at which this permission will expire (RFC 3339 date-time). Expiration dates have the following restrictions:
    /// They can only be set on user and group permissions
    /// The date must be in the future
    /// The date cannot be more than a year in the future
    /// The date can only be set on drive.permissions.update requests
    /// </summary>
    public DateTime expirationDate { get; set; }

    /// <summary>
    /// Details of whether the permissions on this Team Drive item are inherited or directly on this item. This is an output-only field which is present only for Team Drive items.	
    /// </summary>
    [JsonIgnore] ////////////////////////////////////////
    public List<DriveItemMetadata_teamDrivePermissionDetail> teamDrivePermissionDetails { get; set; }

    /// <summary>
    /// Whether the account associated with this permission has been deleted. This field only pertains to user and group permissions.	
    /// </summary>
    [JsonIgnore]
    public bool deleted { get; set; }
  }
  public enum DriveItemMetadata_PermissionRole
  {
    organizer,
    owner,
    reader,
    writer
  }
  public enum DriveItemMetadata_PermissionType
  {
    user,
    group,
    domain,
    anyone
  }
  public enum DriveItemMetadata_TeamPermissionType
  {
    file,
    member
  }

  public class DriveItemMetadata_teamDrivePermissionDetail
  {
    /// <summary>
    /// The Team Drive permission type for this user. While new values may be added in future
    /// </summary>
    [JsonIgnore]
    public DriveItemMetadata_TeamPermissionType teamDrivePermissionType { get; set; }
    
    /// <summary>
    /// The primary role for this user. While new values may be added in the future
    /// </summary>
    [JsonIgnore]
    public DriveItemMetadata_PermissionRole role { get; set; }

    /// <summary>
    /// Additional roles for this user. Only commenter is currently possible, though more may be supported in the future.	
    /// </summary>
    [JsonIgnore]
    public List<string> additionalRoles { get; set; }

    /// <summary>
    /// The ID of the item from which this permission is inherited. This is an output-only field and is only populated for members of the Team Drive.	
    /// </summary>
    [JsonIgnore]
    public string inheritedFrom { get; set; }

    /// <summary>
    /// Whether this permission is inherited. This field is always populated. This is an output-only field.	
    /// </summary>
    [JsonIgnore]
    public bool inherited { get; set; }
  }
  public class DriveItemMetadata_capability
  {
    /// <summary>
    /// 
    /// </summary>
    [JsonIgnore]
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
    /// <summary>
    /// This is always drive#property.	
    /// </summary>
    [JsonIgnore]
    public string kind { get; set; }

    /// <summary>
    /// ETag of the property.	
    /// </summary>
    [JsonIgnore]
    public string etag { get; set; }

    /// <summary>
    /// The link back to this property.	
    /// </summary>
    [JsonIgnore]
    public string selfLink { get; set; }

    /// <summary>
    /// The key of this property.	
    /// </summary>
    [JsonIgnore]
    public string key { get; set; }

    /// <summary>
    /// The visibility of this property.	
    /// </summary>
    [JsonIgnore]
    public string visibility { get; set; }

    /// <summary>
    /// The value of this property.	
    /// </summary>
    [JsonIgnore]
    public string value { get; set; }
  }
  public class DriveItemMetadata_imageMediaMetadata
  {
    /// <summary>
    /// The width of the image in pixels.	
    /// </summary>
    [JsonIgnore]
    public int width { get; set; }

    /// <summary>
    /// The height of the image in pixels.	
    /// </summary>
    [JsonIgnore]
    public int height { get; set; }

    /// <summary>
    /// The rotation in clockwise degrees from the image's original orientation.	
    /// </summary>
    [JsonIgnore]
    public int rotation { get; set; }

    /// <summary>
    /// Geographic location information stored in the image.	
    /// </summary>
    [JsonIgnore]
    public DriveItemMetadata_location location { get; set; }

    /// <summary>
    /// The date and time the photo was taken (EXIF format timestamp).	
    /// </summary>
    [JsonIgnore]
    public string date { get; set; }

    /// <summary>
    /// The make of the camera used to create the photo.	
    /// </summary>
    [JsonIgnore]
    public string cameraMake { get; set; }

    /// <summary>
    /// The model of the camera used to create the photo.	
    /// </summary>
    [JsonIgnore]
    public string cameraModel { get; set; }

    /// <summary>
    /// The length of the exposure, in seconds.	
    /// </summary>
    [JsonIgnore]
    public float exposureTime { get; set; }

    /// <summary>
    /// The aperture used to create the photo (f-number).	
    /// </summary>
    [JsonIgnore]
    public float aperture { get; set; }

    /// <summary>
    /// Whether a flash was used to create the photo.	
    /// </summary>
    [JsonIgnore]
    public bool flashUsed { get; set; }

    /// <summary>
    /// The focal length used to create the photo, in millimeters.	
    /// </summary>
    [JsonIgnore]
    public float focalLength { get; set; }

    /// <summary>
    /// The ISO speed used to create the photo.	
    /// </summary>
    [JsonIgnore]
    public int isoSpeed { get; set; }

    /// <summary>
    /// The metering mode used to create the photo.	
    /// </summary>
    [JsonIgnore]
    public string meteringMode { get; set; }

    /// <summary>
    /// The type of sensor used to create the photo.	
    /// </summary>
    [JsonIgnore]
    public string sensor { get; set; }

    /// <summary>
    /// The exposure mode used to create the photo.	
    /// </summary>
    [JsonIgnore]
    public string exposureMode { get; set; }

    /// <summary>
    /// The color space of the photo.	
    /// </summary>
    [JsonIgnore]
    public string colorSpace { get; set; }

    /// <summary>
    /// The white balance mode used to create the photo.	
    /// </summary>
    [JsonIgnore]
    public string whiteBalance { get; set; }

    /// <summary>
    /// The exposure bias of the photo (APEX value).	
    /// </summary>
    [JsonIgnore]
    public float exposureBias { get; set; }

    /// <summary>
    /// The smallest f-number of the lens at the focal length used to create the photo (APEX value).	
    /// </summary>
    [JsonIgnore]
    public float maxApertureValue { get; set; }

    /// <summary>
    /// The distance to the subject of the photo, in meters.	
    /// </summary>
    [JsonIgnore]
    public int subjectDistance { get; set; }

    /// <summary>
    /// The lens used to create the photo.	
    /// </summary>
    [JsonIgnore]
    public string lens { get; set; }
  }
  public class DriveItemMetadata_videoMediaMetadata
  {
    /// <summary>
    /// The width of the video in pixels.	
    /// </summary>
    [JsonIgnore]
    public int width { get; set; }

    /// <summary>
    /// The height of the video in pixels.	
    /// </summary>
    [JsonIgnore]
    public int height { get; set; }

    /// <summary>
    /// The duration of the video in milliseconds.	
    /// </summary>
    [JsonIgnore]
    public long durationMillis { get; set; }
  }
  public class DriveItemMetadata_location
  {
    /// <summary>
    /// The latitude stored in the image.	
    /// </summary>
    [JsonIgnore]
    public double latitude { get; set; }

    /// <summary>
    /// The longitude stored in the image.	
    /// </summary>
    [JsonIgnore]
    public double longitude { get; set; }

    /// <summary>
    /// The altitude stored in the image.	
    /// </summary>
    [JsonIgnore]
    public double altitude { get; set; }
  }
  //public class key
  //{

  //}
  public class Drive_User
  {
    /// <summary>
    /// This is always drive#user.	
    /// </summary>
    [JsonIgnore]
    public string kind { get; set; }

    /// <summary>
    /// A plain text displayable name for this user.	
    /// </summary>
    [JsonIgnore]
    public string displayName { get; set; }

    /// <summary>
    /// The user's profile picture.	
    /// </summary>
    [JsonIgnore]
    public DriveItemMetadata_picture picture { get; set; }

    /// <summary>
    /// Whether this user is the same as the authenticated user for whom the request was made.	
    /// </summary>
    [JsonIgnore]
    public bool isAuthenticatedUser { get; set; }

    /// <summary>
    /// The user's ID as visible in the permissions collection.	
    /// </summary>
    [JsonIgnore]
    public string permissionId { get; set; }

    /// <summary>
    /// The email address of the user.	
    /// </summary>
    [JsonIgnore]
    public string emailAddress { get; set; }
  }
  #endregion

  //#region MetaDataInterFace
  //public interface IDriveItemMetadata_Item
  //{
  //  string kind { get; }
  //  string id { get; set; }
  //  string etag { get; }
  //  string selfLink { get; }
  //  string webContentLink { get; }
  //  string webViewLink { get; }
  //  string alternateLink { get; }
  //  string embedLink { get; }
  //  //openWithLinks { (key): string }
  //  string defaultOpenWithLink { get; }
  //  string iconLink { get; }
  //  bool hasThumbnail { get; }
  //  string thumbnailLink { get; }
  //  long thumbnailVersion { get; }
  //  DriveItemMetadata_thumbnail thumbnail { get; }
  //  string title { get; set; }
  //  string mimeType { get; set; }
  //  string description { get; set; }
  //  DriveItemMetadata_label labels { get; set; }
  //  DateTime createdDate { get; set; }
  //  DateTime modifiedDate { get; set; }
  //  DateTime modifiedByMeDate { get; set; }
  //  DateTime lastViewedByMeDate { get; set; }
  //  DateTime markedViewedByMeDate { get; set; }
  //  DateTime sharedWithMeDate { get; set; }
  //  long version { get; set; }
  //  Drive_User sharingUser { get; set; }
  //  List<DriveItemMetadata_parent> parents { get; set; }
  //  string downloadUrl { get; set; }
  //  //exportLinks { (key): string }
  //  DriveItemMetadata_indexableText indexableText { get; set; }
  //  DriveItemMetadata_permission userPermission { get; set; }
  //  List<DriveItemMetadata_permission> permissions { get; set; }
  //  bool hasAugmentedPermissions { get; set; }
  //  string originalFilename { get; set; }
  //  string fileExtension { get; set; }
  //  string fullFileExtension { get; set; }
  //  string md5Checksum { get; set; }
  //  long fileSize { get; set; } = -1;
  //  long quotaBytesUsed { get; set; }
  //  List<string> ownerNames { get; set; }
  //  List<Drive_User> owners { get; set; }
  //  string teamDriveId { get; set; }
  //  string lastModifyingUserName { get; set; }
  //  Drive_User lastModifyingUser { get; set; }
  //  bool ownedByMe { get; set; }
  //  DriveItemMetadata_capability capabilities { get; set; }
  //  bool editable { get; set; }
  //  bool canComment { get; set; }
  //  bool canReadRevisions { get; set; }
  //  bool shareable { get; set; }
  //  bool copyable { get; set; }
  //  bool writersCanShare { get; set; }
  //  bool shared { get; set; }
  //  bool explicitlyTrashed { get; set; }
  //  Drive_User trashingUser { get; set; }
  //  DateTime trashedDate { get; set; }
  //  bool appDataContents { get; set; }
  //  string headRevisionId { get; set; }
  //  List<DriveItemMetadata_property> properties { get; set; }
  //  string folderColorRgb { get; set; }
  //  DriveItemMetadata_imageMediaMetadata imageMediaMetadata { get; set; }
  //  DriveItemMetadata_videoMediaMetadata videoMediaMetadata { get; set; }
  //  List<string> spaces { get; set; }
  //  bool isAppAuthorized { get; set; }
  //}
  //#endregion
}
