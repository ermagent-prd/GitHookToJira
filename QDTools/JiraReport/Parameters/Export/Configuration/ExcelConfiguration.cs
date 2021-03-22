using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReport.Parameters.Export
{
    public class ExcelConfiguration
    {
        public string JsqlQuery { get; set; }
        public string SheetName { get; set; }
        public string ExcelFileName { get; set; }
        public string ExcelFilePath { get; set; }
        public List<string> FieldNames { get; set; }
        
    }
}
