using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Log;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters.Import;
using JiraTools.Engine;
using static GeminiToJira.Engine.JiraAccountIdEngine;

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
            var stories = configurationSetup.Filter.UAT_CHECK_RELATED_DEV ?
                getStories(projectCode, configurationSetup.Filter.STORY_RELEASES):
                null;

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

                var geminiUatIssueList = GetFilteredGeminiIssueList(
                    geminiItemsEngine, 
                    filter, 
                    functionalityList,
                    configurationSetup.Filter.UAT_SELECTED_ITEMS);

                foreach (var geminiIssue in geminiUatIssueList.OrderBy(f => f.CreatedTime).ThenBy(f => f.Id).ToList())
                {
                    if (geminiCodes.Contains(geminiIssue.IssueKey))
                        continue;

                    /*
                    if (geminiIssue.Status == "Closed" || geminiIssue.Status == "Cancelled")
                        continue;

                    */

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
                            configurationSetup);

                        if (string.IsNullOrWhiteSpace(relatedDevSummary))
                            continue;

                        Issue relatedDev = getStoryBySummary(relatedDevSummary, stories);

                        if (relatedDev == null && configurationSetup.Filter.UAT_CHECK_RELATED_DEV)
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
                        if (relatedDev?.Key != null)
                            linkEngine.Execute(jiraIssue, relatedDev.Key.ToString(), "Relates");

                        if (relatedDev != null)
                        {
                            foreach (var c in relatedDev.Components)
                                jiraIssue.Components.Add(c);

                        }

                        if (relatedDev != null)
                        {
                            foreach (var v in relatedDev.FixVersions)
                                jiraIssue.FixVersions.Add(v);

                        }


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

        private bool checkSelected(string issueKey, HashSet<string> codes)
        {
            if (codes == null || !codes.Any())
                return true;

            return
                codes.Contains(issueKey);

        }

        private Dictionary<String, Issue> getStoriesTemp(string projectCode, List<string> releases)
        {
            string jsql = $"Project = \"" + projectCode + "\" and key = ERMAS-17462";

            var jiraStories = this.jqlgetter.Execute(jsql);

            var filteredIssues = jiraStories
                .Where(s => s.FixVersions.Select(f => f.Name).Intersect(releases).Any())
                .Select(s => s);

            return filteredIssues.ToDictionary(s => s.Summary);

        }


        private Dictionary<String,Issue> getStories(string projectCode, List<string> releases)
        {
            string jsql = $"Project = \"" + projectCode + "\" and (type = \"Story\" or type = \"Epic\")";

            var jiraStories = this.jqlgetter.Execute(jsql);

            var filteredIssues = jiraStories
                .Where(s => s.FixVersions.Select(f => f.Name).Intersect(releases).Any())
                .Select(s => s);

            return filteredIssues.ToDictionary(s => s.Summary);

        }

        private string getRelatedDevelopmentSummary(IssueDto geminiIssue, string ermPrefix, GeminiToJiraParameters config)
        {
            //Related Development Build
            var relatedDev = geminiIssue.CustomFields.FirstOrDefault(x => x.Name == config.Mapping.RELATED_DEVELOPMENT_LABEL);

            if (relatedDev == null)
                return null;

            var devSummary = relatedDev.FormattedData;

            if (String.IsNullOrWhiteSpace(devSummary))
                return null;

            //Check dev filter if active
            if (config.Filter?.UAT_RELATED_DEV == null || !config.Filter.UAT_RELATED_DEV.Any())
                return devSummary;

            if (!config.Filter.UAT_RELATED_DEV.Contains(devSummary))
                return null;

            return devSummary;
        }


    private Issue getStoryBySummary(string summary, Dictionary<string,Issue> stories)
        {
            if (stories == null)
                return null;

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
            List<string> functionalityList,
            HashSet<string> selectedItems)
        {
            var geminiIssueList = geminiItemsEngine.Execute(filter);
            if (functionalityList != null && functionalityList.Count > 0)
                return filterUat(geminiIssueList, functionalityList, selectedItems);
            else
                return geminiIssueList;
        }

        private IEnumerable<IssueDto> filterUat(
            IEnumerable<IssueDto> geminiIssueList, 
            List<string> functionalityList,
            HashSet<string> selectedItems)
        {
            List<IssueDto> result = new List<IssueDto>();

            foreach(var issue in geminiIssueList)
            {
                if (!checkSelected(issue.IssueKey, selectedItems))
                    continue;


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
