using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;

namespace GeminiToJira.Engine.Common
{
    public class ReporterSaveEngine
    {
        #region Private properties

        private readonly JiraAccountIdEngine accountEngine;

        public ReporterSaveEngine(JiraAccountIdEngine accountEngine)
        {
            this.accountEngine = accountEngine;
        }

        #endregion

        #region Constructor

        #endregion

        #region Public methods

        public void Execute(Issue jiraIssue, IssueDto geminiIssue, string accountdefault)
        {
            if (geminiIssue.Reporter != "")
            {
                jiraIssue.Reporter = this.accountEngine.Execute(geminiIssue.Reporter, accountdefault).AccountId;
                jiraIssue.SaveChanges();
            }
        }


        #endregion

        #region Private methods


        #endregion
    }
}
