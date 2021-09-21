using System.Collections.Generic;

namespace TCALauncher.HistoryProcess
{
    internal class AggregationInfo
    {
        public string Caption { get; }
        public IEnumerable<string> IDs { get; }
        public int Value { get; }
        public int Total { get; }

        public AggregationInfo(string caption, IEnumerable<string> ids, int value, int total)
        {
            Caption = caption;
            IDs = ids;
            Value = value;
            Total = total;
        }
    }
}
