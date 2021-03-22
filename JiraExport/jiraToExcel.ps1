<#
Jira to excel implementation
Use SecureString.ps1 to create token secure string file
#>

param (
    [string]$Username = "pierluigi.nanni@prometeia.com",
    [string]$TokenFilePath = "C:\Projects\Others\Processtools\JiraExport\ExportedPassword.txt",
    [string]$Jql = "project = MOD",
    [string]$server = "https://prometeia-erm.atlassian.net/",
    [string]$ExcelFilePath = "C:\Tmp\jIRA\JiraIssues.xlsx",
    [string]$sheetName = "Jira Issues"
) 

Import-Module PSExcel
 
if (Get-Module -ListAvailable -Name "JiraPS") {
    Write-Host "JiraPS Module exists"
} 
else {
    Install-Module JiraPS -Scope CurrentUser
}

try {
    Set-JiraConfigServer $server;

    $TokenTxt = Get-Content $TokenFilePath
    $securePwd = $TokenTxt | ConvertTo-SecureString 
    $cred = New-Object System.Management.Automation.PSCredential -ArgumentList $username, $securePwd
    
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    New-JiraSession -Credential $cred 

    $list = Get-JiraIssue `
        -Fields "project, Epic, IssueType, key,summary,timespent,parent,worklog"  `
        -Query $Jql `
        -First 100 | Select-Object `
        -Property project, epic, issuetype, key, summary


    $list | Export-XLSX -WorksheetName $sheetName -Path $ExcelFilePath -Table -Force -Autofit

}
Catch {
    Write-Error "Processing failed  $_.Exception.Message"
    Break
}
