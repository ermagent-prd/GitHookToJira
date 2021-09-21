using System.Diagnostics;

namespace TCALauncher
{
    internal interface ITraceSourceFactory
    {
        TraceSource Get();
    }
}
