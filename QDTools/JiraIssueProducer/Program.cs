using System;
using System.ComponentModel.DataAnnotations;
using JiraIssueProducer.Configuration;
using JiraIssueProducer.Container;
using JiraIssueProducer.Engine;
using McMaster.Extensions.CommandLineUtils;
using Unity;

namespace JiraIssueProducer
{
    class Program
    {
        #region Command line options


        [Option("-o|--option", CommandOptionType.SingleValue, Description = "Command mode")]
        [Required]
        public string Option { get; }

        [Option("-p|--jira-project", CommandOptionType.SingleValue, Description = "Jira project")]
        [Required]
        public string Project { get; }

        [Option("-s|--summary", CommandOptionType.SingleValue, Description = "Summary")]
        [Required]
        public string Summary { get; }

        [Option("-d|--description", CommandOptionType.SingleValue, Description = "Issue description")]
        [Required]
        public string Description { get; }

        [Option("-r|--resources", CommandOptionType.SingleValue, Description = "Gemini resources username (comma separated)")]
        [Required]
        public string Resources { get; }

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
        public static int Main(string[] args)
        {
            return   
                CommandLineApplication.Execute<Program>(args);
        }
        

        public int OnExecute()
        {
            var unityContainer = ContainerFactory.Execute();

            var optionParser = unityContainer.Resolve<JiraIssueProducerOptionsParser>();

            if (optionParser.TryParse(Option, out JiraIssueProducerOption commandOption))
            {
                var cmdOptions = getCmdOptions(commandOption);

                var cfgContainer = new ConfigurationContainer();

                cfgContainer.Configuration.CommandLineOptions = cmdOptions;

                unityContainer.RegisterInstance<ConfigurationContainer>(cfgContainer);

                var engine = unityContainer.Resolve<MainEngine>();

                return engine.Execute();
            }

            return CommandOptions.ERR_WRONG_OPTION;
        }

        private CommandLineOptions getCmdOptions(JiraIssueProducerOption commandOption)
        {
            var cmdOptions = new CommandLineOptions();

            cmdOptions.ProducerOption = commandOption;
            
            cmdOptions.JiraProject = this.Project;

            cmdOptions.Summary = this.Summary;

            cmdOptions.Description = this.Description;

            cmdOptions.Watchers = this.Resources;

            cmdOptions.Attachments = this.Attachments;

            cmdOptions.Comment = this.Comment;

            return cmdOptions;

        }

    }
}
