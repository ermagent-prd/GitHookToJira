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
            var svc = new ServiceManagerFactory();

            var getter = new ItemListGetter(svc);

            var filter = new IssuesFilter
            {
                IncludeClosed = true,

                Projects = "38"
            };

            var issues = getter.Execute(Constants.GeminiUrl, filter);

            Assert.IsNotNull(issues);
        }
    }
}
