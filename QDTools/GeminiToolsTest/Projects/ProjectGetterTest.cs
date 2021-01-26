using GeminiTools.Parameters;
using GeminiTools.Projects;
using GeminiToolsTest.Container;
using GeminiToolsTest.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace GeminiToolsTest.Projects
{
    [TestClass]
    public class ProjectGetterTest
    {
        [TestMethod]
        public void GetProjectList_ValidUrl_ReturnProjects()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var getter = container.Resolve<ProjectListGetter>();

            var issues = getter.Execute();

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void GetProjectById_ValidUrl_ReturnProject()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var getter = container.Resolve<ProjectGetter>();

            int projectId = 38;

            var issues = getter.Execute(projectId);

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void GetProjectByCode_ValidUrl_ReturnProject()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var getter = container.Resolve<ProjectFinder>();

            var issues = getter.FindByCode(GeminiToolsTestConstants.ErmBugProjectCode);

            Assert.IsNotNull(issues);
        }

    }
}
