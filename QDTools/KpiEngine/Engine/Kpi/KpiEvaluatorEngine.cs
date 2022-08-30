using KpiEngine.Models;
using System.Collections.Generic;

namespace KpiEngine.Engine
{
    internal class KpiEvaluatorEngine : IKpiEvaluatorEngine
    {
        private IProjectReleaseLoopEngine releaseEngine;

        public KpiEvaluatorEngine(IProjectReleaseLoopEngine releaseEngine)
        {
            this.releaseEngine = releaseEngine;
        }

        public IEnumerable<KpiOutput> Execute()
        {
            var kpiResult = new List<KpiOutput>();

            //1. releases Kpi
            var releasesKpi = this.releaseEngine.Execute();

            kpiResult.AddRange(releasesKpi);

            //2. Other kpis..
            //......

            return kpiResult;
        }
    }
}
