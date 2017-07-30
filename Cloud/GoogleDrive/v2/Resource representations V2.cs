using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Cloud;

namespace Cloud.GoogleDrive
{
  #region Files
  public interface IDrive2_Files_list
  {
    /// <summary>
    /// This is always drive#fileList.	
    /// </summary>
    string kind { get; }
    /// <summary>
    /// The ETag of the list.	
    /// </summary>
    string etag { get; }
    /// <summary>
    /// A link back to this list.	
    /// </summary>
    string selfLink { get;}
    /// <summary>
    /// The page token for the next page of files. This will be absent if the end of the files list has been reached. If the token is rejected for any reason, it should be discarded, and pagination should be restarted from the first page of results.	
    /// </summary>
    string nextPageToken { get; }
    /// <summary>
    /// A link to the next page of files.	
    /// </summary>
    string nextLink { get; }
    /// <summary>
    /// Whether the search process was incomplete. If true, then some search results may be missing, since all documents were not searched. This may occur when searching multiple Team Drives with the "default,allTeamDrives" corpora, but all corpora could not be searched. When this happens, it is suggested that clients narrow their query by choosing a different corpus such as "default" or "teamDrive".	
    /// </summary>
    bool? incompleteSearch { get; }
    /// <summary>
    /// The list of files. If nextPageToken is populated, then this list may be incomplete and an additional page of results should be fetched.	
    /// </summary>
    IDrive2_File[] items { get; }
  }
  internal class Drive2_Files_list : IDrive2_Files_list
  {
    [JsonProperty("items")]
    Drive2_File[] items_;
    public string etag { get; set; }
    public bool? incompleteSearch { get; set; }
    [JsonIgnore]
    public IDrive2_File[] items { get { return items_; } }
    public string kind { get; set; }
    public string nextLink { get; set; }
    public string nextPageToken { get; set; }
    public string selfLink { get; set; }
  }

  public interface IDrive2_File
  {
    /// <summary>
    /// The type of file. This is always drive#file.	
    /// </summary>
    [JsonIgnoreSerialize]
    string kind { get; }

    /// <summary>
    /// The ID of the file.
    /// </summary>
    [JsonIgnoreSerialize]
    string id { get; }

    /// <summary>
    /// ETag of the file.	
    /// </summary>
    [JsonIgnoreSerialize]
    string etag { get; }

    /// <summary>
    /// A link back to this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    string selfLink { get; }

    /// <summary>
    /// A link for downloading the content of the file in a browser using cookie based authentication. In cases where the content is shared publicly, the content can be downloaded without any credentials.	
    /// </summary>
    [JsonIgnoreSerialize]
    string webContentLink { get; }

    /// <summary>
    /// A link only available on folders for viewing their static web assets (HTML, CSS, JS, etc) via Google Drive's Website Hosting.	
    /// </summary>
    [JsonIgnoreSerialize]
    string webViewLink { get; }

    /// <summary>
    /// A link for opening the file in a relevant Google editor or viewer.	
    /// </summary>
    [JsonIgnoreSerialize]
    string alternateLink { get; }

    /// <summary>
    /// A link for embedding the file.	
    /// </summary>
    [JsonIgnoreSerialize]
    string embedLink { get; }

    //openWithLinks { (key): string }

    /// <summary>
    /// A link to open this file with the user's default app for this file. Only populated when the drive.apps.readonly scope is used.	
    /// </summary>
    [JsonIgnoreSerialize]
    string defaultOpenWithLink { get; }

    /// <summary>
    /// A link to the file's icon.	
    /// </summary>
    [JsonIgnoreSerialize]
    string iconLink { get; }

    /// <summary>
    /// Whether this file has a thumbnail. This does not indicate whether the requesting app has access to the thumbnail. To check access, look for the presence of the thumbnailLink field.	
    /// </summary>
    [JsonIgnoreSerialize]
    bool? hasThumbnail { get; }

    /// <summary>
    /// A short-lived link to the file's thumbnail. Typically lasts on the order of hours. Only populated when the requesting app can access the file's content.	
    /// </summary>
    [JsonIgnoreSerialize]
    string thumbnailLink { get; }

