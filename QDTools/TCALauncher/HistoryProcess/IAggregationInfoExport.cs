using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCALauncher.HistoryProcess
{
    interface IAggregationInfoExport
    {
        void Export(IEnumerable<AggregationInfo> aggregationInfos);
    }
}
