using System;
using System.Linq;
using JiraTools.Engine;
using SvnTools;

namespace SvnToJira.Engine
{
    internal class PropertiesToCommentEngine
    {
        #region Private properties

        private readonly RevisionPropertiesEngine svnEngine;

        private readonly PublicAddCommentEngine jiraEngine;

        #endregion

        #region Constructor

        public PropertiesToCommentEngine(RevisionPropertiesEngine svnEngine, PublicAddCommentEngine jiraEngine)
        {
            this.svnEngine = svnEngine;
            this.jiraEngine = jiraEngine;
        }

        #endregion

        #region Public methods

        public void Execute(
            int svnCommit)
        {
            var properties = this.svnEngine.Execute(svnCommit);

            if (properties == null)
                return;

            if (properties.TrackingIssues == null || !properties.TrackingIssues.Any())
                return;

            string body = string.Format("Commit {0} from {1}: {2}",
                properties.Revision.ToString(),
                properties.Author,
                properties.Log);

            foreach(var issue in properties.TrackingIssues)
            {
                this.jiraEngine.Execute(issue, properties.Author, body);
            }
        }

        #endregion

        #region Private methods


        #endregion
    }
}
