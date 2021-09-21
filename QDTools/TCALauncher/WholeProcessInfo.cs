using Parameters;
using PlanProcess;
using Results;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TCAProcess;

namespace TCALauncher
{
    internal class WholeProcessInfo
    {
        #region Private fields

        private readonly PlanParameters planParameters;
        private readonly TCAParameters tcaParameters;
        private readonly string planOwner;

        #endregion

        #region Properties

        public ProcessHistory History { get; }

        public bool Skipped { get; }

        #endregion

        #region Constructor

        public WholeProcessInfo(bool skipped, PlanParameters planParameters, TCAParameters tcaParameters, string planOwner)
        {
            Skipped = skipped;

            this.planParameters = planParameters;
            this.tcaParameters = tcaParameters;
            this.planOwner = planOwner;

            History = new ProcessHistory($"[{GetGlobalId()}]");
        }

        #endregion

        #region Public methods

        public Task<int> Execute(ExecutionParameters exeParams)
        {
            if(Skipped)
            {
                History.Add(new ProcessPhase(ProcessPhaseId.Skipped));
                return Task.FromResult(TCALauncherConstants.OK);
            }

            var plan = new PlanProcessInfo(planParameters, History);

            return
                plan
                .Execute(exeParams)
                .ContinueWith(
                        (a) =>
                        {
                            if (a.Result == TCALauncherConstants.OK)
                            {
                                var tca =
                                    new TCAProcessInfo(tcaParameters, History);
   
                                TCACheckResult checkResult =
                                    tca.Execute(exeParams);

                                if(checkResult.Phase.PhasePassed)
                                {
                                    var exportParams = 
                                        PackExportParameters(exeParams);

                                    var resultEvaluator =
                                        new TCAResultEvaluator(exeParams, exportParams, History);
                                    resultEvaluator.Execute(checkResult.Result);
                                }

                                return checkResult.Phase.ExitCode ?? TCALauncherConstants.OK;
                            }

                            return a.Result;
                        }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private ExportParameters PackExportParameters(ExecutionParameters exeParams)
        {
            return new ExportParameters()
            {
                PlanningCodes = GetAsSingleString(planParameters.Plannings),
                ReportFileFullPath = BuildFilePath(exeParams.ReportsFolder, planParameters.Plannings),
                TcaDB = tcaParameters.TCADB,
                DBServer = tcaParameters.DBServer,
                IdForTitle = GetAsSingleString(planParameters.Plannings), // for uniformity with history and file path should be: GetGlobalId(),
                Owner = planOwner
            };
        }

        #endregion

        #region Private methods

        private string BuildFilePath(string reportDirectory, List<SinglePlanning> plannings)
        {
            return 
                Path.Combine(reportDirectory, $"{GetGlobalId()}.csv");
        }

        private string GetAsSingleString(List<SinglePlanning> plannings)
        {
            return string.Join(",", plannings.Select(p => p.Code));
        }

        private string GetGlobalId()
        {
            return tcaParameters.TCADB;
        }

        #endregion
    }
}
