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

        public static string Drive
        {
            get { return "https://www.googleapis.com/auth/drive"; }
        }

        public static string DriveFile
        {
            get { return "https://www.googleapis.com/auth/drive.file"; }
        }

        public static string DriveReadonly
        {
            get { return "https://www.googleapis.com/auth/drive.readonly"; }
        }

        public static string DriveMetadataReadonly
        {
            get { return "https://www.googleapis.com/auth/drive.metadata.readonly"; }
        }

        public static string DriveAppdata
        {
            get { return "https://www.googleapis.com/auth/drive.appdata"; }
        }

        public static string DriveMetadata
        {
            get { return "https://www.googleapis.com/auth/drive.metadata"; }
        }

        public static string DrivePhotosReadonly
        {
            get { return "https://www.googleapis.com/auth/drive.photos.readonly"; }
        }
    }
}