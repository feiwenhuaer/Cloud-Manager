using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;

namespace Cloud.Dropbox.Oauth
{
    public class DropboxOauthv2: OauthV2
    {
        public DropboxOauthv2()
        {
            DropboxAppKey.Check();
        }


        internal const string LoopbackCallback = "http://localhost:{0}";
        internal const int MaxPortRange = 22439;
        internal const int MinPortRange = 22430;
        int port = -1;
        public void GetCode(IOauth ui, object owner)
        {
            port = GetFirstAvailableRandomPort(MinPortRange, MaxPortRange);
            redirectURI = string.Format(LoopbackCallback, port) + "/";
            authorizationRequest = string.Format("https://www.dropbox.com/1/oauth2/authorize?client_id={0}&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A{1}", DropboxAppKey.ApiKey, port.ToString());
            ui.EventUriResponse += Ui_EventUriResponse;
            GetCode_(ui, owner);
        }

        private void Ui_EventUriResponse(Uri uri)
        {
            string code = HttpUtility.ParseQueryString(uri.Query).Get("code");
            if(code == null)
            {
                ReturnToken(null);
                return;
            }
            DropboxRequestAPIv2 client = new DropboxRequestAPIv2();
            client.GetAccessToken(code, port);
            ReturnToken(client.AccessToken);
        }
        
        int GetFirstAvailableRandomPort(int startPort, int stopPort)
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
