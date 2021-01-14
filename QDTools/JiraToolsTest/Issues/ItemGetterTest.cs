using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Atlassian.Jira;
using JiraTools.Engine;
using JiraTools.Service;
using JiraToolsTest.Container;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace JiraToolsTest
{
    [TestClass]
    public class SdkTest
    {
        [TestMethod]
        public void Jql_Execute_GetIssues()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<JqlGetter>();

            string jsql = $"Project = \"ER\" and type = \"Story\"";

            var issues = engine.Execute(jsql);

            var list = new List<Issue>();

            foreach(var issue in issues)
            {
                list.Add(issue);
            }

            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void Linq_Execute_GetIssues()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<ItemListGetter>();

            var issues = engine.Execute("ER");

            var list = new List<Issue>();

            foreach (var issue in issues)
            {
                list.Add(issue);
            }

            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void JQL_Execute_GetSingleIssue()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<JqlGetter>();

            string jsql = $"Key = \"ER-5832\"";

            var issues = engine.Execute(jsql);

            var issue = issues.FirstOrDefault();

            Assert.IsNotNull(issue);
        }

        [TestMethod]
        public void Linq_Execute_GetAndModifyAssigneeIssues()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<ItemListGetter>();

            var issues = engine.Execute("ER", "ER-5946");   //ER-5892 is a subtask of ER-5885

            var list = new List<Issue>();

            foreach (var issue in issues)
            {
                list.Add(issue);
            }

            //list[0].Assignee = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            //list[0].SetPropertyAsync("Assignee", "Paolo Luca").Wait();
            //list[0].Assignee = "70121:67b933a3 - 5693 - 47d2 - 82c0 - 3f997f279387";
            //
            //list[0].LinkToIssueAsync("ER-5886", "Subtask").Wait();

            //list[0].CustomFields.First(c => c.Name == "Owner").Values = new string[] { "Paolo Luca" };
            //list[0].CustomFields.Add("Owner", "Paolo Luca");
            //list[0].CustomFields.AddById("customfield_10040", "Paolo Luca");

            list[0].SaveChangesAsync().Wait();

            //var issueManager = ComponentManager.getInstance().getIssueManager()

            Assert.IsTrue(list.Any());
        }
    }
}
