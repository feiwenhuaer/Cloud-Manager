using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud.GoogleDrive
{
    public class GoogleDriveErrorMessage
    {
        public GDErrorMessage error;
    }

    public class GDErrorMessage
    {
        public int code;
        public string message;
        public List<GDErrorreason> errors;

    }

    public class GDErrorreason
    {
        public string domain;
        public string message;
        public string reason;
    }

}
