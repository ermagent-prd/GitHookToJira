using KpiEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Engine
{
    internal interface IKpiEngine
    {
        KpiOutput Execute(KpiInput input);
    }
}
