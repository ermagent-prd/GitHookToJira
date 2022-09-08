#Examples:
#			.\UpdateServerPackage.ps1 4003d786-cc37-4004-bfdf-c4f3e8ef9b3a http://10.102.2.27:93/nuget  c:\temp System.ComponentModel.TypeConverter 4.3.0 
#			.\UpdateServerPackage.ps1 4003d786-cc37-4004-bfdf-c4f3e8ef9b3a http://10.102.2.27:93/nuget  c:\temp 

param (
	[string]$cfgFile = "c:\temp\Nuget_Push.config",				#[MANDATORY] Configuration file
	[string]$TempDIR = "c:\temp\nuget", 								#[MANDATORY] Folder with packages
	[string]$PackageName = "Microsoft.CSharp",#Package to download with deps, if empty upload all packages in TempDir
	[string]$PackageVersion = "4.5.0" 						 	#Version to download
 )

if ($PackageName -ne "" -and $PackageVersion -ne "") 
{
	Write-Host "Download $PackageName version $PackageVersion in $TempDIR"
	nuget.exe install $PackageName -ConfigFile $cfgFile -Source "nuget.org" -Version $PackageVersion -OutputDirectory $TempDIR -DirectDownload -PackageSaveMode nupkg
}

Write-Host "Upload all packages in $TempDIR on destination repository"
Get-ChildItem -File -Path $TempDIR -Filter *.nupkg -Recurse -ErrorAction SilentlyContinue -Force | Foreach-Object {
	if ($_ -match "Prometeia.") {
		Write-Host  "    Skipped $_"
	}
	else {
		nuget push $_.fullname  -ConfigFile $cfgFile -src "PromNuget" -SkipDuplicate
	}
}