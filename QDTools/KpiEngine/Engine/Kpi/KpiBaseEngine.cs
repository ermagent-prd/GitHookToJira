using KpiEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KpiEngine.Engine
{
    internal abstract class KpiBaseEngine : IKpiEngine
    {
        public KpiOutput Execute(KpiInput input)
        {
            try
            {
                if (!checkEvaluation(input))
                    return null;

                return KernelExecute(input);
            }
            catch (Exception ex)
            {
                return new KpiOutput(
                    this.getKpiInfo(),
                    new ProcessResult(
                        ExecutionResult.Error,
                        ex.Message),
                    null);
            }

        }

        #region Private

        protected abstract KpiOutput KernelExecute(KpiInput input);

        protected abstract bool checkEvaluation(KpiInput input);

        protected abstract KpiInfo getKpiInfo();

        protected abstract IEnumerable<KpiKey> getKpiKeys(KpiInput input);

        protected abstract DateTime getReferenceDate(KpiInput input);

        protected KpiValue getKpiKeyValue(KpiInput input, double? value)
        {
            var kpiKeys = getKpiKeys(input);

            var refDate = getReferenceDate(input);

            var kpiInfo = getKpiInfo();

            return new KpiValue(
                input.JiraRelease.Project,
                refDate,
                getUniqueKey(kpiInfo, kpiKeys),
                kpiKeys,
                value);
        }

        protected string getUniqueKey(KpiInfo kpiInfo,IEnumerable<KpiKey> keys)
        {
            return kpiInfo.Key + "-" + String.Join("-", keys.Select(k => k.KeyValue));
        }
        #endregion
    }
}