    /// <summary>
    /// The thumbnail version for use in thumbnail cache invalidation.	
    /// </summary>
    [JsonIgnoreSerialize]
    long? thumbnailVersion { get; }
    
    /// <summary>
    /// A thumbnail for the file. This will only be used if Drive cannot generate a standard thumbnail.	
    /// </summary>
    [JsonIgnoreSerialize]
    IDrive2_File_thumbnail thumbnail { get; }

    /// <summary>
    /// The title of this file. Note that for immutable items such as the top level folders of Team Drives, My Drive root folder, and Application Data folder the title is constant.	
    /// </summary>
    string title { get; set; }

    /// <summary>
    /// The MIME type of the file. This is only mutable on update when uploading new content. This field can be left blank, and the mimetype will be determined from the uploaded content's MIME type.	
    /// </summary>
    string mimeType { get; set; }

    /// <summary>
    /// A short description of the file.	
    /// </summary>
    string description { get; set; }

    /// <summary>
    /// A group of labels for the file.	
    /// </summary>
    Drive2_File_label labels { get; set; }

    /// <summary>
    /// Create time for this file (formatted RFC 3339 timestamp).	
    /// </summary>
    [JsonIgnoreSerialize]
    DateTime? createdDate { get; }

    /// <summary>
    /// Last time this file was modified by anyone (formatted RFC 3339 timestamp). This is only mutable on update when the setModifiedDate parameter is set.	
    /// </summary>
    DateTime? modifiedDate { get; set; }

    /// <summary>
    /// Last time this file was modified by the user (formatted RFC 3339 timestamp). Note that setting modifiedDate will also update the modifiedByMe date for the user which set the date.	
    /// </summary>
    [JsonIgnoreSerialize]
    DateTime? modifiedByMeDate { get; }

    /// <summary>
    /// Last time this file was viewed by the user (formatted RFC 3339 timestamp).	
    /// </summary>
    DateTime? lastViewedByMeDate { get; set; }

    /// <summary>
    /// Deprecated
    /// </summary>
    DateTime? markedViewedByMeDate { get; set; }

    /// <summary>
    /// Time at which this file was shared with the user (formatted RFC 3339 timestamp).	
    /// </summary>
    [JsonIgnoreSerialize]
    DateTime? sharedWithMeDate { get; }

    /// <summary>
    /// A monotonically increasing version number for the file. This reflects every change made to the file on the server, even those not visible to the requesting user.	
    /// </summary>
    [JsonIgnoreSerialize]
    long? version { get; }

    /// <summary>
    /// User that shared the item with the current user, if available.	
    /// </summary>
    [JsonIgnoreSerialize]
    Drive2_User sharingUser { get; }

    /// <summary>
    /// Collection of parent folders which contain this file. Setting this field will put the file in all of the provided folders.On insert, if no folders are provided, the file will be placed in the default root folder.
    /// </summary>
    List<Drive2_Parent> parents { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonIgnoreSerialize]
    string downloadUrl { get; }

    //exportLinks { (key): string }

    /// <summary>
    /// Indexable text attributes for the file. This property can only be written, and is not returned by files.get. For more information, see https://developers.google.com/drive/v3/web/practices#custom_thumbnails_and_indexable_text
    /// </summary>
    Drive2_File_indexableText indexableText { get; set; }

    /// <summary>
    /// The permissions for the authenticated user on this file.	
    /// </summary>
    Drive2_Permission userPermission { get; set; }

    /// <summary>
    /// The list of permissions for users with access to this file. Not populated for Team Drive files.	
    /// </summary>
    List<Drive2_Permission> permissions { get; set; }

    /// <summary>
    /// Whether any users are granted file access directly on this file. This field is only populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    bool? hasAugmentedPermissions { get; }

    /// <summary>
    /// The original filename of the uploaded content if available, or else the original value of the title field. This is only available for files with binary content in Drive.	
    /// </summary>
    string originalFilename { get; set; }

    /// <summary>
    /// The final component of fullFileExtension with trailing text that does not appear to be part of the extension removed. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnoreSerialize]
    string fileExtension { get; }

