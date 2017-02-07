namespace Cloud
{
    public interface OauthUI
    {
        string Url { set; }
        string CheckUrl { set; }
        void ShowUI(object owner);

        void CloseUI();
    }
}
