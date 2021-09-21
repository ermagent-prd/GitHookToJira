using System.Collections.Generic;
using System.Linq;

namespace TCALauncher.HistoryProcess
{
    internal partial class AggregateHistoryStrategy : IHistoryProcessingStrategy
    {
        private readonly IAggregationInfoExport exporter;
        private readonly List<AggregationInfo> aggregations;

        public AggregateHistoryStrategy(IAggregationInfoExport exporter)
        {
            aggregations = new List<AggregationInfo>();
            this.exporter = exporter;
        }

        #region Interface implementation

        public void Process(IEnumerable<IProcessHistory> histories)
        {

            //aggregations.Add(GetProcessCompleted(histories));
            aggregations.Add(GetTCAChecksOk(histories));
            aggregations.Add(GetTCARegressions(histories));
            aggregations.Add(GetScriptFail(histories));
            aggregations.Add(GetSubmissionFail(histories));
            aggregations.Add(GetTCALaunchFail(histories));
            aggregations.Add(GetTCACheckFail(histories));
            aggregations.Add(GetExportFail(histories));
            aggregations.Add(GetGeminiFail(histories));
            aggregations.Add(GetProcessSkipped(histories));

            exporter.Export(aggregations);
        }

        #endregion

        #region Private methods

        private AggregationInfo GetProcessCompleted(IEnumerable<IProcessHistory> histories)
        {
            IEnumerable<string> idsCompleted =
                Filter(histories, ProcessPhaseId.GeminiUpdated, true);

            return new AggregationInfo("Execution completed", idsCompleted, idsCompleted.Count(), histories.Count());
        }

        private AggregationInfo GetTCAChecksOk(IEnumerable<IProcessHistory> histories)
        {
            IEnumerable<string> noTcaRegressions =
                Filter(histories, ProcessPhaseId.CheckTCADone, true, TCALauncherConstants.OK);

            return new AggregationInfo("No regressions", noTcaRegressions, noTcaRegressions.Count(), histories.Count());
        }

        private AggregationInfo GetTCARegressions(IEnumerable<IProcessHistory> histories)
        {
            IEnumerable<string> tcaRegressions =
                Filter(histories, ProcessPhaseId.CheckTCADone, true, TCALauncherConstants.TCA_REGR_FOUND);

            return new AggregationInfo("Regressions found", tcaRegressions, tcaRegressions.Count(), histories.Count());
        }

        private AggregationInfo GetScriptFail(IEnumerable<IProcessHistory> histories)
        {
            IEnumerable<string> scriptFailed =
                Filter(histories, ProcessPhaseId.ScriptExecuted, false);

            return new AggregationInfo("Script fails", scriptFailed, scriptFailed.Count(), histories.Count());
        }

        private AggregationInfo GetSubmissionFail(IEnumerable<IProcessHistory> histories)
        {
            IEnumerable<string> submissionFailed =
                Filter(histories, ProcessPhaseId.PlanSubmitted, false)
                .Union(Filter(histories, ProcessPhaseId.PlanExecutionDone, false));

            return new AggregationInfo("Submission fails", submissionFailed, submissionFailed.Count(), histories.Count());
        }

        private AggregationInfo GetTCALaunchFail(IEnumerable<IProcessHistory> histories)
        {
            IEnumerable<string> tcaLaunchFailed =
                Filter(histories, ProcessPhaseId.LaunchingTCA, false)
                .Union(Filter(histories, ProcessPhaseId.TCACompleted, false));

            return new AggregationInfo("TCA launch fails", tcaLaunchFailed, tcaLaunchFailed.Count(), histories.Count());
        }

        private AggregationInfo GetTCACheckFail(IEnumerable<IProcessHistory> histories)
        {
            IEnumerable<string> tcaCheckFailed =
                Filter(histories, ProcessPhaseId.CheckTCADone, false);

            return new AggregationInfo("TCA check fails", tcaCheckFailed, tcaCheckFailed.Count(), histories.Count());
        }

        private AggregationInfo GetExportFail(IEnumerable<IProcessHistory> histories)
        {
            IEnumerable<string> exportFailed =
                Filter(histories, ProcessPhaseId.ExportDone, false);

            return new AggregationInfo("Regression exports fails", exportFailed, exportFailed.Count(), histories.Count());
        }

        private AggregationInfo GetGeminiFail(IEnumerable<IProcessHistory> histories)
        {
            IEnumerable<string> geminiCallFailed =
                Filter(histories, ProcessPhaseId.GeminiUpdated, false);

            return new AggregationInfo("Gemini call fails", geminiCallFailed, geminiCallFailed.Count(), histories.Count());
        }

        private AggregationInfo GetProcessSkipped(IEnumerable<IProcessHistory> histories)
        {
            IEnumerable<string> idsCompleted =
                Filter(histories, ProcessPhaseId.Skipped, true);

            return new AggregationInfo("Skipped", idsCompleted, idsCompleted.Count(), histories.Count());
        }

        IEnumerable<string> Filter(IEnumerable<IProcessHistory> histories, ProcessPhaseId phaseId, bool phasePassed)
        {
            return
                histories
                .Where(h => h.Phases.Any(p => p.PhaseId.Equals(phaseId) && p.PhasePassed == phasePassed))
                .Select(h => h.ProcessId);
        }

        IEnumerable<string> Filter(IEnumerable<IProcessHistory> histories, ProcessPhaseId phaseId, bool phasePassed, int phaseExitCode)
        {
            return
                histories
                .Where(h => h.Phases.Any(p => p.PhaseId.Equals(phaseId) && p.PhasePassed == phasePassed && p.ExitCode == phaseExitCode))
                .Select(h => h.ProcessId);
        }

        #endregion
    }
}
