param (
    [string]$projectFolder= "C:\Projects\Ermas5\AlmProSuite\Source\ALMProCommon\GeneralUtilities",
    [string]$projectFile = "Prometeia.ALMPro.GeneralUtilities.csproj",
    [string]$version = "1.0.1",
    [string]$suffix = "alpha",
    [string]$author = "Prometeia spa",
    [string]$Description = "general utilities",
    [string]$PackageDirectory = "C:\Projects\Others\NugetServer\PromNugetServer\PromNugetServer\Packages\"
 )

 Set-Location $projectFolder

 nuget pack $projectFile `
    -Symbols `
    -SymbolPackageFormat snupkg `
    -Version $version `
    -Suffix $suffix `
    -PackagesDirectory $PackageDirectory `
    -properties "author=$author;description=$Description" `
    -Force 



