using System.Collections.Generic;
using System.Linq;
using TCALauncher;

namespace TCAProcess
{
    internal class TCACheckResult
    {
        public ProcessPhase Phase { get; }
        public TCAResultObj Result { get; }

        public TCACheckResult(bool passed, int exitCode, string logMessage) : this(passed, exitCode, logMessage, null)
        { }

        public TCACheckResult(bool passed, int exitCode, string logMessage, TCAResultObj result) 
            : this(new ProcessPhase(ProcessPhaseId.CheckTCADone, passed, logMessage, exitCode), result)
        { }

        public TCACheckResult(ProcessPhase phase, TCAResultObj result)
        {
            Phase = phase;
            Result = result;
        }
    }

    public class TCAResultObj
    {
        public TCAResultRow Titles { get; }

        public IEnumerable<TCAResultRow> Values { get; }

        public TCAResultObj(TCAResultRow titles, IEnumerable<TCAResultRow> values)
        {
            Titles = titles;
            Values = values;
        }

        public bool IsEmpty()
        {
            return Values == null || Values.Count() == 0;
        }
    }

    public class TCAResultRow
    {
        public IEnumerable<string> Values { get; }

        public TCAResultRow(IEnumerable<string> values)
        {
            Values = values;
        }
    }
}
