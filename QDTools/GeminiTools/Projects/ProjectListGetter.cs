using System.Collections.Generic;
using Countersoft.Gemini.Commons.Dto;
using GeminiTools.Service;

namespace GeminiTools.Projects
{
    internal class ProjectListGetter
    {
        private readonly ServiceManagerContainer svc;

        public ProjectListGetter(ServiceManagerContainer svcFactory)
        {
            this.svc = svcFactory;
        }

        public IEnumerable<ProjectDto> Execute()
        {
            return svc.Service.Projects.GetProjects();
        }
    }
}
