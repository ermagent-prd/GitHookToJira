using KpiEngine.Models;
using System.Collections.Generic;

namespace KpiEngine.Engine
{
    internal interface IKpiEvaluatorEngine
    {
        IEnumerable<KpiOutput> Execute();
    }
}