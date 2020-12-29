using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;

namespace GeminiTools.Projects
{
    internal class ProjectGetter
    {

        private readonly ServiceManager svc;

        public ProjectGetter(ServiceManager svc)
        {
            this.svc = svc;
        }

        public ProjectDto Execute(int projectId)
        {
            return svc.Projects.GetProject(projectId);
        }

    }
}
