using JiraTools.Engine;
using KpiEngine.Models;
using KpiEngine.Parameters;
using System.Collections.Generic;

namespace KpiEngine.Engine
{

    internal class JiraReleasesLoader : IJiraReleasesLoader
    {
        private readonly IKpiCoreParametersContainer kpiParameters;

        private readonly IProjectGetter projectGetter;

        private readonly IProjectReleasesGetter releaseGetter;

        public JiraReleasesLoader(
            IKpiCoreParametersContainer kpiParameters,
            IProjectGetter projectGetter,
            IProjectReleasesGetter releaseGetter)
        {
            this.kpiParameters = kpiParameters;
            this.projectGetter = projectGetter;
            this.releaseGetter = releaseGetter;
        }

        /// <summary>
        /// Gets released project releases using project filters
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JiraProjectRelease> Execute()
        {
            var result = new List<JiraProjectRelease>();

            var projects = this.projectGetter.Execute();

            foreach (var p in projects)
            {
                if (!this.kpiParameters.Parameters.Filters.JiraProjects.Contains(p.Key))
                    continue;

                var releases = this.releaseGetter.Execute(p.Key);

                foreach (var r in releases)
                {
                    if (r.ReleasedDate.HasValue && r.IsReleased)
                        result.Add(new JiraProjectRelease(
                            r.ProjectKey, 
                            r.Id, 
                            r.Name,
                            r.ReleasedDate.Value));
                }
            }

            return result;
        }
    }
}
