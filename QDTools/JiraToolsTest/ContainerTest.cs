using JiraTools.Engine;
using JiraToolsTest.Container;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;


namespace JiraToolsTest
{
    [TestClass]
    public class ContainerTest
    {
        [TestMethod]
        public void Resolve_AddissueEngine_ReturnNotNullInstance()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<AddIssueEngine>();

            Assert.IsNotNull(engine);
        }
    }
}
