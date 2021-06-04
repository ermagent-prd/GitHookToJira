using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraToExcel.Engines;

namespace JiraToExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            var repositoryPath = args[0];

            var svnCommit = args[1];


            var engine = new Main();

            engine.Execute();

        }
    }
}
