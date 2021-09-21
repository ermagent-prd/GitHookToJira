using GeminiIssueProducer;
using GeminiIssueProducer.Parameters;
using Parameters;
using System;
using TCALauncher;

namespace Results
{
    internal class TCAGeminiAdapter
    {
        #region Private fields

        private readonly ProcessHistory processHistory;

        private readonly ExecutionEngine geminiIssueProcess;

        #endregion

        #region Constructor

        public TCAGeminiAdapter(string geminiUsername, string geminiPassword, ProcessHistory processHistory)
        {
            this.processHistory = processHistory;

            geminiIssueProcess =
                 new ExecutionEngine(
                    geminiUsername,
                    geminiPassword);
        }

        #endregion

        #region Public methods

        public void Execute(string build, GeminiIssueProducerOptions geminiCommand, ExportParameters exportParams)
        {
            IssueParams packedParameters =
                GetIssueParams(build, geminiCommand, exportParams);

            int geminiResult =
                geminiIssueProcess.Run(geminiCommand, packedParameters);

            string detail = null;
            bool phasePassed = false;


            if (geminiResult > 0)   // geminiResult == issue ID
            {
                detail =
                    $"Gemini {geminiCommand} found UAT {geminiResult}: (Title: {packedParameters.FreeParams.Title}, Resource: {exportParams.Owner})";
                phasePassed = true;
            }

            if (geminiResult == 0)
            {
                detail = $"Gemini {geminiCommand}: no UAT found";
                phasePassed = true;
            }

            if (geminiResult < 0)   // geminiResult == error code
            {
                detail = $"Gemini {geminiCommand} returned error {geminiResult}";
                phasePassed = false;
            }

            processHistory.Add(new ProcessPhase(ProcessPhaseId.GeminiUpdated, phasePassed, detail, geminiResult));
        }

        #endregion

        #region Private methods

        private IssueParams GetIssueParams(
            string build,
            GeminiIssueProducerOptions geminiCommand,
            ExportParameters exportParams)
        {
            return new IssueParams(
                GeminiConstants.PROJ_ESF_UAT,
                GeminiConstants.TYPE_INTERNAL_UAT,
                GeminiConstants.ISSUE_TYPE_REGRESSION,
                GeminiConstants.SEVERITY_MEDIUM,
                GeminiConstants.FUNCTIONALITY_ERMAS,
                exportParams.Owner,
                GetTitle(exportParams.IdForTitle),
                GetDescription(exportParams.PlanningCodes, exportParams.TcaDB, exportParams.DBServer),
                build,
                exportParams.ReportFileFullPath,
                GetComment(geminiCommand, build));
        }

        private string GetTitle(string titlePart)
        {
            return $"TCA Regression - [{titlePart}]";
        }

        private string GetDescription(string planningString, string tcaDB, string dbServer)
        {
            return $"TCA regression found for {planningString} planning.{Environment.NewLine}TCA DB: {tcaDB}{Environment.NewLine}DB Server: {dbServer}";
        }

        private string GetComment(GeminiIssueProducerOptions geminiCommand, string build)
        {
            if (geminiCommand == GeminiIssueProducerOptions.AddOrUpdate)
                return $"Regression still present in revision {build}";

            if (geminiCommand == GeminiIssueProducerOptions.UpdateNotClosed)
                return $"Regression solved from revision {build}";

            return string.Empty;
        }

        #endregion
    }
}
