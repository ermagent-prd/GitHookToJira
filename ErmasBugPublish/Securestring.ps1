$password = "pippo"
$secureStringPwd = $password | ConvertTo-SecureString -AsPlainText -Force 
$secureStringText = $secureStringPwd | ConvertFrom-SecureString 
Set-Content "H:\My Drive\Projects\SqlToExcel\ExportedPassword.txt" $secureStringText