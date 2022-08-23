using JiraTools.Constant;
using JiraTools.Engine;
using SvnToJira.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvnToJira.Engine
{
    internal class TrackingIssueChecker
    {
        #region private properties

        private readonly IssueGetter issueGetter;

        #endregion

        #region constructor

        public TrackingIssueChecker(IssueGetter issueGetter)
        {
            this.issueGetter = issueGetter;
        }


        #endregion

        #region Public methods

        public ActionResult Execute(
            string trackingIssue,
            IEnumerable<ReleasesBranchInfo> committedReleases)
        {
            if (trackingIssue == null)
                return new ActionResult(false, "Tracking Issue not specified");


            //1. recupero issue jira
            var issue = this.issueGetter.Execute(trackingIssue);


            //2. check issue jira 
            return checkIssue(
                trackingIssue, 
                issue, 
                committedReleases);
        }

        #endregion

        #region Private method

        private ActionResult checkIssue(
            string trackingIssue,
            Atlassian.Jira.Issue issue,
            IEnumerable<ReleasesBranchInfo> committedReleases)
        {
            if (issue == null)
              return new ActionResult(false, String.Format("Jira Issue {0} not found", trackingIssue));

            //Bug Type
            if (issue.Type.Id != JiraConstant.BugIssueTypeId)
              return new ActionResult(false, String.Format("Jira Issue {0} is not a valid bug", trackingIssue));

            return ActionResult.Passed();
        }
        #endregion
    }
}
