using System;
using Unity;
using JiraReport.Container;
using JiraReport.Parameters.Export;
using JiraReport.Engine;

namespace JiraReport
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[" + DateTime.Now + "] Started...");

            var cfgKey = ExportReportType.Test;
            ExcelConfiguration configurationSetup = Readconfiguration(cfgKey);

            var unityContainer = ContainerFactory.Execute();
            var exportEngine = unityContainer.Resolve<ExportEngine>();

            try
            {
                var filename = exportEngine.Execute(configurationSetup);
                Console.WriteLine("[" + DateTime.Now + "] Exported " + filename);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("[" + DateTime.Now + "] Failed");
            }
            finally
            {   
                Console.WriteLine("Press a key to close");
                Console.ReadLine();
            }
        }

        private static ExcelConfiguration Readconfiguration(ExportReportType cfgKey)
        {
            var cfgManager = new ConfigurationManager();
            var configurationSetup = cfgManager.Execute(cfgKey);
            return configurationSetup;
        }
    }
}