    /// <summary>
    /// The full file extension; extracted from the title. May contain multiple concatenated extensions, such as "tar.gz". Removing an extension from the title does not clear this field; however, changing the extension on the title does update this field. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnoreSerialize]
    string fullFileExtension { get; }

    /// <summary>
    /// An MD5 checksum for the content of this file. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnoreSerialize]
    string md5Checksum { get; }

    /// <summary>
    /// The size of the file in bytes. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnoreSerialize]
    long? fileSize { get; }

    /// <summary>
    /// The number of quota bytes used by this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    long? quotaBytesUsed { get; }

    /// <summary>
    /// Name(s) of the owner(s) of this file. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    List<string> ownerNames { get; }

    /// <summary>
    /// The owner(s) of this file. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    List<Drive2_User> owners { get; }

    /// <summary>
    /// ID of the Team Drive the file resides in.	
    /// </summary>
    [JsonIgnoreSerialize]
    string teamDriveId { get; }

    /// <summary>
    /// Name of the last user to modify this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    string lastModifyingUserName { get; }

    /// <summary>
    /// The last user to modify this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    Drive2_User lastModifyingUser { get; }

    /// <summary>
    /// Whether the file is owned by the current user. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    bool? ownedByMe { get; }

    /// <summary>
    /// Capabilities the current user has on this file. Each capability corresponds to a fine-grained action that a user may take.
    /// </summary>
    [JsonIgnoreSerialize]
    Drive2_File_capability capabilities { get; }

    /// <summary>
    /// Deprecated: use capabilities/canEdit.	
    /// </summary>
    [JsonIgnoreSerialize]
    bool? editable { get; }

    /// <summary>
    /// Deprecated: use capabilities/canComment.
    /// </summary>
    [JsonIgnoreSerialize]
    bool? canComment { get; }

    /// <summary>
    /// Deprecated: use capabilities/canReadRevisions.	
    /// </summary>
    [JsonIgnoreSerialize]
    bool? canReadRevisions { get; }

    /// <summary>
    /// Deprecated: use capabilities/canShare.	
    /// </summary>
    [JsonIgnoreSerialize]
    bool? shareable { get; }

    /// <summary>
    /// Deprecated: use capabilities/canCopy.	
    /// </summary>
    [JsonIgnoreSerialize]
    bool? copyable { get; }

    /// <summary>
    /// Whether writers can share the document with other users. Not populated for Team Drive files.	
    /// </summary>
    bool? writersCanShare { get; set; }

    /// <summary>
    /// Whether the file has been shared. Not populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    bool? shared { get; }

    /// <summary>
    /// Whether this file has been explicitly trashed, as opposed to recursively trashed.	
    /// </summary>
    [JsonIgnoreSerialize]
    bool? explicitlyTrashed { get; }

    /// <summary>
    /// If the file has been explicitly trashed, the user who trashed it. Only populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    Drive2_User trashingUser { get; }

    /// <summary>
    /// The time that the item was trashed (formatted RFC 3339 timestamp). Only populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    DateTime? trashedDate { get; }

    /// <summary>
    /// Whether this file is in the Application Data folder.	
    /// </summary>
    [JsonIgnoreSerialize]
    bool? appDataContents { get; }

    /// <summary>
    /// The ID of the file's head revision. This field is only populated for files with content stored in Drive; it is not populated for Google Docs or shortcut files.	
    /// </summary>
    [JsonIgnoreSerialize]
    string headRevisionId { get; }

    /// <summary>
    /// The list of properties.	
    /// </summary>
    List<Drive2_File_property> properties { get; set; }

    /// <summary>
    /// Folder color as an RGB hex string if the file is a folder. The list of supported colors is available in the folderColorPalette field of the About resource. If an unsupported color is specified, it will be changed to the closest color in the palette. Not populated for Team Drive files.	
    /// </summary>
    string folderColorRgb { get; set; }

    /// <summary>
    /// Metadata about image media. This will only be present for image types, and its contents will depend on what can be parsed from the image content.	
    /// </summary>
    [JsonIgnoreSerialize]
    Drive2_File_imageMediaMetadata imageMediaMetadata { get; }

    /// <summary>
    /// Metadata about video media. This will only be present for video types.	
    /// </summary>
    [JsonIgnoreSerialize]
    Drive2_File_videoMediaMetadata videoMediaMetadata { get; }

