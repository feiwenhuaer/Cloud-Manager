using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace Cloud.GoogleDrive
{
  public class Drive3_About//read-only
  {
    /// <summary>
    /// Identifies what kind of resource this is. Value: the fixed string "drive#about".	
    /// </summary>
    public string kind { get; set; }
    /// <summary>
    /// The authenticated user.	
    /// </summary>
    public Drive3_User user { get; set; }
    /// <summary>
    /// The user's storage quota limits and usage. All fields are measured in bytes.	
    /// </summary>
    public Drive3_About_storageQuota storageQuota { get; set; }

    //importFormats
    //exportFormats
    //maxImportSizes

    /// <summary>
    /// The maximum upload size in bytes.	
    /// </summary>
    public long maxUploadSize { get; set; }
    /// <summary>
    /// Whether the user has installed the requesting app.	
    /// </summary>
    public bool appInstalled { get; set; }
    /// <summary>
    /// The currently supported folder colors as RGB hex strings.	
    /// </summary>
    public List<string> folderColorPalette { get; set; }
    /// <summary>
    /// A list of themes that are supported for Team Drives.	
    /// </summary>
    public List<Drive2_About_teamDriveTheme> teamDriveThemes { get; set; }
  }

  public class Drive3_About_storageQuota
  {
    /// <summary>
    /// The usage limit, if applicable. This will not be present if the user has unlimited storage.	
    /// </summary>
    public long limit { get; set; }
    /// <summary>
    /// The total usage across all services.	
    /// </summary>
    public long usage { get; set; }
    /// <summary>
    /// The usage by all files in Google Drive.	
    /// </summary>
    public long usageInDrive { get; set; }
    /// <summary>
    /// The usage by trashed files in Google Drive.	
    /// </summary>
    public long usageInDriveTrash { get; set; }
  }
  











  public class Drive3_User//read-only
  {
    /// <summary>
    /// Identifies what kind of resource this is. Value: the fixed string "drive#user".	
    /// </summary>
    public string kind { get; set; }
    /// <summary>
    /// A plain text displayable name for this user.	
    /// </summary>
    public string displayName { get; set; }
    /// <summary>
    /// A link to the user's profile photo, if available.	
    /// </summary>
    public string photoLink { get; set; }
    /// <summary>
    /// Whether this user is the requesting user.	
    /// </summary>
    public bool me { get; set; }
    /// <summary>
    /// The user's ID as visible in Permission resources.	
    /// </summary>
    public string permissionId { get; set; }
    /// <summary>
    /// The email address of the user. This may not be present in certain contexts if the user has not made their email address visible to the requester.	
    /// </summary>
    public string emailAddress { get; set; }
  }
}
