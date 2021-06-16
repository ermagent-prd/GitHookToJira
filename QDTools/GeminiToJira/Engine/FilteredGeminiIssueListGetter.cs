using System;
using System.Collections.Generic;
using System.Linq;
using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using GeminiToJira.Parameters.Import;
using GeminiTools.Items;

namespace GeminiToJira.Engine
{
    public class FilteredGeminiIssueListGetter
    {
        #region Private properties

        private readonly ItemListGetter geminiItemsEngine;

        private readonly GeminiIssueChecker checker;

        #endregion

        #region Constructor

        public FilteredGeminiIssueListGetter(
            ItemListGetter geminiItemsEngine,
            GeminiIssueChecker checker)
        {
            this.geminiItemsEngine = geminiItemsEngine;

            this.checker = checker;
        }


        #endregion

        #region Public methods

        public IEnumerable<IssueDto> Execute(GeminiToJiraParameters configurationSetup)
        {
            var filter = GetDevFilter(configurationSetup);

            return GetFilteredGeminiIssueList(filter, configurationSetup);

        }

        #endregion

        #region Private methods

        private IssuesFilter GetDevFilter(GeminiToJiraParameters configurationSetup)
        {
            return new IssuesFilter
            {
                IncludeClosed = configurationSetup.Filter.STORY_INCLUDED_CLOSED,
                Projects = configurationSetup.Filter.STORY_PROJECT_ID,
                Types = configurationSetup.Filter.STORY_TYPES,
            };
        }

        private IEnumerable<IssueDto> GetFilteredGeminiIssueList(
            Countersoft.Gemini.Commons.Entity.IssuesFilter filter,
            GeminiToJiraParameters configurationSetup)
        {
            var geminiIssueList = geminiItemsEngine.Execute(filter);

            List<IssueDto> filteredList = new List<IssueDto>();

            foreach (var l in geminiIssueList.OrderBy(f => f.Id))
            {
                if (this.checker.Execute(l, configurationSetup))
                    filteredList.Add(l);
            }

            return filteredList;
        }


        #endregion
    }
}
