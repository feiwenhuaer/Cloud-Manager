using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace GoogleDriveHttprequest.Oauth
{
    public class GoogleAPIOauth2
    {
        #region Value
        string refresh_token = "";
        string Error_refresh_token = "";
        const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";
        const string LoopbackCallback = "http://localhost:{0}/authorize/";
        string[] scopes = { Scope.Drive, Scope.DriveFile, Scope.DriveMetadata };
        HttpListener listener;
        private const string ClosePageResponse =
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
        public delegate void DelegateToken(TokenGoogleDrive token);
        public event DelegateToken TokenCallBack;

        string state;
        string code_verifier;
        string code_challenge;
        const string code_challenge_method = "S256";
        string redirectURI;
        public string ErrorRefreshToken
        {
            get { return this.Error_refresh_token; }
        }

        TokenGoogleDrive tokencode = new TokenGoogleDrive();
        public TokenGoogleDrive GetTokenKey
        {
            get { return tokencode; }
        }
        #endregion
        public GoogleAPIOauth2(){}
        public GoogleAPIOauth2(string[] scopes)
        {
            this.scopes = scopes;
        }
        public GoogleAPIOauth2(string refresh_token)
        {
            this.refresh_token = refresh_token;
        }
        
        public void GetCode(OauthUI UI, object owner = null)
        {
            state = randomDataBase64url(32);
            code_verifier = randomDataBase64url(32);
            code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
            string scopepara = Scope.GetParameters(this.scopes);

            redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
            string authorizationRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}&state={4}&code_challenge={5}&code_challenge_method={6}",
                authorizationEndpoint,scopepara,Uri.EscapeDataString(redirectURI), APPkey.ClientID,state,code_challenge,code_challenge_method);

            listener = new HttpListener();
            listener.Prefixes.Add(redirectURI);
            try
            {
                listener.Start();
                UI.Show(owner);
                listener.BeginGetContext(new AsyncCallback(RecieveCode), null);
            }
            catch(Exception ex)
            {
                tokencode.IsError = true;
                MessageBox.Show("redirectURI:" + redirectURI +"\r\n" + ex.Message + "\r\n" + ex.StackTrace);
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
            //listener.Close();
            if (ls.Request.QueryString.Get("error") != null | ls.Request.QueryString.Get("code") == null | ls.Request.QueryString.Get("state") == null)
            {
                this.tokencode.IsError = true;
                try { listener.Close(); }catch { }
                throw new Exception(ls.Request.RawUrl);
            }
            tokencode.Code = ls.Request.QueryString.Get("code");
            var incoming_state = ls.Request.QueryString.Get("state");
            if(incoming_state != state)
            {
                this.tokencode.IsError = true;
                try { listener.Close(); } catch { }
                throw new Exception(ls.Request.RawUrl);
            }
            GetToken(code_verifier, redirectURI);
        }

        void GetToken(string code_verifier, string redirectURI)
        {
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                this.tokencode.Code, System.Uri.EscapeDataString(redirectURI), APPkey.ClientID, code_verifier, APPkey.Clientsecret);

            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            using (Stream stream = tokenRequest.GetRequestStream())
            {
                stream.Write(_byteVersion, 0, _byteVersion.Length);
                stream.Flush();
            }
            WebResponse tokenResponse = tokenRequest.GetResponse();
            using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
            {
                string responseText = reader.ReadToEnd();
                dynamic json = JsonConvert.DeserializeObject(responseText);
                this.tokencode.access_token = json.access_token;
                this.tokencode.refresh_token = json.refresh_token;
                this.tokencode.id_token = json.id_token;
                if (string.IsNullOrEmpty(this.tokencode.access_token)) throw new ArgumentNullException(this.tokencode.access_token);
                if (string.IsNullOrEmpty(this.tokencode.refresh_token)) throw new ArgumentNullException(this.tokencode.refresh_token);
                TokenCallBack.Invoke(tokencode);
            }
        }

        public TokenGoogleDrive RefreshToken()
        {
            if (string.IsNullOrEmpty(this.refresh_token)) throw new Exception("refresh_token can't be null");
            string tokenRequestBody = string.Format("client_id={0}&client_secret={1}&refresh_token={2}&grant_type=refresh_token",
                                                    APPkey.ClientID, APPkey.Clientsecret,this.refresh_token);
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            using (Stream stream = tokenRequest.GetRequestStream())
            {
                stream.Write(_byteVersion, 0, _byteVersion.Length);
                stream.Flush();
            }
            try
            {
                WebResponse tokenResponse = tokenRequest.GetResponse();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    dynamic json = JsonConvert.DeserializeObject(responseText);
                    TokenGoogleDrive token = new TokenGoogleDrive();
                    token.refresh_token = this.refresh_token;
                    token.access_token = json.access_token;
                    token.expires_in = json.expires_in;
                    token.id_token = json.id_token;
                    token.Code = this.tokencode.Code;
                    return token;
                }
            }
            catch (Exception ex)
            {
                this.Error_refresh_token = ex.Message;
                return null;
            }
        }

        #region SubMethod
        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            try
            {
                listener.Start();
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }
        
        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        public static string randomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return base64urlencodeNoPadding(bytes);
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        /// <param name="inputStirng"></param>
        /// <returns></returns>
        public static byte[] sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
        #endregion
    }
}