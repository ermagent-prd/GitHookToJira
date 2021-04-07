param (
    [string]$ExcelFilePath = "E:\Projects\ErmasBugPublish\Work\ERMBugsWip.xlsx",
    [string]$dbServer = "ERM-DB-05",
    [string]$dbName = "GEMINI_CLOUD",
    [string]$sheetName = "Ermas5 WIP Bugs",
    [string]$rowCountFilePath = "E:\Projects\ErmasBugPublish\Work\BugWipCount.txt",
    [string]$ConfluenceURL = "https://intranet.prometeia.it/rest/api/content/",
    [string]$pageId = "30606277", # PageId (from edit page)
    [string]$fName = "ERMBugsWip.xlsx", # File name to search in the attachment list 
    [string]$pswdPath = "E:\Projects\ErmasBugPublish\Work\ExportedPassword.txt",
    [string]$logFilePath = "E:\Projects\ErmasBugPublish\Work\BugsWip.log",
    [string]$sqlQuery = "SELECT cast(ID as int) ID, [Bug fixing], Status, [Product Modules], Title, [Deliverable Version], [Fixed In Build], Description FROM V_BUG_WIP_ERM ORDER BY [Bug fixing] DESC",
    [string]$newDataFile = "E:\Projects\ErmasBugPublish\Work\bugsWip.csv",
    [string]$oldDataFile = "E:\Projects\ErmasBugPublish\Work\busWipOld.csv"
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
