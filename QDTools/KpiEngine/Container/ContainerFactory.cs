using JiraTools.Container;
using JiraTools.Parameters;
using KpiEngine.Engine.TestEfficacy;
using KpiEngine.Parameters;
using SvnTools.Container;
using SvnTools.Parameters;
using Unity;

namespace KpiEngine.Container
{
    internal static class ContainerFactory
    {
        public static IUnityContainer Execute(
            KpiEngineParameters parameters)
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<KpiEngineParameters>(parameters);

            container.RegisterInstance<ISvnToolsParameters>(new SvnParamContainer(parameters.SvnParameters));
            container.RegisterInstance<IJiraToolsParameters>(new JiraParamContainer(parameters.JiraParameters));
            
            container.AddNewExtension<JiraToolsContainerExtension>();
            container.AddNewExtension<SvnToolsContainerExtension>();

            container.RegisterType<ITestEfficacyKpiEngine, TestEfficacyKpiEngine>();


            return container;
        }
    }
}
