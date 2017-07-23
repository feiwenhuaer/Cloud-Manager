namespace Cloud.GoogleDrive
{
    public enum CorpusEnum
    {
        DEFAULT, DOMAIN
    }

    public enum OrderByEnum
    {
        createdDate, folder, lastViewedByMeDate, modifiedByMeDate, modifiedDate, quotaBytesUsed, recency, sharedWithMeDate, starred, title
    }
    public enum ProjectionEnum
    {
        BASIC, FULL
    }

    public enum SpacesEnum
    {
        drive, appDataFolder, photos
    }

    public enum TypeReturn
    {
        string_, streamresponse_, streamupload_, header_response
    }

    public enum uploadType
    {
        media, multipart, resumable
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
    public enum Error403
    {
        dailyLimitExceeded,
        userRateLimitExceeded,
        rateLimitExceeded,
        sharingRateLimitExceeded,
        appNotAuthorizedToFile,
        insufficientFilePermissions,
        domainPolicy,

        forbidden,
        abuse//malware
    }
}
