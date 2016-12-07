namespace GoogleDriveHttprequest
{
    public class TokenGoogleDrive
    {
        public bool IsError = false;
        public string Code;
        public string access_token;
        public string refresh_token;
        public string id_token;
        public int expires_in = 0;
    }
}
