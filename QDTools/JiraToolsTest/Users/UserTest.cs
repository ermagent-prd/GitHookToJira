using System;
using Atlassian.Jira;
using JiraTools.Engine;
using JiraTools.Model;
using JiraToolsTest.Container;
using JiraToolsTest.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace JiraToolsTest
{
    [TestClass]
    public class UserTest
    {

        [TestMethod]
        public void GetUsers()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<UserGetter>();



            var users = engine.Execute();

            Assert.IsNotNull(users);
        }
    }
}
