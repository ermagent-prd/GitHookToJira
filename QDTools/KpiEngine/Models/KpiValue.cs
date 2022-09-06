using System;
using System.Collections.Generic;

namespace KpiEngine.Models
{
    internal class KpiValue
    {
        public KpiValue(
            string project,
            DateTime referenceDate, 
            string uniqueKey,
            IEnumerable<KpiKey> keys, double? value)
        {
            this.Project = project;
            this.ReferenceDate = referenceDate;
            this.UniqueKey = uniqueKey; 
            this.Keys = keys;
            this.Value = value;
        }

        public string Project { get; }

        public DateTime ReferenceDate { get; }

        public String UniqueKey { get; }

        public IEnumerable<KpiKey> Keys { get; }

        public Double? Value { get; }   

    }
}
