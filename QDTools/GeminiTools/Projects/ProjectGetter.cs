using Countersoft.Gemini.Commons.Dto;
using GeminiTools.Service;

namespace GeminiTools.Projects
{
    internal class ProjectGetter
    {

        private readonly ServiceManagerContainer svc;

        public ProjectGetter(ServiceManagerContainer svc)
        {
            this.svc = svc;
        }

        public ProjectDto Execute(int projectId)
        {
            return svc.Service.Projects.GetProject(projectId);
        }

    }
}
