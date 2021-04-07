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

            var fromProjectCode = "MOD";
            var fromProjectName = "Modeling Platform";

            var destProjectCode = "ESMP";
            var destProjectName = "ERM SHL Modeling Platform";

            var type = " Epic";
            //var type = " Task";
            //var type = " Story";
            //var type = "Bug";
            //var type = " Sub-task";


            importEngine.Execute(fromProjectCode, destProjectCode, fromProjectName, destProjectName, type);

            //jqlSearch = "project = \"ERM-OTHERS-ILIAS-BSM\" and key = EOIB-6479 order by created ASC";
            //
            //importEngine.Execute(fromProject, destProject, jqlSearch);

            Console.WriteLine("[" + DateTime.Now + "] Finished");
            Console.WriteLine("Press a key to close");
            Console.ReadLine();
        }
    }
}
