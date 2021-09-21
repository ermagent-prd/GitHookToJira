namespace TCALauncher.HistoryProcess
{
    internal interface IHistoryExportBuilder
    {
        void BuildHistory(IProcessHistory history);

        void Build();
    }
}
