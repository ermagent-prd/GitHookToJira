using SvnToJira.Parameters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvnToJiraTest
{
    internal class ReleasesBranchInfoComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return CompareObj((ReleasesBranchInfo)x, (ReleasesBranchInfo)y);
        }

        public int CompareObj(ReleasesBranchInfo x, ReleasesBranchInfo y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null || y == null)
                return -1;

            if (x.Path.Equals(y.Path) && x.ReleaseName.Equals(y.ReleaseName))
                return 0;

            return -1;
        }
    }
}
