using System;
using System.Collections.Generic;
using System.Linq;
using Countersoft.Gemini.Commons.Dto;

namespace GeminiTools.Projects
{
    internal class ProjectFinder
    {
        #region Private properties

        private readonly ProjectListGetter projectGetter;

        private readonly Lazy<Dictionary<string, ProjectDto>> projDict;

        #endregion

        #region Constructor
        public ProjectFinder(ProjectListGetter projectGetter)
        {
            this.projectGetter = projectGetter;

            this.projDict = getDict();
        }

        #endregion

        #region Public methods
        public ProjectDto FindByCode(string code)
        {
            if (this.projDict.Value.ContainsKey(code))
                return projDict.Value[code];

            return null;
        }

        #endregion

        #region Private methods

        private Lazy<Dictionary<string, ProjectDto>> getDict()
        {
            return new Lazy<Dictionary<string, ProjectDto>>(
                () => 
                {
                    var projects = this.projectGetter.Execute();

                    return projects.ToDictionary(p => p.Entity.Code); 
                }
           );

        }

        #endregion


    }
}
