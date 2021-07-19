param (
    [string]$Username = "pierluigi.nanni@prometeia.com",
    [string]$TokenFilePath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\JiraToken.txt",
    [string]$server = "https://prometeia.atlassian.net/",
    [string]$logFilePath = "C:\Projects\Others\Processtools\Bin\Tools\JiraWorkLogs\JiraWorkLog.log",
    [string]$JiraIssue = "ESIBT-520",
    [string]$Comment = "Test comment"
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
        #New-JiraSession -Credential $cred 

        #Add-JiraIssueWorklog -Credential $cred -Comment $Comment -Issue $JiraIssue -TimeSpent 60 -DateStarted (Get-Date)   


        Add-JiraIssueWorklog -Credential $cred -Comment $Comment -Issue $JiraIssue -TimeSpent "6" -DateStarted (Get-Date)
           

    }
    Catch {
    
        Write-Host ("File update -- > failure " + $_.Exception.Message )

        exit 1
    }        
    finally {

        Stop-Transcript
    }
