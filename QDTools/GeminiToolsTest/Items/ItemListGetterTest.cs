using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using GeminiTools.Container;
using GeminiTools.Engine;
using GeminiTools.Items;
using GeminiTools.Parameters;
using GeminiToolsTest.Container;
using GeminiToolsTest.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity;

namespace GeminiToolsTest.Items
{
    [TestClass]
    public class ItemListGetterTest
    {
        [TestMethod]
        public void Execute_ValidIssues_ReturnIssues()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var getter = container.Resolve<ItemListGetter>();

            var filter = new IssuesFilter
            {
                IncludeClosed = true,

                Projects = "38"
            };

            var issues = getter.Execute(filter);

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void Execute_LoadUatItem()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var getter = container.Resolve<ItemGetter>();

            var issue = getter.Execute(65514);

            Assert.IsNotNull(issue);
        }


        [TestMethod]
        public void Execute_LoadAttachmentIssue()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var getter = container.Resolve<ItemGetter>();


            var issue = getter.Execute(63705);

            List<IssueCommentDto> itemComments = issue.Comments;
            var commentAttach = itemComments[9].Attachments[0];

            var attachmentGetter = container.Resolve<AttachmentGetter>();

            attachmentGetter.FileDownload(
                commentAttach.Entity.ProjectId,
                commentAttach.Entity.IssueId,
                commentAttach.Entity.Id,
                commentAttach.Entity.Name,
                GeminiToolsTestConstants.GeminiUrl,
                GeminiToolsTestConstants.SAVING_PATH);

            Assert.IsTrue(File.Exists(GeminiToolsTestConstants.SAVING_PATH + commentAttach.Entity.Name));
        }

        [TestMethod]
        public void Execute_LoadAttachmentuatIssue()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var getter = container.Resolve<ItemGetter>();

            var issue = getter.Execute(65628);

            List<IssueCommentDto> itemComments = issue.Comments;
            var commentAttach = itemComments[0].Attachments.FirstOrDefault();

            var attachmentGetter = container.Resolve<AttachmentGetter>();

            if (commentAttach != null)
                attachmentGetter.FileDownload(
                    commentAttach.Entity.ProjectId,
                    commentAttach.Entity.IssueId,
                    commentAttach.Entity.Id,
                    commentAttach.Entity.Name,
                    GeminiToolsTestConstants.GeminiUrl,
                    GeminiToolsTestConstants.SAVING_PATH);

            Assert.IsTrue(File.Exists(GeminiToolsTestConstants.SAVING_PATH + commentAttach.Entity.Name));
        }

        //59844
        [TestMethod]
        public void Execute_LoadAnalysisDocumentUrl()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var getter = container.Resolve<ItemGetter>();
            var linkItemEngine = container.Resolve<LinkItemEngine>();

            var issue = getter.Execute(59844);

            var attachmentGetter = container.Resolve<AttachmentGetter>();

            var changesDoc = issue.CustomFields.FirstOrDefault(c => c.Name == "EDEVChangesDoc");

            attachmentGetter.Save(linkItemEngine.Execute(changesDoc.FormattedData), GeminiToolsTestConstants.SAVING_PATH);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Execute_LoadDevelopmentIssue()
        {
            string DEVELOPMENT_PROJECT_ID = "36";
            string DEVELOPMENT_TYPES = "|Developer|Task|";
            List<string> DEVELOPMENT_RELEASES = new List<string> {
                "ERMAS",
                "ERMAS 5.24.0",
                "ERMAS 5.24.1",
                "ERMAS 5.25.0",
                "ERMAS 5.26.0",
                "ERMAS 5.27.0",
                "ERMAS 5.28.0",
                "ERMAS 5.29.0",
                "0.0.0.0"
            };

            List<string> DEVELOPMENT_LINES = new List<string> {
                //"BSM",
                //"ILIAS",
                "ILIAS-STA"
                //"Other" 
            };

            string DEVELOPMENT_RELEASE_KEY = "Release Version";
            string DEVELOPMENT_LINE_KEY = "DVL";

            var filter = new IssuesFilter
            {
                IncludeClosed = true,
                Projects = DEVELOPMENT_PROJECT_ID,
                Types = DEVELOPMENT_TYPES,
            };

            var container = GeminiContainerFactory.Execute();

            var engine = container.Resolve<ItemListGetter>();

            var list = engine.Execute(filter);

            List<IssueDto> filteredList = new List<IssueDto>();

            foreach (var l in list.OrderBy(f => f.Id))
            {
                var release = l.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_RELEASE_KEY);
                var devLine = l.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_LINE_KEY);

                if (release != null && devLine != null &&
                    DEVELOPMENT_RELEASES.Contains(release.FormattedData) &&
                    DEVELOPMENT_LINES.Contains(devLine.FormattedData))
                    filteredList.Add(l);
            }

            Assert.IsTrue(filteredList.Any());

        }

        [TestMethod]
        public void Execute_LoadUatIssue()
        {
            string UAT_PROJECT_ID = "|37|";

            string dateFormat = "yyyy/MM/dd";

            var filter = new IssuesFilter
            {
                IncludeClosed = true,
                Projects = UAT_PROJECT_ID,
                CreatedAfter = DateTime.Today.AddDays(-7).ToString(dateFormat)
                //CreatedBefore = DateTime.Today.ToString(dateFormat)
            };

            var container = GeminiContainerFactory.Execute();

            var engine = container.Resolve<ItemListGetter>();

            var list = engine.Execute(filter);

            Assert.IsTrue(list.Any());

        }

    }
    }
