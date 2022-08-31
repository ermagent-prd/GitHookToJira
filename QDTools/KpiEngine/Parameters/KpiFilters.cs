using System;
using System.Collections.Generic;

namespace KpiEngine.Parameters
{
    public class KpiFilters
    {
        public DateTime MinReleaseDate { get; set; }
        public List<string> JiraProjects { get; set; }
    }
}
