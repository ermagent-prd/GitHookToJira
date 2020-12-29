using System.Collections.Generic;
using Countersoft.Gemini.Api;
using Countersoft.Gemini.Commons.Dto;

namespace GeminiTools.Projects
{
    internal class ProjectListGetter
    {
        private readonly ServiceManager svc;

        public ProjectListGetter(ServiceManager svcFactory)
        {
            this.svc = svcFactory;
        }

        public IEnumerable<ProjectDto> Execute()
        {
            return svc.Projects.GetProjects();
        }
    }
}
