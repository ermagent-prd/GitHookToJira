using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using JiraTools.Engine;
using JiraTools.Model;

namespace GeminiToJira.Engine
{
    public class AddWatchersEngine
    {
        #region Private properties

        private readonly JiraAccountIdEngine accountEngine;

        private readonly AddWatcherEngine watcherEngine;


        #endregion

        #region Constructor
        public AddWatchersEngine(JiraAccountIdEngine accountEngine, AddWatcherEngine watcherEngine)
        {
            this.accountEngine = accountEngine;
            this.watcherEngine = watcherEngine;
        }

        #endregion

        #region Public methods

        public void Execute(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            if (geminiIssue.Watchers != null && geminiIssue.Watchers.Count > 0)
            {
                jiraIssue.Watchers = new List<string>();

                foreach (var resource in geminiIssue.Watchers)
                {
                    var account = accountEngine.Execute(resource.Fullname, null);

                    if (account != null)
                        jiraIssue.Watchers.Add(account.AccountId);
                }
            }
        }



        #endregion
    }
}
