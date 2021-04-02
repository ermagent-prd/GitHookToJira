using Atlassian.Jira;
using JiraTools.Parameters;
using JiraTools.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JiraTools.Engine
{
    public class IssueLinkSearchEngine
    {
        #region Private properties

        private readonly ServiceManagerContainer requestFactory;

        private readonly IJiraToolsParameters parameters;

        #endregion

        #region Public

        #endregion

        #region Constructor

        public IssueLinkSearchEngine(
            ServiceManagerContainer requestFactory,
            IJiraToolsParameters parameters)
        {
            this.requestFactory = requestFactory;

            this.parameters = parameters;
        }

        #endregion

        public IEnumerable<IssueLink> Execute(Issue issue)
        {
            var task = GetLinksForIssueAsync(issue, null);

            task.Wait();

            return task.Result;
        }

        public async Task<IEnumerable<IssueLink>> GetLinksForIssueAsync(Issue issue, IEnumerable<string> linkTypeNames = null, CancellationToken token = default(CancellationToken))
        {
            var serializerSettings = requestFactory.Service.RestClient.Settings.JsonSerializerSettings;
            var resource = String.Format("rest/api/2/issue/{0}?fields=issuelinks,created", issue.Key.Value);
            var issueLinksResult = await requestFactory.Service.RestClient.ExecuteRequestAsync(Method.GET, resource, null, token).ConfigureAwait(false);
            var issueLinksJson = issueLinksResult["fields"]["issuelinks"];

            if (issueLinksJson == null)
            {
                throw new InvalidOperationException("There is no 'issueLinks' field on the issue data, make sure issue linking is turned on in JIRA.");
            }

            var issueLinks = issueLinksJson.Cast<JObject>();
            var filteredIssueLinks = issueLinks;

            if (linkTypeNames != null)
            {
                filteredIssueLinks = issueLinks.Where(link => linkTypeNames.Contains(link["type"]["name"].ToString(), StringComparer.InvariantCultureIgnoreCase));
            }

            var issuesToGet = filteredIssueLinks.Select(issueLink =>
            {
                var issueJson = issueLink["outwardIssue"] ?? issueLink["inwardIssue"];
                return issueJson["key"].Value<string>();
            }).ToList();

            var issuesMap = await requestFactory.Service.Issues.GetIssuesAsync(issuesToGet, token).ConfigureAwait(false);
            if (!issuesMap.Keys.Contains(issue.Key.ToString()))
            {
                issuesMap.Add(issue.Key.ToString(), issue);
            }


            return filteredIssueLinks.Select(issueLink =>
            {
                var linkType = JsonConvert.DeserializeObject<IssueLinkType>(issueLink["type"].ToString(), serializerSettings);
                var outwardIssue = issueLink["outwardIssue"];
                var inwardIssue = issueLink["inwardIssue"];
                var outwardIssueKey = outwardIssue != null ? (string)outwardIssue["key"] : null;
                var inwardIssueKey = inwardIssue != null ? (string)inwardIssue["key"] : null;
                return new IssueLink(
                    linkType,
                    outwardIssueKey == null ? issue : issuesMap[outwardIssueKey],
                    inwardIssueKey == null ? issue : issuesMap[inwardIssueKey]);
            });
        }
    }
}
