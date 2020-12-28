using System;
using System.Net;
using JiraTools.Model;

namespace JiraTools.Engine
{
    internal class JiraRequestFactory
    {
        public HttpWebRequest Execute(string method, RequestInfo reqInfo)
        {
            string restUrl = String.Format("{0}{1}", reqInfo.JiraServerUrl, reqInfo.Api);
            HttpWebRequest request = WebRequest.Create(restUrl) as HttpWebRequest;
            request.Method = method;
            //request.Accept = "application/json";
            //request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Basic " + CredentialFactory.Execute(reqInfo.User, reqInfo.Token));
            request.Headers.Add("Content-Type", "application/json");
            request.Headers.Add("verify", "false");

            return request;



        }
    }
}
