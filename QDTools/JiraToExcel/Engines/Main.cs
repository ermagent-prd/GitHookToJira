using System;
using System.Linq;
using JiraToExcel.Container;
using Unity;

namespace JiraToExcel.Engines
{
    internal class Main
    {
        public void Execute()
        {
            var unityContainer = ContainerFactory.Execute();

            var engine = unityContainer.Resolve<JiraLoaderEngine>();

            string jql = $"Project = \"ER\" and type = \"Story\"";

            var issues = engine.Execute(jql);

            Console.WriteLine(String.Format("Numero issues: {0}", issues.Count()));

            Console.ReadLine();

        }
    }
}
