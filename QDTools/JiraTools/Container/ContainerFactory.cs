using Unity;

namespace JiraTools.Engine
{
    internal static class ContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<JiraRequestFactory>();
            container.RegisterType<AddIssueEngine>();
            container.RegisterType<AddIssueEngineTest>();
            return container;
        }
    }
}
