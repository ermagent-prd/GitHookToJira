using ElasticTools.Service;
using Nest;
using System.Collections.Generic;

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
            return this.client.Service.IndexMany<TDocument>(docs);

            /*
             * System.ObjectDisposedException: 'Impossibile accedere a un oggetto eliminato.
Nome oggetto: 'System.Net.Sockets.NetworkStream'.'

             */
        }

    }
}
