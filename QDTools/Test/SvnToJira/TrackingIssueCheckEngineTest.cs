using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvnToJira.Engine;
using SvnToJiraTest;
using Unity;

namespace SvnToolsTest
{
    [TestClass]
    public class TrackingIssueCheckEngineTest
    {
        [TestMethod]
        public void SvnLook_Resolve()
        {
            var container = SvnToJiraContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<TrackingIssueCheckEngine>();


            Assert.IsInstanceOfType(engine,typeof(TrackingIssueCheckEngine));
        }
    }
}
