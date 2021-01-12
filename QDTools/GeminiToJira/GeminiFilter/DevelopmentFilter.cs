using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiToJira.GeminiFilter
{
    public class DevelopmentFilter
    {
        readonly string DEVELOPMENT_PROJECT_ID = "36";  //developmente project
        readonly string DEVELOPMENT_TYPES = "|Developer|Task|";
        readonly List<string> DEVELOPMENT_RELEASES = new List<string> {
                "ERMAS",
                "ERMAS 5.24.0",
                "ERMAS 5.24.1",
                "ERMAS 5.25.0",
                "ERMAS 5.26.0",
                "ERMAS 5.27.0",
                "ERMAS 5.28.0",
                "ERMAS 5.29.0",
                "0.0.0.0"
            };

        readonly List<string> DEVELOPMENT_LINES = new List<string> {
                "BSM",
                "ILIAS",
                "ILIAS-STA",
                "Other" 
            };

        readonly string DEVELOPMENT_RELEASE_KEY = "Release Version";
        readonly string DEVELOPMENT_LINE_KEY = "DVL";

        public DevelopmentFilter()
        {
            // Method intentionally left empty.
        }

        public IssuesFilter GetDevelopmentFilter()
        {
            return new IssuesFilter
            {
                IncludeClosed = true,
                Projects = DEVELOPMENT_PROJECT_ID,
                Types = DEVELOPMENT_TYPES,
            };
        }

        public List<IssueDto> FilterIssuesList(List<IssueDto> list)
        {
            List<IssueDto> filteredList = new List<IssueDto>();

            foreach (var l in list.OrderBy(f => f.Id))
            {
                var release = l.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_RELEASE_KEY);
                var devLine = l.CustomFields.FirstOrDefault(x => x.Name == DEVELOPMENT_LINE_KEY);

                if (release != null && devLine != null &&
                    DEVELOPMENT_RELEASES.Contains(release.FormattedData) &&
                    DEVELOPMENT_LINES.Contains(devLine.FormattedData))
                    filteredList.Add(l);
            }

            return filteredList;
        }
    }
}
