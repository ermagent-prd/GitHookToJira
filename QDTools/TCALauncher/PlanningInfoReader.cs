using Parameters;
using PlanProcess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TCALauncher
{
    internal class PlanningInfoReader
    {
        #region Private fields

        private static readonly string PowershellExtension = ".ps1";
        private string filePath;

        #endregion

        #region Constructors

        public PlanningInfoReader(string filePath)
        {
            this.filePath = filePath;
        }

        #endregion

        #region Public methods

        public IEnumerable<WholeProcessInfo> Execute(string tcaDirectoryPath, string scriptDirectoryPath, string reportsDirectoryPath)
        {
            var result =
                new List<WholeProcessInfo>();

            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] pieces = line.Split(';');

                bool toSkip = false;

                if (pieces[0] == "*")
                {
                    toSkip = true;
                    pieces = pieces.Skip(1).ToArray();
                }

                string entity = pieces[0];
                string[] planCodes = pieces[1].Split(',');
                string planMode = pieces[2];
                string tcaDB = pieces[3];
                string tcaIni = pieces[4];
                string dbServer = pieces[5];
                string ermasDB = pieces[6];
                string owner = pieces[7];
                string[] scripts = pieces[8].Split(',');

                if(planCodes.Length == 0)
                {
                    Program.Tracer.TraceEvent(TraceEventType.Warning, TCALauncherConstants.NO_PLAN_LIST, $"Skipping row {line} for missing plan codes");
                    continue;
                }

                List<SinglePlanning> plannings = 
                    ParsePlannings(entity, planCodes, scripts, scriptDirectoryPath);

                result.Add(new WholeProcessInfo(
                    toSkip,
                    new PlanParameters()
                    {
                        PlanningMode = int.Parse(planMode),
                        Plannings = plannings,
                        PostScript = EvalPostScript(planCodes, scripts, scriptDirectoryPath)
                    },
                    new TCAParameters()
                    {
                        TCAIniFile = Path.Combine(tcaDirectoryPath, tcaIni),
                        TCADB = tcaDB,
                        PlanningMode = int.Parse(planMode),
                        DBServer = dbServer,
                        ErmasDB = ermasDB
                    },
                    owner));
            }

            return
                result.Where(x => x != null);
        }

        #endregion

        #region Private methods

        private List<SinglePlanning> ParsePlannings(string entity, string[] codes, string[] scripts, string baseScriptPath)
        {
            List<SinglePlanning> result = null;

            if (scripts.Any(p => !string.IsNullOrWhiteSpace(p)))
            {
                result = new List<SinglePlanning>();

                for (int i = 0; i < codes.Length; i++)
                {
                    if (i < scripts.Length && !string.IsNullOrWhiteSpace(scripts[i]))
                        result.Add(
                            new SinglePlanning(
                                entity,
                                Path.ChangeExtension(Path.Combine(baseScriptPath, scripts[i]), PowershellExtension),
                                codes[i]));
                    else
                        result.Add(
                            new SinglePlanning(entity,codes[i]));
                }
            }
            else
                result =
                    codes
                    .Select(code => new SinglePlanning(entity, code))
                    .ToList();

            return result;
        }

        private string EvalPostScript(string[] codes, string[] scripts, string baseScriptPath)
        {
            if (scripts.Length > codes.Length)
                return Path.ChangeExtension(Path.Combine(baseScriptPath, scripts[codes.Length]), PowershellExtension);

            return null;
        }

        private void LogSkipped(
            string entity,
            string planningCode)
        {
            Program.Tracer.TraceInformation($"[{entity}].[{planningCode}] skipped");
        }

        #endregion
    }
}
