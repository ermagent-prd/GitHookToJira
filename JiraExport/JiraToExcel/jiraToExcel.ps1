param (
    [string]$Username = "pierluigi.nanni@prometeia.com",
    [string]$TokenFilePath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\JiraToken.txt",
    [string]$Jql = "project = MRM and issuetype = Bug and ""Epic Link"" = ESOA-1 and status = Fixed",
    [string]$server = "https://prometeia.atlassian.net/",
    [string]$ExcelFilePath = "C:\Projects\Others\Processtools\Bin\JiraExport\MRMBugs.xlsx",
    [string]$sheetName = "Bugs",
    [string]$logFilePath = "C:\Projects\Others\Processtools\Bin\JiraExport\MRMBugs.log"
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

   
        $jiraData = Get-JiraIssue `
        -Fields "project, key, customfield_10125, status, summary, fixVersions, customfield_10124, description"  `
        -Query $Jql | `
        Select-Object project, key, customfield_10125, status, summary, fixVersions, customfield_10124, description

        $Data = New-Object Collections.Generic.List[PSCustomObject]

        foreach ($jiraItem in $jiraData)
        {
            $Object = New-Object PSObject -Property @{
                project       = $jiraItem.project
                key             = $jiraItem.key
                bugFixing = $jiraItem.customfield_10125
                status = $jiraItem.Status 
                summary = $jiraItem.Summary
                fixVersions = & "$PSScriptRoot\FixVersionGetter.ps1" -jiraItem $jiraItem
                fixedInBuild = $jiraItem.customfield_10124
                description = $jiraItem.Description
            }            

            $Data.add($Object)
        }


        Try {
            $Data | Select-Object Project, key, BugFixing, Status, summary, fixVersions, fixedInBuild, description | Export-XLSX -WorksheetName $sheetName -Path $ExcelFilePath -Table -Force -Autofit `
            -Header Project, ID, BugFixing, Status, summary, fixVersions, FixedInBuild, description
        }
        
        Catch {

            Write-Host ("File update -- > failure " + $_.Exception.Message )

            exit 1
        }        

    }
    Catch {
    
        Write-Host ("File update -- > failure " + $_.Exception.Message )

        exit 1
    }        
    finally {

        Stop-Transcript
    }
