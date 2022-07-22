using System;
using System.IO;
using Newtonsoft.Json;
using SvnToJira.Container;
using SvnToJira.Engine;
using SvnToJira.Parameters;
using Unity;

namespace SvnToJira
{
    class Program
    {
        /// <summary>
        /// Svn pre and postcommit actions
        /// arguments: 
        /// svnCommit (int) 
        /// actionType (int): 0: Add committ comment to tracking issue, 1: Check tracking issue (bug fixing check)
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid args");
                return;
            }

            var svnCommit = Convert.ToInt32(args[0]);

            var action = Convert.ToInt32(args[1]);

            var cfgLoader = new ConfigurationLoader();

            var cfg = cfgLoader.Execute();

            var unityContainer = ContainerFactory.Execute(cfg);


            var engine = getEngine(unityContainer, action);
           

            engine.Execute(svnCommit);

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
