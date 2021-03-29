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

            SpreadsheetDocument document;

            var template = configurationSetup.ExcelTemplateFilePath + configurationSetup.ExcelTemplateFileName + ".xlsx";
            if (File.Exists(template))
            {
                document = SpreadsheetDocument.CreateFromTemplate(template);

                UpdateExcelFile(document, issueList, configurationSetup);

                SetRefreshOnLoad(document);

                document.SaveAs(fileFullname).Close();

                return fileFullname;
            }
            else
            {
                document = SpreadsheetDocument.Create(fileFullname, SpreadsheetDocumentType.Workbook);
                CreateExcelFile(document, issueList, configurationSetup);
                return fileFullname;
            }
        }

        

        private void UpdateExcelFile(SpreadsheetDocument document, IEnumerable<Issue> issueList, ExcelConfiguration configurationSetup)
        {
            WorksheetPart worksheetPart =
                      GetWorksheetPartByName(document, configurationSetup.ReportSheetName);

            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            Row headerRow = new Row();
                        
            foreach (var column in configurationSetup.FieldNames)
            {
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

            // Save the worksheet.
            worksheetPart.Worksheet.Save();
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

            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = configurationSetup.ReportSheetName};
            sheets.Append(sheet);

            Row headerRow = new Row();

            foreach (var column in configurationSetup.FieldNames)
            {
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

        private WorksheetPart GetWorksheetPartByName(
            SpreadsheetDocument document,
            string sheetName)
        {
            IEnumerable<Sheet> sheets =
               document.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>().Where(s => s.Name == sheetName);

            if (!sheets.Any())
                return null;

            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)
                 document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;
        }

        private void SetRefreshOnLoad(SpreadsheetDocument document)
        {
            var uriPartDictionary = BuildUriPartDictionary(document);

            PivotTableCacheDefinitionPart pivotTableCacheDefinitionPart1 = (PivotTableCacheDefinitionPart)uriPartDictionary["/xl/pivotCache/pivotCacheDefinition11.xml"];
            PivotCacheDefinition pivotCacheDefinition1 = pivotTableCacheDefinitionPart1.PivotCacheDefinition;
            pivotCacheDefinition1.RefreshOnLoad = true;
        }

        protected Dictionary<String, OpenXmlPart> BuildUriPartDictionary(SpreadsheetDocument document)
        {
            var uriPartDictionary = new Dictionary<String, OpenXmlPart>();
            var queue = new Queue<OpenXmlPartContainer>();
            queue.Enqueue(document);
            while (queue.Count > 0)
            {
                foreach (var part in queue.Dequeue().Parts.Where(part => !uriPartDictionary.Keys.Contains(part.OpenXmlPart.Uri.ToString())))
                {
                    uriPartDictionary.Add(part.OpenXmlPart.Uri.ToString(), part.OpenXmlPart);
                    queue.Enqueue(part.OpenXmlPart);
                }
            }
            return uriPartDictionary;
        }
    }
}
