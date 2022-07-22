using SvnToJira.Parameters;
using SvnTools;
using System;

namespace SvnToJira.Engine
{
    internal class TrackingIssueCheckEngine : ISvnToJiraEngine
    {
        #region Private properties

        private readonly ReleaseInfoParamContainer releaseParameters;

        private readonly RevisionPropertiesEngine svnEngine;

        private readonly TrackingIssueChecker issueChecker;

        #endregion

        #region Constructor

        public TrackingIssueCheckEngine(
            RevisionPropertiesEngine svnEngine, 
            TrackingIssueChecker issueChecker, 
            ReleaseInfoParamContainer releaseParameters)
        {
            this.releaseParameters = releaseParameters;
            this.svnEngine = svnEngine;
            this.issueChecker = issueChecker;
        }

        #endregion

        #region Public methods

        public void Execute(
            int svnCommit)
        {
            //1. Recupero properties da revision svn

            var properties = this.svnEngine.Execute(svnCommit);

            if (properties == null)
                return;

            /*
            if (properties.TrackingIssues == null || !properties.TrackingIssues.Any())
                return;


            var issue = properties.TrackingIssues.FirstOrDefault();

            if (issue == null)
                return;
            */

            //2. Check sulla release (verifica se il commit ha interessato le release specificate in configurazione (releaseParameters.Param)


            //3. Check sul traking issue: recupero della issue jira, verifica sulle propietà

            throw new NotImplementedException();
        }

        #endregion

        #region Private methods


        #endregion
    }
}
