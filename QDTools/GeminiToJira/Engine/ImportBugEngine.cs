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
    public class ImportBugEngine
    {
        private readonly BugIssueMapper geminiToJiraMapper;
        private readonly GeminiTools.Items.ItemListGetter geminiItemsEngine;
        private readonly JiraTools.Engine.ItemListGetter jiraItemsEngine;
        private readonly CreateIssueEngine jiraSaveEngine;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly LinkEngine linkEngine;

        public ImportBugEngine(
            BugIssueMapper geminiToJiraMapper,
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

        public void Execute(string projectCode)
        {
            var geminiBugIssueList = filterGeminiIssueList(geminiItemsEngine);
            var bugLogFile = "BugLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";


            foreach (var geminiIssue in geminiBugIssueList.OrderBy(f => f.Id).ToList())
            {
                //try
                //{
                var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);

                var jiraIssueInfo = geminiToJiraMapper.Execute(currentIssue, JiraConstants.BugType, projectCode);

                var jiraIssue = jiraSaveEngine.Execute(jiraIssueInfo);
                SetAndSaveReporter(jiraIssue, geminiIssue);

                if (jiraIssueInfo.RelatedDevelopment != null && jiraIssueInfo.RelatedDevelopment != "")
                {
                    Issue relatedDev = GetRelatedDevelopment(jiraItemsEngine, jiraIssueInfo);

                    if (relatedDev != null)
                    {
                        linkEngine.Execute(jiraIssue, relatedDev.Key.ToString(), "Relates");

                        foreach (var c in relatedDev.Components)
                            jiraIssue.Components.Add(c);

                        jiraIssue.SaveChanges();
                    }
                }

                //}
                //catch
                //{
                //    File.AppendAllText(JiraConstants.LogDirectory + bugLogFile, geminiIssue + Environment.NewLine);
                //}
            }
        }

        #region Private 
        private Issue GetRelatedDevelopment(JiraTools.Engine.ItemListGetter jiraItemsEngine, JiraTools.Model.CreateIssueInfo jiraIssueInfo)
        {
            Issue jiraDev = null;
            var jiraDevList = jiraItemsEngine.Execute(jiraIssueInfo.RelatedDevelopment, QuerableType.BySummary);

            foreach (var curr in jiraDevList)
            {
                var geminiId = curr.CustomFields.FirstOrDefault(j => j.Name == "GEMINI");
                if (geminiId != null && geminiId.Values[0] == jiraIssueInfo.RelatedDevelopmentId)
                    jiraDev = curr;
            }

            return jiraDev;
        }

        private IEnumerable<IssueDto> filterGeminiIssueList(
            GeminiTools.Items.ItemListGetter geminiItemsEngine)
        {
            var geminiIssueList = geminiItemsEngine.Execute(Filter.GetFilter(FilterType.ERMBUG));
            Filter.FilterIssuesList(FilterType.ERMBUG, geminiIssueList);
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

        //private Issue GetRelatedDevelopment(JiraTools.Engine.ItemListGetter jiraItemsEngine, JiraTools.Model.CreateIssueInfo jiraIssueInfo)
        //{
        //    Issue jiraDev = null;
        //    var jiraDevList = jiraItemsEngine.Execute(jiraIssueInfo.RelatedDevelopment, QuerableType.BySummary);
        //
        //    foreach (var curr in jiraDevList)
        //    {
        //        var geminiId = curr.CustomFields.FirstOrDefault(j => j.Name == "GEMINI");
        //        if (geminiId != null && geminiId.Values[0] == jiraIssueInfo.RelatedDevelopmentId)
        //            jiraDev = curr;
        //    }
        //
        //    return jiraDev;
        //}
        #endregion
    }


}
