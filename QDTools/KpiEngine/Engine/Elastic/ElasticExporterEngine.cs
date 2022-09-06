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
        private readonly ElasticDocPostEngine postEngine;
        

        public ElasticExporterEngine(
            ElasticDocPostEngine engine,
            IElasticSearchEngine searchEngine)
        {
            this.postEngine = engine;
        }

        public bool Execute(IEnumerable<KpiOutput> kpiResult)
        {
            var validResults = kpiResult
                .Where(r => 
                r.ProcessResult.Result == ExecutionResult.Ok && 
                r.KpiValue.Value.HasValue)
                .Select(r => new KpiElkDocument
                {
                    KpiCode = r.KpiInfo.Key,
                    KpiName = r.KpiInfo.Description,
                    Project = r.KpiValue.Project,
                    KpiUniqueKey = r.KpiValue.UniqueKey,
                    KpiDimensions = String.Join("-", r.KpiValue.Keys.Select(k => k.KeyValue)),
                    ReferenceDate = r.KpiValue.ReferenceDate,
                    KpiValue = r.KpiValue.Value.Value
                });
            if (validResults.Any())
            {
                var bulkResult = this.postEngine.Execute<KpiElkDocument>(validResults);

                return bulkResult.IsValid;
            }

            return true;
        }

    }
}
