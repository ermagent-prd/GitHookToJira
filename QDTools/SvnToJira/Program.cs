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
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid args");
                return;
            }

            var svnCommit = args[0];

                      

            var cfgLoader = new ConfigurationLoader();

            var cfg = cfgLoader.Execute();

            var unityContainer = ContainerFactory.Execute(cfg);

            var engine = unityContainer.Resolve<PropertiesToCommentEngine>();

            engine.Execute(Convert.ToInt32(svnCommit));

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
