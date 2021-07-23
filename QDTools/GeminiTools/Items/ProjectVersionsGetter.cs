using System.Collections.Generic;
using Countersoft.Gemini.Commons.Dto;
using GeminiTools.Service;

namespace GeminiTools.Items
{
    public class ProjectVersionsGetter
    {
        private readonly ServiceManagerContainer svc;

        public ProjectVersionsGetter(ServiceManagerContainer svcFactory)
        {
            this.svc = svcFactory;
        }

        public IEnumerable<VersionDto> Execute(int projectId)
        {
            return svc.Service.Projects.GetVersions(projectId);
        }
    }
}
