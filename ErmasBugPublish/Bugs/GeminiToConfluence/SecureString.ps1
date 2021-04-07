$password = "Pippo"
$secureStringPwd = $password | ConvertTo-SecureString -AsPlainText -Force 
$secureStringText = $secureStringPwd | ConvertFrom-SecureString 
Set-Content "E:\Projects\ErmasBugPublish\Work\ExportedPassword.txt" $secureStringText