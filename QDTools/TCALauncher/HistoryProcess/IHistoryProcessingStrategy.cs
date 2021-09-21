using System.Collections.Generic;

namespace TCALauncher.HistoryProcess
{
    internal interface IHistoryProcessingStrategy
    {
        void Process(IEnumerable<IProcessHistory> histories);
    }
}
