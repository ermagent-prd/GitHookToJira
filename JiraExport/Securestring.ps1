$password = "pippo"
$secureStringPwd = $password | ConvertTo-SecureString -AsPlainText -Force 
$secureStringText = $secureStringPwd | ConvertFrom-SecureString 
Set-Content "C:\Projects\Others\Processtools\JiraExport\ExportedPassword.txt" $secureStringText