using JiraTools.Engine;
using JiraTools.Service;
using Unity;
using Unity.Extension;

namespace JiraTools.Container
{
    public class ContainerExtension : UnityContainerExtension
    {
        #region Private properties

        #endregion

        #region Constructor

        #endregion

        #region Public methods

        protected override void Initialize()
        {
            Container.RegisterType<JqlGetter>();

            Container.RegisterType<ServiceManagerContainer>();
            

        }

        #endregion

        #region Private methods


        #endregion

    }
}