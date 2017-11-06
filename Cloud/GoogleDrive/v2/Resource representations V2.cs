using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Cloud;
using System.Linq;
namespace Cloud.GoogleDrive
{
  public class Drive2_Files_list
  {
    /// <summary>
    /// This is always drive#fileList.	
    /// </summary>
    public string kind { get; internal set; }
    /// <summary>
    /// The ETag of the list.	
    /// </summary>
    public string etag { get; internal set; }
    /// <summary>
    /// A link back to this list.	
    /// </summary>
    public string selfLink { get;}
    /// <summary>
    /// The page token for the next page of files. This will be absent if the end of the files list has been reached. If the token is rejected for any reason, it should be discarded, and pagination should be restarted from the first page of results.	
    /// </summary>
    public string nextPageToken { get; internal set; }
    /// <summary>
    /// A link to the next page of files.	
    /// </summary>
    public string nextLink { get; internal set; }
    /// <summary>
    /// Whether the search process was incomplete. If true, then some search results may be missing, since all documents were not searched. This may occur when searching multiple Team Drives with the "default,allTeamDrives" corpora, but all corpora could not be searched. When this happens, it is suggested that clients narrow their query by choosing a different corpus such as "default" or "teamDrive".	
    /// </summary>
    public bool? incompleteSearch { get; internal set; }
    /// <summary>
    /// The list of files. If nextPageToken is populated, then this list may be incomplete and an additional page of results should be fetched.	
    /// </summary>
    public List<Drive2_File> items { get; internal set; }
  }//readonly
  public class Drive2_File
  {
    //exportLinks { (key): string }
    //openWithLinks { (key): string }

    #region public get, internal set
    /// <summary>
    /// The type of file. This is always drive#file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string kind { get; internal set; }

    /// <summary>
    /// The ID of the file.
    /// </summary>
    [JsonIgnoreSerialize]
    public string id { get; internal set; }

    /// <summary>
    /// ETag of the file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string etag { get; internal set; }

    /// <summary>
    /// A link back to this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string selfLink { get; internal set; }

    /// <summary>
    /// A link for downloading the content of the file in a browser using cookie based authentication. In cases where the content is shared publicly, the content can be downloaded without any credentials.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string webContentLink { get; internal set; }

    /// <summary>
    /// A link only available on folders for viewing their static web assets (HTML, CSS, JS, etc) via Google Drive's Website Hosting.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string webViewLink { get; internal set; }

    /// <summary>
    /// A link for opening the file in a relevant Google editor or viewer.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string alternateLink { get; internal set; }

    /// <summary>
    /// A link for embedding the file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string embedLink { get; internal set; }

    /// <summary>
    /// A link to open this file with the user's default app for this file. Only populated when the drive.apps.readonly scope is used.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string defaultOpenWithLink { get; internal set; }

    /// <summary>
    /// A link to the file's icon.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string iconLink { get; internal set; }

    /// <summary>
    /// Whether this file has a thumbnail. This does not indicate whether the requesting app has access to the thumbnail. To check access, look for the presence of the thumbnailLink field.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? hasThumbnail { get; internal set; }

    /// <summary>
    /// A short-lived link to the file's thumbnail. Typically lasts on the order of hours. Only populated when the requesting app can access the file's content.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string thumbnailLink { get; internal set; }

    /// <summary>
    /// The thumbnail version for use in thumbnail cache invalidation.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? thumbnailVersion { get; internal set; }
    
    /// <summary>
    /// A thumbnail for the file. This will only be used if Drive cannot generate a standard thumbnail.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_File_thumbnail thumbnail { get; internal set; }

    /// <summary>
    /// Create time for this file (formatted RFC 3339 timestamp).	
    /// </summary>
    [JsonIgnoreSerialize]
    public DateTime? createdDate { get; internal set; }

    /// <summary>
    /// Last time this file was modified by the user (formatted RFC 3339 timestamp). Note that setting modifiedDate will also update the modifiedByMe date for the user which set the date.	
    /// </summary>
    [JsonIgnoreSerialize]
    public DateTime? modifiedByMeDate { get; internal set; }

    /// <summary>
    /// Time at which this file was shared with the user (formatted RFC 3339 timestamp).	
    /// </summary>
    [JsonIgnoreSerialize]
    public DateTime? sharedWithMeDate { get; internal set; }

    /// <summary>
    /// A monotonically increasing version number for the file. This reflects every change made to the file on the server, even those not visible to the requesting user.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? version { get; internal set; }

    /// <summary>
    /// User that shared the item with the current user, if available.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_User sharingUser { get; internal set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonIgnoreSerialize]
    public string downloadUrl { get; internal set; }

    /// <summary>
    /// Whether any users are granted file access directly on this file. This field is only populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? hasAugmentedPermissions { get; internal set; }

    /// <summary>
    /// The final component of fullFileExtension with trailing text that does not appear to be part of the extension removed. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string fileExtension { get; internal set; }

