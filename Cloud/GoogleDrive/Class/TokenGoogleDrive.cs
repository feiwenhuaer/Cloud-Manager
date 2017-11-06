using Newtonsoft.Json;
using System;
using System.Threading;

namespace Cloud.GoogleDrive
{
  public delegate void TokenUpdate(TokenGoogleDrive token);
  public class TokenGoogleDrive
  {
    [JsonIgnore]
    public AutoResetEvent wait = new AutoResetEvent(false);
    [JsonIgnore]
    public Thread refresh_thread;

    public event TokenUpdate EventTokenUpdate;
    public string Email { get; set; }
    public bool IsError { get; set; } = false;
    public string Code { get; set; }

    [JsonIgnore]
    string access_token_ = "";
    public string access_token { get { return access_token_; } set { access_token_ = value; if (EventTokenUpdate != null) EventTokenUpdate.Invoke(this); } }
    public string refresh_token { get; set; }
    public string id_token { get; set; }
    public int expires_in { get; set; } = 0;
    internal bool CheckToken()
    {
      return !IsError && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(access_token) && !string.IsNullOrEmpty(access_token);
    }
  }
}
