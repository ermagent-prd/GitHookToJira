using Atlassian.Jira;
using JiraTools.Engine;
using KpiEngine.Models;
using KpiEngine.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;

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

                var releases = getFilteredReleases(
                    this.kpiParameters.Parameters.Filters.MinReleaseDate, 
                    p.Key);

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

        #region private methods

        private IEnumerable<ProjectVersion> getFilteredReleases(DateTime minReleaseDate, string projectKey)
        {
            var releases = this.releaseGetter.Execute(projectKey);
            return releases.Select(r => r).Where(r => r.IsReleased && r.ReleasedDate.HasValue && r.ReleasedDate >= minReleaseDate);
        }

        #endregion
    }
}
