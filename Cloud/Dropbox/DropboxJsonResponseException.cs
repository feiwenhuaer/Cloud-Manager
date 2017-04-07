using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud.Dropbox
{
    public class DropboxJsonResponseException:Exception
    {
        public DropboxJsonResponseException(IDropbox_Response_Error error)
        {
            this.Error_ = error;
        }

        IDropbox_Response_Error Error_;
        public IDropbox_Response_Error Error
        {
            get { return Error_; }
        }
    }
}
