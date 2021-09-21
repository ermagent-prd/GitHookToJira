using GeminiIssueProducer.Parameters;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel.DataAnnotations;

namespace GeminiIssueProducer
{
    class Program
    {
        #region Command line options


        [Option("-o|--option", CommandOptionType.SingleValue, Description = "Command mode")]
        [Required]
        [OptionValidation]
        public string Option { get; }

        [Option("-v|--verbose", CommandOptionType.NoValue, Description = "Print parameters")]
        public bool Verbose { get; }

        [Option("-g|--gemini-username", CommandOptionType.SingleValue, Description = "Gemini username")]
        [Required]
        public string GeminiUsername { get; }

        [Option("-w|--gemini-password", CommandOptionType.SingleValue, Description = "Gemini password")]
        [Required]
        public string GeminiPassword { get; }

        [Option("-p|--project", CommandOptionType.SingleValue, Description = "Gemini project")]
        [AllowedValues(GeminiConstants.PROJ_ESF_UAT, IgnoreCase = false, ErrorMessage = "Please specify \"" + GeminiConstants.PROJ_ESF_UAT + "\"")]
        [Required]
        public string Project { get; }

        [Option("-y|--type", CommandOptionType.SingleValue, Description = "Gemini type")]
        [AllowedValues(GeminiConstants.TYPE_INTERNAL_UAT, IgnoreCase = false, ErrorMessage = "Please specify \"" + GeminiConstants.TYPE_INTERNAL_UAT + "\"")]
        [Required]
        public string Type { get; }

        [Option("-i|--issue-type", CommandOptionType.SingleValue, Description = "Gemini issue type")]
        [AllowedValues(GeminiConstants.ISSUE_TYPE_REGRESSION, IgnoreCase = false, ErrorMessage = "Please specify \"" + GeminiConstants.ISSUE_TYPE_REGRESSION + "\"")]
        [Required]
        public string IssueType { get; }

        [Option("-b|--build", CommandOptionType.SingleValue, Description = "Affected build")]
        [Required]
        public string Build { get; }

        [Option("-r|--resources", CommandOptionType.SingleValue, Description = "Gemini resources username (comma separated)")]
        [Required]
        public string Resources { get; }

        [Option("-s|--severity", CommandOptionType.SingleValue, Description = "Gemini severity level")]
        [AllowedValues(GeminiConstants.SEVERITY_MEDIUM, IgnoreCase = false, ErrorMessage = "Please specify \"" + GeminiConstants.SEVERITY_MEDIUM + "\"")]
        [Required]
        public string Severity { get; }

        [Option("-f|--functionality", CommandOptionType.SingleValue, Description = "Gemini functionality")]
        [AllowedValues(GeminiConstants.FUNCTIONALITY_ERMAS, IgnoreCase = false, ErrorMessage = "Please specify \"" + GeminiConstants.FUNCTIONALITY_ERMAS + "\"")]
        [Required]
        public string Functionality { get; }

        [Option("-t|--title", CommandOptionType.SingleValue, Description = "Issue title")]
        [Required]
        public string Title { get; }

        [Option("-d|--description", CommandOptionType.SingleValue, Description = "Issue description")]
        [Required]
        public string Description { get; }

        [Option("-a|--attachments", CommandOptionType.SingleValue, Description = "File attachments full paths (comma separated)")]
        public string Attachments { get; } = string.Empty;

        [Option("-c|--comment", CommandOptionType.SingleValue, Description = "Issue comment")]
        [Required]
        public string Comment { get; }

        #endregion

        /// <summary>
        /// add-or-update: checks if an issue exists with the same specified title. Adds a comment and updates the attachment to it if it does.
        /// update-not-closed: updates the comment if an issue exists with the same specified title.
        /// Otherwise creates a new issue.
        /// </summary>
        /// <param name="args">
        /// --option = add-or-update / update-not-closed
        /// [--verbose]
        /// --gemini-username geminiUser
        /// --gemini-password geminiPassword
        /// --project ESF-UAT 
        /// --type "Internal UAT" 
        /// --issue-type Regression 
        /// --build "build number" 
        /// --resources "username1, username2.. usernameN" 
        /// --severity Medium
        /// --functionality ERMAS 
        /// --title title1 
        /// --description description1 
        /// --attachments "fullFilePath1, fullFilePath2.. fullFilePathN" 
        /// --comment comment1
        /// </param>
        /// <returns>The id of the modified issue or negative error code</returns>
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        public int OnExecute()
        {
            PrintParameters();

            if(GeminiIssueProducerOptionsParser.TryParse(Option, out GeminiIssueProducerOptions commandOption))
            {
                IssueParams issueParams =
                    PackParameters();
                
                var mainEngine =
                    new ExecutionEngine(
                        GeminiUsername,
                        GeminiPassword);

                return mainEngine.Run(commandOption, issueParams);
            }

            return GeminiConstants.ERR_WRONG_OPTION;
        }

        private void PrintParameters()
        {
            if (!Verbose)
                return;

            Console.WriteLine($"Option: {Option}");
            Console.WriteLine($"Gemini Username: {GeminiUsername}");
            Console.WriteLine($"Gemini Password: {GeminiPassword}");
            Console.WriteLine($"Project: {Project}");
            Console.WriteLine($"Type: {Type}");
            Console.WriteLine($"IssueType: {IssueType}");
            Console.WriteLine($"Severity: {Severity}");
            Console.WriteLine($"Functionality: {Functionality}");
            Console.WriteLine($"Resources: {Resources}");
            Console.WriteLine($"Title: {Title}");
            Console.WriteLine($"Description: {Description}");
            Console.WriteLine($"Build: {Build}");
            Console.WriteLine($"Attachments: {Attachments}");
            Console.WriteLine($"Comment: {Comment}");
        }

        private IssueParams PackParameters()
        {
            return
                new IssueParams(
                    new IssueFixedParams(
                        Project,
                        Type,
                        IssueType,
                        Severity,
                        Functionality),
                    new IssueFreeTextParams(
                        Resources,
                        Title,
                        Description,
                        Build,
                        Attachments,
                        Comment));
        }
    }
}
