

using SvnTools.Parameters;
using System.Collections.Generic;

namespace SvnToJira.Parameters
{
    public class ReleaseInfoParamContainer 
    {
        public ReleaseInfoParamContainer(IEnumerable<ReleasesBranchInfo> param)
        {
            this.Param = param;
        }

        public IEnumerable<ReleasesBranchInfo> Param { get; }


    }
}
