using System;
using Countersoft.Gemini.Commons.Entity;
using GeminiTools;
using GeminiTools.Items;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeminiToolsTest
{
    [TestClass]
    public class ItemListGetterTest
    {
        [TestMethod]
        public void Execute_ValidIssues_ReturnIssues()
        {
            var svcmng = new ServiceManagerFactory();

            var svc = svcmng.Execute(Constants.GeminiUrl);

            var getter = new ItemListGetter(svc);

            var filter = new IssuesFilter
            {
                IncludeClosed = true,

                Projects = "38"
            };

            var issues = getter.Execute(filter);

            Assert.IsNotNull(issues);
        }
    }
}
