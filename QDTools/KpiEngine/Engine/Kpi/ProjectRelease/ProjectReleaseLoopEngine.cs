using KpiEngine.Models;
using System.Collections.Generic;

namespace KpiEngine.Engine
{
    internal class ProjectReleaseLoopEngine : IProjectReleaseLoopEngine
    {
        #region Private properties

        private readonly IJiraReleasesLoader relasesLoader;

        private readonly IReleasesKpiEnginesExecutor executor;

        #endregion

        #region constructor

        public ProjectReleaseLoopEngine(
            IJiraReleasesLoader relasesLoader,
            IReleasesKpiEnginesExecutor executor)
        {
            this.relasesLoader = relasesLoader;
            this.executor = executor;   
        }

        #endregion

        #region Public methods
        public IEnumerable<KpiOutput> Execute()
        {
            var result = new List<KpiOutput>(); 

            var releases = this.relasesLoader.Execute();

            foreach(var r in releases)
            {
                var releaseKpis = this.executor.Execute(r,false);
                
                result.AddRange(releaseKpis);

            }

            return result;
        }

        #endregion

    }
}
