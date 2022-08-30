using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Models
{
    internal class JiraProjectRelease
    {
        public JiraProjectRelease(string project, string release, DateTime releaseDate)
        {
            Project = project;
            Release = release;
            ReleaseDate = releaseDate;
        }

        public string Project { get; }

        public string Release { get; }

        public DateTime ReleaseDate { get; }


    }
}
