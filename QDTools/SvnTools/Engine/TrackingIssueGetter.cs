using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SvnTools.Engine
{
    public class TrackingIssueGetter
    {
        #region Private properties

        #endregion

        #region Constructor

        #endregion

        #region Public methods

        public IEnumerable<string> Execute(
            string pattern,
            string log)
        {
            var matches = from Match match in Regex.Matches(log, pattern)
                          select match.Groups[1].Value;

            return matches;
        }

        #endregion

        #region Private methods


        #endregion
    }
}
