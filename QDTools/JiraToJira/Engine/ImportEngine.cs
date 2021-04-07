using Atlassian.Jira;
using JiraToJira.Container.Jira;
using JiraTools.Engine;
using JiraTools.Parameters;
using JiraTools.Service;
using System;
using System.IO;
using System.Linq;

namespace JiraToJira.Engine
{
    public class ImportEngine
    {
        private readonly CloneIssueEngine cloneIssueEngine;
        private readonly IssueLinkSearchEngine issueLinkearchEngine;
        private readonly LinkEngine linkEngine;

        //Original search engine
        private readonly ItemListGetter searchJiraOriginalItemsEngine;
        private readonly JqlGetter jqlOriginalEngine;

        //Destinantion search engine
        private readonly JqlGetter jqlDestItemsEngine;

        public ImportEngine(JqlGetter jqlEngine, 
            CloneIssueEngine cloneIssueEngine, 
            ItemListGetter jiraItemsEngine, 
            LinkEngine linkEngine, IssueLinkSearchEngine issueLinkearchEngine)
        {
            this.cloneIssueEngine = cloneIssueEngine;

            this.issueLinkearchEngine = issueLinkearchEngine;
            this.linkEngine = linkEngine;

            this.jqlOriginalEngine = jqlEngine;
            this.searchJiraOriginalItemsEngine = jiraItemsEngine;

            this.jqlDestItemsEngine = GetJiraDestJqlGetterEngine();
        }

        internal void Execute(string fromProjectCode, string destProjectCode, string fromProjectName, string destProjectName, string type)
        {
            var jqlSearch = "project = \"" + fromProjectName + "\" AND type = " + type + " ORDER BY key ASC";

            string outputFilePath = "C:\\GeminiPorting\\Log\\";

            var issueList = jqlOriginalEngine.Execute(jqlSearch);

            var datetime = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_");
            string mappingFileName = "Migration" + "_" + destProjectCode + "_" + type + "_" + datetime + ".txt";
            string exceptionFileName = "MigrationException" + "_" + destProjectCode + "_" + type + "_" + datetime + ".txt";

            var oFile = Path.Combine(outputFilePath, mappingFileName);
            var eFile = Path.Combine(outputFilePath, exceptionFileName);

            File.AppendAllText(oFile, jqlSearch + Environment.NewLine);
            File.AppendAllText(eFile, jqlSearch + Environment.NewLine);

            foreach (var issue in issueList)
            {
                Issue relatedDev = GetRelatedDevelopment(issue.ParentIssueKey, destProjectName, fromProjectCode);
                Issue relatedEpicDev = GetEpicRelatedDev(fromProjectCode, destProjectName, issue);

                var clonedIssue = cloneIssueEngine.Execute(issue, destProjectCode, relatedDev);

                if (clonedIssue == null)
                {
                    File.AppendAllText(oFile, "During " + type + " import failed to save " + issue.Key.Value + "because parent value is null" + Environment.NewLine);
                    continue;
                }

                if (relatedEpicDev != null)
                {
                    clonedIssue.CustomFields.Add("Epic Link", relatedEpicDev.Key.Value);
                    clonedIssue.SaveChanges();
                }

                ManageRelatedLinks(destProjectName, issue, clonedIssue, eFile);

                File.AppendAllText(oFile, issue.Key.Value + ";" + clonedIssue.Key.Value + Environment.NewLine);


            }
        }


        private void ManageRelatedLinks(string destProjectName, Issue issue, Issue clonedIssue, string eFile)
        {
            var linkItems = issueLinkearchEngine.Execute(issue);

            foreach (var item in linkItems)
            {
                var jsql = "project = \"" + destProjectName + "\" and summary ~ \"" + RemoveSpecialChar(item.InwardIssue.Summary) + "\" and type = " + item.InwardIssue.Type.Name + " ORDER BY key ASC";
                var links = jqlDestItemsEngine.Execute(jsql).ToList();

                if (links != null && links.Count > 0)
                {
                    foreach (var l in links)
                    {
                        if (clonedIssue.Key.Value != l.Key.ToString())
                            linkEngine.Execute(clonedIssue, l.Key.ToString(), "Relates");
                    }
                }
                else
                {
                    File.AppendAllText(eFile, "Impossibile trovare la corrispondente issue relativa a " + item.InwardIssue.Key + " nel progetto " + destProjectName + Environment.NewLine);
                }
            }
        }

        private Issue GetEpicRelatedDev(string fromProject, string destProjectName, Issue issue)
        {
            Issue relatedEpicDev = null;
            if (issue.ParentIssueKey == null || issue.ParentIssueKey == "")
            {
                var epicLink = issue.CustomFields.FirstOrDefault(x => x.Name == "Epic Link");
                if (epicLink != null && epicLink.Values[0] != "")
                {
                    relatedEpicDev = GetRelatedDevelopment(epicLink.Values[0], destProjectName, fromProject);
                }

            }

            return relatedEpicDev;
        }

        private JqlGetter GetJiraDestJqlGetterEngine()
        {
            var parContainer = new JiraToJiraParamContainer();
            var serviceManagerContainer = new ServiceManagerContainer(parContainer);
            return new JqlGetter(serviceManagerContainer, parContainer);
        }

        private Issue GetRelatedDevelopment(string parentIssueKey, string destProjectName, string fromProject)
        {
            if (parentIssueKey == "" || parentIssueKey == null)
                return null;

            Issue jiraDev = null;

            var jiraOriginalDevList = searchJiraOriginalItemsEngine.Execute(parentIssueKey, QuerableType.ByCode, fromProject).ToList();

            if(jiraOriginalDevList != null && jiraOriginalDevList.Count > 0)
            {
                var key = jiraOriginalDevList[0].Key.Value;
                var devType = jiraOriginalDevList[0].Type.Name;
                var summary = jiraOriginalDevList[0].Summary;

                var jsql = "project = \"" + destProjectName + "\" and summary ~ \"" + RemoveSpecialChar(summary) + "\" and type = " + devType + " ORDER BY key ASC";
                var issueList = jqlDestItemsEngine.Execute(jsql);

                if (issueList != null)
                {
                    foreach (var i in issueList)
                    {
                        var originalKey = i.CustomFields.First(x => x.Name == "OriginalKey").Values[0];

                        if (originalKey == key)
                            jiraDev = i;
                    }
                }
            }

            return jiraDev;
        }

        private string RemoveSpecialChar(string summary)
        {
            var search = summary.Replace("\"", "");
            search = search.Replace(" [", " ");
            search = search.Replace("[", "");
            search = search.Replace("] ", " ");
            search = search.Replace("]", "");
            search = search.Replace(" : ", " ");
            search = search.Replace(": ", " ");
            search = search.Replace(":", " ");            
            search = search.Replace(" , ", " ");
            search = search.Replace(", ", " ");
            search = search.Replace(",", " ");
            search = search.Replace(" + ", " ");
            search = search.Replace("+", " ");
            search = search.Replace(" - ", " ");

            return search;
        }
    }
}
