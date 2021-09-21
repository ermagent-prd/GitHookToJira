using System.Collections.Generic;

namespace TCALauncher
{
    internal interface IProcessHistory
    {
        IEnumerable<IProcessPhase> Phases { get; }
        string ProcessId { get; }

        void Add(IProcessPhase phase);
    }
}