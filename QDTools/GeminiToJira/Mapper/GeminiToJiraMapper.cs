using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine;
using GeminiTools.Items;
using JiraTools.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeminiToJira.Mapper
{
    public class GeminiToJiraMapper
    {
        private const string AFFECTEDBUILD = "AffectedBuild";
        private const string DEVELOPMENT_RELEASE_KEY = "Release Version";
        private const string DEVELOPMENT_LINE_KEY = "DVL";

        public GeminiToJiraMapper()
        {

        }

        public CreateIssueInfo Execute(IssueDto geminiIssue, string type)
        {
            
            var mappedIssue = new CreateIssueInfo
            {
                ProjectKey = "ER", //TODO issue.Project.Code,
                Summary = geminiIssue.Title,
                Description = geminiIssue.Description + DateTime.Now.ToString(),
                Priority = geminiIssue.Priority,
                Type = type,
                OriginalEstimate = geminiIssue.EstimatedHours + "h",
                RemainingEstimate = geminiIssue.RemainingTime,
                //TODOPL
                DueDate = new DateTime(2021, 12, 31),
            };

            //Save affected build, if present
            var affectedBuild = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == AFFECTEDBUILD);
            if(affectedBuild != null && affectedBuild.FormattedData != "")
                mappedIssue.FixVersions.Add(affectedBuild.FormattedData);

            var release = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_RELEASE_KEY);
            if (release != null && release.FormattedData != "")
                mappedIssue.FixVersions.Add(release.FormattedData);

            //TODO ?? var devLine = issue.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_LINE_KEY);
            //TODO ?? if (devLine != null && devLine.FormattedData != "")
            //TODO ??     mappedIssue.FixVersions.Add(devLine.FormattedData);

            //TODO
            mappedIssue.Assignee = geminiIssue.Resources.FirstOrDefault()?.Entity.Fullname;

            //Load all issue attachment
            LoadAttachments(mappedIssue, geminiIssue.Attachments);

            //Load all gemini comments
            LoadComments(mappedIssue, geminiIssue);

            //Load custom fields
            LoadCustomFields(mappedIssue, geminiIssue);

            //TODO Components
            //issueInfo.Components.Add("ILIAS");

            //TODO 
            mappedIssue.Logged.Add(new WorkLogInfo(
                "Pierluigi Nanni",
                DateTime.Now,
                "1d",
                "WorkLog Logging test"));

            return mappedIssue;
        }


        #region Private

        private void LoadAttachments(CreateIssueInfo jiraIssue, List<IssueAttachmentDto> attachments)
        {
            jiraIssue.Attachments = new List<string>();

            foreach (var attachment in attachments)
            {
                AttachmentGetter.Save(
                    attachment.Entity.ProjectId,
                    attachment.Entity.IssueId,
                    attachment.Entity.Id,
                    attachment.Entity.Name);

                jiraIssue.Attachments.Add(attachment.Entity.Name);
            }
        }

        private void LoadComments(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            jiraIssue.CommentList = new List<Comment>();

            foreach(var comment in geminiIssue.Comments)
            {
                var jiraComment = new Comment();
                jiraComment.Author = comment.Entity.Fullname;
                jiraComment.Body = comment.Entity.Fullname + " : " + ParseCommentEngine.Execute(comment.Entity.Comment);
                jiraIssue.CommentList.Add(jiraComment);

                //Load Comment attachments
                LoadAttachments(jiraIssue, comment.Attachments);
            }
        }

        

        private void LoadCustomFields(CreateIssueInfo jiraIssue, IssueDto geminiIssue)
        {
            //TODO da caricare solo quelli creati in JIRA 
            //foreach (var field in issue.CustomFields)
            //    jiraIssue.CustomFields.Add(new CustomFieldInfo(field.Name, field.FormattedData));


            jiraIssue.CustomFields.Add(new CustomFieldInfo("OwnerTmp", geminiIssue.Creator));
            jiraIssue.CustomFields.Add(new CustomFieldInfo("ResourcesTmp", geminiIssue.Resources.FirstOrDefault()?.Entity.Fullname));

        }

        #endregion
    }
}
