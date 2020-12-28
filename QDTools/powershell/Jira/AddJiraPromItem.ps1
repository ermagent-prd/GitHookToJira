param (
    [string] $target = "https://prometeia-erm.atlassian.net",
    [string] $Username = "pierluigi.nanni@prometeia.com",
    [string] $token = 'GOojveJqyGUAgi6mzSAu3FAA',
    [string] $projectKey = "ER",
    [string] $issueType = "Story",
    [string] $summary = "Api call",
    [string] $description = "Testing api call",
    [string] $completition = "10",
    $components = "",
    $dueDate =  "2020-12-04T20:22:53.138+0100", #Due date
    $epicLink = "ILIAS", #Epic Link
    $epicName = "", #Epic Name
    $estimate = "", #Original Estimate
    $issueKey = "ERM-59656", #GEMINI
    $jde = "", #JDE
    $logged = "", #Time Spent
    $owner = "", #OwnerTmp
    $priority = "", #Priority
    $release = "", #Fix Versions
    $resources = "", #ResourcesTmp
    $startDate = "",
    $status = ""#StatusTmp


)


[string] $body = "{`"fields`":{`"project`":{`"key`":`"$projectKey`"},`
    `"issuetype`":{`"name`":`"$issueType`"},`
    `"summary`":`"$summary`",`
    `"description`":`"$description`",`
    `"duedate`":`"$dueDate`",`
    `"customfield_10033`":{`"value`": `"$completition`"},`
    `"customfield_10036`":`"$issueKey`"
    }}";

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