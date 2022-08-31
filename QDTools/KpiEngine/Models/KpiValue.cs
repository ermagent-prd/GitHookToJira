using System;
using System.Collections.Generic;

namespace KpiEngine.Models
{
    internal class KpiValue
    {
        public KpiValue(DateTime referenceDate, IEnumerable<KpiKey> keys, double? value)
        {
            this.ReferenceDate = referenceDate;
            this.Keys = keys;
            this.Value = value;
        }

        public DateTime ReferenceDate { get; }

        public IEnumerable<KpiKey> Keys { get; }

        public Double? Value { get; }   

    }
}
