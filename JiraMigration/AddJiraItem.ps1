param (
    [string] $target = "https://prometeia-erm.atlassian.net",
    [string] $Username = "pierluigi.nanni@prometeia.com",
    [string] $token = 'GOojveJqyGUAgi6mzSAu3FAA',
    [string] $projectKey = "SKP",
    [string] $issueType = "Story",
    [string] $summary = "Api call",
    [string] $description = "Testing api call"
)


[string] $body = "{`"fields`":{`"project`":{`"key`":`"$projectKey`"},`"issuetype`":{`"name`":`"$issueType`"},`"summary`":`"$summary`",`"description`":`"$description`"}}";

try 
    {
        $basicAuth = "Basic " + [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes("$($Username):$token"))
        $headers = @{
        "Authorization" = $basicAuth
        "Content-Type" = "application/json"
        "verify" = "false"
    }
    $requestUri = "$target/rest/api/latest/issue"
    $response = Invoke-RestMethod -Uri $requestUri -Method POST -Headers $headers -Body $body
    Write-Output "ID: $($response.id)"
    Write-Output "Key: $($response.key)"
    Write-Output "Self: $($response.self)" 
}
catch {
    Write-Warning "Remote Server Response: $($_.Exception.Message)"
    Write-Output "Status Code: $($_.Exception.Response.StatusCode)"
}