using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Models
{
    internal class KpiOutput
    {
        public KpiOutput(ProcessResult executionResult, KpiValue kpiValue)
        {
            ExecutionResult = executionResult;
            KpiValue = kpiValue;
        }

        /// <summary>
        /// Kpi info
        /// </summary>
        public Kpi KpiInfo { get; }

        public ProcessResult ExecutionResult { get; }   

        public KpiValue KpiValue { get; }
    }
}
