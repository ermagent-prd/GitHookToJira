using System.Collections.Generic;

namespace KpiEngine.Parameters
{
    public class KpiCoreParametersContainer : IKpiCoreParametersContainer
    {
        public KpiCoreParametersContainer(KpiCoreParameters param)
        {
            this.Parameters = param;
        }

        public KpiCoreParameters Parameters { get; }


    }
}
