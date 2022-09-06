using ElasticTools.Engine;
using ElasticTools.Service;
using Unity;
using Unity.Extension;

namespace ElasticTools.Container
{
    public class ElasticToolsContainerExtension : UnityContainerExtension
    {
        #region Private properties

        #endregion

        #region Constructor

        #endregion

        #region Public methods

        protected override void Initialize()
        {
            Container.RegisterType<ServiceManagerContainer>();
            Container.RegisterType<ElasticDocPostEngine>();
            Container.RegisterType<IElasticSearchEngine,ElasticSearchEngine>();

        }

        #endregion

        #region Private methods


        #endregion

    }
}