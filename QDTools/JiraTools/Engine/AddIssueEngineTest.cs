using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FromGemini.Jira;
using FromGemini.Jira.Model;
using JiraTools.Model;
using Newtonsoft.Json;

namespace JiraTools.Engine
{
    internal class AddIssueEngineTest
    {

        public void Execute(RequestInfo reqInfo, JiraMinimalIssue issue)
        {
            RunAsync(reqInfo, issue).GetAwaiter().GetResult();
        }

        public async Task<Uri> RunAsync(RequestInfo reqInfo, JiraMinimalIssue issue)
        {

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(reqInfo.JiraServerUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", CredentialFactory.Execute(reqInfo.User, reqInfo.Token));
                
            var result =  await CreateIssueAsync(client, issue, reqInfo.Api);

            return result;

        }

        static async Task<Uri> CreateIssueAsync(HttpClient client, JiraMinimalIssue issue, string api)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                api, 
                issue);

            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
    }
}
