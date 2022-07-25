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
        public IEnumerable<ReleasesBranchInfo> Execute(
            IEnumerable<ReleasesBranchInfo> checkedBranch,
            IEnumerable<string> commitDiffList)
        {

            throw new NotImplementedException();    
        }
    }
}
