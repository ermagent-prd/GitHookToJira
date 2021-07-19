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

            var cfgKey = ImportCfgType.ILIASBSM;
            GeminiToJiraParameters configurationSetup = Readconfiguration(cfgKey);

            var storyLogFile = configurationSetup.LogDirectory + "ImportLog_" + configurationSetup.JiraProjectCode + "_" + DateTime.Now.ToString("yyyyMMdd-hh_mm") + ".log";

            Trace.AutoFlush = true;
            Trace.Listeners.Add(new TextWriterTraceListener(storyLogFile, "myListener"));

            Stopwatch timer = new Stopwatch();

            var startMessage = "[" + DateTime.Now + "] Started...";

            Console.WriteLine(startMessage);
            Trace.TraceInformation(startMessage);
            

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

            var endLog = "[" + DateTime.Now + "] Finished";


            Trace.TraceInformation(endLog);

            Console.WriteLine(endLog);
          

            //Console.WriteLine("Press a key to close");
            //Console.ReadLine();
        }

        private static GeminiToJiraParameters Readconfiguration(ImportCfgType cfgKey)
        {
            //var cfgManager = new ConfigurationManager();
            var cfgManager = new FileConfigurationManager();
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
            var startMsg = "[" + DateTime.Now + "] Start Development";
            Console.WriteLine(startMsg);
            Trace.TraceInformation(startMsg);
            developmentEngine.Execute(configurationSetup);
            timer.Stop();
            var endMsg = "[" + DateTime.Now + "] Development imported in " + timer.Elapsed;

            Console.WriteLine(endMsg);
            Trace.TraceInformation(endMsg);
            
        }

        private static void ImportUat(GeminiToJiraParameters configurationSetup, IUnityContainer unityContainer, Stopwatch timer)
        {
            var uatEngine = unityContainer.Resolve<ImportUatEngine>();
            timer.Restart();
            var startmsg = "[" + DateTime.Now + "] Start UAT";
            Console.WriteLine(startmsg);
            Trace.TraceInformation(startmsg);
            uatEngine.Execute(configurationSetup);
            timer.Stop();
            var stopMsg = "[" + DateTime.Now + "] UAT imported in " + timer.Elapsed;
            Console.WriteLine("[" + DateTime.Now + "] UAT imported in " + timer.Elapsed);
            Trace.TraceInformation(stopMsg);
        }

        private static void ImportBug(GeminiToJiraParameters configurationSetup, IUnityContainer unityContainer, Stopwatch timer)
        {
            var bugEngine = unityContainer.Resolve<ImportBugEngine>();
            timer.Restart();
            var startMsg = "[" + DateTime.Now + "] Start BUG";
            Console.WriteLine(startMsg);
            Trace.TraceInformation(startMsg);
            bugEngine.Execute(configurationSetup);
            timer.Stop();
            var stopMsg = "[" + DateTime.Now + "] BUG imported in " + timer.Elapsed;
            Console.WriteLine(stopMsg);
            Trace.TraceInformation(stopMsg);

        }
    }
}
