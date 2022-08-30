using JiraTools.Engine;
using KpiEngine.Models;
using System;

namespace KpiEngine.Engine.TestEfficacy
{
    internal class TestEfficacyKpiEngine : ITestEfficacyKpiEngine
    {
        private const string kpiKey = "Kpi0001";

        private const string description = "Test Efficacy";

        private readonly JqlGetter jqlGetter;

        public TestEfficacyKpiEngine(JqlGetter jqlGetter)
        {
            this.jqlGetter = jqlGetter;
        }

        public KpiOutput Execute(KpiInput input)
        {
            throw new NotImplementedException();
        }
    }
}
