using System;
using System.Linq;
using JiraTools.Engine;
using SvnTools;

namespace SvnToJira.Engine
{
    internal class PropertiesToCommentEngine : ISvnToJiraEngine
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

        public ActionResult Execute(EngineInput input)
        {
            try
            {
                var properties = this.svnEngine.Execute(input.SvnRevision);

                if (properties == null)
                    return ActionResult.Passed();

                if (properties.TrackingIssues == null || !properties.TrackingIssues.Any())
                    return ActionResult.Passed();

                string body = string.Format("SVN Commit \n Revision {0} from {1}: \n {2} Repository: {3}",
                    properties.Revision.ToString(),
                    properties.Author,
                    properties.Log,
                    properties.Repo);

                var issue = properties.TrackingIssues.FirstOrDefault();

                if (issue == null)
                    return ActionResult.Passed();

                this.jiraEngine.Execute(issue, properties.Author, body);

                return ActionResult.Passed();
            }
            catch (Exception ex)
            {
                return new ActionResult(false, ex.Message);
            }
        }

        #endregion

        #region Private methods


        #endregion
    }
}
