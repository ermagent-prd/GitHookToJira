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

            var fromProject = "EOIB";
            var destProject = "RMS5";
            var jqlSearch = "project = \"ERM-OTHERS-ILIAS-BSM\" and  key = EOIB-6478 order by created ASC";

            importEngine.Execute(fromProject, destProject, jqlSearch);

            jqlSearch = "project = \"ERM-OTHERS-ILIAS-BSM\" and key = EOIB-6479 order by created ASC";

            importEngine.Execute(fromProject, destProject, jqlSearch);

            Console.WriteLine("[" + DateTime.Now + "] Finished");
            Console.WriteLine("Press a key to close");
            Console.ReadLine();
        }
    }
}
