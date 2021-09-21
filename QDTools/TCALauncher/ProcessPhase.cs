namespace TCALauncher
{
    internal class ProcessPhase : IProcessPhase
    {
        #region Properties

        public ProcessPhaseId PhaseId { get; }

        public bool PhasePassed { get; }

        public string Detail { get; }

        public int? ExitCode { get; }
        
        #endregion

        #region Constructors

        public ProcessPhase(ProcessPhaseId phaseId) : this(phaseId, true)
        { }

        public ProcessPhase(ProcessPhaseId phaseId, bool phasePassed) : this(phaseId, phasePassed, null)
        { }

        public ProcessPhase(ProcessPhaseId phaseId, bool phasePassed, string detail) : this(phaseId, phasePassed, detail, null)
        { }

        public ProcessPhase(ProcessPhaseId phaseId, bool phasePassed, string detail, int? exitCode)
        {
            PhaseId = phaseId;
            PhasePassed = phasePassed;
            Detail = detail;
            ExitCode = exitCode;
        }

        #endregion

        public override string ToString()
        {
            return $"{EvaluatePhasePassed()}{EvaluateDetails()}";
        }

        private string EvaluatePhasePassed()
        {
            return PhasePassed ? PhaseId.Positive : PhaseId.Negative;
        }

        private string EvaluateDetails()
        {
            return string.IsNullOrWhiteSpace(Detail) ? string.Empty : $" - {Detail}";
        }
    }

}
