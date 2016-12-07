namespace DropboxHttpRequest
{
    public static class ErrorCode
    {
        public static string ErrorText(int err)
        {
            switch (err)
            {
                case 400:
                    return "Bad input parameter. Error message should indicate which one and why.";
                case 401:
                    return "Bad or expired token. This can happen if the user or Dropbox revoked or expired an access token. To fix, you should re-authenticate the user.";
                case 403:
                    return "Bad OAuth request (wrong consumer key, bad nonce, expired timestamp...). Unfortunately, re-authenticating the user won't help here.";
                case 404:
                    return "File or folder not found at the specified path.";
                case 405:
                    return "Request method not expected (generally should be GET or POST).";
                case 429:
                    return "Your app is making too many requests and is being rate limited. 429s can trigger on a per-app or per-user basis.";
                case 503:
                    return "If the response includes the Retry-After header, this means your OAuth 1.0 app is being rate limited. Otherwise, this indicates a transient server error, and your app should retry its request.";
                case 507:
                    return "User is over Dropbox storage quota.";
                default:
                    return "Server error. Check DropboxOps.";
            }
        }
    }
}
