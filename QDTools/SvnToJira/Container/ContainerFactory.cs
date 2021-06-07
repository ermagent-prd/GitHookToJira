using JiraTools.Container;
using JiraTools.Parameters;
using SvnToJira.Engine;
using SvnToJira.Parameters;
using SvnTools.Parameters;
using Unity;

namespace SvnToJira.Container
{
    internal static class ContainerFactory
    {
        public static IUnityContainer Execute(
            SvnToJiraParameters parameters)
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<SvnToJiraParameters>(parameters);

            container.RegisterInstance<ISvnToolsParameters>(new SvnParamContainer(parameters.SvnParameters));
            container.RegisterInstance<IJiraToolsParameters>(new JiraParamContainer(parameters.JiraParameters));

            container.RegisterType<ConfigurationLoader>();

            container.RegisterType<PropertiesToCommentEngine>();
            
            container.AddNewExtension<JiraToolsContainerExtension>();

            return container;
        }
    }
}
