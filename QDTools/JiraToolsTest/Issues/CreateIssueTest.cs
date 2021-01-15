using System;
using System.Collections.Generic;
using System.IO;
using Atlassian.Jira;
using JiraTools.Engine;
using JiraTools.Model;
using JiraTools.Parameters;
using JiraToolsTest.Container;
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
                Summary = "Api call Test PL2 custom (Atlassian SDK)",
                Description = "This is a (PL2 custom) test " + DateTime.Now.ToString(),
                Priority = "Medium",
                Type = "Story",
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

            //custom fields
            issueInfo.CustomFields.Add(new CustomFieldInfo("JDE", "JDE Sample code"));

            //Epic link
            issueInfo.CustomFields.Add(new CustomFieldInfo("Epic Link", "ER-2859"));

            //assignee and owner custom
            issueInfo.CustomFields.Add(new CustomFieldInfo("Owner", "Paolo Luca"));
            issueInfo.CustomFields.Add(new CustomFieldInfo("AssigneeTest", "Paolo Luca"));

            issueInfo.CustomFields.Add(new CustomFieldInfo("OwnerTmp", "Paolo Luca"));
            
            //Components
            issueInfo.Components.Add("ILIAS");

            //Logged
            issueInfo.Logged.Add(new WorkLogInfo(
                "Pierluigi Nanni",
                DateTime.Now,
                "1d",
                "Logging test"));


            var issue = engine.Execute(issueInfo);

            issue.SetPropertyAsync("SubTask", "ER-5885").Wait();

            //issue.Assignee = "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2";
            //
            //issue.AssignAsync("70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2").Wait();
            //
            //issue.SetPropertyAsync("Assignee", "70121:c13ce356-ec00-4ffd-b615-a45a86aa99e2").Wait();
            //
            issue.SaveChanges();
        }

        [TestMethod]
        public void AddSingleIssueWithAttachment()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<CreateIssueEngine>();

            var issueInfo = new CreateIssueInfo
            {
                ProjectKey = "ER",
                Summary = "Attachments Test PL custom (Atlassian SDK)",
                Description = "This is a (PL attachment) test " + DateTime.Now.ToString(),
                Priority = "Medium",
                Type = "Story",
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
                "Paolo Luca",
                DateTime.Now,
                "1d",
                "Logging attachment test"));


            var issue = engine.Execute(issueInfo);

            var byteArray = System.IO.File.ReadAllBytes(Constants.AttachmentPath + "PD_AKBANK_PD_SML10_BAD20201031.xlsx");
            var uAttachmentInfo = new UploadAttachmentInfo("PD_AKBANK_PD_SML10_BAD20201031.xlsx", byteArray);
            issue.AddAttachment(uAttachmentInfo);

            byteArray = System.IO.File.ReadAllBytes(Constants.AttachmentPath + "prova.txt");
            uAttachmentInfo = new UploadAttachmentInfo("prova.txt", byteArray);
            issue.AddAttachment(uAttachmentInfo);

            issue.SaveChanges();
        }
    }
}
