param (
    [string]$CfgPath = "C:\Projects\Others\Processtools\ErmasIssuesPublish\PublishCommands\JiraToConfluence\Cfg\JiraToConfluence.json",
    [string]$Jql = "project = ERMAS and issuetype = Bug and ""Bug Category[Dropdown]"" = Post-release and status = Fixed",
    [string]$workingfolderPath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\",
    [string]$ExcelFile = "ERMBugs.xlsx",
    [string]$sheetName = "Bugs",
    [string]$pageId = "32768480", # PageId (from edit page)
    [string]$fName = "ERMBugs.xlsx", # File name to search in the attachment list 
    [string]$logFilePath = "ErmBugs.log",
    [string]$newDataFile = "ErmBugs.csv",
    [string]$oldDataFile = "ErmBugsOld.csv"
 )

 $CfgData=Get-Content -Path $CfgPath | ConvertFrom-Json

 $logFilePath = $workingfolderPath + $logFile
 $ExcelFilePath = $workingfolderPath + $ExcelFile
 $newDataFilePath = $workingfolderPath + $newDataFile
 $oldDataFilePath = $workingfolderPath + $oldDataFile

 $scriptPath = $CfgData.engine.publishScript

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



