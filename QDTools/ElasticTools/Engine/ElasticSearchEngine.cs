using ElasticTools.Service;
using Nest;
using System;
using System.Threading.Tasks;

namespace ElasticTools.Engine
{
    public class ElasticSearchEngine : IElasticSearchEngine
    {
        private readonly ServiceManagerContainer client;

        public ElasticSearchEngine(ServiceManagerContainer requestFactory)
        {
            this.client = requestFactory;
        }

        public ISearchResponse<TDocument> Execute<TDocument>(Func<QueryContainerDescriptor<TDocument>, QueryContainer> query) where TDocument : class
        {
            var task = this.SearchByQuery(query);

            task.Wait();

            return task.Result;


        }

        private async Task<ISearchResponse<TDocument>> SearchByQuery<TDocument>(Func<QueryContainerDescriptor<TDocument>, QueryContainer> query) where TDocument : class
        {
            return await this.client.Service.SearchAsync<TDocument>(s => s
                .AllIndices()
                .From(0)
                .Size(10)
                .Query(query)
            );
        }
    }
}
