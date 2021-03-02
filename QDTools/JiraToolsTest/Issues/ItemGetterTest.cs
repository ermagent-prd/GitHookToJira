using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using Atlassian.Jira.Remote;
using JiraTools.Engine;
using JiraTools.Parameters;
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

            foreach (var issue in issues)
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

            var issues = engine.Execute("ER-5946");   //ER-5892 is a subtask of ER-5885

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

        [TestMethod]
        public void Linq_Execute_GetUatIssues()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<ItemListGetter>();

            var issues = engine.Execute("ER-6518", QuerableType.ByCode, "ER");

            var list = new List<Issue>();

            foreach (var issue in issues)
            {
                list.Add(issue);
            }

            list[0].LinkToIssueAsync("ER-6296", "Relates").Wait();

            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void Linq_Execute_GetAndUpdateWorkLogAuthor()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<ItemListGetter>();

            var issues = engine.Execute("ER-6050", QuerableType.ByCode, "ER");

            var list = new List<Issue>();

            foreach (var issue in issues)
            {
                list.Add(issue);
            }

            var wLog = list[0].GetWorklogsAsync();

            var result = wLog.Result;
            
            Assert.IsNotNull(result);
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void Linq_Execute_SearchBySummary()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<ItemListGetter>();

            var issues = engine.Execute("ILiAS-BSM_2 volumi", QuerableType.BySummary, "ER");

            var list = new List<Issue>();

            foreach (var issue in issues)
            {
                list.Add(issue);
            }

            Assert.IsTrue(list.Any());
        }


        [TestMethod]
        public void Linq_Execute_GetIssueAndUpdateSingleUserField()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<ItemListGetter>();

            var issues = engine.Execute("ER-6460", QuerableType.ByCode, "ER");

            var list = new List<Issue>();

            foreach (var issue in issues)
            {
                list.Add(issue);
            }


            //AssigneeTest
            //nessuno dei seguenti esempi funziona
            //list[0].CustomFields[7].Values = new string[] { "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2" };
            //list[0].CustomFields[7].Values = new string[] { "Paolo Luca" };
            //list[0].CustomFields[7].Values = new string[] { "accountId = \"70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2\" "};
            //list[0].CustomFields[7].Values = new string[] { "[~accountId:70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2]" };

            //list[0].SaveChanges();

            var user = list[0].CustomFields.GetAs<JiraUser>("AssigneeTest");
            //
            //// Get a custom field with single value.
            //var singleValue = list[0]["AssigneeTest"];
            //
            ////  Get a custom field with multiple values.
            //var multiValue = list[0].CustomFields["AssigneeTest"].Values;
            //
            //// Get a cascading select custom field.
            //var cascadingSelect = list[0].CustomFields.GetCascadingSelectField("AssigneeTest");
            //
            //list[0].CustomFields["AssigneeTest"].Values[0] = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            //
            //var settings = new JiraRestClientSettings();
            //settings.EnableUserPrivacyMode = true;

            list[0]["AssigneeTest"] = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            //list[0].CustomFields.AddArray("AssigneeTest", "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2", "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2");

            list[0].Reporter = "70121:67b933a3-5693-47d2-82c0-3f997f279387";
            list[0].SaveChangesAsync().Wait();
            //list[0].CustomFields.AddArray("AssigneeTest", "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2", "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2");
            //var singleAccountId = list[0]["AssigneeTest"];
            //var multiAccounIds = list[0].CustomFields["AssigneeTest"].Values;

            list[0].SaveChanges();

            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void Linq_Execute_GetIssueAndUpdateReporter()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<ItemListGetter>();

            var issues = engine.Execute("ER-6361", QuerableType.ByCode, "ER");

            var list = new List<Issue>();

            foreach (var issue in issues)
            {
                list.Add(issue);
            }

                       
            //this only works for admin user logged
            list[0].Reporter = "70121:67b933a3-5693-47d2-82c0-3f997f279387";
            list[0].SaveChangesAsync().Wait();            

            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void Linq_Execute_GetIssueAndComments()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<ItemListGetter>();

            var issues = engine.Execute("ER-6464", QuerableType.ByCode, "ER");

            var list = new List<Issue>();

            foreach (var issue in issues)
            {
                list.Add(issue);
            }

            var comments = list[0].GetCommentsAsync();

            foreach (var comment in comments.Result)
                Console.WriteLine(comment.Body);
            
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void Linq_Execute_GetUatIssueBySummary()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<ItemListGetter>();

            var issues = engine.Execute("H - XBRL FP \"Sylos based\" + Controlli", QuerableType.BySummary, "EOIB");

            var list = new List<Issue>();

            foreach (var issue in issues)
            {
                list.Add(issue);
            }

            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void Linq_Execute_GetBugIssue()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<ItemListGetter>();

            var issues = engine.Execute("ER-6506", QuerableType.ByCode, "ER");

            var list = new List<Issue>();

            foreach (var issue in issues)
            {
                list.Add(issue);
            }

            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void Linq_Execute_GetIssueAndAddComments()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<ItemListGetter>();

            var issues = engine.Execute("ER-6476", QuerableType.ByCode, "ER");

            var list = new List<Issue>();

            foreach (var issue in issues)
            {
                list.Add(issue);
            }

            var remoteComment = new RemoteComment();
            remoteComment.author = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            remoteComment.updateAuthor = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            
            remoteComment.body = "new comment da paololuca Body Comment";
            
            var comment = new Comment(remoteComment);

            comment.Author = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            comment.Body = "new comment da paololuca Body Comment";
            //            


            list[0].AddCommentAsync(comment);
            list[0].SaveChanges();
            //list[0].UpdateCommentAsync(comment);
            
            list[0].SaveChangesAsync();

            

            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void Linq_Execute_GetIssueAndUpdateWorkLog()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<ItemListGetter>();

            var issues = engine.Execute("EIB-12759", QuerableType.ByCode, "EIB");

            var list = new List<Issue>();

            foreach (var issue in issues)
            {
                list.Add(issue);
            }


            //this only works for admin user logged
            var wrk = list[0].GetWorklogsAsync();
            var worklog = wrk.Result;
            worklog.ElementAt(0).Author = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            list[0].AddWorklogAsync(worklog.ElementAt(0));

            list[0].SaveChanges();


            //
            //var workLog = new Worklog("1d", DateTime.Now, "comment worklog di paolo luca");
            //
            //workLog.Author = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            //
            //
            //list[0].AddWorklogAsync(workLog);


            list[0].SaveChanges();

            Assert.IsTrue(list.Any());
        }
    }
}
