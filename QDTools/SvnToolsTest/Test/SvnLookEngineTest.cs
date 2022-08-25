using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvnTools;
using SvnToolsTest.Container;
using Unity;

namespace SvnToolsTest
{
    [TestClass]
    public class SvnLookEngineTest
    {
        [TestMethod]
        public void SvnLook_Resolve()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<SvnLookEngine>();


            Assert.IsInstanceOfType(engine,typeof(SvnLookEngine));
        }
    }
}
