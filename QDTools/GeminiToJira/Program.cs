using GeminiToJira.Container;
using GeminiToJira.Engine;
using GeminiToJira.Engine.Common;
using GeminiToJira.Parameters.Import;
using Unity;

namespace GeminiToJira
{
    class Program
    {           
        static void Main(string[] args)
        {
            var unityContainer = ContainerFactory.Execute();

            var configContainer = new ConfigurationContainer(ImportCfgType.ILIASBSM);

            unityContainer.RegisterInstance<ConfigurationContainer>(configContainer);


            var mainEngine = unityContainer.Resolve<MainEngine>();

            mainEngine.Execute();
        }

    }
}
