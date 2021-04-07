param (
    [string]$ExcelFilePath = "E:\Projects\ErmasBugPublish\Work\ERMBugs.xlsx",
    [string]$dbServer = "ERM-DB-05",
    [string]$dbName = "GEMINI_CLOUD",
    [string]$sheetName = "Ermas5 Bugs",
    [string]$rowCountFilePath = "E:\Projects\ErmasBugPublish\Work\BugBusinessCount.txt",
    [string]$ConfluenceURL = "https://intranet.prometeia.it/rest/api/content/",
    [string]$pageId = "30605372", # PageId (from edit page)
    [string]$fName = "ERMBugs.xlsx", # File name to search in the attachment list 
    [string]$pswdPath = "E:\Projects\ErmasBugPublish\Work\ExportedPassword.txt",
    [string]$logFilePath = "E:\Projects\ErmasBugPublish\Work\BusinessBugs.log",
    [string]$sqlQuery = "SELECT cast(ID as int) ID, [Bug fixing], Status, [Product Modules], Title, [Deliverable Version], [Fixed In Build], Description FROM V_BUG_LIST_ERM ORDER BY [Bug fixing] DESC",
    [string]$newDataFile = "E:\Projects\ErmasBugPublish\Work\busbugs.csv",
    [string]$oldDataFile = "E:\Projects\ErmasBugPublish\Work\busbugsOld.csv"
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
