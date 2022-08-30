using Atlassian.Jira;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JiraTools.Engine
{
    public class ProjectReleasesGetter : IProjectReleasesGetter
    {
        #region Private properties

        private readonly ProjectGetter projectGetter;

        #endregion

        #region Constructor

        public ProjectReleasesGetter(
            ProjectGetter projectGetter)
        {

            this.projectGetter = projectGetter;
        }

        #endregion

        #region Public methods

        public IEnumerable<ProjectVersion> Execute(string projectKey)
        {

            var task = GetProjectReleases(projectKey);

            task.Wait();

            return task.Result;
        }



        #endregion

        #region Private methods

        private async Task<IEnumerable<ProjectVersion>> GetProjectReleases(string projectKey)
        {
            var project = this.projectGetter.Execute(projectKey);

            if (project == null)
                return null;

            return await project.GetVersionsAsync();
        }

        #endregion
    }
}
