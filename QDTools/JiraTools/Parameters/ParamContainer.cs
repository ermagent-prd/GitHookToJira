

namespace JiraTools.Parameters
{
    public class ParamContainer : IJiraToolsParameters
    {
        public string ServerUrl => Constants.JiraUrl;

        public string User => Constants.User;

        public string Token => Constants.Token;

        public string IssueApi => Constants.IssueApi;

        public int MaxIssuesPerRequest => Constants.MaxIssuesPerRequest;
    }
}
