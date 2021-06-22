<#
Jira to excel implementation
Use SecureString.ps1 to create token secure string file
#>

param (
    [string]$Username = "pierluigi.nanni@prometeia.com",
    [string]$TokenFilePath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\JiraToken.txt",
    [string]$Jql = "project = MRM and issuetype = Bug and ""Epic Link"" = ESOA-1",
    [string]$server = "https://prometeia.atlassian.net/",
    [string]$ExcelFilePath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\MRMBugs.xlsx",
    [string]$sheetName = "Bugs"
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
    -Fields "project, key, status, summary"  `
    -Query $Jql | `
    Select-Object project, key,  status, summary

    $list | Export-XLSX -WorksheetName $sheetName -Path $ExcelFilePath -Table -Force -Autofit `
        -Header project, ID, Status, Summary

}
Catch {
    Write-Error "Processing failed  $_.Exception.Message"
    Break
}
