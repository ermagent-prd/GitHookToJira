using KpiEngine.Container;
using KpiEngine.Parameters;
using McMaster.Extensions.CommandLineUtils;
using QDToolsUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine
{
    internal class Program
    {
        #region Parameters

        [Option("-o|--option", CommandOptionType.SingleValue, Description = "Command option")]
        [OptionValidation]
        [Required]
        public OptionType Option { get; } = OptionType.None;

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

                throw new NotImplementedException();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }

    }
}
