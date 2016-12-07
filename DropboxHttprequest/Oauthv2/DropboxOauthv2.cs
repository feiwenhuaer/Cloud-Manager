using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace DropboxHttpRequest
{
    public class DropboxOauthv2
    {
        private string APPKey = "sdxv9bvu37pjd5r";
        private string APPSecret = "xsfy8way52uuf1j";
        internal const string LoopbackCallback = "http://localhost:{0}";
        internal const int MaxPortRange = 22439;
        internal const int MinPortRange = 22430;
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
        string redirectURI;

        public delegate void DelegateToken(string token);
        public event DelegateToken TokenCallBack;

        HttpListener listener;
        int port = -1;
        public void GetCode(Form owner = null)
        {
            port = GetFirstAvailableRandomPort(MinPortRange, MaxPortRange);
            listener = new HttpListener();
            redirectURI = string.Format(LoopbackCallback, port);
            listener.Prefixes.Add(redirectURI +"/");
            string urloauth = string.Format("https://www.dropbox.com/1/oauth2/authorize?client_id={0}&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A{1}"
                , APPKey, port.ToString());
            try
            {
                listener.Start();
                Oauthv2FormDropbox f = new Oauthv2FormDropbox(urloauth,redirectURI);
                if (owner == null) f.Show();
                else f.Show(owner);
                listener.BeginGetContext(new AsyncCallback(RecieveCode), null);
            }catch
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
            string code = ls.Request.QueryString.Get("code");
            if (code == null)
            {
                try { listener.Close(); } catch { }
                TokenCallBack.Invoke(null);
                return;
            }
            DropboxRequestAPIv2 client = new DropboxRequestAPIv2();
            client.GetAccessToken(code, port);
            TokenCallBack.Invoke(client.AccessToken);
        }
        
        internal static int GetFirstAvailableRandomPort(int startPort, int stopPort)
        {
            Random r = new Random();
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            var busyPorts = tcpConnInfoArray.Select(t => t.LocalEndPoint.Port).Where(v => v >= startPort && v <= stopPort).ToArray();
            var firstAvailableRandomPort = Enumerable.Range(startPort, stopPort - startPort).OrderBy(v => r.Next()).FirstOrDefault(p => !busyPorts.Contains(p));
            return firstAvailableRandomPort;
        }
    }
}
