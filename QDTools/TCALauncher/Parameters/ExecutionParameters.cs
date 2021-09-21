namespace Parameters
{
    internal class ExecutionParameters
    {
        public string PlanningExe { get; set; }
        public string TCAExe { get; set; }
        public int TimeRangeSecs { get; set; }
        public int TimeSleepMs { get; set; }
        public string Build { get; set; }
        public string ReportsFolder { get; set; }
        public string GeminiUsername { get; set; }
        public string GeminiPassword { get; set; }
    }
}
