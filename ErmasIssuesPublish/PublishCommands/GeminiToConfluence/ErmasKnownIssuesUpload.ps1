param (
    [string]$ExcelFilePath = "E:\Projects\ErmasBugPublish\Work\ERMASKnownIssues.xlsx",
    [string]$dbServer = "ERM-DB-05",
    [string]$dbName = "GEMINI_CLOUD",
    [string]$sheetName = "Ermas5 Known Issues",
    [string]$rowCountFilePath = "E:\Projects\ErmasBugPublish\Work\KnownIssuesCount.txt",
    [string]$ConfluenceURL = "https://intranet.prometeia.it/rest/api/content/",
    [string]$pageId = "85161704", # PageId (from edit page)
    [string]$fName = "ERMASKnownIssues.xlsx", # File name to search in the attachment list 
    [string]$pswdPath = "E:\Projects\ErmasBugPublish\Work\ExportedPassword.txt",
    [string]$logFilePath = "E:\Projects\ErmasBugPublish\Work\ErmasKnownIssues.log",
    [string]$sqlQuery = "SELECT cast(ID as int) ID, [Issue Type],  [Product Modules], [Specific Version] ,Title, Description, Workaround  from V_CONFLUENCE_KNOWN_ISSUES where product = 'ERMAS'",
    [string]$newDataFile = "E:\Projects\ErmasBugPublish\Work\ErmasKnownIssues.csv",
    [string]$oldDataFile = "E:\Projects\ErmasBugPublish\Work\ErmasKnownIssuesOld.csv"
 )
    & "$PSScriptRoot\ErmasBugFileConfluenceUpload.ps1" `
        -ExcelFilePath $ExcelFilePath `
        -dbServer $dbServer `
        -dbName $dbName `
        -sheetName $sheetName `
        -rowCountFilePath $rowCountFilePath `
        -ConfluenceURL $ConfluenceURL `
        -pageId $pageId `
        -fName $fName `
        -pswdPath $pswdPath `
        -logFilePath $logFilePath `
        -sqlQuery $sqlQuery `
        -newDataFile $newDataFile `
        -oldDataFile $oldDataFile `
