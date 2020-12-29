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
            var svcmng = new ServiceManagerFactory();

            var svc = svcmng.Execute(Constants.GeminiUrl);

            var getter = new ProjectListGetter(svc);

            var issues = getter.Execute();

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void GetProjectById_ValidUrl_ReturnProject()
        {
            var svcmng = new ServiceManagerFactory();

            var svc = svcmng.Execute(Constants.GeminiUrl);

            var getter = new ProjectGetter(svc);

            int projectId = 38;

            var issues = getter.Execute(projectId);

            Assert.IsNotNull(issues);
        }

        [TestMethod]
        public void GetProjectByCode_ValidUrl_ReturnProject()
        {
            var svcmng = new ServiceManagerFactory();

            var svc = svcmng.Execute(Constants.GeminiUrl);

            var projectGetter = new ProjectListGetter(svc);

            var getter = new ProjectFinder(projectGetter);

            var issues = getter.FindByCode(Constants.ErmBugProjectCode);

            Assert.IsNotNull(issues);
        }

    }
}
