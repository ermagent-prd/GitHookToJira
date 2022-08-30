using KpiEngine.Container;
using KpiEngine.Engine;
using KpiEngine.Parameters;
using McMaster.Extensions.CommandLineUtils;
using QDToolsUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace KpiEngine
{
    internal class Program
    {
        #region Parameters

        [Option("-cfg|--configuration", CommandOptionType.SingleValue, Description = "Configuration file path")]
        public string ConfigurationPath { get; } = string.Empty;

        #endregion

        public static int Main(string[] args)
        {
            return
                CommandLineApplication.Execute<Program>(args);
        }

        public void OnExecute()
        {
            try
            {
                var cfgLoader = new ConfigurationLoader();

                var cfg = cfgLoader.Execute<KpiEngineParameters>(this.ConfigurationPath, "KpiEngine.json");

                var unityContainer = ContainerFactory.Execute(cfg);

                var engine = unityContainer.Resolve<IMainEngine>();

                engine.Execute();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }

    }
}
