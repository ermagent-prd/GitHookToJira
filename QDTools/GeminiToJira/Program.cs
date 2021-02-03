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

namespace GeminiToJira
{
    class Program
    {   
        private static JiraAccountIdEngine accountEngine;

        static void Main(string[] args)
        {
            var unityContainer = ContainerFactory.Execute();

            string projectCode = "ER";

            var components = new List<String> { "ILIAS", "ILIAS-STA", "BSM", "Other" };

            var developmentEngine = unityContainer.Resolve<ImportDevelopmentEngine>();
            developmentEngine.Execute(projectCode, components);
            
            //var uatEngine = unityContainer.Resolve<ImportUatEngine>();
            //uatEngine.Execute(projectCode);
            //
            //var bugEngine = unityContainer.Resolve<ImportBugEngine>();
            //bugEngine.Execute(projectCode);
        }
    }
}
