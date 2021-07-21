using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using JiraTools.Model;

namespace GeminiToJira.Engine
{
    public class AffectedVersionsEngine
    {
        #region Private properties



        #endregion

        #region Constructor

        #endregion

        #region Public methods

        public void Execute(IssueDto geminiIssue, CreateIssueInfo jiraIssue)
        {
            jiraIssue.AffectVersions = new List<string>();

            var versions = getFromAffectedVersions(geminiIssue, jiraIssue);

            if (versions != null)
                jiraIssue.AffectVersions.AddRange(versions);

        }

        public void AddFromRelatedDevelopment(Issue relatedDevelopment, CreateIssueInfo jiraIssue)
        {
            if (relatedDevelopment == null)
                return;

            if (jiraIssue.AffectVersions != null && jiraIssue.AffectVersions.Any())
                return;


            if (relatedDevelopment.FixVersions == null || !relatedDevelopment.FixVersions.Any())
                return;

            jiraIssue.AffectVersions.AddRange(relatedDevelopment.FixVersions.Select(v => v.Name));
        }

        #endregion

        #region Private methods

        private List<string> getFromAffectedVersions(IssueDto geminiIssue, CreateIssueInfo jiraIssue)
        {
            if (string.IsNullOrWhiteSpace(geminiIssue.AffectedVersionNumbers))
                return null;

            return ExtractVersions(geminiIssue.AffectedVersionNumbers);
        }



        private List<string> ExtractVersions(string versionList)
        {
            var list = versionList.Split(',');
            return list.Select(x => x.TrimEnd().TrimStart()).ToList();
        }


        #endregion
    }
}
