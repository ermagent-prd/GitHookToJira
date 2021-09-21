namespace TCALauncher
{
    public class ProcessPhaseId
    {
        #region static

        public static ProcessPhaseId None { get { return new ProcessPhaseId(0, "None", "None"); } }
        public static ProcessPhaseId Skipped { get { return new ProcessPhaseId(1, "Skipped", "Skipped"); } }
        public static ProcessPhaseId ScriptExecuted { get { return new ProcessPhaseId(2, "Script execution done", "Script execution failed"); } }
        public static ProcessPhaseId PlanSubmitted { get { return new ProcessPhaseId(3, "Plan submitted" ,"Plan submission failed"); } }
        public static ProcessPhaseId PlanExecutionDone { get { return new ProcessPhaseId(4, "Plan execution done", "Plan execution failed"); } }
        public static ProcessPhaseId LaunchingTCA { get { return new ProcessPhaseId(5, "Launching TCA", "Launching TCA failed"); } }
        public static ProcessPhaseId TCACompleted { get { return new ProcessPhaseId(6, "Call TCA completed", "Call TCA failed"); } }
        public static ProcessPhaseId CheckTCADone { get { return new ProcessPhaseId(7, "Check TCA done", "Check TCA failed"); } }
        public static ProcessPhaseId ExportDone { get { return new ProcessPhaseId(8, "Export completed", "Export failed"); } }
        public static ProcessPhaseId GeminiUpdated { get { return new ProcessPhaseId(9, "Gemini call done", "Gemini call failed"); } }

        #endregion

        public int Value { get; }

        public string Positive { get; }
        public string Negative { get; }

        private ProcessPhaseId(int value, string positive, string negative)
        {
            Value = value;
            Positive =  positive;
            Negative = negative;
        }

        public override bool Equals(object obj)
        {
            ProcessPhaseId other = obj as ProcessPhaseId;
            if (other == null)
                return false;

            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return 17 + Value.GetHashCode();
        }
    }
}
