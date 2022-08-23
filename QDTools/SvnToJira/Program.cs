﻿using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using SvnToJira.Container;
using SvnToJira.Engine;
using SvnToJira.Parameters;
using Unity;
using System.ComponentModel.DataAnnotations;

namespace SvnToJira
{
    class Program
    {
        [Option("-r|--revision", CommandOptionType.SingleValue, Description = "Svn Revision Number")]
        [Required]
        public int SvnRevision { get; } = -1;

        [Option("-o|--option", CommandOptionType.SingleValue, Description = "Command option")]
        [OptionValidation]
        [Required]
        public int Option { get; } = 0;


        /// <summary>
        /// Svn pre and postcommit actions
        /// arguments: 
        /// svnCommit (int) 
        /// actionType (int): 0: Add commit comment to tracking issue, 1: Check tracking issue (bug fixing check)
        /// </summary>
        /// <param name="args"></param>
        public static int Main(string[] args)
        {
            return
                CommandLineApplication.Execute<Program>(args);
        }

        public void OnExecute()
        {
            var cfgLoader = new ConfigurationLoader();

            var cfg = cfgLoader.Execute();

            var unityContainer = ContainerFactory.Execute(cfg);

            var engine = getEngine(unityContainer, this.Option);

            var result = engine.Execute(this.SvnRevision);

            if (!result.Ok)
            {
                Console.Error.WriteLine(result.Message);

                Environment.Exit(1);
            }
        }

        private static ISvnToJiraEngine getEngine(IUnityContainer unityContainer, int action)
        {
            switch (action)
            {
                case SvnToJiraConstants.AddJiraComment:
                    { return unityContainer.Resolve<PropertiesToCommentEngine>(); }

                case SvnToJiraConstants.CheckJiraBugFix:
                    return unityContainer.Resolve<TrackingIssueCheckEngine>();

                default: return null;
            }

        }

        private void CreateJson(IUnityContainer unityContainer)
        {
            var obj = new SvnToJiraParameters();
            obj.JiraParameters = new JiraTools.Parameters.JiraToolConfiguration();
            obj.SvnParameters = new SvnTools.Parameters.SvnToolsParameters();

            var jiraParameters = unityContainer.Resolve<JiraParamContainer>();
            var svnParameters = unityContainer.Resolve<SvnParamContainer>();

            obj.JiraParameters.User = jiraParameters.User;
            obj.JiraParameters.IssueApi = jiraParameters.IssueApi;
            obj.JiraParameters.Token = jiraParameters.Token;
            obj.JiraParameters.Url = jiraParameters.ServerUrl;
            obj.JiraParameters.MaxIssuesPerRequest = jiraParameters.MaxIssuesPerRequest;

            obj.SvnParameters.ServerUrl = svnParameters.ServerUrl;
            obj.SvnParameters.TrackingIssuePattern = svnParameters.TrackingIssuePattern;

            var json = JsonConvert.SerializeObject(obj);

            File.WriteAllText(@"c:\tmp\movie.json", json);

        }
    }
}
