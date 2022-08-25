using SvnToJira.Parameters;
using SvnTools;
using System;
using System.Linq;


namespace SvnToJira.Engine
{
    internal class TrackingIssueCheckEngine : ISvnToJiraEngine
    {
        #region Private properties

        private readonly ReleaseInfoParamContainer releaseParameters;

        private readonly SvnLookEngine svnEngine;

        private readonly BranchCheckerEngine branchChecker;

        private readonly TrackingIssueChecker issueChecker;

        #endregion


        #region Constructor

        public TrackingIssueCheckEngine(
            SvnLookEngine svnEngine, 
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

        public ActionResult Execute(EngineInput input)
        {
            try
            {
                #region 1. Recupero properties da revision svn

                var commitProperties = this.svnEngine.Execute(
                    input.RepoFolder,
                    input.SvnTransaction);

                if (commitProperties == null)
                    return new ActionResult(false, String.Format("Invalid svn input : Repo Folder: {0}, Transaction {1}", input.RepoFolder,input.SvnTransaction));

                #endregion

                #region 2. Check sulla release (verifica se il commit ha interessato le release specificate in configurazione 

                var committedReleases = branchChecker.Execute(
                    releaseParameters.ReleasesToCheck,
                    commitProperties.Paths);

                if (committedReleases == null || !committedReleases.Any())
                    return ActionResult.Passed();

                #endregion

                #region 3. Check sul tracking issue: recupero della issue jira, verifica sulle propietà

                if (commitProperties.TrackingIssues == null || !commitProperties.TrackingIssues.Any())
                    return new ActionResult(false, "Tracking Issue not specified");

                var issue = commitProperties.TrackingIssues.FirstOrDefault();


                return this.issueChecker.Execute(
                    issue,
                    committedReleases);

                #endregion
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
