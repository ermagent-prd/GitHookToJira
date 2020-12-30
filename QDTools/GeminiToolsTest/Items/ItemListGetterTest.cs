using Countersoft.Gemini.Commons.Entity;
using GeminiTools.Items;
using GeminiToolsTest.Container;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
