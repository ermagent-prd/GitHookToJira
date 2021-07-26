using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Log;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters.Import;
using JiraTools.Engine;

namespace GeminiToJira.Engine
{
    public class ImportUatEngine
    {
        private readonly UATIssueMapper geminiToJiraMapper;
        private readonly GeminiTools.Items.ItemListGetter geminiItemsEngine;
        private readonly JiraTools.Engine.ItemListGetter jiraItemsEngine;
        private readonly CreateIssueEngine jiraSaveEngine;
        private readonly JiraAccountIdEngine accountEngine;
        private readonly LinkEngine linkEngine;
        private readonly AddWatchersEngine watcherEngine;
        private readonly AffectedVersionsEngine affectedVersionEngine;
        private readonly LogManager logManager;
        private readonly DebugLogManager dbgLogManager;

        private readonly JqlGetter jqlgetter;

        public ImportUatEngine(
            UATIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            JiraTools.Engine.ItemListGetter jiraItemsEngine,
            CreateIssueEngine jiraSaveEngine,
            JiraAccountIdEngine accountEngine,
            LinkEngine linkEngine,
            AddWatchersEngine watcherEngine,
            AffectedVersionsEngine affectedVersionEngine,
            LogManager logManager,
            DebugLogManager dbgLogManager,
            JqlGetter jqlgetter)
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
            this.jqlgetter = jqlgetter;
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

            //Load developments
            string jsql = $"Project = \""+projectCode+"\" and type = \"Story\"";

            var jiraStories = this.jqlgetter.Execute(jsql);

            var stories = jiraStories.ToDictionary(s =>s.Summary);

            //initial date from when we start the import
            var dateFrom = Convert.ToDateTime(configurationSetup.Filter.UAT_CREATED_FROM);
            int daysInterval = configurationSetup.Filter.UAT_DAYS_BLOCK == 0 ? 
                7: 
                configurationSetup.Filter.UAT_DAYS_BLOCK;

            var dateTo = dateFrom.AddDays(daysInterval);

            //filtro in base a related development/release !!!

            string dateFormat = "yyyy/MM/dd";

            HashSet<string> geminiCodes = new HashSet<string>();

            while (dateFrom <= DateTime.Now)
            { 

                filter.CreatedAfter = dateFrom.AddDays(-1).ToString(dateFormat);
                filter.CreatedBefore = dateTo.ToString(dateFormat);

                var geminiUatIssueList = GetFilteredGeminiIssueList(geminiItemsEngine, filter, functionalityList);

                foreach (var geminiIssue in geminiUatIssueList.OrderBy(f => f.CreatedTime).ThenBy(f => f.Id).ToList())
                {
                    if (geminiCodes.Contains(geminiIssue.IssueKey))
                        continue;

                    //for debug only

                    this.dbgLogManager.Execute(geminiIssue.IssueKey + ";" +
                        geminiIssue.Title + ";" +
                        geminiIssue.CreatedTime + ";" +
                        geminiIssue.Status);

                    try
                    {
                        #region releated developmen checks

                        var relatedDevSummary = getRelatedDevelopmentSummary(
                            geminiIssue,
                            configurationSetup.Gemini.ErmPrefix,
                            configurationSetup.Mapping);

                        if (string.IsNullOrWhiteSpace(relatedDevSummary))
                            continue;

                        Issue relatedDev = getStoryBySummary(relatedDevSummary, stories);

                        if (relatedDev == null)
                            continue;

                        if (!relatedDev.FixVersions.Select(f => f.Name).Intersect(configurationSetup.Filter.STORY_RELEASES).Any())
                            continue;
                        #endregion

                        var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);           //we need a new call to have the attachments

                        var jiraIssueInfo = geminiToJiraMapper.Execute(
                            configurationSetup,
                            currentIssue,
                            configurationSetup.Jira.BugTypeCode,
                            projectCode,
                            configurationSetup.Gemini.UatPrefix,
                            null,
                            relatedDev);

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

                        geminiCodes.Add(geminiIssue.IssueKey);

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

        private string getRelatedDevelopmentSummary(IssueDto geminiIssue, string ermPrefix, JiraTools.Parameters.MappingConfiguration mapping)
        {
            //Related Development Build
            var relatedDev = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == mapping.RELATED_DEVELOPMENT_LABEL);

            if (relatedDev != null)
            {
                return relatedDev.FormattedData;
            }

            return null;
        }


        private Issue getStoryBySummary(string summary, Dictionary<string,Issue> stories)
        {
            Issue item = null;
            if (stories.TryGetValue(summary, out item))
                return item;

            return null;

        }

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

            if (string.IsNullOrWhiteSpace(geminiIssue.Reporter))
                return;

            var jiraUser = accountEngine.Execute(geminiIssue.Reporter, defaultAccount);

            if (jiraUser == null)
                return;

            jiraIssue.Reporter = jiraUser.AccountId;
        }


        #endregion
    }


}
