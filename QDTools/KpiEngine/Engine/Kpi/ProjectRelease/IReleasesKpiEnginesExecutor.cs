using KpiEngine.Models;
using System;
using System.Collections.Generic;

namespace KpiEngine.Engine
{
    internal interface IReleasesKpiEnginesExecutor
    {
        IEnumerable<KpiOutput> Execute(JiraProjectRelease release, Boolean stopOnFailure);
    }
}
