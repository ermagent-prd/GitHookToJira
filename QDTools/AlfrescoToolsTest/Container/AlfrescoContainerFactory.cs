using AlfrescoTools.Container;
using AlfrescoTools.Parameters;
using AlfrescoToolsTest.Parameters;
using Unity;

namespace AlfrescoToolsTest.Container
{
    public static class AlfrescoContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IAlfrescoToolsParameters, ParamContainer>();
            container.AddNewExtension<AlfrescoToolsContainerExtension>();                        
            return container;
        }
    }
}
