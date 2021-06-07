using System;
using System.Collections.Generic;
using System.Linq;

namespace SvnTools.Parameters
{
    public class SvnToolsParameters
    {

        #region Public methods
        public string ServerUrl { get; set; }

        public string TrackingIssuePattern { get; set; }

        public bool WindowsAuth { get; set; }

        public string User { get; set; }

        public string Password { get; set; }


        #endregion

    }
}
