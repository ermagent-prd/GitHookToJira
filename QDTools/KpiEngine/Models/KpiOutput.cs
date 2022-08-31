using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Models
{
    internal class KpiOutput
    {
        public KpiOutput(
            KpiInfo kpiInfo,
            ProcessResult executionResult,
            KpiValue kpiValue)
        {
            KpiInfo = kpiInfo;
            ProcessResult = executionResult;
            KpiValue = kpiValue;
        }

        /// <summary>
        /// Kpi info
        /// </summary>
        public KpiInfo KpiInfo { get; }

        public ProcessResult ProcessResult { get; }   

        public KpiValue KpiValue { get; }
    }
}
