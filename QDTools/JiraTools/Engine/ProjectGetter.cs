using System.Collections.Generic;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTools.Service;

namespace JiraTools.Engine
{
    public class ProjectGetter : IProjectGetter
    {
        #region Private properties

        private readonly ServiceManagerContainer requestFactory;

        #endregion

        #region Constructor

        public ProjectGetter(
            ServiceManagerContainer requestFactory)
        {
            this.requestFactory = requestFactory;
        }

        #endregion

        #region Public methods

        public Project Execute(string projectKey)
        {

            var task = GetProject(projectKey);

            task.Wait();

            return task.Result;
        }

        public IEnumerable<Project> Execute()
        {
            var task = GetProjects();

            task.Wait();

            return task.Result;
        }


        #endregion

        #region Private methods

        private async Task<Project> GetProject(string projectKey)
        {
            return await this.requestFactory.Service.Projects.GetProjectAsync(projectKey);
        }

        private async Task<IEnumerable<Project>> GetProjects()
        {
            return await this.requestFactory.Service.Projects.GetProjectsAsync();
        }


        #endregion
    }
}