    /// <summary>
    /// The list of spaces which contain the file. Supported values are 'drive', 'appDataFolder' and 'photos'.	
    /// </summary>
    [JsonIgnoreSerialize]
    List<string> spaces { get; }

    /// <summary>
    /// Whether the file was created or opened by the requesting app.	
    /// </summary>
    [JsonIgnoreSerialize]
    bool? isAppAuthorized { get; }
  }
  internal class Drive2_File: IDrive2_File//for deserialize
  {
    public override string ToString()
    {
      return title;
    }
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
    public bool? hasThumbnail { get; set; }
    public string thumbnailLink { get; set; }
    public long? thumbnailVersion { get; set; }
    [JsonProperty("thumbnail")]
    Drive2_File_thumbnail thumbnail_;
    [JsonIgnore]
    public IDrive2_File_thumbnail thumbnail { get { return thumbnail_; } }
    public string title { get; set; }
    public string mimeType { get; set; }
    public string description { get; set; }
    public Drive2_File_label labels { get; set; }
    public DateTime? createdDate { get; set; }
    public DateTime? modifiedDate { get; set; }
    public DateTime? modifiedByMeDate { get; set; }
    public DateTime? lastViewedByMeDate { get; set; }
    public DateTime? markedViewedByMeDate { get; set; }
    public DateTime? sharedWithMeDate { get; set; }
    public long? version { get; set; }
    public Drive2_User sharingUser { get; set; }
    public List<Drive2_Parent> parents { get; set; }
    public string downloadUrl { get; set; }
    //exportLinks { (key): string }
    public Drive2_File_indexableText indexableText { get; set; }
    public Drive2_Permission userPermission { get; set; }
    public List<Drive2_Permission> permissions { get; set; }
    public bool? hasAugmentedPermissions { get; set; }
    public string originalFilename { get; set; }
    public string fileExtension { get; set; }
    public string fullFileExtension { get; set; }
    public string md5Checksum { get; set; }
    public long? fileSize { get; set; }
    public long? quotaBytesUsed { get; set; }
    public List<string> ownerNames { get; set; }
    public List<Drive2_User> owners { get; set; }
    public string teamDriveId { get; set; }
    public string lastModifyingUserName { get; set; }
    public Drive2_User lastModifyingUser { get; set; }
    public bool? ownedByMe { get; set; }
    public Drive2_File_capability capabilities { get; set; }
    public bool? editable { get; set; }
    public bool? canComment { get; set; }
    public bool? canReadRevisions { get; set; }
    public bool? shareable { get; set; }
    public bool? copyable { get; set; }
    public bool? writersCanShare { get; set; }
    public bool? shared { get; set; }
    public bool? explicitlyTrashed { get; set; }
    public Drive2_User trashingUser { get; set; }
    public DateTime? trashedDate { get; set; }
    public bool? appDataContents { get; set; }
    public string headRevisionId { get; set; }
    public List<Drive2_File_property> properties { get; set; }
    public string folderColorRgb { get; set; }
    public Drive2_File_imageMediaMetadata imageMediaMetadata { get; set; }
    public Drive2_File_videoMediaMetadata videoMediaMetadata { get; set; }
    public List<string> spaces { get; set; }
    public bool? isAppAuthorized { get; set; }
  }

  
  public interface IDrive2_File_thumbnail
  {
    /// <summary>
    /// The URL-safe Base64 encoded bytes of the thumbnail image. It should conform to RFC 4648 section 5.	
    /// </summary>
    byte[] image { get; }
    /// <summary>
    /// The MIME type of the thumbnail.	
    /// </summary>
    string mimeType { get; }
  }
  internal class Drive2_File_thumbnail : IDrive2_File_thumbnail//for deserialize
  {
    public byte[] image { get; set; }
    public string mimeType { get; set; }
  }

  
  public class Drive2_File_indexableText//writable
  {
    /// <summary>
    /// The text to be indexed for this file.	
    /// </summary>
    public string text { get; set; }
  }
  public class Drive2_File_label//writable
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
    public bool? modified { get; set; }
  }
  public class Drive2_File_capability//read-only
  {
    /// <summary>
    /// Whether the current user can add children to this folder. This is always false when the item is not a folder.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canAddChildren { get; set; }

    /// <summary>
    /// Whether the current user can change the restricted download label of this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canChangeRestrictedDownload { get; set; }

    /// <summary>
    /// Whether the current user can comment on this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canComment { get; set; }

    /// <summary>
    /// Whether the current user can copy this file. For a Team Drive item, whether the current user can copy non-folder descendants of this item, or this item itself if it is not a folder.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canCopy { get; set; }

    /// <summary>
    /// Whether the current user can delete this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canDelete { get; set; }

    /// <summary>
    /// Whether the current user can download this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canDownload { get; set; }

    /// <summary>
    /// Whether the current user can edit this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canEdit { get; set; }

    /// <summary>
    /// Whether the current user can list the children of this folder. This is always false when the item is not a folder.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canListChildren { get; set; }

    /// <summary>
    /// Whether the current user can move this item into a Team Drive. If the item is in a Team Drive, this field is equivalent to canMoveTeamDriveItem.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canMoveItemIntoTeamDrive { get; set; }

    /// <summary>
    /// Whether the current user can move this Team Drive item by changing its parent. Note that a request to change the parent for this item may still fail depending on the new parent that is being added. Only populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canMoveTeamDriveItem { get; set; }

    /// <summary>
    /// Whether the current user can read the revisions resource of this file. For a Team Drive item, whether revisions of non-folder descendants of this item, or this item itself if it is not a folder, can be read.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canReadRevisions { get; set; }

    /// <summary>
    /// Whether the current user can read the Team Drive to which this file belongs. Only populated for Team Drive files.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canReadTeamDrive { get; set; }

    /// <summary>
    /// Whether the current user can remove children from this folder. This is always false when the item is not a folder.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canRemoveChildren { get; set; }

    /// <summary>
    /// Whether the current user can rename this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canRename { get; set; }

    /// <summary>
    /// Whether the current user can modify the sharing settings for this file.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canShare { get; set; }

    /// <summary>
    /// Whether the current user can move this file to trash.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canTrash { get; set; }

    /// <summary>
    /// Whether the current user can restore this file from trash.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? canUntrash { get; set; }
  }
  public class Drive2_File_property//read-only
  {
    /// <summary>
    /// This is always drive#property.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string kind { get; set; }

    /// <summary>
    /// ETag of the property.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string etag { get; set; }

    /// <summary>
    /// The link back to this property.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string selfLink { get; set; }

    /// <summary>
    /// The key of this property.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string key { get; set; }

    /// <summary>
    /// The visibility of this property.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string visibility { get; set; }

    /// <summary>
    /// The value of this property.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string value { get; set; }
  }
  public class Drive2_File_imageMediaMetadata//read-only
  {
    /// <summary>
    /// The width of the image in pixels.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? width { get; set; }

    /// <summary>
    /// The height of the image in pixels.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? height { get; set; }

    /// <summary>
    /// The rotation in clockwise degrees from the image's original orientation.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? rotation { get; set; }

    /// <summary>
    /// Geographic location information stored in the image.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_File_location location { get; set; }

    /// <summary>
    /// The date and time the photo was taken (EXIF format timestamp).	
    /// </summary>
    [JsonIgnoreSerialize]
    public string date { get; set; }

    /// <summary>
    /// The make of the camera used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string cameraMake { get; set; }

    /// <summary>
    /// The model of the camera used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string cameraModel { get; set; }

    /// <summary>
    /// The length of the exposure, in seconds.	
    /// </summary>
    [JsonIgnoreSerialize]
    public float? exposureTime { get; set; }

    /// <summary>
    /// The aperture used to create the photo (f-number).	
    /// </summary>
    [JsonIgnoreSerialize]
    public float? aperture { get; set; }

    /// <summary>
    /// Whether a flash was used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? flashUsed { get; set; }

    /// <summary>
    /// The focal length used to create the photo, in millimeters.	
    /// </summary>
    [JsonIgnoreSerialize]
    public float? focalLength { get; set; }

    /// <summary>
    /// The ISO speed used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? isoSpeed { get; set; }

    /// <summary>
    /// The metering mode used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string meteringMode { get; set; }

    /// <summary>
    /// The type of sensor used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string sensor { get; set; }

    /// <summary>
    /// The exposure mode used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string exposureMode { get; set; }

    /// <summary>
    /// The color space of the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string colorSpace { get; set; }

    /// <summary>
    /// The white balance mode used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string whiteBalance { get; set; }

    /// <summary>
    /// The exposure bias of the photo (APEX value).	
    /// </summary>
    [JsonIgnoreSerialize]
    public float? exposureBias { get; set; }

    /// <summary>
    /// The smallest f-number of the lens at the focal length used to create the photo (APEX value).	
    /// </summary>
    [JsonIgnoreSerialize]
    public float? maxApertureValue { get; set; }

    /// <summary>
    /// The distance to the subject of the photo, in meters.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? subjectDistance { get; set; }

    /// <summary>
    /// The lens used to create the photo.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string lens { get; set; }
  }
  public class Drive2_File_videoMediaMetadata//read-only
  {
    /// <summary>
    /// The width of the video in pixels.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? width { get; set; }

    /// <summary>
    /// The height of the video in pixels.	
    /// </summary>
    [JsonIgnoreSerialize]
    public int? height { get; set; }

    /// <summary>
    /// The duration of the video in milliseconds.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? durationMillis { get; set; }
  }
  public class Drive2_File_location//read-only
  {
    /// <summary>
    /// The latitude stored in the image.	
    /// </summary>
    [JsonIgnoreSerialize]
    public double? latitude { get; set; }

    /// <summary>
    /// The longitude stored in the image.	
    /// </summary>
    [JsonIgnoreSerialize]
    public double? longitude { get; set; }

    /// <summary>
    /// The altitude stored in the image.	
    /// </summary>
    [JsonIgnoreSerialize]
    public double? altitude { get; set; }
  }
  #endregion

  #region About
  public class Drive2_About//read-only
  {
    /// <summary>
    /// This is always drive#about.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string kind { get; set; }

    /// <summary>
    /// The ETag of the item.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string etag { get; set; }

    /// <summary>
    /// A link back to this item.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string selfLink { get; set; }

    /// <summary>
    /// The name of the current user.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string name { get; set; }

    /// <summary>
    /// The authenticated user.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_User user { get; set; }

    /// <summary>
    /// The total number of quota bytes.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? quotaBytesTotal { get; set; }

    /// <summary>
    /// The number of quota bytes used by Google Drive.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? quotaBytesUsed { get; set; }

    /// <summary>
    /// The number of quota bytes used by all Google apps (Drive, Picasa, etc.).	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? quotaBytesUsedAggregate { get; set; }

    /// <summary>
    /// The number of quota bytes used by trashed items.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? quotaBytesUsedInTrash { get; set; }

    /// <summary>
    /// The type of the user's storage quota.
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_About_quotaType quotaType { get; set; }

    [JsonIgnoreSerialize]
    public List<Drive2_About_quotaBytesByService> quotaBytesByService { get; set; }

    /// <summary>
    /// The largest change id.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? largestChangeId { get; set; }

    /// <summary>
    /// The number of remaining change ids, limited to no more than 2500.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? remainingChangeIds { get; set; }

    /// <summary>
    /// The id of the root folder.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string rootFolderId { get; set; }

    /// <summary>
    /// The domain sharing policy for the current user.
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_About_domainSharingPolicy domainSharingPolicy { get; set; }

    /// <summary>
    /// The current user's ID as visible in the permissions collection.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string permissionId { get; set; }

    /// <summary>
    /// The allowable import formats.	
    /// </summary>
    [JsonIgnoreSerialize]
    public List<Drive2_About_Format> importFormats { get; set; }

    /// <summary>
    /// The allowable export formats.	
    /// </summary>
    [JsonIgnoreSerialize]
    public List<Drive2_About_Format> exportFormats { get; set; }

    /// <summary>
    /// Information about supported additional roles per file type. The most specific type takes precedence.	
    /// </summary>
    [JsonIgnoreSerialize]
    public List<Drive2_About_additionalRoleInfo> additionalRoleInfo { get; set; }

    /// <summary>
    /// List of additional features enabled on this account.	
    /// </summary>
    [JsonIgnoreSerialize]
    public List<Drive2_About_feature> features { get; set; }

    /// <summary>
    /// List of max upload sizes for each file type. The most specific type takes precedence.	
    /// </summary>
    [JsonIgnoreSerialize]
    public List<Drive2_About_maxUploadSize> maxUploadSizes { get; set; }

    /// <summary>
    /// A boolean indicating whether the authenticated app is installed by the authenticated user.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? isCurrentAppInstalled { get; set; }

    /// <summary>
    /// The user's language or locale code, as defined by BCP 47, with some extensions from Unicode's LDML format (http://www.unicode.org/reports/tr35/).	
    /// </summary>
    [JsonIgnoreSerialize]
    public string languageCode { get; set; }

    /// <summary>
    /// The palette of allowable folder colors as RGB hex strings.	
    /// </summary>
    [JsonIgnoreSerialize]
    public List<string> folderColorPalette { get; set; }

    /// <summary>
    /// A list of themes that are supported for Team Drives.	
    /// </summary>
    [JsonIgnoreSerialize]
    public List<Drive2_About_teamDriveTheme> teamDriveThemes { get; set; }
  }

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
  public class Drive2_About_quotaBytesByService//read-only
  {
    /// <summary>
    /// The service's name, e.g. DRIVE, GMAIL, or PHOTOS.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string serviceName { get; set; }

    /// <summary>
    /// The storage quota bytes used by the service.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? bytesUsed { get; set; }
  }
  public class Drive2_About_Format//read-only
  {
    /// <summary>
    /// The imported file's content type to convert from.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string source { get; set; }

    /// <summary>
    /// The possible content types to convert to.	
    /// </summary>
    [JsonIgnoreSerialize]
    public List<string> targets { get; set; }
  }
  public class Drive2_About_additionalRoleInfo//read-only
  {
    /// <summary>
    /// The content type that this additional role info applies to.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string type { get; set; }

    /// <summary>
    /// The supported additional roles per primary role.	
    /// </summary>
    [JsonIgnoreSerialize]
    public List<Drive2_About_roleSet> roleSets { get; set; }
  }
  public class Drive2_About_roleSet//read-only
  {
    /// <summary>
    /// A primary permission role.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string primaryRole { get; set; }

    /// <summary>
    /// The supported additional roles with the primary role.	
    /// </summary>
    [JsonIgnoreSerialize]
    public List<string> additionalRoles { get; set; }
  }
  public class Drive2_About_feature//read-only
  {
    /// <summary>
    /// The name of the feature.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string featureName { get; set; }

    /// <summary>
    /// The request limit rate for this feature, in queries per second.	
    /// </summary>
    [JsonIgnoreSerialize]
    public double? featureRate { get; set; }
  }
  public class Drive2_About_maxUploadSize//read-only
  {
    /// <summary>
    /// The file type.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string type { get; set; }

    /// <summary>
    /// The max upload size for this type.	
    /// </summary>
    [JsonIgnoreSerialize]
    public long? size { get; set; }
  }
  public class Drive2_About_teamDriveTheme//read-only
  {
    /// <summary>
    /// The ID of the theme.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string id { get; set; }

    /// <summary>
    /// A link to this Team Drive theme's background image.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string backgroundImageLink { get; set; }

    /// <summary>
    /// The color of this Team Drive theme as an RGB hex string.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string colorRgb { get; set; }
  }
  #endregion

  #region Changes
  #endregion

  #region Children
  #endregion

  #region Parents
  public class Drive2_Parents_list//read-only
  {
    /// <summary>
    /// This is always drive#parentList.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string kind { get; set; }

    /// <summary>
    /// The ETag of the list.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string etag { get; set; }

    /// <summary>
    /// A link back to this list.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string selfLink { get; set; }

    /// <summary>
    /// The list of parents.	
    /// </summary>
    public List<Drive2_Parent> items { get; set; }
  }
  public class Drive2_Parent
  {
    /// <summary>
    /// This is always drive#parent Reference.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string kind { get; set; }

    /// <summary>
    /// The ID of the parent.	
    /// </summary>
    public string id { get; set; }

    /// <summary>
    /// A link back to this reference.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string selfLink { get; set; }

    /// <summary>
    /// A link to the parent.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string parentLink { get; set; }

    /// <summary>
    /// Whether or not the parent is the root folder.	
    /// </summary>
    public bool? isRoot { get; set; }
  }
  #endregion

  #region Permissions
  public class Drive2_Permission//writeable
  {
    /// <summary>
    /// This is always drive#permission.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string kind { get; set; }

    /// <summary>
    /// The ETag of the permission.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string etag { get; set; }

    /// <summary>
    /// The ID of the user this permission refers to, and identical to the permissionId in the About and Files resources. When making a drive.permissions.insert request, exactly one of the id or value fields must be specified unless the permission type is anyone, in which case both id and value are ignored.	
    /// </summary>
    public string id { get; set; }

    /// <summary>
    /// A link back to this permission.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string selfLink { get; set; }

    /// <summary>
    /// The name for this permission.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string name { get; set; }

    /// <summary>
    /// The email address of the user or group this permission refers to. This is an output-only field which is present when the permission type is user or group.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string emailAddress { get; set; }

    /// <summary>
    /// The domain name of the entity this permission refers to. This is an output-only field which is present when the permission type is user, group or domain.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string domain { get; set; }

    /// <summary>
    /// The primary role for this user. While new values may be supported in the future
    /// </summary>
    public Drive2_Permission_PermissionRole role { get; set; }

    /// <summary>
    /// Additional roles for this user. Only commenter is currently allowed, though more may be supported in the future.	
    /// </summary>
    public List<string> additionalRoles { get; set; }

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
    public string authKey { get; set; }

    /// <summary>
    /// Whether the link is required for this permission.	
    /// </summary>
    public bool? withLink { get; set; }

    /// <summary>
    /// A link to the profile photo, if available.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string photoLink { get; set; }

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
    public List<Drive2_Permission_teamDrivePermissionDetail> teamDrivePermissionDetails { get; set; }

    /// <summary>
    /// Whether the account associated with this permission has been deleted. This field only pertains to user and group permissions.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? deleted { get; set; }
  }


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
  public class Drive2_Permission_teamDrivePermissionDetail//read-only
  {
    /// <summary>
    /// The Team Drive permission type for this user. While new values may be added in future
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_Permission_TeamPermissionType teamDrivePermissionType { get; set; }

    /// <summary>
    /// The primary role for this user. While new values may be added in the future
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_Permission_PermissionRole role { get; set; }

    /// <summary>
    /// Additional roles for this user. Only commenter is currently possible, though more may be supported in the future.	
    /// </summary>
    [JsonIgnoreSerialize]
    public List<string> additionalRoles { get; set; }

    /// <summary>
    /// The ID of the item from which this permission is inherited. This is an output-only field and is only populated for members of the Team Drive.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string inheritedFrom { get; set; }

    /// <summary>
    /// Whether this permission is inherited. This field is always populated. This is an output-only field.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? inherited { get; set; }
  }
  #endregion

  #region Revisions
  #endregion

  #region Apps
  #endregion

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
  public class Drive2_User//read-only
  {
    /// <summary>
    /// This is always drive#user.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string kind { get; set; }
    /// <summary>
    /// A plain text displayable name for this user.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string displayName { get; set; }
    /// <summary>
    /// The user's profile picture.	
    /// </summary>
    [JsonIgnoreSerialize]
    public Drive2_User_picture picture { get; set; }
    /// <summary>
    /// Whether this user is the same as the authenticated user for whom the request was made.	
    /// </summary>
    [JsonIgnoreSerialize]
    public bool? isAuthenticatedUser { get; set; }
    /// <summary>
    /// The user's ID as visible in the permissions collection.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string permissionId { get; set; }
    /// <summary>
    /// The email address of the user.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string emailAddress { get; set; }
  }
  public class Drive2_User_picture//read-only
  {
    /// <summary>
    /// A URL that points to a profile picture of this user.	
    /// </summary>
    [JsonIgnoreSerialize]
    public string url { get; set; }
  }
  #endregion
}
