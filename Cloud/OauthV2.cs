using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Cloud
{
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
        internal HttpListener listener;
        internal string redirectURI;
        internal string authorizationRequest;

        public delegate void DelegateToken(string token);
        public event DelegateToken TokenCallBack;
        internal delegate void HttpListenerContextRecieve(HttpListenerContext ls);

        internal void GetCode_(OauthUI ui, object owner,HttpListenerContextRecieve rev)
        {
            if (string.IsNullOrEmpty(authorizationRequest) | string.IsNullOrEmpty(redirectURI)) throw new Exception("Oauth:authorizationRequest or redirectURI is null.");
            listener = new HttpListener();
            listener.Prefixes.Add(redirectURI);
            try
            {
                listener.Start();
                ui.Url = authorizationRequest;
                ui.CheckUrl = redirectURI;
                ui.ShowUI(owner);
                listener.BeginGetContext(new AsyncCallback(RecieveCode), rev);
            }
            catch
            {
                TokenCallBack.Invoke(null);
            }
        }

        void RecieveCode(IAsyncResult rs)
        {
            HttpListenerContext ls = listener.EndGetContext(rs);
            using (var writer = new StreamWriter(ls.Response.OutputStream))
            {
                writer.WriteLine(ClosePageResponse);
                writer.Flush();
            }
            ls.Response.OutputStream.Close();
            ((HttpListenerContextRecieve)rs.AsyncState)(ls);
        }

        internal void ReturnToken(string token)
        {
            TokenCallBack.Invoke(token);
        }
    }
}
