namespace Cloud
{
    public static class DropboxAppKey
    {
        internal static void Check()
        {
            if (ApiKey == null || ApiSecret == null) throw new System.Exception("API Key is null.");
        }
        public static string ApiKey { internal get; set; } = null;
        public static string ApiSecret { internal get; set; } = null;
    }

    public static class GoogleDriveAppKey
    {
        internal static void Check()
        {
            if( ApiKey == null || ClientID == null || Clientsecret == null) throw new System.Exception("API Key is null.");
        }
        public static string ClientID { internal get; set; } = null;
        public static string Clientsecret { internal get; set; } = null;
        public static string ApiKey { internal get; set; } = null;
    }

    public static class MegaNzAppKey
    {
        internal static void Check()
        {
            if( ApiKey == null) throw new System.Exception("API Key is null.");
        }
        public static string ApiKey { internal get; set; } = null;
    }
}
