<#
https://iwconnect.com/boosting-jira-reports-using-powershell/
#>

param (
    [string]$Names = "pierluigi.nanni@prometeia.com",
    [string]$project = "RMS5",
    [string]$server = "https://prometeia-erm.atlassian.net/",
    [DateTime]$Date = "2021/03/17",
    [int]$days = 7
) 
 
if (Get-Module -ListAvailable -Name "JiraPS") {
    Write-Host "JiraPS Module exists"
} 
else {
    Install-Module JiraPS -Scope CurrentUser
}

try {
    Set-JiraConfigServer $server;

    $cred = Get-Credential 

    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    New-JiraSession -Credential $cred 

    $worklogAuthor = $Names -split ','
    $startDate = $Date
    $endDate = $startDate.AddDays($days)

    $list = for ($i = $startDate; $i -lt $endDate; $i = $i.AddDays(1)) {
        $datestr = get-date $i -UFormat "%Y\u002f%m\u002f%d";
        $datestrFormatted = get-date $i -UFormat "%d.%m.%Y";
					
        foreach ($wa in $worklogAuthor) {
            Get-JiraIssue -Fields "key,summary,timespent,parent,worklog"  -Query "project = $project AND worklogAuthor = $wa AND (worklogDate =  $datestr )" -First 1000 | Select-Object -Property key, summary,
            @{
                Name       = 'Author'
                Expression = { $wa } 
            }, @{
                Name       = 'Minutes'
                Expression = { ($_.worklog.worklogs | Select-Object -Property timeSpentSeconds, started -ExpandProperty updateauthor |  Where-Object {$_.name -eq $wa -AND (get-date $_.started).Date -eq (get-date $i).Date   } | measure-object -Property  timeSpentSeconds  -sum).Sum / 60  } 
            }, @{
                Name       = 'parent'
                Expression = { if (  [string]::IsNullOrEmpty($_.parent.key) ) { $_.key } else { $_.parent.key } }
            } | 
                Group-Object -Property parent | 
                Select-Object -Property @{
                Name       = 'Key'
                Expression = {$_.Name}
            }, 
            @{
                Name       = 'Name'
                Expression = { Get-JiraIssue $_.Name | Select-Object -ExpandProperty summary }
            },
            @{
                Name       = 'Date'
                Expression = {$datestrFormatted}
            },
            @{
                Name       = 'Author'
                Expression = { $_.Group.Author | select-object -first 1 }
            },
            @{
                Name       = 'TimeSpent'
                Expression = { ($_.Group | measure-object -Property  Minutes -sum).Sum  }
            },
            @{
                Name       = 'TimeSpentHours'
                Expression = { ($_.Group | measure-object -Property  Minutes -sum).Sum / 60 }
            }
        } 
    } 
    $list | Export-Csv -NoTypeInformation -Path "data.csv"
}
Catch {
    Write-Error "Processing failed  $_.Exception.Message"
    Break
}