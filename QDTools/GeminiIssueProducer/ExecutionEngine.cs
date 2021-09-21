using Countersoft.Gemini.Api;
using GeminiIssueProducer.Parameters;

namespace GeminiIssueProducer
{
    public class ExecutionEngine
    {
        private readonly string geminiUsername, geminiPassword;
        
        public ExecutionEngine(string geminiUsername, string geminiPassword)
        {
            this.geminiUsername = geminiUsername;
            this.geminiPassword = geminiPassword;
        }


        public int Run(GeminiIssueProducerOptions commandOption, IssueParams packedParameters)
        {
            ServiceManager serviceManager =
                BuildServiceManager();

            IGeminiCommandOutput runCheck =
                new RunCheckEngine()
                .Execute(serviceManager, packedParameters);

            if (!runCheck.Result)
                return runCheck.ErrorCode;

            SetUserId(serviceManager, packedParameters);

            var commandFactory =
                new GeminiCommandFactory(serviceManager);

            IGeminiCommand command =
                commandFactory.Get(commandOption);

            if (command == null)
                return GeminiConstants.ERR_WRONG_OPTION;

            return
                command
                .Execute(packedParameters)
                .ErrorCode;
        }

        private ServiceManager BuildServiceManager()
        {
            return new ServiceManager(
               Properties.Settings.Default.GeminiURL,
               geminiUsername,
               geminiPassword, 
               string.Empty, 
               Properties.Settings.Default.UseWindowsAuthentication);
        }

        private void SetUserId(ServiceManager serviceManager, IssueParams issueParams)
        {
            issueParams.FixedParams.SetUserId(
                serviceManager.User.WhoAmI().Entity.Id);
        }
    }
}
