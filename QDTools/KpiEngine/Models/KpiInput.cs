using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Models
{
    internal class KpiInput
    {
        public KpiInput(
            JiraProjectRelease jiraRelease)
        {
            JiraRelease = jiraRelease;
        }

        public JiraProjectRelease JiraRelease { get; }

    }
}
