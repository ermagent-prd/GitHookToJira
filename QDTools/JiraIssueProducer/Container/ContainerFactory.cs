using JiraIssueProducer.Engine;
using Unity;

namespace JiraIssueProducer.Container
{
    internal static class ContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<JiraIssueProducerOptionsParser>();
            container.RegisterType<MainEngine>();
            

            //container.AddNewExtension<JiraToolsContainerExtension>();

            return container;
        }
    }
}
