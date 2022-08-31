using System;

namespace KpiEngine.ElkIndex
{
    //Elastic cserach kpi Document
    internal class KpiElkDocument
    {
        public string KpiCode { get; set; }
        public string KpiName { get; set; }
        public string Project { get; set; }
        public string KpiUniqueKey { get; set; }
        public string KpiDimensions { get; set; }
        public DateTime ReferenceDate { get; set; }
        public double KpiValue { get; set; }

    }
}
