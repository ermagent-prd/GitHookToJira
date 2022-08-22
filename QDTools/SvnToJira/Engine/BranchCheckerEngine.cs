using SvnToJira.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvnToJira.Engine
{
    internal class BranchCheckerEngine
    {
        /*public IEnumerable<ReleasesBranchInfo> Execute(
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
        }*/


        public IEnumerable<ReleasesBranchInfo> Execute(
           IEnumerable<ReleasesBranchInfo> checkedBranch,
           IEnumerable<string> commitDiffList)
        {

            var resultList = new List<ReleasesBranchInfo>();

            foreach (var cb in checkedBranch)
            {
                var query =
                           from diffLine in commitDiffList
                           where diffLine.Contains(cb.Path)
                           select cb;

               foreach(var x in query)
               {
                    resultList.Add(x);
                    break;
               }

            }
            return resultList;
        }
    }
}