    /// <summary>
    /// The full file extension; extracted from the title. May contain multiple concatenated extensions, such as "tar.gz". Removing an extension from the title does not clear this field; however, changing the extension on the title does update this field. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string fullFileExtension { get; internal set; }

    /// <summary>
    /// An MD5 checksum for the content of this file. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string md5Checksum { get; internal set; }

    /// <summary>
    /// The size of the file in bytes. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? fileSize { get; internal set; }

    /// <summary>
    /// The number of quota bytes used by this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? quotaBytesUsed { get; internal set; }

    /// <summary>
    /// Name(s) of the owner(s) of this file. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string[] ownerNames { get; internal set; }

    /// <summary>
    /// The owner(s) of this file. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_User[] owners { get; internal set; }

    /// <summary>
    /// ID of the Team Drive the file resides in.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string teamDriveId { get; internal set; }

    /// <summary>
    /// Name of the last user to modify this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string lastModifyingUserName { get; internal set; }

    /// <summary>
    /// The last user to modify this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_User lastModifyingUser { get; internal set; }

    /// <summary>
    /// Whether the file is owned by the current user. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? ownedByMe { get; internal set; }

    /// <summary>
    /// Capabilities the current user has on this file. Each capability corresponds to a fine-grained action that a user may take.
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_File_capability capabilities { get; internal set; }

    /// <summary>
    /// Deprecated: use capabilities/canEdit.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? editable { get; internal set; }

    /// <summary>
    /// Deprecated: use capabilities/canComment.
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canComment { get; internal set; }

    /// <summary>
    /// Deprecated: use capabilities/canReadRevisions.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canReadRevisions { get; internal set; }

    /// <summary>
    /// Deprecated: use capabilities/canShare.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? shareable { get; internal set; }

    /// <summary>
    /// Deprecated: use capabilities/canCopy.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? copyable { get; internal set; }

    /// <summary>
    /// Whether the file has been shared. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? shared { get; internal set; }

    /// <summary>
    /// Whether this file has been explicitly trashed, as opposed to recursively trashed.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? explicitlyTrashed { get; internal set; }

    /// <summary>
    /// If the file has been explicitly trashed, the user who trashed it. Only populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_User trashingUser { get; internal set; }

    /// <summary>
    /// The time that the item was trashed (formatted RFC 3339 timestamp). Only populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public DateTime? trashedDate { get; internal set; }

    /// <summary>
    /// Whether this file is in the Application Data folder.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? appDataContents { get; internal set; }

    /// <summary>
    /// The ID of the file's head revision. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string headRevisionId { get; internal set; }

    /// <summary>
    /// Metadata about image media. This will only be present for image types, and its contents will depend on what can be parsed from the image content.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_File_imageMediaMetadata imageMediaMetadata { get; internal set; }

    /// <summary>
    /// Metadata about video media. This will only be present for video types.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_File_videoMediaMetadata videoMediaMetadata { get; internal set; }

    /// <summary>
    /// The list of spaces which contain the file. Supported values are 'drive', 'appDataFolder' and 'photos'.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string[] spaces { get; internal set; }

    /// <summary>
    /// Whether the file was created or opened by the requesting app.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? isAppAuthorized { get; internal set; }
    #endregion

    #region public get;set;
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
    public Drive2_File_label labels { get; set; }

    /// <summary>
    /// Last time this file was modified by anyone (formatted RFC 3339 timestamp). This is only mutable on update when the setModifiedDate parameter is set.	
    /// </summary>
    public DateTime? modifiedDate { get; set; }

    /// <summary>
    /// Last time this file was viewed by the user (formatted RFC 3339 timestamp).	
    /// </summary>
    public DateTime? lastViewedByMeDate { get; set; }

    /// <summary>
    /// Deprecated
    /// </summary>
    public DateTime? markedViewedByMeDate { get; set; }

    /// <summary>
    /// Collection of parent folders which contain this file. Setting this field will put the file in all of the provided folders.On insert, if no folders are provided, the file will be placed in the default root folder.
    /// </summary>
    public List<Drive2_Parent> parents { get; set; }

    /// <summary>
    /// Indexable text attributes for the file. This property can only be written, and is not returned by files.get. For more information, see https://developers.google.com/drive/v3/web/practices#custom_thumbnails_and_indexable_text
    /// </summary>
    public Drive2_File_indexableText indexableText { get; set; }

    /// <summary>
    /// The permissions for the authenticated user on this file.	
    /// </summary>
    public Drive2_Permission userPermission { get; set; }

    /// <summary>
    /// The list of permissions for users with access to this file. Not populated for Team Drive files.	
    /// </summary>
    public Drive2_Permission[] permissions { get; set; }

    /// <summary>
    /// The original filename of the uploaded content if available, or else the original value of the title field. This is only available for files with binary content in Drive.	
    /// </summary>
    public string originalFilename { get; set; }

