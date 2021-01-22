using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using System.Collections.Generic;
using System.Linq;


namespace GeminiToJira.GeminiFilter
{
    public static class Filter
    {
        public static IssuesFilter GetFilter(FilterType fType)
        {
            switch(fType)
            {
                case FilterType.Development:
                    return new IssuesFilter
                    {
                        IncludeClosed = true,
                        Projects = DevelopmentConstants.DEVELOPMENT_PROJECT_ID,
                        Types = DevelopmentConstants.DEVELOPMENT_TYPES,
                        //TODO ?? AffectedVersions = |TRunk|Ermas x.xx....|
                    };
                case FilterType.UAT:
                    return new IssuesFilter
                    {
                        IncludeClosed = true,
                        Projects = UatConstants.UAT_PROJECT_ID,
                        Issues = "|65628|"  //TODO da eliminare, il filtro va fatto meglio
                        //Types = UatConstants.UAT_TYPES,
                    };
                default:
                    return new IssuesFilter();
            }
            
        }

        public static IEnumerable<IssueDto> FilterIssuesList(FilterType fType, IEnumerable<IssueDto> list)
        {
            switch (fType)
            {
                case FilterType.Development:
                    return filterDevelopmentIssuesList(list);
                default:
                    return list;
            }
        }


        #region Private
        private static List<IssueDto> filterDevelopmentIssuesList(IEnumerable<IssueDto> list)
        {
            List<IssueDto> filteredList = new List<IssueDto>();

            foreach (var l in list.OrderBy(f => f.Id))
            {
                var release = l.CustomFields.FirstOrDefault(x => x.Name == DevelopmentConstants.DEVELOPMENT_RELEASE_KEY);
                var devLine = l.CustomFields.FirstOrDefault(x => x.Name == DevelopmentConstants.DEVELOPMENT_LINE_KEY);

                if (release != null && devLine != null &&
                    DevelopmentConstants.DEVELOPMENT_RELEASES.Contains(release.FormattedData) &&
                    DevelopmentConstants.DEVELOPMENT_LINES.Contains(devLine.FormattedData))
                    filteredList.Add(l);
            }

            return filteredList;
        }

        #endregion
    }
}
