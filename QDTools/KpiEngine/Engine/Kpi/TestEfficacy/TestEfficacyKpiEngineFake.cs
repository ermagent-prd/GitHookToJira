using JiraTools.Engine;
using KpiEngine.Models;
using QDToolsUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KpiEngine.Engine.TestEfficacy
{
    internal class TestEfficacyKpiEngineFake : KpiBaseEngine , ITestEfficacyKpiEngine
    {
        private const string kpiKey = "Kpi0001";

        private const string kpiDescription = "Test Efficacy";

        private const int monthLag = 3;

        private readonly JqlGetter jqlGetter;

        public TestEfficacyKpiEngineFake(JqlGetter jqlGetter)
            :base()
        {
            this.jqlGetter = jqlGetter;
        }

        protected override KpiOutput KernelExecute(KpiInput input)
        {


            double? kpiResult = 1.99;

            var kpiValue = getKpiKeyValue(input, kpiResult);

            var processResult = (kpiResult.HasValue) ?
                new ProcessResult(
                        ExecutionResult.Ok,
                        String.Empty) :
                new ProcessResult(
                        ExecutionResult.Error,
                        "Kpi not evaluable: pre release bugs not found");

            return new KpiOutput(
                this.getKpiInfo(), 
                processResult,
                kpiValue);

        }

        #region Private methods

        protected override bool checkEvaluation(KpiInput input)
        {
            var lagDate = DateTimeUtilities.AddToDate(DateTime.Now, 0, -monthLag, 0, true);
            if(lagDate < input.JiraRelease.ReleaseDate)
                return false;
            return true;
        }

        protected override DateTime getReferenceDate(KpiInput input)
        {
            return input.JiraRelease.ReleaseDate;
        }

        protected override KpiInfo getKpiInfo()
        {
            return new KpiInfo(kpiKey, kpiDescription);
        }

        protected override IEnumerable<KpiKey> getKpiKeys(KpiInput input)
        {
            var kpiKeys = new List<KpiKey>();

            kpiKeys.Add(new KpiKey("Project", input.JiraRelease.Project));
            kpiKeys.Add(new KpiKey("Release", input.JiraRelease.ReleaseName));

            return kpiKeys;
        }


        private DateTime getPreviousDate(JiraProjectRelease release)
        {
            return DateTimeUtilities.AddToDate(release.ReleaseDate, 0, -monthLag, 0, true);        
        }

        private DateTime getLastDate(JiraProjectRelease release)
        {
            return DateTimeUtilities.AddToDate(release.ReleaseDate, 0, monthLag, 0, true);
        }

        #endregion
    }
}
