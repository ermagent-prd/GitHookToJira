﻿using SvnToJira.Parameters;
using SvnTools;
using System;
using System.Linq;

namespace SvnToJira.Engine
{
    internal class TrackingIssueCheckEngine : ISvnToJiraEngine
    {
        #region Private properties

        private readonly ReleaseInfoParamContainer releaseParameters;

        private readonly RevisionPropertiesEngine svnEngine;

        private readonly BranchCheckerEngine branchChecker;

        private readonly TrackingIssueChecker issueChecker;

        #endregion

        #region Constructor

        public TrackingIssueCheckEngine(
            RevisionPropertiesEngine svnEngine, 
            TrackingIssueChecker issueChecker, 
            ReleaseInfoParamContainer releaseParameters,
            BranchCheckerEngine branchChecker)
        {
            this.releaseParameters = releaseParameters;
            this.svnEngine = svnEngine;
            this.issueChecker = issueChecker;
            this.branchChecker = branchChecker;
        }

        #endregion

        #region Public methods

        public ActionResult Execute(
            int svnCommit)
        {
            try
            {
                //1. Recupero properties da revision svn

                var commitProperties = this.svnEngine.Execute(svnCommit);

                if (commitProperties == null)
                    return new ActionResult(false, String.Format("Invalid commit revision: {0}",svnCommit));

                if (commitProperties.TrackingIssues == null || !commitProperties.TrackingIssues.Any())
                    return new ActionResult(false, "Tracking Issue not specified");

                var issue = commitProperties.TrackingIssues.FirstOrDefault();

                if (issue == null)
                    return new ActionResult(false, "Tracking Issue not specified");


                //2. Check sulla release (verifica se il commit ha interessato le release specificate in configurazione (releaseParameters.Param)


                //3. Check sul traking issue: recupero della issue jira, verifica sulle propietà


                throw new NotImplementedException();

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