using Atlassian.Jira;
using JiraTools.Engine;

namespace GeminiToJira.Engine.Common
{
    public class JiraRemoteLinkerEngine
    {
        #region Private properties
        private readonly URLChecker urlChecker;

        private readonly RemoteLinkEngine linkEngine;

        private readonly AddCommentEngine jiraCommentEngine;

        private readonly ConfigurationContainer config;


        #endregion

        #region Constructor

        public JiraRemoteLinkerEngine(
            URLChecker urlChecker, 
            RemoteLinkEngine linkEngine,
            AddCommentEngine jiraCommentEngine,
            ConfigurationContainer config)
        {
            this.urlChecker = urlChecker;
            this.linkEngine = linkEngine;
            this.jiraCommentEngine = jiraCommentEngine;
            this.config = config;
        }

        #endregion

        #region Public methods

        public void Execute(Issue jiraIssue, string url, string title, string summary = null)
        {
            if (!this.urlChecker.IsValid(url))
            {
                this.jiraCommentEngine.Execute(
                    jiraIssue, 
                    this.config.Configuration.Jira.DefaultAccount,
                    string.Format("{0}: {1}",title,url));

                return;
            }

            this.linkEngine.Execute(jiraIssue, url, title, summary);
        }

        #endregion

    }
}
