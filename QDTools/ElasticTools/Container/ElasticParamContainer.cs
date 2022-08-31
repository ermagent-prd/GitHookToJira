using ElasticTools.Parameters;

namespace ElasticTools.Container
{
    public class ElasticParamContainer : IElasticToolsParameters
    {
        private ElasticToolConfiguration parameters;

        public ElasticParamContainer(ElasticToolConfiguration parameters)
        {
            this.parameters = parameters;
        }

        public string ServerUrl => parameters.Url;

        public string IndexName => parameters.IndexName;

    }
}
