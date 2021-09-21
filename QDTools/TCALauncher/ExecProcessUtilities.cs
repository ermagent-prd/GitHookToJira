using System;

namespace TCALauncher
{
    internal class ExecProcessUtilities
    {
        public static int Launch(string exePath, string args)
        {
            var planningManager =
                new ExecProcess(exePath, true);

            return planningManager.Execute(args);  //waits for exit
        }
    }
}
