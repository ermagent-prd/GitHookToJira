using Countersoft.Gemini.Commons.Dto;
using GeminiToJira.GeminiFilter;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters;
using JiraTools.Engine;
using System.Collections.Generic;
using System.Linq;
using JiraTools.Parameters;
using System;
using Atlassian.Jira;
using System.IO;
using GeminiToJira.Parameters.Import;
using GeminiToJira.Log;

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

        public ImportBugEngine(
            BugIssueMapper geminiToJiraMapper,
            GeminiTools.Items.ItemListGetter geminiItemsEngine,
            JiraTools.Engine.ItemListGetter jiraItemsEngine,
            CreateIssueEngine jiraSaveEngine,
            JiraAccountIdEngine accountEngine,
            LinkEngine linkEngine,
            LogManager logManager,
            AffectedVersionsEngine affectedVEngine)
        {
            this.geminiToJiraMapper = geminiToJiraMapper;
            this.geminiItemsEngine = geminiItemsEngine;
            this.jiraItemsEngine = jiraItemsEngine;
            this.jiraSaveEngine = jiraSaveEngine;
            this.accountEngine = accountEngine;
            this.linkEngine = linkEngine;
            this.logManager = logManager;
            this.affectedVEngine = affectedVEngine;
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


                var geminiIssues = geminiBugIssueList.OrderBy(f => f.Id);

                foreach (var geminiIssue in geminiIssues)
                {
                    try
                    {
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

        private bool idRelevant(IssueDto geminiIssue, GeminiToJiraParameters configurationSetup)
        {
            //Product check
            var product = geminiIssue.CustomFields.FirstOrDefault(i => i.Name == "Product");
            if (product == null)
                return false;

            if (product.FormattedData != configurationSetup.Filter.BUG_PRODUCT)
                return false;

            //Affected versions check
            var affectedVersions = this.affectedVEngine.ExtractVersions(geminiIssue.AffectedVersionNumbers);
            if (!affectedVersions.Intersect(configurationSetup.Filter.BUG_RELEASES).Any())
                return false;

            return true;
        }

        private IEnumerable<IssueDto> getFiltered(GeminiToJiraParameters configurationSetup, IEnumerable<IssueDto> bugIssueList)
        {
            return bugIssueList
                .Where(i => idRelevant(i, configurationSetup))
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

        
        #endregion
    }


}
