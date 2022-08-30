using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Models
{
    internal class KpiInfo
    {
        public KpiInfo(string key, string description)
        {
            Key = key;
            Description = description;
        }

        public string Key { get; }

        public string Description { get; }    
    }
}
