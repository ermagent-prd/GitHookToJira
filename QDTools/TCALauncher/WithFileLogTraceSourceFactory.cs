using System.Diagnostics;
using System.IO;

namespace TCALauncher
{
    internal class WithFileLogTraceSourceFactory : ITraceSourceFactory
    {
        private static readonly string TracesourceName = "TCALauncher";
        private static readonly string LogFileListenerName = "logFile";

        private string logFileFullPath;


        public WithFileLogTraceSourceFactory(string logFileFullPath)
        {
            this.logFileFullPath = logFileFullPath;
        }

        public TraceSource Get()
        {
            var ts =
                new TraceSource(TracesourceName);

            if (!string.IsNullOrEmpty(logFileFullPath))
            {
                // Clean before using: gocd keeps memory of the past files as artifacts
                var logFileListener =
                    new TextWriterTraceListener(new StreamWriter(logFileFullPath, false), LogFileListenerName);
                ts.Listeners.Add(logFileListener);
            }

            return ts;
        }
    }
}
