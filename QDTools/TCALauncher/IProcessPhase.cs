namespace TCALauncher
{
    internal interface IProcessPhase
    {
        string Detail { get; }
        int? ExitCode { get; }
        ProcessPhaseId PhaseId { get; }
        bool PhasePassed { get; }
    }
}