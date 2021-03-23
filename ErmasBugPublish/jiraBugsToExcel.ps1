<#
Jira to excel implementation
Use SecureString.ps1 to create token secure string file
#>

param (
    [string]$Username = "pierluigi.nanni@prometeia.com",
    [string]$TokenFilePath = "C:\Projects\Others\Processtools\JiraExport\ExportedPassword.txt",
    [string]$Jql = "project = Ermas5 and issuetype = Bug and ""Epic Link"" = OA-28 and status = Fixed",
    [string]$server = "https://prometeia-erm.atlassian.net/",
    [string]$ExcelFilePath = "C:\Tmp\jIRA\JiraBugs.xlsx",
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
    -Fields "project, key, customfield_10052, status, summary"  `
    -Query $Jql | `
    Select-Object project, key, customfield_10052, status, summary

    $list | Export-XLSX -WorksheetName $sheetName -Path $ExcelFilePath -Table -Force -Autofit `
        -Header project, ID,  BugFixing, Status, Summary

}
Catch {
    Write-Error "Processing failed  $_.Exception.Message"
    Break
}
