using System.Collections.Generic;

namespace SvnToJira.Parameters
{
    public class ReleaseInfoParamContainer 
    {
        public ReleaseInfoParamContainer(IEnumerable<ReleasesBranchInfo> param)
        {
            this.ReleasesToCheck = param;
        }

        public IEnumerable<ReleasesBranchInfo> ReleasesToCheck { get; }


    }
}
