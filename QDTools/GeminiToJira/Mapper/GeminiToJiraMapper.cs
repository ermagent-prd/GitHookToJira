using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Engine;
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

            //Load all gemini comments
            LoadComments(mappedIssue, geminiIssue);

            //custom fields
            //TODO da caricare solo quelli creati in JIRA LoadCustomFields(mappedIssue, issue);
            
            mappedIssue.CustomFields.Add(new CustomFieldInfo("OwnerTmp", geminiIssue.Creator));
            mappedIssue.CustomFields.Add(new CustomFieldInfo("ResourcesTmp", geminiIssue.Resources.FirstOrDefault()?.Entity.Fullname));           

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

        private void LoadComments(CreateIssueInfo jiraIssue, IssueDto issue)
        {
            jiraIssue.CommentList = new List<Comment>();

            foreach(var comment in issue.Comments)
            {
                var jiraComment = new Comment();

                jiraComment.Author = comment.Entity.Fullname;

                jiraComment.Body = ParseCommentEngine.Execute(comment.Entity.Comment);

                jiraIssue.CommentList.Add(jiraComment);
            }
        }

        private void LoadCustomFields(CreateIssueInfo jiraIssue, IssueDto issue)
        {
            foreach(var field in issue.CustomFields)
                jiraIssue.CustomFields.Add(new CustomFieldInfo(field.Name, field.FormattedData));
        }
    }
}
