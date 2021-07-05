param (
    [string]$CfgPath = "C:\Projects\Others\Processtools\ErmasIssuesPublish\PublishCommands\JiraToConfluence\Cfg\JiraToConfluence.json",
    [string]$Jql = "project = ERMPAT and issuetype = Bug and ""Epic Link"" = ESOA-1 and status = Fixed",
    [string]$workingfolderPath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\",
    [string]$ExcelFile = "PATBugs.xlsx",
    [string]$sheetName = "Bugs",
    [string]$pageId = "32768480", # PageId (from edit page)
    [string]$fName = "PATBugs.xlsx", # File name to search in the attachment list 
    [string]$logFile = "PATBugs.log",
    [string]$newDataFile = "PATbugs.csv",
    [string]$oldDataFile = "PATbugsOld.csv"
 )

 $CfgData=Get-Content -Path $CfgPath | ConvertFrom-Json

 $scriptPath = $CfgData.engine.publishScript

 $logFilePath = $workingfolderPath + $logFile
 $ExcelFilePath = $workingfolderPath + $ExcelFile
 $newDataFilePath = $workingfolderPath + $newDataFile
 $oldDataFilePath = $workingfolderPath + $oldDataFile

 & $scriptPath `
    $CfgData.jira.userName `
    $CfgData.jira.tokenFilePath `
    $Jql `
    $CfgData.jira.server `
    $ExcelFilePath  `
    $sheetName  `
    $CfgData.confluence.url `
    $pageId `
    $fName `
    $CfgData.confluence.pswdPath `
    $logFilePath `
    $newDataFilePath `
    $oldDataFilePath



