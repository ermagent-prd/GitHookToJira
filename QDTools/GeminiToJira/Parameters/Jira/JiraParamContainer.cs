using JiraTools.Parameters;

namespace GeminiToJira.Parameters
{
    public class JiraParamContainer : IJiraToolsParameters
    {
        public string ServerUrl => JiraConstants.JiraUrl;

        public string User => JiraConstants.User;

        public string Token => JiraConstants.Token;

        public string IssueApi => JiraConstants.IssueApi;

        public int MaxIssuesPerRequest => JiraConstants.MaxIssuesPerRequest;
    }
}
