param (
    [string] $target = "https://prometeia-erm.atlassian.net",
    [string] $Username = "pierluigi.nanni@prometeia.com",
    [string] $token = 'GOojveJqyGUAgi6mzSAu3FAA',
    [string] $projectKey = "ER"
)

try 
    {
        $basicAuth = "Basic " + [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes("$($Username):$token"))
        $headers = @{
        "Authorization" = $basicAuth
        "Accept" = "application/json"
    }
    $requestUri = "$target/rest/api/3/issue/createmeta?projectKeys=$projectKey"
   
    
    $response = Invoke-RestMethod -Uri $requestUri -Method GET -Headers $headers
    Write-Output "ID: $($response.id)"
    Write-Output "Key: $($response.key)"
    Write-Output "Self: $($response.self)" 
    Write-Output "Issue types: $($response.projects[0].issuetypes.Count)"
    
}
catch {
    Write-Warning "Remote Server Response: $($_.Exception.Message)"
    Write-Output "Status Code: $($_.Exception.Response.StatusCode)"
}