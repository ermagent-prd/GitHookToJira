﻿using Countersoft.Gemini.Commons.Dto;
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
            Console.WriteLine("Started...");

            var unityContainer = ContainerFactory.Execute();

            //string projectCode = "ER";
            string projectCode = "EIB";

            var components = new List<String> { "ILIAS", "ILIAS-STA", "BSM", "Other" };

            var developmentEngine = unityContainer.Resolve<ImportDevelopmentEngine>();
            developmentEngine.Execute(projectCode, components);

            Console.WriteLine("Development imported");

            var fromDate = new DateTime(2020, 08, 27);  //TODO
            var uatEngine = unityContainer.Resolve<ImportUatEngine>();
            uatEngine.Execute(projectCode, fromDate);

            Console.WriteLine("UAT imported");

            var bugEngine = unityContainer.Resolve<ImportBugEngine>();
            bugEngine.Execute(projectCode);

            Console.WriteLine("BUG imported");

            Console.WriteLine("Press a key to close");
            Console.ReadLine();
        }
    }
}
