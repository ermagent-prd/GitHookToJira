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

        public void Execute(Issue jiraIssue, string reporter, string accountdefault)
        {
            if (reporter != "")
            {
                jiraIssue.Reporter = this.accountEngine.Execute(reporter, accountdefault).AccountId;
                jiraIssue.SaveChanges();
            }
        }


        #endregion

        #region Private methods


        #endregion
    }
}
