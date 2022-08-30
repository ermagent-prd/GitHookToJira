using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Engine
{
    internal class MainEngine : IMainEngine
    {
        private readonly IKpiEvaluatorEngine kpiEngine;

        public MainEngine(IKpiEvaluatorEngine kpiEngine)
        {
            this.kpiEngine = kpiEngine;
        }

        public void Execute()
        {
            //1. Kpi Evaluation
            var kpiList = this.kpiEngine.Execute();

            foreach (var k in kpiList)
            {
                Console.WriteLine(string.Format(
                    "Kpi: {0} - {1} --> ({3},{4})",
                    k.KpiInfo.Key,
                    k.KpiInfo.Description,
                    k.KpiValue.Keys.ToString(),
                    k.KpiValue.Value.ToString()));
            }

            //2. kpi To Db

            //3. Kpi to Elk


        }
    }
}
