using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.GeminiFilter;
using GeminiToJira.Mapper;
using GeminiTools.Items;
using GeminiTools.Container;
using JiraTools.Engine;
using JiraTools.Container;
using System.Collections.Generic;
using System.Linq;
using Unity;
using GeminiToJira.Container;
using Atlassian.Jira;
using JiraTools.Parameters;
using GeminiToJira.Parameters;
using System;
using GeminiToJira.Engine;
using System.Diagnostics;
using System.IO;
using GeminiToJira.Parameters.Import;
using System.Reflection;

namespace GeminiToJira
{
    class Program
    {   
        
        static void Main(string[] args)
        {
            var cfgKey = ImportType.EIB;
            var cfgManager = new ConfigurationManager();
            var fileCfg = cfgManager.Execute(cfgKey, Assembly.GetExecutingAssembly());

            var unityContainer = ContainerFactory.Execute();

            Stopwatch timer = new Stopwatch();
            Console.WriteLine("["+ DateTime.Now + "] Started...");

            #region Development

            //string jiraProjectCode = "ER";
            string jiraProjectCode = "EOIB";
            var components = new List<String> { "ILIAS", "ILIAS-STA", "BSM", "Other" };
            var developmentEngine = unityContainer.Resolve<ImportDevelopmentEngine>();
            timer.Start();
            Console.WriteLine("[" + DateTime.Now + "] Start Development");
            developmentEngine.Execute(jiraProjectCode, components);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] Development imported in " + timer.Elapsed);

            #endregion

            #region UAT

            var uatEngine = unityContainer.Resolve<ImportUatEngine>();
            timer.Restart();
            Console.WriteLine("[" + DateTime.Now + "] Start UAT");
            uatEngine.Execute(jiraProjectCode);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] UAT imported in " + timer.Elapsed);

            #endregion

            #region BUG

            var bugEngine = unityContainer.Resolve<ImportBugEngine>();
            timer.Restart();
            Console.WriteLine("[" + DateTime.Now + "] Start BUG");
            //bugEngine.Execute(jiraProjectCode);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] BUG imported in " + timer.Elapsed); 
            
            #endregion

            Console.WriteLine("Press a key to close");
            Console.ReadLine();
        }
    }
}
