using JiraTools.Engine;
using JiraToolsTest.Container;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace JiraToolsTest
{
    [TestClass]
    public class AddWatcherTest
    {

        [TestMethod]
        public void Execute_AddWatcher()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<AddWatcherEngine>();

            string issueKey = "ESIBT-433";

            string author = "pierluigi.nanni@prometeia.com";

            engine.Execute(issueKey, author);

        }
       
    }
}
