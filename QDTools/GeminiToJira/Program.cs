using System.Collections.Generic;
using Unity;
using GeminiToJira.Container;
using System;
using GeminiToJira.Engine;
using System.Diagnostics;
using GeminiToJira.Parameters.Import;
using System.Reflection;

namespace GeminiToJira
{
    class Program
    {           
        static void Main(string[] args)
        {
            var cfgKey = ImportCfgType.ERM;
            var cfgManager = new ConfigurationManager();
            var configurationSetup = cfgManager.Execute(cfgKey);

            var unityContainer = ContainerFactory.Execute();

            Stopwatch timer = new Stopwatch();
            Console.WriteLine("["+ DateTime.Now + "] Started...");

            #region Development

            var developmentEngine = unityContainer.Resolve<ImportDevelopmentEngine>();
            timer.Start();
            Console.WriteLine("[" + DateTime.Now + "] Start Development");
            developmentEngine.Execute(configurationSetup);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] Development imported in " + timer.Elapsed);

            #endregion

            #region UAT

            var uatEngine = unityContainer.Resolve<ImportUatEngine>();
            timer.Restart();
            Console.WriteLine("[" + DateTime.Now + "] Start UAT");
            //uatEngine.Execute(configurationSetup);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] UAT imported in " + timer.Elapsed);

            #endregion

            #region BUG

            var bugEngine = unityContainer.Resolve<ImportBugEngine>();
            timer.Restart();
            Console.WriteLine("[" + DateTime.Now + "] Start BUG");
            //bugEngine.Execute(configurationSetup);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] BUG imported in " + timer.Elapsed);

            #endregion

            Console.WriteLine("[" + DateTime.Now + "] Finished");
            Console.WriteLine("Press a key to close");
            Console.ReadLine();
        }
    }
}
