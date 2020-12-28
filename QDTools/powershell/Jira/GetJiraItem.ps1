param (
    [string] $target = "https://prometeia-erm.atlassian.net",
    [string] $Username = "pierluigi.nanni@prometeia.com",
    [string] $token = 'GOojveJqyGUAgi6mzSAu3FAA',
    [string] $issueKey = "ER-4364"
)

try 
    {
        $basicAuth = "Basic " + [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes("$($Username):$token"))
        $headers = @{
        "Authorization" = $basicAuth
        "Content-Type" = "application/json"
        "verify" = "false"
    }
    $requestUri = "$target/rest/agile/1.0/issue/$issueKey"
    $response = Invoke-RestMethod -Uri $requestUri -Method GET -Headers $headers 
    Write-Output "ID: $($response.id)"
    Write-Output "Key: $($response.key)"
    Write-Output "Self: $($response.self)" 
}
catch {
    Write-Warning "Remote Server Response: $($_.Exception.Message)"
    Write-Output "Status Code: $($_.Exception.Response.StatusCode)"
}