    /// <summary>
    /// Whether writers can share the document with other users. Not populated for Team Drive files.	
    /// </summary>
    public bool? writersCanShare { get; set; }

    /// <summary>
    /// The list of properties.	
    /// </summary>
    public Drive2_File_property[] properties { get; set; }

    /// <summary>
    /// Folder color as an RGB hex string if the file is a folder. The list of supported colors is available in the folderColorPalette field of the About resource. If an unsupported color is specified, it will be changed to the closest color in the palette. Not populated for Team Drive files.	
    /// </summary>
    public string folderColorRgb { get; set; }
    #endregion
  }
  public class Drive2_File_label
  {
    /// <summary>
    /// Whether this file is starred by the user.	
    /// </summary>
    public bool? starred { get; set; }

    /// <summary>
    /// Deprecated
    /// </summary>
    public bool? hidden { get; set; }

    /// <summary>
    /// Whether this file has been trashed. This label applies to all users accessing the file; however, only owners are allowed to see and untrash files.	
    /// </summary>
    public bool? trashed { get; set; }

    /// <summary>
    /// Whether viewers and commenters are prevented from downloading, printing, and copying this file.	
    /// </summary>
    public bool? restricted { get; set; }

    /// <summary>
    /// Whether this file has been viewed by this user.	
    /// </summary>
    public bool? viewed { get; set; }

    /// <summary>
    /// Whether the file has been modified by this user.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? modified { get; internal set; }
  }//writeable
  public class Drive2_File_indexableText
  {
    /// <summary>
    /// The text to be indexed for this file.	
    /// </summary>
    public string text { get; set; }
  }//writeable
  public class Drive2_File_thumbnail
  {
    /// <summary>
    /// The URL-safe Base64 encoded bytes of the thumbnail image. It should conform to RFC 4648 section 5.	
    /// </summary>
    [JsonIgnoreSerialize]
    public byte[] image { get; internal set; }
    /// <summary>
    /// The MIME type of the thumbnail.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string mimeType { get; internal set; }
  }//readonly  
  public class Drive2_File_capability
  {
    /// <summary>
    /// Whether the current user can add children to this folder. This is always false when the item is not a folder.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canAddChildren { get; internal set; }

    /// <summary>
    /// Whether the current user can change the restricted download label of this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canChangeRestrictedDownload { get; internal set; }

    /// <summary>
    /// Whether the current user can comment on this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canComment { get; internal set; }

    /// <summary>
    /// Whether the current user can copy this file. For a Team Drive item, whether the current user can copy non-folder descendants of this item, or this item itself if it is not a folder.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canCopy { get; internal set; }

    /// <summary>
    /// Whether the current user can delete this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canDelete { get; internal set; }

    /// <summary>
    /// Whether the current user can download this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canDownload { get; internal set; }

    /// <summary>
    /// Whether the current user can edit this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canEdit { get; internal set; }

    /// <summary>
    /// Whether the current user can list the children of this folder. This is always false when the item is not a folder.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canListChildren { get; internal set; }

    /// <summary>
    /// Whether the current user can move this item into a Team Drive. If the item is in a Team Drive, this field is equivalent to canMoveTeamDriveItem.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canMoveItemIntoTeamDrive { get; internal set; }

    /// <summary>
    /// Whether the current user can move this Team Drive item by changing its parent. Note that a request to change the parent for this item may still fail depending on the new parent that is being added. Only populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canMoveTeamDriveItem { get; internal set; }

    /// <summary>
    /// Whether the current user can read the revisions resource of this file. For a Team Drive item, whether revisions of non-folder descendants of this item, or this item itself if it is not a folder, can be read.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canReadRevisions { get; internal set; }

    /// <summary>
    /// Whether the current user can read the Team Drive to which this file belongs. Only populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canReadTeamDrive { get; internal set; }

    /// <summary>
    /// Whether the current user can remove children from this folder. This is always false when the item is not a folder.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canRemoveChildren { get; internal set; }

    /// <summary>
    /// Whether the current user can rename this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canRename { get; internal set; }

    /// <summary>
    /// Whether the current user can modify the sharing settings for this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canShare { get; internal set; }

    /// <summary>
    /// Whether the current user can move this file to trash.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canTrash { get; internal set; }

    /// <summary>
    /// Whether the current user can restore this file from trash.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canUntrash { get; internal set; }
  }//readonly
  public class Drive2_File_property
  {
    /// <summary>
    /// This is always drive#property.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string kind { get; internal set; }

    /// <summary>
    /// ETag of the property.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string etag { get; internal set; }

    /// <summary>
    /// The link back to this property.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string selfLink { get; internal set; }

    /// <summary>
    /// The key of this property.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string key { get; internal set; }

    /// <summary>
    /// The visibility of this property.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string visibility { get; internal set; }

    /// <summary>
    /// The value of this property.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string value { get; internal set; }
  }//read-only
  public class Drive2_File_imageMediaMetadata
  {
    /// <summary>
    /// The width of the image in pixels.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? width { get; internal set; }

