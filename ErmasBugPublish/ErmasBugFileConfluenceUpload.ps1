param (
    [string]$ExcelFilePath = "C:\tmp\Bugs.xlsx",
    [string]$dbServer = "ERM-DB-05",
    [string]$dbName = "GEMINI_CLOUD",
    [string]$sheetName = "Ermas5 Bugs",
    [string]$rowCountFilePath = "c:\tmp\BugCount.txt",
    [string]$ConfluenceURL = "https://intranet.prometeia.it/rest/api/content/",
    [string]$pageId = "80338723", # PageId (from edit page)
    [string]$fName = "Bugs.xlsx", # File name to search in the attachment list 
    [string]$pswdPath = "H:\My Drive\Projects\SqlToExcel\ExportedPassword.txt"
 )

    import-module sqlserver

    $environmentQuery = "USE $($dbName) SELECT ID FROM V_BUG_LIST_ERM"
    
    $Data =  invoke-sqlcmd -query $environmentQuery -ServerInstance $dbServer 
    
    $RowCount = $Data.count
    
    Write-Host "New Row count: " $RowCount
    
    $oldRowNumber = "0";
    
    if (Test-Path -Path $rowCountFilePath)
    {
        $oldRowNumber = Get-Content -Path $rowCountFilePath
    }
    
    Write-Host "Old row count:" $oldRowNumber
    
    $OldRowNumberInt = [int]$oldRowNumber
    
    if ($RowCount -gt $OldRowNumberInt)
    {
        Try {
    
            & "$PSScriptRoot\ToExcel.ps1" -ExcelFilePath $ExcelFilePath -dbServer $dbServer -dbName $dbName -sheetName $sheetName

            & "$PSScriptRoot\UpdateConfluenceFile.ps1" -ConfluenceURL $ConfluenceURL -pageId $pageId -fName $fName -fPath $ExcelFilePath -pswdPath $pswdPath
    
            Set-Content -Path $rowCountFilePath -Value $RowCount
           
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


