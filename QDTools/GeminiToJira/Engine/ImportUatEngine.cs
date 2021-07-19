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
using GeminiToJira.Log;

namespace GeminiToJira.Engine
{
    public class ImportUatEngine
    {
        private readonly BugIssueMapper geminiToJiraMapper;
        private readonly GeminiTools.Items.ItemListGetter geminiItemsEngine;
        private readonly JiraTools.Engine.ItemListGetter jiraItemsEngine;
        private readonly CreateIssueEngine jiraSaveEngine;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly LinkEngine linkEngine;
        private readonly AddWatchersEngine watcherEngine;
        private readonly AffectedVersionsEngine affectedVersionEngine;
        private readonly LogManager logManager;
        private readonly DebugLogManager dbgLogManager;

        public ImportUatEngine(
            BugIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            JiraTools.Engine.ItemListGetter jiraItemsEngine,
            CreateIssueEngine jiraSaveEngine,
            JiraAccountIdEngine accountEngine,
            LinkEngine linkEngine,
            AddWatchersEngine watcherEngine,
            AffectedVersionsEngine affectedVersionEngine,
            LogManager logManager,
            DebugLogManager dbgLogManager)
        {
            this.geminiToJiraMapper = geminiToJiraMapper;
            this.geminiItemsEngine = geminiItemsEngine;
            this.jiraItemsEngine = jiraItemsEngine;
            this.jiraSaveEngine = jiraSaveEngine;
            this.accountEngine = accountEngine;
            this.linkEngine = linkEngine;
            this.watcherEngine = watcherEngine;
            this.affectedVersionEngine = affectedVersionEngine;
            this.logManager = logManager;
            this.dbgLogManager = dbgLogManager;
        }

        public void Execute(GeminiToJiraParameters configurationSetup)
        {
            var projectCode = configurationSetup.JiraProjectCode;

            var uatLogFile = "UatLog_" + projectCode + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            this.logManager.SetLogFile(configurationSetup.LogDirectory + uatLogFile);

            //for debug only
            var uatImportedFile = "UatImported_" + projectCode + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            this.dbgLogManager.SetLogFile(configurationSetup.LogDirectory + uatImportedFile);

            this.dbgLogManager.Execute("IssueKey" + ";" + "Title" + ";" + "CreatedTime" + ";" + "Status");

            Countersoft.Gemini.Commons.Entity.IssuesFilter filter = GetUatFilter(configurationSetup);
            List<String> functionalityList = configurationSetup.Filter.UAT_FUNCTIONALITY;

            //initial date from when we start the import
            var dateFrom = Convert.ToDateTime(configurationSetup.Filter.UAT_CREATED_FROM);
            int daysInterval = 1;
            var dateTo = dateFrom.AddDays(daysInterval);

            //filtro in base a related development/release !!!

            string dateFormat = "yyyy/MM/dd";

            while (dateFrom <= DateTime.Now)
            { 

                filter.CreatedAfter = dateFrom.AddDays(-1).ToString(dateFormat);
                filter.CreatedBefore = dateTo.ToString(dateFormat);

                var geminiUatIssueList = GetFilteredGeminiIssueList(geminiItemsEngine, filter, functionalityList);

                foreach (var geminiIssue in geminiUatIssueList.OrderBy(f => f.CreatedTime).ThenBy(f => f.Id).ToList())
                {
                    /*
                    if (geminiIssue.IssueKey != "UAT-72359")
                        continue;
                    */

                    //for debug only

                    this.dbgLogManager.Execute(geminiIssue.IssueKey + ";" +
                        geminiIssue.Title + ";" +
                        geminiIssue.CreatedTime + ";" +
                        geminiIssue.Status);

                    try
                    {

                        var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);           //we need a new call to have the attachments

                        var jiraIssueInfo = geminiToJiraMapper.Execute(
                            configurationSetup,
                            currentIssue,
                            configurationSetup.Jira.BugTypeCode,
                            projectCode,
                            configurationSetup.Gemini.UatPrefix,
                            null);

                        if (string.IsNullOrWhiteSpace(jiraIssueInfo.RelatedDevelopment))
                            continue;

                        Issue relatedDev = GetRelatedDevelopment(jiraItemsEngine, jiraIssueInfo, projectCode);

                        if (relatedDev == null)
                            continue;

                        if (!relatedDev.FixVersions.Select(f => f.Name).Intersect(configurationSetup.Filter.STORY_RELEASES).Any())
                            continue;

                        foreach (var v in relatedDev.FixVersions)
                            configurationSetup.Filter.STORY_RELEASES.Contains(v.Name);

                        //Add Affected build
                        this.affectedVersionEngine.AddFromRelatedDevelopment(relatedDev, jiraIssueInfo);

                        var jiraIssue = jiraSaveEngine.Execute(
                            jiraIssueInfo,
                            configurationSetup.Jira,
                            configurationSetup.AttachmentDownloadedPath);

                        SetAndSaveReporter(jiraIssue, geminiIssue, configurationSetup.Jira.DefaultAccount);



                        //Add Link to development
                        linkEngine.Execute(jiraIssue, relatedDev.Key.ToString(), "Relates");

                        foreach (var c in relatedDev.Components)
                            jiraIssue.Components.Add(c);

                        foreach (var v in relatedDev.FixVersions)
                            jiraIssue.FixVersions.Add(v);


                        jiraIssue.SaveChanges();

                    }
                    catch(Exception ex)
                    {
                        this.logManager.Execute(geminiIssue.IssueKey + " "+ ex.Message);
                    }
                }

                if (dateTo > DateTime.Now)
                    break;

                dateFrom = dateTo;
                dateTo = dateTo.AddDays(daysInterval);
            }
        }

        #region Private 
        private Countersoft.Gemini.Commons.Entity.IssuesFilter GetUatFilter(GeminiToJiraParameters configurationSetup)
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

        private void SetAndSaveReporter(Issue jiraIssue, IssueDto geminiIssue,string defaultAccount)
        {
            if (geminiIssue.Reporter != "")
            {
                jiraIssue.Reporter = accountEngine.Execute(geminiIssue.Reporter, defaultAccount).AccountId;
                //jiraIssue.SaveChanges();
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

        private string RemoveSpecialChar(JiraTools.Model.CreateIssueInfo jiraIssueInfo)
        {
            var search = jiraIssueInfo.RelatedDevelopment.Replace("\"", "");
            search = search.Replace("+", "");
            search = search.Replace("-", "");

            return search;
        }

        private Issue SearchRelatedDevelopment(JiraTools.Model.CreateIssueInfo jiraIssueInfo, Issue jiraDev, IEnumerable<Issue> jiraDevList)
        {
            foreach (var curr in jiraDevList)
            {
                var geminiId = curr.CustomFields.FirstOrDefault(j => j.Name == "OriginalKey");
                if (geminiId != null && geminiId.Values[0] == jiraIssueInfo.RelatedDevelopmentId)
                    jiraDev = curr;
            }

            return jiraDev;
        }
        #endregion
    }


}
