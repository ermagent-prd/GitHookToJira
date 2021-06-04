using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraTools.Engine;
using SvnToJira.Container;
using Unity;

namespace SvnToJira
{
    class Program
    {
        static void Main(string[] args)
        {
            var repoPath = args[0];

            var svnCommit = args[1];

            var unityContainer = ContainerFactory.Execute();

            var commentEngine = unityContainer.Resolve<AddCommentEngine>();

/*
svn log http://erm-codever-p01.prometeia.lan/svn/ErmasNet/trunk -r 145106
------------------------------------------------------------------------
r145106 | gianquintog | 2021-06-04 17:51:08 +0200 (ven, 04 giu 2021) | 1 line

(I/E): UAT-71593
------------------------------------------------------------------------
*/

        }
    }
}
