using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvnTools;
using SvnToolsTest.Container;
using Unity;

namespace SvnToolsTest
{
    [TestClass]
    public class GetRevisionPropertiesTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<GetRevisionPropertiesEngine>();

            int revisionNumber = 5;

            var properties = engine.Execute(revisionNumber);

            Assert.IsNotNull(properties);
        }
    }
}
