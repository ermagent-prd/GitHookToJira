using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using JiraTools.Model;
using JiraTools.Parameters;

namespace GeminiToJira.Engine
{
    public class AffectedVersionsEngine
    {
        #region Private properties



        #endregion

        #region Constructor

        #endregion

        #region Public methods

        public void Execute(IssueDto geminiIssue, CreateIssueInfo jiraIssue, Dictionary<string,string> mapping)
        {
            jiraIssue.AffectVersions = new List<string>();

            var versions = getFromAffectedVersions(geminiIssue, jiraIssue, mapping);

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

        private List<string> getFromAffectedVersions(IssueDto geminiIssue, CreateIssueInfo jiraIssue, Dictionary<string, string> mapping)
        {
            if (string.IsNullOrWhiteSpace(geminiIssue.AffectedVersionNumbers))
                return null;

            return ExtractVersions(geminiIssue.AffectedVersionNumbers,mapping);
        }



        public List<string> ExtractVersions(string versionList, Dictionary<string, string> mapping)
        {
            var list = versionList.Split(',');

            var result = new List<string>();

            foreach(var ver in list)
            {
                var mappedVersion = mapVersion(ver.TrimEnd().TrimStart(), mapping);

                result.Add(mappedVersion);
            }

            return result;
        }


        private string mapVersion(string version, Dictionary<string, string> mapping)
        {
            if (mapping == null || !mapping.Any())
                return version;

            if (mapping.ContainsKey(version))
                return mapping[version];

            return version;
        }


        #endregion
    }
}
