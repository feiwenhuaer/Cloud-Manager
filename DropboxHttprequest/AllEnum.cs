namespace DropboxHttpRequest
{
    public enum DropboxUploadMode
    {
        add,
        update
    }

    public enum DropboxSearchMode
    {
        filename
    }

    public enum Dropboxthumbnail
    {
        w32h32,
        w64h64,
        w128h128,
        w640h480,
        w1024h768
    }

    public enum DropboxImageFormat
    {
        jpeg,
        png
    }

    public enum DropboxPreviewReceived
    {
        html,pdf
    }

    internal enum TypeRequest
    {
        POST,
        GET
    }
    public enum DropboxTag
    {
        email, dropbox_id
    }
    public enum DropboxSharingAccess_level
    {
        viewer, editor
    }

    public enum DropboxMemberPolicy
    {
        team, anyone
    }

    public enum DropboxAclUpdatePolicy
    {
        owner, editors
    }

    public enum DropboxSharedLinkPolicy
    {
        anyone, members
    }

    public enum DropboxRequestedVisibility
    {
        Public, team_only, password
    }
}
