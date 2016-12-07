using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DropboxHttprequest.Oauthv2
{
    public interface OauthUI
    {
        void Show(object owner = null);
    }
}
