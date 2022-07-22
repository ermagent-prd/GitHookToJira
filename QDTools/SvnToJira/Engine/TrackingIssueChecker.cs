using JiraTools.Engine;
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

        public bool Execute()
        {
            
            throw new NotImplementedException();    
        }

        #endregion
    }
}
