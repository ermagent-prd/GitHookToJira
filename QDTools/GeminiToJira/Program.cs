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
            var unityContainer = ContainerFactory.Execute();

            var cfgKey = ImportCfgType.ERM;
            GeminiToJiraParameters configurationSetup = Readconfiguration(cfgKey);

            Stopwatch timer = new Stopwatch();
            Console.WriteLine("[" + DateTime.Now + "] Started...");

            #region Task

            if (configurationSetup.Jira.ImportTask)
                ImportTask(configurationSetup, unityContainer, timer);

            #endregion

            #region Story

            if (configurationSetup.Jira.ImportStory)
                ImportDevelopment(configurationSetup, unityContainer, timer);

            #endregion

            #region UAT

            if (configurationSetup.Jira.ImportUat)
                ImportUat(configurationSetup, unityContainer, timer);

            #endregion

            #region BUG

            if (configurationSetup.Jira.ImportBug)
                ImportBug(configurationSetup, unityContainer, timer);

            #endregion

            Console.WriteLine("[" + DateTime.Now + "] Finished");
            Console.WriteLine("Press a key to close");
            Console.ReadLine();
        }

        private static GeminiToJiraParameters Readconfiguration(ImportCfgType cfgKey)
        {
            var cfgManager = new ConfigurationManager();
            var configurationSetup = cfgManager.Execute(cfgKey);
            return configurationSetup;
        }

        private static void ImportTask(GeminiToJiraParameters configurationSetup, IUnityContainer unityContainer, Stopwatch timer)
        {
            var taskEngine = unityContainer.Resolve<ImportTaskEngine>();
            timer.Start();
            Console.WriteLine("[" + DateTime.Now + "] Start Task");
            taskEngine.Execute(configurationSetup);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] Task imported in " + timer.Elapsed);
        }

        private static void ImportDevelopment(GeminiToJiraParameters configurationSetup, IUnityContainer unityContainer, Stopwatch timer)
        {
            var developmentEngine = unityContainer.Resolve<ImportStoryEngine>();
            timer.Start();
            Console.WriteLine("[" + DateTime.Now + "] Start Development");
            developmentEngine.Execute(configurationSetup);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] Development imported in " + timer.Elapsed);
        }

        private static void ImportUat(GeminiToJiraParameters configurationSetup, IUnityContainer unityContainer, Stopwatch timer)
        {
            var uatEngine = unityContainer.Resolve<ImportUatEngine>();
            timer.Restart();
            Console.WriteLine("[" + DateTime.Now + "] Start UAT");
            uatEngine.Execute(configurationSetup);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] UAT imported in " + timer.Elapsed);
        }

        private static void ImportBug(GeminiToJiraParameters configurationSetup, IUnityContainer unityContainer, Stopwatch timer)
        {
            var bugEngine = unityContainer.Resolve<ImportBugEngine>();
            timer.Restart();
            Console.WriteLine("[" + DateTime.Now + "] Start BUG");
            bugEngine.Execute(configurationSetup);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] BUG imported in " + timer.Elapsed);
        }
    }
}
