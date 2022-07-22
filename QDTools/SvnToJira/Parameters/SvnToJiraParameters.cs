using System;
using System.Collections.Generic;
using System.Linq;
using JiraTools.Parameters;
using SvnTools.Parameters;

namespace SvnToJira.Parameters
{
    internal class SvnToJiraParameters
    {

        #region Public properties

        public SvnToolsParameters SvnParameters { get; set; }

        public JiraToolConfiguration JiraParameters { get; set; }


        public List<ReleasesBranchInfo> ReleaseBranches { get; set; }


        #endregion
    }
}
