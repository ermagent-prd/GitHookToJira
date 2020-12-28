param (
    [string]$ExcelFilePath = "H:\Il mio Drive\Jira\Import from Gemini\ESF_PROGRESS ForJira_SecondImport.xlsx",
    [string]$SheetName = "ForJira",
    [int]$startingDataRow = 2,
    [string] $target = "https://prometeia-erm.atlassian.net",
    [string] $Username = "pierluigi.nanni@prometeia.com",
    [string] $token = 'GOojveJqyGUAgi6mzSAu3FAA'    
 )

 Import-Module PSExcel

 $Excel = New-Excel -Path $ExcelFilePath

 $Workbook = $Excel | Get-Workbook

 $Worksheet = $Workbook | Get-Worksheet -Name $SheetName

 

 for ($rowIndex = $startingDataRow; $rowIndex -le 2 <#$Worksheet.Dimension.Rows#>; $rowIndex++)
 {
    $issueType =  $Worksheet.Cells.Item($rowIndex,2).Value

    if ([string]::IsNullOrEmpty($issueType)) { continue }

    $description = $Worksheet.Cells.Item($rowIndex,8).Value #Description

    $completition = $Worksheet.Cells.Item($rowIndex,18).Value #Completition
    $components = $Worksheet.Cells.Item($rowIndex,14).Value #Components
    $dueDate =  Get-apiDate -inputDate $Worksheet.Cells.Item($rowIndex,17).Value
    $epicLink = $Worksheet.Cells.Item($rowIndex,4).Value #Epic Link
    $epicName = $Worksheet.Cells.Item($rowIndex,5).Value #Epic Name
    $estimate = $Worksheet.Cells.Item($rowIndex,19).Value #Original Estimate
    $issueKey = $Worksheet.Cells.Item($rowIndex,1).Value #GEMINI
    $jde = $Worksheet.Cells.Item($rowIndex,13).Value #JDE
    $logged = $Worksheet.Cells.Item($rowIndex,20).Value #Time Spent
    $owner = $Worksheet.Cells.Item($rowIndex,21).Value #OwnerTmp
    $priority = $Worksheet.Cells.Item($rowIndex,6).Value #Priority
    $release = $Worksheet.Cells.Item($rowIndex,15).Value #Fix Versions
    $resources = $Worksheet.Cells.Item($rowIndex,22).Value #ResourcesTmp
    $startDate = Get-apiDate -inputDate $Worksheet.Cells.Item($rowIndex,16).Value
    $status = $Worksheet.Cells.Item($rowIndex,2).Value #StatusTmp
    $title = $Worksheet.Cells.Item($rowIndex,7).Value #Summary

    & "$PSScriptRoot\AddJiraItem.ps1" -target $target -username $Username -token $token -projectKey "SKP" -issuetype $issueType -summary $title -description $description
 }

 
 Function Get-apiDate{

    Param ($inputDate)

    Write-Output  [DateTime]::FromOADate($inputDate).ToUniversalTime().ToString( "yyyy-MM-ddTHH:mm:ss.fffffffZ" ) 
 }
