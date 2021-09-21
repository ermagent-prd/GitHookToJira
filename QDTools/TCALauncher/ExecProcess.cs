using System.Diagnostics;

namespace TCALauncher
{
    internal class ExecProcess
    {
        private string commandFilePath;
        private bool redirectOutput;

        public ExecProcess(string commandFilePath, bool redirectOutput)
        {
            this.commandFilePath = commandFilePath;
            this.redirectOutput = redirectOutput;
        }

        public int Execute(string args)
        {
            Process p = 
                new Process();

            p.StartInfo.FileName = commandFilePath;
            p.StartInfo.Arguments = args;
            p.StartInfo.UseShellExecute = false;

            p.StartInfo.RedirectStandardOutput = redirectOutput;
            p.StartInfo.RedirectStandardError = redirectOutput;
            
            p.Start();
            p.WaitForExit();

            return p.ExitCode;
        }
    }
}
