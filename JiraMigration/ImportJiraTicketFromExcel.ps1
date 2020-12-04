param (
    [string]$ExcelFilePath = "H:\Il mio Drive\Jira\Import from Gemini\ESF_PROGRESS ForJira_SecondImport.xlsx",
    [string]$SheetName = "For Jira No Groups",
    [int]$startingDataRow = 2,
    [string] $target = "https://prometeia-erm.atlassian.net",
    [string] $Username = "pierluigi.nanni@prometeia.com",
    [string] $token = 'GOojveJqyGUAgi6mzSAu3FAA'    
 )

 Import-Module PSExcel

 $Excel = New-Excel -Path $ExcelFilePath

 $Workbook = $Excel | Get-Workbook

 $Worksheet = $Workbook | Get-Worksheet -Name $SheetName

 for ($rowIndex = $startingDataRow; $rowIndex -le 5 <#$Worksheet.Dimension.Rows#>; $rowIndex++)
 {
    $summ = $Worksheet.Cells.Item($rowIndex,7).Value #Title

    $desc = $Worksheet.Cells.Item($rowIndex,8).Value

    & "$PSScriptRoot\AddJiraItem.ps1" -target $target -username $Username -token $token -projectKey "SKP" -issuetype "Story" -summary $summ -description $desc
 }

 
  
