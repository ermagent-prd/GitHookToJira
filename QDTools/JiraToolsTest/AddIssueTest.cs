using System;
using FromGemini.Jira.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using JiraTools.Engine;
using JiraTools.Model;

namespace JiraToolsTest
{
    [TestClass]
    public class AddIssueTest
    {
        #region private methods

        private RequestInfo getrequestInfo()
        {
            return new RequestInfo
            { 
                Api = Constants.IssueApi,

                User = Constants.User,

                Token = Constants.Token,
                
                JiraServerUrl = Constants.JiraUrl
            };
        }

        #endregion


        [TestMethod]
        public void AddSingleIssue()
        {
            var container = ContainerFactory.Execute();

            var engine = container.Resolve<AddIssueEngineTest>();

            var body = new JiraMinimalIssue();

            body.fields = new Fields();
            body.fields.project = new Project();
            body.fields.project.key = "ER";
            body.fields.issuetype = new Issuetype();
            body.fields.issuetype.name = "Story";
            body.fields.summary = ".Net Api test";
            body.fields.description = "Come summary";

            var requestInfo = getrequestInfo();

            engine.Execute(requestInfo, body);


//            Assert.IsNotNull(response);
        }
    }
}
