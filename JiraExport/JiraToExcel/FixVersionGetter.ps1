param (
    [PSObject]$jiraItem
 )

 If($null -eq $jiraItem.fixVersions) 
    {
        return ""
    } 
 Else 
    {
        if($jiraItem.fixVersions.Length -gt 1)
        {
            return $jiraItem.fixVersions.ForEach('name') -join ','

        }
        else {
            return $jiraItem.fixVersions.name
        }
    }