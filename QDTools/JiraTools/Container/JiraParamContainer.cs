using JiraTools.Parameters;

namespace JiraTools.Container
{
    public class JiraParamContainer : IJiraToolsParameters
    {
        private JiraToolConfiguration parameters;

        public JiraParamContainer(JiraToolConfiguration parameters)
        {
            this.parameters = parameters;
        }

        public string ServerUrl => parameters.Url;

        public string User => parameters.User;

        public string Token => parameters.Token;

        public string IssueApi => parameters.IssueApi;

        public int MaxIssuesPerRequest => parameters.MaxIssuesPerRequest;

    }
}
