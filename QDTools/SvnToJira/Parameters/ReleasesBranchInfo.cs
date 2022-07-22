using System;
using System.Collections.Generic;
using System.Linq;
using JiraTools.Parameters;
using SvnTools.Parameters;

namespace SvnToJira.Parameters
{
    public class ReleasesBranchInfo
    {

        #region Public properties

        public string ReleaseName { get; set; }

        public string Path { get; set; }

        #endregion
    }
}
