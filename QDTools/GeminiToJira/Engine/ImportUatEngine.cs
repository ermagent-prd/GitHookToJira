using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.GeminiFilter;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters;
using JiraTools.Engine;
using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using JiraTools.Parameters;
using System;
using System.IO;

namespace GeminiToJira.Engine
{
    public class ImportUatEngine
    {
        private readonly UatIssueMapper geminiToJiraMapper;
        private readonly GeminiTools.Items.ItemListGetter geminiItemsEngine;
        private readonly JiraTools.Engine.ItemListGetter jiraItemsEngine;
        private readonly CreateIssueEngine jiraSaveEngine;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly LinkEngine linkEngine;

        public ImportUatEngine(
            UatIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            JiraTools.Engine.ItemListGetter jiraItemsEngine,
            CreateIssueEngine jiraSaveEngine,
            JiraAccountIdEngine accountEngine,
            LinkEngine linkEngine)
        {
            this.geminiToJiraMapper = geminiToJiraMapper;
            this.geminiItemsEngine = geminiItemsEngine;
            this.jiraItemsEngine = jiraItemsEngine;
            this.jiraSaveEngine = jiraSaveEngine;
            this.accountEngine = accountEngine;
            this.linkEngine = linkEngine;

        }

        public void Execute(string projectCode, DateTime fromDate)
        {
            var geminiUatIssueList = filterGeminiIssueList(geminiItemsEngine, fromDate);
            var uatLogFile = "UatLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            foreach (var geminiIssue in geminiUatIssueList.OrderBy(f => f.Id).ToList())
            {
                //try
                //{
                    var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);           //we need a new call to have the attachments

                    var jiraIssueInfo = geminiToJiraMapper.Execute(currentIssue, JiraConstants.UatType, projectCode);

                    var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo);
                    SetAndSaveReporter(jiraIssue, geminiIssue);

                    if (jiraIssueInfo.RelatedDevelopment != null && jiraIssueInfo.RelatedDevelopment != "")
                    {
                        Issue relatedDev = GetRelatedDevelopment(jiraItemsEngine, jiraIssueInfo, projectCode);

                        if (relatedDev != null)
                        {
                            linkEngine.Execute(jiraIssue, relatedDev.Key.ToString(), "Relates");

                            foreach (var c in relatedDev.Components)
                                jiraIssue.Components.Add(c);

                            foreach (var v in relatedDev.FixVersions)
                                jiraIssue.FixVersions.Add(v);

                            var owner = jiraIssue.CustomFields.FirstOrDefault(c => c.Name == "Owner");
                            if(owner == null)
                                jiraIssue.CustomFields.Add("Owner", relatedDev.CustomFields.First(c => c.Name == "Owner").Values);

                            jiraIssue.SaveChanges();
                        }
                    }
                //}
                //catch
                //{
                //    File.AppendAllText(JiraConstants.LogDirectory + uatLogFile, geminiIssue.IssueKey + Environment.NewLine);
                //}
            }
        }

        #region Private 
        private IEnumerable<IssueDto> filterGeminiIssueList(
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            DateTime fromDate)
        {
            var geminiIssueList = geminiItemsEngine.Execute(Filter.GetFilter(FilterType.UAT));
            Filter.FilterUatIssuesList(geminiIssueList, fromDate);
            return geminiIssueList;
        }

        private void SetAndSaveReporter(Issue jiraIssue, IssueDto geminiIssue)
        {
            if (geminiIssue.Reporter != "")
            {
                jiraIssue.Reporter = accountEngine.Execute(geminiIssue.Reporter).AccountId;
                jiraIssue.SaveChanges();
            }
        }

        private Issue GetRelatedDevelopment(JiraTools.Engine.ItemListGetter jiraItemsEngine, JiraTools.Model.CreateIssueInfo jiraIssueInfo, string projectCode)
        {
            Issue jiraDev = null;
            var jiraDevList = jiraItemsEngine.Execute(jiraIssueInfo.RelatedDevelopment, QuerableType.BySummary, projectCode);

            foreach (var curr in jiraDevList)
            {
                var geminiId = curr.CustomFields.FirstOrDefault(j => j.Name == "GEMINI");
                if (geminiId != null && geminiId.Values[0] == jiraIssueInfo.RelatedDevelopmentId)
                    jiraDev = curr;
            }

            return jiraDev;
        }
        #endregion
    }


}
