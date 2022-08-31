using ElasticTools.Container;
using ElasticTools.Parameters;
using JiraTools.Container;
using JiraTools.Parameters;
using KpiEngine.Engine;
using KpiEngine.Engine.Csv;
using KpiEngine.Engine.Elastic;
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
            container.RegisterInstance<ISvnToolsParameters>(new SvnParamContainer(parameters.SvnParameters));
            container.RegisterInstance<IJiraToolsParameters>(new JiraParamContainer(parameters.JiraParameters));
            container.RegisterInstance<IElasticToolsParameters>(new ElasticParamContainer(parameters.ElasticParameters));
            
            container.RegisterInstance<IKpiCoreParametersContainer>(new KpiCoreParametersContainer(parameters.KpiCoreParameters));

            container.AddNewExtension<JiraToolsContainerExtension>();
            container.AddNewExtension<SvnToolsContainerExtension>();

            container.RegisterType<IProjectReleaseLoopEngine, ProjectReleaseLoopEngine>(); 
            container.RegisterType<IJiraReleasesLoader, JiraReleasesLoader>();
            container.RegisterType<ITestEfficacyKpiEngine, TestEfficacyKpiEngine>();
            container.RegisterType<IReleasesKpiEnginesExecutor, ReleasesKpiEnginesExecutor>();
            container.RegisterType<ICsvExportEngine, CsvExportEngine>();
            container.RegisterType<ElasticExporterEngine>();

            container.RegisterType<IKpiEvaluatorEngine, KpiEvaluatorEngine>();

            container.RegisterType<IMainEngine, MainEngine>();

            return container;
        }
    }
}