    /// <summary>
    /// The height of the image in pixels.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? height { get; internal set; }

    /// <summary>
    /// The rotation in clockwise degrees from the image's original orientation.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? rotation { get; internal set; }

    /// <summary>
    /// Geographic location information stored in the image.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_File_location location { get; internal set; }

    /// <summary>
    /// The date and time the photo was taken (EXIF format timestamp).	
    /// </summary>
    [JsonIgnoreSerialize]
    public string date { get; internal set; }

    /// <summary>
    /// The make of the camera used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string cameraMake { get; internal set; }

    /// <summary>
    /// The model of the camera used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string cameraModel { get; internal set; }

    /// <summary>
    /// The length of the exposure, in seconds.	
    /// </summary>
    [JsonIgnoreSerialize]
    public float? exposureTime { get; internal set; }

    /// <summary>
    /// The aperture used to create the photo (f-number).	
    /// </summary>
    [JsonIgnoreSerialize]
    public float? aperture { get; internal set; }

    /// <summary>
    /// Whether a flash was used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? flashUsed { get; internal set; }

    /// <summary>
    /// The focal length used to create the photo, in millimeters.	
    /// </summary>
    [JsonIgnoreSerialize]
    public float? focalLength { get; internal set; }

    /// <summary>
    /// The ISO speed used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? isoSpeed { get; internal set; }

    /// <summary>
    /// The metering mode used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string meteringMode { get; internal set; }

    /// <summary>
    /// The type of sensor used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string sensor { get; internal set; }

    /// <summary>
    /// The exposure mode used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string exposureMode { get; internal set; }

    /// <summary>
    /// The color space of the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string colorSpace { get; internal set; }

    /// <summary>
    /// The white balance mode used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string whiteBalance { get; internal set; }

    /// <summary>
    /// The exposure bias of the photo (APEX value).	
    /// </summary>
    [JsonIgnoreSerialize]
    public float? exposureBias { get; internal set; }

    /// <summary>
    /// The smallest f-number of the lens at the focal length used to create the photo (APEX value).	
    /// </summary>
    [JsonIgnoreSerialize]
    public float? maxApertureValue { get; internal set; }

    /// <summary>
    /// The distance to the subject of the photo, in meters.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? subjectDistance { get; internal set; }

    /// <summary>
    /// The lens used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string lens { get; internal set; }
  }//read-only
  public class Drive2_File_videoMediaMetadata
  {
    /// <summary>
    /// The width of the video in pixels.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? width { get; internal set; }

    /// <summary>
    /// The height of the video in pixels.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? height { get; internal set; }

    /// <summary>
    /// The duration of the video in milliseconds.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? durationMillis { get; internal set; }
  }//read-only
  public class Drive2_File_location
  {
    /// <summary>
    /// The latitude stored in the image.	
    /// </summary>
    [JsonIgnoreSerialize]
    public double? latitude { get; internal set; }

    /// <summary>
    /// The longitude stored in the image.	
    /// </summary>
    [JsonIgnoreSerialize]
    public double? longitude { get; internal set; }

    /// <summary>
    /// The altitude stored in the image.	
    /// </summary>
    [JsonIgnoreSerialize]
    public double? altitude { get; internal set; }
  }//read-only

  public class Drive2_About
  {
    /// <summary>
    /// This is always drive#about.	
    /// </summary>
    public string kind { get; internal set; }

    /// <summary>
    /// The ETag of the item.	
    /// </summary>
    public string etag { get; internal set; }

    /// <summary>
    /// A link back to this item.	
    /// </summary>
    public string selfLink { get; internal set; }

    /// <summary>
    /// The name of the current user.	
    /// </summary>
    public string name { get; internal set; }

    /// <summary>
    /// The authenticated user.	
    /// </summary>
    public Drive2_User user { get; internal set; }

    /// <summary>
    /// The total number of quota bytes.	
    /// </summary>
    public long? quotaBytesTotal { get; internal set; }

    /// <summary>
    /// The number of quota bytes used by Google Drive.	
    /// </summary>
    public long? quotaBytesUsed { get; internal set; }

    /// <summary>
    /// The number of quota bytes used by all Google apps (Drive, Picasa, etc.).	
    /// </summary>
    public long? quotaBytesUsedAggregate { get; internal set; }

    /// <summary>
    /// The number of quota bytes used by trashed items.	
    /// </summary>
    public long? quotaBytesUsedInTrash { get; internal set; }

    /// <summary>
    /// The type of the user's storage quota.
    /// </summary>
    public Drive2_About_quotaType quotaType { get; internal set; }

    public Drive2_About_quotaBytesByService[] quotaBytesByService { get; internal set; }

    /// <summary>
    /// The largest change id.	
    /// </summary>
    public long? largestChangeId { get; internal set; }

    /// <summary>
    /// The number of remaining change ids, limited to no more than 2500.	
    /// </summary>
    public long? remainingChangeIds { get; internal set; }

