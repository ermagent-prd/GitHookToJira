using SvnToJira.Parameters;
using System.Collections.Generic;

namespace SvnToJira.Engine
{
    internal class BranchCheckerEngine
    {
        public IEnumerable<ReleasesBranchInfo> Execute(
            IEnumerable<ReleasesBranchInfo> checkedBranch,
            IEnumerable<string> commitDiffList)
        {

            var resultList = new List<ReleasesBranchInfo>();

            foreach (var cb in checkedBranch)
            {
                foreach (var diffLine in commitDiffList)
                {
                    if (diffLine.Contains(cb.Path))
                    {
                        resultList.Add(cb);
                        break;
                    }
                }
            }
            return resultList;
        }
    }
}
