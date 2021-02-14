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

namespace GeminiToJira
{
    class Program
    {   
        private static JiraAccountIdEngine accountEngine;

        static void Main(string[] args)
        {
            var unityContainer = ContainerFactory.Execute();

            Stopwatch timer = new Stopwatch();
            Console.WriteLine("["+ DateTime.Now + "] Started...");
            

            #region Development

            //string projectCode = "ER";
            string jiraProjectCode = "EIB";
            var components = new List<String> { "ILIAS", "ILIAS-STA", "BSM", "Other" };
            var developmentEngine = unityContainer.Resolve<ImportDevelopmentEngine>();
            timer.Start();
            developmentEngine.Execute(jiraProjectCode, components);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] Development imported in " + timer.Elapsed);

            #endregion

            #region UAT

            var uatEngine = unityContainer.Resolve<ImportUatEngine>();
            timer.Restart();
            uatEngine.Execute(jiraProjectCode);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] UAT imported in " + timer.Elapsed);

            #endregion

            #region BUG

            var bugEngine = unityContainer.Resolve<ImportBugEngine>();
            timer.Restart();
            bugEngine.Execute(jiraProjectCode);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] BUG imported in " + timer.Elapsed); 
            
            #endregion

            Console.WriteLine("Press a key to close");
            Console.ReadLine();
        }
    }
}
