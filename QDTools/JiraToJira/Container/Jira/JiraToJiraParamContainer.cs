using JiraTools.Parameters;

namespace JiraToJira.Container.Jira
{
    public class JiraToJiraParamContainer : IJiraToolsParameters
    {
        public string ServerUrl => JiraConstants.JiraUrl;

        public string User => JiraConstants.User;

        public string Token => JiraConstants.Token;

        public string IssueApi => JiraConstants.IssueApi;

        public int MaxIssuesPerRequest => JiraConstants.MaxIssuesPerRequest;
    }
}
