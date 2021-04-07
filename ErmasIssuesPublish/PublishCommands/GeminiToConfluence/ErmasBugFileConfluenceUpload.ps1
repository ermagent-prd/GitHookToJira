param (
    [string]$ExcelFilePath = "E:\Projects\ErmasBugPublish\Work\Bugs.xlsx",
    [string]$dbServer = "ERM-DB-05",
    [string]$dbName = "GEMINI_CLOUD",
    [string]$sheetName = "Ermas5 Bugs",
    [string]$rowCountFilePath = "E:\Projects\ErmasBugPublish\Work\BugCount.txt",
    [string]$ConfluenceURL = "https://intranet.prometeia.it/rest/api/content/",
    [string]$pageId = "32768480", # PageId (from edit page)
    [string]$fName = "Bugs.xlsx", # File name to search in the attachment list 
    [string]$pswdPath = "E:\Projects\ErmasBugPublish\Work\ExportedPassword.txt",
    [string]$logFilePath = "E:\Projects\ErmasBugPublish\Work\Bugs.log",
    [string]$sqlQuery = "SELECT cast(ID as int) ID, [Bug fixing], Status, [Product Modules], Title, [Deliverable Version], [Fixed In Build], Description FROM V_BUG_LIST_ERM ORDER BY [Bug fixing] DESC",
    [string]$newDataFile = "E:\Projects\ErmasBugPublish\Work\bugs.csv",
    [string]$oldDataFile = "E:\Projects\ErmasBugPublish\Work\bugsOld.csv"
 )
    import-module sqlserver

    try {

        # Delete log files
        Get-ChildItem -Path $logFilePath -Include * | remove-Item -recurse

        Start-Transcript -path $logFilePath -append

        $environmentQuery = "USE $($dbName) " + $sqlQuery
    
        $Data =  invoke-sqlcmd -query $environmentQuery -ServerInstance $dbServer 

        $Data | Export-Csv $newDataFile

        [bool]$ToCheck = -not (Test-Path -Path $oldDataFile) -or (compare-object (get-content $newDataFile) (get-content $oldDataFile))
        
        Write-Host "File to update:" $ToCheck

        if ($ToCheck)
        {
            Try {
        
                & "$PSScriptRoot\ToExcel.ps1" -ExcelFilePath $ExcelFilePath -dbServer $dbServer -dbName $dbName -sheetName $sheetName -sqlQuery $sqlQuery
    
                & "$PSScriptRoot\UpdateConfluenceFile.ps1" -ConfluenceURL $ConfluenceURL -pageId $pageId -fName $fName -fPath $ExcelFilePath -pswdPath $pswdPath
        
                $Data | Export-Csv $oldDataFile -Force
            }
            
            Catch {
    
                Write-Host ("File update -- > failure " + $_.Exception.Message )
    
                exit 1
            }        
    
            Write-Host "File update done successfully"
    
            exit 0
        }
        else {
            Write-Host "File update skipped"
        }
            

    }
    finally {

        Stop-Transcript
    }
