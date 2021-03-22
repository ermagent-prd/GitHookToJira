using JiraTools.Engine;
using System;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using Atlassian.Jira;
using System.Linq;
using JiraReport.Parameters.Export;

namespace JiraReport.Engine
{
    public class ExportEngine
    {
        private readonly ExportFieldsEngine fieldsEngine;
        private readonly JqlGetter jqlEngine;

        public ExportEngine(JqlGetter jqlEngine, ExportFieldsEngine fieldEngine)
        {
            this.fieldsEngine = fieldEngine;
            this.jqlEngine = jqlEngine;
        }

        public string Execute(ExcelConfiguration configurationSetup)
        {
            var issueList = jqlEngine.Execute(configurationSetup.JsqlQuery);

            var datetime = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_");

            string fileFullname = Path.Combine(configurationSetup.ExcelFilePath, configurationSetup.ExcelFileName + ".xlsx");

            if (File.Exists(fileFullname))
            {
                fileFullname = Path.Combine(configurationSetup.ExcelFilePath, configurationSetup.ExcelFileName+"_" + datetime + ".xlsx");
            }

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(fileFullname, SpreadsheetDocumentType.Workbook))
            {
                CreateExcelFile(document, issueList, configurationSetup);
                return fileFullname;
            }
        }

        private void CreateExcelFile(SpreadsheetDocument document, IEnumerable<Issue> issueList, ExcelConfiguration configurationSetup)
        {
            
            var workbookPart = document.AddWorkbookPart();

            document.WorkbookPart.Workbook = new Workbook();

            document.WorkbookPart.Workbook.Sheets = new Sheets();

            var sheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            sheetPart.Worksheet = new Worksheet(sheetData);

            Sheets sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = document.WorkbookPart.GetIdOfPart(sheetPart);

            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId =
                    sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = configurationSetup.SheetName};
            sheets.Append(sheet);

            Row headerRow = new Row();

            List<String> columns = new List<string>();
            foreach (var column in configurationSetup.FieldNames)
            {
                columns.Add(column);

                Cell cell = new Cell();
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue(column);
                headerRow.AppendChild(cell);
            }


            sheetData.AppendChild(headerRow);

            foreach (var issue in issueList)
            {
                Row newRow = new Row();
                foreach (var field in configurationSetup.FieldNames)
                {
                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(fieldsEngine.Execute(issue, field));
                    newRow.AppendChild(cell);
                }

                sheetData.AppendChild(newRow);
            }
        }
    }
}
