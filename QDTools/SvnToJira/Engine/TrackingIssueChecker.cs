using JiraTools.Engine;
using SvnToJira.Parameters;
using System.Collections.Generic;

namespace SvnToJira.Engine
{
    internal class TrackingIssueChecker
    {
        #region private properties

        private readonly IssueGetter issueGetter;

        private readonly TrackingIssuePropertiesChecker issueChecker;

        #endregion

        #region constructor

        public TrackingIssueChecker(
            IssueGetter issueGetter,
            TrackingIssuePropertiesChecker issueChecker)
        {
            this.issueGetter = issueGetter;
            this.issueChecker = issueChecker;
        }


        #endregion

        #region Public methods

        public ActionResult Execute(
            string trackingIssue,
            IEnumerable<ReleasesBranchInfo> committedReleases)
        {
            if (string.IsNullOrEmpty(trackingIssue))
                return new ActionResult(false, "Tracking Issue not specified");

            //1. recupero issue jira
            var issue = this.issueGetter.Execute(trackingIssue);

            var issueProperties = issue == null ? 
                null : 
                new TrackingIssueToCheckFields(
                    trackingIssue,
                    issue.Type.Id);  

            //2. check issue jira 
            return this.issueChecker.Execute(
                trackingIssue,
                issueProperties, 
                committedReleases);
        }

        #endregion

    }
}
