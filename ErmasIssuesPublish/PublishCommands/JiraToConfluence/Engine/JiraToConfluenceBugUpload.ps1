param (
    [string]$Username = "pierluigi.nanni@prometeia.com",
    [string]$TokenFilePath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\JiraToken.txt",
    [string]$Jql = "project = ERMAS and issuetype = Bug and ""Bug Category[Dropdown]"" = Post-release and status = Fixed",
    [string]$server = "https://prometeia.atlassian.net/",
    [string]$ExcelFilePath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\ERMBugs.xlsx",
    [string]$sheetName = "Bugs",
    [string]$ConfluenceURL = "https://intranet.prometeia.it/rest/api/content/",
    [string]$pageId = "30605372", # PageId (from edit page)
    [string]$fName = "ERMBugs.xlsx", # File name to search in the attachment list 
    [string]$ConfluencepswdPath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\ConfluencePassword2.txt",
    [string]$logFilePath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\ERMBugs.log",
    [string]$newDataFile = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\ERMbugs.csv",
    [string]$oldDataFile = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\ERMbugsOld.csv"
 )
    try {

        Import-Module PSExcel

        # Jira plugin install
        if (Get-Module -ListAvailable -Name "JiraPS") {
            Write-Host "JiraPS Module  exists"
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
    
        $jiraData = Get-JiraIssue `
        -Fields "project, key, customfield_10125, status, customfield_10173, summary, fixVersions, customfield_10124, description"  `
        -Query $Jql | `
        Select-Object project, key, customfield_10125, status, customfield_10173, summary, fixVersions, customfield_10124, description


        $Data = New-Object Collections.Generic.List[PSCustomObject]

        foreach ($jiraItem in $jiraData)
        {
            $Object = New-Object PSObject -Property @{
                project       = $jiraItem.project
                key             = $jiraItem.key
                bugFixing = $jiraItem.customfield_10125
                status = $jiraItem.Status 
                devLine = $jiraItem.customfield_10173.value
                summary = $jiraItem.Summary
                fixVersions = If($null -eq $jiraItem.fixVersions) {""} Else 
                    {

                        if($jiraItem.fixVersions.Length -gt 1)
                        {
                            $jiraItem.fixVersions.ForEach('name') -join ','

                        }
                        else {
                            $jiraItem.fixVersions.name
                        }
                        
                    }
                
                fixedInBuild = $jiraItem.customfield_10124
                description = $jiraItem.Description
            }            

            $Data.add($Object)
        }


        $Data | Export-Csv $newDataFile

        [bool]$ToCheck = -not (Test-Path -Path $oldDataFile) -or (compare-object (get-content $newDataFile) (get-content $oldDataFile))
        
        Write-Host "File to update:" $ToCheck

        if ($ToCheck)
        {
            Try {
                $Data | Select-Object Project, key, BugFixing, Status, devLine, summary, fixVersions, fixedInBuild, description | Export-XLSX -WorksheetName $sheetName -Path $ExcelFilePath -Table -Force -Autofit `
                -Header Project, ID, BugFixing, Status, DevLine, summary,  fixVersions, FixedInBuild, description

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
    Catch {
    
        Write-Host ("File update -- > failure " + $_.Exception.Message )

        exit 1
    }        
    finally {

        Stop-Transcript
    }
