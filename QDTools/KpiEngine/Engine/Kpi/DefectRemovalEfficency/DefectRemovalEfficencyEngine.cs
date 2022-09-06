using JiraTools.Engine;
using KpiEngine.Engine.Elastic;
using KpiEngine.Models;
using QDToolsUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KpiEngine.Engine.TestEfficacy
{
    internal class DefectRemovalEfficencyEngine : KpiBaseEngine , IDefectRemovalEfficencyEngine
    {
        private const string kpiKey = "Kpi0002";

        private const string kpiDescription = "Defect Removal Efficancy (DRE)";

        private readonly JqlGetter jqlGetter;

        private readonly IElasticChecker elasticSearch;

        public DefectRemovalEfficencyEngine(JqlGetter jqlGetter, IElasticChecker elasticSearch)
            :base()
        {
            this.jqlGetter = jqlGetter;
            this.elasticSearch = elasticSearch; 
        }

        protected override KpiOutput KernelExecute(KpiInput input)
        {
            var releaseDate = input.JiraRelease.ReleaseDate;

            
            string jsqlFixed = "project = \"{0}\" and issuetype = Bug and affectedVersion = \"{1}\" AND createdDate <= {2} and status = Fixed and \"Fixing Date[Date]\" <= {3}";

            string fixedJsql = String.Format(
                jsqlFixed,
                input.JiraRelease.Project,
                input.JiraRelease.Id,
                releaseDate.ToString("yyyy-MM-dd"),
                releaseDate.ToString("yyyy-MM-dd"));

            string jsqlReleaseAllDefect = "project = \"{0}\" and issuetype = Bug and affectedVersion = \"{1}\" AND createdDate <= {2} and status != \"Not a bug\"";


            string allJsql = String.Format(
                jsqlReleaseAllDefect,
                input.JiraRelease.Project,
                input.JiraRelease.Id,
                input.JiraRelease.ReleaseDate.ToString("yyyy-MM-dd"));


            var fixedDefects = this.jqlGetter.Execute(fixedJsql);

            var allDefects = this.jqlGetter.Execute(allJsql);

            double postCount = fixedDefects.Count();
            double preCount = allDefects.Count();

            double? kpiResult = preCount > 0 ?
                    postCount /
                    preCount :
                    new double?();

            var kpiValue = getKpiKeyValue(input, kpiResult);

            var processResult = (kpiResult.HasValue) ?
                new ProcessResult(
                        ExecutionResult.Ok,
                        String.Empty) :
                new ProcessResult(
                        ExecutionResult.Error,
                        "Kpi not evaluable: release bugs not found");

            return new KpiOutput(
                this.getKpiInfo(), 
                processResult,
                kpiValue);

        }

        #region Private methods

        protected override bool checkEvaluation(KpiInput input)
        {
            //Elastic store check
            var isStored = this.elasticSearch.Execute(getUniqueKey(getKpiInfo(), getKpiKeys(input)));
            if (isStored)
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

        #endregion
    }
}
