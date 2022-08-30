using KpiEngine.Models;
using System.Collections.Generic;

namespace KpiEngine.Engine
{
    internal interface IProjectReleaseLoopEngine
    {
        IEnumerable<KpiOutput> Execute();
    }
}