    /// <summary>
    /// The id of the root folder.	
    /// </summary>
    public string rootFolderId { get; internal set; }

    /// <summary>
    /// The domain sharing policy for the current user.
    /// </summary>
    public Drive2_About_domainSharingPolicy domainSharingPolicy { get; internal set; }

    /// <summary>
    /// The current user's ID as visible in the permissions collection.	
    /// </summary>
    public string permissionId { get; internal set; }

    /// <summary>
    /// The allowable import formats.	
    /// </summary>
    public Drive2_About_Format[] importFormats { get; internal set; }

    /// <summary>
    /// The allowable export formats.	
    /// </summary>
    public Drive2_About_Format[] exportFormats { get; internal set; }

    /// <summary>
    /// Information about supported additional roles per file type. The most specific type takes precedence.	
    /// </summary>
    public Drive2_About_additionalRoleInfo[] additionalRoleInfo { get; internal set; }

    /// <summary>
    /// List of additional features enabled on this account.	
    /// </summary>
    public Drive2_About_feature[] features { get; internal set; }

    /// <summary>
    /// List of max upload sizes for each file type. The most specific type takes precedence.	
    /// </summary>
    public Drive2_About_maxUploadSize[] maxUploadSizes { get; internal set; }

    /// <summary>
    /// A boolean indicating whether the authenticated app is installed by the authenticated user.	
    /// </summary>
    public bool? isCurrentAppInstalled { get; internal set; }

    /// <summary>
    /// The user's language or locale code, as defined by BCP 47, with some extensions from Unicode's LDML format (http://www.unicode.org/reports/tr35/).	
    /// </summary>
    public string languageCode { get; internal set; }

    /// <summary>
    /// The palette of allowable folder colors as RGB hex strings.	
    /// </summary>
    public string[] folderColorPalette { get; internal set; }

    /// <summary>
    /// A list of themes that are supported for Team Drives.	
    /// </summary>
    public Drive2_About_teamDriveTheme[] teamDriveThemes { get; internal set; }
  }//read-only
  public enum Drive2_About_quotaType
  {
    LIMITED,
    UNLIMITED
  }
  public enum Drive2_About_domainSharingPolicy
  {
    allowed,
    allowedWithWarning,
    incomingOnly,
    disallowed
  }
  public class Drive2_About_quotaBytesByService
  {
    /// <summary>
    /// The service's name, e.g. DRIVE, GMAIL, or PHOTOS.	
    /// </summary>
    public string serviceName { get; internal set; }

    /// <summary>
    /// The storage quota bytes used by the service.	
    /// </summary>
    public long? bytesUsed { get; internal set; }
  }
  public class Drive2_About_Format
  {
    /// <summary>
    /// The imported file's content type to convert from.	
    /// </summary>
    public string source { get; internal set; }

    /// <summary>
    /// The possible content types to convert to.	
    /// </summary>
    public string[] targets { get; internal set; }
  }
  public class Drive2_About_additionalRoleInfo
  {
    /// <summary>
    /// The content type that this additional role info applies to.	
    /// </summary>
    public string type { get; internal set; }

    /// <summary>
    /// The supported additional roles per primary role.	
    /// </summary>
    public Drive2_About_roleSet[] roleSets { get; internal set; }
  }
  public class Drive2_About_roleSet
  {
    /// <summary>
    /// A primary permission role.	
    /// </summary>
    public string primaryRole { get; internal set; }

    /// <summary>
    /// The supported additional roles with the primary role.	
    /// </summary>
    public string[] additionalRoles { get; internal set; }
  }
  public class Drive2_About_feature
  {
    /// <summary>
    /// The name of the feature.	
    /// </summary>
    public string featureName { get; internal set; }

    /// <summary>
    /// The request limit rate for this feature, in queries per second.	
    /// </summary>
    public double? featureRate { get; internal set; }
  }
  public class Drive2_About_maxUploadSize
  {
    /// <summary>
    /// The file type.	
    /// </summary>
    public string type { get; internal set; }

    /// <summary>
    /// The max upload size for this type.	
    /// </summary>
    public long? size { get; internal set; }
  }
  public class Drive2_About_teamDriveTheme
  {
    /// <summary>
    /// The ID of the theme.	
    /// </summary>
    public string id { get; internal set; }

    /// <summary>
    /// A link to this Team Drive theme's background image.	
    /// </summary>
    public string backgroundImageLink { get; internal set; }

    /// <summary>
    /// The color of this Team Drive theme as an RGB hex string.	
    /// </summary>
    public string colorRgb { get; internal set; }
  }
  
  public class Drive2_Change
  {
    /// <summary>
    /// This is always drive#change.	
    /// </summary>
    public string kind { get; internal set; }

    /// <summary>
    /// The ID of the change.	
    /// </summary>
    public long? id { get; internal set; }

