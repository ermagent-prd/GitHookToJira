﻿using JiraTools.Engine;
using KpiEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KpiEngine.Engine.TestEfficacy
{
    internal class TestEfficacyKpiEngine : ITestEfficacyKpiEngine
    {
        private const string kpiKey = "Kpi0001";

        private const string kpiDescription = "Test Efficacy";

        private readonly JqlGetter jqlGetter;

        public TestEfficacyKpiEngine(JqlGetter jqlGetter)
        {
            this.jqlGetter = jqlGetter;
        }

        public KpiOutput Execute(KpiInput input)
        {
            try
            {
                if (!checkEvaluation(input))
                    return null;

                string jqlString = "project = \"{0}\" and issuetype = Bug and \"Fixing Date[Date]\" {1} {2} and fixVersion = \"{3}\" and status = Fixed";


                string preReleaseJsql = String.Format(
                    jqlString,
                    input.JiraRelease.Project,
                    "<=",
                    input.JiraRelease.ReleaseDate.ToString("yyyy-MM-dd"),
                    input.JiraRelease.Id);

                string postReleaseJsql = String.Format(
                    jqlString,
                    input.JiraRelease.Project,
                    ">",
                    input.JiraRelease.ReleaseDate.ToString("yyyy-MM-dd"),
                    input.JiraRelease.Id);


                var preReleaseIssues = this.jqlGetter.Execute(preReleaseJsql);

                var postReleaseIssues = this.jqlGetter.Execute(postReleaseJsql);

                double postCount = postReleaseIssues.Count();
                double preCount = preReleaseIssues.Count();

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
                         "Kpi not evaluable: pre release bugs not found");

                return new KpiOutput(
                    this.getKpiInfo(), 
                    processResult,
                    kpiValue);

            }
            catch (Exception ex)
            {
                return new KpiOutput(
                    this.getKpiInfo(),
                    new ProcessResult(
                        ExecutionResult.Error,
                        ex.Message), 
                    null);
            }

        }

        #region Private methods

        private bool checkEvaluation(KpiInput input)
        {
            var months = DateTime.Now.Subtract(input.JiraRelease.ReleaseDate).Days / (365.25 / 12);
            if (months < 3.0)
                return false;
            return true;
        }

        private KpiInfo getKpiInfo()
        {
            return new KpiInfo(kpiKey, kpiDescription);
        }

        private KpiValue getKpiKeyValue(KpiInput input, double? value)
        {
            var kpiKeys = new List<KpiKey>();

            kpiKeys.Add(new KpiKey("Project", input.JiraRelease.Project));
            kpiKeys.Add(new KpiKey("Release", input.JiraRelease.ReleaseName));

            return new KpiValue(kpiKeys, value);

        }

        #endregion
    }
}