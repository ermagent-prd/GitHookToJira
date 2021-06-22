param (
    [string]$Username = "pierluigi.nanni@prometeia.com",
    [string]$TokenFilePath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\JiraToken.txt",
    [string]$Jql = "project = MRM and issuetype = Bug and ""Epic Link"" = ESOA-1",
    [string]$server = "https://prometeia.atlassian.net/",
    [string]$ExcelFilePath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\MRMBugs.xlsx",
    [string]$sheetName = "Bugs",
    [string]$ConfluenceURL = "https://intranet.prometeia.it/rest/api/content/",
    [string]$pageId = "32768480", # PageId (from edit page)
    [string]$fName = "MRMBugs.xlsx", # File name to search in the attachment list 
    [string]$ConfluencepswdPath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\ConfluencePassword2.txt",
    [string]$logFilePath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\MRMBugs.log",
    [string]$newDataFile = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\MRMbugs.csv",
    [string]$oldDataFile = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\MRMbugsOld.csv",
    [string]$FieldList = "project, key,  status, summary"
 )
    try {

        Import-Module PSExcel

        # Jira plugin install
        if (Get-Module -ListAvailable -Name "JiraPS") {
            Write-Host "JiraPS Module exists"
        } 
        else {
            Install-Module JiraPS -Scope CurrentUser
        }

        # Delete log files
        Get-ChildItem -Path $logFilePath -Include * | remove-Item -recurse

        Start-Transcript -path $logFilePath -append

        Set-JiraConfigServer $server;

        $TokenTxt = Get-Content $TokenFilePath
        $securePwd = $TokenTxt | ConvertTo-SecureString 
        $cred = New-Object System.Management.Automation.PSCredential -ArgumentList $username, $securePwd
        
        [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
        New-JiraSession -Credential $cred 

<#
        $Fields = Get-JiraField -Credential $cred 
        $Fields | Export-Csv "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\CustomFields.csv"
#>
    
        $Data = Get-JiraIssue `
        -Fields "project, key, customfield_10125, status, summary, fixVersions, description"  `
        -Query $Jql | `
        Select-Object project, key, customfield_10125, status, summary, fixVersions, description

        $Data | Export-Csv $newDataFile

        [bool]$ToCheck = -not (Test-Path -Path $oldDataFile) -or (compare-object (get-content $newDataFile) (get-content $oldDataFile))
        
        Write-Host "File to update:" $ToCheck

        if ($ToCheck)
        {
            Try {

                $Data | Export-XLSX -WorksheetName $sheetName -Path $ExcelFilePath -Table -Force -Autofit `
                -Header project, ID, BugFixing, Status, Summary, FixVersions, Description
                
                & "$PSScriptRoot\UpdateConfluenceFile.ps1" -ConfluenceURL $ConfluenceURL -pageId $pageId -fName $fName -fPath $ExcelFilePath -pswdPath $ConfluencepswdPath
        
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
