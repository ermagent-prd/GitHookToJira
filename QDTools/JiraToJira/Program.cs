using JiraToJira.Container;
using JiraToJira.Engine;
using Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraToJira
{
    class Program
    {
        static void Main(string[] args)
        {
            var unityContainer = ContainerFactory.Execute();

            var importEngine = unityContainer.Resolve<ImportEngine>();

            Console.WriteLine("[" + DateTime.Now + "] Started...");

            var fromProject = "MOD";
            var destProject = "RMS5";
            var jqlSearch = "project = \"Modeling Platform\" and  key = MOD-26 order by created ASC";

            importEngine.Execute(fromProject, destProject, jqlSearch);

            jqlSearch = "project = \"Modeling Platform\" and key = MOD-132 order by created ASC";

            importEngine.Execute(fromProject, destProject, jqlSearch);

            Console.WriteLine("[" + DateTime.Now + "] Finished");
            Console.WriteLine("Press a key to close");
            Console.ReadLine();
        }
    }
}
