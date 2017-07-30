namespace Cloud.GoogleDrive
{
  public class TokenGoogleDrive
  {
    public string Email { get; set; }
    public bool IsError { get; set; } = false;
    public string Code { get; set; }
    public string access_token { get; set; }
    public string refresh_token { get; set; }
    public string id_token { get; set; }
    public int expires_in { get; set; } = 0;
    internal bool CheckToken()
    {
      return !IsError && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(access_token) && !string.IsNullOrEmpty(access_token);
    }
  }
}
