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

        public void Execute(string projectCode)
        {
            var uatLogFile = "UatLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            //for debug only
            //var uatImportedFile = "UatImported_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            //File.AppendAllText(JiraConstants.LogDirectory + uatImportedFile, "IssueKey" + ";" + "Title" + ";" + "CreatedTime" +";" + "Status" + Environment.NewLine);


            var filter = Filter.GetFilter(FilterType.UAT);
            var dateFrom = Convert.ToDateTime(UatConstants.UAT_CREATED_FROM);
            filter.IncludeClosed = true;

            //date until make the import from dateFrom
            DateTime dateTo = Convert.ToDateTime("31/12/2020");

            while (dateFrom <= dateTo)
            {
                filter.CreatedAfter = dateFrom.ToString("MM/dd/yyyy");
                filter.CreatedBefore = dateFrom.ToString("MM/dd/yyyy");

                var geminiUatIssueList = filterGeminiIssueList(geminiItemsEngine, filter);

                foreach (var geminiIssue in geminiUatIssueList.OrderBy(f => f.CreatedTime).ThenBy(f => f.Id).ToList())
                {
                    //for debug only
                    //File.AppendAllText(JiraConstants.LogDirectory + uatImportedFile, 
                    //    geminiIssue.IssueKey + ";" + 
                    //    geminiIssue.Title + ";" + 
                    //    geminiIssue.CreatedTime + ";" + 
                    //    geminiIssue.Status + 
                    //    Environment.NewLine);

                    try
                    {
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
                                if (owner == null)
                                    jiraIssue.CustomFields.Add("Owner", relatedDev.CustomFields.First(c => c.Name == "Owner").Values);
                    
                                jiraIssue.SaveChanges();
                            }
                        }
                    }
                    catch
                    {
                        File.AppendAllText(JiraConstants.LogDirectory + uatLogFile, geminiIssue.IssueKey + Environment.NewLine);
                    }
                }
                dateFrom = dateFrom.AddDays(1);
            }
        }

        #region Private 
        private IEnumerable<IssueDto> filterGeminiIssueList(
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            Countersoft.Gemini.Commons.Entity.IssuesFilter filter)
        {
            var geminiIssueList = geminiItemsEngine.Execute(filter);
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

            try
            {
                jiraDev = SearchRelatedDevelopment(jiraIssueInfo, jiraDev, jiraDevList);
            }
            catch
            {
                //if related dev contains " we need to search in "like" mode for the substring
                if (jiraIssueInfo.RelatedDevelopment.Contains("\""))
                {
                    jiraDevList = jiraItemsEngine.Execute(
                        RemoveSpecialChar(jiraIssueInfo),
                        QuerableType.BySummary,
                        projectCode);
                    jiraDev = SearchRelatedDevelopment(jiraIssueInfo, jiraDev, jiraDevList);
                }
            }

            return jiraDev;
        }

        private static string RemoveSpecialChar(JiraTools.Model.CreateIssueInfo jiraIssueInfo)
        {
            var search = jiraIssueInfo.RelatedDevelopment.Replace("\"", "");
            search = search.Replace("+", "");
            search = search.Replace("-", "");

            return search;
        }

        private static Issue SearchRelatedDevelopment(JiraTools.Model.CreateIssueInfo jiraIssueInfo, Issue jiraDev, IEnumerable<Issue> jiraDevList)
        {
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
