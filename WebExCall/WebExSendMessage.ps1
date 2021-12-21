param (
    [string]$roomID= "Y2lzY29zcGFyazovL3VzL1JPT00vYmI5M2I4NTAtNDE4Yy0xMWVjLTlhNGMtZmZhNGIyMzRlNDUx",
    [string]$Token= "MzNjNTNjMTMtZDNmYy00YmY5LTliZWEtYjMyYzc0YjE4OGExZTk3ZmVjNWItMDEx_PF84_57c940c3-f7e0-494f-b0d0-8eae6e4e8708",
    [string]$MessageText = "This is a test"
 )

#This is definitely required. When I left it off, stuff didn't work.
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

#secure token of Webex bot acct
$Ciscobottoken = $Token
#roomID of destination in Webex Teams
$body = @{
roomId=$roomID
text=$MessageText
}
$json=$body | ConvertTo-Json
Invoke-RestMethod -Method Post `
-Headers @{"Authorization"="Bearer $Ciscobottoken"} `
-ContentType "application/json" -Body $json `
-Uri "https://api.ciscospark.com/v1/messages"