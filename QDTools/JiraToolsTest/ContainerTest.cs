using System;
using JiraTools.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;


namespace JiraToolsTest
{
    [TestClass]
    public class ContainerTest
    {
        [TestMethod]
        public void GetContainerTest()
        {
            var container = ContainerFactory.Execute();

            Assert.IsNotNull(container);
        }
    }
}
