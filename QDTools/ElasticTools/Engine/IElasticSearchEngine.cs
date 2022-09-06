using Nest;
using System;

namespace ElasticTools.Engine
{
    public interface IElasticSearchEngine
    {
        ISearchResponse<TDocument> Execute<TDocument>(Func<QueryContainerDescriptor<TDocument>, QueryContainer> query) where TDocument : class;
    }
}