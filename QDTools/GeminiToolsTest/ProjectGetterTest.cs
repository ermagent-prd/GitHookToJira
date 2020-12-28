using GeminiTools;
using GeminiTools.Projects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeminiToolsTest
{
    [TestClass]
    public class ProjectGetterTest
    {
        [TestMethod]
        public void GetProjectList_ValidUrl_ReturnProjects()
        {
            var svc = new ServiceManagerFactory();

            var getter = new ProjectListGetter(svc);

            var issues = getter.Execute(Constants.GeminiUrl);

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void GetProjectById_ValidUrl_ReturnProject()
        {
            var svc = new ServiceManagerFactory();

            var getter = new ProjectGetter(svc);

            int projectId = 38;

            var issues = getter.Execute(Constants.GeminiUrl,projectId);

            Assert.IsNotNull(issues);
        }

    }
}
