using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FromGemini.Jira;
using FromGemini.Jira.Model;
using JiraTools.Model;
using Newtonsoft.Json;

namespace JiraTools.Engine
{
    internal class AddIssueEngine
    {
        private readonly JiraRequestFactory requestFactory;

        public AddIssueEngine(JiraRequestFactory requestFactory)
        {
            this.requestFactory = requestFactory;
        }

        public AddIssueResponse Execute(RequestInfo reqInfo, JiraMinimalIssue issue)
        {
            var request = this.requestFactory.Execute("POST", reqInfo);

            string JsonString = JsonConvert.SerializeObject(issue,Formatting.Indented);

            byte[] data = Encoding.UTF8.GetBytes(JsonString);

            using(var requestStream = request.GetRequestStream()) 
            {  
                requestStream.Write(data, 0, data.Length);  
                requestStream.Close();  
            }

            HttpWebResponse response = null;

            using (response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var reader = new StreamReader(response.GetResponseStream());
                    string str = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<AddIssueResponse>(str);
                }
                else
                {
                    request.Abort();
                    return null;
                }
            }
               
        }
    }
}
