<#
Confluence attached excel file update
#>
param (
    [string]$ConfluenceURL = "https://intranet.prometeia.it/rest/api/content/",
    [string]$pageId = "80338723", # PageId (from edit page)
    [string]$fName = "Bugs.xlsx", # File name to search in the attachment list
    [string]$fPath = "C:\tmp\Bugs.xlsx",
    [string]$pswdPath = "H:\My Drive\Projects\SqlToExcel\ExportedPassword.txt"
)

$attchmentUriPath = "/child/attachment"

$uri = "$($ConfluenceURL)$($pageId)$($attchmentUriPath)?expand=body.storage,version,space,ancestors"

$username = "piern"
$pwdTxt = Get-Content $pswdPath
$securePwd = $pwdTxt | ConvertTo-SecureString 
$cred = New-Object System.Management.Automation.PSCredential -ArgumentList $username, $securePwd

$authToken = [System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes(($cred.UserName+":"+[System.Runtime.InteropServices.marshal]::PtrToStringAuto([System.Runtime.InteropServices.marshal]::SecureStringToBSTR($cred.Password)) )))

$Headers = @{'Authorization' = "Basic "+$authToken
            'X-Atlassian-Token' = 'nocheck'
}

$listOfAttachments = Invoke-WebRequest -Method GET -Headers $Headers -Uri $uri -UseBasicParsing  | ConvertFrom-Json

foreach($result in $listOfAttachments.results){
    if($result.title -eq $fName)
    {
        $attachmentId = $result.Id
        $attchmentIdUriPath = "/child/attachment/$attachmentId/data"
        $uri = "$($ConfluenceURL)$($pageId)$($attchmentIdUriPath)"

        $webClient = New-Object System.Net.WebClient;
        $webClient.Headers.Add('X-Atlassian-Token','no-check');
        $webClient.Headers.Add('Authorization',"Basic $authToken"); 
        $webClient.UploadFile($uri,$fPath); 
        
        break
    }
}