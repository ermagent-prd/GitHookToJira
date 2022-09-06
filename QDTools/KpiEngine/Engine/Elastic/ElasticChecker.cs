using ElasticTools.Engine;
using KpiEngine.ElkIndex;
using Nest;
using System;
using System.Linq;

namespace KpiEngine.Engine.Elastic
{
    internal class ElasticChecker : IElasticChecker
    {
        private readonly IElasticSearchEngine searchEngine;

        public ElasticChecker(IElasticSearchEngine searchEngine)
        {
            this.searchEngine = searchEngine;
        }

        public bool Execute(string uniqueKey)
        {
            var query = new Func<QueryContainerDescriptor<KpiElkDocument>, QueryContainer>(
                q => q
                .Match(m => m
                .Field(f => f.KpiUniqueKey)
                .Query(uniqueKey)));

            var result = this.searchEngine.Execute<KpiElkDocument>(query);

            return result.IsValid && result.Documents.Any();
        }
    }
}
