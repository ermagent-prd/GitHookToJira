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
    public class AddIssueTest
    {

        [TestMethod]
        public void AddSingleIssue()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<CreateIssueEngine>();

            var issueInfo = new CreateIssueInfo
            {
                ProjectKey = "ER",
                Summary = "Api call Test (Atlassian SDK)",
                Description = "This is a test " + DateTime.Now.ToString(),
                Priority = "Medium",
                Type = "Story",
                OriginalEstimate = "1w",
                RemainingEstimate = "1d",
                DueDate = new DateTime(2021, 12, 31),
                

                Assignee = "70121:67b933a3-5693-47d2-82c0-3f997f279387"
            };

            issueInfo.FixVersions.Add("ERMAS 5.25.0");

            //custom fields
            issueInfo.CustomFields.Add(new CustomFieldInfo("JDE", "JDE Sample code"));

            //Epic link
            issueInfo.CustomFields.Add(new CustomFieldInfo("Epic Link", "ER-2859"));

            //Components
            issueInfo.Components.Add("ILIAS");

            //Logged
            issueInfo.Logged.Add(new WorkLogInfo(
                "Pierluigi Nanni",
                DateTime.Now,
                "1d",
                "Logging test"));


            engine.Execute(issueInfo);
        }
    }
}
