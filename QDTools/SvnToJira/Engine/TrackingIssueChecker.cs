﻿using JiraTools.Engine;
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


            //2. check issue jira (bug, fixed version inclusa nel committ,  statucategory in progress,...)  
            if (issue==null)
            {
                throw new NotImplementedException("Undefined issue...");
            }
            if(issue.Type=="Bug")
            {
                foreach(var release in committedReleases)
                {
                       if(issue.FixVersions==release.ReleaseName.ToUpper())
                       {
                           break;
                       }
                }
            }
            else
            {
                throw new NotImplementedException("This issue does not refer to a Bug type...");
            }
            return new ActionResult(true, "Corrispondenza...");
        }

        #endregion
    }
}
