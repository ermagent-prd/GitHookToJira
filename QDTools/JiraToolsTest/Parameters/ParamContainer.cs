

using JiraTools.Parameters;

namespace JiraToolsTest.Parameters
{
    public class ParamContainer : IJiraToolsParameters
    {
        public string ServerUrl => JiraTestConstants.JiraUrl;

        public string User => JiraTestConstants.User;

        public string Token => JiraTestConstants.Token;

        public string IssueApi => JiraTestConstants.IssueApi;

        public int MaxIssuesPerRequest => JiraTestConstants.MaxIssuesPerRequest;

        public string AttachmentPath => JiraTestConstants.AttachmentPath;
    }
}
