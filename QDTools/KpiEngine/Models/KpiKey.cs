using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Models
{
    internal class KpiKey
    {
        public KpiKey(string keyName, string keyValue)
        {
            KeyName = keyName;
            KeyValue = keyValue;
        }

        public string KeyName { get; }
        public string KeyValue { get; }    
    }
}
