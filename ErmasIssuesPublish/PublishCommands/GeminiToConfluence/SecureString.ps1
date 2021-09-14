$password = "password"
$secureStringPwd = $password | ConvertTo-SecureString -AsPlainText -Force 
$secureStringText = $secureStringPwd | ConvertFrom-SecureString 
Set-Content "C:\Projects\Others\Processtools\Bin\BugExport\FromJira\ConfluencePassword2.txt" $secureStringText