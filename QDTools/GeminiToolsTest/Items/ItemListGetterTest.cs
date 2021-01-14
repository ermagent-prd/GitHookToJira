using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using GeminiTools.Container;
using GeminiTools.Items;
using GeminiToolsTest.Container;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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
    }
}
