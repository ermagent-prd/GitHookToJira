using System.Collections.Generic;
using Countersoft.Gemini.Commons.Dto;

namespace GeminiTools.Projects
{
    internal class ProjectListGetter
    {
        private readonly ServiceManagerFactory svcFactory;

        public ProjectListGetter(ServiceManagerFactory svcFactory)
        {
            this.svcFactory = svcFactory;
        }

        public IEnumerable<ProjectDto> Execute(string geminiUrl)
        {
            var svc = this.svcFactory.Execute(geminiUrl);

            return svc.Projects.GetProjects();
        }
    }
}
