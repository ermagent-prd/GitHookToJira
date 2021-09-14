using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.Log;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters.Import;
using GeminiTools.Items;
using JiraTools.Engine;

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
        private readonly LogManager logManager;
        private readonly AffectedVersionsEngine affectedVEngine;
        private readonly ProjectVersionsGetter geminiProjectVersionGetter;
        private readonly JqlGetter jqlgetter;

        public ImportBugEngine(
            BugIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            JiraTools.Engine.ItemListGetter jiraItemsEngine,
            CreateIssueEngine jiraSaveEngine,
            JiraAccountIdEngine accountEngine,
            LinkEngine linkEngine,
            LogManager logManager,
            AffectedVersionsEngine affectedVEngine,
            ProjectVersionsGetter geminiProjectVersionGetter,
            JqlGetter jqlgetter)
        {
            this.geminiToJiraMapper = geminiToJiraMapper;
            this.geminiItemsEngine = geminiItemsEngine;
            this.jiraItemsEngine = jiraItemsEngine;
            this.jiraSaveEngine = jiraSaveEngine;
            this.accountEngine = accountEngine;
            this.linkEngine = linkEngine;
            this.logManager = logManager;
            this.affectedVEngine = affectedVEngine;
            this.geminiProjectVersionGetter = geminiProjectVersionGetter;
            this.jqlgetter = jqlgetter;
        }

        public void Execute(GeminiToJiraParameters configurationSetup)
        {
            try
            {
                var projectCode = configurationSetup.JiraProjectCode;

                Countersoft.Gemini.Commons.Entity.IssuesFilter filter = GetBugFilter(configurationSetup);

                var bugIssueList = GetFilteredGeminiIssueList(geminiItemsEngine, filter);

                var geminiBugIssueList = getFiltered(configurationSetup,bugIssueList);

                var bugLogFile = "BugLog_" + projectCode + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

                this.logManager.SetLogFile(configurationSetup.LogDirectory + bugLogFile);


                var originaKeys = getJiraBugsOriginalKeys(projectCode);



                var geminiIssues = geminiBugIssueList.OrderBy(f => f.Id);

                foreach (var geminiIssue in geminiIssues)
                {
                    try
                    {
                        if (originaKeys.Contains(geminiIssue.IssueKey))
                            continue;

                        var currentIssue = geminiItemsEngine.Execute(geminiIssue.Id);
            
                        var jiraIssueInfo = geminiToJiraMapper.Execute(
                            configurationSetup, 
                            currentIssue, 
                            configurationSetup.Jira.BugTypeCode, 
                            projectCode, 
                            configurationSetup.Gemini.ErmBugPrefix,
                            configurationSetup.Mapping.BUG_EPICLINK);

            
                        var jiraIssue = jiraSaveEngine.Execute(
                            jiraIssueInfo, 
                            configurationSetup.Jira, 
                            configurationSetup.AttachmentDownloadedPath);

                        SetAndSaveReporter(jiraIssue, geminiIssue,configurationSetup.Jira.DefaultAccount);

                        originaKeys.Add(geminiIssue.IssueKey);
                    }
                    catch(Exception e)
                    {
                        this.logManager.Execute(geminiIssue.IssueKey + e.Message);
                    }
            
                }

            }
            catch (Exception e)
            {
                this.logManager.Execute(e.Message);
            }
        }

        private HashSet<String> getJiraBugsOriginalKeys(string projectCode)
        {
            string jsql = $"Project = \"" + projectCode + "\" and (type = \"Bug\" and \"Bug Category[Dropdown]\" = Post-release and  \"OriginalKey[Short text]\" IS NOT empty)";

            var jiraBugs = this.jqlgetter.Execute(jsql);

            var keys = new HashSet<string>();

            foreach(var b in jiraBugs)
            {
                var oriKeyField = b.CustomFields.FirstOrDefault(f => f.Name == "OriginalKey");

                if (oriKeyField == null)
                    continue;

                var oriKey = oriKeyField.Values[0];

                if (!keys.Contains(oriKey))
                    keys.Add(oriKey);

            }

            return keys;

        }


        private bool isRelevant(IssueDto geminiIssue, GeminiToJiraParameters configurationSetup)
        {
            
            //sELECTED CHECK
            if (!checkSelected(geminiIssue, configurationSetup))
                return false;

            //Product check
            if (!checkProduct(geminiIssue, configurationSetup))
                return false;

            //Status check
            if (!checkStatus(geminiIssue, configurationSetup))
                return false;

            //Skipped Status check
            if (!checkSkippedStatus(geminiIssue, configurationSetup))
                return false;

            //Fixing Date Check
            if (!checkCreateDate(geminiIssue, configurationSetup))
                return false;

            //Affected versions check
            if (configurationSetup.Filter.BUG_RELEASES != null && configurationSetup.Filter.BUG_RELEASES.Any())
            {
                var affectedVersions = this.affectedVEngine.ExtractVersions(geminiIssue.AffectedVersionNumbers, null);
                if (affectedVersions.Any() && !affectedVersions.Intersect(configurationSetup.Filter.BUG_RELEASES).Any())
                    return false;
            }

            return true;
        }

        private bool checkProduct(IssueDto geminiIssue, GeminiToJiraParameters configurationSetup)
        {
            var product = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Product");
            if (product == null)
                return false;

            if (product.FormattedData != configurationSetup.Filter.BUG_PRODUCT)
                return false;

            return true;
        }

        private bool checkStatus(IssueDto geminiIssue, GeminiToJiraParameters configurationSetup)
        {
            if (configurationSetup.Filter?.BUG_STATUS == null || !configurationSetup.Filter.BUG_STATUS.Any())
                return true;

            return
              configurationSetup.Filter.BUG_STATUS.Contains(geminiIssue.Status);
        }

        private bool checkSelected(IssueDto geminiIssue, GeminiToJiraParameters configurationSetup)
        {
            if (configurationSetup.Filter?.BUG_SELECTED_ITEMS == null || !configurationSetup.Filter.BUG_SELECTED_ITEMS.Any())
                return true;

            return
              configurationSetup.Filter.BUG_SELECTED_ITEMS.Contains(geminiIssue.IssueKey);
        }


        private bool checkSkippedStatus(IssueDto geminiIssue, GeminiToJiraParameters configurationSetup)
        {
            if (configurationSetup.Filter?.BUG_SKIPPED_STATUS == null || !configurationSetup.Filter.BUG_SKIPPED_STATUS.Any())
                return true;

            return
              !configurationSetup.Filter.BUG_SKIPPED_STATUS.Contains(geminiIssue.Status);
        }


        private bool checkCreateDate(IssueDto geminiIssue, GeminiToJiraParameters configurationSetup)
        {

            DateTime createdDate = geminiIssue.Created.Date;

            //from
            if (!String.IsNullOrWhiteSpace(configurationSetup.Filter.BUG_CREATED_DATE_FROM))
            {
                var dateFrom = Convert.ToDateTime(configurationSetup.Filter.BUG_CREATED_DATE_FROM);

                if (createdDate < dateFrom)
                    return false;
            }

            //TO

            if (!String.IsNullOrWhiteSpace(configurationSetup.Filter.BUG_CREATED_DATE_TO))
            {
                var dateTo = Convert.ToDateTime(configurationSetup.Filter.BUG_CREATED_DATE_TO);

                if (createdDate > dateTo)
                    return false;
            }


            return true;
        }

        private IEnumerable<IssueDto> getFiltered(GeminiToJiraParameters configurationSetup, IEnumerable<IssueDto> bugIssueList)
        {
            return bugIssueList
                .Where(i => isRelevant(i, configurationSetup))
                .Select(i => i);
        }

        #region Private 

        private Countersoft.Gemini.Commons.Entity.IssuesFilter GetBugFilter(GeminiToJiraParameters configurationSetup)
        {
            return new Countersoft.Gemini.Commons.Entity.IssuesFilter()
            {
                IncludeClosed = configurationSetup.Filter.ERMBUG_INCLUDED_CLOSED,
                Projects = configurationSetup.Filter.ERMBUG_PROJECT_ID
            };
        }


        private IEnumerable<IssueDto> GetFilteredGeminiIssueList(
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            Countersoft.Gemini.Commons.Entity.IssuesFilter filter)
        {
            return geminiItemsEngine.Execute(filter);
        }

        private void SetAndSaveReporter(Issue jiraIssue, IssueDto geminiIssue,string defaultAccount)
        {
            if (geminiIssue.Reporter != "")
            {
                jiraIssue.Reporter = accountEngine.Execute(geminiIssue.Reporter, defaultAccount).AccountId;
                jiraIssue.SaveChanges();
            }
        }

        private IEnumerable<string> getGeminiVersions(GeminiToJiraParameters configurationSetup)
        {
            var versions = this.geminiProjectVersionGetter.Execute(Convert.ToInt32(configurationSetup.Filter.ERMBUG_PROJECT_ID));

            return versions
                .Where(v => v.Entity.Name.Contains(configurationSetup.Filter.BUG_PRODUCT))
                .Select(v => v.Entity.Name);
        }


        #endregion
    }


}
