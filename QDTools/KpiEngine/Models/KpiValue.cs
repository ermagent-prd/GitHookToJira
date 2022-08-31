using System;
using System.Collections.Generic;

namespace KpiEngine.Models
{
    internal class KpiValue
    {
        public KpiValue(
            string project,
            DateTime referenceDate, 
            IEnumerable<KpiKey> keys, double? value)
        {
            this.Project = project;
            this.ReferenceDate = referenceDate;
            this.Keys = keys;
            this.Value = value;
        }

        public string Project { get; }

        public DateTime ReferenceDate { get; }

        public IEnumerable<KpiKey> Keys { get; }

        public Double? Value { get; }   

    }
}