    /// <summary>
    /// The type of the change. Possible values are file and teamDrive.	
    /// </summary>
    public string type { get; internal set; }

    /// <summary>
    /// The time of this modification.	
    /// </summary>
    public DateTime? modificationDate { get; internal set; }

    /// <summary>
    /// Whether the file or Team Drive has been removed from this list of changes, for example by deletion or loss of access.	
    /// </summary>
    public bool? deleted { get; internal set; }

    /// <summary>
    /// A link back to this change.	
    /// </summary>
    public string selfLink { get; internal set; }

    /// <summary>
    /// The ID of the file associated with this change.	
    /// </summary>
    public string fileId { get; internal set; }

    /// <summary>
    /// The updated state of the file. Present if the type is file and the file has not been removed from this list of changes.	
    /// </summary>
    public Drive2_File file { get; internal set; }

    /// <summary>
    /// The ID of the Team Drive associated with this change.	
    /// </summary>
    public string teamDriveId { get; internal set; }

    ///
    ///The updated state of the Team Drive. Present if the type is teamDrive, the user is still a member of the Team Drive, and the Team Drive has not been deleted.	
    ///

    //public Drive2_TeamDrive teamDrive {get; internal set;}
  }//readonly

  public class Drive2_Children
  {
    /// <summary>
    /// This is always drive#childReference.	
    /// </summary>
    public string kind { get; internal set; }

    /// <summary>
    /// The ID of the child.	
    /// </summary>
    public string id { get; internal set; }

    /// <summary>
    /// A link back to this reference.	
    /// </summary>
    public string selfLink { get; internal set; }

    /// <summary>
    /// A link to the child.	
    /// </summary>
    public string childLink { get; internal set; }
  }//readonly
  
  public class Drive2_Parents_list
  {
    /// <summary>
    /// This is always drive#parentList.	
    /// </summary>
    public string kind { get; internal set; }

    /// <summary>
    /// The ETag of the list.	
    /// </summary>
    public string etag { get; internal set; }

    /// <summary>
    /// A link back to this list.	
    /// </summary>
    public string selfLink { get; internal set; }

    /// <summary>
    /// The list of parents.	
    /// </summary>
    public List<Drive2_Parent> items { get; internal set; }
  }//read-only
  public class Drive2_Parent
  {
    /// <summary>
    /// This is always drive#parent Reference.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string kind { get; internal set; }

    /// <summary>
    /// The ID of the parent.	
    /// </summary>
    public string id { get; set; }

    /// <summary>
    /// A link back to this reference.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string selfLink { get; internal set; }

    /// <summary>
    /// A link to the parent.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string parentLink { get; internal set; }

    /// <summary>
    /// Whether or not the parent is the root folder.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? isRoot { get; internal set; }
  }//isRoot { get; internal set; } [JsonIgnoreSerialize]

  public class Drive2_Permission
  {
    /// <summary>
    /// This is always drive#permission.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string kind { get; internal set; }

    /// <summary>
    /// The ETag of the permission.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string etag { get; internal set; }

    /// <summary>
    /// The ID of the user this permission refers to, and identical to the permissionId in the About and Files resources. When making a drive.permissions.insert request, exactly one of the id or value fields must be specified unless the permission type is anyone, in which case both id and value are ignored.	
    /// </summary>
    public string id { get; set; }

    /// <summary>
    /// A link back to this permission.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string selfLink { get; internal set; }

    /// <summary>
    /// The name for this permission.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string name { get; internal set; }

    /// <summary>
    /// The email address of the user or group this permission refers to. This is an output-only field which is present when the permission type is user or group.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string emailAddress { get; internal set; }

    /// <summary>
    /// The domain name of the entity this permission refers to. This is an output-only field which is present when the permission type is user, group or domain.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string domain { get; internal set; }

    /// <summary>
    /// The primary role for this user. While new values may be supported in the future
    /// </summary>
    public Drive2_Permission_PermissionRole role { get; set; }

    /// <summary>
    /// Additional roles for this user. Only commenter is currently allowed, though more may be supported in the future.	
    /// </summary>
    public string[] additionalRoles { get; set; }

    /// <summary>
    /// The account type
    /// </summary>
    public Drive2_Permission_type type { get; set; }

    /// <summary>
    /// The email address or domain name for the entity. This is used during inserts and is not populated in responses. When making a drive.permissions.insert request, exactly one of the id or value fields must be specified unless the permission type is anyone, in which case both id and value are ignored.	
    /// </summary>
    public string value { get; set; }

    /// <summary>
    /// The authkey parameter required for this permission.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string authKey { get; internal set; }

    /// <summary>
    /// Whether the link is required for this permission.	
    /// </summary>
    public bool? withLink { get; set; }

    /// <summary>
    /// A link to the profile photo, if available.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string photoLink { get; internal set; }

