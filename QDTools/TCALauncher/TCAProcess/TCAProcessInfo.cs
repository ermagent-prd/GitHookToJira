using GeminiIssueProducer;
using Parameters;
using System;
using TCALauncher;

namespace TCAProcess
{
    internal class TCAProcessInfo
    {
        #region Private fields

        private readonly TCAParameters tcaParameters;
        private readonly ProcessHistory processHistory;

        #endregion

        #region Constructors

        public TCAProcessInfo(TCAParameters tcaParameters, ProcessHistory processHistory)
        {
            this.tcaParameters = tcaParameters;
            this.processHistory = processHistory;
        }

        #endregion

        #region Public methods

        public TCACheckResult Execute(ExecutionParameters exeParams)
        {
            // Launch TCA
            TCACheckResult launchTCAResult =
                LaunchTCA(exeParams); 

            if (!launchTCAResult.Phase.PhasePassed)
                return launchTCAResult;

            // Check TCA results
            return CheckTCA(exeParams);
        }

        #endregion

        #region Private methods

        private TCACheckResult LaunchTCA(ExecutionParameters exeParams)
        {
            processHistory.Add(new ProcessPhase(ProcessPhaseId.LaunchingTCA));

            int tcaResult = 0;
            bool phasePassed = false;
            string messageDetail = null;
            
            try
            {
                tcaResult = ExecProcessUtilities.Launch(
                    exeParams.TCAExe,
                    $"-TCA -F\\{tcaParameters.TCAIniFile} -TCAPLAN {tcaParameters.PlanningMode}");

                phasePassed = true;
            }
            catch (Exception exc)
            {
                messageDetail = exc.Message;
                tcaResult = TCALauncherConstants.ERR_TCA_EXE;
            }

            var phase = new ProcessPhase(ProcessPhaseId.TCACompleted, phasePassed, messageDetail, tcaResult);

            processHistory.Add(phase);

            return new TCACheckResult(phase, null);
        }

        private TCACheckResult CheckTCA(ExecutionParameters exeParams)
        {
            var queryHelper =
                new TCAQueryHelper(tcaParameters.DBServer, tcaParameters.TCADB);

            var checkTCA = 
                new TCACheckEngine(queryHelper);

            TCACheckResult tcaCheckResult =
                checkTCA.Execute(exeParams.Build, tcaParameters.ErmasDB);

            processHistory.Add(tcaCheckResult.Phase);

            return tcaCheckResult;
        }

        #endregion
    }
}
