using GeminiIssueProducer;
using Parameters;
using System;
using TCALauncher;
using TCAProcess;

namespace Results
{
    internal class TCAResultEvaluator : ITCAResultEvaluator
    {
        #region Private fields

        private readonly ExecutionParameters exeParams;
        private readonly ExportParameters exportParams;

        private readonly ProcessHistory processHistory;
        private readonly ITCAResultEvaluator exporter;

        #endregion

        #region Constructors

        public TCAResultEvaluator(ExecutionParameters exeParams, ExportParameters exportParams, ProcessHistory processHistory)
        {
            this.exeParams = exeParams;
            this.exportParams = exportParams;
            this.processHistory = processHistory;

            this.exporter =
                new CSVExport(exportParams.ReportFileFullPath);
        }

        #endregion

        #region Public methods

        public void Execute(TCAResultObj tcaObj)
        {
            var geminiCommand =
                GeminiIssueProducerOptions.None;

            if (tcaObj == null || tcaObj.IsEmpty())
                geminiCommand = GeminiIssueProducerOptions.UpdateNotClosed;
            else
            {
                geminiCommand =
                    GeminiIssueProducerOptions.AddOrUpdate;

                if (!Export(tcaObj))
                    return;
            }

            UpdateGemini(geminiCommand);
        }

        #endregion

        #region Private methods

        private bool Export(TCAResultObj tcaObj)
        {
            string filePath = exportParams.ReportFileFullPath;

            string message = null;
            bool phaseDone = false;
            int? exitCode = null;

            try
            {
                exporter.Execute(tcaObj);

                message = $"Check file {filePath} for details";
                exitCode = TCALauncherConstants.TCA_REGR_FOUND;
                phaseDone = true;
            }
            catch (Exception exc)
            {
                message = $"{exc.Message} creating file {filePath}";
                exitCode = TCALauncherConstants.ERR_REPORT_EXP;
                phaseDone = false;
            }

            processHistory.Add(new ProcessPhase(ProcessPhaseId.ExportDone, phaseDone, message, exitCode));
            return phaseDone;
        }

        private void UpdateGemini(GeminiIssueProducerOptions geminiCommand)
        {
            var geminiEngine =
                new TCAGeminiAdapter(exeParams.GeminiUsername, exeParams.GeminiPassword, processHistory);

            geminiEngine.Execute(
                exeParams.Build,
                geminiCommand,
                exportParams);
        }

        #endregion
    }
}
