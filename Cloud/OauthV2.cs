using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace Cloud
{
    public delegate void DelegateToken(string token);

    public abstract class OauthV2
    {
        internal const string ClosePageResponse =
@"<html>
  <head><title>OAuth 2.0 Authentication Token Received</title></head>
  <body>
    Received verification code.
    <script type='text/javascript'>
      // This doesn't work on every browser.
      window.setTimeout(function() {
          window.open('', '_self', ''); 
          window.close(); 
        }, 1000);
      if (window.opener) { window.opener.checkToken(); }
    </script>
  </body>
</html>";
        internal string redirectURI;
        internal string authorizationRequest;
        
        public event DelegateToken TokenCallBack;
        byte[] buffer = new byte[1024];
        Process Cloud_Oauth;
        internal void GetCode_(OauthUI ui, object owner)//, HttpListenerContextRecieve rev)
        {
            if (string.IsNullOrEmpty(authorizationRequest) | string.IsNullOrEmpty(redirectURI)) throw new Exception("Oauth:authorizationRequest or redirectURI is null.");
            //Cloud_Oauth = Process.Start(Directory.GetCurrentDirectory() + "\\Cloud_Oauth.exe", redirectURI);
            //Cloud_Oauth.OutputDataReceived += Cloud_Oauth_OutputDataReceived;
            ui.Url = authorizationRequest;
            ui.CheckUrl = redirectURI;
            ui.ShowUI(owner);
        }

        private void Cloud_Oauth_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            throw new Exception(e.Data);
        }

        internal void ReturnToken(string token)
        {
            TokenCallBack.Invoke(token);
        }
    }
}
