using System.Collections.Generic;

namespace JiraReport.Parameters.Export
{
    public class ExcelConfiguration
    {
        public string JsqlQuery { get; set; }
        public string ReportSheetName { get; set; }
        public string PivotSheetName { get; set; }
        public string ExcelFileName { get; set; }
        public string ExcelTemplateFileName { get; set; }
        public string ExcelFilePath { get; set; }
        public string ExcelTemplateFilePath { get; set; }
        public List<string> FieldNames { get; set; }
        
    }
}
