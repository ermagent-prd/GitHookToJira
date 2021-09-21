namespace TCALauncher
{
    internal class SubProcessPhase : ProcessPhase
    {
        public string SubId { get; }

        public SubProcessPhase(ProcessPhaseId phaseId, string subId) : this(phaseId, subId, true)
        { }

        public SubProcessPhase(ProcessPhaseId phaseId, string subId, bool phasePassed) : this(phaseId, subId, phasePassed, null, null)
        { }

        public SubProcessPhase(ProcessPhaseId phaseId, string subId, bool phasePassed, string errorDetail, int? exitCode) : base(phaseId, phasePassed,errorDetail, exitCode)
        {
            SubId = subId;
        }

        public override string ToString()
        {
            return $"{SubId} {base.ToString()}"; 
        }
    }

}
