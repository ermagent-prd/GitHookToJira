using JiraTools.Container;
using JiraTools.Parameters;
using SvnToJira.Engine;
using SvnToJira.Parameters;
using SvnTools.Container;
using SvnTools.Parameters;
using System.Collections.Generic;
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
            container.RegisterInstance<ReleaseInfoParamContainer>(new ReleaseInfoParamContainer(parameters.ReleaseBranches));

            container.RegisterType<ConfigurationLoader>();

            container.RegisterType<PropertiesToCommentEngine>();

            container.RegisterType<TrackingIssueCheckEngine>();

            container.RegisterType<TrackingIssueChecker>();

            container.RegisterType<BranchCheckerEngine>();

            container.RegisterType<TrackingIssuePropertiesChecker>();

            container.AddNewExtension<JiraToolsContainerExtension>();
            container.AddNewExtension<SvnToolsContainerExtension>();

            return container;
        }
    }
}
