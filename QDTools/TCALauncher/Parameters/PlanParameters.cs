using PlanProcess;
using System.Collections.Generic;

namespace Parameters
{
    internal struct PlanParameters
    {
        public List<SinglePlanning> Plannings { get; set; }

        public string PostScript { get; set; }

        public int PlanningMode { get; set; }
    }
}
