using System;
using System.Collections.Generic;
using System.IO;
using Atlassian.Jira;
using Atlassian.Jira.Remote;
using JiraTools.Engine;
using JiraTools.Model;
using JiraTools.Parameters;
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
                Summary = "Api call Test PL comment user (Atlassian SDK)",
                Description = "This is a (PL timetracking) test " + DateTime.Now.ToString(),
                Type = "Story",
                OriginalEstimate = "480h",
                RemainingEstimate = "400h",
                DueDate = new DateTime(2021, 12, 31),
                ParentIssueKey = null,
                Reporter = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2",
                Assignee = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2",
                AssigneeUser = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2"

            };

            var userEngine = container.Resolve<UserListGetter>();

            var users = userEngine.Execute();
            Dictionary<string, JiraUser> userDictionary = new Dictionary<string, JiraUser>();
            foreach (var user in users)
                userDictionary.Add(user.DisplayName, user);

            issueInfo.CommentList = new List<Comment>();
            
            var remoteComment = new RemoteComment();
            remoteComment.author = "[~accountId:70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2]";
            remoteComment.body = "Body Comment";
            remoteComment.updateAuthor = "[~accountId:70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2]";
            remoteComment.updateAuthorUser = userDictionary["Paolo Luca"];
            remoteComment.updated = DateTime.Now;


            var comment = new Comment(remoteComment);
            comment.Author = "[~accountId:70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2] ";
            comment.Body = "[~accountId:70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2] wrote the Comment";
            issueInfo.CommentList.Add(comment);

            issueInfo.FixVersions.Add("ERMAS 5.25.0");

            //custom fields
            issueInfo.CustomFields.Add(new CustomFieldInfo("JDE", "JDE Sample code"));

            //"70121:67b933a3-5693-47d2-82c0-3f997f279387" pierluigi
            //"70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2" paololuca
            //issueInfo.CustomFields.Add(new CustomFieldInfo("OwnerTmp", "70121:67b933a3-5693-47d2-82c0-3f997f279387"));
            //issueInfo.CustomFields.Add(new CustomFieldInfo("ResourcesTmp", "70121:67b933a3-5693-47d2-82c0-3f997f279387"));


            //issueInfo.CustomFields.Add(new CustomFieldInfo("IT Responsible", "70121:67b933a3-5693-47d2-82c0-3f997f279387"));
            //issueInfo.CustomFields.Add(new CustomFieldInfo("Test Responsible", "70121:67b933a3-5693-47d2-82c0-3f997f279387"));

            //Components
            issueInfo.Components.Add("ILIAS");

            //Logged
            issueInfo.Logged.Add(new WorkLogInfo(
                "[~accountId:70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2] ",
                DateTime.Now,
                "1d",
                "Logging worklog test: author paololuca"));

            var jira = createJiraConfiguration();

            var issue = engine.Execute(issueInfo, jira, JiraTestConstants.AttachmentPath);


            //issue.Assignee = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            //
            //issue.AssignAsync("70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2").Wait();
            //
            //issue.SetPropertyAsync("Assignee", "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2").Wait();
            //
            issue.SaveChanges();

            Assert.IsNotNull(issue);
        }

        

        [TestMethod]
        public void AddSingleIssue_WithSpecificCommentUser()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<CreateIssueEngine>();

            var issueInfo = new CreateIssueInfo
            {
                ProjectKey = "ER",
                Summary = "Api call Test PL usercomment (Atlassian SDK)",
                Description = "This is a (PL usercomment) test " + DateTime.Now.ToString(),
                Type = "Story",
                OriginalEstimate = "1w",
                RemainingEstimate = "1d",
                DueDate = new DateTime(2021, 12, 31),
                ParentIssueKey = null,
                Reporter = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2",
                Assignee = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2",
            };

            issueInfo.CommentList = new List<Comment>();

            var remoteComment = new RemoteComment();
            remoteComment.author = "[~accountId:70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2]";
            remoteComment.updateAuthor = "[~accountId:70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2]";
            remoteComment.body = "Body Comment";

            var comment = new Comment(remoteComment);

            comment.Author = "[~accountId:70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2]"; ;
            comment.Body = "[~accountId:70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2] wrote Body Comment";
            issueInfo.CommentList.Add(comment);


            //Logged
            issueInfo.Logged.Add(new WorkLogInfo(
                "70121:67b933a3-5693-47d2-82c0-3f997f279387",
                DateTime.Now,
                "1d",
                "Logging worklog test: author pierluigi"));

            var jira = createJiraConfiguration();

            var issue = engine.Execute(issueInfo, jira, JiraTestConstants.AttachmentPath);
            
            issue.Reporter = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            issue.SaveChanges();

            Assert.IsNotNull(issue);
        }

        [TestMethod]
        public void AddSingleIssue_WithSpecificWorkLogUser()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<CreateIssueEngine>();

            var issueInfo = new CreateIssueInfo
            {
                ProjectKey = "ER",
                Summary = "Api call Test PL usercomment (Atlassian SDK)",
                Description = "This is a (PL usercomment) test " + DateTime.Now.ToString(),
                Type = "Story",
                OriginalEstimate = "1w",
                RemainingEstimate = "1d",
                DueDate = new DateTime(2021, 12, 31),
                ParentIssueKey = null,
                Reporter = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2",
                Assignee = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2",
            };


            var remoteWrkLog = new RemoteWorklog();
            remoteWrkLog.updateAuthor = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            remoteWrkLog.author = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            remoteWrkLog.comment = "comment worklog";
            remoteWrkLog.startDate = DateTime.Now;
            remoteWrkLog.timeSpent = "1d";

            var jira = createJiraConfiguration();

            var issue = engine.Execute(issueInfo, jira, JiraTestConstants.AttachmentPath);

            issue.Reporter = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            issue.SaveChanges();

            Assert.IsNotNull(issue);
        }

        [TestMethod]
        public void AddSingleIssueWithAttachment()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<CreateIssueEngine>();

            var issueInfo = new CreateIssueInfo
            {
                ProjectKey = "ER",
                Summary = "Attachments Test PL attachment refactoring (Atlassian SDK)",
                Description = "This is a (PL attachment) test " + DateTime.Now.ToString(),
                Type = "StoryD",
                OriginalEstimate = "1w",
                RemainingEstimate = "1d",
                DueDate = new DateTime(2021, 12, 31),
                ParentIssueKey = null,

                Assignee = "70121:67b933a3-5693-47d2-82c0-3f997f279387",
                AssigneeUser = "Paolo Luca"

            };

            issueInfo.CommentList = new List<Comment>();
            var comment = new Comment();
            comment.Author = "Paolo Luca";
            comment.Body = "Body Comment";
            issueInfo.CommentList.Add(comment);

            issueInfo.FixVersions.Add("ERMAS 5.25.0");
            issueInfo.ParentIssueKey = "ER-5885";

            //Logged
            issueInfo.Logged.Add(new WorkLogInfo(
                "Pierluigi Nanni",
                DateTime.Now,
                "1d",
                "Logging attachment test"));

            var jira = createJiraConfiguration();

            var issue = engine.Execute(issueInfo, jira, JiraTestConstants.AttachmentPath);

            var byteArray = System.IO.File.ReadAllBytes(JiraTestConstants.AttachmentPath + "prova.docx");
            var uAttachmentInfo = new UploadAttachmentInfo("prova.docx.xlsx", byteArray);
            issue.AddAttachment(uAttachmentInfo);

            byteArray = System.IO.File.ReadAllBytes(JiraTestConstants.AttachmentPath + "prova.txt");
            uAttachmentInfo = new UploadAttachmentInfo("prova.txt", byteArray);
            issue.AddAttachment(uAttachmentInfo);

            issue.SaveChanges();

            Assert.IsNotNull(issue);
        }

        [TestMethod]
        public void AddSingleIssueUat()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<CreateIssueEngine>();

            var issueInfo = new CreateIssueInfo
            {
                ProjectKey = "ER",
                Summary = "create Test PL with watcher (Atlassian SDK)",
                Description = "This is a (PL UAT) test " + DateTime.Now.ToString(),
                Type = "UAT",
                OriginalEstimate = "1w",
                RemainingEstimate = "1d",

                Assignee = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2",

            };

            issueInfo.CommentList = new List<Comment>();
            var comment = new Comment();
            comment.Author = "Paolo Luca";
            comment.Body = "Body Comment";
            issueInfo.CommentList.Add(comment);

            var jira = createJiraConfiguration();

            var issue = engine.Execute(issueInfo, jira, JiraTestConstants.AttachmentPath);

            Assert.IsNotNull(issue);
        }


        #region Private
        private JiraConfiguration createJiraConfiguration()
        {
            return new JiraConfiguration()
            {

                User = "pierluigi.nanni@prometeia.com", //"paolo.luca@prometeia.com";
                Token = "GOojveJqyGUAgi6mzSAu3FAA", //"1l9yaa74u6wqg4lbhitOE7C7";
                Url = "https://prometeia-erm.atlassian.net/",
                IssueApi = "rest/api/3/issue/",
                MaxIssuesPerRequest = 10, //Max 100 ??

                EpicTypeCode = "10000",
                StoryTypeCode = "10001",
                StorySubTaskTypeCode = "10017",
                TaskTypeCode = "10002",
                SubTaskTypeCode = "10003",
                UatTypeCode = "10014",
                BugTypeCode = "10004",
            };
        }
        #endregion
    }
}
