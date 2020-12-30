using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using JiraTools.Engine;
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

    }
}
