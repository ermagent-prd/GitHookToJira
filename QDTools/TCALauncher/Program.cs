using McMaster.Extensions.CommandLineUtils;
using Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TCALauncher
{
    class Program
    {
        internal static TraceSource Tracer = null;

        #region Command line options

        [Option("-v|--verbose", CommandOptionType.NoValue, Description = "Print parameters")]
        public bool Verbose { get; }

        [Option("-e|--planning-executable", CommandOptionType.SingleValue, Description = "Planning Manager executable (full path)")]
        [Required]
        public string PlanningManagerExe { get; }

        [Option("-p|--planning-list", CommandOptionType.SingleValue, Description = "Planning list file (full path)")]
        [Required]
        public string PlanFileFullPath { get; }

        [Option("-t|--tca-executable", CommandOptionType.SingleValue, Description = "TCA executable (full path)")]
        [Required]
        public string TCAExe { get; }

        [Option("-d|--tca-ini-folder", CommandOptionType.SingleValue, Description = "TCA ini folder (full path)")]
        [Required]
        public string TCAIniFolder { get; }

        [Option("-l|--log", CommandOptionType.SingleValue, Description = "Log file (full path)")]
        [Required]
        public string LogFileFullPath { get; }

        [Option("-r|--time-range", CommandOptionType.SingleValue, Description = "Planning Manager check lag (seconds)")]
        [Required]
        public int TimeRangeSecs { get; }

        [Option("-s|--sleep", CommandOptionType.SingleValue, Description = "TCA check interval (millisecs)")]
        [Required]
        public int TimeSleepMs { get; }

        [Option("-x|--script-folder", CommandOptionType.SingleValue, Description = "Cleaning scripts folder (full path)")]
        [Required]
        public string ScriptsFolder { get; }

        [Option("-b|--build", CommandOptionType.SingleValue, Description = "Pipeline build version")]
        [Required]
        public string Build { get; }

        [Option("-f|--report-folder", CommandOptionType.SingleValue, Description = "TCA error reports folder (full path)")]
        [Required]
        public string ReportsFolder { get; }

        [Option("-g|--gemini-username", CommandOptionType.SingleValue, Description = "Gemini username")]
        [Required]
        public string GeminiUsername { get; }

        [Option("-w|--gemini-password", CommandOptionType.SingleValue, Description = "Gemini password")]
        [Required]
        public string GeminiPassword { get; }

        #endregion

        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        public int OnExecute()
        {
            try
            {
                Tracer =
                    InitTracing();

                PrintParameters();

                (bool terminate, IEnumerable<WholeProcessInfo> planningList, int retCode) =
                    GetPlanningList();

                if (terminate)
                    return retCode;

                Task<int>[] planTasks =
                     AsRunningTasks(planningList);

                try
                {
                    Task.WaitAll(planTasks); //timeout ?
                }
                catch (AggregateException)
                {
                    foreach (Task<int> planTask in planTasks.Where(t => t.IsFaulted))
                        Tracer.TraceEvent(TraceEventType.Error, TCALauncherConstants.FAIL, $"{planTask.Exception.InnerException.Message}");
                }

                LogHistory(
                    planningList.Select(p => p.History));

                return EvalTasks(planTasks);
            }
            finally
            {
                Tracer.Flush();
                Tracer.Close();
            }
        }

        #region Private methods

        private TraceSource InitTracing()
        {
            ITraceSourceFactory traceSourceFactory =
                new WithFileLogTraceSourceFactory(LogFileFullPath);

            return traceSourceFactory.Get();
        }

        private void PrintParameters()
        {
            if (!Verbose)
                return;

            Tracer.TraceInformation($"PlanningManagerExe: {PlanningManagerExe}");
            Tracer.TraceInformation($"PlanFileFullPath: {PlanFileFullPath}");
            Tracer.TraceInformation($"TCAExe: {TCAExe}");
            Tracer.TraceInformation($"TCAIniFolder: {TCAIniFolder}");
            Tracer.TraceInformation($"LogFileFullPath: {LogFileFullPath}");
            Tracer.TraceInformation($"TimeRangeSecs: {TimeRangeSecs}");
            Tracer.TraceInformation($"TimeSleepMs: {TimeSleepMs}");
            Tracer.TraceInformation($"PreconditionScriptFolder: {ScriptsFolder}");
            Tracer.TraceInformation($"Build: {Build}");
            Tracer.TraceInformation($"ReportsFolder: {ReportsFolder}");
            Tracer.TraceInformation($"GeminiUsername: {GeminiUsername}");
            Tracer.TraceInformation($"GeminiPassword: {GeminiPassword}");
        }

        private ExecutionParameters PackParameters()
        {
            return
                new ExecutionParameters()
                {
                    PlanningExe = this.PlanningManagerExe,
                    TCAExe = this.TCAExe,
                    TimeRangeSecs = this.TimeRangeSecs,
                    TimeSleepMs = this.TimeSleepMs,
                    Build = this.Build,
                    ReportsFolder = this.ReportsFolder,
                    GeminiUsername = this.GeminiUsername,
                    GeminiPassword = this.GeminiPassword
                };
        }

        private (bool terminate, IEnumerable<WholeProcessInfo> planningList, int retCode) GetPlanningList()
        {
            var planReader =
                    new PlanningInfoReader(PlanFileFullPath);

            IEnumerable<WholeProcessInfo> planningList = null;

            try
            {
                planningList =
                    planReader.Execute(TCAIniFolder, ScriptsFolder, ReportsFolder);

                if (!planningList.Any())
                {
                    Tracer.TraceInformation(
                        $"No plan to process in file {PlanFileFullPath}");

                    return (terminate: true, planningList: planningList, retCode: TCALauncherConstants.OK);
                }

                return (terminate: false, planningList: planningList, retCode: TCALauncherConstants.OK);
            }
            catch (Exception exc)
            {
                Tracer.TraceInformation(
                    $"Exception reading file {PlanFileFullPath}: {exc.Message}");

                return (terminate: true, planningList: planningList, retCode: TCALauncherConstants.PLAN_FILE_READ_ERR);
            }
        }

        private Task<int>[] AsRunningTasks(IEnumerable<WholeProcessInfo> plannings)
        {
            ExecutionParameters execParameters =
                PackParameters();

            return plannings
                    .Select(
                        p =>
                        p.Execute(execParameters))
                    .ToArray(); // Forces query materialization!
        }

        private void LogHistory(IEnumerable<IProcessHistory> histories)
        {
            var historyProcessLogger =
                new HistoryProcessLogger(ReportsFolder);

            historyProcessLogger.Run(histories);
        }

        private int EvalTasks(Task<int>[] tasks)
        {
            if (tasks.Any(t => t.IsFaulted || t.IsCanceled || t.Result != TCALauncherConstants.OK))
            {
                Tracer.TraceEvent(TraceEventType.Error, TCALauncherConstants.FAIL, "Execution terminated with failures");
                return TCALauncherConstants.FAIL;
            }

            Tracer.TraceInformation("Execution terminated successfully");
            return TCALauncherConstants.OK;
        }

        #endregion
    }
}