    /// <summary>
    /// The time at which this permission will expire (RFC 3339 date-time). Expiration dates have the following restrictions:
    /// They can only be set on user and group permissions
    /// The date must be in the future
    /// The date cannot be more than a year in the future
    /// The date can only be set on drive.permissions.update requests
    /// </summary>
    public DateTime? expirationDate { get; set; }

    /// <summary>
    /// Details of whether the permissions on this Team Drive item are inherited or directly on this item. This is an output-only field which is present only for Team Drive items.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_Permission_teamDrivePermissionDetail[] teamDrivePermissionDetails { get; internal set; }

    /// <summary>
    /// Whether the account associated with this permission has been deleted. This field only pertains to user and group permissions.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? deleted { get; internal set; }
  }//writeable  
  public enum Drive2_Permission_type
  {
    user,
    group,
    domain,
    anyone
  }
  public enum Drive2_Permission_TeamPermissionType
  {
    file,
    member
  }
  public enum Drive2_Permission_PermissionRole
  {
    organizer,
    owner,
    reader,
    writer
  }
  public class Drive2_Permission_teamDrivePermissionDetail
  {
    /// <summary>
    /// The Team Drive permission type for this user. While new values may be added in future
    /// </summary>
    public Drive2_Permission_TeamPermissionType teamDrivePermissionType { get; internal set; }

    /// <summary>
    /// The primary role for this user. While new values may be added in the future
    /// </summary>
    public Drive2_Permission_PermissionRole role { get; internal set; }

    /// <summary>
    /// Additional roles for this user. Only commenter is currently possible, though more may be supported in the future.	
    /// </summary>
    public string[] additionalRoles { get; internal set; }

    /// <summary>
    /// The ID of the item from which this permission is inherited. This is an output-only field and is only populated for members of the Team Drive.	
    /// </summary>
    public string inheritedFrom { get; internal set; }

    /// <summary>
    /// Whether this permission is inherited. This field is always populated. This is an output-only field.	
    /// </summary>
    public bool? inherited { get; internal set; }
  }//read-only
  
  public class Drive2_Revision
  {
    /// <summary>
    /// This is always drive#revision.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string kind { get; internal set; }

    /// <summary>
    /// The ETag of the revision.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string etag { get; internal set; }

    /// <summary>
    /// The ID of the revision.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string id { get; internal set; }

    /// <summary>
    /// A link back to this revision.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string selfLink { get; internal set; }

    /// <summary>
    /// The MIME type of the revision.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string mimeType { get; internal set; }

    /// <summary>
    /// Last time this revision was modified (formatted RFC 3339 timestamp).	
    /// </summary>
    [JsonIgnoreSerialize]
    public DateTime modifiedDate { get; internal set; }

    /// <summary>
    /// Whether this revision is pinned to prevent automatic purging. This will only be populated and can only be modified on files with content stored in Drive which are not Google Docs. Revisions can also be pinned when they are created through the drive.files.insert/update/copy by using the pinned query parameter.	
    /// </summary>
    public bool pinned { get; set; }

    /// <summary>
    /// Whether this revision is published. This is only populated and can only be modified for Google Docs.	
    /// </summary>
    public bool published { get; set; }

    /// <summary>
    /// A link to the published revision.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string publishedLink { get; internal set; }

    /// <summary>
    /// Whether subsequent revisions will be automatically republished. This is only populated and can only be modified for Google Docs.	
    /// </summary>
    public bool publishAuto { get; set; }

    /// <summary>
    /// Whether this revision is published outside the domain. This is only populated and can only be modified for Google Docs.	
    /// </summary>
    public bool publishedOutsideDomain { get; set; }

    /// <summary>
    /// Short term download URL for the file. This will only be populated on files with content stored in Drive.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string downloadUrl { get; internal set; }

    //"exportLinks": { (key): string}

    /// <summary>
    /// Name of the last user to modify this revision.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string lastModifyingUserName { get; internal set; }

    /// <summary>
    /// The last user to modify this revision.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_User lastModifyingUser { get; internal set; }

    /// <summary>
    /// The original filename when this revision was created. This will only be populated on files with content stored in Drive.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string originalFilename { get; internal set; }

    /// <summary>
    /// An MD5 checksum for the content of this revision. This will only be populated on files with content stored in Drive.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string md5Checksum { get; internal set; }

    /// <summary>
    /// The size of the revision in bytes. This will only be populated on files with content stored in Drive.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? fileSize { get; internal set; }
  }//writeable  

  public class Drive2_App
  {
    /// <summary>
    /// This is always drive#app.	
    /// </summary>
    public string kind { get; internal set; }

    /// <summary>
    /// The ID of the app.	
    /// </summary>
    public string id { get; internal set; }

    /// <summary>
    /// The name of the app.	
    /// </summary>
    public string name { get; internal set; }

    /// <summary>
    /// The type of object this app creates (e.g. Chart). If empty, the app name should be used instead.	
    /// </summary>
    public string objectType { get; internal set; }

    /// <summary>
    /// A short description of the app.	
    /// </summary>
    public string shortDescription { get; internal set; }

