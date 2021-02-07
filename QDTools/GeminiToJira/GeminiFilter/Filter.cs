using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using System;
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
                        //Issues = "|59673|",  //TODO da cancellare     - ILiAS 3 Single ope
                        //Issues = "|60466|",  //TODO da cancellare       - H - XBRL FP "Sylos based" + Controlli
                        //Issues = "|59844|",  //TODO ERM-59844           - C. Refactoring Synth M.
                        Issues = "|61087|",  //TODO ERM-59844           ERM-61087 I - RFF Mediobanca Bank-IT
                        //TODO ?? AffectedVersions = |TRunk|Ermas x.xx....|
                    };
                case FilterType.UAT:
                    return new IssuesFilter
                    {
                        IncludeClosed = true,
                        Projects = UatConstants.UAT_PROJECT_ID,
                        //Issues = "|63715|"  //TODO da eliminare, il filtro va fatto meglio
                        Issues = "|62157|"  //TODO da eliminare, il filtro va fatto meglio
                        //Types = UatConstants.UAT_TYPES,
                    };
                case FilterType.ERMBUG:
                    return new IssuesFilter
                    {
                        IncludeClosed = true,
                        Projects = ErmBugConstants.ERMBUG_PROJECT_ID,
                        //Issues = "|65194|"  //TODO da eliminare, il filtro va fatto meglio
                        Issues = "|63783|"  //TODO da eliminare, il filtro 63783 Margin
                        //Types = UatConstants.UAT_TYPES,
                    };
                default:
                    return new IssuesFilter();
            }
            
        }

        public static IEnumerable<IssueDto> FilterDevIssuesList(IEnumerable<IssueDto> list)
        {
            List<IssueDto> filteredList = new List<IssueDto>();

            foreach (var l in list.OrderBy(f => f.Id))
            {
                var release = l.CustomFields.FirstOrDefault(x => x.Name == DevelopmentConstants.DEVELOPMENT_RELEASE_KEY);
                var devLine = l.CustomFields.FirstOrDefault(x => x.Name == DevelopmentConstants.DEVELOPMENT_LINE_KEY);

                if (l.Type != "Group" && release != null && devLine != null &&
                    DevelopmentConstants.DEVELOPMENT_RELEASES.Contains(release.FormattedData) &&
                    DevelopmentConstants.DEVELOPMENT_LINES.Contains(devLine.FormattedData))
                    filteredList.Add(l);
            }

            return filteredList;
        }

        public static IEnumerable<IssueDto> FilterUatIssuesList(IEnumerable<IssueDto> list, DateTime fromDate)
        {
            List<IssueDto> filteredList = new List<IssueDto>();

            foreach (var l in list.OrderBy(f => f.Id))
            {
                if (l.Created > fromDate)
                    filteredList.Add(l);
            }

            return filteredList;
        }


        #region Private
        

        #endregion
    }
}
