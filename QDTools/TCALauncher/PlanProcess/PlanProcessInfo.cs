using Parameters;
using System;
using System.Threading;
using System.Threading.Tasks;
using TCALauncher;

namespace PlanProcess
{
    internal class PlanProcessInfo
    {
        #region Private fields

        private static readonly string PowershellCommand = "powershell.exe";

        private readonly PlanParameters planParameters;
        private readonly ProcessHistory processHistory;

        #endregion

        #region Constructors

        public PlanProcessInfo(PlanParameters planParameters, ProcessHistory processHistory)
        {
            this.planParameters = planParameters;
            this.processHistory = processHistory;
        }

        #endregion

        #region Public methods

        public Task<int> Execute(ExecutionParameters exeParams)
        {
            var tokenSource =
                new CancellationTokenSource();
            CancellationToken cancellationToken =
                tokenSource.Token;

            Task<int> toReturn =
                Task.FromResult(TCALauncherConstants.OK); //Dummy to start the sequence

            //Plans
            planParameters.Plannings.ForEach(
                (planning) =>
                {
                    SinglePlanning current = planning;

                    toReturn =
                        toReturn.ContinueWith(
                            (antecedent) =>
                            {
                                int launchResult =
                                    LaunchAndPoll(current, exeParams);

                                if (launchResult != TCALauncherConstants.OK)
                                    tokenSource.Cancel();

                                cancellationToken.ThrowIfCancellationRequested();
                                return launchResult;
                            },
                            cancellationToken);
                });

            //Post condition script
            toReturn =
                toReturn.ContinueWith(
                    (antecedent) =>
                    {
                        (bool goon, int scriptResult) = 
                            LaunchScript(planParameters.PostScript);

                        return scriptResult;
                    },
                    TaskContinuationOptions.OnlyOnRanToCompletion);

            // Dispose CancellationTokenSource
            toReturn.ContinueWith(
                (antecedent) => tokenSource.Dispose());

            return toReturn;
        }

        #endregion

        #region Private methods

        private int LaunchAndPoll(SinglePlanning currentPlan, ExecutionParameters exeParams)
        {
            (bool goon, int scriptResult) =
                LaunchScript(currentPlan.Script);

            if (!goon)
                return scriptResult;

            int planResult = TCALauncherConstants.ERR_PLAN_EXE;
            string exceptionMessage = null;

            try
            {
                planResult =
                    ExecProcessUtilities.Launch(
                        exeParams.PlanningExe,
                        $"-sub -pln {currentPlan.Code} -entity {currentPlan.Entity} -TCAPLAN {planParameters.PlanningMode}");
            }
            catch (Exception exc)
            {
                exceptionMessage = exc.Message;
                planResult = TCALauncherConstants.ERR_PLAN_EXE;
            }
            
            if (!(planResult == TCALauncherConstants.OK || planResult == TCALauncherConstants.PLAN_RUNNING))
            {
                //Log exception and exit
                processHistory.Add(new SubProcessPhase(ProcessPhaseId.PlanSubmitted, currentPlan.GetId(), false, exceptionMessage, planResult));
                return planResult;
            }

            processHistory.Add(new SubProcessPhase(ProcessPhaseId.PlanSubmitted, currentPlan.GetId(), true));

            return PollPlanningProcess(currentPlan, exeParams); ;
        }

        private int PollPlanningProcess(SinglePlanning current, ExecutionParameters exeParams)
        {
            while (true)
            {
                int checkExitCode = TCALauncherConstants.ERR_PLAN_EXE;

                try
                {
                    checkExitCode =
                        ExecProcessUtilities.Launch(
                        exeParams.PlanningExe,
                        $"-chk -pln {current.Code} -lag {exeParams.TimeRangeSecs} -entity {current.Entity}");
                }
                catch (Exception exc)
                {
                    processHistory.Add(new SubProcessPhase(ProcessPhaseId.PlanExecutionDone, current.GetId(), false, exc.Message, TCALauncherConstants.ERR_PLAN_EXE));
                    return TCALauncherConstants.ERR_PLAN_EXE;
                }

                if (checkExitCode == TCALauncherConstants.PLAN_RUNNING || checkExitCode == TCALauncherConstants.PLAN_WAIT)
                    Thread.Sleep(exeParams.TimeSleepMs);
                else
                {
                    TraceOutput(checkExitCode, current.GetId());
                    return checkExitCode;
                }

            }
        }

        private (bool goon, int exitCode) LaunchScript(string scriptPath)
        {
            if(!string.IsNullOrWhiteSpace(scriptPath))
            {
                try
                {
                    int scriptResult =
                        ExecProcessUtilities.Launch(
                            PowershellCommand,
                            $"-executionpolicy bypass -File {scriptPath}");

                    if (scriptResult != TCALauncherConstants.OK)
                    {
                        processHistory.Add(new SubProcessPhase(ProcessPhaseId.ScriptExecuted, scriptPath, false, null, scriptResult));
                        return (false, scriptResult);
                    }

                    processHistory.Add(new SubProcessPhase(ProcessPhaseId.ScriptExecuted, scriptPath, true));                
                    return (true, scriptResult);
                }
                catch (Exception exc)
                {
                    processHistory.Add(new SubProcessPhase(ProcessPhaseId.ScriptExecuted, scriptPath, false, exc.Message, TCALauncherConstants.ERR_SCRIPT_EXE));
                    return (false, TCALauncherConstants.ERR_SCRIPT_EXE);
                }
            }

            return (true, TCALauncherConstants.OK);
        }

        private void TraceOutput(int value, string processId)
        {
            string message = String.Empty;
            bool processPhaseResult = false;

            switch (value)
            {
                case TCALauncherConstants.OK:
                    processPhaseResult = true;
                    break;
                case TCALauncherConstants.DISPATCH_UNAV:
                    message = "Planning or dispatcher unavailable";
                    break;
                case TCALauncherConstants.PLAN_FAIL:
                    message = "Planning failure";
                    break;
                case TCALauncherConstants.HEAD_UNREACH:
                    message = "Headnode not reachable";
                    break;
                case TCALauncherConstants.ERR_CONNECT_DB:
                    message = "Cannot connect to database";
                    break;
                case TCALauncherConstants.ERR_APP:
                    message = "Internal application error";
                    break;
                default:
                    message = "Unexpected error";
                    break;
            }

            processHistory.Add(new SubProcessPhase(ProcessPhaseId.PlanExecutionDone, processId, processPhaseResult, message, value));
        }

        #endregion
    }
}