    /// <summary>
    /// A long description of the app.	
    /// </summary>
    public string longDescription { get; internal set; }

    /// <summary>
    /// Whether this app supports creating new objects.	
    /// </summary>
    public bool? supportsCreate { get; internal set; }

    /// <summary>
    /// Whether this app supports importing Google Docs.	
    /// </summary>
    public bool? supportsImport { get; internal set; }

    /// <summary>
    /// Whether this app supports opening more than one file.	
    /// </summary>
    public bool? supportsMultiOpen { get; internal set; }

    /// <summary>
    /// Whether this app supports creating new files when offline.	
    /// </summary>
    public bool? supportsOfflineCreate { get; internal set; }

    /// <summary>
    /// Whether the app is installed.	
    /// </summary>
    public bool? installed { get; internal set; }

    /// <summary>
    /// Whether the app is authorized to access data on the user's Drive.	
    /// </summary>
    public bool? authorized { get; internal set; }

    /// <summary>
    /// Whether the app has drive-wide scope. An app with drive-wide scope can access all files in the user's drive.	
    /// </summary>
    public bool? hasDriveWideScope { get; internal set; }

    /// <summary>
    /// Whether the app is selected as the default handler for the types it supports.	
    /// </summary>
    public bool? useByDefault { get; internal set; }

    /// <summary>
    /// A link to the product listing for this app.	
    /// </summary>
    public string productUrl { get; internal set; }

    /// <summary>
    /// The ID of the product listing for this app.	
    /// </summary>
    public string productId { get; internal set; }

    /// <summary>
    /// The template url for opening files with this app. The template will contain {ids} and/or {exportIds} to be replaced by the actual file ids. See Open Files for the full documentation.	(https://developers.google.com/drive/web/integrate-open)
    /// </summary>
    public string openUrlTemplate { get; internal set; }

    /// <summary>
    /// The url to create a new file with this app.	
    /// </summary>
    public string createUrl { get; internal set; }

    /// <summary>
    /// The template url to create a new file with this app in a given folder. The template will contain {folderId} to be replaced by the folder to create the new file in.	
    /// </summary>
    public string createInFolderTemplate { get; internal set; }

    /// <summary>
    /// The list of primary mime types.	
    /// </summary>
    public string[] primaryMimeTypes { get; internal set; }

    /// <summary>
    /// The list of secondary mime types.	
    /// </summary>
    public string[] secondaryMimeTypes { get; internal set; }

    /// <summary>
    /// The list of primary file extensions.	
    /// </summary>
    public string[] primaryFileExtensions { get; internal set; }

    /// <summary>
    /// The list of secondary file extensions.	
    /// </summary>
    public string[] secondaryFileExtensions { get; internal set; }

    /// <summary>
    /// The various icons for the app.	
    /// </summary>
    public Drive2_App_icon[] icons { get; internal set; }
  }//readonly
  public class Drive2_App_icon
  {
    /// <summary>
    /// Category of the icon.
    /// </summary>
    public Drive2_App_icon_category category { get; internal set; }

    /// <summary>
    /// Size of the icon. Represented as the maximum of the width and height.	
    /// </summary>
    public int? size { get; internal set; }

    /// <summary>
    /// URL for the icon.	
    /// </summary>
    public string iconUrl { get; internal set; }
  }//readonly
  public enum Drive2_App_icon_category
  {
    /// <summary>
    /// icon for the application
    /// </summary>
    application,
    /// <summary>
    /// icon for a file associated with the app
    /// </summary>
    document,
    /// <summary>
    /// icon for a shared file associated with the app
    /// </summary>
    documentShared
  }

  #region Comments
  #endregion

  #region Replies
  #endregion

  #region Properties
  #endregion

  #region Channels
  #endregion

  #region Realtime
  #endregion

  #region Teamdrives
  #endregion


  #region Other
  /// <summary>
  /// User metadata. This class is read-only
  /// </summary>
  public class Drive2_User
  {
    /// <summary>
    /// This is always drive#user.	
    /// </summary>
    public string kind { get; internal set; }

    /// <summary>
    /// A plain text displayable name for this user.	
    /// </summary>
    public string displayName { get; internal set; }

    /// <summary>
    /// The user's profile picture.	
    /// </summary>
    public Drive2_User_picture picture { get; internal set; }

    /// <summary>
    /// Whether this user is the same as the authenticated user for whom the request was made.	
    /// </summary>
    public bool? isAuthenticatedUser { get; internal set; }

    /// <summary>
    /// The user's ID as visible in the permissions collection.	
    /// </summary>
    public string permissionId { get; internal set; }

    /// <summary>
    /// The email address of the user.	
    /// </summary>
    public string emailAddress { get; internal set; }
  }//read-only
  public class Drive2_User_picture
  {
    /// <summary>
    /// A URL that points to a profile picture of this user.	
    /// </summary>
    public string url { get; internal set; }
  }//read-only
  #endregion
}
