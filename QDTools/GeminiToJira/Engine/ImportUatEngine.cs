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
using GeminiToJira.Parameters.Import;

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

        public void Execute(GeminiToJiraParameters configurationSetup)
        {
            var projectCode = configurationSetup.JiraProjectCode;

            var uatLogFile = "UatLog_" + projectCode + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            //for debug only
            var uatImportedFile = "UatImported_" + projectCode + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            File.AppendAllText(configurationSetup.LogDirectory + uatImportedFile, "IssueKey" + ";" + "Title" + ";" + "CreatedTime" +";" + "Status" + Environment.NewLine);

            Countersoft.Gemini.Commons.Entity.IssuesFilter filter = GetUatFilter(configurationSetup);
            List<String> functionalityList = configurationSetup.Filter.UAT_FUNCTIONALITY;
                        
            //initial date from when we start the import
            var dateFrom = Convert.ToDateTime(configurationSetup.Filter.UAT_CREATED_FROM);
            //date until we make the import from dateFrom
            DateTime dateTo = DateTime.Now;

            while (dateFrom <= dateTo)
            {
                filter.CreatedAfter = dateFrom.ToString("MM/dd/yyyy");
                filter.CreatedBefore = dateFrom.ToString("MM/dd/yyyy");

                var geminiUatIssueList = GetFilteredGeminiIssueList(geminiItemsEngine, filter, functionalityList);

                foreach (var geminiIssue in geminiUatIssueList.OrderBy(f => f.CreatedTime).ThenBy(f => f.Id).ToList())
                {
                    //for debug only
                    File.AppendAllText(configurationSetup.LogDirectory + uatImportedFile, 
                        geminiIssue.IssueKey + ";" + 
                        geminiIssue.Title + ";" + 
                        geminiIssue.CreatedTime + ";" + 
                        geminiIssue.Status + 
                        Environment.NewLine);

                    try
                    {
                        var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);           //we need a new call to have the attachments
                    
                        var jiraIssueInfo = geminiToJiraMapper.Execute(currentIssue, configurationSetup.Jira.UatTypeCode, projectCode);
                    
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
                                //if owner is null i'll set it as father's owner
                                if (owner == null)
                                    jiraIssue.CustomFields.Add("Owner", relatedDev.CustomFields.First(c => c.Name == "Owner").Values);
                    
                                jiraIssue.SaveChanges();
                            }
                        }
                    }
                    catch
                    {
                        File.AppendAllText(configurationSetup.LogDirectory + uatLogFile, geminiIssue.IssueKey + Environment.NewLine);
                    }
                }
                dateFrom = dateFrom.AddDays(1);
            }
        }

        #region Private 
        private static Countersoft.Gemini.Commons.Entity.IssuesFilter GetUatFilter(GeminiToJiraParameters configurationSetup)
        {
            return new Countersoft.Gemini.Commons.Entity.IssuesFilter()
            {
                IncludeClosed = configurationSetup.Filter.UAT_INCLUDED_CLOSED,
                GroupDependencies = configurationSetup.Filter.UAT_GROUP_DEPENDENCIES,
                Projects = configurationSetup.Filter.UAT_PROJECT_ID,
            };
        }

        private IEnumerable<IssueDto> GetFilteredGeminiIssueList(
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            Countersoft.Gemini.Commons.Entity.IssuesFilter filter,
            List<string> functionalityList)
        {
            var geminiIssueList = geminiItemsEngine.Execute(filter);
            if (functionalityList != null && functionalityList.Count > 0)
                return FilterByFunctionality(geminiIssueList, functionalityList);
            else
                return geminiIssueList;
        }

        private IEnumerable<IssueDto> FilterByFunctionality(IEnumerable<IssueDto> geminiIssueList, List<string> functionalityList)
        {
            List<IssueDto> result = new List<IssueDto>();

            foreach(var issue in geminiIssueList)
            {
                var functionality = issue.CustomFields.FirstOrDefault(i => i.Name == "Functionality");
                if (functionality != null && functionalityList.Contains(functionality.FormattedData))
                    result.Add(issue);
            }

            return result;
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
                //probably the title contains special charactes, that we need to remove (i.e. +, -, "...)
                jiraDevList = jiraItemsEngine.Execute(
                    RemoveSpecialChar(jiraIssueInfo),
                    QuerableType.BySummary,
                    projectCode);
                jiraDev = SearchRelatedDevelopment(jiraIssueInfo, jiraDev, jiraDevList);
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
