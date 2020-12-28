using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Countersoft.Gemini.Commons.Dto;

namespace GeminiTools.Projects
{
    internal class ProjectGetter
    {

        private readonly ServiceManagerFactory svcFactory;

        public ProjectGetter(ServiceManagerFactory svcFactory)
        {
            this.svcFactory = svcFactory;
        }

        public ProjectDto Execute(string geminiUrl, int projectId)
        {
            var svc = this.svcFactory.Execute(geminiUrl);

            return svc.Projects.GetProject(projectId);
        }

    }
}
