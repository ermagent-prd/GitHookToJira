param (
    [string]$projectFolder= "C:\Projects\Ermas5\AlmProSuite\Source\ALMProCommon\CommonPatterns",
    [string]$projectFile = "Prometeia.ALMPro.CommonPatterns.csproj",
    [string]$version = "1.0.0",
    [string]$suffix = "beta",
    [string]$author = "piern",
    [string]$Description = "SHL Common Patterns",
    [string]$OutputDirectory = "C:\Projects\Others\NugetWork\",
    [string]$PackageFile = "C:\Projects\Others\NugetWork\Prometeia.ALMPro.CommonPatterns.1.0.0-beta.nupkg",
    [string]$ConfigFile = "C:\Projects\Others\NugetWork\nuget.config",
    [string]$Token = "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",
    [string]$PackageRepo = "github",
    [string]$Assemblypath = "C:\Projects\Ermas5\AlmProSuite\Bin\Common\Release\Prometeia.ALMPro.CommonPatterns.dll"

 )

 Set-Location $projectFolder

 
 nuget pack $projectFile `
    -ConfigFile $ConfigFile `
    -OutputDirectory $OutputDirectory `
    -Version $version `
    -Suffix $suffix `
    -Force `
    -properties "author=$author;description=$Description" 



<#
Set-Location $PackageDirectory

nuget push $PackageFile `
  -Source $PackageRepo

#>   


