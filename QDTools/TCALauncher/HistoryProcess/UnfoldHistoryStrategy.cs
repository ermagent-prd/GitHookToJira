using System.Collections.Generic;

namespace TCALauncher.HistoryProcess
{
    internal class UnfoldHistoryStrategy : IHistoryProcessingStrategy
    {
        IHistoryExportBuilder exportBuilder;

        public UnfoldHistoryStrategy(IHistoryExportBuilder exportBuilder)
        {
            this.exportBuilder = exportBuilder;
        }

        public void Process(IEnumerable<IProcessHistory> histories)
        {
            foreach (ProcessHistory history in histories)
                exportBuilder.BuildHistory(history);

            exportBuilder.Build();
        }
    }
}
