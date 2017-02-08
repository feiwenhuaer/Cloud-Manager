namespace Cloud.GoogleDrive
{
    public static class Scope
    {
        public static string GetParameters(string[] scopes)
        {
            if (scopes == null) return null;
            string s = "";
            foreach (string str in scopes)
            {
                s += str + "%20";
            }
            return string.IsNullOrEmpty(s) ? null : s.Remove(s.Length - 3);
        }

        public const string Drive = "https://www.googleapis.com/auth/drive";
        public const string DriveFile = "https://www.googleapis.com/auth/drive.file";
        public const string DriveReadonly = "https://www.googleapis.com/auth/drive.readonly";
        public const string DriveMetadataReadonly = "https://www.googleapis.com/auth/drive.metadata.readonly";
        public const string DriveAppdata = "https://www.googleapis.com/auth/drive.appdata";
        public const string DriveMetadata = "https://www.googleapis.com/auth/drive.metadata";
        public const string DrivePhotosReadonly = "https://www.googleapis.com/auth/drive.photos.readonly";
    }
}