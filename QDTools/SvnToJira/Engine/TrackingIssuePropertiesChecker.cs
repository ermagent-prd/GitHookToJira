using JiraTools.Constant;
using SvnToJira.Parameters;
using System;
using System.Collections.Generic;

namespace SvnToJira.Engine
{
    internal class TrackingIssuePropertiesChecker
    {

        #region Public method

        public ActionResult Execute(
            string trackingIssue,
            TrackingIssueToCheckFields issue,
            IEnumerable<ReleasesBranchInfo> committedReleases)
        {
            if (issue == null)
              return new ActionResult(false, String.Format(Messages.JiraIssueNotFound, trackingIssue));

            //Bug Type
            if (issue.IssueTypeId != JiraConstant.BugIssueTypeId)
              return new ActionResult(false, String.Format(Messages.JiraIssueNotABug, trackingIssue));

            return ActionResult.Passed();
        }
        #endregion
    }
}
