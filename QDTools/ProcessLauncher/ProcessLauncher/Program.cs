using System;
using System.Diagnostics;
using System.Linq;

namespace ProcessLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Error: No arguments provided");
                Console.WriteLine("Usage:");
                Console.WriteLine("AppPath [arg1] [arg2] [arg3]");

                Environment.Exit(1);
            }
                

            var arguments = args.ToList();
            
            Console.WriteLine("Arguments:");
            Console.WriteLine(String.Join(" ", arguments));

            var appPath = arguments.FirstOrDefault();

            if (String.IsNullOrWhiteSpace(appPath))
            {
                Console.WriteLine("app path not specified");

                Environment.Exit(1);
            }
                

            arguments.RemoveAt(0);

            var startInfo = new ProcessStartInfo(appPath);
            startInfo.WindowStyle = ProcessWindowStyle.Normal;

            startInfo.Arguments = String.Join(" ", arguments);

            var process = Process.Start(startInfo);

            if (process == null)
            {
                Console.WriteLine(String.Format("Process {0} has not started", startInfo.FileName));

                Environment.Exit(1);
            }

            process.WaitForExit();
        }
    }
}
