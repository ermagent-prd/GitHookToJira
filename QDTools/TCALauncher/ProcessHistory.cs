using System.Collections.Generic;
using System.Diagnostics;

namespace TCALauncher
{
    internal class ProcessHistory : IProcessHistory
    {
        #region Private methods

        private List<IProcessPhase> history;
        private object lockObj;

        #endregion

        #region Properties

        public string ProcessId { get; }
        public IEnumerable<IProcessPhase> Phases => history;

        #endregion

        #region Constructor

        public ProcessHistory(string processId)
        {
            ProcessId = processId;
            history = new List<IProcessPhase>();
            lockObj = new object();
        }

        #endregion

        #region Public methods

        public void Add(IProcessPhase phase)
        {
            lock(lockObj) // not strictly needed, but just in case, dealing with Tasks
                history.Add(phase);

            Log(phase);
        }

        #endregion

        #region Private methods
        
        private void Log(IProcessPhase phase)
        {
            int exitCode = 
                phase.ExitCode ?? TCALauncherConstants.OK;

            TraceEventType traceEventType =
                phase.PhasePassed ?
                TraceEventType.Information :
                TraceEventType.Error;

            Program.Tracer.TraceEvent(traceEventType, exitCode, $"{ProcessId} {phase.ToString()}");
        }

        #endregion

    }
}
