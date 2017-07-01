using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Cloud.GoogleDrive.Oauth
{
    public class GoogleAPIOauth2: OauthV2
    {
        #region Value
        string Error_refresh_token = "";
        const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";
        const string LoopbackCallback = "http://localhost:{0}/authorize/";
        string[] default_scopes = { Scope.Drive, Scope.DriveFile, Scope.DriveMetadata, Scope.DriveAppdata };

        string state;
        string code_verifier;
        string code_challenge;
        const string code_challenge_method = "S256";
        public string ErrorRefreshToken
        {
            get { return this.Error_refresh_token; }
        }

        TokenGoogleDrive token = new TokenGoogleDrive();
        public TokenGoogleDrive GetToken
        {
            get { return token; }
        }
        #endregion
        public GoogleAPIOauth2() { GoogleDriveAppKey.Check(); }
        public GoogleAPIOauth2(string[] scopes):this()
        {
            this.default_scopes = scopes;
        }
        public GoogleAPIOauth2(TokenGoogleDrive token):this()
        {
            this.token = token;
        }

        public void GetCode(IOauth ui, object owner)
        {
            state = randomDataBase64url(32);
            code_verifier = randomDataBase64url(32);
            code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
            string scopepara = Scope.GetParameters(this.default_scopes);

            redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
            authorizationRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}&state={4}&code_challenge={5}&code_challenge_method={6}",
                authorizationEndpoint, scopepara, Uri.EscapeDataString(redirectURI), GoogleDriveAppKey.ClientID, state, code_challenge, code_challenge_method);

            ui.EventUriResponse += Ui_EventUriResponse;
            GetCode_(ui, owner);//,new HttpListenerContextRecieve(Rev));
        }

        private void Ui_EventUriResponse(Uri uri)
        {
            var querys = HttpUtility.ParseQueryString(uri.Query);
            if(querys.Get("error") != null || querys.Get("code") == null || querys.Get("state") == null || querys.Get("state") != state)
            {
                this.token.IsError = true;
                throw new Exception(uri.ToString());
            }
            token.Code = querys.Get("code");
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
               this.token.Code, System.Uri.EscapeDataString(redirectURI), GoogleDriveAppKey.ClientID, code_verifier, GoogleDriveAppKey.Clientsecret);

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
                this.token.access_token = json.access_token;
                this.token.refresh_token = json.refresh_token;
                this.token.id_token = json.id_token;
                if (string.IsNullOrEmpty(this.token.access_token)) throw new ArgumentNullException(this.token.access_token);
                if (string.IsNullOrEmpty(this.token.refresh_token)) throw new ArgumentNullException(this.token.refresh_token);
                ReturnToken(responseText);
            }
        }
        
        public TokenGoogleDrive RefreshToken()
        {
            if (string.IsNullOrEmpty(token.refresh_token)) throw new Exception("refresh_token can't be null");
            string tokenRequestBody = string.Format("client_id={0}&client_secret={1}&refresh_token={2}&grant_type=refresh_token",
                                                    GoogleDriveAppKey.ClientID, GoogleDriveAppKey.Clientsecret, token.refresh_token);
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
                    TokenGoogleDrive new_token = JsonConvert.DeserializeObject<TokenGoogleDrive>(reader.ReadToEnd());
                    token.access_token = new_token.access_token;
                    token.expires_in = new_token.expires_in;
                    token.id_token = new_token.id_token;
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