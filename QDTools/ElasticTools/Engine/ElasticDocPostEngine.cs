using ElasticTools.Service;
using Nest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElasticTools.Engine
{
    public class ElasticDocPostEngine
    {
        private readonly ServiceManagerContainer client;

        public ElasticDocPostEngine(ServiceManagerContainer requestFactory)
        {
            this.client = requestFactory;
        }

        public BulkResponse Execute<TDocument>(IEnumerable<TDocument> docs) where TDocument : class
        {
            var task =  this.IndexMany<TDocument>(docs);

            task.Wait();

            return task.Result; 

        }

        private async Task<BulkResponse> IndexMany<TDocument>(IEnumerable<TDocument> docs) where TDocument : class
        {
            return await this.client.Service.IndexManyAsync<TDocument>(docs);
        }


    }
}
