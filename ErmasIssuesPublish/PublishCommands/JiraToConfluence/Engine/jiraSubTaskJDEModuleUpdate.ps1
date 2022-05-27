<#
Jira Subtask JDE Module property update (from parent property value)
#>

param (
    [string]$Username = "pierluigi.nanni@prometeia.com",
    [string]$TokenFilePath = "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\JiraToken.txt",
    [string]$Jql = "project = ESQ and issuetype = story and ""JDE Module[Short text]"" IS NOT empty",
    [string]$server = "https://prometeia.atlassian.net/",
    [string]$ExcelFilePath = "C:\tmp\JDEModuleUpdate.xlsx",
    [string]$sheetName = "Log"
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
    -First 1000 `
    -Query $Jql | `
    Select-Object project, key,  status, summary

    foreach($story in $list) 
    {
        $st = Get-JiraIssue -Key $story.key  

        Write-Host "Story " $st.key "(" $st.customfield_10128 ")"
        
        foreach($sb in $st.subtasks)
        {
            $currSb = Get-JiraIssue -Key $sb.key  

            $fields = @{
                customfield_10128 = $st.customfield_10128 
            }

            $currSb | Set-JiraIssue -Fields $fields        
            
            Write-Host "    Sub-Task " $currSb.key "Updated"
        }
    }

    $list | Export-XLSX -WorksheetName $sheetName -Path $ExcelFilePath -Table -Force -Autofit `
        -Header project, Key, Status, Summary

}
Catch {
    Write-Error "Processing failed  $_.Exception.Message"
    Break
}
