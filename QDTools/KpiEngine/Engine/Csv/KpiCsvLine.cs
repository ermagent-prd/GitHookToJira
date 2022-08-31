using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Engine.Csv
{
    internal class KpiCsvLine
    {
        public string Kpi { get; set; }
        public string KpiDescription { get; set; }

        public DateTime ReferenceDate { get; set; }

        public string KpiKeys { get; set; }

        public string KpiResult { get; set; }

        public string KpiResultMessage { get; set; }

        public double? KpiValue { get; set; }
    }
}
