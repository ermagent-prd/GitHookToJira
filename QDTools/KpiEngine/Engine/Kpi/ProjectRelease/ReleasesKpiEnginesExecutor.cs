using KpiEngine.Engine.TestEfficacy;
using KpiEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Engine
{
    internal class ReleasesKpiEnginesExecutor : IReleasesKpiEnginesExecutor
    {
        private readonly Lazy<IEnumerable<IKpiEngine>> engines;

        public ReleasesKpiEnginesExecutor(
            ITestEfficacyKpiEngine efficacyEngine)
        {
            this.engines = new Lazy<IEnumerable<IKpiEngine>>(() => { return getEngines(efficacyEngine); }); 
        }

        public IEnumerable<KpiOutput> Execute(JiraProjectRelease release, Boolean stopOnFailure)
        {
            var result = new List<KpiOutput>(); 

            foreach(var engine in this.engines.Value)
            {
                var input = new KpiInput(release);

                var kpiOut = engine.Execute(input);

                if (kpiOut != null)
                    result.Add(kpiOut);

                if (stopOnFailure && kpiOut.ProcessResult.Result == ExecutionResult.Error)
                    break;

            }
            return result;
        }

        private IEnumerable<IKpiEngine> getEngines(ITestEfficacyKpiEngine efficacyEngine)
        {
            var result = new List<IKpiEngine>();
            result.Add(efficacyEngine);
            return result;
        }
    }
}
