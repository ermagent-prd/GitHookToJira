using Countersoft.Gemini.Api;
using GeminiIssueProducer.Parameters;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeminiIssueProducer
{
    internal class RunCheckEngine
    {
        public IGeminiCommandOutput Execute(ServiceManager serviceManager, IssueParams issueParams)
        {
            int testConnectionResult =
                TestConnection(serviceManager);

            if (testConnectionResult != GeminiConstants.OK)
                return new SimpleCommandOutput(false, testConnectionResult);

            return new SimpleCommandOutput(true, GeminiConstants.OK);
        }

        private int TestConnection(ServiceManager serviceManager)
        {
            var connectionTask =
               Task.Run(
                   () =>
                   serviceManager.User.WhoAmI());

            connectionTask.ContinueWith(
                    (t) =>
                    { var observed = t.Exception; },
                    TaskContinuationOptions.OnlyOnFaulted);
            try
            {
                if (connectionTask.Wait(Properties.Settings.Default.ConnectionTimeoutSecs * 1000))
                    return GeminiConstants.OK;

                return 
                    GeminiConstants.ERR_TIMEOUT_CONNECTION;
            }
            catch (AggregateException exc)
            {
                return GeminiConstants.ERR_CALL_FAIL;
            }
        }
    }
}
