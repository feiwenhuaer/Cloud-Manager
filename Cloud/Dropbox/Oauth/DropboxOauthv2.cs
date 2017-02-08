using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Cloud.Dropbox.Oauth
{
    public class DropboxOauthv2: OauthV2
    {
        internal const string LoopbackCallback = "http://localhost:{0}";
        internal const int MaxPortRange = 22439;
        internal const int MinPortRange = 22430;
        int port = -1;
        public void GetCode(OauthUI ui, object owner)
        {
            port = GetFirstAvailableRandomPort(MinPortRange, MaxPortRange);
            redirectURI = string.Format(LoopbackCallback, port) + "/";
            authorizationRequest = string.Format("https://www.dropbox.com/1/oauth2/authorize?client_id={0}&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A{1}", Appkey.ApiKey, port.ToString());

            GetCode_(ui, owner, new HttpListenerContextRecieve(Rev));
        }

        void Rev(HttpListenerContext ls)
        {
            string code = ls.Request.QueryString.Get("code");
            if (code == null)
            {
                try { listener.Close(); } catch { }
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
