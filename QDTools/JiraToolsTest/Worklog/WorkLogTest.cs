using Atlassian.Jira;
using JiraTools.Engine;
using JiraToolsTest.Container;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace JiraToolsTest
{
    [TestClass]
    public class WorkLogTest
    {

        [TestMethod]
        public void AddWorkLog()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var workLogengine = container.Resolve<AddWorklogEngine>();

            var userEngine = container.Resolve<UserListGetter>();

            string issueKey = "ESIBT-510";

            string author = "Gaetano Gianquinto";

            string groupName = "ERM SHL Team";

            var userList = userEngine.Execute(groupName);

            var userDict = userList.ToDictionary(u => u.DisplayName);

            JiraUser user;
            userDict.TryGetValue(author, out user);

            if (user != null)
                workLogengine.Execute(
                    issueKey, 
                    user.AccountId, 
                    "1d",
                    DateTime.Now,
                    "test"); //non funziona l'attribuzione dello user. Viene settato di default lo user corrente



        }
    }
}
