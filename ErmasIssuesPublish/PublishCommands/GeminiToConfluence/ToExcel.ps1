<#
    Export MSSQL query result to excel
    This script use PSExcel Library
    To install referedd libraries excute this commands before use
    
    Install-Module PSExcel
    install-module sqlserver
    update-module sqlserver
#>
param (
    [string]$ExcelFilePath = "E:\Projects\ErmasBugPublish\Work\Bugs.xlsx",
    [string]$dbServer = "ERM-DB-05",
    [string]$dbName = "GEMINI_CLOUD",
    [string]$sheetName = "Ermas5 Bugs",
    [string]$sqlQuery = "SELECT cast(ID as int) ID, [Bug fixing], Status, [Product Modules], Title, [Deliverable Version], [Fixed In Build], Description FROM V_BUG_LIST_ERM ORDER BY [Bug fixing] DESC"
 )
Import-Module PSExcel
import-module sqlserver

$environmentQuery = "USE $($dbName) " + $sqlQuery

invoke-sqlcmd -query $environmentQuery -ServerInstance $dbServer | 
    Select-Object * -ExcludeProperty  "RowError","RowState","Table","ItemArray","HasErrors" | 
    Export-XLSX -WorksheetName $sheetName -Path $ExcelFilePath -Table -Force -Autofit


