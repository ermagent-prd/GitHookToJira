using ElasticTools.Parameters;
using Nest;
using System;

namespace ElasticTools.Service
{
    public class ServiceManagerContainer
    {
        private readonly Lazy<ElasticClient> svc;

        public ServiceManagerContainer(IElasticToolsParameters parContainer)
        {
            var settings = new ConnectionSettings(new Uri(parContainer.ServerUrl))
                .DefaultIndex(parContainer.IndexName);

            this.svc = new Lazy<ElasticClient>(() => new ElasticClient(settings));
        }

        public ElasticClient Service { get { return this.svc.Value; } }
    }
}
