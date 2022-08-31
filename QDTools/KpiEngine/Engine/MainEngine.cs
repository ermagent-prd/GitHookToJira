using KpiEngine.Engine.Csv;
using KpiEngine.Engine.Elastic;
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
        private readonly ElasticExporterEngine elasticExporter;

        public MainEngine(
            IKpiEvaluatorEngine kpiEngine,
            ICsvExportEngine csvExporter,
            ElasticExporterEngine elasticExporter)
        {
            this.elasticExporter = elasticExporter; 
            this.kpiEngine = kpiEngine;
            this.csvExporter = csvExporter;
        }

        public void Execute()
        {
            //1. Kpi Evaluation
            var kpiList = this.kpiEngine.Execute();

            //3. kpi to csv
            this.csvExporter.Execute("c:\\temp\\kpiErmas.csv",kpiList);

            //2. kpi To Db

            //3. Kpi to Elk
            this.elasticExporter.Execute(kpiList);


        }
    }
}
