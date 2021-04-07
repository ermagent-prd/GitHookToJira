param (
    [string]$ExcelFilePath = "E:\Projects\ErmasBugPublish\Work\ERMEsfBugsWip.xlsx",
    [string]$dbServer = "ERM-DB-05",
    [string]$dbName = "GEMINI_CLOUD",
    [string]$sheetName = "Ermas5 WIP Bugs",
    [string]$rowCountFilePath = "E:\Projects\ErmasBugPublish\Work\EsfBugWipCount.txt",
    [string]$ConfluenceURL = "https://intranet.prometeia.it/rest/api/content/",
    [string]$pageId = "32768482", # PageId (from edit page)
    [string]$fName = "ERMEsfBugsWip.xlsx", # File name to search in the attachment list 
    [string]$pswdPath = "E:\Projects\ErmasBugPublish\Work\ExportedPassword.txt",
    [string]$logFilePath = "E:\Projects\ErmasBugPublish\Work\EsfBugsWip.log",
    [string]$sqlQuery = "SELECT cast(ID as int) ID, [Bug fixing], Status, [Product Modules], Title, [Deliverable Version], [Fixed In Build], Description FROM V_BUG_WIP_ERM ORDER BY [Bug fixing] DESC",
    [string]$newDataFile = "E:\Projects\ErmasBugPublish\Work\EsfBugsWip.csv",
    [string]$oldDataFile = "E:\Projects\ErmasBugPublish\Work\EsfBugsWipOld.csv"
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
