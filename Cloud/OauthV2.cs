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
        internal string redirectURI;
        internal string authorizationRequest;
        public event DelegateToken TokenCallBack;
        //Process Cloud_Oauth;
        internal void GetCode_(OauthUI ui, object owner)
        {
            if (string.IsNullOrEmpty(authorizationRequest) | string.IsNullOrEmpty(redirectURI)) throw new Exception("Oauth:authorizationRequest or redirectURI is null.");
            //Cloud_Oauth = Process.Start(Directory.GetCurrentDirectory() + "\\Cloud_Oauth.exe", redirectURI);
            //Cloud_Oauth.OutputDataReceived += Cloud_Oauth_OutputDataReceived;
            ui.Url = authorizationRequest;
            ui.CheckUrl = redirectURI;
            ui.ShowUI(owner);
        }

        //private void Cloud_Oauth_OutputDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    throw new Exception(e.Data);
        //}

        internal void ReturnToken(string token)
        {
            TokenCallBack.Invoke(token);
        }
    }
}
