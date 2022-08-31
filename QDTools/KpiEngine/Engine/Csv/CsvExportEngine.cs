using CsvHelper;
using KpiEngine.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpiEngine.Engine.Csv
{
    internal class CsvExportEngine : ICsvExportEngine
    {
        public void Execute(
            string csvPath,
            IEnumerable<KpiOutput> kpiResult)
        {
            var kpiLines = getKpiLines(kpiResult);

            using (var writer = new StreamWriter(csvPath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(kpiLines);
                writer.Flush();
            }
        }

        private IEnumerable<KpiCsvLine> getKpiLines(IEnumerable<KpiOutput> lines)
        {
            var result = new List<KpiCsvLine>();

            foreach (var line in lines)
            {
                var csvLine = new KpiCsvLine
                {
                    Kpi = line.KpiInfo.Key,
                    KpiDescription = line.KpiInfo.Description,
                    KpiResult = line.ExecutionResult.Result.ToString(),
                    KpiResultMessage = line.ExecutionResult.Message,
                    KpiValue = line.KpiValue.Value,
                    KpiKeys = String.Join("#", line.KpiValue.Keys.Select(k => k.KeyValue))

                };

                result.Add(csvLine);
            }

            return result;
        }
    }
}
