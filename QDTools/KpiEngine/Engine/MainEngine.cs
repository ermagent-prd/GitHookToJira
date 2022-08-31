using KpiEngine.Engine.Csv;
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
        private readonly ICsvExportEngine csvExporter;

        public MainEngine(
            IKpiEvaluatorEngine kpiEngine,
            ICsvExportEngine csvExporter)
        {
            this.kpiEngine = kpiEngine;
            this.csvExporter = csvExporter;
        }

        public void Execute()
        {
            //1. Kpi Evaluation
            var kpiList = this.kpiEngine.Execute();

            foreach (var k in kpiList)
            {
                Console.WriteLine(string.Format(
                    "Kpi: {0} - {1} --> ({2},{3})",
                    k.KpiInfo.Key,
                    k.KpiInfo.Description,
                    k.KpiValue.Keys.ToString(),
                    k.KpiValue.Value.ToString()));
            }

            //3. kpi to csv
            this.csvExporter.Execute("c:\\temp\\kpi.csv",kpiList);

            //2. kpi To Db

            //3. Kpi to Elk


        }
    }
}
