using KpiEngine.Models;
using System.Collections.Generic;

namespace KpiEngine.Engine
{
    internal interface IJiraReleasesLoader
    {
        IEnumerable<JiraProjectRelease> Execute();
    }
}