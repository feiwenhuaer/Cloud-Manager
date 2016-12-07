namespace GoogleDriveHttprequest
{
    enum TypeRequest
    {
        POST,
        GET,
        DELETE,
        PUT,
        PATCH
    }

    public enum audio
    {
        mp3 = 0
    }
    public enum document
    {
        pdf,
        docx = 0,//default
        txt,
        html,
        odt,
        rtf
    }
    public enum drawing
    {
        png = 0,
    }
    public enum form
    {

    }
    public enum fusiontable
    {

    }
    public enum map
    {

    }
    public enum photo
    {
        jpg,
        jpeg = 0//default
    }
    public enum presentation
    {

    }
    public enum script
    {

    }
    public enum spreadsheet
    {
        csv,
        ods,
        pdf,
        xlsx = 0//default
    }
    public enum unknown
    {
        bin = 0
    }

    public enum video
    {
        mp4 = 0
    }
}
