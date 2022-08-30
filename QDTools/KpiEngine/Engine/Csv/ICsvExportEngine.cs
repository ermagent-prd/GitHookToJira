using KpiEngine.Models;
using System.Collections.Generic;

namespace KpiEngine.Engine.Csv
{
    internal interface ICsvExportEngine
    {
        void Execute(string csvPath, IEnumerable<KpiOutput> kpiResult);
    }
}