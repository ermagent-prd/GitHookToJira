using System.Collections.Generic;
using System.IO;
using TCALauncher.HistoryProcess;

namespace TCALauncher
{
    internal class HistoryProcessLogger
    {
        private const string EXPORT_PHASES_NAME = "exportPhases.html";
        private const string EXPORT_STATS_NAME = "exportStats.html";

        private IEnumerable<IHistoryProcessingStrategy> processors;

        public HistoryProcessLogger(string folder)
        {
            processors = InitProcessors(folder);
        }

        public void Run(IEnumerable<IProcessHistory> histories)
        {
            foreach (IHistoryProcessingStrategy p in processors)
                p.Process(histories);
        }

        private IEnumerable<IHistoryProcessingStrategy> InitProcessors(string folder)
        {
            var result = new List<IHistoryProcessingStrategy>();

            result.Add(
                new UnfoldHistoryStrategy(
                    new HTMLHistoryExportBuilder(Path.Combine(folder, EXPORT_PHASES_NAME))));
            result.Add(
                new AggregateHistoryStrategy(
                    new HTMLAggregationInfoExport(Path.Combine(folder, EXPORT_STATS_NAME))));

            return result;
        }
    }
}
