using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Models
{
    internal class JiraProjectRelease
    {
        public JiraProjectRelease(
            string project, 
            string id, 
            string releaseName,
            DateTime releaseDate)
        {
            Project = project;
            Id = id;
            ReleaseName = releaseName;
            ReleaseDate = releaseDate;
        }

        public string Project { get; }

        public string Id { get; }

        public string ReleaseName { get; }

        public DateTime ReleaseDate { get; }


    }
}
