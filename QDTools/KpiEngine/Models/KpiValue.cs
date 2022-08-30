using System;
using System.Collections.Generic;

namespace KpiEngine.Models
{
    internal class KpiValue
    {
        public KpiValue(IEnumerable<KpiKey> keys, double? value)
        {
            Keys = keys;
            Value = value;
        }

        public IEnumerable<KpiKey> Keys { get; }

        public Double? Value { get; }   

    }
}
