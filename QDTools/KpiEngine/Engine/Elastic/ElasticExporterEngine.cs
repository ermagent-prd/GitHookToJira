using ElasticTools.Engine;
using KpiEngine.ElkIndex;
using KpiEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KpiEngine.Engine.Elastic
{
    internal class ElasticExporterEngine
    {
        private readonly ElasticDocPostEngine elasticEngine;

        public ElasticExporterEngine(ElasticDocPostEngine engine)
        {
            this.elasticEngine = engine;
        }

        public void Execute(IEnumerable<KpiOutput> kpiResult)
        {
            var validResults = kpiResult
                .Where(r => r.ProcessResult.Result == ExecutionResult.Ok && r.KpiValue.Value.HasValue)
                .Select(r => new KpiElkDocument
                {
                    KpiCode = r.KpiInfo.Key,
                    KpiName = r.KpiInfo.Description,
                    Project = r.KpiValue.Project,
                    KpiUniqueKey = r.KpiInfo.Key + "-" + String.Join("-", r.KpiValue.Keys.Select(k => k.KeyValue)),
                    KpiDimensions = String.Join("-", r.KpiValue.Keys.Select(k => k.KeyValue)),
                    ReferenceDate = r.KpiValue.ReferenceDate,
                    KpiValue = r.KpiValue.Value.Value
                });

            this.elasticEngine.Execute<KpiElkDocument>(validResults);



        }
    }
}
